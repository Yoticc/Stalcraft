/// <summary>
/// Look at <href="https://learn.microsoft.com/ru-ru/windows/win32/winmsg/window-styles"/>
/// </summary>
[Flags]
enum WindowExStyles : uint
{
    AcceptFiles = 0x00000010,
    AppWindow = 0x00040000,
    ClientEdge = 0x00000200,
    Composited = 0x02000000,
    ContextHelp = 0x00000400,
    ControlParent = 0x00010000,
    DialogModalFrame = 0x00000001,
    Layered = 0x00080000,
    LayoutRtl = 0x00400000,
    Left = 0x00000000,
    LeftScrollBar = 0x00004000,
    LtrREading = 0x00000000,
    MdiChild = 0x00000040,
    NoActivate = 0x08000000,
    NoInheritLayout = 0x00100000,
    NoParentNotify = 0x00000004,
    OverlappedWindow = WindowEdge | ClientEdge,
    PaletteWindow = WindowEdge | TollWindow | TopMost,
    Right = 0x00001000,
    RightScrollBar = 0x00000000,
    RtlReading = 0x00002000,
    StaticEnge = 0x00020000,
    TollWindow = 0x00000080,
    TopMost = 0x00000008,
    Transparent = 0x00000020,
    WindowEdge = 0x00000100
}