using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.JsObjects
{
    public interface ISpiderBridge
    {

        public void Callback(string content);
    }
}
