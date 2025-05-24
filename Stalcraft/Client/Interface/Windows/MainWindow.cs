#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
class MainWindow : Window
{
    public MainWindow() : base("Stalcraft client", 46, 8) { }

    HackListPanel hackListPanel;

    private protected override void OnInit()
    {
        AddButtons();
        AddHacks();
        AddOptionsPanel();

        void AddButtons()
        {
            LabelEventArgs hideHeaderEvent = sender => sender.SetStyle(
                !ConsoleWindow.Header 
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
                    ConsoleWindow.Header = !ConsoleWindow.Header;
                    hideHeaderEvent(sender);
                }
            };

            LabelEventArgs topmostEvent = sender => sender.SetStyle(
                ConsoleWindow.Topmost
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
                    ConsoleWindow.Topmost = !ConsoleWindow.Topmost;
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
            hackListPanel = new HackListPanel();

            var nameplatedHacksPanel = new NameplatedPanel(
                nameplateText: new(text: "hacks", styles: ConsoleForegroundColor.DarkYellow),
                location: new(0, 1),
                size: new(24, 4),
                panelBorderStyle: PanelBorderStyle.ASCII,
                borderStyles: ConsoleForegroundColor.DarkYellow,
                controls: [hackListPanel]
            );

            AddControls(nameplatedHacksPanel);
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
}