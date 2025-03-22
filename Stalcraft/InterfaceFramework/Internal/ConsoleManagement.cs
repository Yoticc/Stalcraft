using Microsoft.Win32.SafeHandles;

static class ConsoleManagement
{
    const uint GENERIC_WRITE = 0x40000000;
    const uint GENERIC_READ = 0x80000000;
    const uint FILE_SHARE_READ = 0x00000001;
    const uint FILE_SHARE_WRITE = 0x00000002;
    const uint OPEN_EXISTING = 0x00000003;
    const uint FILE_ATTRIBUTE_NORMAL = 0x80;

    public static StreamWriter InitializeOutStream()
    {
        var fileStream = CreateFileStream("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, FileAccess.Write);

        if (fileStream is null)
            return null!;

        var streamWriter = new StreamWriter(fileStream) { AutoFlush = true };
        Console.SetOut(streamWriter);
        Console.SetError(streamWriter);

        return streamWriter;
    }

    public static StreamReader InitializeInStream()
    {
        var fileStream = CreateFileStream("CONIN$", GENERIC_READ, FILE_SHARE_READ, FileAccess.Read);
        if (fileStream is null)
            return null!;

        var streamReader = new StreamReader(fileStream);
        Console.SetIn(streamReader);

        return streamReader;
    }

    static FileStream CreateFileStream(string name, uint win32DesiredAccess, uint win32ShareMode, FileAccess dotNetFileAccess)
    {
        var file = new SafeFileHandle(CreateFileW(name, win32DesiredAccess, win32ShareMode, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero), true);
        if (file.IsInvalid)
            return null!;

        return new FileStream(file, dotNetFileAccess);
    }

    public static void DisableConsoleQuickEdit()
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;
        const int STD_INPUT_HANDLE = -10;

        nint consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

        uint consoleMode;
        if (!GetConsoleMode(consoleHandle, out consoleMode))
            return;

        consoleMode &= ~ENABLE_QUICK_EDIT;
        SetConsoleMode(consoleHandle, consoleMode);
    }
}