using Interception;
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
        var centerX = width / 2;
        var centerY = height / 2;

        if (frameState != FrameState.HasOverlay)
            return;

        if (!InterceptionImpl.IsRightMouseDown)
            return;

        var detection = Aimbot.DetectTab(bitmap, Fov, Offset);

        if (detection.IsValid)
        {
            var diffX = detection.X - centerX;
            var diffY = detection.Y - centerY;

            InterceptionImpl.MoveMouse((int)(diffX * 1.5f), (int)(diffY * 1.5));
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

        return InterceptionImpl.IsRightMouseDown;
    }
}