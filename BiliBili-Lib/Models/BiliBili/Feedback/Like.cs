using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Feedback
{
    public class FeedLikeResponse
    {
        public FeedLikeLatest latest { get; set; }
        public FeedLikeTotal total { get; set; }
    }
    public class FeedLikeLatest
    {
        public List<FeedLikeDetail> items { get; set; }
        public int last_view_at { get; set; }
    }
    public class FeedLikeTotal
    {
        public FeedCursor cursor { get; set; }
        public List<FeedLikeDetail> items { get; set; }
    }
    public class FeedLikeDetail
    {
        public long id { get; set; }
        public List<FeedUser> users { get; set; }
        public FeedLikeItem item { get; set; }
        public int counts { get; set; }
        public int notice_state { get; set; }
        public int like_time { get; set; }
        public bool is_latest { get; set; }
    }

    public class FeedLikeItem:FeedItem
    {
        public long item_id { get; set; }
        public int pid { get; set; }
        public int reply_business_id { get; set; }
        public int like_business_id { get; set; }
        public string desc { get; set; }
        public string detail_name { get; set; }
        public int ctime { get; set; }
    }

}
