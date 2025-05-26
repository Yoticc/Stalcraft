using System.Diagnostics;

static class StalcraftWindow
{
    static StalcraftWindow()
    {
        new Thread(WatchForWindowHandleThreadBody).Start();
    }

    static nint windowHandle;
    public static nint WindowHandle => windowHandle;
    static nint foregroundWindowHandle;
    public static nint ForegroundWindowHandle => foregroundWindowHandle;

    public static bool IsActive => foregroundWindowHandle == windowHandle;

    static int opacity = 100;
    public static int Opacity
    {
        set => User32.SetWindowOpacity(WindowHandle, opacity = value);
    }

    static void WatchForWindowHandleThreadBody()
    {
        const int ProcessCheckDelay = 3000;
        const int WindowCheckDelay = 15;
        const int IteractionCount = ProcessCheckDelay / WindowCheckDelay;

        while (true)
        {
            var processes = Process.GetProcessesByName("stalcraftw");

            if (processes.Length > 0)
            {
                var process = processes[0];
                var newWindowHandle = process.MainWindowHandle;
                if (newWindowHandle != windowHandle)
                {
                    windowHandle = newWindowHandle;
                    FixWindowState(windowHandle);
                }

                for (var i = 0; i < IteractionCount; i++)
                {
                    var newForegoundHandle = User32.GetForegroundWindow();
                    var isNewForegroundWindow = newForegoundHandle != foregroundWindowHandle;
                    foregroundWindowHandle = newForegoundHandle;

                    if (isNewForegroundWindow)
                        ConsoleWindowState.EnsureStateIsApplied();

                    Thread.Sleep(WindowCheckDelay);
                }
            }
            else
            {
                foregroundWindowHandle = int.MaxValue;
                Thread.Sleep(ProcessCheckDelay);
            }
        }
    }

    static void FixWindowState(nint hwnd)
    {
        User32.SetWindowStyles(
            hwnd,
            WindowStyles.Visible |
            WindowStyles.ClipSiblings |
            WindowStyles.ClipChildren |
            WindowStyles.Group
        );

        var resolution = User32.GetMonitorResolution();
        User32.SetWindowRectangle(hwnd, 0, 0, resolution.Width, resolution.Height);

        Opacity = opacity;
    }
}