class OverlayWindow : Window
{
    public static OverlayWindow Instance;

    public OverlayWindow(HackListPanel hackListPanel) : base("Overlay", 24, 4)
    {
        Instance = this;
        this.hackListPanel = hackListPanel;

        ConsoleWindowWorkspace.OneCharCoordinations = new(Width - 1, 0);
    }

    HackListPanel hackListPanel;

    public void DisappearHacks() => RemoveControl(hackListPanel);

    public void AppearHacks() => AddControl(hackListPanel);
}