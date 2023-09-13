using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Shared.Routes;
using ZoDream.Spider.Pages;
using ZoDream.Spider.ViewModels;

namespace ZoDream.Spider
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            RegisterRoute();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length > 0)
            {
                await ViewModel.OpenProjectAsync(e.Args[0]);
                ShellManager.GoToAsync("home");
                return;
            }
            ShellManager.GoToAsync("startup");
        }

        private void RegisterRoute()
        {
            ShellManager.RegisterRoute("home", typeof(HomePage));
            ShellManager.RegisterRoute("startup", typeof(StartupPage));
            ShellManager.RegisterRoute("setting", typeof(SettingPage));
            ShellManager.RegisterRoute("quickly/site", typeof(QuicklySitePage));
            ShellManager.RegisterRoute("quickly/single", typeof(QuicklySinglePage));
            ShellManager.RegisterRoute("quickly/custom", typeof(QuicklyCustomPage));
            ShellManager.RegisterRoute("entry", typeof(EntryPage));
            ShellManager.RegisterRoute("proxy", typeof(ProxyPage));
            ShellManager.RegisterRoute("rule", typeof(RulePage));
            ShellManager.RegisterRoute("header", typeof(HeaderPage));
        }

        public static AppViewModel ViewModel { get; } = new();
    }
}
