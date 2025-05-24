using System.Drawing;

delegate void ButtonEventArgs(Button sender);

class Button : Label
{
    public new ButtonEventArgs? MouseEnter;
    public new ButtonEventArgs? MouseLeave;
    public new ButtonEventArgs? MouseClick;
    public new ButtonEventArgs? MouseLeftClick;
    public new ButtonEventArgs? MouseRightClick;

    private protected override void OnMouseEnter() => MouseEnter?.Invoke(this);
    private protected override void OnMouseLeave() => MouseLeave?.Invoke(this);
    private protected override void OnMouseClick() => MouseClick?.Invoke(this);
    private protected override void OnMouseLeftClick()
    {
        MouseLeftClick?.Invoke(this);
        Click?.Invoke(this);
        OnClick();
    }
    private protected override void OnMouseRightClick() => MouseRightClick?.Invoke(this);

    public Button(ConsoleText text, Point location = default) : base(text, location) { }

    public ButtonEventArgs? Click;

    private protected virtual void OnClick() { }
}