using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSpanParserUtil {
    class TimeSpanBuilder {
        private Units PrevUnit = Units.None;
        private HashSet<Units> DoneUnits = new HashSet<Units>(); //TODO: only create if needed

        //TODO: options as binary flags
        private bool StrictBigToSmall = true; // if true then allow "3 hours 5 seconds" but not "5 seconds 3 hours" (causes AddUnit() to return false, signally an error or to divides into multiple TimeSpans)
        private bool DisallowRepeatedUnit = true; // if false then allow "1h 10m 5m" to equal 1h15m; if true, cause an error (or divide into multiple TimeSpans). If StrictBigToSmall is true then it basically overrides this as if it were true anyway.
        //TODO: flag to treat seconds with decimal point as milliseconds for sake of AllowSameUnit

        public TimeSpan TimeSpan { get => timeSpan; } 
        public bool IsNull { get => PrevUnit == Units.None; } // the builder has recieved no inputs. Might be considered null rather than 00:00:00

        private TimeSpan timeSpan = TimeSpan.Zero;
        private bool initialNegative = false; // did the first item have a negative value. If so, do the same for subsequent time units

        // Returns false if fails StrictBigToSmall or DisallowRepeatUnit. Can be treated as failure, or as a signal to the parser that a new TimeSpan has begun
        // Can throw: ArgumentException, OverflowException
        public bool AddUnit(double time, Units unit) {
            if (unit == Units.None || unit == Units.Error)
                throw new ArgumentException("Bad unit exception: " + unit);

            if (IsNull && time < 0) {
                initialNegative = true;
            } else if (initialNegative) {
                time *= -1;
            }

            if (StrictBigToSmall && unit <= PrevUnit) {
                return false;
            }
            PrevUnit = unit;

            if (DisallowRepeatedUnit && DoneUnits.Contains(unit)) {
                return false;
            }
            DoneUnits.Add(unit);

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

            } else if (unit == Units.Milliseconds) {
                timeSpan += TimeSpan.FromMilliseconds(time);
            }

            return true;
        }
    }
}

