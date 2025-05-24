#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
class MainWindow : Window
{
    public MainWindow() : base("Stalcraft client", 46, 8) { }

    OverlayWindow overlayWindow;
    HackListPanel hackListPanel;
    NameplatedPanel nameplatedHacksPanel;
    Button closeButton;
    Button pinButton;

    private protected override void OnInit()
    {
        InitHacks();
        InitOverlay();
        InitButtons();
        InitOptionsPanel();

        new Thread(() =>
        {
            Thread.Sleep(500);


        }).Start();

        void InitHacks()
        {
            hackListPanel = new HackListPanel();

            nameplatedHacksPanel = new NameplatedPanel(
                nameplateText: new(text: "hacks", styles: ConsoleForegroundColor.DarkYellow),
                location: new(0, 1),
                size: new(24, 4),
                panelBorderStyle: PanelBorderStyle.ASCII,
                borderStyles: ConsoleForegroundColor.DarkYellow
            );

            AppearHacks();
            AddControls(nameplatedHacksPanel);
        }

        void InitOverlay()
        {
            overlayWindow = new OverlayWindow(hackListPanel);
        }

        void InitButtons()
        {
            closeButton = new CloseButton(location: new(Width - 1, 0));
            pinButton = new PinButton(this, overlayWindow, location: new(Width - 3, 0));

            AddControls(closeButton, pinButton);
        }

        void InitOptionsPanel()
        {
            var optionsPanel = new NameplatedPanel(
                nameplateText: new(text: "options", styles: ConsoleForegroundColor.DarkGreen),
                location: new(28, 1),
                size: new(16, 4),
                panelBorderStyle: PanelBorderStyle.ASCII,
                borderStyles: ConsoleForegroundColor.Green,
                controls: []
            );

            AddControls(optionsPanel);
        }
    }

    public void DisappearHacks() => nameplatedHacksPanel.RemoveControl(hackListPanel);

    public void AppearHacks() => nameplatedHacksPanel.AddControl(hackListPanel);
}