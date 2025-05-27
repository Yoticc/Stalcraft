class HackPanel : Panel
{
    public HackPanel(HackListPanel parent, Hack hack) : base(location: new(0, hack.InitIndex), size: new(parent.Width, 1))
    {
        this.hack = hack;

        nameLabel = new HackNameLabel(hack)
        {
            MouseLeftClick = sender =>
            {
                var label = (sender as HackNameLabel)!;
                label.Hack.Turn();
            },
            MouseRightClick = sender =>
            {
                if (ConsoleWindowState.IsHasPinnedState)
                    return;

                var label = (sender as HackNameLabel)!;
                var hack = label.Hack;
                MainWindow.Instance.SetSettingsPanel(hack);
            },
            MouseEnter = sender => UpdateLabelState(),
            MouseLeave = sender => UpdateLabelState(),
        };

        keybindLabel = new KeybindSelector(key: hack.Keybind, valueChange: hack.SetKeybind, location: new(hack.Name.Length + 1, 0));

        AddControls(nameLabel, keybindLabel);
        UpdateHackState();
    }

    Hack hack;
    HackNameLabel nameLabel;
    KeybindSelector keybindLabel;

    public void UpdateHackState()
    {
        UpdateLabelState();
        UpdateKeybindState();
    }

    public void UpdateLabelState()
    {
        var style =
            hack.IsEnabled
            ? ConsoleForegroundColor.Gray | ConsoleTextStyles.Inverse
            : nameLabel.IsHoveredByMouse
              ? ConsoleForegroundColor.White
              : ConsoleForegroundColor.Gray;

        nameLabel.SetStyle(style);
    }

    public void UpdateKeybindState() => keybindLabel.SetValue(hack.Keybind);
}