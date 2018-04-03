using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace TimeSpanParserUtil {

    //Note: Units must be largest to smallest with "None" as the "biggest". 
    //Note: Units must be in strict order for how they'd be parsed in colon-format, i.e. Weeks:Days:Hours:Minutes:Seconds (stops at Milliseconds)
    //Note: add other units after ZeroOnly (or Milliseconds)
    //Do not change to binary flags
    //TODO: separate ordering error value from other errors (e.g. null)
    public enum Units { None, Error, ErrorAmbiguous, Weeks, Days, Hours, Minutes, Seconds, Milliseconds, ErrorTooManyUnits, ZeroOnly }

    public class TimeSpanParser {
        public static TimeSpan Parse(string text) {
            if (TryParse(text, out TimeSpan timeSpan)) {
                return timeSpan;
            }

            throw new ArgumentException("Failed to parse."); // TODO?
        }

        public static TimeSpan Parse(string text, TimeSpanParserOptions options) {
            if (TryParse(text, out TimeSpan timeSpan, options)) {
                return timeSpan;
            }

            throw new ArgumentException("Failed to parse."); // TODO?
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
                _Units = new Dictionary<string, Units>();
                _Units["ms"] = Units.Milliseconds;
                _Units["millisec"] = Units.Milliseconds;
                _Units["millisecs"] = Units.Milliseconds;
                _Units["millisecond"] = Units.Milliseconds;
                _Units["milliseconds"] = Units.Milliseconds;

                _Units["s"] = Units.Seconds;
                _Units["sec"] = Units.Seconds;
                _Units["secs"] = Units.Seconds;
                _Units["second"] = Units.Seconds;
                _Units["seconds"] = Units.Seconds;

                _Units["m"] = Units.Minutes;
                _Units["min"] = Units.Minutes;
                _Units["mins"] = Units.Minutes;
                _Units["minute"] = Units.Minutes;
                _Units["minutes"] = Units.Minutes;

                _Units["h"] = Units.Hours;
                _Units["hr"] = Units.Hours;
                _Units["hrs"] = Units.Hours;
                _Units["hour"] = Units.Hours;
                _Units["hours"] = Units.Hours;

                _Units["d"] = Units.Days;
                _Units["day"] = Units.Days;
                _Units["days"] = Units.Days;
                _Units["solar day"] = Units.Days;
                _Units["solar days"] = Units.Days;

                _Units["w"] = Units.Weeks;
                _Units["wk"] = Units.Weeks;
                _Units["wks"] = Units.Weeks;
                _Units["week"] = Units.Weeks;
                _Units["weeks"] = Units.Weeks;

                // We don't currently handle months.
                //TODO: Gather use cases. Setting to handle as 28/30/31/30.43685 days or as error
                _Units["month"] = Units.ErrorAmbiguous;
                _Units["months"] = Units.ErrorAmbiguous;

                // We don't currently handle years. 
                //TODO: Gather use cases. Perhaps create a min-max possible range. Or allow to be set by the user e.g. 365 / 366 / 365.25 / 365.24 / 365.2422 / ErrorAmbiguous
                _Units["y"] = Units.ErrorAmbiguous;
                _Units["ys"] = Units.ErrorAmbiguous;
                _Units["year"] = Units.ErrorAmbiguous;
                _Units["years"] = Units.ErrorAmbiguous;
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
                _UnitsRegex = new Regex(regex.ToString(), RegexOptions.IgnoreCase & RegexOptions.Compiled);
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
            var success = dict.TryGetValue(match.Groups["units"].Value.ToLowerInvariant(), out units);
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
            return TryParse(text, out timeSpan, options);
        }

        public static bool TryParse(string text, out TimeSpan timeSpan, TimeSpanParserOptions options = null) {

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
            if (options == null)
                options = new TimeSpanParserOptions(); //TODO: default options object

            Units[] badDefaults = new Units[] { Units.Error, Units.ErrorTooManyUnits, Units.ErrorAmbiguous };
            if (badDefaults.Any(bad => options.UncolonedDefault == bad) || badDefaults.Any(bad => options.ColonedDefault == bad)) {
                throw new ArgumentException("Bad default selection.");
            }


            //TODO: https://social.msdn.microsoft.com/Forums/en-US/431d51f9-8003-4c72-ba1f-e830c6ad75ba/regex-to-match-all-number-formats-used-around-the-world?forum=regexp

            //string numberRegexStr = @"(([-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?)\:?)+";
            //options.FormatProvider?.GetFormat(Type.) 
            string numberRegexStr = @"(([-+]?[0-9]*[\.]?[0-9]+(?:[eE][-+]?[0-9]+)?)\:?)+";

            // regex notes:
            // - floating point numbers separated by (or ending with) with colon.
            // - matches a number: 30
            // - also matches floating point number: +3e-10
            // - also allows colons: 10:20:21.70
            // - or crazy combo: 10.2e+2:20:21.70 (note: the dot is sometimes a day separator)

            // weird things:
            // - supports mixed formats like "22:11h 10s" (=22:11:10)
            // - starting colon will be ignored, ":30" =30
            // - but not after: 3: (treated as "3")

            // regex101.com for testing

            //TODO: 
            // - support for https://en.wikipedia.org/wiki/ISO_8601#Durations
            // - better exception messages
            // - support , as decimal separator -- culture-specific formats for the current culture. (eg fr-FR)
            // - word numbers ("one second")

            //TODO some day:
            // - accept "minutes:30, seconds:10" or "minutes=30"
            // - accept "5:10 mm:ss" format (number with its format)
            // - accept ℯ exponent character (U+212F)
            // - accept weird SI units like microsecond, megaseconds, gigaseconds, decaseconds, etc
            // - give error on slash, e.g. 20/sec
            // - explicitly accept weird plurals, e.g. 20 sec/s or second(s) -- already fine already because creates word boundry

            List<TimeSpan> foundTimeSpans = new List<TimeSpan>();

            try {
                var numberRegex = new Regex(numberRegexStr); // TODO: re-use regex + RegexOptions.Compiled

                TimeSpanBuilder builder = new TimeSpanBuilder(options);
                builder.RemainingTimeSpans = max;

                var matches = numberRegex.Matches(text);
                for (int i = 0; i < matches.Count; i++) { //  foreach (Match match in matches) {
                    Match match = matches[i];
                    int numberEnd = match.Index + match.Length;
                    int nextMatchIndex = (i + 1 < matches.Count ? matches[i+1].Index : text.Length);
                    int suffixLength = nextMatchIndex - numberEnd;

                    //Console.WriteLine($"text:{text}. match[{i}]: suffixLength:{suffixLength}");

                    string number = match.Value;
                    string suffix = text.Substring(numberEnd, suffixLength);
                    bool coloned = number.Contains(':');

                    Console.WriteLine($"part[{i}]: num:'{number}', suffix:'{suffix}', colon:{coloned}");

                    Units suffixUnits = ParseSuffix(suffix);

                    if (coloned) {
                        builder.StartParsingColonishNumber();

                        var parts = number.Split(':');
                        if (parts.Length <= 0) {
                            timeSpans = foundTimeSpans.ToArray();
                            return false; // something went wrong
                        }

                        var partUnit = builder.GetUnitOrDefaultOption(suffixUnits);
                        Console.WriteLine($"default partUnit: {partUnit}. default colon options: {options.ColonedDefault}");

                        if (options.AllowDotSeparatedDayHours && parts.Length >= 2 && parts.Length <= 3 && parts[0].Contains('.')) {
                            var p0 = parts[0];
                            if (p0.Count(ch => ch == '.') > 1) {
                                if (p0.TrimEnd('.').Count(ch => ch == '.') > 1) { // last chance.. remove trailing dots
                                    throw new FormatException("Multiple dots. Don't know where to cut days and hour.");
                                } else {
                                    p0 = p0.TrimEnd('.');
                                }
                            }

                            parts = p0.Split('.', 2).Concat(parts.Skip(1)).ToArray();
                            partUnit = Units.Days;

                        } else if (options.AutoUnitsIfTooManyColons) {
                            if (parts.Length == 4 && partUnit >= Units.Hours && partUnit < Units.Milliseconds || partUnit == Units.None) {
                                // unit too small, auto adjust
                                partUnit = Units.Days; // largest unit we'll AutoAdjust to (i.e. Don't do weeks unless explicit)
                            } else if (parts.Length == 3 && (partUnit == Units.Minutes || partUnit == Units.Seconds)) {
                                // unit too small, auto adjust
                                partUnit = Units.Hours;
                            } else if (parts.Length == 2 && partUnit == Units.Seconds) {
                                // unit too small, auto adjust
                                partUnit = Units.Minutes;
                            }
                        }

                        Console.WriteLine($"post AutoUnits: {partUnit}");

                        // Weeks, Days, Hours, Minutes, Seconds, Milliseconds
                        foreach (var part in parts) {
                            Console.WriteLine($"colon bit: {part} -- {partUnit}" );

                            if (partUnit == Units.ErrorTooManyUnits || partUnit == Units.Milliseconds) {
                                timeSpans = foundTimeSpans.ToArray();
                                return false; // too many colons
                            }

                            double dPart = 0;
                            if (!string.IsNullOrWhiteSpace(part) && !double.TryParse(part, options.NumberStyles, options.FormatProvider, out dPart)) {
                                timeSpans = foundTimeSpans.ToArray();
                                return false; // failed to parse number (as double). Empty strings treated as 0.
                            }
                            builder = builder.AddUnit(dPart, partUnit);
                            partUnit = Units.None; // automatically choose future units
                            if (builder.CompleteTimeSpan != null) {
                                Console.WriteLine($"found colonish unit {builder.CompleteTimeSpan.Value}");
                                foundTimeSpans.Add(builder.CompleteTimeSpan.Value);
                                builder.CompleteTimeSpan = null;
                            }

                            Console.WriteLine($"WIP: {builder.TimeSpan}");

                        }
                        Console.WriteLine($"done with colon bits");
                        builder?.EndParsingColonishNumber();

                    } else {
                        
                        double parsedNumber;
                        bool numberSuccess = double.TryParse(number, options.NumberStyles, options.FormatProvider, out parsedNumber);
                        if (!numberSuccess) {
                            Console.WriteLine($" - failed to parse number:'{parsedNumber}'");
                            timeSpans = foundTimeSpans.ToArray();
                            return false;
                        }

                        //bool ok = builder.AddUnit(parsedNumber, suffixUnits);
                        builder = builder.AddUnit(parsedNumber, suffixUnits);
                        suffixUnits = Units.None; // automatically choose future units

                        if (builder.CompleteTimeSpan != null) {
                            Console.WriteLine($"found noncolonish unit {builder.CompleteTimeSpan.Value}");
                            foundTimeSpans.Add(builder.CompleteTimeSpan.Value);
                            builder.CompleteTimeSpan = null;
                        }

                    }
                }

                var final = builder?.Final();
                if (final != null) {
                    foundTimeSpans.Add(final.Value);
                    builder.CompleteTimeSpan = null;
                }

                timeSpans = foundTimeSpans.ToArray();
                return true;

            } catch (Exception e) {
                Console.WriteLine($" - exception:'{e}'");
                timeSpans = foundTimeSpans.ToArray();
                return false;
            }
        }

        /// <summary>
        /// Note: a special entries matches["0"] matches["1"] etc are included if `text` starts with timespans.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="uncolonedDefault"></param>
        /// <param name="colonedDefault"></param>
        /// <param name="prefixes"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool TryParsePrefixed(string text, Units uncolonedDefault, Units colonedDefault, string[] prefixes, out Dictionary<string,TimeSpan?> matches) {
            //string[] prefixes = new string[] { "for", "in", "delay", "wait" };

            //TODO: rename "prefixes" to "parameter names" or "keys" or something

            matches = new Dictionary<string, TimeSpan?>();

            //string pattern = @"\b(for|in|delay|now|wait)\b"; //TODO: build from prefixes

            StringBuilder pattern = new StringBuilder();
            pattern.Append(@"\b("); // must be in (brackets) to be included in results of regex split
            //pattern.Append(@"?<keyword>"); // name group
            pattern.Append(string.Join("|", prefixes.Select(prefix => Regex.Escape(prefix))));
            pattern.Append(@")\b");

            var regex = new Regex(pattern.ToString(), RegexOptions.IgnoreCase);
            string[] parts = regex.Split(text);
            //Console.WriteLine("parts: " + string.Join(" | ", parts));

            for (int i=0; i < parts.Length; i++) {
                var part = parts[i];
                var lc = part.ToLowerInvariant();
                if (prefixes.Contains(lc)) {
                    if (i + 1 < parts.Length) {
                        var lookahead = parts[i + 1];
                        var lookaheadLc = lookahead.ToLowerInvariant();
                        if (!prefixes.Contains(lookaheadLc)) {
                            // try lookahead as timespan
                            if (TryParse(lookahead, uncolonedDefault, colonedDefault, out TimeSpan timespan)) {
                                matches[lc] = timespan;

                                i++;
                                continue;
                            } else {
                                matches[lc] = null;

                                i++; // Still skip the next part anyway because we know it's not a keyword. 
                                     // But in future might want to still process it...?

                                continue;
                            }

                        }
                    }

                    // return prefix keyword without timespan
                    matches[lc] = null;
                } else if (i==0) {
                    // first part before any prefixes

                    if (TryParse(part, uncolonedDefault, colonedDefault, out TimeSpan[] timespans)) {
                        for (int j = 0; j < timespans.Length; j++) {
                            matches[j.ToString()] = timespans[j];
                        }
                    }
                } else {
                    // ignore non-keyword parameters except for first

                }

            }

            return (matches.Count > 0);
        }
    }
}