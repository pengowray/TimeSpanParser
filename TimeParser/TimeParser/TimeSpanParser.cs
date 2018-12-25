using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

namespace TimeSpanParserUtil {

    public partial class TimeSpanParser {
        public static TimeSpan Parse(string text) {
            // possible exceptions:
            // FormatException
            // OverflowException
            // ArgumentException

            if (TryParse(text, timeSpan: out TimeSpan timeSpan)) {
                return timeSpan;
            }

            throw new ArgumentException("Failed to parse.");
        }

        public static TimeSpan Parse(string text, TimeSpanParserOptions options) {
            if (TryParse(text, options, out TimeSpan timeSpan)) {
                return timeSpan;
            }
            throw new ArgumentException("Failed to parse.");
        }

        public static TimeSpan Parse(string text, Units uncolonedDefault, Units colonedDefault) {
            if (TryParse(text, uncolonedDefault, colonedDefault, out TimeSpan timeSpan)) {
                return timeSpan;
            }

            throw new ArgumentException("Failed to parse."); // TODO?
        }


        private static Dictionary<string, Units> _Units;
        protected static Dictionary<string, Units> GetUnitsDict() {

            if (_Units == null) {
                _Units = new Dictionary<string, Units>
                {
                    ["ps"] = Units.Picoseconds,
                    ["picosec"] = Units.Picoseconds,
                    ["picosecs"] = Units.Picoseconds,
                    ["picosecond"] = Units.Picoseconds,
                    ["picoseconds"] = Units.Picoseconds,

                    ["ns"] = Units.Nanoseconds,
                    ["nanosec"] = Units.Nanoseconds,
                    ["nanosecs"] = Units.Nanoseconds,
                    ["nanosecond"] = Units.Nanoseconds,
                    ["nanoseconds"] = Units.Nanoseconds,

                    ["μs"] = Units.Microseconds,
                    ["microsec"] = Units.Microseconds,
                    ["microsecs"] = Units.Microseconds,
                    ["microsecond"] = Units.Microseconds,
                    ["microseconds"] = Units.Microseconds,

                    ["ms"] = Units.Milliseconds,
                    ["millisec"] = Units.Milliseconds,
                    ["millisecs"] = Units.Milliseconds,
                    ["millisecond"] = Units.Milliseconds,
                    ["milliseconds"] = Units.Milliseconds,

                    ["s"] = Units.Seconds,
                    ["sec"] = Units.Seconds,
                    ["secs"] = Units.Seconds,
                    ["second"] = Units.Seconds,
                    ["seconds"] = Units.Seconds,

                    ["m"] = Units.Minutes,
                    ["min"] = Units.Minutes,
                    ["mins"] = Units.Minutes,
                    ["minute"] = Units.Minutes,
                    ["minutes"] = Units.Minutes,

                    ["h"] = Units.Hours,
                    ["hr"] = Units.Hours,
                    ["hrs"] = Units.Hours,
                    ["hour"] = Units.Hours,
                    ["hours"] = Units.Hours,

                    ["d"] = Units.Days,
                    ["day"] = Units.Days,
                    ["days"] = Units.Days,

                    ["w"] = Units.Weeks,
                    ["wk"] = Units.Weeks,
                    ["wks"] = Units.Weeks,
                    ["week"] = Units.Weeks,
                    ["weeks"] = Units.Weeks,

                    //TODO: fortnight

                    // Only 0 months allowed (to avoid ambiguity)
                    //TODO: Gather use cases. Setting to handle as 28/30/31/30.43685 days or as error
                    ["month"] = Units.Months,
                    ["months"] = Units.Months,

                    // Only 0 years allowed (to avoid ambiguity, imprecision or relative-date dependence)
                    //TODO: Gather use cases. Perhaps create a min-max possible range. Or allow to be set by the user e.g. 365 / 366 / 365.25 / 365.24 / 365.2422 / Units.ErrorAmbiguous
                    ["y"] = Units.Years,
                    ["yr"] = Units.Years,
                    ["yrs"] = Units.Years,
                    ["year"] = Units.Years,
                    ["years"] = Units.Years
                };

                // calendar year with 365 days
                //_Units["common year"] = Units.Years; // TODO
                //_Units["common years"] = Units.Years; // TODO
            }

            return _Units;
        }

