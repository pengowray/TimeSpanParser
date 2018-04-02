using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSpanParserUtil {
    class TimeSpanBuilder {
        private Units PrevUnit = Units.None;
        private HashSet<Units> DoneUnits = new HashSet<Units>(); //TODO: only create if needed

        //private bool ColonNumberBuilder = false;
        public int RemainingTimeSpans = int.MaxValue;

        TimeSpanParserOptions Options;
        public TimeSpanBuilder(TimeSpanParserOptions options = null) {
            if (options == null)
                options = new TimeSpanParserOptions();

            this.Options = options;
        }

        private bool StrictBigToSmall { get { return Options.StrictBigToSmall; } } // default: true
        private bool DisallowRepeatedUnit { get { return Options.StrictBigToSmall || Options.StrictBigToSmall; } } // default: true

        public TimeSpan TimeSpan { get => timeSpan; }

        // the builder has recieved no inputs. Might be considered null rather than 00:00:00
        //public bool IsNull { get => PrevUnit == Units.None; }
        public bool IsNull { get => isNull; }
        private bool isNull = true;

        private TimeSpan timeSpan = TimeSpan.Zero;

        // did the first item have a negative value. If so, do the same for subsequent time units
        private bool initialNegative = false;
        private bool currentlyParsingAColonBlock = false;
        private bool currentlyParsingZeroOnly = false;
        private bool finishedParsingZeroOnly = false;
        public TimeSpan? CompleteTimeSpan;

        public void StartParsingColonishNumber() {
            currentlyParsingAColonBlock = true;
        }

        public void EndParsingColonishNumber() {
            currentlyParsingAColonBlock = false;
            if (currentlyParsingZeroOnly) {
                currentlyParsingZeroOnly = false;
                finishedParsingZeroOnly = true;
            }
        }

        protected TimeSpanBuilder NewBuilder() {
            if (RemainingTimeSpans <= 0) {
                return null;
            }

            var newBuilder = new TimeSpanBuilder(Options);
            newBuilder.RemainingTimeSpans = RemainingTimeSpans - 1;
            return newBuilder;
        }
        

        /// <summary>
        /// e.g. for "10:30", if PrevUnit was minutes (10), then the "NextColonUnit" (30) will be seconds
        /// </summary>
        /// <returns></returns>
        public Units NextColonUnit() {
            if (PrevUnit == Units.None)
                return Units.None; // any

            if (PrevUnit >= Units.Days && PrevUnit <= Units.Minutes)
                return PrevUnit + 1;
            
            if (PrevUnit == Units.Seconds)
                return Units.ErrorTooManyUnits;

            if (PrevUnit == Units.ZeroOnly && currentlyParsingZeroOnly)
                return Units.ZeroOnly;

            return Units.Error;
        }

        public TimeSpan? Final() {
            if (IsNull)
                return null;

            return TimeSpan;
        }

        public Units GetUnitOrDefaultOption(Units originalUnit) {
            var unit = originalUnit;
            if (unit == Units.None) {
                if (IsNull) {
                    // resort to default
                    unit = currentlyParsingAColonBlock ? Options.DefaultColon : Options.DefaultPlain;

                } else if (currentlyParsingAColonBlock) {
                    unit = NextColonUnit();
                }
            }

            //Note: doesn't ever return Units.ZeroOnly

            return unit;
        }

        // Returns false if fails StrictBigToSmall or DisallowRepeatUnit. Can be treated as failure, or as a signal to the parser that a new TimeSpan has begun
        // Can throw: ArgumentException, OverflowException
        // noDeeper = don't go deeper (used internally only to avoid infinite recursion)
        // retrieve the just-completed TimeSpan in CompleteTimeSpan
        public TimeSpanBuilder AddUnit(double time, Units originalUnit, bool noDeeper = false) {

            // work out what unit we're using. either originalUnit, a default, or a continuation from a colon-number

            var unit = GetUnitOrDefaultOption(originalUnit);

            // if no default unit, allow a zero anyway (must be at the start, unless 
            if (unit == Units.None && Options.AllowUnitlessZero && time == 0 && !finishedParsingZeroOnly 
                    && (IsNull || currentlyParsingZeroOnly || (!Options.StrictBigToSmall && !Options.StrictBigToSmall))) {

                unit = Units.ZeroOnly;
                if (currentlyParsingAColonBlock) {
                    currentlyParsingZeroOnly = true;
                    //return this;

                } else {

                    finishedParsingZeroOnly = true;

                    //meh, just follow the rules of repeated units and stuff ?
                    //var next = NewBuilder();
                    //next.CompleteTimeSpan = TimeSpan.Zero;
                    //return next;
                }

            } 


            // Done working out what unit is now.

            Units prevUnit = PrevUnit;
            bool repeatedUnit = DoneUnits.Contains(unit);
            PrevUnit = unit;
            DoneUnits.Add(unit);
            CompleteTimeSpan = null;
            currentlyParsingZeroOnly = false;
            bool nowNull = isNull;
            isNull = false;

            Console.WriteLine($"actual unit: {unit}");
            //don't use PrevUnit or DoneUnits now

            if (unit == Units.None || unit == Units.Error || unit == Units.ErrorTooManyUnits || unit == Units.ErrorAmbiguous)
                throw new ArgumentException("Bad unit exception or no default: " + unit);

            if (unit == Units.SplitMe) { // delete me
                var next = NewBuilder();
                next.CompleteTimeSpan = TimeSpan;
                return next;
            }

            if (nowNull && time < 0) {
                initialNegative = true;
            } else if (initialNegative) {
                time *= -1;
            }

            //if (DoneUnits.Contains(Units.ZeroOnly)) {
            //    return false; // already had a 0 with no units. that's enough. (if not a colon number)
            //}

            if (DisallowRepeatedUnit && repeatedUnit) {
                if (noDeeper) throw new ArgumentException("something went wrong");
                var next = NewBuilder();
                next.CompleteTimeSpan = TimeSpan;
                next.AddUnit(time, originalUnit, true);
                return next;
            }

            if (StrictBigToSmall && unit <= prevUnit) {
                //return false;

                if (noDeeper) throw new ArgumentException("something went wrong");
                var next = NewBuilder();
                next.CompleteTimeSpan = TimeSpan;
                next.AddUnit(time, originalUnit, true);
                return next;
            }


            // Weeks, Days, Hours, Minutes, Seconds, Milliseconds

            if (unit == Units.Weeks) {
                timeSpan += TimeSpan.FromDays(time * 7);

            } else if (unit == Units.Days) {
                timeSpan += TimeSpan.FromDays(time);

            } else if (unit == Units.Hours) {
                timeSpan += TimeSpan.FromHours(time);

            } else if (unit == Units.Minutes) {
                timeSpan += TimeSpan.FromMinutes(time);

            } else if (unit == Units.Seconds) {
                timeSpan += TimeSpan.FromSeconds(time);
                //TODO: check for Options.DecimalSecondsCountsAsMilliseconds

            } else if (unit == Units.Milliseconds) {
                timeSpan += TimeSpan.FromMilliseconds(time);

            } else if (unit == Units.ZeroOnly) {
                // do nothing

            }

            return this;
        }
    }
}

