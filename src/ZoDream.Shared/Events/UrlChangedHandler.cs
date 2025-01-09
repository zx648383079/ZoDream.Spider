using ZoDream.Shared.Models;

namespace ZoDream.Shared.Events
{
    public delegate void UrlChangedEventHandler(UrlChangedEventArgs args);

    public class UrlChangedEventArgs
    {
        public UrlChangedEventArgs()
        {
            
        }

        public UrlChangedEventArgs(bool isAdd)
        {
            IsAdd = isAdd;
        }

        public UrlChangedEventArgs(UriItem url, bool isAdd): this(url)
        {
            IsAdd = isAdd;
        }

        public UrlChangedEventArgs(UriItem url)
        {
            Uri = url;
        }

        public UrlChangedEventArgs(UriItem url, string message): this(url)
        {
            Message = message;
        }

        public UriItem? Uri;

        public bool IsAdd { get; private set; }

        public string Message { get; private set; } = string.Empty;
    }
}
