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
            if (HeaderItems.Count > 0)
            {
                writer.WriteLine("[HEADER]");
                WriteHeader(writer);
                writer.WriteLine();
            }
            if (HostItems.Count > 0)
            {
                writer.WriteLine("[HOST]");
                WriteHost(writer);
                writer.WriteLine();
            }
            if (ProxyItems.Count > 0)
            {
                writer.WriteLine("[PROXY]");
                WriteProxy(writer);
                writer.WriteLine();
            }
            if (EntryItems.Count > 0)
            {
                writer.WriteLine("[ENTRY]");
                WriteEntry(writer);
                writer.WriteLine();
            }
            if (RuleItems.Count > 0)
            {
                writer.WriteLine("[RULE]");
                WriteRule(writer);
                writer.WriteLine();
            }
            if (false)
            {
                writer.WriteLine("[URL]");
                WriteUrl(writer);
                writer.WriteLine();
            }
        }

        private void WriteHeader(StreamWriter writer)
        {
            foreach (var item in HeaderItems)
            {
                writer.WriteLine($"{item.Name}: {item.Value}");
            }
        }
        private void WriteHost(StreamWriter writer)
        {
            foreach (var item in HostItems)
            {
                writer.WriteLine($"{item.Ip}      {item.Host}");
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
