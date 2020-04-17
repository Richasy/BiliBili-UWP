using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class AnimeModule
    {
        public AnimeAttribute attr { get; set; }
        public List<AnimeModuleItem> items { get; set; }
        public int module_id { get; set; }
        public AnimeReport report { get; set; }
        public int size { get; set; }
        public string style { get; set; }
        public string title { get; set; }
        public int type { get; set; }
    }

    public class AnimeAttribute
    {
        public int follow { get; set; }
        public int header { get; set; }
        public int random { get; set; }
    }

    public class AnimeReport
    {
        public string module_id { get; set; }
        public string module_type { get; set; }
    }

    public class AnimeModuleItem
    {
        public Badge badge_info { get; set; }
        public string cover { get; set; }
        public int is_preview { get; set; }
        public int item_id { get; set; }
        public string link { get; set; }
        public int oid { get; set; }
        public Report report { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int wid { get; set; }
        public string badge { get; set; }
        public int badge_type { get; set; }
        public string desc { get; set; }
        public int season_id { get; set; }
        public int season_type { get; set; }
        public Stat stat { get; set; }
        public int can_watch { get; set; }
        public int is_auto { get; set; }
        public Status status { get; set; }
        public List<Card> cards { get; set; }
        public string cursor { get; set; }
        public int is_new { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AnimeModuleItem item &&
                   title == item.title;
        }

        public override int GetHashCode()
        {
            return 375585649 + EqualityComparer<string>.Default.GetHashCode(title);
        }

        public class Report
        {
            public string card_type { get; set; }
            public string item_id { get; set; }
            public string module_id { get; set; }
            public string module_type { get; set; }
            public string oid { get; set; }
            public string season_type { get; set; }
        }

    }

    public class Stat
    {
        public int danmaku { get; set; }
        public int follow { get; set; }
        public string follow_view { get; set; }
        public int view { get; set; }
    }

    public class Status
    {
        public int follow { get; set; }
        public int follow_status { get; set; }
    }

    public class Card
    {
        public string badge { get; set; }
        public Badge badge_info { get; set; }
        public int badge_type { get; set; }
        public int can_watch { get; set; }
        public string cover { get; set; }
        public string link { get; set; }
        public int oid { get; set; }
        public string pts { get; set; }
        public int season_type { get; set; }
        public Status status { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string render_sign { get; set; }
    }
}
