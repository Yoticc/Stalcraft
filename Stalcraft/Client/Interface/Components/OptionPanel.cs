using System.Drawing;

class OptionPanel : Panel
{
    public OptionPanel(Panel owner, string optionName, Point location = default) : base(location, size: new(owner.Width - 2, 1))
    {
        var label = new Label(text: new(text: optionName, styles: ConsoleForegroundColor.Gray));
        var slider = new Slider(location: new(owner.Width - 15, 0), minValue: 40, maxValue: 100, defaultValue: 50)
        {
            ValueChange = (sender, value) => ValueChange?.Invoke(value)
        };

        AddControls(label, slider);
    }

    public Action<int>? ValueChange;
}