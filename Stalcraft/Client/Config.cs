using System.Runtime.InteropServices;

unsafe struct Config : IDisposable
{
    public const int HacksCount = 4;
    static readonly string ConfigPath = Path.Combine(Path.GetTempPath(), "stalcraft.cfg");

    public static readonly Config* DefaultConfig;
    static Config()
    {
        *(DefaultConfig = (Config*)Marshal.AllocCoTaskMem(sizeof(Config))) = new Config();

        var config = DefaultConfig;

        var keybinds = (Keys*)config->Keybinds;
        keybinds[0] = Keys.U;
        keybinds[1] = Keys.I;
        keybinds[2] = Keys.O;
        keybinds[3] = Keys.P;

        config->SetOptionsToDefault();

        new Thread(AutoSaveThreadBody).Start();
    }

    public fixed short Keybinds[HacksCount];
    public fixed bool EnableStates[HacksCount];

    public fixed int ClientWindowOpacity[1];
    public fixed int StalcraftWindowOpacity[1];

    public void SetOptionsToDefault()
    {
        fixed (Config* self = &this)
        {
            *self->ClientWindowOpacity = 80;
            *self->StalcraftWindowOpacity = 100;
        }        
    }

    public void Save()
    {
        fixed (Config* pointer = &this)
        {
            var span = new ReadOnlySpan<byte>(pointer, sizeof(Config));
            File.WriteAllBytes(ConfigPath, span);
        }
    }

    public static Config* Load()
    {
        var bytes = File.Exists(ConfigPath) ? File.ReadAllBytes(ConfigPath) : [];

        if (bytes.Length == sizeof(Config))
        {
            var config = (Config*)Marshal.AllocCoTaskMem(sizeof(Config));
            var configSpan = new Span<byte>(config, sizeof(Config));
            new Span<byte>(bytes).CopyTo(configSpan);
            return config;
        }
        else return DefaultConfig;
    }

    static void AutoSaveThreadBody()
    {
        while (true)
        {
            Thread.Sleep(30000);
            var config = HackManager.Config;
            if (config is not null)
                config->Save();
        }
    }

    public void Dispose()
    {
        fixed (Config* pointer = &this)
            Marshal.FreeCoTaskMem((nint)pointer);
    }
}