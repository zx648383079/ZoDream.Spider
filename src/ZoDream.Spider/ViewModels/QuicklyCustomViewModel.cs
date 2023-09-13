using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class QuicklyCustomViewModel: BindableBase
    {

        public QuicklyCustomViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            ConfirmCommand = new RelayCommand(TapConfirm);
            AddCommand = new RelayCommand(TapAdd);
            ProxyDialogConfirmCommand = new RelayCommand(TapProxyDialogConfirm);
            ProxyDeleteCommand = new RelayCommand(TapProxyDelete);
            HeaderDeleteCommand = new RelayCommand(TapHeaderDelete);
            HeaderDialogConfirmCommand = new RelayCommand(TapHeaderDialogConfirm);
            RuleDeleteCommand = new RelayCommand(TapRuleDelete);
            RuleDialogConfirmCommand = new RelayCommand(TapRuleDialogConfirm);
            RuleAddCommand = new RelayCommand(TapRuleAdd);
            RuleConfirmCommand = new RelayCommand(TapRuleConfirm);
            RuleDeleteCommand = new RelayCommand(TapRuleDelete);
            RuleDownCommand = new RelayCommand(TapRuleDown);
            RuleEditCommand = new RelayCommand(TapRuleEdit);
            RuleUpCommand = new RelayCommand(TapRuleUp);
            PanelConfirmCommand = new RelayCommand(TapPanelConfirm);
            GroupDialogConfirmCommand = new RelayCommand(TapGroupDialogConfirm);
            GroupEditCommand = new RelayCommand(TapGroupEdit);
            GroupDeleteCommand = new RelayCommand(TapGroupDelete);
            PluginItems = App.ViewModel.Plugin.All();
        }

        private string[] stepItems = new string[] { "配置规则", 
            "设置请求头", "配置代理", "请求网址、保存地址" };

        public string[] StepItems {
            get => stepItems;
            set => Set(ref stepItems, value);
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


        private int stepIndex;

        public int StepIndex {
            get => stepIndex;
            set {
                Set(ref stepIndex, value);
                OnPropertyChanged(nameof(ConfirmText));
                OnPropertyChanged(nameof(ConfirmIcon));
                OnPropertyChanged(nameof(BackText));
            }
        }


        public string ConfirmText => StepIndex < StepItems.Length - 1 ? "下一步" : "确认";
        public string BackText => StepIndex > 0 ? "上一步" : "返回";
        public string ConfirmIcon => StepIndex < StepItems.Length - 1 ? "\uE111" : "\uE10B";


        public ICommand BackCommand { get; private set; }
        public ICommand AddCommand { get; private set; }

        public ICommand ConfirmCommand { get; private set; }

        private void TapBack(object? _)
        {
            if (StepIndex > 0)
            {
                StepIndex--;
                return;
            }
            ShellManager.BackAsync();
        }

        private void TapConfirm(object? _)
        {
            if (StepIndex >= StepItems.Length - 1)
            {
                TapFinish();
                return;
            }
            if (StepIndex == 0 && GroupItems.Count == 0)
            {
                MessageBox.Show("规则必须设置", "提示");
                return;
            }
            StepIndex++;
        }

        private void TapAdd(object? _)
        {
            switch (StepIndex)
            {
                case 0:
                    GroupDialogVisible = true;
                    break;
                case 1:
                    HeaderDialogVisible = true;
                    break;
                case 2:
                    ProxyDialogVisible = true;
                    break;
                default:
                    break;
            }
        }

        private async void TapFinish()
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
                RuleItems = GroupItems.ToList(),
                HeaderItems = HeaderItems.ToList(),
                ProxyItems = ProxyItems.Select(i => i.Url).ToList(),
            };
            project.EntryItems.Add(InputEntry);
            await App.ViewModel.CreateProjectAsync(picker.FileName, project);
            ShellManager.GoToAsync("home");
        }

    }
}
