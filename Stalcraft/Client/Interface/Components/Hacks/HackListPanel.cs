unsafe class HackListPanel : Panel, IDisposable
{
    public HackListPanel() : base(size: new(22, HackManager.Hacks.Count))
    {
        var hacks = HackManager.Hacks;
        panels = new HackPanel[hacks.Count];
        for (var index = 0; index < hacks.Count; index++)
            panels[index] = new HackPanel(this, hacks[index]);

        AddControls(panels);
        RegisterEvents();
    }

    HackPanel[] panels;

    public void Update()
    {
        foreach (var panel in panels)
            panel.UpdateHackState();
    }

    void OnHackTurned(Hack hack)
    {
        var index = hack.InitIndex;
        var panel = panels[index];
        panel.UpdateLabelState();
    }

    void RegisterEvents() => HackManager.HackTurned += OnHackTurned;

    void UnregisterEvents() => HackManager.HackTurned -= OnHackTurned;

    bool disposed;
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;

        UnregisterEvents();
    }

    ~HackListPanel() => Dispose();
}