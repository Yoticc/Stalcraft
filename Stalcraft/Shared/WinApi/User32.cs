using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

static unsafe class User32
{
    const string user = "user32";

    [DllImport(user)] public static extern bool SetWindowPos(nint hwnd, nint type, int x, int y, int width, int height, SetWindowPosFlags flags);
    [DllImport(user)] public static extern nint CallWindowProc(nint wndproc, nint hwnd, int msg, nint wParam, nint lParam);
    [DllImport(user)] public static extern bool SetLayeredWindowAttributes(nint hwnd, uint color, byte alpha, uint flags);
    [DllImport(user)] public static extern int SetWindowLongPtr(nint hwnd, WindowLongIndex index, uint newLong);
    [DllImport(user)] public static extern int MessageBox(nint hWnd, string text, string caption, uint type);
    [DllImport(user)] public static extern nint DefWindowProc(nint hwnd, int msg, nint wParam, nint lParam);
    [DllImport(user)] public static extern nint SendMessage(nint hwnd, int msg, nint wParam, nint lParam);
    [DllImport(user)] public static extern bool PostMessage(nint hwnd, int msg, nint wParam, nint lParam);
    [DllImport(user)] public static extern nint GetWindowLongPtr(nint hWnd, WindowLongIndex index);
    [DllImport(user)] public static extern int SetWindowRgn(nint hwnd, nint rregion, bool redraw);
    [DllImport(user)] public static extern bool ShowScrollBar(nint hwnd, int bar, bool show);
    [DllImport(user)] public static extern bool GetClientRect(nint hwnd, int* rectangle);
    [DllImport(user)] public static extern bool GetWindowRect(nint hwnd, int* rectangle);
    [DllImport(user)] public static extern bool ClientToScreen(nint hwnd, Point* point);
    [DllImport(user)] public static extern int GetSystemMetrics(SystemMetric index);
    [DllImport(user)] public static extern short GetAsyncKeyState(int key);
    [DllImport(user)] public static extern bool GetCursorPos(Point* point);
    [DllImport(user)] public static extern bool SetCursorPos(int x, int y);
    [DllImport(user)] public static extern nint GetForegroundWindow();
    [DllImport(user)] public static extern bool SetProcessDPIAware();
    [DllImport(user)] public static extern nint GetDesktopWindow();
    [DllImport(user)] public static extern bool ReleaseCapture();
    [DllImport(user)] public static extern nint SetFocus(nint hwnd);
    [DllImport(user)] public static extern nint SetCapture(nint hwnd);
    [DllImport(user)] public static extern nint SetActiveWindow(nint hwnd);
    [DllImport(user)] public static extern nint SetForegroundWindow(nint hwnd);
    [DllImport(user)] public static extern nint GetDC(nint hwnd);

    const int LWA_COLORKEY = 0x01;
    const int LWA_ALPHA = 0x02;
    public static void SetTranparentColor(nint hwnd, Color color)
    {
        var exStyles = GetWindowExStyles(hwnd);
        exStyles |= WindowExStyles.Layered;
        SetWindowExStyles(hwnd, exStyles);

        var value = (uint)color.R | (uint)(color.G << 8) | (uint)(color.B << 16);
        SetLayeredWindowAttributes(hwnd, value, 0, LWA_COLORKEY);
    }

    public static void SetWindowRegion(nint hwnd, GdiRegion regionHandle, bool redraw) => SetWindowRgn(hwnd, *(nint*)&regionHandle, redraw);

    public static bool IsKeyPressed(int key) => ((1 << 15) & GetAsyncKeyState(key)) != 0;

    public static void MessageBox(string caption, string message) => MessageBox(0, message, caption, 0);
    public static void MessageBox(string caption, object message) => MessageBox(caption, message.ToString() ?? "{null}");

