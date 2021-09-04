using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Providers;
using ZoDream.Shared.Spiders.Containers;

namespace ZoDream.Tests
{
    [TestClass]
    public class RuleTest
    {
        [TestMethod]
        public void TestOne()
        {
            var container = new SpiderContainer(null);
        }

        [TestMethod]
        public void TestPlugin()
        {
            var provider = new RuleProvider();
            provider.LoadDll("");
            Assert.IsTrue(provider.AllPlugin().Count() == 1);
        }
    }
}
