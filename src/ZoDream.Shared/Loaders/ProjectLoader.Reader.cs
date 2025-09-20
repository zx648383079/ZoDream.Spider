using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Loaders
{
    public partial class ProjectLoader
    {

        private void Read(StreamReader reader)
        {
            if (reader == null)
            {
                return;
            }
            string? line;
            var regex = new Regex(@"^\[(\w+)\]$");
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (!regex.IsMatch(line))
                {

                    continue;
                }
                var tag = regex.Match(line).Groups[1].Value.ToUpper();
                switch (tag)
                {
                    case "OPTION":
                        ReadOption(reader);
                        break;
                    case "HEADER":
                        ReadHeader(reader);
                        break;
                    case "PROXY":
                        ReadProxy(reader);
                        break;
                    case "HOST":
                        ReadHost(reader);
                        break;
                    case "RULE":
                        ReadRule(reader);
                        break;
                    case "ENTRY":
                        ReadEntry(reader);
                        break;
                    case "URL":
                        ReadUrl(reader);
                        break;
                    default:
                        break;
                }
            }
        }

        private void ReadHeader(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                var args = line.Split(new char[] { ':' }, 2);
                var name = args[0].Trim();
                if (!string.IsNullOrWhiteSpace(args[1]))
                {
                    HeaderItems.Add(new HeaderItem(name, args[1].Trim()));
                }
            }
        }

        private void ReadHost(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                var args = line.Trim().Split(new char[] { ' ' }, 2);
                var ip = args[0].Trim();
                var host = args[1].Trim();
                if (!string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(host))
                {
                    HostItems.Add(new HostItem(host, ip));
                }
            }
        }

        private void ReadProxy(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                ProxyItems.Add(line);
            }
        }

        private void ReadRule(StreamReader reader)
        {
            var sb = new StringBuilder();
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                sb.AppendLine(line);
            }
            if (sb.Length < 1)
            {
                return;
            }
            var items = JsonSerializer.Deserialize<List<RuleGroupItem>>(sb.ToString());
            if (items is not null)
            {
                RuleItems = items;
            }
        }
        private void ReadEntry(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line) || EntryItems.Contains(line))
                {
                    return;
                }
                EntryItems.Add(line);
            }
        }
        private void ReadUrl(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                var args = line.Split(';');
                //if (args.Length == 1)
                //{
                //    Add(line);
                //    continue;
                //}
                //if (args.Length == 2)
                //{
                //    Add(args[1], UriType.Html, (UriCheckStatus)Enum.Parse(typeof(UriCheckStatus), args[0]));
                //    continue;
                //}
                //Add(args[2], (UriType)Enum.Parse(typeof(UriType), args[1]), (UriCheckStatus)Enum.Parse(typeof(UriCheckStatus), args[0]));
            }
        }

        private void ReadOption(StreamReader reader)
        {
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                var args = line.Split(new char[] { ':' }, 2);
                var name = args[0].Trim();
                switch (name)
                {
                    case nameof(ParallelCount):
                        ParallelCount = int.Parse(args[1].Trim());
                        break;
                    case nameof(RetryCount):
                        RetryCount = int.Parse(args[1].Trim());
                        break;
                    case nameof(RetryTime):
                        RetryTime = int.Parse(args[1].Trim());
                        break;
                    case nameof(TimeOut):
                        TimeOut = int.Parse(args[1].Trim());
                        break;
                    case nameof(UseBrowser):
                        UseBrowser = args[1].Trim().ToUpper() == "Y";
                        break;
                    case nameof(Workspace):
                        Workspace = args[1].Trim();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
