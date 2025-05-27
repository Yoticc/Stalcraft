using ScreenCapture;
    
delegate void HackTurnedDelegate(Hack hack);
static unsafe class HackManager
{
    public static readonly AntiRecoilHack AntiRecoilHack = new AntiRecoilHack();
    public static readonly SmartStealHack SmartStealHack = new SmartStealHack();
    public static readonly AimbotHack AimbotHack = new AimbotHack();
    public static readonly AutoXHack AutoXHack = new AutoXHack();

    public static readonly List<Hack> Hacks = 
    [
        AntiRecoilHack,
        SmartStealHack,
        AimbotHack,
        AutoXHack,
    ];

    static HackManager()
    {
        var hacks = Hacks;
        for (var index = 0; index < hacks.Count; index++)
        {
            var hack = hacks[index];

            hack.Dispatcher.InvokeOnInit();
            hack.HackTurned += () => HackTurned?.Invoke(hack);
        }

        Interception.OnKeyUp += OnKeyUp;
        Interception.OnKeyDown += OnKeyDown;
    }

    public static HackTurnedDelegate? HackTurned;

    static bool OnKeyDown(Keys key, bool repeated)
    {
        if (!StalcraftWindow.IsActive)
            return false;

        if (repeated)
            return false;

        foreach (var hack in Hacks)
            hack.Dispatcher.InvokeOnKeyDown(key);

        return false;
    }

    static bool OnKeyUp(Keys key)
    {
        if (!StalcraftWindow.IsActive)
            return false;

        foreach (var hack in Hacks)
        {
            if (hack.Keybind == key)
                hack.Turn();
            hack.Dispatcher.InvokeOnKeyUp(key);
        }

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
            if (hack.IsEnabled && hack.ShouldCaptureFrame())
                hack.Dispatcher.InvokeOnCaptureFrame(frame, frameState, memoryBitmap);
    }
}