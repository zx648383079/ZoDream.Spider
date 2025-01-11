using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Events
{
    public delegate void WebViewResponseReceivedEventHandler(IWebView sender, IWebViewRequest request, IWebViewResponse response);

    public delegate void WebViewDocumentChangedEventHandler(IWebView sender, string uri);
}
