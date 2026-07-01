# Real-Time Clock Overlay for Flow Launcher

A Flow Launcher plugin that shows a draggable, always-on-top, live-updating
clock overlaid on your screen.

## What it does

- Type `clock` in Flow Launcher → **Show Clock Overlay** to pop up a
  frameless, semi-transparent clock in the top-right corner of your screen.
- The overlay updates every second via a `DispatcherTimer`.
- **Drag** the overlay anywhere with the left mouse button.
- **Right-click** the overlay to hide it instantly.
- Type `clock` again for options to **Hide**, **toggle 12/24-hour format**,
  or **reset position**.

It reuses Flow Launcher's own WPF `Dispatcher` rather than spinning up a
second UI thread, since Flow Launcher itself is a WPF application.

## Files

```
RealTimeClockOverlay/
├── plugin.json                  # Flow Launcher plugin manifest
├── ClockOverlay.csproj          # .NET project file
├── Main.cs                      # IPlugin entry point (Query/Actions)
├── ClockOverlayWindow.xaml      # Overlay UI markup
├── ClockOverlayWindow.xaml.cs   # Overlay logic (timer, drag, format)
├── icon.png                     # Plugin icon
└── README.md
```

## Requirements

- Windows 10/11
- [Flow Launcher](https://www.flowlauncher.com/) installed
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) (only
  needed to build the plugin; end users just need the .NET 7 desktop
  runtime, which Flow Launcher's C# plugin host already relies on)

## Build
Clone repo:

```
git clone https://github.com/ColoronX/flowlauncher-clock-overlay
```

From the `RealTimeClockOverlay` folder:

```powershell
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained false -o publish
```

This produces `publish/ClockOverlay.dll` alongside `plugin.json` and
`icon.png`.

## Install

1. Close Flow Launcher.
2. Copy the entire contents of the `publish` folder into a new subfolder
   under Flow Launcher's `Plugins` directory, e.g.:

   ```
   %APPDATA%\FlowLauncher\Plugins\RealTimeClockOverlay-1.0.0\
   ```

   Make sure `plugin.json`, `ClockOverlay.dll`, and `icon.png` all end up
   directly inside that folder.
3. Start Flow Launcher again — it will pick up the new plugin
   automatically.
4. Open Flow Launcher and type `clock`.

## Customizing

- **Change the action keyword**: edit `"ActionKeyword"` in `plugin.json`
  (default is `clock`).
- **Change appearance**: edit the colors, corner radius, font size, or
  padding in `ClockOverlayWindow.xaml`.
- **Change default position**: edit `ResetPosition()` in
  `ClockOverlayWindow.xaml.cs` — it currently anchors to the top-right of
  the primary monitor's work area.
- **Make it click-through** (so mouse clicks pass to whatever's behind it):
  this requires setting the `WS_EX_TRANSPARENT` extended window style via
  P/Invoke on the window handle after it's loaded — let me know if you'd
  like that added, since it also means you'd lose the ability to drag or
  right-click-to-hide the overlay directly and would need another way to
  toggle it (e.g. only through Flow Launcher's `clock` command).

## Notes

- Only one overlay instance runs at a time; reopening it while visible just
  re-activates the existing window.
- The timer automatically pauses while the overlay is hidden, so it costs
  nothing when not on screen.
