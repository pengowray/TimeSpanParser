**TimeSpanParser** parses human-written and natural language time span strings. For example:

`TimeSpanParser.Parse("5 mins")`

`TimeSpan.Parse("00:05:00")` returns the same result with C#'s built-in parser.

TimeSpanParser accepts a number of formats, such as

`TimeSpanParser.Parse("2h10m58s")` == `TimeSpanParser.Parse("2:10:58")` == `TimeSpanParser.Parse("2 hours, 10 minutes 58 seconds")`

See [QuickGuide.cs](https://github.com/quole/TimeSpanParser/blob/master/TimeParser.Tests/QuickGuide.cs) for more examples and options. This is like a tutorial in UnitTest format, to be sure the examples are typo free.

See [WildTests.cs](https://github.com/quole/TimeSpanParser/blob/master/TimeParser.Tests/WildTests.cs) for examples of timespans found in the wild for more odd examples.

**Features**
* Does flexible user input for timespan (duration) parsing.
* Accepts expressions like "1h30m" or "1:30:00" or "1 hour 30 minutes"
* Also accepts whatever .NET's TimeSpan.Parse() will accept (US and common formats only for now)
* Accepts a variety of unusual inputs like "-1.5hrs", "3e1000 seconds", and even strings like "３．１７：２５：３０．５" [in case you missed it, the strangest thing about that last example is it uses a dot rather than a colon to separate the days from the hours, which is oddly Microsoft's default way of [outputting TimeSpans ("c")](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings). Also it's using fullwidth Unicode characters.]
* Can round-trip English expressions from [TimeSpan.Humanizer()](https://github.com/Humanizr/Humanizer)
* Sane, permissive defaults for unambiguous input, but many options to fine tune if you really want.
* By changing the default options, you can change the expected units, e.g. you can have it treat a bare input of "5" as "5 minutes" instead of throwing an exception; or treat "3:22" as 3m22s instead of the default which would be the equivalent of 3h22m.
* Will parse "0 years" and "0 months" unambiguously, as such inputs won't change in meaning even on a leap day during a leap year.
* Many, many unit tests—many of which pass!
* Returns a [`TimeSpan`](https://docs.microsoft.com/en-us/dotnet/api/system.timespan?view=netcore-2.1) (struct), so shares its limitations — min/max value: [approx ±10 million days](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.maxvalue?view=netcore-2.1). Smallest unit: 100 nanoseconds (aka "[1 microsoft tick](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.ticks?view=netcore-2.1)". There are 10 million [ticks per second](https://docs.microsoft.com/en-us/dotnet/api/system.timespan.tickspersecond?view=netcore-2.1)).

**Help needed**
PRs welcome. Especially if you find an input TimeSpanParser.Parse(string) does not deal correctly compared to `TimeSpan.Parse(string)` does, then please create an issue or add a unit test.

[To-do list](https://github.com/quole/TimeSpanParser/wiki/Todo) (Wiki)

See also:
* Quole's post with the [original concept and motivation](https://github.com/Humanizr/Humanizer/issues/691) for TimeSpanParser.
