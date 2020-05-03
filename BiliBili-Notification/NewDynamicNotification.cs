using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BiliBili_Notification
{
    public sealed class NewDynamicNotification : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var def = taskInstance.GetDeferral();
            string _access = AppTool.GetLocalSetting(Settings.AccessToken, "");
            string _refresh = AppTool.GetLocalSetting(Settings.RefreshToken, "");
            int _expiry = Convert.ToInt32(AppTool.GetLocalSetting(Settings.TokenExpiry, "0"));
            if (!string.IsNullOrEmpty(_access))
            {
                var _client = new BiliBiliClient(_access, _refresh, _expiry);
                string lastId = AppTool.GetLocalSetting(Settings.LastSeemDynamicId, "0");
                var newDynamic = await _client.Topic.GetNewDynamicAsync(lastId.ToString());
                if (newDynamic.update_num > 0)
                {
                    var toastItems = new List<NotificationModel>();
                    var lastItem = newDynamic.cards.Where(p => p.desc.dynamic_id_str == lastId.ToString()).FirstOrDefault();
                    int lastStamp = 0;
                    if (lastItem != null)
                        lastStamp = lastItem.desc.timestamp;
                    foreach (var item in newDynamic.cards)
                    {
                        if (item.desc.timestamp > lastStamp && (item.desc.type == 8 || item.desc.type == 512))
                        {
                            if (item.desc.type == 8)
                            {
                                var video = JsonConvert.DeserializeObject<VideoDynamic>(item.card);
                                toastItems.Add(new NotificationModel(video.title, video.desc, video.pic, $"action=video&aid={video.aid}", video.owner.face));
                            }
                            else
                            {
                                var anime = JsonConvert.DeserializeObject<AnimeDynamic>(item.card);
                                toastItems.Add(new NotificationModel(anime.show_title, anime.season.type_name, anime.cover, $"action=bangumi&epid={anime.episode_id}"));
                            }
                        }
                    }
                    if (toastItems.Count > 0)
                        NotificationTool.SendDynamicToast(toastItems);
                }
            }
            def.Complete();
        }
    }
}
