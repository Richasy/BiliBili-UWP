using BiliBili_UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
            gestureRecognizer.GestureSettings = GestureSettings.HoldWithMouse | GestureSettings.Tap | GestureSettings.Hold;
            IsEnabledChanged += OnIsEnabledChanged;
        }

     

        private bool _isAnimateBegin = false;
        GestureRecognizer gestureRecognizer = new GestureRecognizer();
        private bool _isPointerCaptured;
        public event EventHandler Click;
        public event EventHandler<bool> Hold;

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
            if (v)
                VisualStateManager.GoToState(instance, "Checked", true);
            else
                VisualStateManager.GoToState(instance, "Default", true);
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

        
        public bool CanHolding
        {
            get { return (bool)GetValue(CanHoldingProperty); }
            set { SetValue(CanHoldingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanHolding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanHoldingProperty =
            DependencyProperty.Register("CanHolding", typeof(bool), typeof(CheckButton), new PropertyMetadata(false));


        private void PressStoryBoard_Completed(object sender, object e)
        {
            Hold?.Invoke(this, true);
            ShowBubble();
            PressProgressBar.Visibility = Visibility.Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            gestureRecognizer.Holding -= gestureRecognizer_Holding;
            gestureRecognizer.Tapped -= gestureRecognizer_Tapped;
            gestureRecognizer.Holding += gestureRecognizer_Holding;
            gestureRecognizer.Tapped += gestureRecognizer_Tapped;
        }

        private void gestureRecognizer_Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        void gestureRecognizer_Holding(GestureRecognizer sender, HoldingEventArgs args)
        {
            if (Hold != null && CanHolding)
            {
                if (args.HoldingState == HoldingState.Started)
                {
                    PressProgressBar.Visibility = Visibility.Visible;
                    if (!_isAnimateBegin)
                    {
                        _isAnimateBegin = true;
                        PressStoryBoard.Begin();
                    }
                }
                else
                {
                    _isAnimateBegin = false;
                    PressStoryBoard.Stop();
                    PressProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Handled)
                return;

            if (IsEnabled == false)
                return;

            e.Handled = true;
            _isPointerCaptured = IconContainer.CapturePointer(e.Pointer);
            if (_isPointerCaptured == false)
                return;

            Focus(FocusState.Pointer);
            var ps = e.GetIntermediatePoints(null);
            if (ps != null && ps.Count > 0)
            {
                gestureRecognizer.ProcessDownEvent(ps[0]);
                e.Handled = true;
            }
        }

        private void Grid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            gestureRecognizer.ProcessMoveEvents(e.GetIntermediatePoints(null));
            e.Handled = true;
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.Handled)
                return;

            if (IsEnabled == false)
                return;

            ReleasePointerCapture(e.Pointer);
            _isPointerCaptured = false;

            var ps = e.GetIntermediatePoints(null);
            if (ps != null && ps.Count > 0)
            {
                gestureRecognizer.ProcessUpEvent(ps[0]);
                e.Handled = true;
                gestureRecognizer.CompleteGesture();
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                _isPointerCaptured = false;
            }
        }


        public void ShowBubble()
        {
            BubbleView.IsBubbing = true;
        }

       
    }
}
