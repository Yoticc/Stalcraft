using System.Diagnostics.CodeAnalysis;
using System.Drawing;

abstract class Window
{
    public Window(string title, int width, int height)
    {
        (Title, Width, Height) = (title, width, height);

        Dispatcher = new(this);
    }

    public WindowDispatcher Dispatcher;

    [AllowNull] public ConsoleApplication Application;
    public Window? PreviousWindow;
    public Window? NextWindow;

    public readonly string Title;
    public readonly int Width;
    public readonly int Height;
    public readonly List<Control> Controls = new();
    
    public bool HasOwner => PreviousWindow is not null;
    public bool IsInitialized { get; private set; }
    
    public void Open()
    {
        Application.UpdateTitle();
        Draw();
    }

    public void OpenAsChild(Window window)
    {
        if (window is null)
            throw new ArgumentNullException("window");

        if (window.Application is not null)
        {
            DebugTools.Debug($"opening a window as a child that has already been opened");
            return;
        }

        NextWindow = window;
        window.PreviousWindow = this;
        window.Application = Application;
        window.Open();
    }

    //  n      n      y
    // [ ] -> [ ] -> [ ]
    //
    //  n
    // [ ]
    public void Close()
    {
        if (NextWindow is not null)
        {
            DebugTools.Debug($"attempt to close the master window \"{Title}\" with opened window \"{NextWindow.Title}\"");
            return;
        }

        var previousWindow = PreviousWindow;
        if (previousWindow is null)
        {
            DebugTools.Debug($"attempt to close the master window \"{Title}\"");
            return;
        }

        previousWindow.NextWindow = null;
        Application = null;
        PreviousWindow = null;
        NextWindow = null;

        previousWindow.Open();
    }

    void Draw()
    {
        if (!IsInitialized)
            return;

        Application.ClearBuffer();
        OnDraw();

        foreach (var control in Controls)
            control.Draw();
    }

    public void AddControl(Control control)
    {
        Controls.Add(control);
        control.SetWindow(this);

        Draw();
    }

    public void AddControls(params IEnumerable<Control> controls)
    {
        foreach (var control in controls)
            AddControl(control);

        Draw();
    }

    public void RemoveControl(Control control)
    {
        if (!Controls.Remove(control))
            return;
        control.SetWindow(null);

        Draw();
    }

    public void RemoveControls(params Control[] controls)
    {
        foreach (var control in controls)
            RemoveControl(control);

        Draw();
    }

    public Bitmap DumpBounds()
    {
        const int xmult = Console.CharWidth;
        const int ymult = Console.CharHeight;

        var bitmap = new Bitmap(Width * xmult, Height * ymult);
        var graphics = Graphics.FromImage(bitmap);

        var backgroundColor = Color.FromArgb(31, 31, 31);
        var backgroundBrush = new SolidBrush(backgroundColor);
        graphics.FillRectangle(backgroundBrush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

        for (var i = 0; i < Controls.Count; i++)
        {
            var control = Controls[i];
            DrawBounds(0);

            void DrawBounds(int depth)
            {
                var bounds = control.AbsoluteBounds;
                var grayScale = byte.MaxValue - depth * 32;
                var color = Color.FromArgb(grayScale, grayScale, grayScale);
                var pen = new Pen(color, 2);
                var rectangle = Rectangle.FromLTRB(bounds.Left * xmult, bounds.Top * ymult, bounds.Right * xmult, bounds.Bottom * ymult);
                //graphics.FillRectangle(backgroundBrush, rectangle);
                graphics.DrawRectangle(pen, rectangle);

                depth++;
                var controls = control.Controls;
                for (var i = 0; i < controls.Count; i++)
                {
                    control = controls[i];
                    DrawBounds(depth);
                }
            }
        }

        return bitmap;
    }

    Control? lastHoveredControl;
    private protected virtual void OnMouseMove(int x, int y)
    {
        for (var i = 0; i < Controls.Count; i++)
        {
            var control = Controls[i];
            var bounds = control.AbsoluteBounds;
            if (x < bounds.Left || x >= bounds.Right || y < bounds.Top || y >= bounds.Bottom)
                continue;

            var enteredControl = FindEnteredControl(control);
            if (enteredControl != lastHoveredControl)
            {
                if (lastHoveredControl is not null)
                    lastHoveredControl.Dispatcher.InvokeOnMouseLeave();
                
                lastHoveredControl = enteredControl;
                control.Dispatcher.InvokeOnMouseEnter();
            }            
            return;
        }

        if (lastHoveredControl is not null)
        {
            lastHoveredControl.Dispatcher.InvokeOnMouseLeave();
            lastHoveredControl = null;
        }

        Control FindEnteredControl(Control control)
        {
            var currentControl = control;
            var controls = control.Controls;
            for (var i = 0; i < controls.Count; i++)
            {
                control = controls[i];
                var bounds = control.AbsoluteBounds;
                if (x < bounds.Left || x >= bounds.Right || y < bounds.Top || y >= bounds.Bottom)
                    continue;

                currentControl = control;
                controls = control.Controls;
                i = -1;
            }

            return currentControl;
        }
    }

    private protected virtual void OnMouseEnter() { }

    private protected virtual void OnMouseLeave()
    {
        if (lastHoveredControl is not null)
        {
            lastHoveredControl.Dispatcher.InvokeOnMouseLeave();
            lastHoveredControl = null;
        }
    }

    private protected virtual void OnMouseClick()
    {
        if (lastHoveredControl is not null)
            lastHoveredControl.Dispatcher.InvokeOnMouseClick();
    }

    private protected virtual void OnMouseLeftClick() 
    {
        if (lastHoveredControl is not null)
            lastHoveredControl.Dispatcher.InvokeOnMouseLeftClick();
    }

    private protected virtual void OnMouseRightClick() 
    {
        if (lastHoveredControl is not null)
            lastHoveredControl.Dispatcher.InvokeOnMouseRightClick();
    }

    private protected virtual void OnDraw() { }
    private protected virtual void OnInit() { }
    private protected virtual void OnOpen() { }

    public class WindowDispatcher(Window owner)
    {
        public void InvokeOnInit()
        {
            if (owner.IsInitialized)
                return;

            owner.OnInit();
            owner.IsInitialized = true;
        }

        public void InvokeOnOpen() => owner.OnOpen();
        public void InvokeOnMouseMove(int x, int y) => owner.OnMouseMove(x, y);
        public void InvokeOnMouseEnter() => owner.OnMouseEnter();
        public void InvokeOnMouseLeave() => owner.OnMouseLeave();
        public void InvokeOnMouseClick() => owner.OnMouseClick();
        public void InvokeOnMouseLeftClick() => owner.OnMouseLeftClick();
        public void InvokeOnMouseRightClick() => owner.OnMouseRightClick();
    }
}