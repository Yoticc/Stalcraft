enum SetWindowPosFlags : int
{
    AsyncWindowPos = 0x4000,
    DeferRase = 0x2000,
    DrawFrame = 0x0020,
    FrameChanged = 0x0020,
    HideWindow = 0x0080,
    NoActive = 0x0010,
    NoCopyBits = 0x0100,
    NoMove = 0x0002,
    NoOwnerZOrder = 0x0200,
    NoRedraw = 0x0008,
    NoReposition = 0x0200,
    NoSendChanging = 0x0400,
    NoSize = 0x0001,
    NoZOrder = 0x0004,
    ShowWindow = 0x0040
}