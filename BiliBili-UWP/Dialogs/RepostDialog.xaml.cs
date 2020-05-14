using BiliBili_Lib.Models.BiliBili;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Dialogs
{
    public sealed partial class RepostDialog : ContentDialog
    {
        private ObservableCollection<Owner> AtUserCollection = new ObservableCollection<Owner>();
        private ObservableCollection<Owner> SearchCollection = new ObservableCollection<Owner>();
        public RepostDialog()
        {
            this.InitializeComponent();
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
                            .Select((temp) => {
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

        public RepostDialog(string origin):this()
        {
            OriginBlock.Text = origin;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            var context = (sender as FrameworkElement).DataContext as Owner;
            AtUserCollection.Remove(context);
        }

        private void UserListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as Owner;
            if (!AtUserCollection.Contains(data))
            {
                AtUserCollection.Add(data);
            }
            SearchUserFlyout.Hide();
            UserSearchBox.Text = string.Empty;
            SearchCollection.Clear();
        }
    }
}
