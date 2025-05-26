using System.Drawing;

class PinButton : Button
{
    public PinButton(MainWindow mainWindow, OverlayWindow overlayWindow, Point location = default)
        : base(new(text: "o", styles: ConsoleForegroundColor.Gray), location)
        => (this.mainWindow, this.overlayWindow) = (mainWindow, overlayWindow);

    MainWindow mainWindow;
    OverlayWindow overlayWindow;

    private protected override void OnClick()
    {
        var pinned = !ConsoleWindowState.IsPinned;
        ConsoleWindow.IsTopmost = pinned;

        if (pinned)
        {
            ConsoleWindowState.State = ConsoleWindowState.Pinned;

            mainWindow.RemoveControl(this);
            overlayWindow.AddControl(this);

            mainWindow.DisappearHacks();
            overlayWindow.AppearHacks();

            SetLocation(Window.Width - 1, 0);
            mainWindow.OpenAsChild(overlayWindow);
            ConsoleWindow.MoveWindow((mainWindow.Width - overlayWindow.Width - 2) * Console.CharWidth, 0);
        }
        else
        {
            ConsoleWindowState.State = ConsoleWindowState.Default;

            overlayWindow.RemoveControl(this);
            mainWindow.AddControl(this);

            overlayWindow.DisappearHacks();
            mainWindow.AppearHacks();

            SetLocation(Window.Width - 4, 0);
            overlayWindow.Close();
            ConsoleWindow.MoveWindow((overlayWindow.Width - mainWindow.Width + 2) * Console.CharWidth, 0);
        }

        UpdateStyles();
        ConsoleWindowState.EnsureStateIsApplied();
    }

    private protected override void OnMouseEnter() => UpdateStyles();

    private protected override void OnMouseLeave() => UpdateStyles();

    void UpdateStyles() => SetStyle(
        IsHoveredByMouse
        ? ConsoleForegroundColor.White
        : ConsoleForegroundColor.Gray
    );
}