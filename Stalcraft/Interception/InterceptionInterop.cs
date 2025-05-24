using System.Runtime.InteropServices;

static class InterceptionInterop
{
    [Flags]
    public enum Filter : ushort
    {
        None = FilterKeyState.None,
        All = FilterKeyState.All,

        KDown = FilterKeyState.Down,
        KUp = FilterKeyState.Up,
        KE0 = FilterKeyState.E0,
        KE1 = FilterKeyState.E1,
        KTermSrvSetLED = FilterKeyState.TermSrvSetLED,
        KTermSrvShadow = FilterKeyState.TermSrvShadow,
        KTermSrvVKPacket = FilterKeyState.TermSrvVKPacket,

        MLeftButtonDown = FilterMouseState.LeftButtonDown,
        MLeftButtonUp = FilterMouseState.LeftButtonUp,
        MRightButtonDown = FilterMouseState.RightButtonDown,
        MRightButtonUp = FilterMouseState.RightButtonUp,
        MMiddleButtonDown = FilterMouseState.MiddleButtonDown,
        MMiddleButtonUp = FilterMouseState.MiddleButtonUp,

        MButton1Down = FilterMouseState.Button1Down,
        MButton1Up = FilterMouseState.Button1Up,
        MButton2Down = FilterMouseState.Button2Down,
        MButton2Up = FilterMouseState.Button2Up,
        MButton3Down = FilterMouseState.Button3Down,
        MButton3Up = FilterMouseState.Button3Up,

        MButton4Down = FilterMouseState.Button4Down,
        MButton4Up = FilterMouseState.Button4Up,
        MButton5Down = FilterMouseState.Button5Down,
        MButton5Up = FilterMouseState.Button5Up,

        MWheel = FilterMouseState.Wheel,
        MHWheel = FilterMouseState.HWheel,

        MMove = FilterMouseState.Move
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Predicate(int device);

    [Flags]
    public enum KeyState : ushort
    {
        Down = 0x00,
        Up = 0x01,
        E0 = 0x02,
        E1 = 0x04,
        TermSrvSetLED = 0x08,
        TermSrvShadow = 0x10,
        TermSrvVKPacket = 0x20,
    }

    public static bool IsKeyDown(this KeyState keyState) => (keyState & KeyState.Up) == 0;

    [Flags]
    public enum FilterKeyState : ushort
    {
        None = 0x0000,
        All = 0xFFFF,
        Down = KeyState.Up,
        Up = KeyState.Up << 1,
        E0 = KeyState.E0 << 1,
        E1 = KeyState.E1 << 1,
        TermSrvSetLED = KeyState.TermSrvSetLED << 1,
        TermSrvShadow = KeyState.TermSrvShadow << 1,
        TermSrvVKPacket = KeyState.TermSrvVKPacket << 1
    }

    [Flags]
    public enum MouseState : ushort
    {
        LeftButtonDown = 0x001,
        LeftButtonUp = 0x002,
        RightButtonDown = 0x004,
        RightButtonUp = 0x008,
        MiddleButtonDown = 0x010,
        MiddleButtonUp = 0x020,

        Button1Down = LeftButtonDown,
        Button1Up = LeftButtonUp,
        Button2Down = RightButtonDown,
        Button2Up = RightButtonUp,
        Button3Down = MiddleButtonDown,
        Button3Up = MiddleButtonUp,

        Button4Down = 0x040,
        Button4Up = 0x080,
        Button5Down = 0x100,
        Button5Up = 0x200,

        Wheel = 0x400,
        HWheel = 0x800
    }

    [Flags]
    public enum FilterMouseState : ushort
    {
        None = 0x0000,
        All = 0xFFFF,

        LeftButtonDown = MouseState.LeftButtonDown,
        LeftButtonUp = MouseState.LeftButtonUp,
        RightButtonDown = MouseState.RightButtonDown,
        RightButtonUp = MouseState.RightButtonUp,
        MiddleButtonDown = MouseState.MiddleButtonDown,
        MiddleButtonUp = MouseState.MiddleButtonUp,

        Button1Down = MouseState.Button1Down,
        Button1Up = MouseState.Button1Up,
        Button2Down = MouseState.Button2Down,
        Button2Up = MouseState.Button2Up,
        Button3Down = MouseState.Button3Down,
        Button3Up = MouseState.Button3Up,

        Button4Down = MouseState.Button4Down,
        Button4Up = MouseState.Button4Up,
        Button5Down = MouseState.Button5Down,
        Button5Up = MouseState.Button5Up,

        Wheel = MouseState.Wheel,
        HWheel = MouseState.HWheel,

        Move = 0x1000
    }

    [Flags]
    public enum MouseFlag : ushort
    {
        MoveRelative = 0x000,
        MoveAbsolute = 0x001,
        VirturalDesktop = 0x002,
        AttributesChanged = 0x004,
        MoveNoCoalesce = 0x008,
        TermSrvSrcShadow = 0x100
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseStroke
    {
        public MouseState State;
        public MouseFlag Flags;
        public short Rolling;
        public int X;
        public int Y;
        public uint Information;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke
    {
        public ushort Code;
        public KeyState State;
        public uint Information;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Stroke
    {
        [FieldOffset(0)]
        public MouseStroke Mouse;

        [FieldOffset(0)]
        public KeyStroke Key;
    }

    [DllImport("interception", EntryPoint = "interception_create_context")]
    public static extern InterceptionContext CreateContext();
    [DllImport("interception", EntryPoint = "interception_destroy_context")]
    public static extern void DestroyContext(InterceptionContext context);
    [DllImport("interception", EntryPoint = "interception_get_filter")]
    public static extern Filter GetFilter(InterceptionContext context, InterceptionDevice deviceID);
    [DllImport("interception", EntryPoint = "interception_set_filter")]
    public static extern void SetFilter(InterceptionContext context, Predicate predicate, Filter filter);
    [DllImport("interception", EntryPoint = "interception_wait")]
    public static extern InterceptionDevice Wait(InterceptionContext context);
    [DllImport("interception", EntryPoint = "interception_wait_with_timeout")]
    public static extern InterceptionDevice WaitWithTimeout(InterceptionContext context, ulong milliseconds);
    [DllImport("interception", EntryPoint = "interception_send")]
    public static extern int Send(InterceptionContext context, InterceptionDevice deviceID, ref Stroke stroke, uint nstroke);
    [DllImport("interception", EntryPoint = "interception_receive")]
    public static extern int Receive(InterceptionContext context, InterceptionDevice deviceID, ref Stroke stroke, uint nstroke);
    [DllImport("interception", EntryPoint = "interception_is_keyboard")]
    public static extern int IsKeyboard(InterceptionDevice deviceID);
    [DllImport("interception", EntryPoint = "interception_is_mouse")]
    public static extern int IsMouse(InterceptionDevice deviceID);
}