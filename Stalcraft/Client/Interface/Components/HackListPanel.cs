class HackListPanel : Panel, IDisposable
{
    public HackListPanel() : base(size: new(22, HackManager.Hacks.Count))
    {
        var hacks = HackManager.Hacks;
        panels = new HackPanel[hacks.Count];
        for (var index = 0; index < hacks.Count; index++)
            panels[index] = new HackPanel(this, hacks[index]);

        AddControls(panels);
        RegisterEvents();
    }

    HackPanel[] panels;

    void OnHackTurned(Hack hack)
    {
        var index = hack.InitIndex;
        var panel = panels[index];
        panel.UpdateLabelState();
    }

    void RegisterEvents() => HackManager.HackTurned += OnHackTurned;

    void UnregisterEvents() => HackManager.HackTurned -= OnHackTurned;

    bool disposed;
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;

        UnregisterEvents();
    }

    ~HackListPanel() => Dispose();

    class HackPanel : Panel
    {
        public HackPanel(HackListPanel parent, Hack hack) : base(location: new(0, hack.InitIndex), size: new(parent.Width, 1))
        {
            this.hack = hack;

            nameLabel = new HackNameLabel(hack)
            {
                MouseLeftClick = sender => (sender as HackNameLabel)!.Hack.Turn(),
                MouseEnter = sender => UpdateLabelState(),
                MouseLeave = sender => UpdateLabelState()
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

        class HackNameLabel : Label
        {
            public HackNameLabel(Hack hack) : base(text: new(hack.Name)) => Hack = hack;

            public readonly Hack Hack;
        }

        class HackKeybindLabel : MultistyleLabel
        {
            public HackKeybindLabel(Hack hack) : base(text: ConsoleMultistyleText.Empty, location: new(hack.Name.Length + 1, 0)) => Hack = hack;            

            public readonly Hack Hack;
        }
    }
}