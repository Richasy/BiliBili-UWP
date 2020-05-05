using BiliBili_Lib.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Account
{
    public class RelationUser
    {
        public int mid { get; set; }
        public string uname { get; set; }
        public string face { get; set; }
        public string sign { get; set; }
        public OfficialVerify official_verify { get; set; }
        public Vip vip { get; set; }
        public override bool Equals(object obj)
        {
            return obj is RelationUser user &&
                   mid == user.mid;
        }

        public override int GetHashCode()
        {
            return 1557962925 + mid.GetHashCode();
        }
    }
    public class FanUser : RelationUser
    {
        public int attribute { get; set; }
        public int mtime { get; set; }
        public object tag { get; set; }
        public int special { get; set; }
    }

    public class FollowTag:NotifyBase
    {
        public int tagid { get; set; }
        public string name { get; set; }
        private int _count;
        public int count
        {
            get => _count;
            set => Set(ref _count, value);
        }
        public string tip { get; set; }
    }

    public class FanResponse
    {
        public List<FanUser> list { get; set; }
        public int total { get; set; }
        public long re_version { get; set; }
    }

}
