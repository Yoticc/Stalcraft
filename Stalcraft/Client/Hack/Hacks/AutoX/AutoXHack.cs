using ScreenCapture;

unsafe class AutoXHack : Hack
{
    static AutoXSettings* settings = Config->Settings->AutoX;
    public AutoXHack() : base("auto x")
    {
        SettingsPanel = new AutoXSettingsPanel();
    }

    Keys* xKeybind = settings->Keybind;
    public Keys XKeybind { get => *xKeybind; set => *xKeybind = value; }

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
            Interception.KeyDown(Keys.X);
            Thread.Sleep(50);
            Interception.KeyUp(Keys.X);
            RestoreMovementKeys(rememberedKeys);
        }
    }

    static readonly Keys[] movementKeys = [Keys.W, Keys.A, Keys.S, Keys.D, Keys.LControl, Keys.LShift];
    int RememberMovementKeys()
    {
        var state = 0;
        for (var i = 0; i < movementKeys.Length; i++)
            if (Interception.IsKeyDown(movementKeys[i]))
                state |= 1 << i;
        return state;
    }

    void RestoreMovementKeys(int state)
    {
        for (var i = 0; i < movementKeys.Length; i++)
            if ((state & (1 << i)) != 0)
            {
                Interception.KeyUp(movementKeys[i]);
                Interception.KeyDown(movementKeys[i]);
            }
    }

    long lastRequestTime = 0;
    private protected override void OnKeyDown(Keys key)
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

                Interception.KeyDown(Keys.F);
                Interception.KeyUp(Keys.F);
            }
        }
    }
}