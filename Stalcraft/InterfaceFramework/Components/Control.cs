abstract class Control
{
    ControlDispatcher dispatcher;
    public ControlDispatcher Dispatcher => dispatcher is null ? (dispatcher = new(this)) : dispatcher;

    ControlEvents events;
    public ControlEvents Events => events is null ? (events = new(this)) : events;

    public Window Owner { get; private protected set; }
    public List<Control> Controls { get; private protected set; } = new List<Control>();

    int x;
    public int X { get => x; set => ; }

    int y;
    public int Y { get; private protected set; }

    private protected virtual void OnAdded(Window owner) => events?.Added?.Invoke(owner);

    private protected virtual void OnDraw()
    {
        foreach (var control in Controls)
            control.OnDraw();

        events?.Draw?.Invoke();
    }

    public class ControlEvents(Control control)
    {
        public Action? Draw;
        public Action<Window>? Added;
    }

    public class ControlDispatcher(Control control)
    {
        public Window Owner { get => control.Owner; set => control.Owner = value; }

        public void OnDraw() => control.OnDraw();
        public void OnAdded(Window owner) => control.OnAdded(owner);
    }
}