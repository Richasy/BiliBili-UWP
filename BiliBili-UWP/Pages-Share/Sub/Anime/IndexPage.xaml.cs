using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Service;
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

namespace BiliBili_UWP.Pages_Share.Sub.Anime
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndexPage : Page, IRefreshPage
    {
        private ObservableCollection<ConditionContainer> ConditionCollection = new ObservableCollection<ConditionContainer>();
        private ObservableCollection<AnimeIndexResult> BangumiCollection = new ObservableCollection<AnimeIndexResult>();
        private List<KeyValueModel> SelectConditions = new List<KeyValueModel>();
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private int _type = 0;
        private int _page = 1;
        private bool _isEnd = false;
        private bool _isRequesting = false;
        public IndexPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "索引";
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null)
            {
                if (e.Parameter is int type)
                {
                    if (type != _type)
                    {
                        ConditionCollection.Clear();
                    }
                    _type = type;
                    await Refresh();
                }
                else if (e.Parameter is List<KeyValueModel> data)
                {
                    var tempType = Convert.ToInt32(data.Where(p => p.Key == "season_type").First().Value);
                    if (tempType != _type)
                    {
                        ConditionCollection.Clear();
                    }
                    _type = tempType;
                    LoadingRing.IsActive = true;
                    if (await Reset(data))
                    {
                        await LoadBangumi();
                    }
                    LoadingRing.IsActive = false;
                }
            }
            base.OnNavigatedTo(e);
        }
        private async Task<bool> Reset(List<KeyValueModel> temp = null)
        {
            SelectConditions.Clear();
            BangumiCollection.Clear();
            _page = 1;
            _isEnd = false;
            HolderText.Visibility = Visibility.Visible;
            if (ConditionCollection.Count == 0)
            {
                var conditions = await _animeService.GetBangumiIndexConditionAsync(_type);
                if (conditions != null)
                {
                    if (conditions.order != null)
                    {
                        var sort = new ConditionContainer()
                        {
                            field = "order",
                            name = "排序",
                            values = new List<ConditionItem>()
                        };
                        foreach (var so in conditions.order)
                        {
                            var item = new ConditionItem() { name = so.name, keyword = so.field };
                            sort.values.Add(item);
                        }
                        ConditionCollection.Add(sort);
                    }
                    if (conditions.filter != null)
                    {
                        conditions.filter.ForEach(p => ConditionCollection.Add(p));
                    }
                }
                else
                {
                    new TipPopup("筛选条件加载失败，请刷新").ShowError();
                    return false;
                }
            }
            foreach (var item in ConditionCollection)
            {
                bool isPrepare = false;
                if (temp != null)
                {
                    var source = temp.Where(p => p.Key == item.field).FirstOrDefault();
                    if (source != null)
                    {
                        var target = item.values.Where(p => p.keyword == source.Value).FirstOrDefault();
                        if (target != null)
                        {
                            item.SelectIndex = item.values.IndexOf(target);
                            SelectConditions.Add(source);
                            isPrepare = true;
                        }
                    }
                }
                if (!isPrepare)
                {
                    item.SelectIndex = 0;
                    var first = item.values.First();
                    SelectConditions.Add(new KeyValueModel(item.field, first.keyword));
                }
            }
            return true;
        }
        public async Task Refresh()
        {
            LoadingRing.IsActive = true;
            if (await Reset())
            {
                await LoadBangumi();
            }
            LoadingRing.IsActive = false;
        }
        public async Task LoadBangumi()
        {
            if (_isRequesting)
                return;
            LoadingBar.Visibility = Visibility.Visible;
            if (SelectConditions.Count > 0)
            {
                _isRequesting = true;
                var result = await _animeService.GetBangumiIndexResultsAsync(_page, _type, SelectConditions);
                if (result != null)
                {
                    _isEnd = !result.Item1;
                    result.Item2.ForEach(p => BangumiCollection.Add(p));
                }
                _isRequesting = false;
            }
            HolderText.Visibility = BangumiCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
        }
        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                if (!_isEnd)
                {
                    _page += 1;
                    await LoadBangumi();
                }
            }
        }

        private async void ConditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            var data = combo.DataContext as ConditionContainer;
            var select = combo.SelectedItem as ConditionItem;
            var old = SelectConditions.Where(p => p.Key == data.field).FirstOrDefault();
            if (old == null || old.Value != select.keyword)
            {
                if (old != null)
                    old.Value = select.keyword;
                _page = 1;
                _isEnd = false;
                BangumiCollection.Clear();
                await LoadBangumi();
            }
        }

        private void BangumiGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AnimeIndexResult;
            App.AppViewModel.PlayBangumi(item.season_id);
        }
    }
}
