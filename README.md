**TimeSpanParser** parses human-written and natural language time span strings. For example:

`TimeSpanParser.Parse("5 mins")`

`TimeSpan.Parse("00:05:00")` returns the same result with C#'s built-in parser.

TimeSpanParser accepts a number of formats, such as

`TimeSpanParser.Parse("2h10m58s")` == `TimeSpanParser.Parse("2:10:58")` == `TimeSpanParser.Parse("2 hours, 10 minutes 58 seconds")`

See [QuickGuide.cs](https://github.com/quole/TimeSpanParser/blob/master/TimeParser.Tests/QuickGuide.cs) for more examples and options. This is like a tutorial in UnitTest format, to be sure the examples are typo free.

See [WildTests.cs](https://github.com/quole/TimeSpanParser/blob/master/TimeParser.Tests/WildTests.cs) for examples of timespans found in the wild for more odd examples.

Features:
* Does flexible user input for timespan (duration) parsing.
* Accepts expressions like "1h30m" or "1:30:00" or "1 hour 30 minutes"
* Also accepts whatever .NET's TimeSpan.Parse() will accept (US and common formats only for now)
* Accepts a variety of unusual inputs like "-1.5hrs", "3e1000 seconds", and even strings like "３．１７：２５：３０．５" [in case you missed it, the strangest thing about that last example is it uses a dot rather than a colon to separate the days from the hours, which is oddly Microsoft's default way of [outputting TimeSpans ("c")](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings).]
* Can round-trip English expressions from [TimeSpan.Humanizer()](https://github.com/Humanizr/Humanizer)
* Sane, permissive defaults for unambiguous input, but many options to fine tune if you really want.
* By changing the default options, you can change the expected units, e.g. you can have it treat a bare input of "5" as "5 minutes" instead of throwing an exception; or treat "3:22" as 3m22s instead of the default which would be the equivalent of 3h22m.
* Will parse "0 years" and "0 months" unambiguous, as such inputs won't change in meaning even on a leap day during a leap year.
* Many, many unit tests—many of which pass!
* Only returns a [`TimeSpan`](https://docs.microsoft.com/en-us/dotnet/api/system.timespan?view=netcore-2.1) (struct), so shares its limitations — min/max value: [approx ±29,000 years](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.maxvalue?view=netcore-2.1). Smallest unit: 100 nanoseconds (aka "[1 microsoft tick](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.ticks?view=netcore-2.1)". There are 10 million ticks per second).

"Not Yet Implemented" (NYI) and "Out of Scope" (OoS) features:
* OoS: relative time support — e.g. "until next thursday" is not supported and currently no plans to add support.
* NYI: Ambiguous units—namely months and years—as they cannot be unambiguously translated into days or seconds so would require special options for how to handle them, at least one of which would entail relative time support. So not adding support unless all the options for handling months and years are available.
* NYI: Non-English language support, but there is some rudimentary localization support in the parser's number handling.
* NYI: speed and memory optimization. No testing has been done so far.
* OoS: Stable parsing. While still in development, TimeSpanParser is not guaranteed to have identical behavior between versions. Options may change too. [TODO: better instructions]
* OoS: Literal timespan declarations. Although you can do it easily, TimeSpanParser is not designed for parsing timespan declarations embedded in your code, but rather for parsing user input. You may prefer to use the built-in static methods from TimeSpan if you want to stick a timespan in your code.
* NYI: Perfect backwards compatibility with TimeSpan.Parser. It's pretty close.
* OoS: Outputting anything other than TimeSpan objects.
* NYI: SI prefixes (e.g. kiloseconds)
* NYI: Numbers as words (e.g. "five minutes", "one second", "an hour")
* NYI: [ISO 8601 time interval ](https://en.wikipedia.org/wiki/ISO_8601#Time_intervals) support

PRs welcome for any of these missing features.

Road map (short/medium term):
* Raise identical exceptions to TimeSpan.Parse(), and do it in identical scenarios. (e.g. over/underflows)
* Internationalization — Cover all the cultures which TimeSpan.Parse() does. Should be straight forward as there's only small variations: the decimal separator in the seconds, which can be a period (.) or comma (,) or slash (/); and the separator between the days and hours: a period (.) in the common "c" format, and a colon (:) in all others. However, we support more than just TimeSpan.Parse()'s input. Also note if the localization is set to French (for example), the "common" format must still pass too.
* Full tests coverage for all parser settings (and get them all to pass)
* Better handling of unknown tokens (largely ignored currently)
* Todo: make UnitTests use a default localization for when localization is added

Later road map:
* Find timespans within a larger text and return their locations

If you find an input TimeSpanParser.Parse(string) does not deal with which `TimeSpan.Parse(string)` does, then please create an issue or add a unit test.

If you'd like to work on creating a localization framework to allow parsing of strings from other languages, your PR would be appreciated.

See also: Quole's post with the [original concept and motivation](https://github.com/Humanizr/Humanizer/issues/691) for TimeSpanParser.
