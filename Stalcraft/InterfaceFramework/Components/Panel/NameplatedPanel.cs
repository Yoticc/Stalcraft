using System.Drawing;

class NameplatedPanel : BorderedPanel
{
    public NameplatedPanel(
        ConsoleText nameplateText,
        Point? location = null, 
        Size? size = null,
        ConsoleBackgroundColor backgroundColor = default,
        ConsoleTextStyles borderStyles = default,
        PanelBorderStyle panelBorderStyle = default,
        IEnumerable<Control>? controls = null
    ) : base(location, size, backgroundColor, borderStyles, panelBorderStyle, controls)
    {
        NameplateText = nameplateText;

        label = new Label(
            text: nameplateText,
            location = new Point(1, -1)
        );

        AddControls(label);
    }

    Label label;

    public ConsoleText NameplateText { get; private set; }

    public void SetNameplateText(ConsoleText nameplateText)
    {
        label.SetText(NameplateText = nameplateText);
        
        Redraw();
    }
}