using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Tests
{
    [TestClass]
    public class RuleTest
    {
        [TestMethod]
        public void TestMatch()
        {
            var rule = new RuleGroupItem() {
                MatchType = RuleMatchType.Host,
                MatchValue = "zodream.cn",
            };

            Assert.IsFalse(rule.IsAllow("http://zodream.com/", UriType.Html));
        }

        //[TestMethod]
        public void TestOne()
        {
            var ruler = new Spider.BookCrawlerRule.Crawler();
            var text = LocationStorage.ReadAsync("D:\\Desktop\\1.txt").GetAwaiter().GetResult();
            var html = ruler.GetMainContent(text);
            Assert.AreEqual(html.ToString().Length, 10);
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
