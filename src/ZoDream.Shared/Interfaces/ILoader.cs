using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Interfaces
{
    public interface ILoader
    {
        // public string Serializer();
        public void Serializer(StreamWriter writer);

        // public void Deserializer(string value);
        public void Deserializer(StreamReader reader);
    }
}
