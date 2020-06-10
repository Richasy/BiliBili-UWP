using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;

namespace BiliBili_UWP.Models.UI
{
    public class AppMenuItem : NotifyPropertyBase
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
        public AppMenuItemType Type { get; set; }
        public AppMenuGroupType Group { get; set; }
        private int _unread;
        public int Unread
        {
            get => _unread;
            set => Set(ref _unread, value);
        }
        public FontWeight FontWeight { get; set; }
        public AppMenuItem()
        {

        }
        public AppMenuItem(AppMenuItemType type)
        {
            Type = type;
            Unread = 0;
            FontWeight = FontWeights.Normal;
            switch (type)
            {
                case AppMenuItemType.Home:
                    Name = "首页 & 频道";
                    Icon = "";
                    Group = AppMenuGroupType.Basic;
                    break;
                case AppMenuItemType.Recommend:
                    Name = "推荐视频";
                    Icon = "";
                    Group = AppMenuGroupType.Basic;
                    break;
                case AppMenuItemType.Channel:
                    Name = "频道探索";
                    Icon = "";
                    Group = AppMenuGroupType.Basic;
                    break;
                case AppMenuItemType.Region:
                    Name = "视频分区";
                    Icon = "";
                    Group = AppMenuGroupType.Basic;
                    break;
                case AppMenuItemType.Live:
                    Name = "直播中心";
                    Icon = "";
                    Group = AppMenuGroupType.Basic;
                    break;
                case AppMenuItemType.Rank:
                    Name = "排行榜";
                    Icon = "";
                    Group = AppMenuGroupType.Basic;
                    FontWeight = FontWeights.Bold;
                    break;
                case AppMenuItemType.Anime:
                    Name = "番剧推荐";
                    Icon = "";
                    Group = AppMenuGroupType.Basic;
                    break;
                case AppMenuItemType.Dynamic:
                    Name = "动态";
                    Icon = "";
                    Group = AppMenuGroupType.My;
                    FontWeight = FontWeights.Bold;
                    break;
                case AppMenuItemType.MyHistory:
                    Name = "历史记录";
                    Icon = "";
                    Group = AppMenuGroupType.My;
                    break;
                case AppMenuItemType.MyFavorite:
                    Name = "我的收藏";
                    Icon = "";
                    Group = AppMenuGroupType.My;
                    break;
                case AppMenuItemType.MyDownload:
                    Name = "我的下载";
                    Icon = "";
                    Group = AppMenuGroupType.My;
                    break;
                case AppMenuItemType.MyMessage:
                    Name = "我的消息";
                    Icon = "";
                    Group = AppMenuGroupType.My;
                    break;
                case AppMenuItemType.ViewLater:
                    Name = "稍后再看";
                    Icon = "";
                    Group = AppMenuGroupType.My;
                    FontWeight = FontWeights.Bold;
                    break;
                case AppMenuItemType.Settings:
                    Name = "设置";
                    Icon = "";
                    Group = AppMenuGroupType.Other;
                    break;
                case AppMenuItemType.Help:
                    Name = "帮助 & 关于";
                    Icon = "";
                    Group = AppMenuGroupType.Other;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取侧边栏条目
        /// </summary>
        /// <param name="isLogin">是否登录</param>
        /// <param name="selected">选中项</param>
        /// <returns></returns>
        public static List<AppMenuItem> GetSideMenuItems(bool isLogin, AppMenuItemType selected = AppMenuItemType.Home)
        {
            var list = new List<AppMenuItem>
            {
                new AppMenuItem(AppMenuItemType.Line),
                new AppMenuItem(AppMenuItemType.Home),
                new AppMenuItem(AppMenuItemType.Rank),
                new AppMenuItem(AppMenuItemType.Anime),
                new AppMenuItem(AppMenuItemType.Dynamic),
                new AppMenuItem(AppMenuItemType.Line),
                new AppMenuItem(AppMenuItemType.Settings),
                new AppMenuItem(AppMenuItemType.Help)
            };
            if (isLogin)
            {
                list.InsertRange(5, new List<AppMenuItem>
                {
                    new AppMenuItem(AppMenuItemType.Line),
                    new AppMenuItem(AppMenuItemType.MyHistory),
                    new AppMenuItem(AppMenuItemType.MyFavorite),
                    new AppMenuItem(AppMenuItemType.ViewLater),
                    new AppMenuItem(AppMenuItemType.MyMessage),
                });
            }
            if (selected != AppMenuItemType.Line)
                list.ForEach(p => p.IsSelected = p.Type == selected);
            return list;
        }

        public static List<AppMenuItem> GetTopMenuItems(AppMenuItemType selected = AppMenuItemType.Recommend)
        {
            var list = new List<AppMenuItem>
            {
                new AppMenuItem(AppMenuItemType.Recommend),
                new AppMenuItem(AppMenuItemType.Rank),
                new AppMenuItem(AppMenuItemType.Anime),
                new AppMenuItem(AppMenuItemType.Channel),
                new AppMenuItem(AppMenuItemType.Region)
            };
            if (selected != AppMenuItemType.Line)
                list.ForEach(p => p.IsSelected = p.Type == selected);
            return list;
        }

        public override bool Equals(object obj)
        {
            return obj is AppMenuItem item &&
                   Type == item.Type;
        }

        public override int GetHashCode()
        {
            return 2049151605 + Type.GetHashCode();
        }
    }
}
