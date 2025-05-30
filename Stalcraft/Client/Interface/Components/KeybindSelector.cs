using Interception;
using System.Drawing;

class KeybindSelector : MultistyleLabel
{
    public KeybindSelector(Key key, Action<Key>? valueChange = null, Point location = default) : base(ConsoleMultistyleText.Empty, location)
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

        InterceptionImpl.OnKeyUp += OnKeyUp;
        base.OnMouseLeftClick();

        bool OnKeyUp(Key key)
        {
            InterceptionImpl.OnKeyUp -= OnKeyUp;
            if (key == Key.Esc)
                Update();
            else SetValue(key);

            return false;
        }
    }

    Key value;
    public Key Value => value;

    public Action<Key>? ValueChange;

    public void SetValue(Key value)
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