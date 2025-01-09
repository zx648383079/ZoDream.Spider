namespace ZoDream.Shared.Interfaces
{
    public interface IWebViewRule
    {
        public void Ready(IWebView loader, ISpiderContainer container);

        public void Destroy(IWebView loader);
    }
}
