using System.Collections.Generic;
using System.Windows;
using Flow.Launcher.Plugin;

namespace FlowLauncher.RealTimeClock
{
    public class Main : IPlugin, IContextMenu
    {
        private PluginInitContext _context;
        private static ClockOverlayWindow _overlay;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var results = new List<Result>();
            bool overlayVisible = _overlay != null && _overlay.IsVisible;

            if (!overlayVisible)
            {
                results.Add(new Result
                {
                    Title = "Show Clock Overlay",
                    SubTitle = "Display a real-time clock on your screen (top-right corner)",
                    IcoPath = "icon.png",
                    Score = 100,
                    Action = _ =>
                    {
                        ShowOverlay();
                        return true;
                    }
                });
            }
            else
            {
                results.Add(new Result
                {
                    Title = "Hide Clock Overlay",
                    SubTitle = "Close the on-screen clock",
                    IcoPath = "icon.png",
                    Score = 100,
                    Action = _ =>
                    {
                        HideOverlay();
                        return true;
                    }
                });

                results.Add(new Result
                {
                    Title = "Toggle 12 / 24 Hour Format",
                    SubTitle = "Switch the overlay's time format",
                    IcoPath = "icon.png",
                    Score = 90,
                    Action = _ =>
                    {
                        _overlay?.ToggleFormat();
                        return false; // keep the Flow Launcher window open
                    }
                });

                results.Add(new Result
                {
                    Title = "Cycle Overlay Size",
                    SubTitle = "Switch between Small, Medium, and Large clock sizes",
                    IcoPath = "icon.png",
                    Score = 85,
                    Action = _ =>
                    {
                        _overlay?.CycleSize();
                        return false;
                    }
                });

                results.Add(new Result
                {
                    Title = "Reset Overlay Position",
                    SubTitle = "Move the clock back to the top-right corner",
                    IcoPath = "icon.png",
                    Score = 80,
                    Action = _ =>
                    {
                        _overlay?.ResetPosition();
                        return false;
                    }
                });
            }

            return results;
        }

        private void ShowOverlay()
        {
            // Flow Launcher itself is a WPF application, so Application.Current
            // already owns an STA thread with a running Dispatcher. We reuse it
            // rather than spinning up a second UI thread.
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_overlay == null)
                {
                    _overlay = new ClockOverlayWindow();
                    _overlay.Closed += (s, e) => _overlay = null;
                }

                _overlay.Show();
                _overlay.Activate();
            });
        }

        private void HideOverlay()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _overlay?.Hide();
            });
        }

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            return new List<Result>();
        }
    }
}
