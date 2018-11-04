using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TimeSpanParserUtil {

    abstract class ParserToken
    {

        public TimeSpanParserOptions options;
        public Units GivenUnit = Units.None;

        public override string ToString() {
            return ToTimeSpan().ToString() ?? "null";
        }

        public abstract bool IsNull();

        public abstract bool IsZero();

        public abstract bool IsInitialNegative();

        public abstract bool UsesColonedDefault();

        public virtual bool IsUnitlessFailure() {
            var units = BestGuessUnits();
            bool unitless = !(units.IsTimeUnit() || units == Units.ZeroOnly);
            bool ambiguous = !IsZero() && (units == Units.Months || units == Units.Years);

            return unitless || ambiguous;

            // what about IsNull() ?
        }

        public virtual bool IsOtherFailure() {
            var smallest = SmallestUnit();

            return (!smallest.IsTimeUnit() && smallest != Units.ZeroOnly);
        }

        protected virtual Units GivenOrDefaultOrZeroUnits() {
            if (GivenUnit != Units.None)
                return GivenUnit;

            Units def = Units.None;
            if (UsesColonedDefault()) {
                def = options.ColonedDefault;

            } else {
                def = options.UncolonedDefault;
            }

            if (def == Units.None && IsZero() && options.AllowUnitlessZero)
                return Units.ZeroOnly;

            return def; // TODO: check is valid (don't give error values?)
        }


        public virtual Units BestGuessUnits() { // GivenOrDefaultOrSplitUnits
            return GivenOrDefaultOrZeroUnits();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherNext"></param>
        /// <param name="merged"></param>
        /// <returns>Success:
        ///     False: there was a failure and parsing should fail. 
        ///     True && merged == null: did not merge, but continue on.
        ///     True && merged != null: did merge, continue on.</returns>
        public bool TryMerge(ParserToken otherNext, out ParserToken merged) {
            if (otherNext == null) {
                merged = null;
                return false; // shouldn't happen?
            }

            var otherUnits = otherNext.BestGuessUnits();

            if (otherUnits == Units.None || otherUnits == Units.ErrorTooManyUnits) { // && options.FailOnUnitlessNumber
                //Console.WriteLine("error thing");
                merged = null;
                return false;
            }

            var otherTimeSpan = otherNext.ToTimeSpan();
            if (IsInitialNegative())
                otherTimeSpan = -otherNext.ToTimeSpan();

            if (options.StrictBigToSmall) {
                var smallestUnit = SmallestUnit();

                //Console.WriteLine($"combining: {ToTimeSpan()?.ToString() ?? "null"} ({smallestUnit}) & {otherNext.ToTimeSpan()?.ToString() ?? "null"} ({otherUnits})");
                if (otherUnits.IsTimeUnit() && smallestUnit.IsTimeUnit() && otherUnits > smallestUnit) {

                    var newTokenWithSmallest = new TimeSpanToken();
                    newTokenWithSmallest.options = options;
                    newTokenWithSmallest.timespan = this.ToTimeSpan() + otherTimeSpan;
                    newTokenWithSmallest.smallest = otherNext.SmallestUnit();
                    newTokenWithSmallest.GivenUnit = BestGuessUnits(); // shouldn't matter
                    newTokenWithSmallest.initialNegative = IsInitialNegative();

                    merged = newTokenWithSmallest;
                    return true;
                }

                merged = null;
                return true;
            }

            var newToken = new TimeSpanToken();
            newToken.options = options;
            newToken.timespan = this.ToTimeSpan() + otherTimeSpan;
            newToken.initialNegative = IsInitialNegative();

            //newToken.smallest = otherNext.SmallestUnit(); // shouldn't matter because not StrictBigToSmall
            //newToken.GivenUnit = BestGuessUnits(); // shouldn't matter
            merged = newToken;
            return true;
        }

        public abstract TimeSpan? ToTimeSpan();

        protected virtual Units SmallestUnit() {
            return BestGuessUnits();
        }

        protected Units NextSmallestUnit(Units unit, int next=1) {
            if (unit == Units.None)
                return Units.None;

            if (unit == Units.ZeroOnly) {
                return Units.ZeroOnly; // return Units.None;
            }

            if (unit.IsTimeUnit()) {
                var nextSmallest = unit + next;
                if (nextSmallest.IsTimeUnit())
                    return nextSmallest;

                return Units.ErrorTooManyUnits;
            }

            return Units.None; // or return unit? (may be an error)
        }

        protected static TimeSpan? GetValue(decimal? time, Units unit) {
            if (time == null || unit == Units.None)
                return null;

            if (unit == Units.Years) {
                if (time == 0)
                    return TimeSpan.Zero;
                return null; // Ambiguous / error (support in future maybe)

            } else if (unit == Units.Months) {
                if (time == 0)
                    return TimeSpan.Zero;
                return null; // Ambiguous / errror (support in future maybe)    

            } else if (unit == Units.Weeks) {
                return TimeSpan.FromDays((double)time * 7);

            } else if (unit == Units.Days) {
                return TimeSpan.FromDays((double)time);

            } else if (unit == Units.Hours) {
                return TimeSpan.FromHours((double)time);

            } else if (unit == Units.Minutes) {
                return TimeSpan.FromMinutes((double)time);

            } else if (unit == Units.Seconds) {
                //timeSpan += TimeSpan.FromSeconds(time); // not very accurate, use ticks instead.

                long ticks = (long)(time * 10_000_000);
                return TimeSpan.FromTicks(ticks);

                /*
                if (Options.DecimalSecondsCountsAsMilliseconds) {
                    var timeTrunc = Math.Truncate(time);
                    if (time != timeTrunc) {
                        DoneUnits.Add(Units.Milliseconds);
                    }
                }
                */

            } else if (unit == Units.Milliseconds) {
                //return TimeSpan.FromMilliseconds((double)time);
                //TODO overflow checking
                long ticks = (long)(time * 10_000);
                return TimeSpan.FromTicks(ticks);

            } else if (unit == Units.Microseconds) {
                var absTime = Math.Abs(time.Value);
                if (absTime > 0 && absTime < new decimal(0.1)) {
                    throw new OverflowException("A component of the timespan was out of range (too small).");
                }

                long ticks = (long)(time * 10);
                return TimeSpan.FromTicks(ticks);

            } else if (unit == Units.Nanoseconds) {
                var absTime = Math.Abs(time.Value);
                if (absTime > 0 && absTime < 100) {
                    throw new OverflowException("A component of the timespan was out of range (too small).");
                }

                long ticks = (long)(time / 100);
                return TimeSpan.FromTicks(ticks);

            } else if (unit == Units.ZeroOnly) {
                // do nothing
                return TimeSpan.Zero;

            }

                return TimeSpan.Zero; // TODO: error?
            }


    }
}
