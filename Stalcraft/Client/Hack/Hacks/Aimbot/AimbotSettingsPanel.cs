unsafe class AimbotSettingsPanel : SettingsPanel
{
    static AimbotSettings* settings = Config->Settings->Aimbot;
    public AimbotSettingsPanel()
    {
        fovOption = new OptionPanel(
            width: 25,
            optionName: "fov",
            minValue: 150,
            maxValue: 400,
            valueChange: value => *settings->Fov = value,
            defaultValue: *settings->Fov,
            location: new(0, 0));

        offsetOption = new OptionPanel(
            width: 25,
            optionName: "offset",
            minValue: 12,
            maxValue: 24,
            valueChange: value => *settings->Offset = value,
            defaultValue: *settings->Offset,
            location: new(0, 1));

        AddControls(fovOption, offsetOption);
    }

    OptionPanel fovOption;
    OptionPanel offsetOption;

    public override void Update()
    {
        fovOption.SetValue(*settings->Fov);
        offsetOption.SetValue(*settings->Offset);
    }
}