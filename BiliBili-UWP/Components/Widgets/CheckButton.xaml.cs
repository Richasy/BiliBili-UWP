using BiliBili_UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class CheckButton : UserControl
    {
        public CheckButton()
        {
            this.InitializeComponent();
        }

        public event EventHandler Click;

        public bool IsCheck
        {
            get { return (bool)GetValue(IsCheckProperty); }
            set { SetValue(IsCheckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCheck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckProperty =
            DependencyProperty.Register("IsCheck", typeof(bool), typeof(CheckButton), new PropertyMetadata(false, new PropertyChangedCallback(IsCheck_Changed)));

        private static void IsCheck_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool v = (bool)e.NewValue;
            var instance = d as CheckButton;
            instance.HandleButton.Style = v ? UIHelper.GetStyle("PrimaryCircleButtonStyle") : UIHelper.GetStyle("DefaultGhostCircleButtonStyle");
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(CheckButton), new PropertyMetadata(""));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CheckButton), new PropertyMetadata(""));

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        
    }
}
