using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;

namespace TimeSpanParserUtil {
    public partial class TimeSpanParser {

        /// <summary>
        /// Note: a special entries matches["0"] matches["1"] etc are included if `text` starts with timespans.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="uncolonedDefault"></param>
        /// <param name="colonedDefault"></param>
        /// <param name="prefixes"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool TryParsePrefixed(string text, string[] prefixes, Units uncolonedDefault, Units colonedDefault, out Dictionary<string, TimeSpan?> matches) {
            var options = new TimeSpanParserOptions();
            options.UncolonedDefault = uncolonedDefault;
            options.ColonedDefault = colonedDefault;
            return TryParsePrefixed(text, prefixes, options, out matches);
        }
        public static bool TryParsePrefixed(string text, string[] prefixes, out Dictionary<string, TimeSpan?> matches) {
            return TryParsePrefixed(text, prefixes, null, out matches);
        }

        public static bool TryParsePrefixed(string text, string[] prefixes, TimeSpanParserOptions options, out Dictionary<string, TimeSpan?> matches) {
            //string[] prefixes = new string[] { "for", "in", "delay", "wait" };

            //TODO: rename "prefixes" to "parameter names" or "keys" or something

            if (options == null) {
                options = new TimeSpanParserOptions();
            }

            matches = new Dictionary<string, TimeSpan?>();


            //e.g. string pattern = @"\b(for|in|delay|now|wait)\b"; 
            //TODO: replace spaces with any amount of whitespace (currently @"\ ") e.g. "[\s.']*" (spaces dots or ' ) // perhaps do replacements first to make it easier, e.g. replace "
            StringBuilder pattern = new StringBuilder();
            pattern.Append(@"\b("); // must be in (brackets) to be included in results of regex split
            //pattern.Append(@"?<keyword>"); // name group
            pattern.Append(string.Join("|", prefixes.Select(prefix => Regex.Escape(prefix))));
            pattern.Append(@")\b");

            var regex = new Regex(pattern.ToString(), RegexOptions.IgnoreCase);
            string[] parts = regex.Split(text);
            //Console.WriteLine("pattern: " + pattern.ToString());
            int nonkeywordCounter = 0;
            string currentPrefix = null;

            try {
                for (int i = 0; i < parts.Length; i++) {
                    var part = parts[i];
                    var lc = part.ToLowerInvariant();
                    if (prefixes.Contains(lc)) {
                        matches[lc] = null;
                        currentPrefix = lc;

                    } else {
                        if (DoParseMutliple(part, out TimeSpan[] timespans, options)) {
                            for (int j = 0; j < timespans.Length; j++) {
                                string prefix = currentPrefix ?? nonkeywordCounter++.ToString();

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
