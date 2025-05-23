struct ConsoleMultistyleText
{
    public ConsoleMultistyleText() => Parts = [];
    public ConsoleMultistyleText(params List<ConsoleText> parts) => Parts = parts;

    public List<ConsoleText> Parts;
    public int Length
    {
        get
        {
            var length = 0;
            foreach (var part in Parts)
                length += part.Length;
            return length;
        }
    }

    public void Add(string text, int x = int.MaxValue, int y = int.MaxValue, ConsoleTextStyles styles = default) => Add(new(text, x, y, styles));
    public void Add(ConsoleText part, int x, int y) => Parts.Add(part with { X = x, Y = y });
    public void Add(ConsoleText part) => Parts.Add(part);

    public void AddStyle(ConsoleTextStyles setStyles)
    {
        for (var i = 0; i < Parts.Count; i++)
        {
            var part = Parts[i];
            var styles = part.Styles | setStyles;
            Parts[i] = part with { Styles = styles };
        }
    }

    public void RemoveStyle(ConsoleTextStyles setStyles)
    {
        for (var i = 0; i < Parts.Count; i++)
        {
            var part = Parts[i];
            var styles = part.Styles & ~setStyles;
            Parts[i] = part with { Styles = styles };
        }
    }

    public void SetStyle(ConsoleTextStyles setStyles, bool flag)
    {
        if (flag)
            AddStyle(setStyles);
        else RemoveStyle(setStyles);
    }

    public static readonly ConsoleMultistyleText Empty = new();
}

record struct ConsoleText
{
    public ConsoleText(string text, int x = int.MaxValue, int y = int.MaxValue, ConsoleTextStyles styles = default) => (X, Y, Text, Styles) = (x, y, text, styles);

    public int X, Y;
    public string Text;
    public ConsoleTextStyles Styles;

    public int Length => Text.Length;

    public static readonly ConsoleText Empty = new(string.Empty);
}