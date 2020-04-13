using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_UWP.Models.UI
{
    public class IconItem
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public object Param { get; set; }
        public IconItem(string icon,string name,object pa)
        {
            Icon = icon;
            Name = name;
            Param = pa;
        }

        public override bool Equals(object obj)
        {
            return obj is IconItem item &&
                   Name == item.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public static List<IconItem> GetChannelVideoSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","近期热门","hot"),
                new IconItem("","播放最多（近30天投稿）","view"),
                new IconItem("","最新发布","new"),
            };
        }
    }
}
