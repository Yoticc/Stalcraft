using System.Diagnostics.CodeAnalysis;

abstract class Window
{
    public Window(string title)
    {
        Dispatcher = new(this);
        Events = new();
        Title = title;
    }

    public readonly WindowDispatcher Dispatcher;
    public readonly WindowEvents Events;

    [AllowNull] public ConsoleApplication Application { get; private set; }
    public Window? Owner { get; private set; }
    public Window? OpenedWindow { get; private set; }
    public string Title { get; private set; }
    public List<Control> Controls { get; private set; } = new();
    
    public bool HasOwner => Owner is not null;
    public bool IsInitialized => Application is not null;

    public void Load()
    {

    }

    public void Open()
    {
        Draw();
    }

    public void Draw()
    {
        if (!IsInitialized)
            return;

        Application.ClearBuffer();

        foreach (var control in Controls)
            control.Dispatcher.OnDraw();

        Dispatcher.OnDraw();
    }

    public void AddControl(Control control)
    {
        Dispatcher.OnControlAdded(control);

        Draw();
    }

    public void AddControls(params IEnumerable<Control> controls)
    {
        foreach (var control in controls)
            Dispatcher.OnControlAdded(control);

        Draw();
    }

    public void RemoveControl(Control control)
    {
        if (!Controls.Remove(control))
            return;

        Dispatcher.OnControlRemoved(control);

        Draw();
    }

    public void RemoveControls(params Control[] controls)
    {
        foreach (var control in controls)
            Dispatcher.OnControlRemoved(control);

        Draw();
    }

    private protected virtual void OnLoad() => Events.Load?.Invoke();
    private protected virtual void OnOpen() => Events.Open?.Invoke();
    private protected virtual void OnClose() => Events.Close?.Invoke();
    private protected virtual void OnDraw() => Events.Draw?.Invoke();
    private protected virtual void OnControlAdded(Control control)
    {
        control.Dispatcher.Owner = this;
        Events.ControlAdded?.Invoke(control);
    }
    private protected virtual void OnControlRemoved(Control control)
    {
        Events.ControlRemoved?.Invoke(control);
    }

    public class WindowDispatcher(Window owner)
    {
        public ConsoleApplication Application { get => owner.Application; set => owner.Application = value; }
        public Window? Owner { get => owner.Owner; set => owner.Owner = value; }
        public Window? OpenedWindow { get => owner.OpenedWindow; set => owner.OpenedWindow = value; }

        public void OnLoad() => owner.OnLoad();
        public void OnOpen() => owner.OnOpen();
        public void OnClose() => owner.OnClose();
        public void OnDraw() => owner.OnDraw();
        public void OnControlAdded(Control control) => owner.OnControlAdded(control);
        public void OnControlRemoved(Control control) => owner.OnControlRemoved(control);
    }

    public class WindowEvents
    {
        public Action? Load, Open, Close, Draw;
        public Action<Control>? ControlAdded, ControlRemoved;
    }
}