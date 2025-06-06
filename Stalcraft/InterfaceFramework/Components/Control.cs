﻿using System.Diagnostics.CodeAnalysis;
using System.Drawing;

delegate void ControlEventArgs(Control sender);
delegate void ControlDragEventArgs(Control sender, int x, int y);
delegate void ControlMouseMoveEventArgs(Control sender, int x, int y);

abstract class Control
{
    public Control(Point location = default, Size size = default, IEnumerable<Control>? controls = null)
    {
        Dispatcher = new ControlDispatcher(this);

        SetLocation(location, silence: true);
        SetSize(size, silence: true);

        if (controls is not null)
            AddControls(controls);
    }

    [AllowNull] public Window Window;
    public Control? Parent;
    public ControlDispatcher Dispatcher;
    public readonly List<Control> Controls = new List<Control>();

    public bool IsInitialized => Window is not null;
    public bool HasParent => Parent is not null;

    public Point Location { get; private set; }
    public int X => Location.X;
    public int Y => Location.Y;

    public Size Size { get; private set; }
    public int Width => Size.Width;
    public int Height => Size.Height;

    public Point ClientLocation => ClientBounds.Location;

    public Point AbsoluteLocation
    {
        get
        {
            if (Parent is null)
                return Location;

            var parentLocation = Parent.AbsoluteClientLocation;
            var location = Location;

            var absoluteLocation = new Point(parentLocation.X + location.X, parentLocation.Y + location.Y);
            return absoluteLocation;
        }
    }

    public Point AbsoluteClientLocation
    {
        get
        {
            if (Parent is null)
                return ClientLocation;

            var parentLocation = Parent.AbsoluteClientLocation;
            var location = ClientLocation;

            var absoluteLocation = new Point(parentLocation.X + location.X, parentLocation.Y + location.Y);
            return absoluteLocation;
        }
    }

    public Rectangle Bounds => new(Location, Size);
    public Rectangle AbsoluteBounds => new Rectangle(AbsoluteLocation, Size);

    public virtual Rectangle ClientBounds => new(Location, Size);

    public bool IsHoveredByMouse { get; private set; }
    public bool IsInDragState { get; private set; }

    public ControlEventArgs? MouseEnter;
    public ControlEventArgs? MouseLeave;
    public ControlEventArgs? MouseClick;
    public ControlEventArgs? MouseLeftClick;
    public ControlEventArgs? MouseLeftDown;
    public ControlEventArgs? MouseLeftUp;
    public ControlEventArgs? MouseRightClick;
    public ControlDragEventArgs? MouseDrag;
    public ControlMouseMoveEventArgs? MouseRelativeMove;

    public void SetWindow(Window? window)
    {
        Window = window;

        foreach (var control in Controls)
            control.SetWindow(window);        
    }

    public void SetParent(Control? parent) => Parent = parent;

    public void SetLocation(int x, int y) => SetLocation(new(x, y));

    public void SetLocation(Point location, bool silence = false)
    {
        Location = location;
        if (!silence)
            Draw();
    }

    public void SetSize(Size size, bool silence = false)
    {
        Size = size;
        if (!silence)
            Draw();
    }

    public void SetWidth(int width, bool silence = false) => SetSize(new(width, Height), silence);

    public void SetHeight(int height, bool silence = false) => SetSize(new(Width, height), silence);

    public void Redraw()
    {
        if (!IsInitialized)
            return;

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
            control.Draw();
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
    private protected virtual void OnMouseEnter() => MouseEnter?.Invoke(this);
    private protected virtual void OnMouseLeave() => MouseLeave?.Invoke(this);
    private protected virtual void OnMouseClick() => MouseClick?.Invoke(this);
    private protected virtual void OnMouseLeftClick() => MouseLeftClick?.Invoke(this);
    private protected virtual void OnMouseLeftDown() => MouseLeftDown?.Invoke(this);
    private protected virtual void OnMouseLeftUp() => MouseLeftUp?.Invoke(this);
    private protected virtual void OnMouseRightClick() => MouseRightClick?.Invoke(this);
    private protected virtual void OnMouseDrag(int x, int y) => MouseDrag?.Invoke(this, x, y);
    private protected virtual void OnMouseRelativeMove(int x, int y) => MouseRelativeMove?.Invoke(this, x, y);

    public class ControlDispatcher(Control owner)
    {
        public void InvokeOnMouseEnter()
        {
            owner.IsHoveredByMouse = true;
            owner.OnMouseEnter();
        }
        public void InvokeOnMouseLeave()
        {
            owner.IsHoveredByMouse = false;
            owner.OnMouseLeave();
        }
        public void InvokeOnMouseClick() => owner.OnMouseClick();
        public void InvokeOnMouseLeftClick() => owner.OnMouseLeftClick();
        public void InvokeOnMouseLeftDown() => owner.OnMouseLeftDown();
        public void InvokeOnMouseLeftUp() => owner.OnMouseLeftUp();
        public void InvokeOnMouseRightClick() => owner.OnMouseRightClick();
        public void InvokeOnMouseDrag(int x, int y) => owner.OnMouseDrag(x, y);
        public void InvokeOnMouseRelativeMove(int x, int y) => owner.OnMouseRelativeMove(x, y);
        public void SetIsInDragState(bool state) => owner.IsInDragState = state;
    }
}