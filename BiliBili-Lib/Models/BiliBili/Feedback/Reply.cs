using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Feedback
{

    public class FeedReplyResponse
    {
        public FeedCursor cursor { get; set; }
        public List<FeedReplyDetail> items { get; set; }
        public int last_view_at { get; set; }
    }

    public class FeedReplyDetail:FeedDetail
    {
        public FeedReplyItem item { get; set; }
        public int counts { get; set; }
        public int is_multi { get; set; }
        public int reply_time { get; set; }
    }

    public class FeedReplyItem:FeedItem
    {
        public string desc { get; set; }
        public string detail_title { get; set; }
        public string root_reply_content { get; set; }
        public string target_reply_content { get; set; }
        public int like_state { get; set; }
    }

}
