using System.Diagnostics.CodeAnalysis;
using System.Drawing;

delegate void LabelEventArgs(Label sender);

class Label : Control
{
    public new LabelEventArgs? MouseEnter;
    public new LabelEventArgs? MouseLeave;
    public new LabelEventArgs? MouseClick;
    public new LabelEventArgs? MouseLeftClick;
    public new LabelEventArgs? MouseRightClick;

    private protected override void OnMouseEnter() => MouseEnter?.Invoke(this);
    private protected override void OnMouseLeave() => MouseLeave?.Invoke(this);
    private protected override void OnMouseClick() => MouseClick?.Invoke(this);
    private protected override void OnMouseLeftClick() => MouseLeftClick?.Invoke(this);
    private protected override void OnMouseRightClick() => MouseRightClick?.Invoke(this);

    public Label(ConsoleText text, Point location = default) : base(location: location, size: new(0, 1)) => SetText(text);

    [AllowNull] public ConsoleText Text { get; private set; }

    private protected override void OnDraw()
    {
        ConsoleApplication.DrawText(this, Text, 0, 0);

        base.OnDraw();
    }

    public void SetText(ConsoleText text)
    {
        var oldText = Text;
        Text = text;

        SetWidth(width: text.Length, silence: true);

        if (oldText.Length == text.Length)
            Draw();
        else Redraw();
    }

    public void SetStyle(ConsoleTextStyles styles)
    {
        Text = Text with { Styles = styles };
        Draw();
    }

    public void AddStyle(ConsoleTextStyles setStyles)
    {
        var text = Text;
        var styles = text.Styles | setStyles;
        Text = text with { Styles = styles };
        Draw();
    }

    public void RemoveStyle(ConsoleTextStyles setStyles)
    {
        var text = Text;
        var styles = text.Styles & ~setStyles;
        Text = text with { Styles = styles };
        Draw();
    }

    public void SetStyle(ConsoleTextStyles setStyles, bool flag)
    {
        if (flag)
            AddStyle(setStyles);
        else RemoveStyle(setStyles);
    }
}