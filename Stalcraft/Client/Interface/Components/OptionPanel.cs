using System.Drawing;

class OptionPanel : Panel
{
    public OptionPanel(Panel owner, string optionName, int minValue = 0, int maxValue = 100, int defaultValue = 50, Point location = default) : base(location, size: new(owner.Width - 2, 1))
    {
        var label = new Label(text: new(text: optionName, styles: ConsoleForegroundColor.Gray));
        slider = new Slider(location: new(owner.Width - 15, 0), numberOfDevisions: 10, minValue, maxValue, defaultValue)
        {
            ValueChange = (sender, value) => ValueChange?.Invoke(value)
        };

        AddControls(label, slider);
    }

    Slider slider;

    public Action<int>? ValueChange;

    public void SetValue(int value) => slider.SetValue(value);
}