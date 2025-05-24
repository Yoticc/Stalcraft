using System.Diagnostics.SymbolStore;
using System.Drawing;   

#pragma warning disable CS0618 // Type or member is obsolete
unsafe static class ConsoleApplication
{
    static ConsoleApplication()
    {
        AllocateConsole();
        SetupConsole();
        SetupWindow();
        StartHandlersTask();
        StartMouseEvents();
    }

    public static string Title { get => System.Console.Title; private set => System.Console.Title = value; }
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    public static Rectangle Bounds => new Rectangle(new Point(0, 0), new Size(Width, Height));

    static Window? runnedWindow;
    static Window? currentWindow;
    public static nint ActiveWindowHandle;
    static bool isClientWindowActive;

    static void AllocateConsole() => Console.AllocateConsole();

    static void SetupConsole()
    {
        Console.CursorVisible = false;
        Console.QuickEditFeature = false;
        Console.VirtualTerminalProcessingFeature = true;

        Console.SetFont("Consolas", 16);
        System.Console.Title = string.Empty;
    }

    static void SetupWindow() =>
        ConsoleWindow.WindowStyles =
            WindowStyles.Visible |
            WindowStyles.Border |
            WindowStyles.DlgFrame |
            WindowStyles.TabStop;

    public static void ClearAndSetSize(int width, int height)
    {
        VT100Clear();
        SetSize(width, height);
    }

    public static void SetSize(int width, int height)
    {
        (Width, Height) = (width, height);

        Console.SetWindowSize(width, height);
        Console.SetBufferSize(width + 32, height + 32);

        ConsoleWindow.ShowScrollbar = false;
    }

    static void OnMouseMove(int x, int y)
    {
        var rect = windowRectangle;
        if (x < rect.Left || x >= rect.Right || y < rect.Top || y >= rect.Bottom)
            SetMouseEntered(false);
        else
        {
            SetMouseEntered(true);
            var consoleX = (x - rect.Left) / 8;
            var consoleY = (y - rect.Top) / 12;
            OnMouseOnConsoleMove(consoleX, consoleY);
        }
    }

    static int lastMouseOnConsoleX, lastMouseOnConsoleY;
    static void OnMouseOnConsoleMove(int x, int y)
    {
        if (lastMouseOnConsoleX != x || lastMouseOnConsoleY != y)
        {
            (lastMouseOnConsoleX, lastMouseOnConsoleY) = (x, y);
            if (currentWindow is null)
                return;

            currentWindow.Dispatcher.InvokeOnMouseMove(x, y);
            if (Interception.IsLeftMouseDown)
                OnMouseDrag(x, y);
        }
    }

    static void OnMouseLeave()
    {
        mouseIsEntered = false;
        if (currentWindow is null)
            return;

        currentWindow.Dispatcher.InvokeOnMouseLeave();
    }

    static void OnMouseEnter()
    {
        mouseIsEntered = true;
        if (currentWindow is null)
            return;

        currentWindow.Dispatcher.InvokeOnMouseEnter();
    }

    static void SetMouseEntered(bool entered)
    {
        if (entered)
        {
            if (!mouseIsEntered)
                OnMouseEnter();
        }
        else if (mouseIsEntered)
            OnMouseLeave();
    }

    static void StartHandlersTask() => new Thread(HandlersBody).Start();

    static void StartMouseEvents()
    {
        Interception.OnKeyUp += key => 
        {
            OnKeyUp(key);
            return false;
        };

        Interception.OnKeyDown += (key, repeat) =>
        {
            if (repeat)
                return false;

            OnKeyDown(key);
            return false;
        };
    }

    static void OnKeyUp(Keys key)
    {
        if (key == Keys.MouseLeft)
        {
            OnMouseLeftUp();
            OnMouseClick();
            OnMouseLeftClick();
        }
        else if (key == Keys.MouseRight)
        {
            OnMouseClick();
            OnMouseRightClick();
        }
    }

    static void OnKeyDown(Keys key)
    {
        if (key == Keys.MouseLeft)
        {
            OnMouseLeftDown();
            OnMouseDrag(lastMouseOnConsoleX, lastMouseOnConsoleY);
        }
    }

    static void OnMouseClick() => currentWindow?.Dispatcher.InvokeOnMouseClick();

    static void OnMouseRightClick() => currentWindow?.Dispatcher.InvokeOnMouseRightClick();

    static void OnMouseLeftClick() => currentWindow?.Dispatcher.InvokeOnMouseLeftClick();

    static void OnMouseLeftDown() => currentWindow?.Dispatcher.InvokeOnMouseLeftDown();

    static void OnMouseLeftUp() => currentWindow?.Dispatcher.InvokeOnMouseLeftUp();

    static void OnMouseDrag(int x, int y) => currentWindow?.Dispatcher.InvokeOnMouseDrag(x, y);

