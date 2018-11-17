using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSpanParserUtil {

    //Note: Units must be largest to smallest with "None" as the "biggest". 
    //Note: Units must be in strict order for how they'd be parsed in colon-format, i.e. Weeks:Days:Hours:Minutes:Seconds (stops at Milliseconds)
    //Note: add other units after ZeroOnly (or Milliseconds)
    //Note: only 0 months and 0 years are allowed
    //Do not change to binary flags
    //TODO: separate ordering error value from other errors (e.g. null)
    public enum Units { None, Error, ErrorAmbiguous, Years, Months, Weeks, Days, Hours, Minutes, Seconds, Milliseconds, Microseconds, Nanoseconds, Picoseconds, ErrorTooManyUnits, ZeroOnly }

    public static class UnitsExtensions {
        public static bool IsTimeUnit(this Units unit) {
            return (unit >= Units.Years && unit <= Units.Picoseconds);
        }
    }

}
