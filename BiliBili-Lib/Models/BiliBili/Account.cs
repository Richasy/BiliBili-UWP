using BiliBili_Lib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class Vip
    {
        public int type { get; set; }
        public int status { get; set; }
        public string due_date { get; set; }
    }
    public class Pendant
    {
        public string image { get; set; }
    }
    public class Me
    {
        public int mid { get; set; }
        public string name { get; set; }
        public string sign { get; set; }
        public double coins { get; set; }
        public DateTime birthday { get; set; }
        private string _face;

        public string face
        {
            get
            {
                return _face + "@100w.jpg";
            }
            set { _face = value; }
        }
        public int sex { get; set; }
        public int level { get; set; }
        public int rank { get; set; }
        public int silence { get; set; }
        public Vip vip { get; set; }
        public int email_status { get; set; }
        public int tel_status { get; set; }
        public Pendant pendant { get; set; }
        public string Sex
        {
            get
            {
                switch (sex)
                {
                    case 0:
                        return "保密";
                    case 1:
                        return "男";
                    case 2:
                        return "女";
                    default:
                        return "保密";
                }
            }
        }
        public int dynamic { get; set; }
        public int follower { get; set; }
        public int following { get; set; }
        public int new_followers { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Me me &&
                   mid == me.mid;
        }

        public override int GetHashCode()
        {
            return 1557962925 + mid.GetHashCode();
        }
    }


    public class UserStat
    {
        public int mid { get; set; }
        public int following { get; set; }
        public int whisper { get; set; }
        public int black { get; set; }
        public int follower { get; set; }
    }
    public class SlimUserInfo
    {
        public int uid { get; set; }
        public string uname { get; set; }
        public string face { get; set; }
    }
    public class LevelInfo
    {
        public int current_level { get; set; }
        public int current_min { get; set; }
        public int current_exp { get; set; }
        public string next_exp { get; set; }
    }

    public class MyMessage
    {
        public int at { get; set; }
        public int chat { get; set; }
        public int like { get; set; }
        public int reply { get; set; }
        public int sys_msg { get; set; }
        public int up { get; set; }
    }
    public class OfficialVerify
    {
        public int type { get; set; }
        public string desc { get; set; }
    }
}
