using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.Others
{
    public class ApiKeyInfo
    {
        public ApiKeyInfo(string key, string secret)
        {
            Appkey = key;
            Secret = secret;
        }
        public string Appkey { get; set; }
        public string Secret { get; set; }
    }
}
