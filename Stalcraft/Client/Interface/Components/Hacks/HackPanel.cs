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

        keybindLabel = new HackKeybindLabel(hack)
        {
            MouseRightClick = sender =>
            {
                var label = (sender as HackKeybindLabel)!;
                var hack = label.Hack;
                OnKeybindMouseClick(label, hack);
            }
        };

        AddControls(nameLabel, keybindLabel);
        UpdateHackState();
    }

    Hack hack;
    HackNameLabel nameLabel;
    HackKeybindLabel keybindLabel;

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

    public void UpdateKeybindState()
    {
        var text = new ConsoleMultistyleText();
        text.Add(text: "[", styles: ConsoleForegroundColor.DarkGray);

        var keybind = hack.Keybind;
        if (keybind != default)
        {
            var formattedKeybind = KeysFormatter.Formate(keybind);
            text.Add(text: formattedKeybind);
        }

        text.Add(text: "]", styles: ConsoleForegroundColor.DarkGray);
        keybindLabel.SetText(text);
    }

    void OnKeybindMouseClick(HackKeybindLabel label, Hack hack)
    {
        var text = new ConsoleMultistyleText();
        text.Add(text: "[", styles: ConsoleForegroundColor.DarkGray);
        text.Add(text: "...", styles: ConsoleForegroundColor.Gray | ConsoleTextStyles.Awaiting);
        text.Add(text: "]", styles: ConsoleForegroundColor.DarkGray);
        label.SetText(text);

        Interception.OnKeyUp += OnKeyUp;

        bool OnKeyUp(Keys key)
        {
            Interception.OnKeyUp -= OnKeyUp;
            hack.SetKeybind(key);
            UpdateKeybindState();

            return false;
        }
    }
}