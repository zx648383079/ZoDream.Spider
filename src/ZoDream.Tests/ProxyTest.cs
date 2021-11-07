using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZoDream.Shared.Http;
using ZoDream.Shared.Models;

namespace ZoDream.Tests
{
    [TestClass]
    public class ProxyTest
    {
        [TestMethod]
        public void TestParse()
        {
            var item = new ProxyItem("125.72.106.160:3256");
            Assert.IsTrue(item.Port == 3256);
        }

        [TestMethod]
        public async void TestRequest()
        {
            var item = new ProxyItem("118.190.244.234:3128");
            Assert.IsTrue(await HttpProxy.TestAsync(item));
        }
    }
}
