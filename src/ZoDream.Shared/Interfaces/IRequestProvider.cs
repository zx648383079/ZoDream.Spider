namespace ZoDream.Shared.Interfaces
{
    public interface IRequestProvider
    {
        public bool SupportTask { get; }

        public IHttpRequest Getter();

        public IDownloadRequest Downloader();
    }

    public interface IWebViewProvider
    {
        public IWebView AsWebView();
    }
}
