using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class RuleViewModel: BindableBase
    {

        public RuleViewModel()
        {
            PluginItems = App.ViewModel.Plugin.All();
        }

        private int selectedIndex = -1;

        public int SelectedIndex
        {
            get => selectedIndex;
            set => Set(ref selectedIndex, value);
        }

        private IList<PluginItem> pluginItems = new List<PluginItem>();

        public IList<PluginItem> PluginItems
        {
            get => pluginItems;
            set => Set(ref pluginItems, value);
        }


        private ObservableCollection<RuleItem> ruleItems = new ObservableCollection<RuleItem>();

        public ObservableCollection<RuleItem> RuleItems
        {
            get => ruleItems;
            set => Set(ref ruleItems, value);
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
    }
}
