using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.Others
{
    public class TokenPackage
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
        public int Expiry { get; set; }
        public TokenPackage()
        {
            AccessToken = "";
            RefreshToken = "";
            UserId = "";
            Expiry = 0;
        }
        public TokenPackage(string acc,string refe,string uid,int exp)
        {
            AccessToken = acc;
            RefreshToken = refe;
            UserId = uid;
            Expiry = exp;
        }
    }
}
