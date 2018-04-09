using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TimeSpanParserUtil // TimeSpanParserUtil.TimeParser.Tokens
{
    class ColonedToken : ParserToken {

        //public bool coloned; // use colonedColumns -- TODO: make a separate subclass
        public bool negativeColoned; // started with a negative sign, not included in numbers
        //public bool zeroOnly; // == IsZero()
        public bool startsWithColon;

        Units Autounit = Units.None; // e.g. if ColonedDefault = Units.Minutes, parse "05:10:30" as "05h10m30s" rather than failing
        Units SplitUnits = Units.None; // if we had to split the day column into days.hours, then SplitUnits will be Units.Days.

        public bool firstColumnContainsDot;  //Note: specifically '.' and NOT the regional decimal separator
        public decimal? firstColumnRightHalf = null;

        // units apply to the first column, i.e. colonedColumns[0]
        // a null value means an empty column, e.g. 10::30
        // if [0] == null then number started with a colon. 
        // [0] may be later separated into days.hours at the decimal point.
        public decimal?[] colonedColumns;
        decimal?[] startingColonRemovedColumns; // same as above but the first entry removed because it was empty, e.g. [null]:30
        decimal?[] splitColonedColumns; // same as above but days.hours have been split if needed
        bool calcDone = false;

        public override bool UsesColonedDefault() {
            return true;
        }

        public override bool IsNull() {
            return colonedColumns == null || colonedColumns.All(c => c == null);
        }

        public override bool IsZero() {
            if (IsNull()) return false; // technically not zero

            return colonedColumns.Any(c => c == 0) && colonedColumns.All(c => c == 0 || c == null);
        }

        private bool IsFirstColNegative() {
            var cols = Columns();
            if (cols.Length >= 1 && cols[0].HasValue && cols[0].Value < 0)
                return true;

            return false;

        }
        public override bool IsInitialNegative() {
            return (negativeColoned || IsFirstColNegative());
        }

        protected decimal?[] Columns() {
            CalcSplitDays();

            return startingColonRemovedColumns ?? splitColonedColumns ?? colonedColumns;

        }
        public override TimeSpan? ToTimeSpan() {
            if (IsNull())
                return null;

            var columns = Columns();
            if (columns == null)
                return null;

            Units units = BestGuessUnits();
            bool flip = IsFirstColNegative() && !negativeColoned; // inverse all numbers, not just the first. But if negativeColoned, flip whole thing at the end instead.

            bool first = true;
            TimeSpan sum;
            foreach (var c in columns) {
                if (c != null) {
                    //TODO: error if (!units.IsTimeUnit()) and not ZeroOnly etc ?

                    if (!first && flip) {
                        sum += GetValue(-c, units).Value;
                    } else {
                        sum += GetValue(c, units).Value;
                    }
                }

                units++;
                first = false;

            }
            if (negativeColoned) sum = -sum;
            return sum;

        }


        protected bool ShouldSplitDaysHours() {
            //TODO future: throw new FormatException("Multiple dots. Don't know where to cut days and hour."); // e.g. "1.2.3" (1.2 days + 3 hours, or 1 day, 2.3 hours) // may require a special custom token or something / guessing rules... but these wouldn't be found by the initial regex anyway

            //TODO: maybe should also split OneUnitToken. e.g. 1.7 days ? maybe require another option

            var expected = GivenOrDefaultOrZeroUnits();
            return (options.AllowDotSeparatedDayHours && firstColumnContainsDot && firstColumnRightHalf != null && colonedColumns[0].HasValue) // though presumably it has a value because firstColumnContainedPeriod
                && (expected == Units.Days
                 || ((colonedColumns.Length == 2 | colonedColumns.Length == 3) 
                    && (expected == Units.Hours || expected == Units.None)));
        }

        protected bool ShouldIgnoreStartingColon() {
            return options.IgnoreStartingColon && colonedColumns.Length >= 2 && colonedColumns[0] == null;
        }

        protected void CalcSplitDays() {
            if (calcDone)
                return;

            if (ShouldIgnoreStartingColon()) {
                //not truly ignoring it. //TODO
                startingColonRemovedColumns = colonedColumns.Skip(1).ToArray();

                Autounit = AutoUnits();

            } else if (ShouldSplitDaysHours()) {
                List<decimal?> splitDays = new List<decimal?>();
                var daysHours = colonedColumns[0].Value;
                var days = decimal.Truncate(colonedColumns[0].Value);
                var hours = firstColumnRightHalf; // daysHours - days * Math.Pow(10, places));
                splitDays.Add(days);
                splitDays.Add(hours);
                splitDays.AddRange(colonedColumns.Skip(1));
                splitColonedColumns = splitDays.ToArray();

                Autounit = Units.Days;
                calcDone = true;
                return; 

            }

            Autounit = AutoUnits();
            calcDone = true;

        }

        protected Units AutoUnits() {
            if (!options.AutoUnitsIfTooManyColons) {
                return Units.None;
            }

            if (splitColonedColumns != null)
                return Units.Days;

            var columns = startingColonRemovedColumns ?? colonedColumns;
            var parts = columns.Length;
            var probableUnits = GivenOrDefaultOrZeroUnits();

            //TODO: check for bad units
            if (parts == 4 && probableUnits != Units.Days) { // partUnit >= Units.Hours && partUnit < Units.Milliseconds || partUnit == Units.None
                // unit too small, auto adjust
                return Units.Days; // largest unit we'll AutoAdjust to (i.e. Don't do weeks unless explicit)

            } else if (parts == 3 && (probableUnits == Units.Minutes || probableUnits == Units.Seconds)) {
                // unit too small, auto adjust
                return Units.Hours;

            } else if (parts == 2 && probableUnits == Units.Seconds) {
                // unit too small, auto adjust
                return Units.Minutes;
            }

            return Units.None;
        }

        public override Units BestGuessUnits() { // GivenOrDefaultOrSplitUnits

            CalcSplitDays();

            if (Autounit != Units.None)
                return Autounit;

            if (SplitUnits != Units.None)
                return SplitUnits;

            return GivenOrDefaultOrZeroUnits();
        }

        protected override Units SmallestUnit() {
            var start = BestGuessUnits();

            if (start == Units.None) {
                if (IsZero())
                    return Units.ZeroOnly;
                return Units.None;
            }

            if (start == Units.ZeroOnly)
                return Units.ZeroOnly;

            var columns = Columns();
            if (columns == null)
                return Units.None;

            var smallestColumn = NextSmallestUnit(start, columns.Length - 1);

            if (smallestColumn == Units.ErrorTooManyUnits && IsZero())
                return Units.ZeroOnly;

            return smallestColumn;
        }

    }
}
