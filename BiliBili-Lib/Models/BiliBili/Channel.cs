using BiliBili_Lib.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class ChannelBase
    {
        public int id { get; set; }
        public string title { get; set; }
        public string cover { get; set; }
        public string @goto { get; set; }
        public string param { get; set; }
        public string render_cover
        {
            get => cover + "@60w.jpg";
        }

        public override bool Equals(object obj)
        {
            return obj is ChannelBase channel &&
                   id == channel.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }
    }
    public class ChannelSlim : ChannelBase
    {
        public string label { get; set; }
        public string name { get; set; }
        public int is_update { get; set; }

    }
    public class ChannelView : ChannelBase, IAlphaBackground
    {
        public string background { get; set; }
        public string render_background
        {
            get => background + "@150w.jpg";
        }
        public string theme_color { get; set; }
        public int alpha { get; set; }
        public string desc { get; set; }
        public int position { get; set; }
    }
    public class ChannelSquare
    {
        public List<ChannelSlim> Subscribes { get; set; }
        public List<ChannelView> Scaneds { get; set; }
    }
    public class ChannelDetail : ChannelBase, IAlphaBackground
    {
        public string label_1 { get; set; }
        public string label_2 { get; set; }
        public string label_3 { get; set; }
        public int is_atten { get; set; }
        public string theme_color { get; set; }
        public int alpha { get; set; }
        public List<Tag> tags { get; set; }
        public List<ChannelSlim> child { get; set; }
    }
    /// <summary>
    /// 频道索引
    /// </summary>
    public class ChannelTab
    {
        public int id { get; set; }
        public int tab_type { get; set; }
        public string title { get; set; }
        public int count { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ChannelTab tab &&
                   id == tab.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }
        public override string ToString()
        {
            return title;
        }
    }
    /// <summary>
    /// 频道列表条目
    /// </summary>
    public class ChannelListItem : ChannelSlim
    {
        public int is_atten { get; set; }
        public string uri { get; set; }
        public int official_verify { get; set; }
        public string GetSubscribeButtonText()
        {
            return is_atten == 0 ? "订阅" : "取消订阅";
        }
        public override string ToString()
        {
            return title;
        }
    }

    public class ChannelTag
    {
        public int is_atten { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int atten { get; set; }
        public string cover { get; set; }
        public string head_cover { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ChannelTag tag &&
                   id == tag.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }
    }
}
