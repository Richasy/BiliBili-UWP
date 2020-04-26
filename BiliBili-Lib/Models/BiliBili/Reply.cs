using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class Reply
    {
        public long rpid { get; set; }
        public long oid { get; set; }
        public int type { get; set; }
        public long mid { get; set; }
        public long root { get; set; }
        public long parent { get; set; }
        public long dialog { get; set; }
        public int count { get; set; }
        public int rcount { get; set; }
        public int floor { get; set; }
        public int state { get; set; }
        public int fansgrade { get; set; }
        public int attr { get; set; }
        public int ctime { get; set; }
        public string rpid_str { get; set; }
        public string root_str { get; set; }
        public string parent_str { get; set; }
        public int like { get; set; }
        public int action { get; set; }
        public Member member { get; set; }
        public ReplyContent content { get; set; }
        public List<Reply> replies { get; set; }
        public int assist { get; set; }
        public Folder folder { get; set; }
        public UpAction up_action { get; set; }
        public bool show_follow { get; set; }
        
        public class ReplyContent
        {
            public string message { get; set; }
            public int plat { get; set; }
            public string device { get; set; }
            public List<Author> members { get; set; }
            public int max_line { get; set; }
            public Dictionary<string, Emote> emote { get; set; }
        }

        public class Folder
        {
            public bool has_folded { get; set; }
            public bool is_folded { get; set; }
            public string rule { get; set; }
        }

        public class UpAction
        {
            public bool like { get; set; }
            public bool reply { get; set; }
        }
        public class Member
        {
            public int mid { get; set; }
            public string uname { get; set; }
            public string avatar { get; set; }
            public string sex { get; set; }
            public string sign { get; set; }
            public Pendant pendant { get; set; }
            public Vip vip { get; set; }
            public LevelInfo level_info { get; set; }
        }
        public string render_content
        {
            get => ToString();
        }
        public override string ToString()
        {
            return $"`{member.uname}` : {content.message}";
        }

        public override bool Equals(object obj)
        {
            return obj is Reply reply &&
                   rpid == reply.rpid;
        }

        public override int GetHashCode()
        {
            return -1199556862 + rpid.GetHashCode();
        }
    }

    public class ReplyDetailResponse
    {
        public Cursor cursor { get; set; }
        public bool show_bvid { get; set; }
        public class Cursor
        {
            public bool is_begin { get; set; }
            public bool is_end { get; set; }
            public int next { get; set; }
            public int prev { get; set; }
        }
        public Reply root { get; set; }
    }
}
