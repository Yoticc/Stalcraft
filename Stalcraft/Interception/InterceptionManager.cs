global using InterceptionContext = nint;
global using InterceptionDevice = int;
global using KeyList = System.Collections.Generic.List<Keys>;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
class InterceptionManager : IDisposable
{
    bool isLeftMouseDown, isRightMouseDown;
    int keyboardDeviceID = 1, mouseDeviceID = 13;
    nint keyboard, mouse;
    public KeyList DownedKeys = new KeyList(256);
    public int LastMouseX, LastMouseY;

    Thread driverupdaterkeyboard, driverupdatermouse;

    public int MouseX => LastMouseX;
    public int MouseY => LastMouseY;

    public InterceptionManager()
    {
#if DEBUG
        return;
#endif

        keyboard = Interception.CreateContext();
        Interception.SetFilter(keyboard, Interception.IsKeyboard, Interception.Filter.All);

        mouse = Interception.CreateContext();
        Interception.SetFilter(mouse, Interception.IsMouse, Interception.Filter.All);

        (driverupdaterkeyboard = new(DriverKUpdater)
        {
            Priority = ThreadPriority.Highest
        }).Start();

        (driverupdatermouse = new(DriverMUpdater)
        {
            Priority = ThreadPriority.Highest
        }).Start();
    }

    public Keys ToKey(Interception.KeyStroke keyStroke)
    {
        var result = keyStroke.Code;
        if ((keyStroke.State & Interception.KeyState.E0) != 0)
            result += 0x100;
        return (Keys)result;
    }

    public Interception.KeyStroke ToKeyStroke(Keys key, bool down)
    {
        var result = new Interception.KeyStroke();
        if (!down)
            result.State = Interception.KeyState.Up;
        var code = (short)key;
        if (code >= 0x100)
        {
            code -= 0x100;
            result.State |= Interception.KeyState.E0;
        }
        else if (code < 0)
        {
            code += 100;
            result.State |= Interception.KeyState.E0;
        }
        result.Code = (ushort)code;
        return result;
    }

    public void Quit()
    {
        Interception.DestroyContext(keyboard);
        Interception.DestroyContext(mouse);

        try { driverupdaterkeyboard.Interrupt(); } catch { }
        try { driverupdatermouse.Interrupt(); } catch { }
    }

    private void QuitWithException(Exception ex)
    {
        Console.WriteLine($"Thrown exception at IterceptionManager: \n{ex}");

        Quit();
    }

    void DriverKUpdater()
    {
        try
        {
            while (true)
            {
                DriverUpdaterKeyBoard();
                Thread.Sleep(1);
            }

        }
        catch (Exception ex)
        {
            QuitWithException(ex);
        }
    }

    void DriverMUpdater()
    {
        try
        {
            while (true)
            {
                DriverUpdaterMouse();
                Thread.Sleep(1);
            }
        }
        catch (Exception ex)
        {
            QuitWithException(ex);
        }

    }

