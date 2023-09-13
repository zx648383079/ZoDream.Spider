using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ZoDream.Shared.Routes
{
    public static class ShellManager
    {

        private static readonly ShellService Service = new();

        public static void RegisterRoute(string routeName, Type page)
        {
            Service.RegisterRoute(routeName, page);
        }

        public static void RegisterRoute(string routeName, Type page, Type viewModel)
        {
            Service.RegisterRoute(routeName, page, viewModel);
        }

        public static void GoToAsync(string routeName, IDictionary<string, object> queries)
        {
            Service.GoToAsync(routeName, queries);
        }

        public static void GoToAsync(string routeName)
        {
            Service.GoToAsync(routeName);
        }

        public static void BackAsync()
        {
            Service.BackAsync();
        }

        public static void BindFrame(Frame bodyPanel)
        {
            Service.Bind(bodyPanel);
        }

        public static void UnBind()
        {
            Service.Dispose();
        }
    }
}
