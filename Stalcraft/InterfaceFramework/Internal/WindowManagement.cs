using System.Drawing;

static class WindowManagement
{
    static nint windowHandle;
    public static nint WindowHandle => windowHandle;
    public static void SetWindowHandle(nint handle)
    {
        windowHandle = handle;
        EnsureIsWindowMetricsKnown();
    }

    static bool topmost;
    public static bool Topmost 
    {
        get => topmost;
        set
        {
            const nint HWND_TOPMOST = -1;
            const nint HWND_NOTOPMOST = -2;

            User32.SetWindowType(windowHandle, value ? HWND_TOPMOST : HWND_NOTOPMOST);
            topmost = value;
        }
    }

    static bool header = true;
    public static bool Header
    {
        get => header;
        set
        {
            const WindowStyles HeaderStyles = WindowStyles.DlgFrame | WindowStyles.Border;

            if (value)
            {
                AddWindowStyles(HeaderStyles);
                MoveWindow(-windowSidePadding, -windowHeaderHeight);
            }
            else
            {
                RemoveWindowStyles(HeaderStyles);
                MoveWindow(windowSidePadding, windowHeaderHeight);
            }
            header = value;
        }
    }

    public static bool ShowScrollbar
    {
        set => User32.ShowScrollBar(windowHandle, 3, value);
    }

    static int windowHeaderHeight;
    public static int WindowHeaderHeight => windowHeaderHeight;
    static int windowSidePadding;
    public static int WindowSidePadding => windowSidePadding;
    static void EnsureIsWindowMetricsKnown()
    {
        var window = User32.GetWindowRectangle(windowHandle);        
        var screen = User32.GetWindowScreenRectangle(windowHandle);

        windowHeaderHeight = screen.Top - window.Top;
        windowSidePadding = (window.Width - screen.Width - 12) / 2;      
    }

    public static Rectangle WindowRectangle => User32.GetWindowRectangle(windowHandle);

    public static Rectangle ClientRectangle => User32.GetWindowScreenRectangle(windowHandle);

    public static WindowStyles WindowStyles
    {
        get => User32.GetWindowStyles(windowHandle);
        set => User32.SetWindowStyles(windowHandle, value);
    }

    static void AddWindowStyles(WindowStyles styleToAdd) => WindowStyles |= styleToAdd;

    static void RemoveWindowStyles(WindowStyles styleToRemove) => WindowStyles &= ~styleToRemove;

    public static void RedrawWindow() => User32.SetWindowTypeAndFlags(windowHandle, 0, SetWindowPosFlags.DrawFrame);

    public static void SetWindowLocation(int x, int y) => User32.SetWindowLocation(windowHandle, x, y);

    public static void MoveWindow(int x, int y)
    {
        var oldLocation = User32.GetWindowLocation(windowHandle);
        User32.SetWindowLocation(windowHandle, oldLocation.X + x, oldLocation.Y + y);
    }

    public static void SetWindowSize(int width, int height) => User32.SetWindowSize(windowHandle, width, height);
}