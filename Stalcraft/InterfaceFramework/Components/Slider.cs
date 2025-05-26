using System.Drawing;

delegate void SliderEventArgs(Slider sender, int value);

class Slider : Control
{
    public Slider(Point location = default, int numberOfDevisions = 10, int minValue = 0, int maxValue = 100, int defaultValue = 50, SliderEventArgs? valueChange = null)
        : base(location, size: new(numberOfDevisions + 3, 1))
    {
        (NumberOfDevisions, MinValue, MaxValue) = (numberOfDevisions, minValue, maxValue);

        differenceValue = maxValue - minValue;
        singleDevision = (float)differenceValue / numberOfDevisions;
        ValueChange = valueChange;

        SetValue(defaultValue);
    }

    public readonly int NumberOfDevisions;
    public readonly int MinValue;
    public readonly int MaxValue;

    int differenceValue;
    float singleDevision;
    int value;
    int simpleValue;

    public int Value => value;

    public SliderEventArgs? ValueChange;

    public void SetValue(int value)
    {
        var simpleValue = GetNearestSimpleValueTo(value);
        SetValue(value, simpleValue);
    }

    public void SetValue(int value, int simpleValue)
    {
        (this.value, this.simpleValue) = (value, simpleValue);
        ValueChange?.Invoke(this, value);
        Draw();
    }

    private protected override void OnMouseDrag(int x, int y)
    {
        if (x < 1 || x > Width - 2)
            return;

        var simpleValue = x - 1;
        var value = GetNearestValueTo(simpleValue);
        SetValue(value, simpleValue);

        base.OnMouseDrag(x, y);
    }

    private protected override void OnDraw()
    {
        var text = new ConsoleMultistyleText();

        text.AddFlex(new(text: "[", styles: ConsoleForegroundColor.Gray));
        var valueText = new ConsoleText(Value.ToString(), styles: ConsoleForegroundColor.Gray);
        if (simpleValue > (NumberOfDevisions / 2))
        {
            text.AddFlex(valueText);
            text.AddFlex(new(text: new string('-', simpleValue - valueText.Length), styles: ConsoleForegroundColor.Gray));
            text.AddFlex(new(text: "o", styles: ConsoleForegroundColor.Gray));
            text.AddFlex(new(text: new string(' ', NumberOfDevisions - simpleValue)));
        }
        else
        {
            text.AddFlex(new(text: new string('-', simpleValue), styles: ConsoleForegroundColor.Gray));
            text.AddFlex(new(text: "o", styles: ConsoleForegroundColor.Gray));
            text.AddFlex(new(text: new string(' ', NumberOfDevisions - simpleValue - valueText.Length)));
            text.AddFlex(valueText);
        }
        text.AddFlex(new(text: "]", styles: ConsoleForegroundColor.Gray));

        ConsoleApplication.DrawText(this, text);
    }

    int GetNearestSimpleValueTo(int value) => (int)((float)(value - MinValue) / differenceValue * NumberOfDevisions);
    int GetNearestValueTo(int simpleValue) => MinValue + (int)(singleDevision * simpleValue);
}