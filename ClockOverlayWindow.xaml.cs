using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace FlowLauncher.RealTimeClock
{
    public partial class ClockOverlayWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private bool _use24HourFormat = true;

        // Size presets: (fontSize, paddingH, paddingV, cornerRadius)
        private static readonly (double Font, double PadH, double PadV, double Corner)[] _sizes =
        {
            (20, 10, 4, 6),   // Small
            (28, 12, 6, 8),   // Medium (default)
            (40, 18, 10, 11)  // Large
        };
        private int _sizeIndex = 1; // start at Medium

        public ClockOverlayWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => ResetPosition();

            // Pause the timer while hidden so it costs nothing when not shown.
            IsVisibleChanged += (s, e) =>
            {
                if (IsVisible)
                {
                    UpdateTime();
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            };

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => UpdateTime();

            UpdateTime();
        }

        private void UpdateTime()
        {
            var now = DateTime.Now;
            TimeText.Text = _use24HourFormat
                ? now.ToString("HH:mm:ss")
                : now.ToString("hh:mm:ss tt");
        }

        public void CycleSize()
        {
            _sizeIndex = (_sizeIndex + 1) % _sizes.Length;
            var (font, padH, padV, corner) = _sizes[_sizeIndex];
            TimeText.FontSize = font;
            RootBorder.Padding = new Thickness(padH, padV, padH, padV);
            RootBorder.CornerRadius = new CornerRadius(corner);
            // Re-snap to corner after size change
            Dispatcher.InvokeAsync(ResetPosition, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void ToggleFormat()
        {
            _use24HourFormat = !_use24HourFormat;
            UpdateTime();
        }

        public void ResetPosition()
        {
            var workArea = SystemParameters.WorkArea;

            // Snap flush into the top-right corner.
            Left = ActualWidth > 0 ? workArea.Right - ActualWidth : workArea.Right - 160;
            Top = workArea.Top;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Right-click hides the overlay; reopen it from Flow Launcher with "clock".
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
        }
    }
}
