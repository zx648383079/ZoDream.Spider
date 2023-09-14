using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ZoDream.Spider;

namespace ZoDream.Shared.Routes
{
    public class ShellService : IDisposable
    {
        private Frame? InnerFrame;
        private readonly Dictionary<string, ShellRoute> Routes = new();

        private readonly Dictionary<string, FrameworkElement> RouteCaches = new();
        private readonly List<string> Histories = new();

        public FrameworkElement? Current { get; private set; }
        public ShellRoute? CurrentRoute { get; private set; }

        public void RegisterRoute(string routeName, Type page)
        {
            if (Routes.ContainsKey(routeName))
            {
                Routes[routeName] = new ShellRoute(routeName, page);
                RouteCaches.Remove(routeName);
            } else
            {
                Routes.Add(routeName, new ShellRoute(routeName, page));
            }
        }

        public void RegisterRoute(string routeName, Type page, Type viewModel)
        {
            if (Routes.ContainsKey(routeName))
            {
                Routes[routeName] = new ShellRoute(routeName, page, viewModel);
                RouteCaches.Remove(routeName);
            }
            else
            {
                Routes.Add(routeName, new ShellRoute(routeName, page, viewModel));
            }
        }

        public void GoToAsync(string routeName, IDictionary<string, object> queries)
        {
            GoToAsync(routeName);
            if (Current is not null && Current.DataContext is IQueryAttributable o)
            {
                o.ApplyQueryAttributes(queries);
            }
        }

        public void GoToAsync(string routeName)
        {
            if (CurrentRoute is not null && CurrentRoute.Name == routeName)
            {
                return;
            }
            Current = null;
            if (!Routes.TryGetValue(routeName, out var route))
            {
                return;
            }
            var i = Histories.IndexOf(routeName);
            if (i >= 0)
            {
                Histories.RemoveRange(i + 1, Histories.Count - i - 1);
            }
            else
            {
                Histories.Add(routeName);
            }
            if (InnerFrame is null)
            {
                CurrentRoute = route;
                return;
            }
            Navigate(route);
        }

        private void Navigate(string routeName)
        {
            if (!Routes.TryGetValue(routeName, out var route))
            {
                return;
            }
            Navigate(route);
        }

        private void Navigate(ShellRoute route)
        {
            if (InnerFrame is null)
            {
                return;
            }
            ExitPage(Current);
            var page = CreatePage(route);
            if (!InnerFrame.Navigate(page))
            {
                return;
            }
            if (page is FrameworkElement o)
            {
                Current = o;
                CurrentRoute = route;
            }
            if (page is Page p)
            {
                App.ViewModel.Title = p.Title;
            }
            if (route.DataContext is not null && Current is not null)
            {
                if (Current.DataContext is null)
                {
                    Current.DataContext = Activator.CreateInstance(route.DataContext);
                }
            }
        }

        public void BackAsync()
        {
            if (Histories.Count <= 1)
            {
                return;
            }
            var i = Histories.Count - 1;
            Histories.RemoveAt(i);
            Navigate(Histories[i - 1]);
        }
        private object? CreatePage(ShellRoute route)
        {
            if (RouteCaches.TryGetValue(route.Name, out var page))
            {
                return page;
            }
            var instance = Activator.CreateInstance(route.Page);
            if (instance is FrameworkElement o)
            {
                RouteCaches.Add(route.Name, o);
            }
            return instance;
        }

        private void ExitPage(FrameworkElement? page)
        {
            if (page is not null && page.DataContext is IExitAttributable o)
            {
                o.ApplyExitAttributes();
            }
        }

        public void Bind(Frame frame)
        {
            InnerFrame = frame;
            if (frame is null || CurrentRoute is null)
            {
                return;
            }
            Navigate(CurrentRoute);
        }

        public void Dispose()
        {
            ExitPage(Current);
            Routes.Clear();
            Histories.Clear();
        }
    }

}
