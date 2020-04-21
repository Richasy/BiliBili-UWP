using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class CourseDynamic
    {
        public Badge badge { get; set; }
        public string cover { get; set; }
        public int ep_count { get; set; }
        public int id { get; set; }
        public NewEp new_ep { get; set; }
        public string subtitle { get; set; }
        public string title { get; set; }
        public int up_id { get; set; }
        public UpInfo up_info { get; set; }
        public int update_count { get; set; }
        public string url { get; set; }
        public UserProfile user_profile { get; set; }
        public class NewEp
        {
            public string cover { get; set; }
            public int id { get; set; }
            public int reply { get; set; }
            public string title { get; set; }
        }
        public class UpInfo
        {
            public string avatar { get; set; }
            public string name { get; set; }
        }

        public class UserProfile
        {
            public Card card { get; set; }
            public SlimUserInfo info { get; set; }
            public Pendant pendant { get; set; }
            public string rank { get; set; }
            public string sign { get; set; }
            public Vip vip { get; set; }
        }
    }
}
