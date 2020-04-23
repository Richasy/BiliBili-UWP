using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.Others
{
    public class KeyValueModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public KeyValueModel()
        {

        }
        public KeyValueModel(string key,string value)
        {
            Key = key;
            Value = value;
        }
    }
}
