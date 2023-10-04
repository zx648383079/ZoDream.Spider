using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loaders;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Spider.Pages;
using ZoDream.Spider.Plugins;

namespace ZoDream.Spider.ViewModels
{
    public class AppViewModel : IDisposable
    {

        public AppViewModel()
        {
            Plugin = new PluginLoader();
            _ = LoadOptionAsync();
            Task.Factory.StartNew(() => {
                Plugin.Load(AppDomain.CurrentDomain.BaseDirectory);
            });
        }

        public AppOption Option { get; private set; } = new();

        public ProjectLoader? Project { get; private set; }

        public IPluginLoader Plugin;

        private Window? _baseWindow;
        public Window BaseWindow {
            set {
                _baseWindow = value;
            }
        }

        public string Title {
            set {
                if (_baseWindow is null)
                {
                    return;
                }
                _baseWindow.Title = $"{value} - ZoDream Spider";
            }
        }

        public string Version {
            get {
                var val = Application.ResourceAssembly?.GetName()?.Version?.ToString();
                return string.IsNullOrEmpty(val) ? "-" : val;
            }
        }


        /// <summary>
        /// UI线程.
        /// </summary>
        public Dispatcher DispatcherQueue => Application.Current.Dispatcher;

        private BrowserDebugView? browserRequest;

        public BrowserDebugView BrowserRequest {
            get {
                if (browserRequest == null)
                {
                    browserRequest = new BrowserDebugView();
                    browserRequest.Closed += (s, arg) => {
                        browserRequest = null;
                    };
                }
                if (!browserRequest.IsVisible)
                {
                    DispatcherQueue.Invoke(() => {
                        browserRequest.Show();
                    });
                }
                return browserRequest;
            }
        }
        public async Task<AppOption> LoadOptionAsync()
        {
            var option = await AppData.LoadAsync<AppOption>();
            if (option != null)
            {
                Option = option;
            }
            return Option;
        }

        public async Task SaveOptionAsync()
        {
            await AppData.SaveAsync(Option);
        }

        public async Task OpenProjectAsync(string path)
        {
            Project = new ProjectLoader(path);
            await Project.LoadAsync();
        }

        public async Task CreateProjectAsync(string path, ProjectLoader loader)
        {
            Project = loader;
            Project.FileName = path;
            await Project.SaveAsync();
        }

        public void Dispose()
        {
            browserRequest?.Close();
        }
    }
}