    public void DriverUpdaterMouse()
    {
        var mousedeviceID = Interception.WaitWithTimeout(mouse, 0);
        if (mousedeviceID == 0)
            return;

        var stroke = new Interception.Stroke();
        while (true)
        {
            try
            {
                while (Interception.Receive(mouse, mousedeviceID, ref stroke, 1) > 0)
                {
                    var processed = false;
                    switch (stroke.Mouse.State)
                    {
                        case Interception.MouseState.LeftButtonDown:
                            isLeftMouseDown = true;
                            processed = InternalOnKeyDown(Keys.MouseLeft, false);
                            break;
                        case Interception.MouseState.RightButtonDown:
                            isRightMouseDown = true;
                            processed = InternalOnKeyDown(Keys.MouseRight, false);
                            break;
                        case Interception.MouseState.MiddleButtonDown:
                            processed = InternalOnKeyDown(Keys.MouseMiddle, false);
                            break;
                        case Interception.MouseState.Button4Down:
                            processed = InternalOnKeyDown(Keys.Button1, false);
                            break;
                        case Interception.MouseState.Button5Down:
                            processed = InternalOnKeyDown(Keys.Button2, false);
                            break;
                        case Interception.MouseState.LeftButtonUp:
                            isLeftMouseDown = false;
                            processed = InternalOnKeyUp(Keys.MouseLeft);
                            break;
                        case Interception.MouseState.RightButtonUp:
                            isRightMouseDown = false;
                            processed = InternalOnKeyUp(Keys.MouseRight);
                            break;
                        case Interception.MouseState.MiddleButtonUp:
                            processed = InternalOnKeyUp(Keys.MouseMiddle);
                            break;
                        case Interception.MouseState.Button4Up:
                            processed = InternalOnKeyUp(Keys.Button1);
                            break;
                        case Interception.MouseState.Button5Up:
                            processed = InternalOnKeyUp(Keys.Button2);
                            break;
                        case Interception.MouseState.Wheel:
                            processed = InternalOnMouseWheel(stroke.Mouse.Rolling);
                            break;
                    }
                    processed = InternalOnMouseMove(stroke.Mouse.X, stroke.Mouse.Y);
                    if (!processed)
                        Interception.Send(mouse, mousedeviceID, ref stroke, 1);
                }
            }
            catch { }
        }
    }

