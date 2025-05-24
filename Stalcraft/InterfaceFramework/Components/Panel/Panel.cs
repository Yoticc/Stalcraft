using System.Drawing;

class Panel : BasePanel
{
    public Panel(
        Point location = default,
        Size size = default,
        ConsoleBackgroundColor backgroundColor = default,
        IEnumerable<Control>? controls = null
    ) : base(location, size, backgroundColor, controls) { }

    private protected override void OnDraw()
    {
        ConsoleApplication.Fill(this, BackgroundColor, ' ');

        base.OnDraw();
    }
}