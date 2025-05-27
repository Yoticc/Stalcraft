using System.Drawing;
using System.Net.Http.Headers;
using System.Text;

abstract class Window
{
    public Window(string title, int width, int height)
    {
        (Title, Width, Height) = (title, width, height);

        Dispatcher = new(this);
    }

    public WindowDispatcher Dispatcher;

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
        ConsoleApplication.SetCurrentWindow(this);
        ConsoleApplication.SetSize(Width, Height);

        Dispatcher.InvokeOnInit();
        Draw();
        Dispatcher.InvokeOnOpen();
    }

    public void OpenAsChild(Window window)
    {
        if (window is null)
            throw new ArgumentNullException("window");

        NextWindow = window;
        window.PreviousWindow = this;
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
        PreviousWindow = null;
        NextWindow = null;

        previousWindow.Open();
    }

    void Draw()
    {
        if (!IsInitialized)
            return;

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

    public string DumpLayers()
    {
        var sb = new StringBuilder();

        for (var i = 0; i < Controls.Count; i++)
        {
            var control = Controls[i];
            DumpLayer(0);

            void DumpLayer(int depth)
            {
                sb.Append(new string(' ', depth * 2));
                sb.Append(control.GetType().Name);
                sb.Append(' ');
                sb.Append(control.AbsoluteBounds.Left);
                sb.Append(' ');
                sb.Append(control.AbsoluteBounds.Top);
                sb.Append(' ');
                sb.Append(control.AbsoluteBounds.Right);
                sb.Append(' ');
                sb.Append(control.AbsoluteBounds.Bottom);
                sb.Append('\n');

                depth++;
                var controls = control.Controls;
                for (var i = 0; i < controls.Count; i++)
                {
                    control = controls[i];
                    DumpLayer(depth);
                }
            }
        }

        return sb.ToString();
    }

    private protected virtual void OnMouseRelativeMove(int x, int y)
    {
        if (lastClickedControl is null)
            return;

        lastClickedControl.Dispatcher.InvokeOnMouseRelativeMove(x, y);
    }

    void SetLastHoveredControl(Control? control)
    {
        if (lastHoveredControl == control)
            return;
        
        if (control is not null)
            control.Dispatcher.InvokeOnMouseEnter();
            
        if (lastHoveredControl is not null)
            lastHoveredControl.Dispatcher.InvokeOnMouseLeave();
        
        if (lastClickedControl is not null)
              SetLastClickedControl(null);

        lastHoveredControl = control;
    }

    void SetLastClickedControl(Control? control)
    {
        if (lastClickedControl == control)
            return;

        if (lastClickedControl is not null)
            lastClickedControl.Dispatcher.SetIsInDragState(false);

        if (control is not null)
            control.Dispatcher.SetIsInDragState(true);

        lastClickedControl = control;
    }

    Control? lastHoveredControl;
    Control? lastClickedControl;
    private protected virtual void OnMouseMove(int x, int y)
    {
        for (var i = 0; i < Controls.Count; i++)
        {
            var control = Controls[i];
            var bounds = control.AbsoluteBounds;
            if (x < bounds.Left || x >= bounds.Right || y < bounds.Top || y >= bounds.Bottom)
                continue;

            var enteredControl = FindEnteredControl(control);
            SetLastHoveredControl(enteredControl);
            return;
        }

        SetLastHoveredControl(null);

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
        SetLastHoveredControl(null);
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

    private protected virtual void OnMouseLeftDown()
    {
        if (lastHoveredControl is not null)
        {
            lastHoveredControl.Dispatcher.InvokeOnMouseLeftDown();

            SetLastClickedControl(lastHoveredControl);
        }
    }

    private protected virtual void OnMouseLeftUp()
    {
        if (lastHoveredControl is not null)
        {
            lastHoveredControl.Dispatcher.InvokeOnMouseLeftUp();

            SetLastClickedControl(null);
        }
    }

    private protected virtual void OnMouseRightClick() 
    {
        if (lastHoveredControl is not null)
            lastHoveredControl.Dispatcher.InvokeOnMouseRightClick();
    }

    private protected virtual void OnMouseDrag(int x, int y)
    {
        if (lastHoveredControl is not null)
        {
            if (lastClickedControl != lastHoveredControl)
                SetLastClickedControl(null);
            else
            {
                var location = lastHoveredControl.AbsoluteLocation;
                lastHoveredControl.Dispatcher.InvokeOnMouseDrag(x - location.X, y - location.Y);
            }
        }
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
        public void InvokeOnMouseRelativeMove(int x, int y) => owner.OnMouseRelativeMove(x, y);
        public void InvokeOnMouseMove(int x, int y) => owner.OnMouseMove(x, y);
        public void InvokeOnMouseEnter() => owner.OnMouseEnter();
        public void InvokeOnMouseLeave() => owner.OnMouseLeave();
        public void InvokeOnMouseClick() => owner.OnMouseClick();
        public void InvokeOnMouseLeftClick() => owner.OnMouseLeftClick();
        public void InvokeOnMouseLeftDown() => owner.OnMouseLeftDown();
        public void InvokeOnMouseLeftUp() => owner.OnMouseLeftUp();
        public void InvokeOnMouseRightClick() => owner.OnMouseRightClick();
        public void InvokeOnMouseDrag(int x, int y) => owner.OnMouseDrag(x, y);
    }
}