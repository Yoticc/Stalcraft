class ConsoleWindowState
{
    static readonly ConsoleWindowState None = new();

    public static readonly ConsoleWindowState
        Default = new(),
        Pinned = new(),
        Hidden = new();

    ConsoleWindowState() { }

    public static bool IsHasPinnedState => state == Pinned || state == Hidden;

    public static bool IsDefault { get => state == Default; set => State = Default; }
    public static bool IsPinned { get => state == Pinned; set => State = Pinned; }
    public static bool IsHidden { get => state == Hidden; set => State = Hidden; }

    static ConsoleWindowState state = None;
    public static ConsoleWindowState State
    {
        get => state;
        set
        {
            state = value;
            ConsoleWindowWorkspace workspace;
            if (value == Default)
                workspace = ConsoleWindowWorkspace.NoScrollbars;
            else if (value == Pinned)
                workspace = ConsoleWindowWorkspace.NoHeaderAndScrollbars;
            else if (value == Hidden)
                workspace = ConsoleWindowWorkspace.OneChar;
            else throw new InvalidOperationException();
            ConsoleWindowWorkspace.State = workspace;
        }
    }

    public static void EnsureStateIsApplied()
    {
        if (IsDefault)
            return;

        var isActive = StalcraftWindow.IsActive || ConsoleWindow.IsActive;
        if (IsPinned && !isActive)
            IsHidden = true;
        else if (IsHidden && isActive)
            IsPinned = true;
    }
}