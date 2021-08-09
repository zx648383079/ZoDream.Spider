using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Http;

namespace ZoDream.Tests
{
    [TestClass]
    public class HtmlTest
    {
        [TestMethod]
        public void TestMatch()
        {
            var client = new Client();
            var html = client.Get("https://www.ly990.com/10_10398/10756807.html");
            Assert.IsTrue(html.IndexOf("½­¼ÒÊÙÑç") > 0);
        }
    }
}
