using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Models.Others;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class RepostPopup : UserControl, IAppPopup
    {
        public Popup _popup { get; set; }
        public Guid _popupId { get; set; }
        private ObservableCollection<Owner> AtUserCollection = new ObservableCollection<Owner>();
        private ObservableCollection<Owner> SearchCollection = new ObservableCollection<Owner>();
        private Topic _tempTopic = null;
        private VideoDetail _tempVideo = null;
        private BangumiDetail _tempBangumi = null;
        private Episode _tempPart = null;
        public RepostPopup()
        {
            this.InitializeComponent();
            UIHelper.PopupInit(this);
            AtUserCollection.Add(new Owner());
            var changed = Observable.FromEventPattern<TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>, AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>(
                          handler => UserSearchBox.TextChanged += handler,
                          handler => UserSearchBox.TextChanged -= handler);
            var input = changed
                        .DistinctUntilChanged(temp => temp.Sender.Text)
                        .Throttle(TimeSpan.FromSeconds(0.4));

            var notUserInput = input
                               .ObserveOnDispatcher()
                               .Where(temp => temp.EventArgs.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                               .Select(temp => Task.FromResult<List<Owner>>(null));

            var userInput = input
                            .ObserveOnDispatcher()
                            .Where(temp => temp.EventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                            .Where(temp => !string.IsNullOrEmpty(temp.Sender.Text))
                            .Select((temp) =>
                            {
                                LoadingBar.Visibility = Visibility.Visible;
                                return App.BiliViewModel._client.SearchUserAsync(temp.Sender.Text);
                            });
            var merge = Observable.Merge(notUserInput, userInput).Switch();
            merge.ObserveOnDispatcher().Subscribe(suggestions =>
            {
                if (suggestions != null)
                {
                    SearchCollection.Clear();
                    suggestions.ForEach(p => SearchCollection.Add(p));
                }
                LoadingBar.Visibility = Visibility.Collapsed;
            });
        }
        public void Init(string origin, Topic topic)
        {
            OriginBlock.Text = origin;
            _tempTopic = topic;
        }
        public void Init(string origin, VideoDetail video)
        {
            OriginBlock.Text = origin;
            _tempVideo = video;
        }
        public void Init(string origin, BangumiDetail bangumi, Episode part)
        {
            OriginBlock.Text = origin;
            _tempBangumi = bangumi;
            _tempPart = part;
            RepostBox.Text = $"#{bangumi.season_title}# ";
            RepostBox.Select(RepostBox.Text.Length - 1, 0);
        }
        public void ShowPopup()
        {
            UIHelper.PopupShow(this);
            PopupIn.Begin();
        }
        public void HidePopup()
        {
            _tempTopic = null;
            _tempVideo = null;
            _tempBangumi = null;
            _tempPart = null;
            RepostBox.Text = string.Empty;
            OriginBlock.Text = string.Empty;
            AtUserCollection.Clear();
            AtUserCollection.Add(new Owner());
            SearchCollection.Clear();
            UserSearchBox.Text = string.Empty;
            PopupOut.Begin();
            PopupOut.Completed -= PopupOut_Completed;
            PopupOut.Completed += PopupOut_Completed;
        }
        private void PopupOut_Completed(object sender, object e)
        {
            App.AppViewModel.WindowsSizeChangedNotify.RemoveAll(p => p.Item1 == _popupId);
            _popup.IsOpen = false;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HidePopup();
        }

        private async void RepostButton_Click(object sender, RoutedEventArgs e)
        {
            string content = RepostBox.Text ?? "";
            var atList = new List<RepostLocation>();
            if (AtUserCollection.Count > 1)
            {
                foreach (var item in AtUserCollection)
                {
                    if (item.mid == 0)
                        continue;
                    int locate = content.IndexOf($"@{item.name}");
                    if (locate == -1)
                    {
                        content += $" @{item.name} ";
                        locate= content.IndexOf($"@{item.name}");
                    }
                    int length = item.name.Length + 2;
                    var data = new RepostLocation(locate, length, item.mid);
                    atList.Add(data);
                }
            }
            RepostButton.IsLoading = true;
            bool result = false;
            if (_tempTopic != null)
            {
                result = await App.BiliViewModel._client.Topic.RepostDynamicAsync(content, _tempTopic.desc.dynamic_id_str, _tempTopic.desc.rid_str, _tempTopic.desc.type, atList);
            }
            else if (_tempVideo != null)
            {
                result = await App.BiliViewModel._client.Video.RepostVideoAsync(content, _tempVideo.aid, atList);
            }
            else if (_tempBangumi != null && _tempPart != null)
            {
                result = await App.BiliViewModel._client.Anime.RepostBangumiAsync(content, _tempPart.id, _tempBangumi.type_name, atList);
            }
            if (result)
            {
                new TipPopup("转发成功").ShowMessage();
                HidePopup();
            }
            else
                new TipPopup("转发失败").ShowError();
            RepostButton.IsLoading = false;
        }
        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            var context = (sender as FrameworkElement).DataContext as Owner;
            AtUserCollection.Remove(context);
            RepostBox.Text = RepostBox.Text.Replace($"@{context.name}", "");
        }

        private void UserListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as Owner;
            if (!AtUserCollection.Contains(data))
            {
                AtUserCollection.Add(data);
                RepostBox.Text = RepostBox.Text + $" @{data.name} ";
            }
            SearchUserFlyout.Hide();
            UserSearchBox.Text = string.Empty;
            SearchCollection.Clear();
        }
    }
}
