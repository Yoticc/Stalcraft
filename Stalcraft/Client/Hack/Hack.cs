using ScreenCapture;

abstract unsafe class Hack
{
    static int initIndex;

    public Hack(string name)
    {
        InitIndex = initIndex++;

        Name = name;
        Dispatcher = new(this);

        var hackState = Config->Hacks->GetHackState(this);
        keybind = hackState->Keybind;
        isEnabled = hackState->IsEnabled;
    }

    public HackDispatcher Dispatcher;

    public Action? HackTurned;

    private protected static Configuration* Config => ConfigurationFile.Config;

    public SettingsPanel? SettingsPanel { get; private protected set; }

    Keys* keybind;
    public Keys Keybind { get => *keybind; private set => *keybind = value; }

    bool* isEnabled;
    public bool IsEnabled { get => *isEnabled; private set => *isEnabled = value; }

    public string Name { get; private init; }
    public int InitIndex { get; private set; }

    public void SetKeybind(Keys key) => Keybind = key;

    public void Turn()
    {
        if (IsEnabled)
            Disable();
        else Enable();
    }

    public void Enable()
    {
        if (IsEnabled)
            return;
        IsEnabled = true;

        OnEnable();
        HackTurned?.Invoke();
    }

    public void Disable()
    {
        if (!IsEnabled)
            return;
        IsEnabled = false;

        OnDisable();
        HackTurned?.Invoke();
    }

    public virtual bool ShouldCaptureFrame() => false;

    protected private virtual void OnCaptureFrame(Frame frame, FrameState frameState, MemoryBitmap bitmap) { }
    protected private virtual void OnInit() { }
    protected private virtual void OnEnable() { }
    protected private virtual void OnDisable() { }
    protected private virtual void OnKeyDown(Keys key) { }
    protected private virtual void OnKeyUp(Keys key) { }

    public class HackDispatcher(Hack owner)
    {
        public void InvokeOnCaptureFrame(Frame frame, FrameState frameState, MemoryBitmap bitmap) => owner.OnCaptureFrame(frame, frameState, bitmap);
        public void InvokeOnInit() => owner.OnInit();
        public void InvokeOnKeyDown(Keys key) => owner.OnKeyDown(key);
        public void InvokeOnKeyUp(Keys key) => owner.OnKeyUp(key);
    }
}