using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Form
{
    public class File : IFormInput
    {
        public string Name { get; private set; }

        public string Label { get; private set; }

        public bool Required { get; private set; }
        public bool IsSave { get; private set; }
        public bool IsFolder { get; private set; }
        public string Tip { get; private set; } = string.Empty;

        public bool TryParse(ref object input)
        {
            return true;
        }

        public File(string name, string label, bool required)
        {
            Name = name;
            Label = label;
            Required = required;
        }

        public File(string name, string label, 
            bool required, bool isSave, bool isFolder): this(name, label, required)
        {
            IsSave = isSave;
            IsFolder = isFolder;
        }
    }
}
