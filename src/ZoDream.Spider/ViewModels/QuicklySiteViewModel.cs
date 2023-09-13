using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Loaders;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Utils;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class QuicklySiteViewModel : BindableBase
    {

        public QuicklySiteViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirmAsync);
        }

        private string inputEntry = string.Empty;

        public string InputEntry {
            get => inputEntry;
            set => Set(ref inputEntry, value);
        }

        private string workspace = string.Empty;

        public string Workspace {
            get => workspace;
            set => Set(ref workspace, value);
        }

        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        private void TapBack(object? _)
        {
            ShellManager.BackAsync();
        }

        private async void TapConfirmAsync(object? _)
        {
            if (string.IsNullOrWhiteSpace(InputEntry) || string.IsNullOrWhiteSpace(Workspace))
            {
                MessageBox.Show("网址和保存地址必填", "提示");
                return;
            }
            var picker = new Microsoft.Win32.SaveFileDialog() 
            { 
                Title = "保存项目",
                Filter = "爬虫项目文件|*.sp|所有文件|*.*",
                FileName = "新项目",
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            var project = new ProjectLoader(picker.FileName)
            {
                WorkFolder = Workspace,
            };
            project.EntryItems.Add(InputEntry);
            project.RuleItems.Add(new RuleGroupItem()
            {
                MatchType = RuleMatchType.Host,
                MatchValue = Html.MatchHost(InputEntry),
                Rules =
                {
                    new RuleItem()
                    {
                        Name = "提取默认链接",
                    },
                    new RuleItem()
                    {
                        Name = "本地保存"
                    }
                }
            });
            await App.ViewModel.CreateProjectAsync(picker.FileName, project);
            ShellManager.GoToAsync("home");
        }
    }
}
