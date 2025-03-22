global using static Interop;
using System.Runtime.InteropServices;

unsafe class Interop
{
    const string kernel = "kernel32";
    const string user = "user32";

    [DllImport(kernel)]
    public static extern
        int GetTickCount();

    [DllImport(user)]
    public static extern
        int SetWindowLong(nint hwnd, WindowLongIndex index, uint newLong);

    [DllImport(user)]
    public static extern
        nint GetForegroundWindow();

    [DllImport(user)]
    public static extern
        bool SetWindowPos(nint hwnd, nint hwndAfter, int x, int y, int width, int height, SetWindowPosFlags flags);

    public static void SetWindowPos(nint handle, int x, int y, int width, int height)
        => SetWindowPos(handle, handle, x, y, width, height, SetWindowPosFlags.NoZOrder);

    public static void SetWindowStyle(nint hwnd, WindowStyles style) => SetWindowLong(hwnd, WindowLongIndex.Style, (uint)style);

    [DllImport(user)]
    public static extern
        nint GetDesktopWindow();

    [DllImport(user)]
    public static extern
        bool GetWindowRect(nint hwnd, int* rect);

    [DllImport(user)]
    public static extern
        short GetAsyncKeyState(int key);

    public static bool IsKeyPressed(int key) => ((1 << 15) & GetAsyncKeyState(key)) != 0;

    [DllImport(kernel)]
    public static extern
        bool AttachConsole();

    [DllImport(kernel)]
    public static extern
        int AllocConsole();

    [DllImport(kernel)]
    public static extern
        uint AttachConsole(uint processId);

    [DllImport(kernel)]
    public static extern
        nint CreateFileW([MarshalAs(UnmanagedType.LPTStr)] string fileName, uint desiredAccess, uint shareMode, nint securityAttributes, uint creationDisposition, uint flags, nint temlate);

    [DllImport(kernel)]
    public static extern
        nint GetStdHandle(int stdHandle);

    [DllImport(kernel)]
    public static extern
        bool GetConsoleMode(nint consoleHandle, out uint mode);

    [DllImport(kernel)]
    public static extern
        bool SetConsoleMode(nint consoleHandle, uint mode);
}