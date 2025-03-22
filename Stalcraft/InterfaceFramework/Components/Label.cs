using System.Diagnostics.CodeAnalysis;
using System.Drawing;

class Label : Control
{
    public Label(Point? location = null, string? text = null, ConsoleColor? foregroundColor = null) : base(location)
    {
        SetText(text);

        ForegroundColor = foregroundColor??ConsoleColor.White;
    }

    [AllowNull] public string Text { get; private set; }
    public ConsoleColor ForegroundColor { get; private set; }

    public int TextLength => Text is not null ? Text.Length : 0;
    public override Rectangle Bounds => new(X, Y, Text.Length, Text.Length > 0 ? 1 : 0);

    private protected override void OnDraw()
    {
        if (Text is not null)
            Application.DrawText(this, Text, 0, 0, ForegroundColor);

        base.OnDraw();
    }

    public void SetText(string? text)
    {
        var oldText = Text;
        if (oldText == text)
            return;
                
        if (text is null)
            text = string.Empty;

        Text = text;

        var oldLength = TextLength;
        if (oldLength != text.Length)
            SetWidth(text.Length);

        Redraw();
    }
}