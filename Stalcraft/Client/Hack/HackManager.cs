using ScreenCapture;
using System.Diagnostics.CodeAnalysis;

static class HackManager
{
    public delegate void HackTurnedDelegate(Hack hack);
    public static HackTurnedDelegate? HackTurned;

    [AllowNull] public static InterceptionManager Macro { get; private set; }
    public static void SetMacro(InterceptionManager macro) => Macro = macro;

    public static readonly List<Hack> Hacks = new List<Hack>()
    {
        new AimbotHack(),
        new AntiRecoilHack(),
        new AutoXHack(),
        new SmartStealHack()
    }.OrderBy(l => -l.Name.Length).ToList();

    public static void InitHacks()
    {
        var hacks = Hacks;
        for (var index = 0; index < hacks.Count; index++)
        {
            var hack = hacks[index];

            hack.Dispatcher.InvokeOnInit();
            hack.HackTurned += () => HackTurned?.Invoke(hack);
            hack.SetInitIndex(index);
        }
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