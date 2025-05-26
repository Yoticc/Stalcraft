using System.Drawing;

static class ConsoleWindow
{
    static nint windowHandle;
    public static nint WindowHandle => windowHandle;
    public static void SetWindowHandle(nint handle)
    {
        windowHandle = handle;
        EnsureIsWindowMetricsKnown();
    }

    public static bool IsActive => User32.GetForegroundWindow() == windowHandle;

    static bool isTopmost;
    public static bool IsTopmost
    {
        get => isTopmost;
        set
        {
            const nint HWND_TOPMOST = -1;
            const nint HWND_NOTOPMOST = -2;

            User32.SetWindowType(windowHandle, value ? HWND_TOPMOST : HWND_NOTOPMOST);
            isTopmost = value;
        }
    }

    static bool hasHeader = true;
    public static bool HasHeader
    {
        get => hasHeader;
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
            hasHeader = value;
        }
    }

    static bool isHidden;
    public static bool IsHidden
    {
        get => isHidden;
        set
        {
            isHidden = value;
            var styles = WindowStyles;
            if (value)
                ShowOnlyPinButtonInOverlay();
            else RemoveUnusedConsoleSpace();
            WindowStyles = styles;
        }
    }

    public static int Opacity
    {
        set => User32.SetWindowOpacity(WindowHandle, value);
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

    public static Rectangle WindowRectangle { get => User32.GetWindowRectangle(windowHandle); set => User32.SetWindowRectangle(windowHandle, value.X, value.Y, value.Width, value.Height); }

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

    public static void SetWindowRegion(Rectangle rectangle)
    {
        using var region = Gdi32.CreateRegion(rectangle);
        SetWindowRegion(region);
    }

    public static void SetWindowRegion(GdiRegion region) => User32.SetWindowRegion(windowHandle, region, true);

    static bool nowIsCustomRegion;
    public static void RemoveUnusedConsoleSpace()
    {
        var windowRectangle = WindowRectangle;
        var width = windowRectangle.Width - Console.CharWidth;
        var height = windowRectangle.Height - Console.CharHeight;

        if (hasHeader)
            height += windowHeaderHeight;

        var clippedWindowRectangle = new Rectangle(0, 0, width, height);
        SetWindowRegion(clippedWindowRectangle);
        nowIsCustomRegion = true;
    }

    public static void ShowOnlyPinButtonInOverlay()
    {
        var windowRectangle = WindowRectangle;

        var clippedWindowRectangle = new Rectangle((OverlayWindow.Instance.Width - 1) * Console.CharWidth, 0, Console.CharWidth, Console.CharHeight);
        SetWindowRegion(clippedWindowRectangle);
        nowIsCustomRegion = true;
    }

    public static void ResetWindowRegion()
    {
        SetWindowRegion(GdiRegion.Empty);
        nowIsCustomRegion = false;
    }

    public static void ResetCurtomRegion()
    {
        if (nowIsCustomRegion)
            ResetWindowRegion();
    }

    public static void EnsureHideState()
    {
        if (MainWindow.Instance.IsPinned)
        {
            if ((StalcraftWindow.IsActive || IsActive) && isHidden)
                IsHidden = false;
            else if (!(StalcraftWindow.IsActive || IsActive) && !isHidden)
                IsHidden = true;
        }
        else if (isHidden)
            IsHidden = false;
    }
}