﻿using System;
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

        //NYI TODO and make true by default
        public bool FailIfMoreTimeSpansFoundThanRequested = false;

        // e.g. if true and ColonedDefault = Units.Minutes, parse "05:10:30" as "05h10m30s" rather than failing
        //TODO: change into "Segments must match units" or something
        public bool AutoUnitsIfTooManyColons = true;

        // "1.12:13" becomes "1d 12h 13m" (regardless of ColonedDefault). Only works for coloned numbers.
        // But won't if already has four parts, e.g. 1.2:20:40:50 (move this to testing docs)
        // if AutoUnitsIfTooManyColons == false, then must specify "days" as unit (text or ColonedDefault) 
        // (or what about None?)
        public bool AllowDotSeparatedDayHours = true;

        //TODO: implement
        // If true, treat :30 the same as 30, and :10:30 the same as 10:30. Especially useful if you're parsing input like "time:30" (where "time:" is to be ignored)
        // If false (NYI), treat :30 like 0:30, and :10:30 like 00:10:30
        public readonly bool IgnoreStartingColon = true;

        //TODO: handling of empty colons, e.g. 10::30 // treat as 10:30 or 10:00:30 (or error) ?

        // If true then allow "3 hours 5 seconds" but not "5 seconds 3 hours" 
        // (causes AddUnit() to return false, signally an error or to divides into multiple TimeSpans)
        public bool StrictBigToSmall = true;

        // [remove as redundant to StrictBigToSmall]
        // If false, allow "1h 10m 5m" to equal 1h15m;
        // If true, cause an error (or divide into multiple TimeSpans) when a unit is repeated.
        // If StrictBigToSmall is true then it basically overrides this as if it were true anyway.
        // public bool DisallowRepeatedUnit = true;

        // If true and StrictBigToSmall, disallow a timespan with both milliseconds and seconds with decimal point.
        // if true and StrictBigToSmall, disallow "10.5 seconds 200 milliseconds", otherwise treat it like "10.7 seconds"
        // TODO: something for all units with decimals, e.g. to force "10.5min 40s" to fail
        public bool DecimalSecondsCountsAsMilliseconds = false;

        // Apart AllowUnitlessZero, covered below, 
        // should unitless numbers with no default unit cause parsing to fail?
        // If false, just ignore them
        public bool FailOnUnitlessNumber = true;

        // If true, a 0 or 0:00 by itself doesn't need any units, even if FailOnUnitlessNumber is true. (todo: just say "see GuideTests for details")
        // If true, "0" will be parsed as TimeSpan.Zero even if UncolonedDefault = Units.None. 
        // Likewise "0:00" or "0:0:0:0" etc will be parsed even if and ColonedDefault is set to Units.None.
        // if !StrictBigToSmall, then multiple zeros can be parsed (and ignored) in the same timespan. 
        // Otherwise it will parse a unitless "0" only for the first unit, and not allow further units
        public bool AllowUnitlessZero = true;

        // if true, units which are too small will cause an OverflowException. E.g. 1 nanosecond (which is 100x smaller than the smallest unit, a tick).
        //public bool ErrorOnTooSmallOutOfRange = true; 
        //TODO: options: Overflow if any element is too smaller; Overflow if entire TimeSpan is too small; Round to zero
        //TODO: same for overflow of too large elements 

        //TODO: options as binary flags?

        // default to very permissive, but do not: AllowTrailingSign, AllowParentheses, AllowCurrencySymbol
        public NumberStyles NumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands | NumberStyles.AllowTrailingWhite ;

        //TODO: normal initial negative handling. Should -30h 30m mean -30:30h or -30h +30m (-29.5h).. argh.
        //public bool WeirdNegativeHandling = false;

        // For parsing numbers, etc
        // Defaulting to CultureInfo.InvariantCulture for now until support is improved.
        // In future, will default to CultureInfo.CurrentCulture
        // Not fully supported yet. Does support changing the units decimal separator but then might fail on a US-style coloned number where .net's TimeSpan.Parser would not
        public IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

    }
}
