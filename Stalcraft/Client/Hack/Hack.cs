using ScreenCapture;

abstract unsafe class Hack
{
    public Hack(string name)
    {
        Name = name;
        Dispatcher = new(this);
    }

    public HackDispatcher Dispatcher;

    public Action? HackTurned;

    public string Name { get; private init; }
    public Keys DefaultKeybind { get; private init; }
    public Keys* KeybindPointer { get; private set; }
    public Keys Keybind { get => *KeybindPointer; private set => *KeybindPointer = value; }
    public int InitIndex { get; private set; }

    public bool* IsEnabledPointer { get; private set; }
    public bool IsEnabled { get => *IsEnabledPointer; private set => *IsEnabledPointer = value; }
    public bool HasKeybind => Keybind != default;

    public void SetIsEnabledPointer(bool* pointer) => IsEnabledPointer = pointer;

    public void SetKeybindPointer(Keys* key) => KeybindPointer = key;

    public void SetKeybind(Keys key) => *KeybindPointer = key;

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