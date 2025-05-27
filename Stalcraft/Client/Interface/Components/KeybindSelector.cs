using System.Drawing;

class KeybindSelector : MultistyleLabel
{
    public KeybindSelector(Keys key, Action<Keys>? valueChange = null, Point location = default) : base(ConsoleMultistyleText.Empty, location)
    {
        ValueChange = valueChange;
        SetValue(key);
    }

    private protected override void OnMouseLeftClick()
    {
        var text = new ConsoleMultistyleText();
        text.Add(text: "[", styles: ConsoleForegroundColor.DarkGray);
        text.Add(text: "...", styles: ConsoleForegroundColor.Gray | ConsoleTextStyles.Awaiting);
        text.Add(text: "]", styles: ConsoleForegroundColor.DarkGray);
        SetText(text);

        Interception.OnKeyUp += OnKeyUp;
        base.OnMouseLeftClick();

        bool OnKeyUp(Keys key)
        {
            Interception.OnKeyUp -= OnKeyUp;
            if (key == Keys.Esc)
                Update();
            else SetValue(key);

            return false;
        }
    }

    Keys value;
    public Keys Value => value;

    public Action<Keys>? ValueChange;

    public void SetValue(Keys value)
    {
        this.value = value;
        ValueChange?.Invoke(value);
        Update();
    }

    public void Update()
    {
        var text = new ConsoleMultistyleText();
        text.Add(text: "[", styles: ConsoleForegroundColor.DarkGray);
        text.Add(text: KeysFormatter.Formate(value));
        text.Add(text: "]", styles: ConsoleForegroundColor.DarkGray);
        SetText(text);
    }
}