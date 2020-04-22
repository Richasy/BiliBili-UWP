using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Favorites
{

    public class FavoriteAnime
    {
        public string badge { get; set; }
        public Badge badge_info { get; set; }
        public int badge_type { get; set; }
        public int can_watch { get; set; }
        public string cover { get; set; }
        public int follow { get; set; }
        public int is_finish { get; set; }
        public int movable { get; set; }
        public int mtime { get; set; }
        public New_Ep new_ep { get; set; }
        public Progress progress { get; set; }
        public int season_id { get; set; }
        public int season_type { get; set; }
        public string season_type_name { get; set; }
        public Series series { get; set; }
        public string square_cover { get; set; }
        public string title { get; set; }
        public string url { get; set; }

        public override bool Equals(object obj)
        {
            return obj is FavoriteAnime anime &&
                   season_id == anime.season_id;
        }

        public override int GetHashCode()
        {
            return 1209251872 + season_id.GetHashCode();
        }

        public class Progress
        {
            public string index_show { get; set; }
            public int last_ep_id { get; set; }
            public int last_time { get; set; }
        }

        public class Series
        {
            public int count { get; set; }
            public int id { get; set; }
            public string title { get; set; }
        }

    }
    public class New_Ep
    {
        public string cover { get; set; }
        public int duration { get; set; }
        public int id { get; set; }
        public string index_show { get; set; }
        public int is_new { get; set; }
    }
}
