using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_UWP.Models.UI;
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

namespace BiliBili_UWP.Pages_Share.Sub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page, IRefreshPage
    {
        private string _keyword = "";
        private ObservableCollection<SearchTab> TabCollection = new ObservableCollection<SearchTab>();
        private ObservableCollection<SearchVideo> VideoCollection = new ObservableCollection<SearchVideo>();
        private ObservableCollection<SearchAnime> AnimeCollection = new ObservableCollection<SearchAnime>();
        private ObservableCollection<SearchUser> UserCollection = new ObservableCollection<SearchUser>();
        private ObservableCollection<SearchAnime> MovieCollection = new ObservableCollection<SearchAnime>();
        private ObservableCollection<SearchDocument> DocumentCollection = new ObservableCollection<SearchDocument>();

        private List<IconItem> VideoSortList = IconItem.GetSearchVideoSortItems();
        private List<IconItem> VideoDurationList = IconItem.GetSearchVideoDurationSortItems();
        private List<IconItem> VideoRegionList = IconItem.GetSearchVideoRegionSortItems();
        private List<IconItem> UserSortList = IconItem.GetSearchUserSortItems();
        private List<IconItem> UserTypeList = IconItem.GetSearchUserTypeSortItems();
        private List<IconItem> DocumentSortList = IconItem.GetSearchDocumentSortItems();
        private List<IconItem> DocumentRegionList = IconItem.GetSearchDocumentRegionSortItems();

        private BiliBiliClient _client = App.BiliViewModel._client;
        private string videoSort = "default";
        private string videoDuration = "0";
        private string videoRegion = "0";
        private string animeSort = "totalrank";
        private string movieSort = "totalrank";
        private string userSort = "totalrank_0";
        private string userType = "0";
        private string documentSort = "";
        private string documentRegion = "0";

        private int videoPage = 1;
        private int animePage = 1;
        private int userPage = 1;
        private int moviePage = 1;
        private int documentPage = 1;

        private bool _isVideoRequesting = false;
        private bool _isAnimeRequesting = false;
        private bool _isUserRequesting = false;
        private bool _isMovieRequesting = false;
        private bool _isDocumentRequesting = false;

        private string _currentTab = "video";
        private bool _isInit = false;
        public SearchPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = StaticString.SEARCH_RESULT;
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is string _key)
            {
                _keyword = _key;
                await Refresh();

            }
        }

        public async Task Refresh()
        {
            LoadingRing.IsActive = true;
            Reset();
            await RefreshVideo();
            _isInit = true;
            LoadingRing.IsActive = false;
        }

        private void Reset()
        {
            _isInit = false;
            videoSort = "default";
            videoDuration = "0";
            videoRegion = "0";
            animeSort = "totalrank";
            movieSort = "totalrank";
            userSort = "totalrank_0";
            userType = "0";
            documentSort = "";
            documentRegion = "0";

            videoPage = 1;
            animePage = 1;
            userPage = 1;
            moviePage = 1;
            documentPage = 1;

            VideoCollection.Clear();
            AnimeCollection.Clear();
            UserCollection.Clear();
            MovieCollection.Clear();
            DocumentCollection.Clear();

            TabListView.SelectedIndex = -1;
            VideoSortComboBox.SelectedIndex = 0;
            VideoDurationComboBox.SelectedIndex = 0;
            VideoRegionComboBox.SelectedIndex = 0;
            UserSortComboBox.SelectedIndex = 0;
            UserTypeComboBox.SelectedIndex = 0;
            DocumentSortComboBox.SelectedIndex = 0;
            DocumentRegionComboBox.SelectedIndex = 0;

            VideoContainer.Visibility = Visibility.Visible;
            UserContainer.Visibility = Visibility.Collapsed;
            AnimeContainer.Visibility = Visibility.Collapsed;
            MovieContainer.Visibility = Visibility.Collapsed;
            DocumentContainer.Visibility = Visibility.Collapsed;

            _currentTab = "综合";
        }

        private async Task RefreshVideo(bool isClear = true)
        {
            if (isClear)
            {
                videoPage = 1;
                VideoCollection.Clear();
                LoadingRing.IsActive = true;
            }
            else
            {
                videoPage += 1;
                LoadingBar.Visibility = Visibility.Visible;
            } 
            var complex = await _client.GetComplexSearchResult(_keyword, videoSort, videoPage, videoRegion, videoDuration);
            if (complex != null)
            {
                var tabs = complex.nav;
                if (TabCollection.Count == 0)
                {
                    TabCollection.Add(new SearchTab() { name = "综合", total = 0, pages = 1, type = -1 });
                    tabs.Where(p => p.type != 4).ToList().ForEach(p => TabCollection.Add(p));
                    TabListView.SelectedIndex = 0;
                }
                else
                {
                    foreach (var tab in TabCollection)
                    {
                        var source = tabs.Where(p => p.name == tab.name).FirstOrDefault();
                        if (source != null)
                        {
                            tab.total = source.total;
                            tab.pages = source.pages;
                        }
                    }
                }
                var items = complex.item.Where(p => p.linktype == "video").ToList();
                items.ForEach(p => VideoCollection.Add(p));
            }
            VideoHolderBlock.Visibility = VideoCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }
        private async Task RefreshAnime(bool isClear = true)
        {
            var tab = TabCollection.Where(p => p.name == "番剧").FirstOrDefault();
            if (!isClear && tab.total <= AnimeCollection.Count)
            {
                AnimeHolderBlock.Visibility = AnimeCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                return;
            }
            if (isClear)
            {
                animePage = 1;
                AnimeCollection.Clear();
                LoadingRing.IsActive = true;
            }
            else
            {
                animePage += 1;
                LoadingBar.Visibility = Visibility.Visible;
            }
            var type = tab.type;
            var param = new Dictionary<string, string>();
            param.Add("order_sort", "1");
            param.Add("user_type", "0");
            var data = await _client.SearchTypeItems<List<SearchAnime>>(_keyword, type, animeSort, animePage, param);
            if (data != null && data.Count > 0)
            {
                data.ForEach(p => AnimeCollection.Add(p));
            }
            AnimeHolderBlock.Visibility = AnimeCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }
        private async Task RefreshUser(bool isClear = true)
        {
            var tab = TabCollection.Where(p => p.name == "用户").FirstOrDefault();
            if (!isClear && tab.total <= UserCollection.Count)
            {
                UserHolderBlock.Visibility = UserCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                return;
            }
            if (isClear)
            {
                userPage = 1;
                UserCollection.Clear();
                LoadingRing.IsActive = true;
            }
            else
            {
                LoadingBar.Visibility = Visibility.Visible;
                userPage += 1;
            }
            var type = tab.type;
            var param = new Dictionary<string, string>();
            var sp = userSort.Split("_");
            param.Add("order_sort", sp[1]);
            param.Add("user_type", userType);
            var data = await _client.SearchTypeItems<List<SearchUser>>(_keyword, type, sp[0], userPage, param);
            if (data != null && data.Count > 0)
            {
                data.ForEach(p => UserCollection.Add(p));
            }
            UserHolderBlock.Visibility = UserCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }
        private async Task RefreshMovie(bool isClear = true)
        {
            var tab = TabCollection.Where(p => p.name == "影视").FirstOrDefault();
            if (!isClear && tab.total <= MovieCollection.Count)
            {
                MovieHolderBlock.Visibility = MovieCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                return;
            }
            if (isClear)
            {
                moviePage = 1;
                MovieCollection.Clear();
                LoadingRing.IsActive = true;
            }
            else
            {
                LoadingBar.Visibility = Visibility.Visible;
                moviePage += 1;
            }
            var type = tab.type;
            var data = await _client.SearchTypeItems<List<SearchAnime>>(_keyword, type, movieSort, moviePage);
            if (data != null && data.Count > 0)
            {
                data.ForEach(p => MovieCollection.Add(p));
            }
            MovieHolderBlock.Visibility = MovieCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }
        private async Task RefreshDocument(bool isClear = true)
        {
            var tab = TabCollection.Where(p => p.name == "专栏").FirstOrDefault();
            if (!isClear && tab.total <= DocumentCollection.Count)
            {
                DocumentHolderBlock.Visibility = DocumentCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                return;
            }
            if (isClear)
            {
                documentPage = 1;
                DocumentCollection.Clear();
                LoadingRing.IsActive = true;
            }
            else
            {
                LoadingBar.Visibility = Visibility.Visible;
                documentPage += 1;
            } 
            var type = tab.type;
            var param = new Dictionary<string, string>();
            param.Add("category_id", documentRegion);
            var data = await _client.SearchTypeItems<List<SearchDocument>>(_keyword, type, documentSort.Replace("-",""), documentPage, param);
            if (data != null && data.Count > 0)
            {
                data.ForEach(p => DocumentCollection.Add(p));
            }
            DocumentHolderBlock.Visibility = DocumentCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }

        private async void TabListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SearchTab;
            if (_currentTab != item.name)
            {
                VideoContainer.Visibility = Visibility.Collapsed;
                AnimeContainer.Visibility = Visibility.Collapsed;
                UserContainer.Visibility = Visibility.Collapsed;
                MovieContainer.Visibility = Visibility.Collapsed;
                DocumentContainer.Visibility = Visibility.Collapsed;
                _currentTab = item.name;
                if (_currentTab == "综合")
                {
                    VideoContainer.Visibility = Visibility.Visible;
                }
                else if (_currentTab == "番剧")
                {
                    AnimeContainer.Visibility = Visibility.Visible;
                    if (AnimeCollection.Count == 0 && item.total > 0)
                    {
                        await RefreshAnime();
                    }
                    else if (AnimeCollection.Count == 0)
                        AnimeHolderBlock.Visibility = Visibility.Visible;
                }
                else if (_currentTab == "用户")
                {
                    UserContainer.Visibility = Visibility.Visible;
                    if (UserCollection.Count == 0 && item.total > 0)
                    {
                        await RefreshUser();
                    }
                    else if (UserCollection.Count == 0)
                        UserHolderBlock.Visibility = Visibility.Visible;
                }
                else if (_currentTab == "影视")
                {
                    MovieContainer.Visibility = Visibility.Visible;
                    if (MovieCollection.Count == 0 && item.total > 0)
                    {
                        await RefreshMovie();
                    }
                    else if (MovieCollection.Count == 0)
                        MovieHolderBlock.Visibility = Visibility.Visible;
                }
                else if (_currentTab == "专栏")
                {
                    DocumentContainer.Visibility = Visibility.Visible;
                    if (DocumentCollection.Count == 0 && item.total > 0)
                    {
                        await RefreshDocument();
                    }
                    else if(DocumentCollection.Count==0)
                        DocumentHolderBlock.Visibility = Visibility.Visible;
                }
            }
        }

        private void VideoListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as SearchVideo;
            App.AppViewModel.PlayVideo(Convert.ToInt32(video.param));
        }

        private void UserListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var user = e.ClickedItem as SearchUser;
            App.AppViewModel.NavigateToSubPage(typeof(Account.DetailPage), user.mid);
        }

        private void DocumentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SearchDocument;
            App.AppViewModel.ShowDoucmentPopup(item.title, item.id);
        }

        private async void VideoSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            await RefreshVideo();
        }
        private async void UserSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            await RefreshUser();
        }
        private async void DocumentSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            await RefreshDocument();
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                if (_currentTab == "综合")
                {
                    if (!_isVideoRequesting)
                    {
                        _isVideoRequesting = true;
                        await RefreshVideo(false);
                        _isVideoRequesting = false;
                    }
                }
                else if (_currentTab == "番剧")
                {
                    if (!_isAnimeRequesting)
                    {
                        _isAnimeRequesting = true;
                        await RefreshAnime(false);
                        _isAnimeRequesting = false;
                    }
                }
                else if (_currentTab == "用户")
                {
                    if (!_isUserRequesting)
                    {
                        _isUserRequesting = true;
                        await RefreshUser(false);
                        _isUserRequesting = false;
                    }
                }
                else if (_currentTab == "影视")
                {
                    if (!_isMovieRequesting)
                    {
                        _isMovieRequesting = true;
                        await RefreshMovie(false);
                        _isMovieRequesting = false;
                    }
                }
                else if (_currentTab == "专栏")
                {
                    if (!_isDocumentRequesting)
                    {
                        _isDocumentRequesting = true;
                        await RefreshDocument(false);
                        _isDocumentRequesting = false;
                    }
                }
            }
        }
    }
}
