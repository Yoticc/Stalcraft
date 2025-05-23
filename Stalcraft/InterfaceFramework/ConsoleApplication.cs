using System.Drawing;   

#pragma warning disable CS0618 // Type or member is obsolete
unsafe class ConsoleApplication
{
    public ConsoleApplication(InterceptionManager manager)
    {
        this.manager = manager;

        AllocateConsole();
        SetupConsole();
        SetupWindow();
        StartHandlersTask();
        StartMouseEvents();
    }

    public string Title { get => Console.Title; private set => Console.Title = value; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Rectangle Bounds => new Rectangle(new Point(0, 0), new Size(Width, Height));

    InterceptionManager manager;
    Window? runnedWindow;
    public nint ActiveWindowHandle;
    bool isClientWindowActive;

    void AllocateConsole() => ConsoleManagement.AllocateConsole();

    void SetupConsole()
    {
        ConsoleManagement.CursorVisible = false;
        ConsoleManagement.QuickEditFeature = false;
        ConsoleManagement.VirtualTerminalProcessingFeature = true;

        ConsoleManagement.SetFont("Consolas", 16);
    }

    void SetupWindow() =>
        WindowManagement.WindowStyles =
            WindowStyles.Visible |
            WindowStyles.Border |
            WindowStyles.DlgFrame |
            WindowStyles.TabStop;

    public void SetSize(int width, int height)
    {
        (Width, Height) = (width, height);

        ConsoleManagement.SetWindowSize(width, height);
        ConsoleManagement.SetBufferSize(width + 32, height + 32);

        WindowManagement.ShowScrollbar = false;
    }

    void OnMouseMove(int x, int y)
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

    int lastMouseOnConsoleX, lastMouseOnConsoleY;
    void OnMouseOnConsoleMove(int x, int y)
    {
        if (lastMouseOnConsoleX != x || lastMouseOnConsoleY != y)
        {
            (lastMouseOnConsoleX, lastMouseOnConsoleY) = (x, y);
            if (runnedWindow is null)
                return;

            runnedWindow.Dispatcher.InvokeOnMouseMove(x, y);
        }
    }

    void OnMouseLeave()
    {
        mouseIsEntered = false;
        if (runnedWindow is null)
            return;

        runnedWindow.Dispatcher.InvokeOnMouseLeave();
    }

    void OnMouseEnter()
    {
        mouseIsEntered = true;
        if (runnedWindow is null)
            return;

        runnedWindow.Dispatcher.InvokeOnMouseEnter();
    }

    void SetMouseEntered(bool entered)
    {
        if (entered)
        {
            if (!mouseIsEntered)
                OnMouseEnter();
        }
        else if (mouseIsEntered)
            OnMouseLeave();
    }

    void StartHandlersTask() => new Thread(HandlersBody).Start();

    void StartMouseEvents()
    {
        manager.OnKeyUp += key => 
        {
            OnKeyUp(key);
            return false;
        };
    }

    void OnKeyUp(Keys key)
    {
        if (runnedWindow is null)
            return;

        if (key == Keys.MouseLeft)
        {
            OnMouseClick();
            OnMouseLeftClick();
        }
        else if (key == Keys.MouseRight)
        {
            OnMouseClick();
            OnMouseRightClick();
        }
    }

    void OnMouseClick() => runnedWindow!.Dispatcher.InvokeOnMouseClick();

    void OnMouseRightClick() => runnedWindow!.Dispatcher.InvokeOnMouseRightClick();

    void OnMouseLeftClick() => runnedWindow!.Dispatcher.InvokeOnMouseLeftClick();

    Rectangle windowRectangle;
    Point cursorPosition;
    bool mouseIsEntered;
    void HandlersBody()
    {
        var hwnd = WindowManagement.WindowHandle;

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

            windowRectangle = WindowManagement.ClientRectangle;
            var newCursorPosition = User32.GetCursorPosition();

            if (cursorPosition != newCursorPosition)
            {
                cursorPosition = newCursorPosition;
                OnMouseMove(newCursorPosition.X, newCursorPosition.Y);
            }

            Thread.Sleep(15);
        }
    }

    public void Run<T>() where T : Window, new() => Run(new T());

    public void Run(Window masterWindow)
    {
        runnedWindow = masterWindow;
        masterWindow.Application = this;
        SetSize(masterWindow.Width, masterWindow.Height);

        masterWindow.Dispatcher.InvokeOnInit();
        masterWindow.Open();
        masterWindow.Dispatcher.InvokeOnOpen();

        Thread.Sleep(int.MaxValue);
    }

    public void SetTitle(string title) => Title = title;

    public void UpdateTitle()
    {
        SetTitle(string.Empty);
        return;

        // fuck titles, blanks looks awesome
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

    public void ClearBuffer() => ConsoleManagement.Clear();

    public void DrawText(Control painter, ConsoleText text, int x, int y)
    {
        var painterLocation = painter.AbsoluteLocation;
        x += painterLocation.X;
        y += painterLocation.Y;

        DrawText(text, x, y);
    }

    public void DrawText(ConsoleText text, int x, int y) => ConsoleManagement.Write(text, x, y);

    public void DrawText(Control painter, ConsoleMultistyleText text, int x, int y)
    {
        var painterLocation = painter.AbsoluteLocation;
        x += painterLocation.X;
        y += painterLocation.Y;

        DrawText(text, x, y);
    }

    public void DrawText(ConsoleMultistyleText text, int x, int y) => ConsoleManagement.Write(text, x, y);

    public void Fill(Control painter, ConsoleTextStyles styles, char fillChar) => Fill(painter.AbsoluteBounds, styles, fillChar);

    void Fill(Rectangle bounds, ConsoleTextStyles styles, char fillChar) => Fill(bounds.X, bounds.Y, bounds.Width, bounds.Height, styles, fillChar);

    void Fill(int x, int y, int width, int height, ConsoleTextStyles styles, char fillChar)
    {
        var text = new ConsoleMultistyleText();
        var line = new string(fillChar, width);
        var part = new ConsoleText(text: line, styles: styles);
        var ey = y + height;
        for (; y < ey; y++)
            text.Add(part, x, y);

        ConsoleManagement.Write(text);
    }

    public void DrawBorder(Control painter, ConsoleTextStyles borderStyles, PanelBorderStyle panelBorderStyle, ConsoleBackgroundColor backgroundColor)
        => DrawBorder(painter.AbsoluteBounds, borderStyles, panelBorderStyle, backgroundColor);

    public void DrawBorder(Rectangle bounds, ConsoleTextStyles borderColor, PanelBorderStyle panelBorderStyle, ConsoleBackgroundColor backgroundColor)
        => DrawBorder(bounds.X, bounds.Y, bounds.Width, bounds.Height, borderColor, panelBorderStyle, backgroundColor);

    void DrawBorder(int x, int y, int width, int height, ConsoleTextStyles borderStyles, PanelBorderStyle panelBorderStyle, ConsoleBackgroundColor backgroundColor)
    {
        if (panelBorderStyle == PanelBorderStyle.Dot)
            DrawBorder(x, y, width, height, borderStyles, '.', '.', '.', backgroundColor);
        else if (panelBorderStyle == PanelBorderStyle.ASCII)
            DrawBorder(x, y, width, height, borderStyles, '-', '|', '+', backgroundColor);
    }

    void DrawBorder(int x, int y, int width, int height, ConsoleTextStyles borderStyles, char horizontalChar, char verticalChar, char cornerChar, ConsoleBackgroundColor backgroundColor)
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

        ConsoleManagement.Write(text);
    }
}