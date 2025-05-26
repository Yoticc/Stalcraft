#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
unsafe class MainWindow : Window
{
    public static MainWindow Instance;

    public MainWindow() : base("Stalcraft client", 52, 16) => Instance = this;

    OverlayWindow overlayWindow;
    HackListPanel hackListPanel;
    NameplatedPanel nameplatedHacksPanel;
    NameplatedPanel nameplatedSettingsPanel;
    CloseButton closeButton;
    PinButton pinButton;

    static readonly DefaultSettingsPanel defaultSettingsPanel = new();
    Panel? settingsPanel;
    public void SetSettingsPanel(Hack hack)
    {
        var panel = hack.SettingsPanel;
        if (panel is null)
            return;

        if (panel == settingsPanel)
        {
            nameplatedSettingsPanel.RemoveControl(settingsPanel);
            nameplatedSettingsPanel.AddControl(defaultSettingsPanel);
            settingsPanel = null;
            nameplatedSettingsPanel.SetNameplateText($"settings");
        }
        else
        {
            if (settingsPanel is not null)
            {
                nameplatedSettingsPanel.RemoveControl(settingsPanel);
                nameplatedSettingsPanel.AddControl(panel);
                settingsPanel = panel;
            }
            else
            {
                nameplatedSettingsPanel.RemoveControl(defaultSettingsPanel);
                nameplatedSettingsPanel.AddControl(panel);
                settingsPanel = panel;
            }

            nameplatedSettingsPanel.SetNameplateText($"{hack.Name} settings");
        }   
    }

    private protected override void OnInit()
    {
        InitHacks();
        InitOverlay();
        Initheader();
        InitOptions();
        InitSettings();

        void InitHacks()    
        {
            hackListPanel = new HackListPanel();

            nameplatedHacksPanel = new NameplatedPanel(
                nameplateText: new(text: "hacks", styles: ConsoleForegroundColor.Gray),
                location: new(1, 1),
                size: new(24, 4),
                borderStyles: ConsoleForegroundColor.Gray
            );

            AppearHacks();
            AddControls(nameplatedHacksPanel);
        }

        void InitOverlay()
        {
            overlayWindow = new OverlayWindow(hackListPanel);
        }

        void Initheader()
        {
            closeButton = new CloseButton(location: new(Width - 2, 0));
            pinButton = new PinButton(this, overlayWindow, location: new(Width - 4, 0));

            AddControls(closeButton, pinButton);
        }

        void InitOptions()
        {
            var optionsPanel = new NameplatedPanel(
                nameplateText: new(text: "options", styles: ConsoleForegroundColor.Gray),
                location: new(29, 1),
                size: new(20, 4),
                borderStyles: ConsoleForegroundColor.Gray
            );

            var clientWindowOpacity = Config->Options->ClientWindowOpacity;
            var stalcraftWindowOpacity = Config->Options->StalcraftWindowOpacity;
            var copaq = new OptionPanel(
                optionsPanel,
                optionName: "copaq",
                minValue: 40,
                maxValue: 100,
                defaultValue: *clientWindowOpacity,
                opacity => *clientWindowOpacity = ConsoleWindow.Opacity = opacity,
                location: new(0, 0));

            var gopaq = new OptionPanel(
                optionsPanel,
                optionName: "gopaq",
                minValue: 40,
                maxValue: 100,
                defaultValue: *stalcraftWindowOpacity,
                opacity => *stalcraftWindowOpacity = StalcraftWindow.Opacity = opacity,
                location: new(0, 1));

            var defaults = new Button(text: new(text: "default", styles: ConsoleForegroundColor.Gray), location: new(optionsPanel.Width - 9, 3))
            {
                MouseEnter = button => button.SetStyle(ConsoleForegroundColor.White),
                MouseLeave = button => button.SetStyle(ConsoleForegroundColor.Gray),
                MouseLeftClick = button =>
                {
                    ConfigurationFile.SetConfigToDefault();

                    copaq.SetValue(*clientWindowOpacity);
                    gopaq.SetValue(*stalcraftWindowOpacity);

                    hackListPanel.Update();

                    foreach (var hack in HackManager.Hacks)
                        hack.SettingsPanel?.Update();
                }
            };

            optionsPanel.AddControls(copaq, gopaq, defaults);

            AddControls(optionsPanel);
        }
        
        void InitSettings()
        {
            nameplatedSettingsPanel = new(
                nameplateText: new(text: "settings", styles: ConsoleForegroundColor.Gray),
                location: new(1, 8),
                size: new(48, 5),
                borderStyles: ConsoleForegroundColor.Gray
            );

            AddControl(nameplatedSettingsPanel);
            nameplatedSettingsPanel.AddControl(defaultSettingsPanel);
        }
    }

    public void DisappearHacks() => nameplatedHacksPanel.RemoveControl(hackListPanel);

    public void AppearHacks() => nameplatedHacksPanel.AddControl(hackListPanel);
}