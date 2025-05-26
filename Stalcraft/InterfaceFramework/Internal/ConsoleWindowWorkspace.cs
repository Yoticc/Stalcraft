using System.Drawing;

class ConsoleWindowWorkspace
{
    public static readonly ConsoleWindowWorkspace
        Default = new(),
        NoScrollbars = new(),
        NoHeaderAndScrollbars = new(),
        OneChar = new();

    ConsoleWindowWorkspace() { }

    static ConsoleWindowWorkspace state = Default;
    public static ConsoleWindowWorkspace State 
    {
        get => state;
        set
        {
            state = value;

            if (value == Default)
                SetWindowRegion(GdiRegion.Empty);
            else if (value == NoScrollbars)
                SetWindowRegion(GetNoScrollbarsWindowRectangle());
            else if (value == NoHeaderAndScrollbars)
                SetWindowRegion(GetNoHeaderAndScrollbarsWindowRectangle());
            else if (value == OneChar)
                SetWindowRegion(GetOneCharWindowRectangle());
            else throw new InvalidOperationException();
        }
    }

    public static Point OneCharCoordintions;

    public static void EnsureStateIsApplied() => State = state;

    static Rectangle GetNoScrollbarsWindowRectangle()
    {
        var hwnd = ConsoleWindow.WindowHandle;
        var location = new Point(0, 0);
        var difference = User32.GetDiffenceBetweenWindowAndScreen(hwnd);
        var size = new Size(difference.X + Console.ClientWidth, difference.Y + Console.ClientHeight);
        var rectangle = new Rectangle(location, size);
        return rectangle;
    }

    static Rectangle GetNoHeaderAndScrollbarsWindowRectangle()
    {
        var hwnd = ConsoleWindow.WindowHandle;
        var location = User32.GetDiffenceBetweenWindowAndScreen(hwnd);
        var size = new Size(location.X + Console.ClientWidth, location.Y + Console.ClientHeight);
        var rectangle = new Rectangle(location, size);
        return rectangle;
    }

    static Rectangle GetOneCharWindowRectangle()
    {
        var hwnd = ConsoleWindow.WindowHandle;
        var location = User32.GetDiffenceBetweenWindowAndScreen(hwnd);
        var charCoords = OneCharCoordintions;
        var (relativeX, relativeY) = (Console.CharWidth * charCoords.X, Console.CharHeight * charCoords.Y);
        location = new Point(location.X + relativeX, location.Y + relativeY);
        var size = new Size(Console.CharWidth, Console.CharWidth);
        var rectangle = new Rectangle(location, size);
        return rectangle;
    }

    static void SetWindowRegion(Rectangle rectangle)
    {
        using var region = Gdi32.CreateRegion(rectangle);
        SetWindowRegion(region);
    }

    static void SetWindowRegion(GdiRegion region) => User32.SetWindowRegion(ConsoleWindow.WindowHandle, region, false);
}