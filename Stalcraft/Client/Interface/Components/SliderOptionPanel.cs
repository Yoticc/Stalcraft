using System.Drawing;

class SliderOptionPanel : Panel
{
    public SliderOptionPanel(Panel owner, string optionName, int minValue = 0, int maxValue = 100, int defaultValue = 50, Action<int>? valueChange = null, Point location = default)
        : this(owner.Width - 2, optionName, minValue, maxValue, defaultValue, valueChange, location) { }

    public SliderOptionPanel(int width, string optionName, int minValue = 0, int maxValue = 100, int defaultValue = 50, Action<int>? valueChange = null, Point location = default)
        : base(location, size: new(width, 1))
    {
        ValueChange = valueChange;

        var label = new Label(text: new(text: optionName, styles: ConsoleForegroundColor.Gray));
        slider = new Slider(location: new(optionName.Length + 1, 0), width - optionName.Length - 4, minValue, maxValue, defaultValue, (sender, value) => ValueChange?.Invoke(value));

        AddControls(label, slider);
    }

    Slider slider;

    public Action<int>? ValueChange;

    public void SetValue(int value) => slider.SetValue(value);
}