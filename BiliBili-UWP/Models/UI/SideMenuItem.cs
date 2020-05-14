using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_UWP.Models.UI
{
    public class SideMenuItem : NotifyPropertyBase
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }
        public SideMenuItemType Type { get; set; }
        public SideMenuGroupType Group { get; set; }
        private int _unread;
        public int Unread
        {
            get => _unread;
            set => Set(ref _unread, value);
        }
        public SideMenuItem()
        {

        }
        public SideMenuItem(SideMenuItemType type)
        {
            Type = type;
            Unread = 0;
            switch (type)
            {
                case SideMenuItemType.Home:
                    Name = "首页 & 频道";
                    Icon = "";
                    Group = SideMenuGroupType.Basic;
                    break;
                case SideMenuItemType.Live:
                    Name = "直播中心";
                    Icon = "";
                    Group = SideMenuGroupType.Basic;
                    break;
                case SideMenuItemType.Rank:
                    Name = "排行榜";
                    Icon = "";
                    Group = SideMenuGroupType.Basic;
                    break;
                case SideMenuItemType.Anime:
                    Name = "番剧推荐";
                    Icon = "";
                    Group = SideMenuGroupType.Basic;
                    break;
                case SideMenuItemType.Dynamic:
                    Name = "动态";
                    Icon = "";
                    Group = SideMenuGroupType.Basic;
                    break;
                case SideMenuItemType.MyHistory:
                    Name = "历史记录";
                    Icon = "";
                    Group = SideMenuGroupType.My;
                    break;
                case SideMenuItemType.MyFavorite:
                    Name = "我的收藏";
                    Icon = "";
                    Group = SideMenuGroupType.My;
                    break;
                case SideMenuItemType.MyDownload:
                    Name = "我的下载";
                    Icon = "";
                    Group = SideMenuGroupType.My;
                    break;
                case SideMenuItemType.ViewLater:
                    Name = "稍后再看";
                    Icon = "";
                    Group = SideMenuGroupType.My;
                    break;
                case SideMenuItemType.Settings:
                    Name = "设置";
                    Icon = "";
                    Group = SideMenuGroupType.Other;
                    break;
                case SideMenuItemType.Help:
                    Name = "帮助 & 关于";
                    Icon = "";
                    Group = SideMenuGroupType.Other;
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
        public static List<SideMenuItem> GetSideMenuItems(bool isLogin, SideMenuItemType selected = SideMenuItemType.Home)
        {
            var list = new List<SideMenuItem>
            {
                new SideMenuItem(SideMenuItemType.Line),
                new SideMenuItem(SideMenuItemType.Home),
                new SideMenuItem(SideMenuItemType.Rank),
                new SideMenuItem(SideMenuItemType.Anime),
                new SideMenuItem(SideMenuItemType.Dynamic),
                new SideMenuItem(SideMenuItemType.Line),
                new SideMenuItem(SideMenuItemType.Settings),
                new SideMenuItem(SideMenuItemType.Help)
            };
            if (isLogin)
            {
                list.InsertRange(5, new List<SideMenuItem>
                {
                    new SideMenuItem(SideMenuItemType.Line),
                    new SideMenuItem(SideMenuItemType.MyHistory),
                    new SideMenuItem(SideMenuItemType.MyFavorite),
                    new SideMenuItem(SideMenuItemType.MyDownload),
                    new SideMenuItem(SideMenuItemType.ViewLater)
                });
            }
            if (selected != SideMenuItemType.Line)
                list.ForEach(p => p.IsSelected = p.Type == selected);
            return list;
        }

        public override bool Equals(object obj)
        {
            return obj is SideMenuItem item &&
                   Type == item.Type;
        }

        public override int GetHashCode()
        {
            return 2049151605 + Type.GetHashCode();
        }
    }
}
