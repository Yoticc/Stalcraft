class HackKeybindLabel : MultistyleLabel
{
    public HackKeybindLabel(Hack hack) : base(text: ConsoleMultistyleText.Empty, location: new(hack.Name.Length + 1, 0)) => Hack = hack;

    public readonly Hack Hack;
}