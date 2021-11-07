using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Http;

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
    }
}
