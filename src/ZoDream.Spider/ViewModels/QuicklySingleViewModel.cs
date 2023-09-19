using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZoDream.Shared.Loaders;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class QuicklySingleViewModel : BindableBase
    {

        public QuicklySingleViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
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

        private bool withResource;

        public bool WithResource {
            get => withResource;
            set => Set(ref withResource, value);
        }


        public ICommand BackCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        private void TapBack(object? _)
        {
            ShellManager.BackAsync();
        }

        private async void TapConfirm(object? _)
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
            var project = new ProjectLoader(picker.FileName) { Workspace = Workspace };
            project.EntryItems.Add(InputEntry);
            project.RuleItems.Add(new RuleGroupItem()
            {
                MatchType = WithResource ? RuleMatchType.Page : RuleMatchType.None,
                MatchValue = "page",
                Rules =
                {
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
