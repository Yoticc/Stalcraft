using ScreenCapture;

delegate void HackTurnedDelegate(Hack hack);
static class HackManager
{
    public static readonly List<Hack> Hacks = new List<Hack>()
    {
        new AimbotHack(),
        new AntiRecoilHack(),
        new AutoXHack(),
        new SmartStealHack()
    }.OrderBy(l => -l.Name.Length).ToList();

    static HackManager()
    {
        var hacks = Hacks;
        for (var index = 0; index < hacks.Count; index++)
        {
            var hack = hacks[index];

            hack.Dispatcher.InvokeOnInit();
            hack.HackTurned += () => HackTurned?.Invoke(hack);
            hack.SetInitIndex(index);
        }

        Interception.OnKeyUp += OnKeyUp;
    }

    public static HackTurnedDelegate? HackTurned;

    static bool OnKeyUp(Keys key)
    {
        if (!StalcraftWindow.IsActive)
            return false;

        foreach (var hack in Hacks)
            if (hack.Keybind == key)
                hack.Turn();

        return false;
    }

    public static bool ShouldCaptureFrame()
    {
        foreach (var hack in Hacks)
            if (hack.IsEnabled && hack.ShouldCaptureFrame())
                return true;
        return false;
    }

    public static void PushFrame(Frame frame, FrameState frameState, MemoryBitmap memoryBitmap)
    {
        foreach (var hack in Hacks)
            hack.Dispatcher.InvokeOnCaptureFrame(frame, frameState, memoryBitmap);
    }

    public static void PushUpdate()
    {
        foreach (var hack in Hacks)
            hack.Dispatcher.InvokeOnUpdate();
    }
}