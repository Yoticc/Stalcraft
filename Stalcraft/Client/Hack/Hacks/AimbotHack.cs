class AimbotHack : Hack
{
    public const int TAB_WIDTH_PADDING = 82;
    public const int TAB_HEIGHT_PADDING = 22;
    public const int TAB_POINTER_LENGTH = 5;

    public AimbotHack() : base("aimbot", Keys.Y) { }

    public static AimbotTab? DetectTab(SlicedMemoryBitmap bitmap)
    {
        var width = bitmap.ParentWidth;
        var height = bitmap.ParentHeight;
        var centerX = width / 2;
        var centerY = height / 2;

        var tabs = DetectTabs(bitmap);

        if (tabs.Count == 0)
            return null;

        var bestTabIndex = 0;
        var bestDistance = 10000d;
        for (var i = 1; i < tabs.Count; i++)
        {
            var distance = GetDistanceToTab(tabs[i]);
            if (bestDistance > distance)
            {
                bestTabIndex = i;
                bestDistance = distance;
            }
        }

        return tabs[bestTabIndex];

        double GetDistanceToTab(AimbotTab tab)
        {
            var x = bitmap.X + tab.ScreenX;
            var y = bitmap.Y + tab.ScreenY;

            return Math.Sqrt(Math.Pow(Math.Abs(centerX - x), 2) + Math.Pow(Math.Abs(centerY - y), 2));
        }
    }

    static List<AimbotTab> DetectTabs(SlicedMemoryBitmap bitmap)
    {
        List<AimbotTab> tabs = [];

        var ys = bitmap.Height - TAB_HEIGHT_PADDING - 1;
        var ye = TAB_HEIGHT_PADDING;
        var xs = TAB_WIDTH_PADDING;
        var xe = bitmap.Width - TAB_WIDTH_PADDING;

        for (var y = ys; y >= ye; y--)
            for (var x = xs; x < xe; x++)
            {
                if (IsPixelRed(x, y))
                {
                    if (ValidateTab(x, y))
                    {
                        var tab = new AimbotTab(bitmap.X + x, bitmap.Y + y + 24); // 18/28
                        tabs.Add(tab);
                    }
                    y -= TAB_POINTER_LENGTH;
                }
            }

        return tabs;

        bool ValidateTab(int tx, int ty)
        {
            FixNoFirstPixel();
            return CheckPerfect() || CheckDoubleStart();

            void FixNoFirstPixel()
            {
                if (IsPixelRed(tx + 1, ty) && IsPixelRed(tx + 2, ty))
                {
                    var a = bitmap[tx, ty];

                    ty += 1;
                    tx += 1;
                }
            }

            bool CheckPerfect()
            {
                for (var t = 1; t < TAB_POINTER_LENGTH; t++)
                    if (!IsPixelRed(tx + t, ty - t) ||
                        !IsPixelRed(tx - t, ty - t) ||
                         IsPixelRed(tx - t - 1, ty - t) ||
                         IsPixelRed(tx + t + 1, ty - t) ||
                        !IsPixelRed(tx - t, ty - t - 1) ||
                        !IsPixelRed(tx + t, ty - t - 1))
                        return false;

                return true;
            }

            bool CheckDoubleStart()
            {
                for (var t = 0; t < TAB_POINTER_LENGTH; t++)
                    if (!IsPixelRed(tx + t + 1, ty - t) ||
                        !IsPixelRed(tx - t, ty - t) ||
                         IsPixelRed(tx - t - 1, ty - t) ||
                         IsPixelRed(tx + t + 2, ty - t) ||
                        !IsPixelRed(tx - t, ty - t - 1) ||
                        !IsPixelRed(tx + t + 1, ty - t - 1))
                        return false;

                return true;
            }
        }

        bool IsPixelRed(int x, int y) => IsColorRed(bitmap[x, y]);
        bool IsColorRed(Pixel pixel) => pixel.R >= 150 && pixel.G <= 80 && pixel.B <= 80;
    }
}

record struct AimbotTab(int ScreenX, int ScreenY);