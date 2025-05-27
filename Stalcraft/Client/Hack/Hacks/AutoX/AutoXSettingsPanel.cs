unsafe class AutoXSettingsPanel : SettingsPanel
{
    static AutoXSettings* settings = Config->Settings->AutoX;
    public AutoXSettingsPanel()
    {
        xKeybindOption = new(
            optionName: "x key",
            key: *settings->Keybind,
            valueChange: value => *settings->Keybind = value,
            location: new(0, 0));

        AddControls(xKeybindOption);
    }

    KeybindOptionPanel xKeybindOption;

    public override void Update()
    {
        xKeybindOption.SetValue(*settings->Keybind);
    }
}