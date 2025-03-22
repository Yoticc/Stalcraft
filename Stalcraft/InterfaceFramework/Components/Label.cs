class Label : Control
{
    string? text;
    public string? Text { get => text; set => SetText(text); }

    private protected override void OnDraw()
    {
        if (Text is null)
            return;

        Owner.Application.DrawText(Text, X, Y);

        base.OnDraw();
    }

    private protected virtual OnTextChanged()
    {

    }

    public void SetText(string text)
    {

    }
}