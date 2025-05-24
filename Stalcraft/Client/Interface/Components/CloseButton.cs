using System.Drawing;

class CloseButton : Button
{
    public CloseButton(Point location = default) : base(new(text: "x", styles: ConsoleForegroundColor.DarkRed), location) { }

    private protected override void OnClick() => Environment.Exit(0);

    private protected override void OnMouseEnter() => UpdateStyles();

    private protected override void OnMouseLeave() => UpdateStyles();

    void UpdateStyles() => SetStyle(IsHoveredByMouse ? ConsoleForegroundColor.Red : ConsoleForegroundColor.DarkRed);
}