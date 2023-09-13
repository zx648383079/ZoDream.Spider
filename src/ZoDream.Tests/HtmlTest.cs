using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ZoDream.Shared.Http;
using ZoDream.Shared.Utils;

namespace ZoDream.Tests
{
    [TestClass]
    public class HtmlTest
    {
        // [TestMethod]
        public async void TestMatch()
        {
            var client = new Client();
            var html = await client.GetAsync("");
            Assert.IsTrue(html != null && html.IndexOf("") > 0);
        }

        [TestMethod]
        public void TestHost()
        {
            var url = "//job.zodream.cn";
            Assert.AreEqual(Html.MatchHost(url), "job.zodream.cn");
        }
    }
}
