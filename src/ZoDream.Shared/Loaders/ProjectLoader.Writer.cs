using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Loaders
{
    public partial class ProjectLoader
    {

        private void Write(StreamWriter writer)
        {
            writer.WriteLine("[OPTION]");
            WriteOption(writer);
            writer.WriteLine();
            writer.WriteLine("[HEADER]");
            WriteHeader(writer);
            writer.WriteLine();
            writer.WriteLine("[PROXY]");
            WriteProxy(writer);
            writer.WriteLine();
            writer.WriteLine("[ENTRY]");
            WriteEntry(writer);
            writer.WriteLine();
            writer.WriteLine("[RULE]");
            WriteRule(writer);
            writer.WriteLine();
            writer.WriteLine("[URL]");
            WriteUrl(writer);
            writer.WriteLine();
        }

        private void WriteHeader(StreamWriter writer)
        {
            foreach (var item in HeaderItems)
            {
                writer.WriteLine($"{item.Name}: {item.Value}");
            }
        }

        private void WriteEntry(StreamWriter writer)
        {
            foreach (var item in EntryItems)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                writer.WriteLine(item);
            }
        }

        private void WriteProxy(StreamWriter writer)
        {
            foreach (var item in ProxyItems)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                writer.WriteLine(item);
            }
        }

        private void WriteRule(StreamWriter writer)
        {
            writer.WriteLine(JsonConvert.SerializeObject(RuleItems));
        }

        private void WriteUrl(StreamWriter writer)
        {
            //foreach (var item in UrlItems)
            //{
            //    if (string.IsNullOrWhiteSpace(item))
            //    {
            //        continue;
            //    }
            //    writer.WriteLine($"{item.Status};{item.Type};{item.Source}");
            //}
        }

        private void WriteOption(StreamWriter writer)
        {
            writer.WriteLine($"count: {MaxCount}");
            writer.WriteLine($"timeout: {TimeOut}");
            writer.WriteLine("browser: " + (UseBrowser ? 'Y' : 'N'));
            writer.WriteLine($"folder: {WorkFolder}");
        }
    }
}
