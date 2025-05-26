using System.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

static unsafe class Console
{
    public const int CharWidth = 8, CharHeight = 12;

    public static int Width { get; private set; } = 100;
    public static int Height { get; private set; } = 30;

    public static int ClientWidth => Width * CharWidth;
    public static int ClientHeight => Height * CharHeight;

    public static nint InputHandle
    {
        get
        {
            const int STD_INPUT_HANDLE = -10;

            var handle = Kernel32.GetStdHandle(STD_INPUT_HANDLE);
            return handle;
        }
    }

    public static nint OutputHandle
    {
        get
        {
            const int STD_OUTPUT_HANDLE = -11;

            var handle = Kernel32.GetStdHandle(STD_OUTPUT_HANDLE);
            return handle;
        }
    }

    public static bool QuickEditFeature
    {
        set
        {
            const uint ENABLE_QUICK_EDIT = 0x0040;

            TurnInputFeature(ENABLE_QUICK_EDIT, value);
        }
    }

    public static bool VirtualTerminalProcessingFeature
    {
        set
        {
            const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

            TurnOutputFeature(ENABLE_VIRTUAL_TERMINAL_PROCESSING, value);
        }
    }

    public static bool CursorVisible
    {
        set => Kernel32.SetConsoleCursorInfo(OutputHandle, value);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int time;
        public int pt_x;
        public int pt_y;
    }

