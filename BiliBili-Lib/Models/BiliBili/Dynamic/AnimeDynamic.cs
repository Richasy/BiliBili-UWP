using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class AnimeDynamic
    {
        public int aid { get; set; }
        public int cid { get; set; }
        public string cover { get; set; }
        public int duration { get; set; }
        public int episode_id { get; set; }
        public string index_title { get; set; }
        public int is_finish { get; set; }
        public int is_preview { get; set; }
        public string new_desc { get; set; }
        public string region_uri { get; set; }
        public Season season { get; set; }
        public string short_title { get; set; }
        public string show_title { get; set; }
        public Stats stat { get; set; }
        public string url { get; set; }
        public class Season
        {
            public string cover { get; set; }
            public int is_finish { get; set; }
            public int season_id { get; set; }
            public string square_cover { get; set; }
            public string title { get; set; }
            public int total_count { get; set; }
            public int ts { get; set; }
            public int type { get; set; }
            public string type_name { get; set; }
        }
    }

    

}
