using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weakly;
using Windows.UI.Xaml;

namespace BiliBili_UWP.Models.Core
{
    public partial class BiliViewModel
    {
        public ObservableCollection<ChannelView> MyScanedChannelCollection = new ObservableCollection<ChannelView>();
        public ObservableCollection<ChannelSlim> MySubscribeChannelCollection = new ObservableCollection<ChannelSlim>();
        public event EventHandler<bool> MyChannelChanged;
        public bool _isChannelChanged = false;
        private bool _isChannelRequesting = false;
        private DispatcherTimer _channelChangeTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };

        

        /// <summary>
        /// 获取频道页信息
        /// </summary>
        /// <returns></returns>
        public async Task GetChannelSquareAsync()
        {
            var data = await _client.Channel.GetSquareAsync();
            if (data == null)
                return;
            MySubscribeChannelCollection.Clear();
            var mySub = data.Subscribes.Distinct();
            if (!App._isTabletMode)
                mySub = mySub.Take(6);
            mySub.ForEach(p => MySubscribeChannelCollection.Add(p));
            if (data.Scaneds != null)
            {
                if (MyScanedChannelCollection.Count == 0)
                    data.Scaneds.ForEach(p => MyScanedChannelCollection.Add(p));
                else
                {
                    // 插值更新，避免因清空数据造成明显的列表闪烁
                    for (int i = MyScanedChannelCollection.Count - 1; i >= 0; i--)
                    {
                        var item = MyScanedChannelCollection[i];
                        if (!data.Scaneds.Contains(item))
                            MyScanedChannelCollection.RemoveAt(i);
                        else
                        {
                            var source = data.Scaneds.Where(p => p.Equals(item)).First();
                            item.position = source.position;
                        }
                    }
                    foreach (var item in data.Scaneds)
                    {
                        if (!MyScanedChannelCollection.Contains(item))
                            MyScanedChannelCollection.Insert(item.position - 1, item);
                    }
                }
                MyChannelChanged?.Invoke(this, mySub.Count() > 6);
            }
        }
        /// <summary>
        /// 频道更改定时任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChannelChangTimer_Tick(object sender, object e)
        {
            if (_isChannelRequesting)
                return;
            if (_isChannelChanged)
            {
                _isChannelRequesting = true;
                await GetChannelSquareAsync();
                _isChannelChanged = false;
                _isChannelRequesting = false;
            }
        }
    }
}
