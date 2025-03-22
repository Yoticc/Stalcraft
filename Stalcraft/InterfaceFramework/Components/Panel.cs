using System.Drawing;

class BasePanel : Control
{
    public BasePanel(
        Point? location = null,
        Size? size = null,
        bool fillBackground = false,
        ConsoleColor? backgroundColor = null,
        char? backgroundChar = null,
        IEnumerable<Control>? controls = null
    ) : base(location, size, controls)
    {
        FillBackground = fillBackground;

        BackgroundColor = backgroundColor ?? ConsoleColor.White;
        BackgroundChar = backgroundChar ?? ' ';
    }

    public bool FillBackground { get; private set; }
    public ConsoleColor BackgroundColor { get; private set; }
    public char BackgroundChar { get; private set; }

    public void SetFillBackground(bool fill)
    {
        var oldValue = FillBackground;
        if (oldValue == fill)
            return;

        FillBackground = fill;
        Draw();
    }

    public void SetBackgroundColor(ConsoleColor color)
    {
        var oldColor = BackgroundColor;
        if (oldColor == color)
            return;

        BackgroundColor = color;
        Draw();
    }

    public void SetBackgroundChar(char backgroundChar)
    {
        var oldChar = backgroundChar;
        if (oldChar == backgroundChar)
            return;

        BackgroundChar = backgroundChar;
        Draw();
    }
}

class Panel : BasePanel
{
    public Panel(
        Point? location = null,
        Size? size = null,
        bool fillBackground = false,
        ConsoleColor? backgroundColor = null,
        char? backgroundChar = null,
        IEnumerable<Control>? controls = null
    ) : base(location, size, fillBackground, backgroundColor, backgroundChar, controls) { }

    private protected override void OnDraw()
    {
        if (FillBackground)
            Application.Fill(this, BackgroundColor, BackgroundChar);

        base.OnDraw();
    }
}

class BorderedPanel : Panel
{
    public BorderedPanel(
        Point? location = null, 
        Size? size = null, 
        bool fillBackground = false,
        ConsoleColor? backgroundColor = null, 
        char? backgroundChar = null,
        ConsoleColor? borderColor = null,
        BorderStyle? borderStyle = null,
        IEnumerable<Control>? controls = null
    ) : base(location, size, fillBackground, backgroundColor, backgroundChar, controls)
    {
        if (borderColor.HasValue)
            BorderColor = borderColor.Value;

        if (borderStyle.HasValue)
            BorderStyle = borderStyle.Value;
    }

    public ConsoleColor BorderColor { get; private set; }
    public BorderStyle BorderStyle { get; private set; }

    public override Rectangle Bounds => new Rectangle(X + 1, Y + 1, Width - 2, Height - 2);

    private protected override void OnDraw()
    {
        Application.DrawBorder(this, BorderColor, BorderStyle);

        if (FillBackground)
            Application.Fill(this, BackgroundColor, BackgroundChar);

        base.OnDraw();
    }
}

public enum BorderStyle
{
    Dot,
    ASCII
}