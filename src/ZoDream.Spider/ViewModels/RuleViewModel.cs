using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class RuleViewModel: BindableBase
    {

        public RuleViewModel()
        {
            PluginItems = new List<PluginItem>()
            {
                new PluginItem() { Name = "下载文件"},
                new PluginItem() {Name = "正则截取"},
                new PluginItem() {Name = "网址提取"},
                new PluginItem() {Name = "普通截取"},
                new PluginItem() {Name = "普通替换"},
                new PluginItem() {Name = "正则替换"},
                new PluginItem() {Name = "正则匹配"},
                new PluginItem() {Name = "合并网页"},
                new PluginItem() {Name = "替换HTML"},
                new PluginItem() {Name = "简繁转换"},
                new PluginItem() {Name = "XPath选择"},
                new PluginItem() {Name = "JQuery选择"},
                new PluginItem() {Name = "Csv保存"},
                new PluginItem() {Name = "Excel保存"},
                new PluginItem() {Name = "保存"},
                new PluginItem() {Name = "JSON保存"},
                new PluginItem() {Name = "追加"},
            };
        }

        private IList<PluginItem> pluginItems;

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
            for (int i = 0; i < PluginItems.Count; i++)
            {
                if (PluginItems[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void MoveUp(int index)
        {
            if (index <= 1) return;
            var item = RuleItems[index];
            RuleItems[index] = RuleItems[index - 1];
            RuleItems[index - 1] = item;
        }

        internal void MoveDown(int index)
        {
            if (index < 0 || index > RuleItems.Count - 2) return;
            var item = RuleItems[index];
            RuleItems[index] = RuleItems[index + 1];
            RuleItems[index + 1] = item;
        }
    }
}