    class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr CreateWindowEx(
            int dwExStyle,
            string lpClassName,
            string lpWindowName,
            int dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr DefWindowProc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr DispatchMessage(ref MSG msg);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        const int WS_OVERLAPPEDWINDOW = 0x00C00000;
        const int WS_VISIBLE = 0x10000000;
        const int WM_DESTROY = 0x0002;
        const int WM_QUIT = 0x0012;
        const int SW_SHOW = 5;

        static IntPtr hWnd;
        static IntPtr hInstance;

        private const int WM_NCHITTEST = 0x0084;
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;

        public static void Main2()
        {
            hInstance = GetModuleHandle(null);

            hWnd = CreateWindowEx(
                0,
                "Static",
                string.Empty,
                WS_VISIBLE,
                100,
                100,
                ClientWidth,
                ClientHeight,
                IntPtr.Zero,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);

            //SetParent(ConsoleWindow.WindowHandle, hWnd);

            User32.SetWindowStyles(hWnd, WindowStyles.Visible | WindowStyles.TabStop | WindowStyles.DlgFrame | WindowStyles.Border | WindowStyles.Maximize | WindowStyles.SystemMenu);

            ShowWindow(hWnd, 1);

            
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    User32.ReleaseCapture();
                    User32.SendMessage(hWnd, 0xA1, 0x02, 0x00);
                }
            }).Start();

            MSG msg;
            while (true)
            {
                if (PeekMessage(out msg, IntPtr.Zero, 0, 0, 1))
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);

                    User32.DefWindowProc(hWnd, msg.message, msg.wParam, msg.lParam);
                }
            }
        }

        [DllImport("user32.dll")]
        static extern bool PeekMessage(out MSG msg, IntPtr hWnd, int wMsgFilterMin, int wMsgFilterMax, int wRemoveMsg);

        [DllImport("user32.dll")]
        static extern bool TranslateMessage(ref MSG msg);
    }

    public static void AllocateConsole()
    {
        User32.SetProcessDPIAware();
        Kernel32.AllocConsole();
        InitializeOutStream();
        InitializeInStream();
        System.Console.Title = string.Empty;
        (System.Console.WindowWidth, System.Console.WindowHeight) = (System.Console.BufferWidth, System.Console.BufferHeight) = (120, 30);

        var windowHandle = Kernel32.GetConsoleWindow();
        if (windowHandle == 0)
            DebugTools.Debug("ConsoleApp: No console window found. Probably because the debugger is running");
        else ConsoleWindow.WindowHandle = windowHandle;

        
        new Thread(() =>
        {
            Program.Main2();
        }).Start();
        
    }

    public static StreamWriter InitializeOutStream()
    {    
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint GENERIC_WRITE = 0x40000000;

        var fileStream = CreateFileStream("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, FileAccess.Write);
        var streamWriter = new StreamWriter(fileStream) { AutoFlush = true };
        System.Console.SetOut(streamWriter);
        System.Console.SetError(streamWriter);

        return streamWriter;
    }

    public static StreamReader InitializeInStream()
    {
        const uint FILE_SHARE_READ = 0x00000001;
        const uint GENERIC_READ = 0x80000000;

        var fileStream = CreateFileStream("CONIN$", GENERIC_READ, FILE_SHARE_READ, FileAccess.Read);
        var streamReader = new StreamReader(fileStream);
        System.Console.SetIn(streamReader);

        return streamReader;
    }

    static FileStream CreateFileStream(string name, uint win32DesiredAccess, uint win32ShareMode, FileAccess dotNetFileAccess)
    {
        const uint OPEN_EXISTING = 0x00000003;
        const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        var fileHandle = Kernel32.CreateFileW(name, win32DesiredAccess, win32ShareMode, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
        var file = new SafeFileHandle(fileHandle, true);
        var fileStream = new FileStream(file, dotNetFileAccess);

        return fileStream;
    }

    public static void EnableInputFeature(uint feature) => EnableFeature(InputHandle, feature);
    public static void EnableOutputFeature(uint feature) => EnableFeature(OutputHandle, feature);
    public static void EnableFeature(nint handle, uint feature)
    {
        uint consoleMode;
        if (!Kernel32.GetConsoleMode(handle, out consoleMode))
            return;

        consoleMode |= feature;
        Kernel32.SetConsoleMode(handle, consoleMode);
    }

    public static void DisableInputFeature(uint feature) => DisableFeature(InputHandle, feature);
    public static void DisableOutputFeature(uint feature) => DisableFeature(OutputHandle, feature);
    public static void DisableFeature(nint handle, uint feature)
    {
        uint consoleMode;
        if (!Kernel32.GetConsoleMode(handle, out consoleMode))
            return;

        consoleMode &= ~feature;
        Kernel32.SetConsoleMode(handle, consoleMode);
    }

    public static void TurnInputFeature(uint feature, bool enable) => TurnFeature(InputHandle, feature, enable);
    public static void TurnOutputFeature(uint feature, bool enable) => TurnFeature(OutputHandle, feature, enable);
    public static void TurnFeature(nint handle, uint feature, bool enable)
    {
        if (enable)
            EnableFeature(handle, feature);
        else DisableFeature(handle, feature);
    }

    public static void SetFont(string fontName, int size) => Kernel32.SetConsoleFont(OutputHandle, fontName, size);

    public static void ApplySize(int width, int height) => (Width, Height) = (width, height);

    public static void Write(char symbol, int x = int.MaxValue, int y = int.MaxValue) => Write(symbol.ToString(), x , y);

    public static void Write(string text, int x = int.MaxValue, int y = int.MaxValue) => Write(new ConsoleText(text, x, y));

    public static void Write(ConsoleMultistyleText text) => Write(text.Parts);

    public static void Write(ConsoleMultistyleText text, int x, int y)
    {
        var parts = text.Parts;
        if (parts.Count != 0)
            parts[0] = parts[0] with { X = x, Y =  y };

        Write(text.Parts);
    }

    public static void Write(ConsoleText text, int x, int y)
    {
        (text.X, text.Y) = (x, y);
        Write(text);
    }

    [SkipLocalsInit]
    public static void Write(params List<ConsoleText> datas)
    {
        const int StackBufferSize = 0x4000;

        const byte
            EscapeChar = (byte)'\e',
            OpenBracketChar = (byte)'[',
            ZeroChar = (byte)'0',
            MChar = (byte)'m',
            SemicolonChar = (byte)';',
            HChar = (byte)'H';

        const int EscapeStartChars = EscapeChar | OpenBracketChar << 0x08;
        const int ResetTagChars = EscapeStartChars | ZeroChar << 0x10 | MChar << 0x18;
        byte* allocatedStack = stackalloc byte[StackBufferSize];
        var buffer = allocatedStack;

        foreach (var data in datas)
        {
            var x = data.X;
            var y = data.Y;

            if (!ValidateCoordinations(ref x, ref y, out var hasCoordinations))
                continue;

            if (hasCoordinations)
                buffer += WriteSetCursorTag(buffer, x, y);

            var text = data.Text;
            var textLength = text.Length;
            if (textLength != 0)
            {
                var styles = data.Styles;
                if (styles != default)
                    buffer += WriteStyles(buffer, styles);
                else buffer += WriteResetTag(buffer);

                fixed (char* textChars = text)
                    Encoding.UTF8.GetBytes(textChars, textLength, buffer, StackBufferSize);
            }

            buffer += textLength;
        }
        
        Kernel32.WriteConsole(OutputHandle, allocatedStack, (int)(buffer - allocatedStack), null, null);

        static int WriteStyles(byte* input, ConsoleTextStyles styles)
        {
            var pointer = input;

            *(long*)pointer = EscapeStartChars;
            pointer += 2;
            *pointer++ = ZeroChar;

            var stylesValue = styles.Value;
            long value;

            for (var shift = 0; shift < 64; shift += 8)
                if ((value = stylesValue >> shift & 0xFF) != 0)
                {
                    *pointer++ = SemicolonChar;
                    pointer += WriteInteger(pointer, value);
                }

            *pointer++ = MChar;

            return (int)(pointer - input);
        }

        static int WriteResetTag(byte* input)
        {
            *(int*)input = ResetTagChars;
            return 4;
        }

        static int WriteSetCursorTag(byte* input, int x, int y)
        {
            var pointer = input;

            *(long*)pointer = EscapeStartChars;
            pointer += 2;

            pointer += WriteInteger(pointer, y);
            *pointer++ = SemicolonChar;
            pointer += WriteInteger(pointer, x);
            *pointer++ = HChar;

            return (int)(pointer - input);
        }

        static int WriteInteger(byte* input, long value)
        {
            const int MaxLength = 4;

            var pointer = input + MaxLength;
            do
            {
                *--pointer = (byte)(value % 10 + ZeroChar);
                value /= 10;
            } while (value != 0);

            var left = (int)(pointer - input);
            *(long*)input >>= left * 8;
            return MaxLength - left;
        }

        static bool ValidateCoordinations(ref int x, ref int y, out bool hasCoordinations)
        {            
            if (x == int.MaxValue || y == int.MaxValue)
            {
                hasCoordinations = false;
                return true;
            }
            hasCoordinations = true;

            if (x < 0 || y < 0)
            {
                DebugTools.Debug($"ConsoleManagement->Write: Bad cursor location. X: {x}, Y: {y}");
                return false;
            }

            if (x >= Width || y >= Height)
                return false;

            x++; y++;
            return true;
        }
    }

    public static void VT100Clear()
    {
        const int ClearBufferTV100Command = '\e' | '[' << 8 | '2' << 16 | 'J' << 24;

        var command = ClearBufferTV100Command;
        Kernel32.WriteConsole(OutputHandle, (byte*)&command, sizeof(int), null, null);
    }
}