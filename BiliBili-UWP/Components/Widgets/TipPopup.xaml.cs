using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI;
using System;
using System.Collections.Generic;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class TipPopup : UserControl
    {
        private string _popupContent;
        private Popup _popup = null;
        public TipPopup()
        {
            this.InitializeComponent();
            _popup = new Popup();
            _popup.Child = this;
            this.Loaded += PopupNoticeLoaded;
        }
        public TipPopup(string popupContentString) : this()
        {
            _popupContent = popupContentString;
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
        }
        public void ShowMessage()
        {
            PopupBackground = UIHelper.GetThemeBrush(ColorType.SecondaryColor);
            _popup.IsOpen = true;
        }
        public void ShowError()
        {
            PopupBackground = UIHelper.GetThemeBrush(ColorType.ErrorColor);
            _popup.IsOpen = true;
        }
        public void ShowPopup(ColorType type)
        {
            PopupBackground = UIHelper.GetThemeBrush(type);
            _popup.IsOpen = true;
        }
        public void PopupNoticeLoaded(object sender, RoutedEventArgs e)
        {
            PopupContent.Text = _popupContent;
            this.PopupIn.Begin();
            this.PopupIn.Completed += PopupInCompleted;
        }
        public async void PopupInCompleted(object sender, object e)
        {
            await Task.Delay(2000);
            this.PopupOut.Begin();
            this.PopupOut.Completed += PopupOutCompleted;
        }
        public void PopupOutCompleted(object sender, object e)
        {
            _popup.IsOpen = false;
        }
        public Brush PopupBackground
        {
            get { return (Brush)GetValue(PopupBackgroundProperty); }
            set { SetValue(PopupBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PopupBackgroundProperty =
            DependencyProperty.Register("PopupBackground", typeof(Brush), typeof(TipPopup), new PropertyMetadata(UIHelper.GetThemeBrush(ColorType.PrimaryColor)));
    }
}
