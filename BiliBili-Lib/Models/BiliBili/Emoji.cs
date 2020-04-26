using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class EmojiReplyContainer
    {
        public int id { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public int mtime { get; set; }
        public int type { get; set; }
        public int attr { get; set; }
        public List<Emote> emote { get; set; }
    }

    public class EmojiContainer
    {
        public int pid { get; set; }
        public string pname { get; set; }
        public int pstate { get; set; }
        public string purl { get; set; }
        public List<EmojiItem> emojis { get; set; }
    }

    public class EmojiItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public int state { get; set; }
        public string remark { get; set; }
        public string size { get; set; }
    }

    public class Emote
    {
        public int id { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public int state { get; set; }
        public Meta meta { get; set; }
        public class Meta
        {
            public int size { get; set; }
        }
    }

}
