using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Utils
{
    public static class ListExtension
    {
        public static int IndexOf<T>(IList<T> items, string name)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item == null)
                {
                    continue;
                }
                if (
                    (item is HeaderItem && (item as HeaderItem).Name == name) ||
                    (item is RuleItem && (item as RuleItem).Name == name) ||
                    (item is PluginItem && (item as PluginItem).Name == name)
                    )
                {
                    return i;
                }
            }
            return -1;
        }

        public static int IndexOf<T>(IList<T> items, T item)
        {
            return items.IndexOf(item);
        }

        public static void Remove<T>(IList<T> items, string name)
        {
            Remove(items, name, -1);
        }

        public static void Remove<T>(IList<T> items, string name, int excludeIndex)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (i == excludeIndex)
                {
                    continue;
                }
                var item = items[i];
                if (item == null)
                {
                    continue;
                }
                if (
                    (item is HeaderItem && (item as HeaderItem).Name == name) ||
                    (item is RuleItem && (item as RuleItem).Name == name) ||
                    (item is PluginItem && (item as PluginItem).Name == name)
                    )
                {
                    items.RemoveAt(i);
                }
            }
        }

        public static void Remove<T>(IList<T> items, T item)
        {
            items.Remove(item);
        }

        public static void Remove<T>(IList<T> items, IEnumerable<T> removeItems)
        {
            foreach (var item in removeItems)
            {
                items.Remove(item);
            }
        }

        /// <summary>
        /// 移动元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="selected">要移动的元素</param>
        /// <param name="offset">负为前移 正为后移</param>
        public static void Move<T>(IList<T> items, int selected, int offset)
        {
            if (offset < 0 && selected + offset < 0)
            {
                offset = - selected;
            } else if (offset > 0 && selected + offset >= items.Count)
            {
                offset = items.Count - selected;
            }
            if (offset == 0)
            {
                return;
            }
            var cloneItems = new T[offset < 0 ? - offset : offset];
            var start = offset < 0 ? selected - offset : selected + 1;
            for (int i = 0; i < cloneItems.Length; i++)
            {
                cloneItems[i] = items[start + i];
            }
            items[selected + offset] = items[selected];
            var newStart = offset < 0 ? start + 1 : start - 1;
            for (int i = 0; i < cloneItems.Length; i++)
            {
                items[newStart + i] = cloneItems[i];
            }
        }

        public static void MoveUp<T>(IList<T> items, int selected)
        {
            if (selected <= 1) return;
            var item = items[selected];
            items[selected] = items[selected - 1];
            items[selected - 1] = item;
        }

        public static void MoveDown<T>(IList<T> items, int selected)
        {
            if (selected < 0 || selected > items.Count - 2) return;
            var item = items[selected];
            items[selected] = items[selected + 1];
            items[selected + 1] = item;
        }
    }
}
