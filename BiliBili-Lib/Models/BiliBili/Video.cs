using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class VideoBase
    {
        public string card_type { get; set; }
        public string card_goto { get; set; }
        public int idx { get; set; }
        public string title { get; set; }
        public string cover { get; set; }
        public string render_cover
        {
            get => cover + "@400w.jpg";
        }
        public string param { get; set; }
        public string cover_left_text_1 { get; set; }
        public int cover_left_icon_1 { get; set; }
        public string cover_left_text_2 { get; set; }
        public int cover_left_icon_2 { get; set; }
        public string cover_left_text_3 { get; set; }

        public override bool Equals(object obj)
        {
            return obj is VideoBase @base &&
                   title == @base.title &&
                   param == @base.param;
        }

        public override int GetHashCode()
        {
            var hashCode = 1685139875;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(title);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(param);
            return hashCode;
        }

        public string GetResolutionCover(string resolution)
        {
            return cover + $"@{resolution}w.jpg";
        }

    }

    public class VideoRecommend : VideoBase
    {
        public string uri { get; set; }
        public Args args { get; set; }
        public List<ThreePointV2> three_point_v2 { get; set; }
        public string top_rcmd_reason { get; set; }
        public int official_icon { get; set; }
        public int can_play { get; set; }
        public string desc { get; set; }

    }
    public class VideoChannel : VideoBase
    {
        public string @goto { get; set; }
        public string sort { get; set; }
    }

    public class Args
    {
        public int up_id { get; set; }
        public string up_name { get; set; }
        public int rid { get; set; }
        public string rname { get; set; }
        public int tid { get; set; }
        public string tname { get; set; }
        public int aid { get; set; }
    }

    public class ThreePointV2
    {
        public string title { get; set; }
        public string type { get; set; }
        public string subtitle { get; set; }
        public List<Reason> reasons { get; set; }
    }

    public class Reason
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class WebVideo
    {
        public string aid { get; set; }
        public string bvid { get; set; }
        public string typename { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public int play { get; set; }
        public int review { get; set; }
        public int video_review { get; set; }
        public int favorites { get; set; }
        public int mid { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string create { get; set; }
        public string pic { get; set; }
        public int coins { get; set; }
        public string duration { get; set; }
        public bool badgepay { get; set; }
        public int pts { get; set; }
        public string render_sign { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WebVideo video &&
                   aid == video.aid;
        }

        public override int GetHashCode()
        {
            return 1525766729 + EqualityComparer<string>.Default.GetHashCode(aid);
        }
    }
}
