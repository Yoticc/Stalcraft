using System.Drawing;

class BasePanel : Control
{
    public BasePanel(
        Point location = default,
        Size size = default,
        ConsoleBackgroundColor backgroundColor = default,
        IEnumerable<Control>? controls = null
    ) : base(location, size, controls)
    {
        BackgroundColor = backgroundColor;
    }

    public ConsoleBackgroundColor BackgroundColor { get; private set; }

    public void SetBackgroundColor(ConsoleBackgroundColor color)
    {
        var oldColor = BackgroundColor;
        if (oldColor == color)
            return;

        BackgroundColor = color;
        Draw();
    }
}