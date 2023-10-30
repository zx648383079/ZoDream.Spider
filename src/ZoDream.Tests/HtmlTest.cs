using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
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
            var html = "charset=utf-8,daaaa";
            var match = Regex.Match(html, @"charset\b\s*=\s*""*([\da-zA-Z\-]*)");
            Assert.AreEqual(match.Groups[1].Value, "utf-8");
        }


        [TestMethod]
        public void TestRelative()
        {
            var baseFile = "a\\css\\index.html";
            var fileName = "a\\js\\css.js";
            Assert.AreEqual(Html.PathToRelativeUri(baseFile, fileName), "../js/css.js");
        }
    }
}
