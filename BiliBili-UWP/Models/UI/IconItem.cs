using BiliBili_Lib.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_UWP.Models.UI
{
    public class IconItem : NotifyBase
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        private object _param;
        public object Param
        {
            get => _param;
            set => Set(ref _param, value);
        }
        public IconItem(string icon, string name, object pa)
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
        public static List<IconItem> GetReplySortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","按热门","3"),
                new IconItem("","按时间","2"),
            };
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
        public static List<IconItem> GetSearchVideoSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","默认排序","default"),
                new IconItem("","播放多","view"),
                new IconItem("","新发布","pubdate"),
                new IconItem("","弹幕多","danmaku"),
            };
        }
        public static List<IconItem> GetSubRegionSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","默认排序",""),
                new IconItem("","播放最多","view"),
                new IconItem("","最新视频","senddate"),
                new IconItem("","弹幕最多","danmaku"),
                new IconItem("","评论最多","reply"),
                new IconItem("","收藏最多","favorite"),
            };
        }
        public static List<IconItem> GetSearchVideoDurationSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","全部时长","0"),
                new IconItem("","0-10分钟","1"),
                new IconItem("","10-30分钟","2"),
                new IconItem("","30-60分钟","3"),
                new IconItem("","60分钟+","4"),
            };
        }
        public static List<IconItem> GetSearchVideoRegionSortItems()
        {
            var list = new List<IconItem>();
            foreach (var item in App.BiliViewModel.RegionCollection)
            {
                list.Add(new IconItem("", item.name, item.tid.ToString()));
            }
            list.Insert(0, new IconItem("", "全部分区", "0"));
            return list;
        }
        public static List<IconItem> GetSearchUserSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","默认排序","totalrank_0"),
                new IconItem("","粉丝数由高到低","fan_0"),
                new IconItem("","粉丝数由低到高","fan_1"),
                new IconItem("","Lv等级由高到低","level_0"),
                new IconItem("","Lv等级由低到高","level_1"),
            };
        }
        public static List<IconItem> GetSearchUserTypeSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","全部用户","0"),
                new IconItem("","UP主","1"),
                new IconItem("","认证用户","3"),
                new IconItem("","普通用户","2"),
            };
        }
        public static List<IconItem> GetSearchDocumentSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","默认排序",""),
                new IconItem("","发布时间","pubdate"),
                new IconItem("","阅读数","click"),
                new IconItem("","评论数","scores"),
                new IconItem("","点赞数","attention"),
            };
        }
        public static List<IconItem> GetSearchDocumentRegionSortItems()
        {
            return new List<IconItem>
            {
                new IconItem("","全部分类","0"),
                new IconItem("","动画","2"),
                new IconItem("","游戏","1"),
                new IconItem("","影视","28"),
                new IconItem("","生活","3"),
                new IconItem("","兴趣","29"),
                new IconItem("","轻小说","16"),
                new IconItem("","科技","17"),
            };
        }
        public static List<IconItem> GetMessageHeaderItems()
        {
            return new List<IconItem>
            {
                new IconItem("",StaticString.MESSAGE_REPLYME,""),
                new IconItem("",StaticString.MESSAGE_AT,""),
                new IconItem("",StaticString.MESSAGE_LIKE,"")
            };
        }
    }
}
