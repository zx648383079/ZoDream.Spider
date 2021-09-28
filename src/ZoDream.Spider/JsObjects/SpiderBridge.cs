using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.JsObjects
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class SpiderBridge: ISpiderBridge
    {

        public event ContentReadyEventHandler? ContentReady;

        public void Callback(string content)
        {
            ContentReady?.Invoke(content);
            Debug.WriteLine("js callback:" + content);
        }
    }

    public delegate void ContentReadyEventHandler(string html);
}
