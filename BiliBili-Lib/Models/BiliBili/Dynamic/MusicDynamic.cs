using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class MusicDynamic
    {
        public int id { get; set; }
        public int upId { get; set; }
        public string title { get; set; }
        public string upper { get; set; }
        public string cover { get; set; }
        public string author { get; set; }
        public long ctime { get; set; }
        public int replyCnt { get; set; }
        public int playCnt { get; set; }
        public string intro { get; set; }
        public string schema { get; set; }
        public string typeInfo { get; set; }
        public string upperAvatar { get; set; }
    }
}
