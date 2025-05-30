using Interception;
using ScreenCapture;

unsafe class AutoXHack : Hack
{
    static AutoXSettings* settings = Config->Settings->AutoX;
    public AutoXHack() : base("auto x")
    {
        SettingsPanel = new AutoXSettingsPanel();
    }

    Key* xKeybind = settings->Keybind;
    public Key XKeybind { get => *xKeybind; set => *xKeybind = value; }

    public override bool ShouldCaptureFrame() => true;

    AutoXState state = AutoXState.None;
    private protected override void OnCaptureFrame(Frame frame, FrameState frameState, MemoryBitmap bitmap)
    {
        if (frameState != FrameState.InCrateGui)
            return;

        if (state == AutoXState.RequestedPressing)
        {
            state = AutoXState.RestoreKeys;

            var rememberedKeys = RememberMovementKeys();
            InterceptionImpl.KeyDown(Key.X);
            Thread.Sleep(50);
            InterceptionImpl.KeyUp(Key.X);
            RestoreMovementKeys(rememberedKeys);
        }
    }

    static readonly Key[] movementKeys = [Key.W, Key.A, Key.S, Key.D, Key.LControl, Key.LShift];
    int RememberMovementKeys()
    {
        var state = 0;
        for (var i = 0; i < movementKeys.Length; i++)
            if (InterceptionImpl.IsKeyDown(movementKeys[i]))
                state |= 1 << i;
        return state;
    }

    void RestoreMovementKeys(int state)
    {
        for (var i = 0; i < movementKeys.Length; i++)
            if ((state & (1 << i)) != 0)
            {
                InterceptionImpl.KeyUp(movementKeys[i]);
                InterceptionImpl.KeyDown(movementKeys[i]);
            }
    }

    long lastRequestTime = 0;
    private protected override void OnKeyDown(Key key)
    {
        if (key == XKeybind)
        {
            if (state != AutoXState.None)
                if (Kernel32.GetTickCount() - lastRequestTime > 400)
                    state = AutoXState.None;

            if (state != AutoXState.RequestedPressing)
            {
                state = AutoXState.RequestedPressing;
                lastRequestTime = Kernel32.GetTickCount();

                InterceptionImpl.KeyDown(Key.F);
                InterceptionImpl.KeyUp(Key.F);
            }
        }
    }
}