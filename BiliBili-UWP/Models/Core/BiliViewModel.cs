using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_UWP.Models.Core
{
    public class BiliViewModel
    {
        public ObservableCollection<Region> RegionCollection = new ObservableCollection<Region>();
        public BiliViewModel()
        {

        }

        /// <summary>
        /// 获取分区
        /// </summary>
        /// <returns></returns>
        public async Task GetRegionsAsync()
        {
            string url = string.Format("https://app.bilibili.com/x/v2/region/index?appkey={0}&build={2}&mobi_app=android&platform=android&ts={1}", BiliTool.AndroidKey.Appkey, AppTool.DateToTimeStamp(DateTime.Now), BiliTool.BuildNumber);
            url += "&sign=" + BiliTool.GetSign(url);
            var result = await BiliTool.ConvertEntityFromWebAsync<List<Region>>(url);
            if (result != null)
            {
                result.RemoveAll(p => p.children == null || p.children.Count == 0);
                RegionCollection.Clear();
                result.ForEach(p => RegionCollection.Add(p));
            }
        }
    }
}
