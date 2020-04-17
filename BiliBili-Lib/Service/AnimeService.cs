using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Service
{
    public class AnimeService
    {
        /// <summary>
        /// 获取动漫区块综合信息
        /// </summary>
        /// <param name="isJp"><c>true</c>代表番剧，<c>false</c>代表国创</param>
        /// <returns></returns>
        public async Task<List<AnimeModule>> GetAnimeSquareAsync(bool isJp = true)
        {
            string api = isJp ? Api.ANIME_JP_SQUARE : Api.ANIME_CHN_SQUARE;
            var url = BiliTool.UrlContact(api, null, true);
            return await BiliTool.ConvertEntityFromWebAsync<List<AnimeModule>>(url, "result.modules");
        }
        /// <summary>
        /// 获取区块刷新信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="moduleId">模块ID</param>
        /// <returns></returns>
        public async Task<List<AnimeModuleItem>> GetAnimeSectionExchange(int type,int moduleId)
        {
            var param = new Dictionary<string, string>();
            param.Add("type", type.ToString());
            param.Add("oid", moduleId.ToString());
            string url = BiliTool.UrlContact(Api.ANIME_EXCHANGE, param, true);
            return await BiliTool.ConvertEntityFromWebAsync<List<AnimeModuleItem>>(url, "result");
        }
    }
}
