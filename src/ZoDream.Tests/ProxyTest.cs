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
        public void TestRequest()
        {
            var item = new ProxyItem("61.163.32.88:3128");
            Assert.IsTrue(HttpProxy.Test(item));
        }
    }
}
