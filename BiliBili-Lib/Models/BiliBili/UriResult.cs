using BiliBili_Lib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class UriResult
    {
        public UriType Type { get; set; }
        public string Param { get; set; }
        public UriResult()
        {

        }
        public UriResult(UriType t,string p)
        {
            Type = t;
            Param = p;
        }
    }
}
