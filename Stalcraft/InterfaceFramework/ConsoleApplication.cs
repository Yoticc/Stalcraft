using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

unsafe class ConsoleApplication
{
    public ConsoleApplication(int width, int height)
    {
        Width = width; 
        Height = height;

        SetupWindow();
        SetupConsole();
        EnsureSize();
    }

    [AllowNull] public StreamWriter OutStream { get; private set; }
    [AllowNull] public StreamReader InStream { get; private set; }

    public string Title { get => Console.Title; private set => Console.Title = value; }
    public int Width { get; private set; }
    public int Height { get; private set; }    
    public bool CursorVisible { get => Console.CursorVisible; set => Console.CursorVisible = value; }

    Window? runnedWindow;

    void SetupWindow()
    {
        var process = Process.GetCurrentProcess();
        var hwnd = process.MainWindowHandle;

        if (hwnd == 0)
        {
            DebugTools.Debug("ConsoleApp: No console window found. Probably because the debugger is running");
            return;
        }
               
        SetWindowStyle(
            hwnd,
            WindowStyles.Visible |
            WindowStyles.Border |
            WindowStyles.DlgFrame |
            WindowStyles.TabStop
        );
    }

    void SetupConsole()
    {
        ConsoleManagement.DisableConsoleQuickEdit();

        OutStream = ConsoleManagement.InitializeOutStream();
        InStream = ConsoleManagement.InitializeInStream();

        CursorVisible = false;
    }

    public void EnsureSize()
    {
        Console.BufferWidth = Console.WindowWidth = Width;
        Console.BufferHeight = Console.WindowHeight = Height;
    }

    public void Run<T>() where T : Window, new() => Run(new T());

    public void Run(Window masterWindow)
    {
        runnedWindow = masterWindow;
        UpdateTitle();
        masterWindow.Dispatcher.Application = this;
        masterWindow.Open();

        Thread.Sleep(int.MaxValue);
    }

    public void SetTitle(string title) => Title = title;

    public void UpdateTitle()
    {
        var window = runnedWindow;
        if (window is null)
            return;

        var title = window.Title;
        while (true)
        {
            window = window.OpenedWindow;
            if (window is null)
                break;

            title += $" -> {window.Title}";
        }

        SetTitle($"[{title}]");
    }

    public void ClearBuffer() => Console.Clear();

    void PushStates(ConsoleColor? foreground, out PushedStates pushedStates)
    {
        pushedStates = new();

        if (foreground is null)
            return;

        var oldForeground = Console.ForegroundColor;
        if (oldForeground == foreground)
            return;

        pushedStates.HasChanges = true;
        pushedStates.OldForeground = oldForeground;

        Console.ForegroundColor = foreground.Value;
    }

    void PopStates(PushedStates pushedStates)
    {
        if (!pushedStates.HasChanges)
            return;

        Console.ForegroundColor = pushedStates.OldForeground;
    }

    public void DrawText(string text, int x, int y, ConsoleColor? foreground = null)
    {
        PushStates(foreground, out var states);

        var textLength = text.Length;
        var availableLength = Width - x;

        if (textLength > availableLength)
            text = text[..(availableLength - 1)] + '…';

        Console.SetCursorPosition(x, y);
        Console.Write(text);

        PopStates(states);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x02)]
    struct PushedStates
    {
        [FieldOffset(0)]
        public bool HasChanges;
        [FieldOffset(1)]
        byte byteOldForeground;

        public ConsoleColor OldForeground { get => (ConsoleColor)byteOldForeground; set => byteOldForeground = (byte)value; }
    }
}