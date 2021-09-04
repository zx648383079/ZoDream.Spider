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
            if (App.ViewModel.Instance == null)
            {
                return;
            }
            PluginItems = App.ViewModel.Instance.RuleProvider.AllPlugin();
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
