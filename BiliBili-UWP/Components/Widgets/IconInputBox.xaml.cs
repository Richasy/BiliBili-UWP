using BiliBili_UWP.Models.Enums;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class IconInputBox : UserControl
    {
        public IconInputBox()
        {
            this.InitializeComponent();
        }
        public event EventHandler InputBoxLostFocus;

        public string PrefixIcon
        {
            get { return (string)GetValue(PrefixIconProperty); }
            set { SetValue(PrefixIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrefixIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrefixIconProperty =
            DependencyProperty.Register("PrefixIcon", typeof(string), typeof(IconInputBox), new PropertyMetadata("", new PropertyChangedCallback(PrefixIcon_Changed)));

        private static void PrefixIcon_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                var instance = d as IconInputBox;
                string icon = e.NewValue.ToString();
                if (string.IsNullOrEmpty(icon))
                    instance.IconBlock.Visibility = Visibility.Collapsed;
                else
                {
                    instance.IconBlock.Visibility = Visibility.Visible;
                }
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(IconInputBox), new PropertyMetadata(""));

        public InputBoxType InputType
        {
            get { return (InputBoxType)GetValue(InputTypeProperty); }
            set { SetValue(InputTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register("InputType", typeof(InputBoxType), typeof(IconInputBox), new PropertyMetadata(InputBoxType.Text, new PropertyChangedCallback(InputType_Changed)));

        private static void InputType_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is InputBoxType type)
            {
                var instance = d as IconInputBox;
                switch (type)
                {
                    case InputBoxType.Text:
                        instance.InputTextBox.Visibility = Visibility.Visible;
                        instance.InputPasswordBox.Visibility = Visibility.Collapsed;
                        break;
                    case InputBoxType.Password:
                        instance.InputTextBox.Visibility = Visibility.Collapsed;
                        instance.InputPasswordBox.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }



        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(IconInputBox), new PropertyMetadata(""));


        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(IconInputBox), new PropertyMetadata("", new PropertyChangedCallback(HeaderText_Changed)));

        private static void HeaderText_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                var instance = d as IconInputBox;
                string header = e.NewValue.ToString();
                if (string.IsNullOrEmpty(header))
                    instance.HeaderBlock.Visibility = Visibility.Collapsed;
                else
                    instance.HeaderBlock.Visibility = Visibility.Visible;
            }
        }

        public Thickness InputPadding
        {
            get { return (Thickness)GetValue(InputPaddingProperty); }
            set { SetValue(InputPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputPaddingProperty =
            DependencyProperty.Register("InputPadding", typeof(Thickness), typeof(IconInputBox), new PropertyMetadata(new Thickness(18, 10, 18, 10)));



        public double InputHeight
        {
            get { return (double)GetValue(InputHeightProperty); }
            set { SetValue(InputHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputHeightProperty =
            DependencyProperty.Register("InputHeight", typeof(double), typeof(IconInputBox), new PropertyMetadata(0, new PropertyChangedCallback(InputHeight_Changed)));

        private static void InputHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var height = Convert.ToDouble(e.NewValue);
                var instance = d as IconInputBox;
                instance.InputTextBox.Height = height;
                instance.InputTextBox.AcceptsReturn = true;
                instance.InputTextBox.TextWrapping = TextWrapping.Wrap;
            }
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(IconInputBox), new PropertyMetadata(false, new PropertyChangedCallback(IsLoading_Changed)));

        private static void IsLoading_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isLoading)
            {
                var instance = d as IconInputBox;
                if (isLoading)
                {
                    instance.LoadingRing.Visibility = Visibility.Visible;
                    instance.IconBlock.Visibility = Visibility.Collapsed;
                    instance.LoadingRing.IsActive = true;
                }
                else
                {
                    instance.LoadingRing.Visibility = Visibility.Collapsed;
                    instance.IconBlock.Visibility = Visibility.Visible;
                    instance.LoadingRing.IsActive = false;
                }
            }
        }

        public void SetText(string text)
        {
            if (InputType == InputBoxType.Text)
                InputTextBox.Text = text;
            else
                InputPasswordBox.Password = text;
        }

        private void Container_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (InputTextBox.FocusState == FocusState.Unfocused && InputPasswordBox.FocusState == FocusState.Unfocused)
                VisualStateManager.GoToState(this, "PointerOver", false);
        }

        private void Container_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (InputTextBox.FocusState == FocusState.Unfocused && InputPasswordBox.FocusState == FocusState.Unfocused)
                VisualStateManager.GoToState(this, "Normal", false);
        }

        private void InputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Focus", false);
        }

        private void InputPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InputBoxLostFocus?.Invoke(this, EventArgs.Empty);
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Text = InputPasswordBox.Password;
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = InputTextBox.Text;
        }
    }
}
