using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class HotSearch
    {
        public string keyword { get; set; }
        public string status { get; set; }
        public string name_type { get; set; }
        public string show_name { get; set; }
        public int word_type { get; set; }
        public string icon { get; set; }
        public int position { get; set; }
        public int module_id { get; set; }
        public override string ToString()
        {
            return show_name;
        }
    }

    public class SearchResult
    {
        public string trackid { get; set; }
        public int page { get; set; }
        public List<SearchTab> nav { get; set; }
        public List<SearchVideo> item { get; set; }
        public int array { get; set; }
        public int attribute { get; set; }
        public string exp_str { get; set; }
    }

    public class SearchTab : NotifyBase
    {
        public string name { get; set; }
        private int _total;
        public int total
        {
            get => _total;
            set => Set(ref _total, value);
        }
        public int pages { get; set; }
        public int type { get; set; }
        public string display
        {
            get => name + (total == 0 ? "" : $"({total})");
        }
    }
    public class SearchBase
    {
        public string title { get; set; }
        public string trackid { get; set; }
        public string linktype { get; set; }
        public int position { get; set; }
        public string param { get; set; }
        public string @goto { get; set; }
        public string uri { get; set; }
        public string cover { get; set; }
        public int mid { get; set; }

    }
    public class SearchVideo : SearchBase
    {
        public int play { get; set; }
        public string author { get; set; }
        public string desc { get; set; }
        public string duration { get; set; }
        public string face { get; set; }
        public int danmaku { get; set; }
    }
    public class SearchDocument : SearchBase
    {
        public string name { get; set; }
        public int play { get; set; }
        public string desc { get; set; }
        public int id { get; set; }
        public int template_id { get; set; }
        public List<string> image_urls { get; set; }
        public int view { get; set; }
        public int like { get; set; }
        public int reply { get; set; }
        public string badge { get; set; }
    }

    public class SearchLive : SearchBase
    {
        public string name { get; set; }
        public int roomid { get; set; }
        public string type { get; set; }
        public int attentions { get; set; }
        public int region { get; set; }
        public int online { get; set; }
        public string badge { get; set; }
    }

    public class SearchUser : SearchBase
    {
        public string sign { get; set; }
        public int fans { get; set; }
        public string render_fans
        {
            get => AppTool.GetNumberAbbreviation(fans);
        }
        public int level { get; set; }
        public string render_level
        {
            get => $"ms-appx:///Assets/Level/level_{level}.png";
        }
        public bool is_up { get; set; }
        public string live_uri { get; set; }
        public int archives { get; set; }
        public string render_archives
        {
            get => AppTool.GetNumberAbbreviation(archives);
        }
        public int roomid { get; set; }
        public int attentions { get; set; }
        public string render_follow
        {
            get => attentions == 1 ? "已关注" : "关注";
        }
    }

    public class SearchAnime : SearchBase
    {
        public int ptime { get; set; }
        public int season_id { get; set; }
        public int season_type { get; set; }
        public string season_type_name { get; set; }
        public int media_type { get; set; }
        public string style { get; set; }
        public string styles { get; set; }
        public string cv { get; set; }
        public float rating { get; set; }
        public int vote { get; set; }
        public string render_vote
        {
            get => AppTool.GetNumberAbbreviation(vote) + "人";
        }
        public string area { get; set; }
        public string staff { get; set; }
        public int is_selection { get; set; }
        public string badge { get; set; }
        /// <summary>
        /// 分集
        /// </summary>
        public List<Episode> episodes { get; set; }
        public string label { get; set; }
        public string selection_style { get; set; }
        public int is_atten { get; set; }
        public List<Badge> badges { get; set; }
        public string render_follow
        {
            get
            {
                if (season_type_name == "番剧")
                    return is_atten == 1 ? "已追番" : "追番";
                else
                    return is_atten == 1 ? "已追剧" : "追剧";
            }
        }
        public class Episode
        {
            public int position { get; set; }
            public string uri { get; set; }
            public string param { get; set; }
            public string index { get; set; }
        }


    }
    public class Badge
    {
        public string text { get; set; }
    }

    public class SearchSuggestion
    {
        public string value { get; set; }
        public int @ref { get; set; }
        public string name { get; set; }
        public int spid { get; set; }
        public override string ToString()
        {
            return value;
        }
    }

}
