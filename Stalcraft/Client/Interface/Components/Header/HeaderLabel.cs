class HeaderLabel : Label
{
    public HeaderLabel(int width) : base(text: new(text: new string('=', width), styles: ConsoleForegroundColor.DarkGray | ConsoleTextStyles.Inverse)) { }

    private protected override void OnMouseLeftDown()
    {
        User32.ReleaseCapture();
        ConsoleWindow.SendMessage(0xA1, 0x02, 0x00);
    }

    private protected override void OnMouseLeftUp() => ConsoleApplication.UncatchWindow();
}