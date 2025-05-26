global using static SharedConfiguration;
using static Extensions;

static unsafe class SharedConfiguration
{
    public static Configuration* Config = ConfigurationFile.Config;
}

static unsafe class ConfigurationFile
{
    static readonly string ConfigPath = Path.Combine(Path.GetTempPath(), "stalcraft.cfg");

    static Configuration nogcConfig = new();
    public static Configuration* Config;

    static ConfigurationFile()
    {
        fixed (Configuration* config = &nogcConfig)
            Config = config;

        LoadSavedconfig();

        new Thread(AutoSaveThreadBody).Start();
    }

    public static void SetConfigToDefault() => nogcConfig = new();

    static void SaveConfigToFile() => File.WriteAllBytes(ConfigPath, new ReadOnlySpan<byte>(Config, sizeof(Configuration)));

    static void LoadSavedconfig()
    {
        if (!File.Exists(ConfigPath))
            return;

        var bytes = File.ReadAllBytes(ConfigPath);
        if (bytes.Length != sizeof(Configuration))
            return;

        fixed (byte* pointer = bytes)
            *Config = *(Configuration*)pointer;
    }

    static void AutoSaveThreadBody()
    {
        while (true)
        {
            Thread.Sleep(10000);
            SaveConfigToFile();
        }
    }
}

unsafe record struct Configuration()
{
    HacksConfiguration hacks = new();
    public HacksConfiguration* Hacks => &self(ref this)->hacks;

    OptionsConfiguration options = new();
    public OptionsConfiguration* Options => &self(ref this)->options;

    SettingsConfiguration settings = new();
    public SettingsConfiguration* Settings => &self(ref this)->settings;
}

unsafe record struct HacksConfiguration()
{
    HackState antiRecoil = new(Keys.Y);
    HackState smartSteal = new(Keys.U);
    HackState aimbot = new(Keys.I);
    HackState autoX = new(Keys.O);

    public HackState* States => &self(ref this)->antiRecoil;
    public HackState* GetHackState(Hack hack) => States + hack.InitIndex;

    public struct HackState
    {
        public HackState(Keys keybind) => this.keybind = keybind;

        Keys keybind;
        public Keys* Keybind => &self(ref this)->keybind;

        bool isEnabled;
        public bool* IsEnabled => &self(ref this)->isEnabled;
    }
}

unsafe record struct OptionsConfiguration()
{
    int clientWindowOpacity = 80;
    public int* ClientWindowOpacity => &self(ref this)->clientWindowOpacity;

    int stalcraftWindowOpacity = 100;
    public int* StalcraftWindowOpacity => &self(ref this)->stalcraftWindowOpacity;
}

unsafe record struct SettingsConfiguration()
{
    AimbotSettings aimbot = new();
    public AimbotSettings* Aimbot => &self(ref this)->aimbot;

    AntiRecoilSettings antiRecoil = new();
    public AntiRecoilSettings* AntiRecoil => &self(ref this)->antiRecoil;
}

unsafe record struct AimbotSettings()
{
    int fov = 250;
    public int* Fov => &self(ref this)->fov;

    int offset = 22;
    public int* Offset => &self(ref this)->offset;
}

unsafe record struct AntiRecoilSettings()
{
    Profile profile1, profile2, profile3, profile4;

    public Profile* Profiles => &self(ref this)->profile1;

    public record struct Profile
    {
        bool isExists;
        public bool* IsExists => &self(ref this)->isExists;

        int shift;
        public int* Shift => &self(ref this)->shift;

        int delay;
        public int* Delay => &self(ref this)->delay;

        Keys keybind;
        public Keys* Keybind => &self(ref this)->keybind;
    }
}

unsafe static class Extensions
{
    public static T* self<T>(ref T value) where T : unmanaged
    {
        fixed (T* pointer = &value)
            return pointer;
    }
}