        private static Regex _UnitsRegex;
        protected static Regex GetUnitsRegex() {

            if (_UnitsRegex == null) {
                StringBuilder regex = new StringBuilder();
                // can start with any non-letter characters including underscore, which are all ignored. 
                regex.Append(@"^(?:[_\W])*(");
                regex.Append(@"?<units>"); // name group
                regex.Append(string.Join("|", GetUnitsDict().Keys.Select(k => Regex.Escape(k))));
                regex.Append(@")\b");
                _UnitsRegex = new Regex(regex.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            return _UnitsRegex;
        }

        protected static Units ParseSuffix(string suffix) { // was: TryParseSuffix()
            if (suffix == null) {
                //return Units.Error;
                throw new ArgumentNullException();
            }

            var regex = GetUnitsRegex();
            var match = regex.Match(suffix);
            if (!match.Success) {
                return Units.None;
                //return false;
            }

            var dict = GetUnitsDict();
            var units = Units.None;
            var success = dict.TryGetValue(match.Groups["units"].Value.ToLowerInvariant().Trim(), out units);
            //success = success && units != Units.ErrorAmbiguous && units != Units.Error;

            if (!success)
                return Units.None;

            return units;
        }

        public static bool TryParse(string text, Units uncolonedDefault, Units colonedDefault, out TimeSpan timeSpan) {
            var options = new TimeSpanParserOptions()
            {
                UncolonedDefault = uncolonedDefault,
                ColonedDefault = colonedDefault
            };
            return TryParse(text, options, out timeSpan);
        }

        public static bool TryParse(string text, out TimeSpan timeSpan) {
            return TryParse(text, null, out timeSpan);
        }

        public static bool TryParse(string text, TimeSpanParserOptions options, out TimeSpan timeSpan) {

            try {
                TimeSpan[] timeSpans;
                var success = TryParse(text, out timeSpans, options, 1);
                if (!success)
                    return false;

                if (timeSpans.Length == 0)
                    return false;

                timeSpan = timeSpans[0];
                return true;

            } catch (Exception e) {
                Console.WriteLine($" - exception:'{e}'");
                return false;
            }
        }

        public static bool TryParse(string text, Units uncolonedDefault, Units colonedDefault, out TimeSpan[] timeSpans, int max = int.MaxValue) {
            var options = new TimeSpanParserOptions()
            {
                UncolonedDefault = uncolonedDefault,
                ColonedDefault = colonedDefault
            };

            return TryParse(text, out timeSpans, options, max);

        }
        public static bool TryParse(string text, out TimeSpan[] timeSpans, TimeSpanParserOptions options = null, int max = int.MaxValue) {
            try {
                return DoParseMutliple(text, out timeSpans, options, max);
            } catch (ArgumentException e) {
                //Console.WriteLine("error: " + e);
                timeSpans = null;
                return false;
            }
        }
        
        private static decimal? ParseNumber(string part, TimeSpanParserOptions options) {
            decimal dPart = 0;
            if (decimal.TryParse(part, options.NumberStyles, options.FormatProvider, out dPart)) {
                return dPart;
            }

            return null;
        }
        protected static bool DoParseMutliple(string text, out TimeSpan[] timeSpans, TimeSpanParserOptions options = null, int max = int.MaxValue) {
            if (options == null)
                options = new TimeSpanParserOptions(); //TODO: default options object

            Units[] badDefaults = new Units[] { Units.Error, Units.ErrorTooManyUnits, Units.ErrorAmbiguous };
            if (badDefaults.Any(bad => options.UncolonedDefault == bad) || badDefaults.Any(bad => options.ColonedDefault == bad)) {
                throw new ArgumentException("Bad default selection.");
            }

            //TODO (or not: overly limited: requires groups of 3 numbers or fails): https://social.msdn.microsoft.com/Forums/en-US/431d51f9-8003-4c72-ba1f-e830c6ad75ba/regex-to-match-all-number-formats-used-around-the-world?forum=regexp

            text = text.Normalize(NormalizationForm.FormKC); // fixing any fullwidth characters
            text = text.Replace('_', ' ');

            var numberFormatInfo = (options.FormatProvider == null) 
                ? CultureInfo.CurrentCulture.NumberFormat 
                : NumberFormatInfo.GetInstance(options.FormatProvider);
            
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            bool allowThousands = ((options.NumberStyles & NumberStyles.AllowThousands) > 0);
            string groupSeparator = allowThousands ?
                Regex.Escape(numberFormatInfo.NumberGroupSeparator) : string.Empty;
            string plusMinus = numberFormatInfo.PositiveSign + numberFormatInfo.NegativeSign; // TODO?

            if (options.AllowDotSeparatedDayHours && decimalSeparator != ".") decimalSeparator += "."; // always also need a dot for day.hour separation (unless that's off)

            string zeroRegexStr = @"([+-]?:)?(([-+]?[0"+ groupSeparator + "]*[" + Regex.Escape(decimalSeparator) + @"}]?[0]+(?:[eE][-+]?[0-9]+)?)\:?)+"; // 0:00:00 0e100 0.00:00:00:0.000:0e20:00
            string numberRegexStr;
            //TODO: +- at start or end depending on culture
            if (allowThousands) {
                numberRegexStr = @"([+-]?:)?(([-+]?([0-9]+([" + groupSeparator + "]?)(?=[0-9]))*[" + Regex.Escape(decimalSeparator) + @"}]?[0-9]+(?:[eE][-+]?[0-9]+)?)\:?)+";
            } else {
                numberRegexStr = @"([+-]?:)?(([-+]?[0-9]*[" + Regex.Escape(decimalSeparator) + @"}]?[0-9]+(?:[eE][-+]?[0-9]+)?)\:?)+";
            }

            // regex notes:
            // - floating point numbers separated by (or ending with) with colon.
            // - matches a number: 30
            // - also matches floating point number: +3e-10
            // - also allows colons: 10:20:21.70
            // - or crazy combo: 10.2e+2:20:21.70 (note: the dot is sometimes a day separator)
            // - regex101.com for testing

            // weird things:
            // - supports mixed formats like "22:11h 10s" (=22:11:10)

            // may change:
            // - starting colon will be ignored, ":30" treated as "30"
            // - but not after: 3: (treated as "3")
            // - in future, starting-colon numbers may get their own option


            var numberRegex = new Regex(numberRegexStr); // TODO: re-use regex + RegexOptions.Compiled
            var zeroRegex = new Regex(zeroRegexStr);

            List<ParserToken> tokens = new List<ParserToken>();

            var matches = numberRegex.Matches(text);
            for (int i = 0; i < matches.Count; i++) { //  foreach (Match match in matches) {
                Match match = matches[i];

                int numberEnd = match.Index + match.Length;
                int nextMatchIndex = (i + 1 < matches.Count ? matches[i + 1].Index : text.Length);
                int suffixLength = nextMatchIndex - numberEnd;

                //Console.WriteLine($"text:{text}. match[{i}]: suffixLength:{suffixLength}");

                string number = match.Value;
                string suffix = text.Substring(numberEnd, suffixLength);
                bool coloned = number.Contains(':');

                //Console.WriteLine($"part[{i}]: num:'{number}', suffix:'{suffix}', colon:{coloned}");

                Units suffixUnits = ParseSuffix(suffix);

                //TODO: ignore initial colon (now) if requested

                if (coloned) {
                    var parts = number.Split(':');
                    if (parts.Length <= 1) {
                        timeSpans = null; //  timeSpans = builder.FinalSpans(); // foundTimeSpans.ToArray();
                        return false; // something went wrong. should never happen
                    }

                    var token = new ColonedToken();
                    token.options = options;
                    token.GivenUnit = suffixUnits;

                    //TODO: maybe don't do this if parsing a localization that doesn't use a dot separator for days.months ?
                    if (parts != null && parts.Length >= 1 && parts[0].Contains('.')) {
                        token.firstColumnContainsDot = true; //Note: specifically '.' and NOT the regional decimal separator
                        token.firstColumnRightHalf = ParseNumber(parts[0].Split('.')[1], options); //TODO: error checking
                    }

                    if (string.IsNullOrWhiteSpace(parts[0])) {
                        // TODO
                        token.startsWithColon = true;
                        parts[0] = null;

                    } else if (parts != null && parts.Length >= 1 && parts[0] != null && parts[0].Trim() == "-") {
                        //don't attempt to parse
                        parts[0] = null;
                        token.negativeColoned = true;
                        token.startsWithColon = true;

                    } else if (parts != null && parts.Length >= 1 && parts[0] != null && parts[0].Trim() == "+") { //TODO tidy
                        parts[0] = null;
                        token.startsWithColon = true;
                    }

                    token.colonedColumns = parts.Select(p => ParseNumber(p, options)).ToArray();
                    tokens.Add(token);

                    //Console.WriteLine($"token: {token}");

                } else {

                    //decimal parsedNumber;
                    //bool numberSuccess = decimal.TryParse(number, options.NumberStyles, options.FormatProvider, out parsedNumber);

                    var token = new OneUnitToken();
                    token.options = options;
                    token.GivenUnit = suffixUnits;
                    token.uncolonedValue = ParseNumber(number, options);
                    
                    tokens.Add(token);

                    //Console.WriteLine($"token= {token}");
                }
            }

            List<TimeSpan?> timespans = new List<TimeSpan?>();
            ParserToken last = null;
            bool willSucceed = true;
            foreach (ParserToken token in tokens) {
                if (token.IsUnitlessFailure() || token.IsOtherFailure()) {
                    //Console.WriteLine($"wont succeed..." + (!options.FailOnUnitlessNumber ? "or actually it might" : ""));
                    //throw new ArgumentException("failed to parse because of a unitless number.");
                    willSucceed = false;
                    if (last != null)
                        timespans.Add(last.ToTimeSpan());
                    last = null;
                    continue;
                }

                if (last != null) {
                    bool success = last.TryMerge(token, out ParserToken newToken);
                    if (!success)
                        throw new ArgumentException("Failed to parse. Probably because of a unitless number.");

                    if (newToken == null) {
                        timespans.Add(last.ToTimeSpan());
                        last = token;

                    } else {
                        last = newToken;
                    }
                    
                } else {
                    last = token;
                    
                }
            }
            if (last != null)
                timespans.Add(last.ToTimeSpan());

            timeSpans = timespans.Where(t => t.HasValue).Select(t => t.Value).ToArray(); // just the nonnull for now
            return !options.FailOnUnitlessNumber || willSucceed;
        }

    }
}