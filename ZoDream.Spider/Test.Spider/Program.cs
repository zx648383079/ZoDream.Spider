using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpHelper http;
            switch (args.Length)
            {
                case 0:
                    Console.WriteLine("请输入网址或保存网址的文件路径，第二个参数为保存文件夹路径！");
                    break;
                case 1:
                    http = new HttpHelper(args[0]);
                    http.Start();
                    Console.WriteLine("运行中。。。");
                    break;
                case 2:
                default:
                    http = new HttpHelper(args[0], args[1]);
                    http.Start();
                    Console.WriteLine("运行中。。。");
                    break;
            }
            Console.ReadKey();
        }

        
    }
}
