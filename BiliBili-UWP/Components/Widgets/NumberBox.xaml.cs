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

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class NumberBox : UserControl
    {
        public NumberBox()
        {
            this.InitializeComponent();
        }
        public event EventHandler<double> ValueChanged;

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumberBox), new PropertyMetadata(0.0, new PropertyChangedCallback(Value_Changed)));

        private static void Value_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var instance = d as NumberBox;
                instance.InputBox.Text = e.NewValue.ToString();
                instance.ValueChanged?.Invoke(instance, Convert.ToDouble(e.NewValue));
            }
        }


        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumberBox), new PropertyMetadata(double.MaxValue, new PropertyChangedCallback(Maximum_Changed)));

        private static void Maximum_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var instance = d as NumberBox;
                var max = Convert.ToDouble(e.NewValue);
                if (instance.Value > max)
                    instance.Value = max;
            }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumberBox), new PropertyMetadata(double.MinValue, new PropertyChangedCallback(Minimum_Changed)));

        private static void Minimum_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var instance = d as NumberBox;
                var min = Convert.ToDouble(e.NewValue);
                if (instance.Value < min)
                    instance.Value = min;
            }
        }

        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(double), typeof(NumberBox), new PropertyMetadata(1.0));


        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(NumberBox), new PropertyMetadata(""));




        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PopupUpSpinButton.IsEnabled = true;
            PopupDownSpinButton.IsEnabled = true;
            if (Value >= Maximum)
                PopupUpSpinButton.IsEnabled = false;
            if (Value <= Minimum)
                PopupDownSpinButton.IsEnabled = false;
            if (PopupVisibility == Visibility.Visible)
                UpDownPopup.IsOpen = true;
        }

        private void InputBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpDownPopup.IsOpen = false;
            CheckNumber();
        }

        private void CheckNumber()
        {
            double num = 0;
            try
            {
                num = Convert.ToDouble(InputBox.Text);
            }
            catch (Exception)
            {
                InputBox.Text = Value.ToString();
                return;
            }
            if (num > Maximum || num < Minimum)
            {
                InputBox.Text = Value.ToString();
                return;
            }
            Value = num;
        }

        private void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                CheckNumber();
            }
        }

        private void PopupUpSpinButton_Click(object sender, RoutedEventArgs e)
        {
            double num = Value + Math.Round(Step,1);
            if (num > Minimum)
                PopupDownSpinButton.IsEnabled = true;
            if (num >= Maximum)
            {
                num = Maximum;
                PopupUpSpinButton.IsEnabled = false;
            }
            Value = num;
        }

        private void PopupDownSpinButton_Click(object sender, RoutedEventArgs e)
        {
            double num = Value - Math.Round(Step, 1);
            if (num < Maximum)
                PopupUpSpinButton.IsEnabled = true;
            if (num <= Minimum)
            {
                num = Minimum;
                PopupDownSpinButton.IsEnabled = false;
            }
            Value = num;
        }


        public Visibility PopupVisibility
        {
            get { return (Visibility)GetValue(PopupVisibilityProperty); }
            set { SetValue(PopupVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupVisibilityProperty =
            DependencyProperty.Register("PopupVisibility", typeof(Visibility), typeof(NumberBox), new PropertyMetadata(Visibility.Visible));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(NumberBox), new PropertyMetadata(null));

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(NumberBox), new PropertyMetadata(null));



        public double BoxWidth
        {
            get { return (double)GetValue(BoxWidthProperty); }
            set { SetValue(BoxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoxWidthProperty =
            DependencyProperty.Register("BoxWidth", typeof(double), typeof(NumberBox), new PropertyMetadata(150d));
    }
}
