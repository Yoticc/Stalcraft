using System.Diagnostics.CodeAnalysis;

abstract class Window
{
    public Window(string title)
    {
        Title = title;
    }

    [AllowNull] public ConsoleApplication Application;
    public Window? PreviousWindow;
    public Window? NextWindow;

    public readonly string Title;
    public readonly List<Control> Controls = new();
    
    public bool HasOwner => PreviousWindow is not null;
    public bool IsInitialized => Application is not null;

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

    private protected virtual void OnDraw() { }
}