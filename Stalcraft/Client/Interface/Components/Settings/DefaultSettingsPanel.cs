class DefaultSettingsPanel : SettingsPanel
{
    public DefaultSettingsPanel()
    {
        var startTextLabel = new Label(text: new(text: "right click on hack to open its settings", styles: ConsoleForegroundColor.Gray | ConsoleTextStyles.Underline), location: new(4, 2));
        AddControl(startTextLabel);
    }
}