using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Local;

namespace ZoDream.Tests
{
    [TestClass]
    public class RuleTest
    {
        [TestMethod]
        public void TestOne()
        {
            // var container = new SpiderContainer(null);
        }

        [TestMethod]
        public void TestPlugin()
        {
            var fileName = "${path}.txt";
            var res = Regex.Replace(fileName, @"\$\{([a-zA-Z0-9_]+)\}", match =>
            {
                switch (match.Groups[1].Value)
                {
                    case "host":
                        return "127.0.0.1";
                    case "path":
                        return "/aa/";
                    default:
                        return match.Value;
                }
            });
            Assert.AreEqual(res, "/aa/.txt");
        }
    }
}
