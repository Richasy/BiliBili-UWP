using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Anime
{
    public class Timeline
    {
        public string date { get; set; }
        public int date_ts { get; set; }
        public int day_of_week { get; set; }
        public string day_update_text { get; set; }
        public List<TimelineEpisode> episodes { get; set; }
        public int is_today { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Timeline line &&
                   date == line.date;
        }

        public override int GetHashCode()
        {
            return -1340352703 + EqualityComparer<string>.Default.GetHashCode(date);
        }
        public string render_day
        {
            get
            {
                string result = "";
                switch (day_of_week)
                {
                    case 1:
                        result = "一";
                        break;
                    case 2:
                        result = "二";
                        break;
                    case 3:
                        result = "三";
                        break;
                    case 4:
                        result = "四";
                        break;
                    case 5:
                        result = "五";
                        break;
                    case 6:
                        result = "六";
                        break;
                    case 7:
                        result = "日";
                        break;
                    default:
                        result = "蛤";
                        break;
                }
                return result;
            }
        }
    }

    public class TimelineEpisode
    {
        public string cover { get; set; }
        public int delay { get; set; }
        public int delay_id { get; set; }
        public string delay_index { get; set; }
        public string delay_reason { get; set; }
        public int episode_id { get; set; }
        public int follow { get; set; }
        public string pub_index { get; set; }
        public string pub_time { get; set; }
        public int pub_ts { get; set; }
        public int published { get; set; }
        public int season_id { get; set; }
        public int season_type { get; set; }
        public string square_cover { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }

}
