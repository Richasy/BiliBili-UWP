using BiliBili_Lib.Models.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili.Anime
{
    public class IndexCondition
    {
        public List<ConditionContainer> filter { get; set; }
        public List<ConditionContainer> order { get; set; }
    }
    public class ConditionContainer:NotifyBase
    {
        public string field { get; set; }
        public string name { get; set; }
        public string sort { get; set; }
        public List<ConditionItem> values { get; set; }
        private int _selectIndex = 0;
        public int SelectIndex
        {
            get => _selectIndex;
            set => Set(ref _selectIndex, value);
        }

        public override bool Equals(object obj)
        {
            return obj is ConditionContainer container &&
                   field == container.field;
        }

        public override int GetHashCode()
        {
            return -306121345 + EqualityComparer<string>.Default.GetHashCode(field);
        }
    }
    public class ConditionItem
    {
        public string keyword { get; set; }
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ConditionItem item &&
                   keyword == item.keyword;
        }

        public override int GetHashCode()
        {
            return -1834187100 + EqualityComparer<string>.Default.GetHashCode(keyword);
        }
    }

    public class AnimeIndexResult
    {
        public string badge { get; set; }
        public Badge badge_info { get; set; }
        public int badge_type { get; set; }
        public string cover { get; set; }
        public string index_show { get; set; }
        public int is_finish { get; set; }
        public string link { get; set; }
        public int media_id { get; set; }
        public string order { get; set; }
        public string order_type { get; set; }
        public int season_id { get; set; }
        public string title { get; set; }
        public string title_icon { get; set; }
    }
}
