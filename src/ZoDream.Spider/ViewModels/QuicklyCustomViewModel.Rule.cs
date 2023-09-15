using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.ViewModels
{
    public partial class QuicklyCustomViewModel
    {

        private RuleGroupItem? EditGroup;
        private RuleItem? EditRule;

        private IList<PluginItem> pluginItems = new List<PluginItem>();

        public IList<PluginItem> PluginItems {
            get => pluginItems;
            set => Set(ref pluginItems, value);
        }

        private ObservableCollection<RuleGroupItem> groupItems = new();

        public ObservableCollection<RuleGroupItem> GroupItems {
            get => groupItems;
            set => Set(ref groupItems, value);
        }

        private ObservableCollection<RuleItem> ruleItems = new();

        public ObservableCollection<RuleItem> RuleItems {
            get => ruleItems;
            set => Set(ref ruleItems, value);
        }


        private bool ruleDialogVisible;

        public bool RuleDialogVisible {
            get => ruleDialogVisible;
            set => Set(ref ruleDialogVisible, value);
        }

        private bool groupDialogVisible;

        public bool GroupDialogVisible {
            get => groupDialogVisible;
            set => Set(ref groupDialogVisible, value);
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

        private int pluginIndex;

        public int PluginIndex {
            get => pluginIndex;
            set => Set(ref pluginIndex, value);
        }

        private string ruleParam1 = string.Empty;

        public string RuleParam1 {
            get => ruleParam1;
            set => Set(ref ruleParam1, value);
        }
        private string ruleParam2 = string.Empty;

        public string RuleParam2 {
            get => ruleParam2;
            set => Set(ref ruleParam2, value);
        }

        public ICommand GroupDialogConfirmCommand { get; private set; }
        public ICommand RuleDialogConfirmCommand { get; private set; }

        public ICommand GroupEditCommand { get; private set; }
        public ICommand GroupDeleteCommand { get; private set; }

        public ICommand PanelConfirmCommand { get; private set; }
        public ICommand RuleDeleteCommand { get; private set; }


        public ICommand RuleAddCommand { get; private set; }
        public ICommand RuleConfirmCommand { get; private set; }
        public ICommand RuleUpCommand { get; private set; }
        public ICommand RuleDownCommand { get; private set; }
        public ICommand RuleEditCommand { get; private set; }

        private void TapGroupEdit(object? arg)
        {
            if (arg is RuleGroupItem o)
            {
                EditGroup = o;
                PanelTitle = o.MatchValue;
                PanelVisible = true;
                RuleItems.Clear();
                foreach (var item in o.Rules)
                {
                    RuleItems.Add(item);
                }
            }
        }
        private void TapGroupDelete(object? arg)
        {
            if (arg is RuleGroupItem o)
            {
                GroupItems.Remove(o);
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

        private void TapRuleDialogConfirm(object? _)
        {

        }

        private void TapGroupDialogConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(GroupName))
            {
                return;
            }
            var item = new RuleGroupItem()
            {
                MatchValue = GroupName,
                MatchType = GroupType,
            };
            GroupItems.Add(item);
            GroupName = string.Empty;
            EditGroup = item;
            PanelTitle = item.MatchValue;
            RuleItems.Clear();
            GroupDialogVisible = false;
            PanelVisible = true;
        }

        private void TapRuleAdd(object? _)
        {
            EditRule = null;
            PluginIndex = -1;
            RuleParam2 = RuleParam1 = string.Empty;
            RuleDialogVisible = true;
        }
        private void TapRuleDelete(object? arg)
        {
            if (arg is RuleItem o)
            {
                RuleItems.Remove(o);
            }
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
                    Param1 = RuleParam1,
                    Param2 = RuleParam2,
                });
            }
            else
            {
                EditRule.Name = PluginItems[PluginIndex].Name;
                EditRule.Param1 = RuleParam1;
                EditRule.Param2 = RuleParam2;
            }
            RuleParam1 = RuleParam2 = string.Empty;
            RuleDialogVisible = false;
        }

        private void TapRuleUp(object? arg)
        {
            if (arg is RuleItem o)
            {
                ListExtension.MoveUp(RuleItems, RuleItems.IndexOf(o));
            }
        }

        private void TapRuleDown(object? arg)
        {
            if (arg is RuleItem o)
            {
                ListExtension.MoveDown(RuleItems, RuleItems.IndexOf(o));
            }
        }

        private void TapRuleEdit(object? arg)
        {
            if (arg is RuleItem o)
            {
                EditRule = o;
                PluginIndex = PluginIndexOf(o.Name);
                RuleParam1 = o.Param1;
                RuleParam2 = o.Param2;
                RuleDialogVisible = true;
            }
        }

        public int PluginIndexOf(string name)
        {
            return ListExtension.IndexOf(PluginItems, name);
        }
    }
}
