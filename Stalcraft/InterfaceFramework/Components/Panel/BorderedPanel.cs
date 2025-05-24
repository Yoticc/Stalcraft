using System.Drawing;

class BorderedPanel : Panel
{
    public BorderedPanel(
        Point location = default,
        Size size = default,
        ConsoleBackgroundColor backgroundColor = default,
        ConsoleTextStyles borderStyles = default,
        PanelBorderStyle borderStyle = default,
        IEnumerable<Control>? controls = null
    ) : base(location, size == default ? default : new Size(size.Width + 2, size.Height + 2), backgroundColor, controls)
    {
        BorderStyles = borderStyles;
        PanelBorderStyle = borderStyle;
    }

    public ConsoleTextStyles BorderStyles { get; private set; }
    public PanelBorderStyle PanelBorderStyle { get; private set; }

    public override Rectangle ClientBounds => new Rectangle(X + 1, Y + 1, Width - 2, Height - 2);

    private protected override void OnDraw()
    {
        base.OnDraw();

        ConsoleApplication.DrawBorder(this, BorderStyles, PanelBorderStyle, BackgroundColor);
    }
}