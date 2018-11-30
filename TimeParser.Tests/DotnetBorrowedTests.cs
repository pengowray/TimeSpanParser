using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSpanParserUtil.Tests {
    /// <summary>
    /// Tests borrowed from Microsoft's own tests of TimeSpan.Parse.
    /// </summary>
    class DotnetBorrowedTests {

        //via MIT licensed: 
        //https://github.com/dotnet/corefx/blob/master/src/System.Runtime/tests/System/TimeSpanTests.netcoreapp.cs
        //https://github.com/dotnet/corefx/blob/master/src/System.Runtime/tests/System/TimeSpanTests.cs

        //see also:
        //Utf8Parser.TimeSpan.cs (class Utf8Parser) TryParse() namespace System.Buffers.Text
        //https://github.com/Azure/azure-libraries-for-java/blob/master/azure-mgmt-servicebus/src/test/java/com/microsoft/azure/management/servicebus/TimeSpanTests.java (MIT License)
        //https://github.com/mono/mono/blob/master/mcs/class/corlib/Test/System/TimeSpanTest.cs (especially "valid" / "this should be valid" ?)
        //https://github.com/mono/mono/blob/master/mcs/class/referencesource/mscorlib/system/timespan.cs
        //(dupe, mono fork) https://github.com/YichaoLee/U3d-mono/blob/master/mcs/class/corlib/Test/System/TimeSpanTest.cs 
        //(dupe, mono fork) https://github.com/davefmurray/playscript-mono/blob/master/mcs/class/corlib/Test/System/TimeSpanTest.cs
        //https://github.com/OData/odata.net/blob/master/test/FunctionalTests/Microsoft.OData.Edm.Tests/Csdl/EdmValueParserTests.cs

        //see also (non-microsoft):
        //https://github.com/Spodii/netgore/blob/master/netgore/branches/sfmlupdatev2/NetGore.Tests/NetGore/DurationParserTests.cs
        //https://github.com/mconti/CorsoInformatica/blob/master/Librerie/PropertyTools-master/Source/PropertyTools.Wpf.Tests/Helpers/TimeSpanParserTests.cs
        //https://github.com/sumanbabum/TestAZureBoards/blob/master/snippets/csharp/VS_Snippets_CLR/conceptual.timespan.custom/cs/f_specifiers1.cs
        //https://github.com/LetsGoRafting/dbatools/blob/master/bin/projects/dbatools/dbatools/Utility/DbaTimeSpan.cs (ParseTimeSpan)
        //https://github.com/graphql-dotnet/conventions/blob/master/test/Tests/Adapters/Types/TimeSpanGraphTypeTests.cs
        //https://github.com/AndrewGaspar/Podcasts.Dom/blob/master/Podcasts.Dom.Test/ParsingTests.cs 
        //https://github.com/toasty-toast/time-tracker/blob/master/TimeTracker.Tests/Models/TimeEntryParserTests.cs

        //see also, non C# :
        // js) https://github.com/edvinv/time-span/blob/master/test/time-span-test.ts
        // js) https://github.com/moment/moment/blob/2e2a5b35439665d4b0200143d808a7c26d6cd30f/src/test/moment/duration.js
        // js) https://github.com/tolu/ISO8601-duration/tree/master/test
        // js) https://github.com/mstum/TimeSpan.js/blob/master/TimeSpan-1.2.js#L245
        // java) https://github.com/enlo/jmt-projects/blob/master/jmt-core/src/test/java/info/naiv/lab/java/jmt/datetime/TimeSpanTest.java
        // java) https://github.com/jtux270/translate/blob/master/ovirt/backend/manager/modules/compat/src/test/java/org/ovirt/engine/core/compat/TimeSpanTest.java
        // c) https://github.com/sujithshankar/systemd-work/blob/master/src/test/test-time.c
        // c) https://github.com/katarn85/mortimer/blob/master/systemd-210/src/test/test-time.c
        // c) https://github.com/chombourger/android-udev/blob/master/dist/src/test/test-time.c
        // go) https://github.com/Shelnutt2/go-benchmark-ParseDuration/blob/master/ParseDuration_test.go
        // python) https://github.com/oleiade/durations
        // python) https://github.com/wroberts/pytimeparse
        // python) https://github.com/bear/parsedatetime/blob/master/tests/TestDelta.py
        // python) https://github.com/ssj5638/analysis_fb2/blob/master/venv/Lib/site-packages/pandas/tests/scalar/timedelta/test_construction.py + TestUnits.py

        /*
          
    public static IEnumerable<object[]> Parse_ValidWithOffsetCount_TestData() {
        foreach (object[] inputs in Parse_Valid_TestData()) {
            yield return new object[] { inputs[0], 0, ((string)inputs[0]).Length, inputs[1], inputs[2] };
        }

        yield return new object[] { "     12:24:02      ", 5, 8, null, new TimeSpan(0, 12, 24, 2, 0) };
        yield return new object[] { "     12:24:02      ", 6, 7, null, new TimeSpan(0, 2, 24, 2, 0) };
        yield return new object[] { "     12:24:02      ", 6, 6, null, new TimeSpan(0, 2, 24, 0, 0) };
        yield return new object[] { "12:24:02.01", 0, 8, CultureInfo.InvariantCulture, new TimeSpan(0, 12, 24, 2, 0) };
        yield return new object[] { "1:1:1.00000001", 0, 7, CultureInfo.InvariantCulture, new TimeSpan(1, 1, 1) };
        yield return new object[] { "1:1:.00000001", 0, 6, CultureInfo.InvariantCulture, new TimeSpan(36600000000) };
        yield return new object[] { "24:00:00", 1, 7, null, new TimeSpan(4, 0, 0) };
    }

    [Theory]
    [MemberData(nameof(Parse_ValidWithOffsetCount_TestData))]
    public static void Parse_Span(string inputString, int offset, int count, IFormatProvider provider, TimeSpan expected) {
        ReadOnlySpan<char> input = inputString.AsSpan(offset, count);
        TimeSpan result;

        Assert.Equal(expected, TimeSpan.Parse(input, provider));
        Assert.True(TimeSpan.TryParse(input, provider, out result));
        Assert.Equal(expected, result);

        // Also negate
        if (!char.IsWhiteSpace(input[0])) {
            input = ("-" + inputString.Substring(offset, count)).AsSpan();
            expected = -expected;

            Assert.Equal(expected, TimeSpan.Parse(input, provider));
            Assert.True(TimeSpan.TryParse(input, provider, out result));
            Assert.Equal(expected, result);
        }
    }
    */

    }
}
