using System.Drawing;
delegate void MultistyleLabelEventArgs(MultistyleLabel sender);

class MultistyleLabel : Control
{
    public new MultistyleLabelEventArgs? MouseEnter;
    public new MultistyleLabelEventArgs? MouseLeave;
    public new MultistyleLabelEventArgs? MouseClick;
    public new MultistyleLabelEventArgs? MouseLeftClick;
    public new MultistyleLabelEventArgs? MouseRightClick;

    private protected override void OnMouseEnter() => MouseEnter?.Invoke(this);
    private protected override void OnMouseLeave() => MouseLeave?.Invoke(this);
    private protected override void OnMouseClick() => MouseClick?.Invoke(this);
    private protected override void OnMouseLeftClick() => MouseLeftClick?.Invoke(this);
    private protected override void OnMouseRightClick() => MouseRightClick?.Invoke(this);

    public MultistyleLabel(ConsoleMultistyleText text, Point location = default) : base(location: location, size: new(0, 1)) => SetText(text);

    public ConsoleMultistyleText Text { get; private set; }

    private protected override void OnDraw()
    {
        ConsoleApplication.DrawText(this, Text, 0, 0);

        base.OnDraw();
    }

    public void SetText(ConsoleMultistyleText text)
    {
        var oldText = Text;
        Text = text;

        SetWidth(width: text.Length, silence: true);

        if (oldText.Length == text.Length)
            Draw();
        else Redraw();
    }

    public void AddStyle(ConsoleTextStyles setStyles)
    {
        Text.AddStyle(setStyles);
        Draw();
    }

    public void RemoveStyle(ConsoleTextStyles setStyles)
    {
        Text.RemoveStyle(setStyles);
        Draw();
    }

    public void SetStyle(ConsoleTextStyles setStyles, bool flag)
    {
        if (flag)
            AddStyle(setStyles);
        else RemoveStyle(setStyles);
    }
}