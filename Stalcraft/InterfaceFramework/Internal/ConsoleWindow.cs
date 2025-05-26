using System.Drawing;

static class ConsoleWindow
{
    public static nint WindowHandle;

    public static bool IsActive => User32.GetForegroundWindow() == WindowHandle;

    static bool isTopmost;
    public static bool IsTopmost
    {
        get => isTopmost;
        set
        {
            const nint HWND_TOPMOST = -1;
            const nint HWND_NOTOPMOST = -2;

            User32.SetWindowType(WindowHandle, value ? HWND_TOPMOST : HWND_NOTOPMOST);
            isTopmost = value;
        }
    }

    public static bool IsHidden
    {
        get => ConsoleWindowState.IsHidden;
        set
        {
            if (value)
                ConsoleWindowState.State = ConsoleWindowState.Hidden;
            else ConsoleWindowState.State = ConsoleWindowState.Pinned;
        }
    }

    public static int Opacity
    {
        set => User32.SetWindowOpacity(WindowHandle, value);
    }

    public static Rectangle WindowRectangle { get => User32.GetWindowRectangle(WindowHandle); set => User32.SetWindowRectangle(WindowHandle, value.X, value.Y, value.Width, value.Height); }

    public static Rectangle ClientRectangle => User32.GetWindowScreenRectangle(WindowHandle);

    public static WindowStyles WindowStyles
    {
        get => User32.GetWindowStyles(WindowHandle);
        set => User32.SetWindowStyles(WindowHandle, value);
    }

    public static void SetWindowLocation(int x, int y) => User32.SetWindowLocation(WindowHandle, x, y);

    public static void MoveWindow(int x, int y)
    {
        var oldLocation = User32.GetWindowLocation(WindowHandle);
        User32.SetWindowLocation(WindowHandle, oldLocation.X + x, oldLocation.Y + y);
    }

    public static void SetWindowSize(int width, int height) => User32.SetWindowSize(WindowHandle, width, height);

    public static void SendMessage(int msg, nint wParam, nint lParam) => User32.SendMessage(WindowHandle, msg, wParam, lParam);
}