    public void DriverUpdaterKeyBoard()
    {
        var keyboardDeviceID = Interception.WaitWithTimeout(keyboard, 0);
        if (keyboardDeviceID == 0)
            return;

        var stroke = new Interception.Stroke();
        while (true)
        {
            try
            {
                while (Interception.Receive(keyboard, keyboardDeviceID, ref stroke, 1) > 0)
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
                        Interception.Send(keyboard, keyboardDeviceID, ref stroke, 1);
                }
            }
            catch { }
        }
    }

    public void Stop()
    {
        driverupdaterkeyboard.Interrupt();
        driverupdatermouse.Interrupt();
    }

    public delegate bool OnMouseMoveHandler(int x, int y);
    public event OnMouseMoveHandler? OnMouseMove;

    public delegate bool OnMouseWheelHandler(int rolling);
    public event OnMouseWheelHandler? OnMouseWheel;

    public delegate bool OnKeyDownHandler(Keys key, bool repeat);
    public event OnKeyDownHandler? OnKeyDown;

    public delegate bool OnKeyUpHandler(Keys key);
    public event OnKeyUpHandler? OnKeyUp;

    bool InternalOnMouseMove(int x, int y)
    {
        (LastMouseX, LastMouseY) = (x, y);
        if (OnMouseMove != null)
            return OnMouseMove(x, y);
        return false;
    }

    bool InternalOnMouseWheel(int rolling)
    {
        if (OnMouseWheel != null)
            return OnMouseWheel(rolling);
        return false;
    }

    bool InternalOnKeyDown(Keys key, bool repeat)
    {
        if (OnKeyDown != null)
            return OnKeyDown(key, repeat);
        return false;
    }

    bool InternalOnKeyUp(Keys key)
    {
        if (OnKeyUp != null)
            return OnKeyUp(key);
        return false;
    }

    #region Macros
    public void KeyClick(Keys key, int delay)
    {
        KeyDown(key);
        Thread.Sleep(delay);
        KeyUp(key);
    }

    public bool IsLeftMouseDown() => isLeftMouseDown;

    public bool IsRightMouseDown() => isRightMouseDown;

    public bool IsKeyDown(int deviceID, Keys key) => DownedKeys.Contains(key);

    public bool IsKeyDown(Keys key) => IsKeyDown(keyboardDeviceID, key);

    public bool IsKeyUp(int deviceID, Keys key) => !IsKeyDown(deviceID, key);

    public bool IsKeyUp(Keys key) => IsKeyUp(keyboardDeviceID, key);

    public void KeyDown(int deviceID, params Keys[] keys)
    {
        foreach (var key in keys)
        {
            if (((short)key) < 0)
            {
                var stroke = new Interception.Stroke();
                switch (key)
                {
                    case Keys.MouseLeft:
                        stroke.Mouse.State = Interception.MouseState.LeftButtonUp;
                        break;
                    case Keys.MouseRight:
                        stroke.Mouse.State = Interception.MouseState.RightButtonUp;
                        break;
                    case Keys.MouseMiddle:
                        stroke.Mouse.State = Interception.MouseState.MiddleButtonUp;
                        break;
                    case Keys.Button1:
                        stroke.Mouse.State = Interception.MouseState.Button4Up;
                        break;
                    case Keys.Button2:
                        stroke.Mouse.State = Interception.MouseState.Button5Up;
                        break;
                }
                Interception.Send(mouse, mouseDeviceID, ref stroke, 1);
            }
            else
            {
                var stroke = new Interception.Stroke();
                stroke.Key = ToKeyStroke(key, true);
                Interception.Send(keyboard, deviceID, ref stroke, 1);
            }
        }
    }

    public void KeyDown(params Keys[] keys) => KeyDown(keyboardDeviceID, keys);

    public void KeyUp(int deviceID, params Keys[] keys)
    {
        foreach (var key in keys)
        {
            if (((short)key) < 0)
            {
                var stroke = new Interception.Stroke();
                switch (key)
                {
                    case Keys.MouseLeft:
                        stroke.Mouse.State = Interception.MouseState.LeftButtonDown;
                        break;
                    case Keys.MouseRight:
                        stroke.Mouse.State = Interception.MouseState.RightButtonDown;
                        break;
                    case Keys.MouseMiddle:
                        stroke.Mouse.State = Interception.MouseState.MiddleButtonDown;
                        break;
                    case Keys.Button1:
                        stroke.Mouse.State = Interception.MouseState.Button4Down;
                        break;
                    case Keys.Button2:
                        stroke.Mouse.State = Interception.MouseState.Button5Down;
                        break;
                }
                Interception.Send(mouse, mouseDeviceID, ref stroke, 1);
            }
            else
            {
                var stroke = new Interception.Stroke();
                stroke.Key = ToKeyStroke(key, false);
                Interception.Send(keyboard, deviceID, ref stroke, 1);
            }
        }
    }

    public void KeyUp(params Keys[] keys) => KeyUp(keyboardDeviceID, keys);

    public void MouseScroll(int deviceID, short rolling)
    {
        var stroke = new Interception.Stroke();
        stroke.Mouse.State = Interception.MouseState.Wheel;
        stroke.Mouse.Rolling = rolling;
        Interception.Send(mouse, deviceID, ref stroke, 1);
    }

    public void MouseScroll(short rolling) => MouseScroll(mouseDeviceID, rolling);

    public void MouseMove(int deviceID, int x, int y)
    {
        var stroke = new Interception.Stroke();
        stroke.Mouse.X = x;
        stroke.Mouse.Y = y;
        stroke.Mouse.Flags = Interception.MouseFlag.MoveRelative;
        Interception.Send(mouse, deviceID, ref stroke, 1);
    }

    public void MouseSet(int deviceID, int x, int y)
    {
        var stroke = new Interception.Stroke();
        stroke.Mouse.X = x;
        stroke.Mouse.Y = y;
        stroke.Mouse.Flags = Interception.MouseFlag.MoveAbsolute;
        Interception.Send(mouse, deviceID, ref stroke, 1);
    }

    public void MouseMove(int x, int y) => MouseMove(mouseDeviceID, x, y);

    public void MouseSet(int x, int y) => MouseSet(mouseDeviceID, x, y);

    bool disposed;
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;

        Stop();
    }
    ~InterceptionManager() => Dispose();
    #endregion
}