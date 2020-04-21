using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using Newtonsoft.Json;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class AppSearchBox : UserControl
    {
        private ObservableCollection<HotSearch> HotSearchCollection = new ObservableCollection<HotSearch>();
        private ObservableCollection<SearchSuggestion> SuggestionCollection = new ObservableCollection<SearchSuggestion>();
        public AppSearchBox()
        {
            this.InitializeComponent();
            var changed = Observable.FromEventPattern<TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>, AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>(
                          handler => BiliSearchBox.TextChanged += handler,
                          handler => BiliSearchBox.TextChanged -= handler);
            var input = changed
                        .DistinctUntilChanged(temp => temp.Sender.Text)
                        .Throttle(TimeSpan.FromSeconds(0.4));

            var notUserInput = input
                               .ObserveOnDispatcher()
                               .Where(temp => temp.EventArgs.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                               .Select(temp => Task.FromResult<List<SearchSuggestion>>(null));

            var userInput = input
                            .ObserveOnDispatcher()
                            .Where(temp => temp.EventArgs.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                            .Where(temp => !string.IsNullOrEmpty(temp.Sender.Text))
                            .Select((temp) => {
                                LoadingRing.IsActive = true;
                                return App.BiliViewModel._client.GetSearchSuggestionAsync(temp.Sender.Text);
                            });
            var merge = Observable.Merge(notUserInput, userInput).Switch();
            merge.ObserveOnDispatcher().Subscribe(suggestions =>
            {
                if (suggestions != null)
                {
                    BiliSearchBox.ItemsSource = suggestions;
                }
                LoadingRing.IsActive = false;
            });
            Init();
        }
        public async void Init()
        {
            HotSearchCollection.Clear();
            SuggestionCollection.Clear();
            var hotSearch = await App.BiliViewModel._client.GetHotSearchAsync();
            if(hotSearch!=null && hotSearch.Count > 0)
            {
                hotSearch.ForEach(p => HotSearchCollection.Add(p));
            }
        }

        private void BiliSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                BiliSearchBox.ItemsSource = null;
                string key = key = args.ChosenSuggestion.ToString();
                App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.SearchPage), key);
            }
        }

        private void BiliSearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key==Windows.System.VirtualKey.Enter && !string.IsNullOrEmpty(BiliSearchBox.Text))
            {
                string key = BiliSearchBox.Text;
                App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.SearchPage), key);
            }
        }

        private void HotSearchButton_Click(object sender, RoutedEventArgs e)
        {
            HotSearchFlyout.ShowAt(BiliSearchBox);
        }

        private void HotSearchListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var hot = e.ClickedItem as HotSearch;
            string key = hot.keyword;
            App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.SearchPage), key);
        }

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            HotSearchButton.Visibility = Visibility.Visible;
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            HotSearchButton.Visibility = Visibility.Collapsed;
        }
    }
}