    public static void SetWindowLocation(nint hwnd, int x, int y) => SetWindowPos(hwnd, hwnd, x, y, 0, 0, SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoSize);
    public static void SetWindowSize(nint hwnd, int width, int height) => SetWindowPos(hwnd, hwnd, 0, 0, width, height, SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoMove);
    public static void SetWindowRectangle(nint hwnd, int x, int y, int width, int height) => SetWindowPos(hwnd, hwnd, x, y, width, height, SetWindowPosFlags.NoZOrder);
    public static void SetWindowTypeAndFlags(nint hwnd, nint type, SetWindowPosFlags flags) => SetWindowPos(hwnd, type, 0, 0, 0, 0, flags | SetWindowPosFlags.NoSize | SetWindowPosFlags.NoMove);
    public static void SetWindowType(nint hwnd, nint type) => SetWindowTypeAndFlags(hwnd, type, 0);

    public static WindowStyles GetWindowStyles(nint hwnd) => (WindowStyles)GetWindowLongPtr(hwnd, WindowLongIndex.Style);
    public static void SetWindowStyles(nint hwnd, WindowStyles style) => SetWindowLongPtr(hwnd, WindowLongIndex.Style, (uint)style);

    public static WindowExStyles GetWindowExStyles(nint hwnd) => (WindowExStyles)GetWindowLongPtr(hwnd, WindowLongIndex.ExStyle);
    public static void SetWindowExStyles(nint hwnd, WindowExStyles style) => SetWindowLongPtr(hwnd, WindowLongIndex.ExStyle, (uint)style);

    public static void SetWindowOpacity(nint hwnd, float percent) => SetLayeredWindowAttributes(hwnd, 0, (byte)((float)percent / 100 * byte.MaxValue), 0x02/*ALPHA*/);

    public static Point GetCursorPosition()
    {
        Point point;
        GetCursorPos(&point);
        return point;
    }

    public static void SetCursorPosition(int x, int y) => SetCursorPos(x, y);

    public static Rectangle GetWindowRectangle(nint hwnd)
    {
        int* rectangle = stackalloc int[4];
        GetWindowRect(hwnd, rectangle);
        return Rectangle.FromLTRB(rectangle[0], rectangle[1], rectangle[2], rectangle[3]);
    }

    public static Point GetWindowLocation(nint hwnd)
    {
        var rect = GetWindowRectangle(hwnd);
        return rect.Location;
    }

    public static Rectangle GetWindowClientRectangle(nint hwnd)
    {
        int* rectangle = stackalloc int[4];
        GetClientRect(hwnd, rectangle);
        return Rectangle.FromLTRB(rectangle[0], rectangle[1], rectangle[2], rectangle[3]);
    }

    public static bool ClientToScreen(nint hwnd, Rectangle* rectangle)
    {
        var (clientWidth, clientHeight) = (rectangle->Width, rectangle->Height);
        var result = ClientToScreen(hwnd, (Point*)rectangle);
        var ints = (int*)rectangle;
        (ints[2], ints[3]) = (clientWidth, clientHeight);
        return result;
    }

    public static Rectangle GetWindowScreenRectangle(nint hwnd)
    {
        var client = GetWindowClientRectangle(hwnd);
        ClientToScreen(hwnd, &client);
        return client;
    }

    public static Point GetDiffenceBetweenWindowAndScreen(nint hwnd)
    {
        var window = GetWindowRectangle(hwnd).Location;
        var screen = GetWindowScreenRectangle(hwnd).Location;

        var difference = new Point(screen.X - window.X, screen.Y - window.Y);
        return difference;
    }

    public static (int Width, int Height) GetMonitorResolution()
    {
        var desktopHwnd = GetDesktopWindow();
        int* rect = stackalloc int[4];
        GetWindowRect(desktopHwnd, rect);
        return (rect[2], rect[3]);
    }

    public static int HorizontalScrollbarWidth => GetSystemMetrics(SystemMetric.CXVSCROLL);

    public static int VerticalScrollbarWidth => GetSystemMetrics(SystemMetric.CYHSCROLL);
}