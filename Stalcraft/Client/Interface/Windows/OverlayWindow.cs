class OverlayWindow : Window
{
    public OverlayWindow(HackListPanel hackListPanel) : base("Overlay", 24, 4) => this.hackListPanel = hackListPanel;

    HackListPanel hackListPanel;

    public void DisappearHacks() => RemoveControl(hackListPanel);

    public void AppearHacks() => AddControl(hackListPanel);
}