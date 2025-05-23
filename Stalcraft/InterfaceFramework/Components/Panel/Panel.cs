using System.Drawing;

class Panel : BasePanel
{
    public Panel(
        Point? location = null,
        Size? size = null,
        ConsoleBackgroundColor backgroundColor = default,
        IEnumerable<Control>? controls = null
    ) : base(location, size, backgroundColor, controls) { }

    private protected override void OnDraw()
    {
        if (BackgroundColor != default)
            Application.Fill(this, BackgroundColor, ' ');

        base.OnDraw();
    }
}