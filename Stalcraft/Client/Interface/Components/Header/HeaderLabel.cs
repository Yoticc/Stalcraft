class HeaderLabel : Label
{
    public HeaderLabel(int width) : base(text: new(text: new string('=', width), styles: ConsoleForegroundColor.DarkGray | ConsoleTextStyles.Inverse)) { }
}