using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Feedback
{
    public class FeedAtResponse
    {
        public FeedCursor cursor { get; set; }
        public List<FeedAtDetail> items { get; set; }
    }

    public class FeedAtDetail:FeedDetail
    {
        public FeedItem item { get; set; }
        public int at_time { get; set; }
    }
}
