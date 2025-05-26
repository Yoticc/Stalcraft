global using InterceptionContext = nint;
global using InterceptionDevice = int;
global using KeyList = System.Collections.Generic.List<Keys>;
using Microsoft.VisualBasic.Devices;
using static InterceptionInterop;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
static class Interception
{
    static bool isLeftMouseDown, isRightMouseDown;
    static int keyboardDeviceID, mouseDeviceID;
    static nint keyboard, mouse;
    static public KeyList DownedKeys = new KeyList(256);
    static public int LastMouseX, LastMouseY;

    static Thread driverupdaterkeyboard, driverupdatermouse;

    public static int MouseX => LastMouseX;
    public static int MouseY => LastMouseY;

    public static bool IsLeftMouseDown => isLeftMouseDown;
    public static bool IsRightMouseDown => isRightMouseDown;

    static Interception()
    {
#if DEBUG
        return;
#endif

        keyboard = CreateContext();
        SetFilter(keyboard, IsKeyboard, Filter.All);

        mouse = CreateContext();
        SetFilter(mouse, IsMouse, Filter.All);

        (driverupdaterkeyboard = new(DriverKeyboardUpdater)
        {
            Priority = ThreadPriority.Highest
        }).Start();

        (driverupdatermouse = new(DriverMouseUpdaterBootstrapper)
        {
            Priority = ThreadPriority.Highest
        }).Start();
    }

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
                        switch (!DownedKeys.Contains(key))
                        {
                            case true:
                                DownedKeys.Add(key);
                                processed = InternalOnKeyDown(key, false);
                                break;
                            case false:
                                processed = InternalOnKeyDown(key, true);
                                break;
                        }
                    }
                    else
                    {
                        DownedKeys.Remove(key);
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
                    mouseDeviceID = Wait(mouse);

                    if (mouseDeviceID == 0)
                        continue;

                    Receive(mouse, mouseDeviceID, ref stroke, 1);

                    var processed = false;
                    switch (stroke.Mouse.State)
                    {
                        case MouseState.LeftButtonDown:
                            isLeftMouseDown = true;
                            processed = InternalOnKeyDown(Keys.MouseLeft, false);
                            break;
                        case MouseState.RightButtonDown:
                            isRightMouseDown = true;
                            processed = InternalOnKeyDown(Keys.MouseRight, false);
                            break;
                        case MouseState.MiddleButtonDown:
                            processed = InternalOnKeyDown(Keys.MouseMiddle, false);
                            break;
                        case MouseState.Button4Down:
                            processed = InternalOnKeyDown(Keys.Button1, false);
                            break;
                        case MouseState.Button5Down:
                            processed = InternalOnKeyDown(Keys.Button2, false);
                            break;
                        case MouseState.LeftButtonUp:
                            isLeftMouseDown = false;
                            processed = InternalOnKeyUp(Keys.MouseLeft);
                            break;
                        case MouseState.RightButtonUp:
                            isRightMouseDown = false;
                            processed = InternalOnKeyUp(Keys.MouseRight);
                            break;
                        case MouseState.MiddleButtonUp:
                            processed = InternalOnKeyUp(Keys.MouseMiddle);
                            break;
                        case MouseState.Button4Up:
                            processed = InternalOnKeyUp(Keys.Button1);
                            break;
                        case MouseState.Button5Up:
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
    public static void KeyClick(Keys key, int delay)
    {
        KeyDown(key);
        Thread.Sleep(delay);
        KeyUp(key);
    }

    public static bool IsKeyDown(int deviceID, Keys key) => DownedKeys.Contains(key);

    public static bool IsKeyDown(Keys key) => IsKeyDown(keyboardDeviceID, key);

    public static bool IsKeyUp(int deviceID, Keys key) => !IsKeyDown(deviceID, key);

    public static bool IsKeyUp(Keys key) => IsKeyUp(keyboardDeviceID, key);

    public static void KeyDown(int deviceID, params Keys[] keys)
    {
        foreach (var key in keys)
        {
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
                stroke.Key = ToKeyStroke(key, true);
                Send(keyboard, deviceID, ref stroke, 1);
            }
        }
    }

    public static void KeyDown(params Keys[] keys) => KeyDown(keyboardDeviceID, keys);

    public static void KeyUp(int deviceID, params Keys[] keys)
    {
        foreach (var key in keys)
        {
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
                stroke.Key = ToKeyStroke(key, false);
                Send(keyboard, deviceID, ref stroke, 1);
            }
        }
    }

    public static void KeyUp(params Keys[] keys) => KeyUp(keyboardDeviceID, keys);

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