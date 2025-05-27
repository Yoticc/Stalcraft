using System.Drawing;

class KeybindOptionPanel : Panel
{
    public KeybindOptionPanel(string optionName, Keys key, Action<Keys>? valueChange = null, Point location = default)
       : base(location, size: new(optionName.Length + 12, 1))
    {
        ValueChange = valueChange;

        var label = new Label(text: new(text: optionName, styles: ConsoleForegroundColor.Gray));
        selector = new KeybindSelector(key, valueChange: key => ValueChange?.Invoke(key), location: new(optionName.Length + 1, 0));

        AddControls(label, selector);
    }

    KeybindSelector selector;

    public Action<Keys>? ValueChange;

    public void SetValue(Keys value) => selector.SetValue(value);
}