using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Favorites;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages_Share.Sub.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoriteDetailPage : Page, IRefreshPage
    {
        private ObservableCollection<FavoriteVideo> VideoCollection = new ObservableCollection<FavoriteVideo>();
        private List<FavoriteId> IdList = new List<FavoriteId>();
        private AccountService _accountService = App.BiliViewModel._client.Account;
        private int _favoriteId = 0;
        private int index = 0;
        private bool isRequesting = false;
        public FavoriteDetailPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is FavoriteItem item)
            {
                if (_favoriteId == item.id)
                    return;
                _favoriteId = item.id;
                App.AppViewModel.CurrentSubPageControl.SubPageTitle = item.title;
                await Refresh();
            }
        }
        public async Task Refresh()
        {
            VideoCollection.Clear();
            IdList.Clear();
            index = 0;

            if (App.BiliViewModel.CheckAccoutStatus())
            {
                var ids = await _accountService.GetFavoriteIdsAsync(_favoriteId);
                if (ids != null && ids.Count > 0)
                {
                    ids.ForEach(p => IdList.Add(p));
                    await LoadMoreVideo();
                }
            }
            HolderText.Visibility = VideoCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task LoadMoreVideo()
        {
            if (isRequesting)
                return;
            if (index >= IdList.Count)
                return;
            LoadingBar.Visibility = Visibility.Visible;
            HolderText.Visibility = Visibility.Collapsed;
            isRequesting = true;
            var items = IdList.Skip(index).Take(20);
            var videos = await _accountService.GetFavoriteVideosAsync(items);
            if (videos != null && videos.Count > 0)
            {
                index += items.Count();
                videos.ForEach(p => VideoCollection.Add(p));
            }
            isRequesting = false;
            LoadingBar.Visibility = Visibility.Collapsed;
            HolderText.Visibility = VideoCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void FavoriteListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as FavoriteVideo;
            if (video.attr == 1)
                new TipPopup("该视频可能已经失效，无法播放").ShowError();
            else
                App.AppViewModel.PlayVideo(video.id);
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                await LoadMoreVideo();
            }
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarButton;
            btn.IsEnabled = false;
            var context = btn.Tag as FavoriteVideo;
            bool result = await App.BiliViewModel._client.Account.RemoveFavoriteVideoAsync(context.id, context.type, _favoriteId);
            if (result)
            {
                new TipPopup("移除成功").ShowMessage();
                VideoCollection.Remove(context);
            }
            else
            {
                new TipPopup("移除失败").ShowError();
            }
            btn.IsEnabled = true;
        }

        private void FavoriteListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            DefaultVideoPanel card = (DefaultVideoPanel)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
