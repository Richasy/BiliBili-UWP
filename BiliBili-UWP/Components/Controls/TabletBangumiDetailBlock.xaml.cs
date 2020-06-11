using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class TabletBangumiDetailBlock : UserControl,IPlayerHost
    {
        private ObservableCollection<Episode> BangumiPartCollection = new ObservableCollection<Episode>();
        private ObservableCollection<BangumiStyle> TagCollection = new ObservableCollection<BangumiStyle>();
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        public BangumiDetail _detail = null;
        private Episode _currentPart = null;
        private int bangumiId = 0;
        private bool isEp = false;
        public VideoPlayer VideoPlayer;
        public TabletBangumiDetailBlock()
        {
            this.InitializeComponent();
        }

        private void Reset()
        {
            _detail = null;
            _currentPart = null;
            BangumiPartCollection.Clear();
            TagCollection.Clear();

            CoinButton.IsCheck = false;

            TitleBlock.Text = "--";
            PlayCountBlock.Text = "-";
            DanmakuCountBlock.Text = "-";
            CommentButton.Text = "-";
            RepostButton.Text = "-";
            BasicInfoBlock.Text = "-";

            DescriptionBlock.Text = "--";
            ScoreBlock.Text = "--";
            ScoreCountBlock.Text = "0人";
            RatingContainer.Visibility = Visibility.Visible;

            FollowButton.Style = UIHelper.GetStyle("PrimaryAsyncButtonStyle");
            FollowButton.Text = "追番";
        }
        public async Task Init(int bangumiId, bool isep=false)
        {
            LoadingRing.IsActive = true;
            MyVideoPlayer.Close();
            isEp = isep;
            DetailContainer.Visibility = Visibility.Collapsed;
            var detail = await _animeService.GetBangumiDetailAsync(bangumiId, isEp);
            LoadingRing.IsActive = false;
            DetailContainer.Visibility = Visibility.Visible;
            if (detail != null)
            {
                await Init(detail, bangumiId);
            }
        }

        public async Task<bool> Init(BangumiDetail detail, int partId)
        {
            Reset();
            App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.Bangumi;
            App.AppViewModel.CurrentVideoPlayer = VideoPlayer;
            _detail = detail;
            TitleBlock.Text = _detail.title;
            PlayCountBlock.Text = _detail.stat.play;
            TabletMainPage.Current.SetBackgroundImage(_detail.square_cover ?? _detail.cover);
            DanmakuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmakus);
            RepostButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.share);
            CommentButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.reply);

            DescriptionBlock.Text = _detail.evaluate;
            ToolTipService.SetToolTip(DescriptionBlock, _detail.evaluate);

            if (_detail.rating != null)
            {
                ScoreBlock.Text = _detail.rating.score.ToString();
                ScoreCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.rating.count) + "人";
            }
            else
                RatingContainer.Visibility = Visibility.Collapsed;

            BasicInfoBlock.Text = $"{_detail.type_desc}\n{_detail.publish.release_date_show}\n{_detail.publish.time_length_show}";

            CoinButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.coins);
            if(_detail.episodes!=null && _detail.episodes.Count > 0)
            {
                PartGridView.Visibility = Visibility.Visible;
                _detail.episodes.ForEach(p => BangumiPartCollection.Add(p));
            }
            else
            {
                PartGridView.Visibility = Visibility.Collapsed;
            }
            if (isEp)
            {
                for (int i = 0; i < _detail.episodes.Count; i++)
                {
                    var part = _detail.episodes[i];
                    if (part.id == bangumiId)
                    {
                        _currentPart = part;
                        PartGridView.SelectedIndex = i;
                        PartGridView.ScrollIntoView(part, ScrollIntoViewAlignment.Leading);
                        break;
                    }
                }
                bangumiId = _detail.season_id;
            }
            else if (_detail.user_status.progress != null)
            {
                _currentPart = _detail.episodes.Where(p => p.id == _detail.user_status.progress.last_ep_id).FirstOrDefault();
                if (_currentPart != null)
                {
                    int lastIndex = _detail.episodes.IndexOf(_currentPart);
                    PartGridView.SelectedIndex = lastIndex - 1 < -1 ? -1 : lastIndex;
                    PartGridView.ScrollIntoView(_currentPart, ScrollIntoViewAlignment.Leading);
                }
            }

            if (_currentPart == null && _detail.episodes.Count > 0)
            {
                _currentPart = _detail.episodes.First();
                PartGridView.SelectedIndex = 0;
            }
            if (_detail.styles != null && _detail.styles.Count > 0)
            {
                TagGridView.Visibility = Visibility.Visible;
                _detail.styles.ForEach(p => TagCollection.Add(p));
            }
            else
            {
                TagGridView.Visibility = Visibility.Collapsed;
            }

            CheckFollowButton();

            if (_detail.limit != null)
            {
                await new ConfirmDialog(_detail.limit.content).ShowAsync();
                return false;
            }
            await VideoPlayer.Init(_detail, _currentPart);
            return true;
        }
        private void CheckFollowButton()
        {
            if (_detail.user_status != null)
            {
                string followString = "已追剧";
                string unfollowString = "追剧";
                if (_detail.type == 1 || _detail.type == 2 || _detail.type == 4)
                {
                    followString = "已追番";
                    unfollowString = "追番";
                }
                FollowButton.Style = _detail.user_status.follow == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
                FollowButton.Text = _detail.user_status.follow == 1 ? followString : unfollowString;
            }
        }
        private async void CheckCoin()
        {
            if (_currentPart != null)
            {
                bool isCoin = await _animeService.CheckUserCoinAsync(_currentPart.id);
                CoinButton.IsCheck = isCoin;
            }
        }
        public void RemovePlayer()
        {
            VideoContainer.Children.Clear();
        }
        public void InsertPlayer()
        {
            if (VideoContainer.Children.Count == 0)
                VideoContainer.Children.Add(App.AppViewModel.CurrentVideoPlayer);
            App.AppViewModel.CurrentVideoPlayer.Focus(FocusState.Programmatic);
            App.AppViewModel.CurrentVideoPlayer.ResetDanmakuStatus();
        }
        private void VideoPlayer_FullWindowChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoFullScreen(e);
        }

        private void VideoPlayer_CinemaChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCinema(e);
        }
        private async void CoinCountButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int coin = Convert.ToInt32(btn.Tag.ToString());
            CoinButton.IsEnabled = false;
            bool result = await App.BiliViewModel._client.Video.GiveCoinToVideoAsync(_currentPart.aid, coin, false);
            if (result)
            {
                new TipPopup("成功投币！").ShowMessage();
                CoinFlyout.Hide();
            }
            else
                new TipPopup("投币失败").ShowError();
            CoinButton.IsCheck = result;
            CoinButton.IsEnabled = true;
        }

        private void CoinButton_Click(object sender, EventArgs e)
        {
            if (CoinButton.IsCheck || _currentPart == null)
                return;
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                FlyoutBase.ShowAttachedFlyout(CoinButton);
            }
        }

        private async void PartGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Episode;
            if (item.cid != _currentPart?.cid)
            {
                _currentPart = item;
                await VideoPlayer.Init(_detail, _currentPart);
            }
        }
        private void TagGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as BangumiStyle;
            string param = item.url.Split("?").Last().TrimStart('?');
            QueryString args = QueryString.Parse(param);
            var list = new List<KeyValueModel>();
            if (args.Count() > 0)
            {
                foreach (var query in args)
                {
                    list.Add(new KeyValueModel(query.Name, query.Value));
                }
                list.Add(new KeyValueModel("season_type", _detail.type.ToString()));
            }
            if (list.Count > 0)
            {
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Anime.IndexPage), list);
            }
        }
        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            if (_detail.user_status.follow == 1)
            {
                result = await _animeService.UnfollowBangumiAsync(_detail.season_id);
                if (result)
                {
                    _detail.user_status.follow = 0;
                    new TipPopup("已取消追番").ShowMessage();
                }
            }
            else
            {
                result = await _animeService.FollowBangumiAsync(_detail.season_id);
                if (result)
                {
                    _detail.user_status.follow = 1;
                    new TipPopup("追番成功").ShowMessage();
                }
            }
            CheckFollowButton();
            if (!result)
                new TipPopup("操作失败").ShowError();
        }

        private void VideoPlayer_CompactOverlayChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCompactOverlay(e);
        }

        private void VideoPlayer_SeparateButtonClick(object sender, RoutedEventArgs e)
        {
            if (_currentPart != null)
            {
                VideoPlayer.Close();
                App.AppViewModel.PlayVideoSeparate(_detail, _currentPart);
            }
        }

        private void VideoPlayer_PartSwitched(object sender, int e)
        {
            PartGridView.SelectedIndex = e;
            PartGridView.ScrollIntoView(BangumiPartCollection[e], ScrollIntoViewAlignment.Leading);
        }

        private void CommentButton_Click(object sender, EventArgs e)
        {
            if (_currentPart != null)
            {
                var param = new Dictionary<string, string>();
                param.Add("oid", _currentPart.aid.ToString());
                param.Add("type", "1");
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.ReplyPage), param);
            }
        }

        private void ShareDynamicButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.BiliViewModel.CheckAccoutStatus())
                return;
            string content = _currentPart.share_copy;
            App.AppViewModel.ShowRepostPopup(content, _detail, _currentPart);
        }

        private void ShareDataButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = _currentPart.share_copy;
            request.Data.Properties.Description = _detail.evaluate;
            if (!string.IsNullOrEmpty(_detail.evaluate))
                request.Data.SetText(_detail.evaluate);
            request.Data.SetWebLink(new Uri(_currentPart.share_url));
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(_currentPart.cover)));
        }

        private void RepostButton_Click(object sender, EventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            }
        }
        private void MyVideoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            VideoPlayer = MyVideoPlayer;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1000)
            {
                VisualStateManager.GoToState(this, "Narrow", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Wide", true);
            }
        }
    }
}
