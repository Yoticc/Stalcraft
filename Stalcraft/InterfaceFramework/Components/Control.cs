using System.Diagnostics.CodeAnalysis;
using System.Drawing;

abstract class Control
{
    public Control(Point? location = null, Size? size = null, IEnumerable<Control>? controls = null)
    {
        if (location.HasValue)
            Location = location.Value;

        if (size.HasValue)
            Size = size.Value;

        if (controls is not null)
            AddControls(controls);
    }

    [AllowNull] public Window Window;
    public Control? Parent;
    public readonly List<Control> Controls = new List<Control>();

    public ConsoleApplication Application => Window.Application;
    public bool IsInitialized => Window is not null && Application is not null;
    public bool HasParent => Parent is not null;

    public Point Location { get; private set; }
    public int X => Location.X;
    public int Y => Location.Y;

    public Size Size { get; private set; }
    public int Width => Size.Width;
    public int Height => Size.Height;

    public Point AbsoluteLocation
    {
        get
        {
            if (Parent is null)
                return Location;

            var parentLocation = Parent.AbsoluteLocation;
            var location = Location;

            var transformedLocation = new Point(parentLocation.X + location.X, parentLocation.Y + location.Y);
            return transformedLocation;
        }
    }

    // or LocalBounds and GlobalBounds
    public virtual Rectangle Bounds => new(Location, Size);
    public Rectangle AbsoluteBounds => new Rectangle(AbsoluteLocation, Size);

    public Rectangle ParentAbsoluteBounds => Parent is null ? Application.Bounds : Parent.AbsoluteBounds;

    public void SetWindow(Window? window)
    {
        Window = window;

        foreach (var control in Controls)
            control.SetWindow(window);        
    }

    public void SetParent(Control? parent) => Parent = parent;

    public void SetLocation(int x, int y)
    {
        Location = new(x, y);
        Draw();
    }

    public void SetSize(Size size)
    {
        Size = size;
        Draw();
    }

    public void SetWidth(int width) => SetSize(new(width, Height));

    public void SetHeight(int height) => SetSize(new(Width, height));

    public void Redraw()
    {
        if (Parent is not null)
            Parent.Draw();
        else Draw();
    }

    public void Draw()
    {
        if (!IsInitialized)
            return;

        OnDraw();
        foreach (var control in Controls)
            control.OnDraw();
    }

    void InternalAddControl(Control control)
    {
        Controls.Add(control);
        control.SetParent(this);
        control.SetWindow(Window);
    }

    public void AddControl(Control control)
    {
        InternalAddControl(control);
        Draw();
    }

    public void AddControls(params IEnumerable<Control> controls)
    {
        foreach (var control in controls)
            InternalAddControl(control);
        Draw();
    }

    void InternalRemoveControl(Control control)
    {
        control.SetWindow(null);
        control.SetParent(null);
    }

    public void RemoveControl(Control control)
    {
        if (!Controls.Remove(control))
            return;

        InternalRemoveControl(control);
        Draw();
    }

    public void RemoveControls(params Control[] controls)
    {
        foreach (var control in controls)
            InternalRemoveControl(control);
        Draw();
    }

    private protected virtual void OnDraw() { }
}