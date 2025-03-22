[Flags]
enum WindowStyles : uint
{
    Border = 0x800000,
    Caption = 0xc00000,
    Child = 0x40000000,
    ClipChildren = 0x2000000,
    ClipSiblings = 0x4000000,
    Disabled = 0x8000000,
    DlgFrame = 0x400000,
    Group = 0x20000,
    HScroll = 0x100000,
    Maximize = 0x1000000,
    MaximizeBox = 0x10000,
    Minimize = 0x20000000,
    MinimizeBox = 0x20000,
    Overlapped = 0x0,
    OverlappedWindow = Overlapped | Caption | SystemMenu | SizeFrame | MinimizeBox | MaximizeBox,
    Popup = 0x80000000u,
    PopupWindow = Popup | Border | SystemMenu,
    SizeFrame = 0x40000, // Think frame
    SystemMenu = 0x80000,
    TabStop = 0x10000,
    Visible = 0x10000000,
    VScroll = 0x200000
}
