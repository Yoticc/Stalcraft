using System.Drawing;
using System.Runtime.InteropServices;

static unsafe class Kernel32
{
    const string kernel = "kernel32";

    [DllImport(kernel)]
    public static extern nint CreateFileW(
        [MarshalAs(UnmanagedType.LPTStr)] string filename, 
        uint desiredAccess, 
        uint shareMode,
        nint securityAttributes,
        uint creationDisposition,
        uint flags,
        nint temlate
    );
    [DllImport(kernel)] public static extern nint WriteConsoleW(nint handle, char* buffer, int charsToWrite, int* charsWritten, void* reserved);
    [DllImport(kernel)] public static extern nint WriteConsole(nint handle, byte* buffer, int bytesToWrite, int* bytesWritten, void* reserved);
    [DllImport(kernel)] public static extern bool SetCurrentConsoleFontEx(nint handle, bool maxWindow, ConsoleFontInfoEx* font);
    [DllImport(kernel)] public static extern bool SetConsoleCursorInfo(nint handle, ConsoleCursorInfo* cursor);
    [DllImport(kernel)] public static extern bool GetConsoleMode(nint consoleHandle, out uint mode);
    [DllImport(kernel)] public static extern bool SetConsoleMode(nint consoleHandle, uint mode);
    [DllImport(kernel)] public static extern uint AttachConsole(uint processId);    
    [DllImport(kernel)] public static extern nint GetStdHandle(int stdHandle);
    [DllImport(kernel)] public static extern nint GetConsoleWindow();
    [DllImport(kernel)] public static extern bool AttachConsole();
    [DllImport(kernel)] public static extern int AllocConsole();
    [DllImport(kernel)] public static extern int GetTickCount();

    public static void SetConsoleFont(nint handle, string fontName, int size)
    {
        var font = stackalloc ConsoleFontInfoEx[1];
        font->Size = sizeof(ConsoleFontInfoEx);
        font->FontSize = new Point(0, size);
        font->FontWeight = 400;

        fixed (char* fontNameChars = fontName)
            Buffer.MemoryCopy(fontNameChars, font->FaceName, 64, 64);

        SetCurrentConsoleFontEx(handle, false, font);
    }

    public static void SetConsoleCursorInfo(nint handle, bool visible)
    {
        var cursor = stackalloc ConsoleCursorInfo[1];
        cursor->Size = sizeof(ConsoleCursorInfo);
        cursor->Visible = visible;

        SetConsoleCursorInfo(handle, cursor);
    }
}