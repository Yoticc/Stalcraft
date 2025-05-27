global using InterceptionContext = nint;
global using InterceptionDevice = int;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using static InterceptionInterop;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
static unsafe class Interception
{
    static bool isLeftMouseDown, isRightMouseDown;
    static int keyboardDeviceID, mouseDeviceID;
    static nint keyboard, mouse;
    static bool[] downedKeysArray = new bool[1024];
    static bool* downedKeysPointer;
    static public int LastMouseX, LastMouseY;

    public static int MouseX => LastMouseX;
    public static int MouseY => LastMouseY;

    public static bool IsLeftMouseDown => isLeftMouseDown;
    public static bool IsRightMouseDown => isRightMouseDown;

    static Interception()
    {
        fixed (bool* pointer = downedKeysArray)
            downedKeysPointer = pointer + 512;

#if DEBUG
        return;
#endif

        keyboard = CreateContext();
        SetFilter(keyboard, IsKeyboard, Filter.All);

        mouse = CreateContext();
        SetFilter(mouse, IsMouse, Filter.All);

        new Thread(DriverKeyboardUpdater)
        {
            Priority = ThreadPriority.Highest
        }.Start();

        new Thread(DriverMouseUpdaterBootstrapper)
        {
            Priority = ThreadPriority.Highest
        }.Start();
    }

    static void SetKeyIsDown(Keys key) => downedKeysPointer[(int)key] = true;

    static void SetKeyIsUp(Keys key) => downedKeysPointer[(int)key] = false;

    static Keys ToKey(KeyStroke keyStroke)
    {
        var result = keyStroke.Code;
        if ((keyStroke.State & KeyState.E0) != 0)
            result += 0x100;
        return (Keys)result;
    }

    static KeyStroke ToKeyStroke(Keys key, bool down)
    {
        var result = new KeyStroke();
        if (!down)
            result.State = KeyState.Up;
        var code = (short)key;
        if (code >= 0x100)
        {
            code -= 0x100;
            result.State |= KeyState.E0;
        }
        else if (code < 0)
        {
            code += 100;
            result.State |= KeyState.E0;
        }
        result.Code = (ushort)code;
        return result;
    }

    static void DriverKeyboardUpdater()
    {
        var stroke = new Stroke();
        while (true)
        {
            try
            {
                while (Receive(keyboard, keyboardDeviceID = Wait(keyboard), ref stroke, 1) > 0)
                {
                    var key = ToKey(stroke.Key);
                    var processed = false;
                    if (stroke.Key.State.IsKeyDown())
                    {
                        switch (IsKeyUp(key))
                        {
                            case true:
                                SetKeyIsDown(key);
                                processed = InternalOnKeyDown(key, false);
                                break;
                            case false:
                                processed = InternalOnKeyDown(key, true);
                                break;
                        }
                    }
                    else
                    {
                        SetKeyIsUp(key);
                        processed = InternalOnKeyUp(key);
                    }

                    if (!processed)
                        Send(keyboard, keyboardDeviceID, ref stroke, 1);
                }
            }
            catch { }
        }
    }

    static void DriverMouseUpdaterBootstrapper()
    {
        var stroke = new Stroke();
        while (true)
        {
            try
            {
                while (true)
                {
                    Receive(mouse, mouseDeviceID = Wait(mouse), ref stroke, 1);

                    var processed = false;
                    switch (stroke.Mouse.State)
                    {
                        case MouseState.LeftButtonDown:
                            isLeftMouseDown = true;
                            SetKeyIsDown(Keys.MouseLeft);
                            processed = InternalOnKeyDown(Keys.MouseLeft, false);
                            break;
                        case MouseState.RightButtonDown:
                            isRightMouseDown = true;
                            SetKeyIsDown(Keys.MouseRight);
                            processed = InternalOnKeyDown(Keys.MouseRight, false);
                            break;
                        case MouseState.MiddleButtonDown:
                            SetKeyIsDown(Keys.MouseMiddle);
                            processed = InternalOnKeyDown(Keys.MouseMiddle, false);
                            break;
                        case MouseState.Button4Down:
                            SetKeyIsDown(Keys.Button1);
                            processed = InternalOnKeyDown(Keys.Button1, false);
                            break;
                        case MouseState.Button5Down:
                            SetKeyIsDown(Keys.Button2);
                            processed = InternalOnKeyDown(Keys.Button2, false);
                            break;
                        case MouseState.LeftButtonUp:
                            isLeftMouseDown = false;
                            SetKeyIsUp(Keys.MouseLeft);
                            processed = InternalOnKeyUp(Keys.MouseLeft);
                            break;
                        case MouseState.RightButtonUp:
                            isRightMouseDown = false;
                            SetKeyIsUp(Keys.MouseRight);
                            processed = InternalOnKeyUp(Keys.MouseRight);
                            break;
                        case MouseState.MiddleButtonUp:
                            SetKeyIsUp(Keys.MouseMiddle);
                            processed = InternalOnKeyUp(Keys.MouseMiddle);
                            break;
                        case MouseState.Button4Up:
                            SetKeyIsUp(Keys.Button1);
                            processed = InternalOnKeyUp(Keys.Button1);
                            break;
                        case MouseState.Button5Up:
                            SetKeyIsUp(Keys.Button2);
                            processed = InternalOnKeyUp(Keys.Button2);
                            break;
                        case MouseState.Wheel:
                            processed = InternalOnMouseWheel(stroke.Mouse.Rolling);
                            break;
                        default:
                            processed = InternalOnMouseMove(stroke.Mouse.X, stroke.Mouse.Y);
                            break;
                    }

                    if (!processed)
                        Send(mouse, mouseDeviceID, ref stroke, 1);
                }
            }
            catch { }
        }
    }

