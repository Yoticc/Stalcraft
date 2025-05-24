using System.Drawing;

class PinButton : Button
{
    public PinButton(MainWindow mainWindow, OverlayWindow overlayWindow, Point location = default)
        : base(new(text: "o", styles: ConsoleForegroundColor.DarkRed), location)
        => (this.mainWindow, this.overlayWindow) = (mainWindow, overlayWindow);

    MainWindow mainWindow;
    OverlayWindow overlayWindow;

    bool pinned;
    private protected override void OnClick()
    {
        pinned = !pinned;
        ConsoleWindow.Header = !pinned;
        ConsoleWindow.Topmost = pinned;

        if (pinned)
        {
            mainWindow.RemoveControl(this);
            overlayWindow.AddControl(this);

            mainWindow.DisappearHacks();
            overlayWindow.AppearHacks();

            SetLocation(Window.Width - 1, 0);
            mainWindow.OpenAsChild(overlayWindow);
        }
        else
        {
            overlayWindow.RemoveControl(this);
            mainWindow.AddControl(this);

            overlayWindow.DisappearHacks();
            mainWindow.AppearHacks();

            SetLocation(Window.Width - 3, 0);
            overlayWindow.Close();
        }

        UpdateStyles();
    }

    private protected override void OnMouseEnter() => UpdateStyles();

    private protected override void OnMouseLeave() => UpdateStyles();

    void UpdateStyles() => SetStyle(
        pinned
        ? IsHoveredByMouse
          ? ConsoleForegroundColor.Green
          : ConsoleForegroundColor.DarkGreen
        : IsHoveredByMouse
          ? ConsoleForegroundColor.Red
          : ConsoleForegroundColor.DarkRed);
}