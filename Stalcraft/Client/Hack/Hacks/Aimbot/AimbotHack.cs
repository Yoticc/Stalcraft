using ScreenCapture;

unsafe class AimbotHack : Hack
{
    static AimbotSettings* settings = Config->Settings->Aimbot;
    public AimbotHack() : base("aimbot") 
    {
        SettingsPanel = new AimbotSettingsPanel();
    }

    int* fov = settings->Fov;   
    public int Fov { get => *fov; set => *fov = value; }

    int* offset = settings->Offset;
    public int Offset { get => *offset; set => *offset = value; }

    bool skipNextFrame;
    private protected override void OnCaptureFrame(Frame frame, FrameState frameState, MemoryBitmap bitmap)
    {
        var frameBitmap = frame.Bitmap;
        var width = frameBitmap.Width;
        var height = frameBitmap.Height;

        if (frameState != FrameState.HasOverlay)
            return;

        if (!Interception.IsRightMouseDown)
            return;

        var fov = 250;
        var centerBitmap = bitmap.Slice(
            width / 2 - fov - Aimbot.TAB_WIDTH_PADDING,
            height / 2 - fov - Aimbot.TAB_HEIGHT_PADDING,
            (fov + Aimbot.TAB_WIDTH_PADDING) * 2,
            (fov + Aimbot.TAB_HEIGHT_PADDING) * 2
        );
        var detection = Aimbot.DetectTab(centerBitmap, Offset);

        if (detection is not null)
        {
            var diffX = detection.ScreenX - (width / 2);
            var diffY = detection.ScreenY - (height / 2);

            Interception.MouseMove(diffX, diffY);
            skipNextFrame = true;
        }
    }

    public override bool ShouldCaptureFrame()
    {
        if (skipNextFrame)
        {
            skipNextFrame = false;
            return false;
        }

        return Interception.IsRightMouseDown;
    }
}