namespace ZoDream.Shared.Interfaces
{
    public interface IFormInput
    {
        public string Name { get; }
        public string Label { get; }
        public string Tip { get; }

        public bool TryParse(ref object input);
    }
}
