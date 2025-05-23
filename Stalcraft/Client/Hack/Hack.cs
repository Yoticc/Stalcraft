using ScreenCapture;

abstract class Hack
{
    public Hack(string name, Keys? defaultKeybind = null)
    {
        Name = name;
        Keybind = DefaultKeybind = defaultKeybind;

        Dispatcher = new(this);
    }

    public HackDispatcher Dispatcher;

    public Action? HackTurned;

    public InterceptionManager Macro => HackManager.Macro;

    public string Name { get; private init; }
    public Keys? DefaultKeybind { get; private init; }
    public Keys? Keybind { get; private set; }
    public int InitIndex { get; private set; }

    public bool IsEnabled { get; private set; }
    public bool HasKeybind => Keybind != null;

    public void SetKeybind(Keys key) => Keybind = key;

    public void SetInitIndex(int index) => InitIndex = index;

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
    protected private virtual void OnUpdate() { }
    protected private virtual void OnInit() { }
    protected private virtual void OnEnable() { }
    protected private virtual void OnDisable() { }

    public class HackDispatcher(Hack owner)
    {
        public void InvokeOnCaptureFrame(Frame frame, FrameState frameState, MemoryBitmap bitmap) => owner.OnCaptureFrame(frame, frameState, bitmap);
        public void InvokeOnUpdate() => owner.OnUpdate();
        public void InvokeOnInit() => owner.OnInit();
    }
}