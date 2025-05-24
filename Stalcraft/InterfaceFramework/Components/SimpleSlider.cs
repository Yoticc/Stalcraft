using System.Drawing;

delegate void SimpleSliderEventArgs(SimpleSlider sender, int value);

class SimpleSlider : Control
{
    public SimpleSlider(Point location = default, int numberOfDevisions = 10, int value = 0) : base(location, size: new(numberOfDevisions + 3, 1))
    {
        NumberOfDevisions = numberOfDevisions;
    }

    public readonly int NumberOfDevisions;

    int value;
    public int Value => value;

    public SimpleSliderEventArgs? ValueChange;

    public void SetValue(int value)
    {
        this.value = value;
        Draw();
        ValueChange?.Invoke(this, value);
    }

    private protected override void OnMouseDrag(int x, int y)
    {
        if (x < 1 || x > Width - 2)
            return;
        var value = x - 1;

        SetValue(value);

        base.OnMouseDrag(x, y);
    }

    private protected override void OnDraw()
    {
        var text = $"[{new string('-', value)}o{new string(' ', NumberOfDevisions - value)}]";
        var consoleText = new ConsoleText(text);

        ConsoleApplication.DrawText(this, consoleText, 0, 0);
    }
}