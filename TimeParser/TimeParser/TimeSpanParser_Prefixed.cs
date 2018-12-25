using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

namespace TimeSpanParserUtil {
    public partial class TimeSpanParser {

        public static bool TryParsePrefixed(string text, string[] prefixes, Units uncolonedDefault, Units colonedDefault, out Dictionary<string, TimeSpan?> matches) {
            return TryParsePrefixed(text, prefixes, null, uncolonedDefault, colonedDefault, out matches);
        }

        /// <summary>
        /// Note: a special entries matches["0"] matches["1"] etc are included if `text` starts with timespans.
        /// </summary>
        /// <param name="text"></param>34
        /// <param name="uncolonedDefault"></param>
        /// <param name="colonedDefault"></param>
        /// <param name="prefixes">Prefixes which are (optionally) followed by a timespan</param>
        /// <param name="keywords">Keywords which do not have a timespan variable after them (any timespan after it will be considered a default numbered argument)</param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool TryParsePrefixed(string text, string[] prefixes, string[] keywords, Units uncolonedDefault, Units colonedDefault, out Dictionary<string, TimeSpan?> matches) {
            var options = new TimeSpanParserOptions
            {
                UncolonedDefault = uncolonedDefault,
                ColonedDefault = colonedDefault
            };
            return TryParsePrefixed(text, prefixes, keywords, options, out matches);
        }
        public static bool TryParsePrefixed(string text, string[] prefixes, out Dictionary<string, TimeSpan?> matches) {
            return TryParsePrefixed(text, prefixes, null, out matches);
        }

        public static bool TryParsePrefixed(string text, string[] prefixes, TimeSpanParserOptions options, out Dictionary<string, TimeSpan?> matches) {
            return TryParsePrefixed(text, prefixes, null, options, out matches);
        }

        public static bool TryParsePrefixed(string text, string[] prefixes, string[] keywords, TimeSpanParserOptions options, out Dictionary<string, TimeSpan?> matches) {
            //string[] prefixes = new string[] { "for", "in", "delay", "wait" };

            //TODO: rename "prefixes" to "parameter names" or "keys" or something

            bool specialColonPrefix = false; // matches[":"] is the first unnamed timespan starting with a ":", used only if prefixes contains ":".

            if (options == null) {
                options = new TimeSpanParserOptions();
            }

            matches = new Dictionary<string, TimeSpan?>();

            //e.g. string pattern = @"\b(for|in|delay|now|wait)\b"; 
            //TODO: replace spaces with any amount of whitespace (currently @"\ ") e.g. "[\s.']*" (spaces dots or ' ) // perhaps do replacements first to make it easier, e.g. replace "

            var wordsList = Enumerable.Empty<string>();

            if (keywords != null)
                wordsList = wordsList.Concat(keywords);

            if (prefixes != null) {
                wordsList = wordsList.Concat(prefixes);

                if (prefixes.Contains(":")) {
                    specialColonPrefix = true;
                    wordsList = wordsList.Where(w => w != ":");
                }
            }

            // must be in (brackets) to be included in results of regex split
            // @"?<keyword>"; // name group
            string pattern = @"\b(" + string.Join("|", wordsList.Select(word => Regex.Escape(word))) + @")\b";
            var regex = new Regex(pattern.ToString(), RegexOptions.IgnoreCase & RegexOptions.IgnorePatternWhitespace);
            string[] parts = regex.Split(text);
            //Console.WriteLine("pattern: " + pattern.ToString());
            //Console.WriteLine(string.Join("//", parts));
            
            int nonkeywordCounter = 0;
            string currentPrefix = null;

            try {
                for (int i = 0; i < parts.Length; i++) {
                    var part = parts[i];
                    var lc = part.ToLowerInvariant();

                    if (string.IsNullOrWhiteSpace(lc)) {
                        continue;

                    } else if (keywords != null && keywords.Contains(lc)) { // keyword with no arguments
                        matches[lc] = null;
                        currentPrefix = null;


                    } else if (prefixes != null && prefixes.Contains(lc)) { // keyword (prefix) with possible argument
                        matches[lc] = null;
                        currentPrefix = lc;

                    } else {
                        if (DoParseMutliple(part, out TimeSpan[] timespans, options)) {

                            for (int j = 0; j < timespans.Length; j++) {
                                string prefix = currentPrefix 
                                    ?? (specialColonPrefix && part.TrimStart().StartsWith(":") && !matches.ContainsKey(":") ? ":" : null) // use ":" as the prefix name under special circumstances .. note/bug: only works for the first timespan if there's multiple in a row. TODO
                                    ?? nonkeywordCounter++.ToString();

                                matches[prefix] = timespans[j];
                                currentPrefix = null;
                            }

                        } else {
                            if (options.FailOnUnitlessNumber)
                                return false;
                        }
                    }
                }
            } catch (ArgumentException e) {
                //matches = null;
                //Console.WriteLine(e);
                if (options.FailOnUnitlessNumber)
                    return false;
            }


            return true; //return (matches.Count > 0);
        }
    }
}
