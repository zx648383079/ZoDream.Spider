using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Spider.ViewModels
{
    public class EditViewModel: BindableBase
    {
        private Array headerKeys = Enum.GetNames(typeof(HttpRequestHeader));
        public Array HeaderKeys
        {
            get
            {
                return headerKeys;
            }
            set
            {
                Set(ref headerKeys, value);
            }
        }


        private ObservableCollection<RuleGroupItem> ruleItems = new ObservableCollection<RuleGroupItem>();

        public ObservableCollection<RuleGroupItem> RuleItems
        {
            get => ruleItems;
            set => Set(ref ruleItems, value);
        }


        private ObservableCollection<HeaderItem> headerItems = new ObservableCollection<HeaderItem>();

        public ObservableCollection<HeaderItem> HeaderItems
        {
            get => headerItems;
            set => Set(ref headerItems, value);
        }


        public void AddHeader(HeaderItem item)
        {
            var i = HeaderIndexOf(item.Name);
            if (i < 0)
            {
                HeaderItems.Add(item);
            }
            else
            {
                HeaderItems[i] = item;
            }
        }
        public int HeaderIndexOf(string name)
        {
            for (int i = 0; i < HeaderItems.Count; i++)
            {
                if (name == HeaderItems[i].Name)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
