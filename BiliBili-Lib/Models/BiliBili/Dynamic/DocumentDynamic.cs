using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class DocumentDynamic
    {
        public int id { get; set; }
        public Category category { get; set; }
        public List<Category> categories { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string banner_url { get; set; }
        public int template_id { get; set; }
        public int state { get; set; }
        public Author author { get; set; }
        public int reprint { get; set; }
        public string[] image_urls { get; set; }
        public int publish_time { get; set; }
        public int ctime { get; set; }
        public Stats stats { get; set; }
        public Tag[] tags { get; set; }
        public int words { get; set; }
        public string dynamic { get; set; }
        public string[] origin_image_urls { get; set; }
        public DocumentList list { get; set; }
        public bool is_like { get; set; }
        public Media media { get; set; }
        public string apply_time { get; set; }
        public string check_time { get; set; }
        public int original { get; set; }
        public int act_id { get; set; }
        public object dispute { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DocumentDynamic dynamic &&
                   id == dynamic.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }

        public class Category
        {
            public int id { get; set; }
            public int parent_id { get; set; }
            public string name { get; set; }
        }

        public class DocumentList
        {
            public int id { get; set; }
            public int mid { get; set; }
            public string name { get; set; }
            public string image_url { get; set; }
            public int update_time { get; set; }
            public int ctime { get; set; }
            public int publish_time { get; set; }
            public string summary { get; set; }
            public int words { get; set; }
            public int read { get; set; }
            public int articles_count { get; set; }
            public int state { get; set; }
            public string reason { get; set; }
            public string apply_time { get; set; }
            public string check_time { get; set; }
        }

        public class Media
        {
            public int score { get; set; }
            public int media_id { get; set; }
            public string title { get; set; }
            public string cover { get; set; }
            public string area { get; set; }
            public int type_id { get; set; }
            public string type_name { get; set; }
            public int spoiler { get; set; }
            public int season_id { get; set; }
        }

        public class DocumentTag
        {
            public int tid { get; set; }
            public string name { get; set; }
        }

    }

    public class Stats
    {
        public int view { get; set; }
        public int favorite { get; set; }
        public int like { get; set; }
        public int dislike { get; set; }
        public int reply { get; set; }
        public int share { get; set; }
        public int coin { get; set; }
        public int dynamic { get; set; }
        public int danmaku { get; set; }
    }
    public class Author
    {
        public int mid { get; set; }
        public string name { get; set; }
        public string face { get; set; }
        public Pendant pendant { get; set; }
        public Vip vip { get; set; }
    }
}
