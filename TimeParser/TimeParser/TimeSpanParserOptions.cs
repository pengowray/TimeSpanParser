using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
namespace TimeSpanParserUtil
{
    public class TimeSpanParserOptions {

        // How to treat the first number, if it has no units (and no colon)
        // e.g. Units.Minutes will mean "4" gets treated as "4 minutes"
        public Units UncolonedDefault = Units.None;

        // How to treat the first number, if it has no units and does have a colon. 
        // e.g. Units.Minutes will mean "4:30" gets treated as a 4m30s
        // valid things are: Days, Hours, Minutes
        public Units ColonedDefault = Units.Hours;

        // e.g. if true and ColonedDefault = Units.Minutes, parse "05:10:30" as "05h10m30s" rather than failing
        public bool AutoUnitsIfTooManyColons = true;

        // "1.12:13" for "1d 12h 13m" (regardless of ColonedDefault). 
        // But won't if already has four parts, e.g. 1.2:20:40:50 (move this to testing docs)
        public bool AllowDotSeparatedDayHours = true;

        //TODO: implement
        // If true, treat :30 the same as 30, and :10:30 the same as 10:30. Especially useful if you're parsing input like "time:30" (where "time:" is to be ignored)
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

        // If true, treat seconds with decimal point as milliseconds for sake of DisallowRepeatedUnit
        // if true and DisallowRepeatedUnit, disallow "10.5 seconds 200 milliseconds", otherwise treat it like "10.7 seconds"
        public bool DecimalSecondsCountsAsMilliseconds = true;

        // If true, a 0 or 0:00 by itself doesn't need any units. (todo: just say "see GuideTests for details)
        // If true, "0" will be parsed as TimeSpan.Zero even if UncolonedDefault = Units.None. 
        // Likewise "0:00" or "0:0:0:0" etc will be parsed even if and ColonedDefault is set to Units.None.
        // if !DisallowRepeatedUnit && !StrictBigToSmall, then multiple zeros can be parsed (and ignored) in the same timespan. 
        // Otherwise it will parse a unitless "0" only for the first unit, and not allow further units
        public bool AllowUnitlessZero = true;

        //TODO: options as binary flags?

        // default to very permissive, but do not: AllowTrailingSign, AllowParentheses, AllowCurrencySymbol
        public NumberStyles NumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands | NumberStyles.AllowTrailingWhite ;

        //TODO: normal initial negative handling. Should -30h 30m mean -30:30h or -30h +30m (-29.5h).. argh.
        //public bool WeirdNegativeHandling = false;

        // Currently only for parsing numbers (and only partially)
        // In future, may be changed to default to CultureInfo.CurrentCulture or CultureInfo.InvariantCulture
        //TODO: largely NYI
        public IFormatProvider FormatProvider = null; 

    }
}
