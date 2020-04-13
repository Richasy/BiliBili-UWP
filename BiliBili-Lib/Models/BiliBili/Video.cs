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
        public string _cover;
        public string cover
        {
            get => _cover+ "@200w.jpg";
            set => _cover = value;
        }
        public string param { get; set; }
        public string cover_left_text_1 { get; set; }
        public int cover_left_icon_1 { get; set; }
        public string cover_left_text_2 { get; set; }
        public int cover_left_icon_2 { get; set; }
        public string GetResolutionCover(string resolution)
        {
            return _cover + $"@{resolution}w.jpg";
        }
    }

    public class VideoRecommend : VideoBase
    {
        public string uri { get; set; }
        public Args args { get; set; }
        public List<ThreePointV2> three_point_v2 { get; set; }
        public string cover_right_text { get; set; }
        public string rcmd_reason { get; set; }
        public int official_icon { get; set; }
        public int can_play { get; set; }
        public string desc { get; set; }
    }
    public class VideoChannel : VideoBase
    {
        public string @goto { get; set; }
        public string cover_left_text_3 { get; set; }
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

}
