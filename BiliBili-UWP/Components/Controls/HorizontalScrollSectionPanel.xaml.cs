using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public sealed partial class HorizontalScrollSectionPanel : UserControl
    {
        public HorizontalScrollSectionPanel()
        {
            this.InitializeComponent();
        }
        public event EventHandler<ItemClickEventArgs> ItemClick;
        public event EventHandler RefreshButtonClick;
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(HorizontalScrollSectionPanel), new PropertyMetadata(""));

        public string HoldText
        {
            get { return (string)GetValue(HoldTextProperty); }
            set { SetValue(HoldTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HoldText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoldTextProperty =
            DependencyProperty.Register("HoldText", typeof(string), typeof(HorizontalScrollSectionPanel), new PropertyMetadata(""));



        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(HorizontalScrollSectionPanel), new PropertyMetadata(null));

        public Visibility HolderVisibility
        {
            get { return (Visibility)GetValue(HolderVisibilityProperty); }
            set { SetValue(HolderVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HolderVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HolderVisibilityProperty =
            DependencyProperty.Register("HolderVisibility", typeof(Visibility), typeof(HorizontalScrollSectionPanel), new PropertyMetadata(Visibility.Collapsed));

        public Visibility RefreshButtonVisibility
        {
            get { return (Visibility)GetValue(RefreshButtonVisibilityProperty); }
            set { SetValue(RefreshButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshButtonVisibilityProperty =
            DependencyProperty.Register("RefreshButtonVisibility", typeof(Visibility), typeof(HorizontalScrollSectionPanel), new PropertyMetadata(Visibility.Collapsed));



        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(HorizontalScrollSectionPanel), new PropertyMetadata(null));

        public bool IsItemClickEnabled
        {
            get { return (bool)GetValue(IsItemClickEnabledProperty); }
            set { SetValue(IsItemClickEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsItemClickEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsItemClickEnabledProperty =
            DependencyProperty.Register("IsItemClickEnabled", typeof(bool), typeof(HorizontalScrollSectionPanel), new PropertyMetadata(true));

        public string CustomButtonIcon
        {
            get { return (string)GetValue(CustomButtonIconProperty); }
            set { SetValue(CustomButtonIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomButtonIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomButtonIconProperty =
            DependencyProperty.Register("CustomButtonIcon", typeof(string), typeof(HorizontalScrollSectionPanel), new PropertyMetadata("\ue9c7"));

        public string CustomButtonText
        {
            get { return (string)GetValue(CustomButtonTextProperty); }
            set { SetValue(CustomButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomButtonTextProperty =
            DependencyProperty.Register("CustomButtonText", typeof(string), typeof(HorizontalScrollSectionPanel), new PropertyMetadata("换一换"));


        private void ShowListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClick?.Invoke(sender, e);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButtonClick?.Invoke(sender, EventArgs.Empty);
        }
    }
}
