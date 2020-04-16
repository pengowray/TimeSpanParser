using TimeSpanPidgin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PidginTests {
    [TestClass]
    public class PidginUnitTests {
        [TestMethod]
        public void TestMethod1() {
            System.Console.WriteLine(PidginTimeSpanParser.Trial("6m"));
            System.Console.WriteLine(PidginTimeSpanParser.Trial("3h"));
            System.Console.WriteLine(PidginTimeSpanParser.Trial("8hours"));
        }
    }
}
