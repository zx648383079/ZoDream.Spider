﻿using Microsoft.Web.WebView2.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Utils;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class RuleViewModel: BindableBase, IExitAttributable
    {

        public RuleViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            HomeCommand = new RelayCommand(TapHome);
            SettingCommand = new RelayCommand(TapSetting);
            AddCommand = new RelayCommand(TapAdd);
            EditCommand = new RelayCommand(TapEdit);
            DialogConfirmCommand = new RelayCommand(TapDialogConfirm);
            RuleAddCommand = new RelayCommand(TapRuleAdd);
            RuleConfirmCommand = new RelayCommand(TapRuleConfirm);
            DeleteCommand = new RelayCommand(TapDelete);
            RuleDeleteCommand = new RelayCommand(TapRuleDelete);
            RuleDownCommand = new RelayCommand(TapRuleDown);
            RuleEditCommand = new RelayCommand(TapRuleEdit);
            RuleUpCommand = new RelayCommand(TapRuleUp);
            PanelConfirmCommand = new RelayCommand(TapPanelConfirm);
            Load();
        }

        private bool IsUpdated = false;

        private RuleGroupItem? EditGroup;
        private RuleItem? EditRule;


        private IList<PluginItem> pluginItems = [];

        public IList<PluginItem> PluginItems
        {
            get => pluginItems;
            set => Set(ref pluginItems, value);
        }

        private ObservableCollection<RuleGroupItem> groupItems = [];

        public ObservableCollection<RuleGroupItem> GroupItems {
            get => groupItems;
            set => Set(ref groupItems, value);
        }



        private ObservableCollection<RuleItem> ruleItems = [];

        public ObservableCollection<RuleItem> RuleItems
        {
            get => ruleItems;
            set => Set(ref ruleItems, value);
        }

        private bool dialogVisible;

        public bool DialogVisible {
            get => dialogVisible;
            set => Set(ref dialogVisible, value);
        }

        private bool ruleVisible;

        public bool RuleVisible {
            get => ruleVisible;
            set => Set(ref ruleVisible, value);
        }

        private bool panelVisible;

        public bool PanelVisible {
            get => panelVisible;
            set => Set(ref panelVisible, value);
        }

        private string panelTitle = string.Empty;

        public string PanelTitle {
            get => panelTitle;
            set => Set(ref panelTitle, value);
        }


        private RuleMatchType groupType = RuleMatchType.None;

        public RuleMatchType GroupType {
            get => groupType;
            set => Set(ref groupType, value);
        }

        private string groupName = string.Empty;

        public string GroupName {
            get => groupName;
            set => Set(ref groupName, value);
        }

        private string groupMatchValue = string.Empty;

        public string GroupMatchValue {
            get => groupMatchValue;
            set => Set(ref groupMatchValue, value);
        }


        private int pluginIndex;

        public int PluginIndex {
            get => pluginIndex;
            set {
                Set(ref pluginIndex, value);
                var plugin = value >= 0 && value < PluginItems.Count ? 
                    App.ViewModel.Plugin.Render(PluginItems[value].Name) : null;
                RuleInputItems = plugin?.Form();
            }
        }

        private IFormInput[]? ruleInputItems;

        public IFormInput[]? RuleInputItems {
            get => ruleInputItems;
            set => Set(ref ruleInputItems, value);
        }

        private IList<DataItem>? ruleDataItems;

        public IList<DataItem>? RuleDataItems {
            get => ruleDataItems;
            set => Set(ref ruleDataItems, value);
        }



        public ICommand BackCommand { get; private set; }

        public ICommand HomeCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }
        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand RuleDeleteCommand { get; private set; }
        public ICommand PanelConfirmCommand { get; private set; }
        public ICommand DialogConfirmCommand { get; private set; }
        public ICommand RuleAddCommand { get; private set; }
        public ICommand RuleConfirmCommand { get; private set; }
        public ICommand RuleUpCommand { get; private set; }
        public ICommand RuleDownCommand { get; private set; }
        public ICommand RuleEditCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.BackAsync();
        }

        private void TapHome(object? _)
        {
            ShellManager.GoToAsync("home");
        }

        private void TapSetting(object? _)
        {
            ShellManager.GoToAsync("setting");
        }

        private void TapAdd(object? _)
        {
            DialogVisible = true;
        }

        private void TapEdit(object? arg)
        {
            if (arg is RuleGroupItem o)
            {
                EditGroup = o;
                PanelTitle = o.Name;
                PanelVisible = true;
                RuleItems.Clear();
                foreach (var item in o.Rules)
                {
                    RuleItems.Add(item);
                }
            }
        }
        private void TapDelete(object? arg)
        {
            if (arg is RuleGroupItem o)
            {
                GroupItems.Remove(o);
                IsUpdated = true;
            }
        }


        private void TapPanelConfirm(object? _)
        {
            PanelVisible = false;
            if (EditGroup is null)
            {
                return;
            }
            EditGroup.Rules = RuleItems.ToList();
        }

        private void TapRuleDelete(object? arg)
        {
            if (arg is RuleItem o)
            {
                RuleItems.Remove(o);
                IsUpdated = true;
            }
        }

        private void TapDialogConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                return;
            }
            var item = new RuleGroupItem()
            {
                MatchValue = GroupMatchValue,
                MatchType = GroupType,
                Name = GroupName,
            };
            GroupItems.Add(item);
            IsUpdated = true;
            GroupName = string.Empty;
            EditGroup = item;
            PanelTitle = item.Name;
            RuleItems.Clear();
            DialogVisible = false;
            PanelVisible = true;
        }

        private void TapRuleAdd(object? _)
        {
            EditRule = null;
            PluginIndex = -1;
            RuleDataItems = null;
            RuleInputItems = null;
            RuleVisible = true;
        }

        private void TapRuleConfirm(object? _)
        {
            if (PluginIndex < 0 || PluginIndex >= PluginItems.Count)
            {
                return;
            }
            if (EditRule is null)
            {
                RuleItems.Add(new RuleItem()
                {
                    Name = PluginItems[PluginIndex].Name,
                    Values = RuleDataItems is null ? new List<DataItem>() : RuleDataItems,
                });
            } else
            {
                EditRule.Name = PluginItems[PluginIndex].Name;
                EditRule.Values = RuleDataItems is null ? new List<DataItem>() : RuleDataItems;
            }
            IsUpdated = true;
            RuleDataItems = null;
            RuleInputItems = null;
            RuleVisible = false;
        }

        private void TapRuleUp(object? arg)
        {
            if (arg is RuleItem o)
            {
                ListExtension.MoveUp(RuleItems, RuleItems.IndexOf(o));
                IsUpdated = true;
            }
        }

        private void TapRuleDown(object? arg)
        {
            if (arg is RuleItem o)
            {
                ListExtension.MoveDown(RuleItems, RuleItems.IndexOf(o));
                IsUpdated = true;
            }
        }

        private void TapRuleEdit(object? arg)
        {
            if (arg is RuleItem o)
            {
                EditRule = o;
                PluginIndex = PluginIndexOf(o.Name);
                RuleDataItems = o.Values;
                RuleVisible = true;
            }
        }

        public int PluginIndexOf(string name)
        {
            return ListExtension.IndexOf(PluginItems, name);
        }

        internal void MoveUp(int index)
        {
            ListExtension.MoveUp(RuleItems, index);
        }

        internal void MoveDown(int index)
        {
            ListExtension.MoveDown(RuleItems, index);
        }

        private void Load()
        {
            var activeItems = App.ViewModel.Option.PluginItems;
            PluginItems = App.ViewModel.Plugin.All().Where(item => activeItems.Count == 0 || activeItems.Contains(item.FileName)).ToArray();
            var project = App.ViewModel.Project;
            GroupItems.Clear();
            if (project == null)
            {
                return;
            }
            foreach (var item in project.RuleItems)
            {
                GroupItems.Add(item);
            }
        }

        public void ApplyExitAttributes()
        {
            if (!IsUpdated)
            {
                return;
            }
            IsUpdated = false;
            var project = App.ViewModel.Project;
            if (project == null)
            {
                return;
            }
            project.RuleItems = GroupItems.ToList();
            project.SaveAsync();
        }
    }
}
