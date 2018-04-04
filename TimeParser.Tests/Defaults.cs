using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests
{
    [TestClass()]
    public sealed class Defaults
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context) {
            string culture = "en-US";
            //string culture = "fr-FR";
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;


        }
    }
}
