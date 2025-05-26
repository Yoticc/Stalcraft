using System.Drawing;

class NameplatedPanel : BorderedPanel
{
    public NameplatedPanel(
        ConsoleText nameplateText,
        Point location = default, 
        Size size = default,
        ConsoleBackgroundColor backgroundColor = default,
        ConsoleTextStyles borderStyles = default,
        IEnumerable<Control>? controls = null
    ) : base(location, size, backgroundColor, borderStyles, PanelBorderStyle.ASCII, controls)
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
        nameplateText.Text = nameplateText.Text.Replace(' ', '-');

        label.SetText(NameplateText = nameplateText);
        
        Redraw();
    }

    public void SetNameplateText(string nameplateText)
    {
        var text = new ConsoleText(text: nameplateText, styles: NameplateText.Styles);
        SetNameplateText(text);
    }
}