    static Rectangle windowRectangle;
    static Point cursorPosition;
    static bool mouseIsEntered;
    static void HandlersBody()
    {
        var hwnd = ConsoleWindow.WindowHandle;

        while (true)
        {
            ActiveWindowHandle = User32.GetForegroundWindow();
            isClientWindowActive = ActiveWindowHandle == hwnd;

            if (!isClientWindowActive)
            {
                SetMouseEntered(false);
                Thread.Sleep(75);
                continue;
            }

            windowRectangle = ConsoleWindow.ClientRectangle;
            var newCursorPosition = User32.GetCursorPosition();

            if (cursorPosition != newCursorPosition)
            {
                cursorPosition = newCursorPosition;
                OnMouseMove(newCursorPosition.X, newCursorPosition.Y);
            }

            Thread.Sleep(15);
        }
    }

    public static void Run<T>() where T : Window, new() => Run(new T());

    public static void Run(Window masterWindow)
    {
        runnedWindow = masterWindow;
        SetCurrentWindow(masterWindow);
        masterWindow.Open();
    }

    public static void SetCurrentWindow(Window window) => currentWindow = window;

    public static void VT100Clear() => Console.VT100Clear();

    public static void Clear()
    {
        var line = new string(' ', Width);
        var lineText = new ConsoleText(line);
        var linesText = new ConsoleMultistyleText(lineText);
        for (var y = 0; y < Height; y++)
            linesText.Add(lineText with { Y = y });

        Console.Write(linesText);
    }

    public static void DrawText(Control painter, ConsoleText text, int x, int y)
    {
        var painterLocation = painter.AbsoluteLocation;
        x += painterLocation.X;
        y += painterLocation.Y;

        DrawText(text, x, y);
    }

    public static void DrawText(ConsoleText text, int x, int y) => Console.Write(text, x, y);

    public static void DrawText(Control painter, ConsoleMultistyleText text, int x, int y)
    {
        var painterLocation = painter.AbsoluteLocation;
        x += painterLocation.X;
        y += painterLocation.Y;

        DrawText(text, x, y);
    }

    public static void DrawText(ConsoleMultistyleText text, int x, int y) => Console.Write(text, x, y);

    public static void Fill(Control painter, ConsoleTextStyles styles, char fillChar) => Fill(painter.AbsoluteBounds, styles, fillChar);

    static void Fill(Rectangle bounds, ConsoleTextStyles styles, char fillChar) => Fill(bounds.X, bounds.Y, bounds.Width, bounds.Height, styles, fillChar);

    static void Fill(int x, int y, int width, int height, ConsoleTextStyles styles, char fillChar)
    {
        var text = new ConsoleMultistyleText();
        var line = new string(fillChar, width);
        var part = new ConsoleText(text: line, styles: styles);
        var ey = y + height;
        for (; y < ey; y++)
            text.Add(part, x, y);

        Console.Write(text);
    }

    public static void DrawBorder(Control painter, ConsoleTextStyles borderStyles, PanelBorderStyle panelBorderStyle, ConsoleBackgroundColor backgroundColor)
        => DrawBorder(painter.AbsoluteBounds, borderStyles, panelBorderStyle, backgroundColor);

    public static void DrawBorder(Rectangle bounds, ConsoleTextStyles borderColor, PanelBorderStyle panelBorderStyle, ConsoleBackgroundColor backgroundColor)
        => DrawBorder(bounds.X, bounds.Y, bounds.Width, bounds.Height, borderColor, panelBorderStyle, backgroundColor);

    static void DrawBorder(int x, int y, int width, int height, ConsoleTextStyles borderStyles, PanelBorderStyle panelBorderStyle, ConsoleBackgroundColor backgroundColor)
    {
        if (panelBorderStyle == PanelBorderStyle.Dot)
            DrawBorder(x, y, width, height, borderStyles, '.', '.', '.', backgroundColor);
        else if (panelBorderStyle == PanelBorderStyle.ASCII)
            DrawBorder(x, y, width, height, borderStyles, '-', '|', '+', backgroundColor);
    }

    static void DrawBorder(int x, int y, int width, int height, ConsoleTextStyles borderStyles, char horizontalChar, char verticalChar, char cornerChar, ConsoleBackgroundColor backgroundColor)
    {
        var horizontalLine = cornerChar + new string(horizontalChar, width - 2) + cornerChar;
        var horizontalLinePart = new ConsoleText(text: horizontalLine, styles: borderStyles);
        var sideLine = verticalChar.ToString();
        var sideLinePart = new ConsoleText(text: sideLine, styles: borderStyles);
        var centralLine = new string(' ', width - 2);
        var centralLinePart = new ConsoleText(text: centralLine, styles: backgroundColor);
        var text = new ConsoleMultistyleText();

        text.Add(horizontalLinePart, x, y);

        for (var i = 1; i < height - 1; i++)
        {
            var curLevel = y + i;
            text.Add(sideLinePart, x, curLevel);
            text.Add(centralLinePart, x + 1, curLevel);
            text.Add(sideLinePart, x + width - 1, curLevel);
        }

        text.Add(horizontalLinePart, x, y + height - 1);

        Console.Write(text);
    }
}