using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Feedback
{
    public class FeedUser
    {
        public int mid { get; set; }
        public int fans { get; set; }
        public string nickname { get; set; }
        public string avatar { get; set; }
        public string mid_link { get; set; }
        public bool follow { get; set; }
    }
    public class FeedCursor
    {
        public bool is_end { get; set; }
        public long id { get; set; }
        public int time { get; set; }
    }
    public class FeedItem
    {
        public string type { get; set; }
        public string business { get; set; }
        public int business_id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string uri { get; set; }
        public long subject_id { get; set; }
        public long root_id { get; set; }
        public long target_id { get; set; }
        public long source_id { get; set; }
        public string source_content { get; set; }
        public string native_uri { get; set; }
        public List<FeedUser> at_details { get; set; }
        public bool hide_reply_button { get; set; }
    }
    public class FeedDetail
    {
        public long id { get; set; }
        public FeedUser user { get; set; }

        public override bool Equals(object obj)
        {
            return obj is FeedDetail detail &&
                   id == detail.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 + id.GetHashCode();
        }
    }
}
