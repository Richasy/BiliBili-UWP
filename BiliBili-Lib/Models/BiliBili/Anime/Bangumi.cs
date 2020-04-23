using BiliBili_Lib.Models.BiliBili.Favorites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Anime
{

    public class BangumiSlim
    {
        public int ep_id { get; set; }
        public string title { get; set; }
        public string long_title { get; set; }
        public int episode_status { get; set; }
        public int follow { get; set; }
        public string cover { get; set; }
        public Season season { get; set; }
        public class Season
        {
            public int season_id { get; set; }
            public string title { get; set; }
            public int season_status { get; set; }
            public int is_finish { get; set; }
            public int total_count { get; set; }
            public int newest_ep_id { get; set; }
            public string newest_ep_index { get; set; }
            public int season_type { get; set; }
        }
    }



    public class BangumiDetail
    {
        public Actor actor { get; set; }
        public string alias { get; set; }
        public List<Area> areas { get; set; }
        public string badge { get; set; }
        public Badge badge_info { get; set; }
        public int badge_type { get; set; }
        public string cover { get; set; }
        public string detail { get; set; }
        public string dynamic_subtitle { get; set; }
        public List<Episode> episodes { get; set; }
        public string evaluate { get; set; }
        public string link { get; set; }
        public int media_id { get; set; }
        public int mode { get; set; }
        public List<BangumiModule> modules { get; set; }
        public New_Ep new_ep { get; set; }
        public string origin_name { get; set; }
        public Publish publish { get; set; }
        public Rating rating { get; set; }
        public string record { get; set; }
        public string refine_cover { get; set; }
        public int season_id { get; set; }
        public string season_title { get; set; }
        public Series series { get; set; }
        public string share_copy { get; set; }
        public string share_url { get; set; }
        public string short_link { get; set; }
        public string square_cover { get; set; }
        public Actor staff { get; set; }
        public Stat stat { get; set; }
        public int status { get; set; }
        public List<BangumiStyle> styles { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public int total { get; set; }
        public int type { get; set; }
        public string type_desc { get; set; }
        public string type_name { get; set; }
        public UserStatus user_status { get; set; }
        public Limit limit { get; set; }
        public class Actor
        {
            public string info { get; set; }
            public string title { get; set; }
        }
        public class Stat
        {
            public int coins { get; set; }
            public int danmakus { get; set; }
            public int favorites { get; set; }
            public string followers { get; set; }
            public string play { get; set; }
            public int reply { get; set; }
            public int share { get; set; }
            public int views { get; set; }
        }

        public class Limit
        {
            public string content { get; set; }
            public string image { get; set; }
        }

    }


    public class Publish
    {
        public int is_finish { get; set; }
        public int is_started { get; set; }
        public string pub_time { get; set; }
        public string pub_time_show { get; set; }
        public string release_date_show { get; set; }
        public string time_length_show { get; set; }
        public int unknow_pub_date { get; set; }
        public int weekday { get; set; }
    }

    public class Rating
    {
        public int count { get; set; }
        public float score { get; set; }
    }

    public class Series
    {
        public string movie_title { get; set; }
        public int series_id { get; set; }
        public string series_title { get; set; }
    }



    public class UserStatus
    {
        public int follow { get; set; }
        public int follow_bubble { get; set; }
        public int follow_status { get; set; }
        public int pay { get; set; }
        public Review review { get; set; }
        public int sponsor { get; set; }
        public int theme_type { get; set; }
        public int vip { get; set; }
        public int vip_frozen { get; set; }
        public UserProgress progress { get; set; }

        public class UserProgress
        {
            public int last_ep_id { get; set; }
            public string last_ep_index { get; set; }
            public int last_time { get; set; }
        }

    }

    public class Review
    {
        public string article_url { get; set; }
        public int is_open { get; set; }
        public int score { get; set; }
    }

    public class Area
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Episode
    {
        public int aid { get; set; }
        public string badge { get; set; }
        public Badge badge_info { get; set; }
        public int badge_type { get; set; }
        public string bvid { get; set; }
        public int cid { get; set; }
        public string cover { get; set; }
        public string from { get; set; }
        public int id { get; set; }
        public string link { get; set; }
        public string long_title { get; set; }
        public string movie_title { get; set; }
        public int pub_time { get; set; }
        public string release_date { get; set; }
        public string share_copy { get; set; }
        public string share_url { get; set; }
        public string short_link { get; set; }
        public Stat stat { get; set; }
        public int status { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public string vid { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Episode episode &&
                   cid == episode.cid &&
                   id == episode.id;
        }

        public override int GetHashCode()
        {
            var hashCode = 1348689431;
            hashCode = hashCode * -1521134295 + cid.GetHashCode();
            hashCode = hashCode * -1521134295 + id.GetHashCode();
            return hashCode;
        }
    }

    public class BangumiModule
    {
        public Data data { get; set; }
        public int id { get; set; }
        public string more { get; set; }
        public string style { get; set; }
        public string title { get; set; }
        public class Data
        {
            public List<Episode> episodes { get; set; }
            public int episode_id { get; set; }
            public int id { get; set; }
            public string more { get; set; }
            public string title { get; set; }
            public int type { get; set; }
        }
    }

    public class BangumiSeason
    {
        public string badge { get; set; }
        public Badge badge_info { get; set; }
        public int badge_type { get; set; }
        public string cover { get; set; }
        public int is_new { get; set; }
        public string link { get; set; }
        public New_Ep new_ep { get; set; }
        public string resource { get; set; }
        public int season_id { get; set; }
        public string season_title { get; set; }
        public string title { get; set; }
    }
    public class BangumiStyle
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

}
