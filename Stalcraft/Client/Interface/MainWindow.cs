#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
using System.Drawing;
class MainWindow : Window
{
    public MainWindow() : base("Stalcraft client", 46, 8) { }

    MultistyleLabel[] hackLabels;

    private protected override void OnInit()
    {
        HackManager.HackTurned += OnHackTurned;

        var barLabel = new Label(
            text: new(text: "5:20pm", styles: ConsoleForegroundColor.DarkGray),
            location: new(32, 0)
        );
        //AddControl(barLabel);

        AddButtons();
        AddHacks();
        AddOptionsPanel();
        
        //Application.SetSize(new(40, 4));

        void AddButtons()
        {
            LabelEventArgs hideHeaderEvent = sender => sender.SetStyle(
                !WindowManagement.Header 
                ? ConsoleForegroundColor.DarkGreen 
                : sender.IsHoveredByMouse
                  ? ConsoleForegroundColor.Red 
                  : ConsoleForegroundColor.DarkRed
            );

            var location = Width - 5;

            var hideHeaderButton = new Label(
                location: new(location, 0),
                text: new(text: "z", styles: ConsoleForegroundColor.DarkRed)
            )
            { 
                MouseEnter = hideHeaderEvent,
                MouseLeave = hideHeaderEvent,
                MouseLeftClick = sender =>
                {
                    WindowManagement.Header = !WindowManagement.Header;
                    hideHeaderEvent(sender);
                }
            };

            LabelEventArgs topmostEvent = sender => sender.SetStyle(
                WindowManagement.Topmost
                ? ConsoleForegroundColor.DarkGreen
                : sender.IsHoveredByMouse
                  ? ConsoleForegroundColor.Red
                  : ConsoleForegroundColor.DarkRed
            );

            var topmostButton = new Label(
                location: new(location += 2, 0),
                text: new(text: "v", styles: ConsoleForegroundColor.DarkRed)
            )
            {
                MouseEnter = topmostEvent,
                MouseLeave = topmostEvent,
                MouseLeftClick = sender =>
                {
                    WindowManagement.Topmost = !WindowManagement.Topmost;
                    topmostEvent(sender);
                }
            };

            LabelEventArgs closeEvent = sender => sender.SetStyle(
                sender.IsHoveredByMouse
                ? ConsoleForegroundColor.Red
                : ConsoleForegroundColor.DarkRed
            );
            var closeButton = new Label(
                location: new(location += 2, 0),
                text: new(text: "o", styles: ConsoleForegroundColor.DarkRed)
            );
            closeButton.MouseEnter += closeEvent;
            closeButton.MouseLeave += closeEvent;
            closeButton.MouseLeftClick += sender => Environment.Exit(0);

            AddControls(hideHeaderButton, topmostButton, closeButton);
        }

        void AddHacks()
        {
            var hacks = HackManager.Hacks;

            hackLabels = new MultistyleLabel[hacks.Count];
            for (var index = 0; index < hacks.Count; index++)
            {
                var hack = hacks[index];
                var text = GetHackLabelText(hack);
                var label = new MultistyleLabel(
                    location: new(0, index),
                    text: text
                );
                hackLabels[index] = label;
            }

            var hacksPanel = new NameplatedPanel(
                nameplateText: new(text: "hacks", styles: ConsoleForegroundColor.DarkYellow),
                location: new(0, 1),
                size: new(24, 4),
                panelBorderStyle: PanelBorderStyle.ASCII,
                borderStyles: ConsoleForegroundColor.DarkYellow,
                controls: hackLabels
            );

            AddControls(hacksPanel);
        }

        void AddOptionsPanel()
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

    void OnHackTurned(Hack hack)
    {
        var index = hack.InitIndex;
        var label = hackLabels[index];
        var text = GetHackLabelText(hack);  
        label.SetText(text);
    }

    ConsoleMultistyleText GetHackLabelText(Hack hack)
    {
        var text = new ConsoleMultistyleText();

        var styles = hack.IsEnabled ? ConsoleForegroundColor.Green : ConsoleForegroundColor.Red;
        text.Add(text: hack.Name, styles: styles);
        if (hack.HasKeybind)
        {
            var keybind = hack.Keybind.Value;
            var formattedKeybind = KeysFormatter.Formate(keybind);

            text.Add(text: " [", styles: ConsoleForegroundColor.DarkGray);
            text.Add(text: formattedKeybind);
            text.Add(text: "]", styles: ConsoleForegroundColor.DarkGray);
        }

        return text;
    }

    class HackPanel : Panel
    {
        public HackPanel(Hack hack, Point location, Size size) : base(location, size)
        {
            this.hack = hack;

            nameLabel = new HackNameLabel(hack);
            nameLabel.MouseClick += sender => (sender as HackNameLabel)!.Hack.Turn();

            keybindLabel = new HackKeybindLabel(hack);
            keybindLabel.MouseRightClick += sender =>
            {
                var label = (sender as HackKeybindLabel)!;
                var hack = label.Hack;
                OnKeybindClick(label, hack);
            };

            AddControls(nameLabel, keybindLabel);
            UpdateHackState();
        }

        Hack hack;
        HackNameLabel nameLabel;
        HackKeybindLabel keybindLabel;

        void UpdateHackState()
        {
            var style = hack.IsEnabled ? ConsoleForegroundColor.Green : ConsoleForegroundColor.Red;
            nameLabel.SetStyle(style);

            var text = new ConsoleMultistyleText();
            text.Add(text: " [", styles: ConsoleForegroundColor.DarkGray);

            var keybind = hack.Keybind.Value;
            var formattedKeybind = KeysFormatter.Formate(keybind);
            text.Add(text: formattedKeybind);

            text.Add(text: "]", styles: ConsoleForegroundColor.DarkGray);
            keybindLabel.SetText(text);
        }

        void OnKeybindClick(HackKeybindLabel label, Hack hack)
        {
            {
                var text = new ConsoleMultistyleText();
                text.Add(text: " [", styles: ConsoleForegroundColor.DarkGray);
                text.Add(text: "...", styles: ConsoleForegroundColor.DarkGray | ConsoleTextStyles.Awaiting);
                text.Add(text: "]", styles: ConsoleForegroundColor.DarkGray);
                label.SetText(text);
            }

            var macro = hack.Macro;
            macro.OnKeyUp += OnKeyUp;

            bool OnKeyUp(Keys key)
            {
                macro.OnKeyUp -= OnKeyUp;
                hack.SetKeybind(key);
                UpdateHackState();

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