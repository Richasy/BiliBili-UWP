using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class DefaultVideoPanel : UserControl
    {
        public DefaultVideoPanel()
        {
            this.InitializeComponent();
        }
        public string VideoId
        {
            get { return (string)GetValue(VideoIdProperty); }
            set { SetValue(VideoIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VideoId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoIdProperty =
            DependencyProperty.Register("VideoId", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata("0"));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public string Cover
        {
            get { return (string)GetValue(CoverProperty); }
            set { SetValue(CoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cover.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoverProperty =
            DependencyProperty.Register("Cover", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public string Duration
        {
            get { return (string)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public string Tip
        {
            get { return (string)GetValue(TipProperty); }
            set { SetValue(TipProperty, value); }
        }
        public static readonly DependencyProperty TipProperty =
            DependencyProperty.Register("Tip", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));
        
        public string RightTopImage
        {
            get { return (string)GetValue(RightTopImageProperty); }
            set { SetValue(RightTopImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightTopImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightTopImageProperty =
            DependencyProperty.Register("RightTopImage", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public string BottomText
        {
            get { return (string)GetValue(BottomTextProperty); }
            set { SetValue(BottomTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTextProperty =
            DependencyProperty.Register("BottomText", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));
        public Visibility SectionVisibility
        {
            get { return (Visibility)GetValue(SectionVisibilityProperty); }
            set { SetValue(SectionVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionVisibilityProperty =
            DependencyProperty.Register("SectionVisibility", typeof(Visibility), typeof(DefaultVideoPanel), new PropertyMetadata(Visibility.Visible));
        public string FirstSectionIcon
        {
            get { return (string)GetValue(FirstSectionIconProperty); }
            set { SetValue(FirstSectionIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstSectionIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstSectionIconProperty =
            DependencyProperty.Register("FirstSectionIcon", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public string FirstSectionContent
        {
            get { return (string)GetValue(FirstSectionContentProperty); }
            set { SetValue(FirstSectionContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstSectionContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstSectionContentProperty =
            DependencyProperty.Register("FirstSectionContent", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public string SecondSectionIcon
        {
            get { return (string)GetValue(SecondSectionIconProperty); }
            set { SetValue(SecondSectionIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondSectionIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondSectionIconProperty =
            DependencyProperty.Register("SecondSectionIcon", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public string SecondSectionContent
        {
            get { return (string)GetValue(SecondSectionContentProperty); }
            set { SetValue(SecondSectionContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondSectionContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondSectionContentProperty =
            DependencyProperty.Register("SecondSectionContent", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));


        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(DefaultVideoPanel), new PropertyMetadata(""));

        public Visibility ExtraButtonVisibility
        {
            get { return (Visibility)GetValue(ExtraButtonVisibilityProperty); }
            set { SetValue(ExtraButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtraButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtraButtonVisibilityProperty =
            DependencyProperty.Register("ExtraButtonVisibility", typeof(Visibility), typeof(DefaultVideoPanel), new PropertyMetadata(Visibility.Collapsed));

        private async void LaterViewButton_Click(object sender, RoutedEventArgs e)
        {
            await App.BiliViewModel.AddViewLater(sender, Convert.ToInt32(VideoId));
        }

        public FlyoutBase ExtraFlyout
        {
            get { return (FlyoutBase)GetValue(ExtraFlyoutProperty); }
            set { SetValue(ExtraFlyoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtraFlyout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtraFlyoutProperty =
            DependencyProperty.Register("ExtraFlyout", typeof(FlyoutBase), typeof(DefaultVideoPanel), new PropertyMetadata(null,new PropertyChangedCallback(ExtraFlyout_Changed)));

        private static void ExtraFlyout_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue !=null && e.NewValue is FlyoutBase flyout)
            {
                var instance = d as DefaultVideoPanel;
                instance.ExtraButton.Flyout = flyout;
            }
        }
        public void RenderContainer(ContainerContentChangingEventArgs args)
        {
            CoverContainer.Opacity = 0;
            InfoContainer.Opacity = 0;

            args.RegisterUpdateCallback(RenderInfo);
        }

        private void RenderInfo(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            InfoContainer.Opacity = 1;
            args.RegisterUpdateCallback(RenderCover);
        }

        private void RenderCover(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            CoverContainer.Opacity = 1;
        }
    }
}
