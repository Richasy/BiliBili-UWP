using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NSDanmaku.Helper
{
    /// <summary>
    /// 弹弹Play相关API操作
    /// </summary>
    public class TanTanPlay
    {
        public TanTanPlay()
        {
            webHelper = new WebHelper();
        }
        WebHelper webHelper;
        /// <summary>
        /// 通过关键字搜索弹弹Play上的剧集
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<List<episodes>> Search(string keyword)
        {

            var results = await webHelper.GetResults(new Uri("https://api.acplay.net/api/v2/search/episodes?anime=" + Uri.EscapeDataString(keyword)));
            var m = JsonConvert.DeserializeObject<TantanSearchModel>(results);
            if (m.success)
            {
                List<episodes> episodes = new List<episodes>();
                foreach (var item in m.animes)
                {
                    foreach (var item1 in item.episodes)
                    {
                        item1.animeTitle = item.animeTitle;
                        episodes.Add(item1);
                    }
                }
                return episodes;
            }
            else
            {
                throw new Exception(m.errorMessage);
            }

        }


        public async Task<List<NSDanmaku.Model.DanmakuModel>> GetDanmakus(int episodeId)
        {
            try
            {
                var results = await webHelper.GetResults(new Uri("https://api.acplay.net/api/v2/comment/" + episodeId));
                var m = JsonConvert.DeserializeObject<CommentModel>(results);
                List<DanmakuModel> list = new List<DanmakuModel>();
                if (m.comments != null)
                {
                    foreach (var item in m.comments)
                    {
                        var datas = item.p.Split(',');
                        var location = DanmakuLocation.Roll;
                        switch (datas[1])
                        {
                            case "7":
                                location = DanmakuLocation.Position;
                                break;
                            case "4":
                                location = DanmakuLocation.Bottom;
                                break;
                            case "5":
                                location = DanmakuLocation.Top;
                                break;
                            default:
                                location = DanmakuLocation.Roll;
                                break;
                        }
                        list.Add(new DanmakuModel()
                        {
                            text = item.m,
                            color = datas[2].ToColor(),
                            location = location,
                            fromSite = DanmakuSite.Tantan,
                            rowID = item.cid.ToString(),
                            time = Convert.ToDouble(datas[0]),
                            sendID = datas[3],
                            size=25
                        });
                    }
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception)
            {

                return new List<DanmakuModel>();
            }
           
        }
    }
}
