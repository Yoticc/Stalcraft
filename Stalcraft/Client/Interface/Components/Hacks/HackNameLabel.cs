class HackNameLabel : Label
{
    public HackNameLabel(Hack hack) : base(text: new(hack.Name)) => Hack = hack;

    public readonly Hack Hack;
}