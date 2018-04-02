using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
namespace TimeSpanParserUtil
{
    public class TimeSpanParserOptions {

        // How to treat the first number, if it has no units (and no colon)
        // e.g. Units.Minutes will mean "4" gets treated as a 4m
        public Units DefaultPlain = Units.None;

        // How to treat the first number, if it has no units and does have a colon. 
        // e.g. Units.Minutes will mean "4:30" gets treated as a 4m30s
        // valid things are: Days, Hours, Minutes
        public Units DefaultColon = Units.Hours;

        // e.g. if true and DefaultColon = Units.Minutes, parse "05:10:30" as "05h10m30s" rather than failing
        public bool AutoUnitsIfTooManyColons = true;

        //TODO: implement
        // If true, treat :30 the same as 30, and :10:30 the same as 10:30.
        // If false (NYI), treat :30 like 0:30, and :10:30 like 00:10:30
        public readonly bool IgnoreStartingColon = true;

        //TODO: handling of empty colons, e.g. 10::30 // treat as 10:30 or 10:00:30 (or error) ?

        // If true then allow "3 hours 5 seconds" but not "5 seconds 3 hours" 
        // (causes AddUnit() to return false, signally an error or to divides into multiple TimeSpans)
        public bool StrictBigToSmall = true; 

        // If false, allow "1h 10m 5m" to equal 1h15m;
        // If true, cause an error (or divide into multiple TimeSpans) when a unit is repeated.
        // If StrictBigToSmall is true then it basically overrides this as if it were true anyway.
        public bool DisallowRepeatedUnit = true;

        // if true and DisallowRepeatedUnit, disallow "10.5 seconds 200 milliseconds", otherwise treat it like "10.7 seconds"
        public readonly bool DecimalSecondsCountsAsMilliseconds = true;

        // If true, "0" will be parsed as TimeSpan.Zero even if DefaultPlain = Units.None. 
        // Likewise "0:00" will be parsed even if and DefaultColon is set to Units.None.
        // if !DisallowRepeatedUnit && !StrictBigToSmall, then multiple 0's will be parsed. Otherwise it only parses a unitless "0" for the first time.
        public bool AllowUnitlessZero = true;

        //TODO: flag to treat seconds with decimal point as milliseconds for sake of AllowSameUnit
        //TODO: options as binary flags?

        // default to very permissive, but do not: AllowTrailingSign, AllowParentheses, AllowCurrencySymbol
        public NumberStyles NumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands | NumberStyles.AllowTrailingWhite ;

        //TODO: normal initial negative handling. Should -30h 30m mean -30:30h or -30h +30m (-29.5h).. argh.
        //public bool WeirdNegativeHandling = false;

        // Currently only for parsing numbers
        // In future, may be changed to default to CultureInfo.CurrentCulture or CultureInfo.InvariantCulture
        public IFormatProvider FormatProvider = null; 

    }
}
