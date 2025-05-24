using ScreenCapture;

static unsafe class StalcraftWindowCapture
{
    static StalcraftWindowCapture() => new Thread(HandlerThreadBody).Start();

    static void HandlerThreadBody()
    {
        using var device = new GraphicDevice();
        using var screen = new Screen(device);

        while (true)
        {
            if (!StalcraftWindow.IsActive)
            {
                Thread.Sleep(15);
                continue;
            }

            if (!HackManager.ShouldCaptureFrame())
            {
                Thread.Sleep(25);
                continue;
            }

            if (!screen.CaptureFrame(out Frame frame))
            {
                Thread.Sleep(20);
                continue;
            }

            var frameBitmap = frame.Bitmap;
            var memoryBitmap = new MemoryBitmap(frameBitmap.Pixels, frameBitmap.Width, frameBitmap.Height);
            var frameState = DeterminateFrameState();
            HackManager.PushFrame(frame, frameState, memoryBitmap);

            FrameState DeterminateFrameState()
            {
                if (GuiDetermination.HasOverlay(memoryBitmap))
                    return FrameState.HasOverlay;
                if (GuiDetermination.IsInCrateGui(memoryBitmap))
                    return FrameState.InCrateGui;
                return FrameState.None;
            }
        }
    }
}