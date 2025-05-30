﻿public enum SystemMetric
{
    CXSCREEN = 0,  // 0x00
    CYSCREEN = 1,  // 0x01
    CXVSCROLL = 2,  // 0x02
    CYHSCROLL = 3,  // 0x03
    CYCAPTION = 4,  // 0x04
    CXBORDER = 5,  // 0x05
    CYBORDER = 6,  // 0x06
    CXDLGFRAME = 7,  // 0x07
    CXFIXEDFRAME = 7,  // 0x07
    CYDLGFRAME = 8,  // 0x08
    CYFIXEDFRAME = 8,  // 0x08
    CYVTHUMB = 9,  // 0x09
    CXHTHUMB = 10, // 0x0A
    CXICON = 11, // 0x0B
    CYICON = 12, // 0x0C
    CXCURSOR = 13, // 0x0D
    CYCURSOR = 14, // 0x0E
    CYMENU = 15, // 0x0F
    CXFULLSCREEN = 16, // 0x10
    CYFULLSCREEN = 17, // 0x11
    CYKANJIWINDOW = 18, // 0x12
    MOUSEPRESENT = 19, // 0x13
    CYVSCROLL = 20, // 0x14
    CXHSCROLL = 21, // 0x15
    DEBUG = 22, // 0x16
    SWAPBUTTON = 23, // 0x17
    CXMIN = 28, // 0x1C
    CYMIN = 29, // 0x1D
    CXSIZE = 30, // 0x1E
    CYSIZE = 31, // 0x1F
    CXSIZEFRAME = 32, // 0x20
    CXFRAME = 32, // 0x20
    CYSIZEFRAME = 33, // 0x21
    CYFRAME = 33, // 0x21
    CXMINTRACK = 34, // 0x22
    CYMINTRACK = 35, // 0x23
    CXDOUBLECLK = 36, // 0x24
    CYDOUBLECLK = 37, // 0x25
    CXICONSPACING = 38, // 0x26
    CYICONSPACING = 39, // 0x27
    MENUDROPALIGNMENT = 40, // 0x28
    PENWINDOWS = 41, // 0x29
    DBCSENABLED = 42, // 0x2A
    CMOUSEBUTTONS = 43, // 0x2B
    SECURE = 44, // 0x2C
    CXEDGE = 45, // 0x2D
    CYEDGE = 46, // 0x2E
    CXMINSPACING = 47, // 0x2F
    CYMINSPACING = 48, // 0x30
    CXSMICON = 49, // 0x31
    CYSMICON = 50, // 0x32
    CYSMCAPTION = 51, // 0x33
    CXSMSIZE = 52, // 0x34
    CYSMSIZE = 53, // 0x35
    CXMENUSIZE = 54, // 0x36
    CYMENUSIZE = 55, // 0x37
    ARRANGE = 56, // 0x38
    CXMINIMIZED = 57, // 0x39
    CYMINIMIZED = 58, // 0x3A
    CXMAXTRACK = 59, // 0x3B
    CYMAXTRACK = 60, // 0x3C
    CXMAXIMIZED = 61, // 0x3D
    CYMAXIMIZED = 62, // 0x3E
    NETWORK = 63, // 0x3F
    CLEANBOOT = 67, // 0x43
    CXDRAG = 68, // 0x44
    CYDRAG = 69, // 0x45
    SHOWSOUNDS = 70, // 0x46
    CXMENUCHECK = 71, // 0x47
    CYMENUCHECK = 72, // 0x48
    SLOWMACHINE = 73, // 0x49
    MIDEASTENABLED = 74, // 0x4A
    MOUSEWHEELPRESENT = 75, // 0x4B
    XVIRTUALSCREEN = 76, // 0x4C
    YVIRTUALSCREEN = 77, // 0x4D
    CXVIRTUALSCREEN = 78, // 0x4E
    CYVIRTUALSCREEN = 79, // 0x4F
    CMONITORS = 80, // 0x50
    SAMEDISPLAYFORMAT = 81, // 0x51
    IMMENABLED = 82, // 0x52
    CXFOCUSBORDER = 83, // 0x53
    CYFOCUSBORDER = 84, // 0x54
    TABLETPC = 86, // 0x56
    MEDIACENTER = 87, // 0x57
    STARTER = 88, // 0x58
    SERVERR2 = 89, // 0x59
    MOUSEHORIZONTALWHEELPRESENT = 91, // 0x5B
    CXPADDEDBORDER = 92, // 0x5C
    DIGITIZER = 94, // 0x5E
    MAXIMUMTOUCHES = 95, // 0x5F

    REMOTESESSION = 0x1000, // 0x1000
    SHUTTINGDOWN = 0x2000, // 0x2000
    REMOTECONTROL = 0x2001, // 0x2001

    CONVERTABLESLATEMODE = 0x2003,
    SYSTEMDOCKED = 0x2004,
}