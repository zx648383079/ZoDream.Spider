using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Http;

namespace ZoDream.Tests
{
    [TestClass]
    public class HtmlTest
    {
        // [TestMethod]
        public void TestMatch()
        {
            var client = new Client();
            var html = client.Get("");
            Assert.IsTrue(html != null && html.IndexOf("") > 0);
        }
    }
}