    public delegate bool OnMouseMoveDelegate(int x, int y);
    public static OnMouseMoveDelegate? OnMouseMove;

    public delegate bool OnMouseWheelDelegate(int rolling);
    public static OnMouseWheelDelegate? OnMouseWheel;

    public delegate bool OnKeyDownDelegate(Keys key, bool repeat);
    public static OnKeyDownDelegate? OnKeyDown;

    public delegate bool OnKeyUpDelegate(Keys key);
    public static OnKeyUpDelegate? OnKeyUp;

    static bool InternalOnMouseMove(int x, int y)
    {
        (LastMouseX, LastMouseY) = (x, y);
        if (OnMouseMove != null)
            if (x != 0 || y != 0)
                return OnMouseMove(x, y);
        return false;
    }

    static bool InternalOnMouseWheel(int rolling)
    {
        if (OnMouseWheel != null)
            return OnMouseWheel(rolling);
        return false;
    }

    static bool InternalOnKeyDown(Keys key, bool repeat)
    {
        if (OnKeyDown != null)
            return OnKeyDown(key, repeat);
        return false;
    }

    static bool InternalOnKeyUp(Keys key)
    {
        if (OnKeyUp != null)
            return OnKeyUp(key);
        return false;
    }

    #region Macros
    public static bool IsKeyDown(Keys key) => downedKeysPointer[(int)key];

    public static bool IsKeyUp(Keys key) => !IsKeyDown(key);

    public static void KeyUp(params Keys[] keys)
    {
        foreach (var key in keys)
        {
            SetKeyIsUp(key);
            if (((short)key) < 0)
            {
                var stroke = new Stroke();
                switch (key)
                {
                    case Keys.MouseLeft:
                        stroke.Mouse.State = MouseState.LeftButtonUp;
                        break;
                    case Keys.MouseRight:
                        stroke.Mouse.State = MouseState.RightButtonUp;
                        break;
                    case Keys.MouseMiddle:
                        stroke.Mouse.State = MouseState.MiddleButtonUp;
                        break;
                    case Keys.Button1:
                        stroke.Mouse.State = MouseState.Button4Up;
                        break;
                    case Keys.Button2:
                        stroke.Mouse.State = MouseState.Button5Up;
                        break;
                }
                Send(mouse, mouseDeviceID, ref stroke, 1);
            }
            else
            {
                var stroke = new Stroke();
                stroke.Key = ToKeyStroke(key, false);
                Send(keyboard, keyboardDeviceID, ref stroke, 1);
            }
        }
    }

    public static void KeyDown(params Keys[] keys)
    {
        foreach (var key in keys)
        {
            SetKeyIsDown(key);

            if (((short)key) < 0)
            {
                var stroke = new Stroke();
                switch (key)
                {
                    case Keys.MouseLeft:
                        stroke.Mouse.State = MouseState.LeftButtonDown;
                        break;
                    case Keys.MouseRight:
                        stroke.Mouse.State = MouseState.RightButtonDown;
                        break;
                    case Keys.MouseMiddle:
                        stroke.Mouse.State = MouseState.MiddleButtonDown;
                        break;
                    case Keys.Button1:
                        stroke.Mouse.State = MouseState.Button4Down;
                        break;
                    case Keys.Button2:
                        stroke.Mouse.State = MouseState.Button5Down;
                        break;
                }
                Send(mouse, mouseDeviceID, ref stroke, 1);
            }
            else
            {
                var stroke = new Stroke();
                stroke.Key = ToKeyStroke(key, true);
                Send(keyboard, keyboardDeviceID, ref stroke, 1);
            }
        }
    }

    public static void MouseScroll(int deviceID, short rolling)
    {
        var stroke = new Stroke();
        stroke.Mouse.State = MouseState.Wheel;
        stroke.Mouse.Rolling = rolling;
        Send(mouse, deviceID, ref stroke, 1);
    }

    public static void MouseScroll(short rolling) => MouseScroll(mouseDeviceID, rolling);

    public static void MouseMove(int deviceID, int x, int y)
    {
        var stroke = new Stroke();
        stroke.Mouse.X = x;
        stroke.Mouse.Y = y;
        stroke.Mouse.Flags = MouseFlag.MoveRelative;
        Send(mouse, deviceID, ref stroke, 1);
    }

    public static void MouseSet(int deviceID, int x, int y)
    {
        var stroke = new Stroke();
        stroke.Mouse.X = x;
        stroke.Mouse.Y = y;
        stroke.Mouse.Flags = MouseFlag.MoveAbsolute;
        Send(mouse, deviceID, ref stroke, 1);
    }

    public static void MouseMove(int x, int y) => MouseMove(mouseDeviceID, x, y);

    public static void MouseSet(int x, int y) => MouseSet(mouseDeviceID, x, y);
    #endregion
}