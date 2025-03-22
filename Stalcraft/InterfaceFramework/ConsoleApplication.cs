using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

unsafe class ConsoleApplication
{
    public ConsoleApplication(int width, int height)
    {
        Size = new Size(width, height);

        SetupWindow();
        SetupConsole();
        EnsureSize();
    }

    [AllowNull] public StreamWriter OutStream { get; private set; }
    [AllowNull] public StreamReader InStream { get; private set; }

    public string Title { get => Console.Title; private set => Console.Title = value; }
    public Size Size { get; private set; }
    public int Width => Size.Width;
    public int Height => Size.Height;
    public Rectangle Bounds => new Rectangle(new Point(0, 0), Size);
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
        masterWindow.Application = this;
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
            window = window.NextWindow;
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

    // control bounds to console bounds (location(0, 0), size)
    Rectangle GlobalizeBounds(Rectangle bounds)
    {
        var x = Math.Max(0, bounds.X);
        var y = Math.Max(0, bounds.Y);
        var width = Math.Min(Width, bounds.Width);
        var height = Math.Min(Height, bounds.Height);
        var globalizedBounds = new Rectangle(x, y, width, height);
        return globalizedBounds;
    }

    public void DrawText(Control painter, string text, int x, int y, ConsoleColor foreground)
    {
        var bounds = painter.AbsoluteBounds;
        var painterLocation = painter.AbsoluteLocation;
        x += painterLocation.X;
        y += painterLocation.Y;

        DrawText(bounds, text, x, y, foreground);
    }

    public void DrawText(Rectangle bounds, string text, int x, int y, ConsoleColor foreground)
    {
        var lines = text.Split('\n');
        DrawMultilineText(bounds, lines, x, y, foreground);
    }

    public void DrawMultilineText(Rectangle bounds, string[] lines, int x, int y, ConsoleColor foreground)
    {
        PushStates(foreground, out var states);

        foreach (var line in lines)
            DrawText(bounds, line, x, y++);

        PopStates(states);
    }

    void DrawText(Rectangle bounds, string text, int x, int y)
    {
        var textLength = text.Length;
        var availableLength = Width - x;

        if (textLength > availableLength)
            text = text[..(availableLength - 1)] + '…';

        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }

    public void Fill(Control painter, ConsoleColor color, char fillChar)
    {
        var bounds = painter.AbsoluteBounds;
        bounds = GlobalizeBounds(bounds);

        Fill(bounds, color, fillChar);
    }

    void Fill(Rectangle bounds, ConsoleColor color, char fillChar) => Fill(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, fillChar);

    void Fill(int x, int y, int width, int height, ConsoleColor color, char fillChar)
    {
        PushStates(color, out var states);

        var line = new string(fillChar, width);
        var ey = y + height;
        for (; y < ey; y++)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(line);
        }

        PopStates(states);
    }

    public void DrawBorder(Control painter, ConsoleColor borderColor, BorderStyle borderStyle)
    {
        var bounds = painter.AbsoluteBounds;
        bounds = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, bounds.Height + 2);
        bounds = GlobalizeBounds(bounds);

        DrawBorder(bounds, borderColor, borderStyle);
    }

    public void DrawBorder(Rectangle bounds, ConsoleColor borderColor, BorderStyle borderStyle)
        => DrawBorder(bounds.X, bounds.Y, bounds.Width, bounds.Height, borderColor, borderStyle);

    void DrawBorder(int x, int y, int width, int height, ConsoleColor borderColor, BorderStyle borderStyle)
    {
        PushStates(borderColor, out var states);

        if (borderStyle == BorderStyle.Dot)
            DrawBorder(x, y, width, height, '.', '.', '.');
        else if (borderStyle == BorderStyle.ASCII)
            DrawBorder(x, y, width, height, '-', '|', '+');

        PopStates(states);        
    }

    void DrawBorder(int x, int y, int width, int height, char horizontalChar, char verticalChar, char cornerChar)
    {
        var horizontalLine = cornerChar + new string(horizontalChar, width - 2) + cornerChar;
        Console.SetCursorPosition(x, y);
        Console.Write(horizontalLine);

        for (var i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write(verticalChar);

            Console.SetCursorPosition(x + width - 1, y + i);
            Console.Write(verticalChar);
        }

        Console.SetCursorPosition(x, y + height - 1);
        Console.Write(horizontalLine);
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