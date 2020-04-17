using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI.Interface;
using NSDanmaku.Helper;
using SYEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayerPage : Page, IRefreshPage
    {
        private SystemMediaTransportControls _systemMediaTransportControls;
        private DisplayRequest dispRequest = null;
        private ObservableCollection<VideoPart> VideoPartCollection = new ObservableCollection<VideoPart>();
        private VideoDetail _detail = null;
        private ObservableCollection<VideoDetail> RelatedCollection = new ObservableCollection<VideoDetail>();
        private VideoService _videoService = App.BiliViewModel._client.Video;
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private int _currentPartId = 0;
        private int videoId = 0;
        public static PlayerPage Current;

        public PlayerPage()
        {
            this.InitializeComponent();
            Current = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is int aid)
            {
                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("VideoConnectedAnimation");
                if (anim != null)
                {
                    anim.TryStart(VideoPlayer);
                }
                if (aid == videoId)
                    return;
                else
                {
                    videoId = aid;
                    await Refresh();
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VideoPlayer.Pause();
            base.OnNavigatedFrom(e);
        }
        private void Reset()
        {
            _detail = null;
            VideoPartCollection.Clear();
            RelatedCollection.Clear();
        }
        public async Task Refresh()
        {
            Reset();
            var tip = new WaitingPopup("加载视频中...");
            tip.ShowPopup();
            var detail = await _videoService.GetVideoDetailAsync(videoId);
            if (detail != null)
            {
                _detail = detail;
                _detail.pages.ForEach(p => VideoPartCollection.Add(p));
                _currentPartId = _detail.pages.First().cid;
                InitDetail();
                VideoPlayer.RefreshVideoSource(videoId, _currentPartId);
                CheckVideoStatus();
                await RefreshRelatedVideos();
            }
            tip.HidePopup();
        }

        private async void CheckVideoStatus()
        {
            var list = new List<Task>();
            bool isLike = false;
            bool isCoin = false;
            bool isFavorite = false;
            list.Add(Task.Run(async () =>
            {
                isLike = await _videoService.CheckVideoStatusAsync(videoId);
            }));
            list.Add(Task.Run(async () =>
            {
                isCoin = await _videoService.CheckVideoStatusAsync(videoId, "coin");
            }));
            list.Add(Task.Run(async () =>
            {
                isFavorite = await _videoService.CheckVideoStatusAsync(videoId, "favorite");
            }));
            await Task.WhenAll(list.ToArray());
            LikeButton.IsChecked = isLike;
            CoinButton.IsChecked = isCoin;
            FavoriteButton.IsChecked = isFavorite;
        }

        private async Task RefreshRelatedVideos()
        {
            var related = await _videoService.GetVideoRelatedAsync(videoId);
            if (related != null && related.Count > 0)
            {
                related.Take(10).ToList().ForEach(p => RelatedCollection.Add(p));
            }
            RelateVideoContainer.HolderVisibility = RelatedCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void InitDetail()
        {
            TitleBlock.Text = _detail.title;
            PlayCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.view);
            DanmukuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmaku);
            RepostCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.share);
            ReplyCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.reply);

            DescriptionBlock.Text = _detail.desc;
            if(DescriptionBlock.IsTextTrimmed)
                ToolTipService.SetToolTip(DescriptionBlock, _detail.desc);
            LikeCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.like);
            CoinCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.coin);
            FavoriteCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.favorite);

            UPAvatar.ProfilePicture = string.IsNullOrEmpty(_detail.owner.face) ? null : new BitmapImage(new Uri(_detail.owner.face + "@50w.jpg")) { DecodePixelWidth = 40 };
            UPNameBlock.Text = _detail.owner.name;
        }

        private async void RelateVideoContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as VideoDetail;
            videoId = video.aid;
            await Refresh();
        }

        public void RemovePlayer()
        {
            VideoContainer.Children.Clear();
        }
        public void InsertPlayer()
        {
            VideoContainer.Children.Add(App.AppViewModel.CurrentVideoPlayer);
            App.AppViewModel.CurrentVideoPlayer.Focus(FocusState.Programmatic);
        }

        private void VideoPlayer_FullWindowChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoFullScreen(e);
        }

        private void VideoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            App.AppViewModel.CurrentVideoPlayer = VideoPlayer;
        }
    }
}
