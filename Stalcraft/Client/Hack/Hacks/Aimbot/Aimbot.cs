unsafe static class Aimbot
{
    public const int TAB_WIDTH_PADDING = 82;
    public const int TAB_HEIGHT_PADDING = 22;

    public static AimbotTab DetectTab(MemoryBitmap bitmap, int fov, int offset)
    {
        var (width, height) = (fov + TAB_WIDTH_PADDING, fov + TAB_HEIGHT_PADDING);
        (width, height) = (Math.Min(width, bitmap.Width - TAB_WIDTH_PADDING), Math.Min(height, bitmap.Height - TAB_HEIGHT_PADDING));
        var radius = width > height ? height : width;

        var (x, y) = (bitmap.Width / 2 - radius, bitmap.Height / 2 - radius);
        var centerBitmap = bitmap.Slice(x, y, radius * 2, radius * 2);

        AimbotTab tab;
        if (DetectTab(centerBitmap, radius, radius, radius, &tab))
        {
            tab.X += x;
            tab.Y += y;
            tab.Y += offset;
        }
        return tab;
    }

    static bool DetectTab(SlicedMemoryBitmap bitmap, int radius, int x, int y, AimbotTab* tab)
    {
        var radius2 = radius * 2;
        for (var i = 0; i < radius2;)
        {
            for (var i2 = 0; i2 < i; i2++)
                if (CheckPixel(x++, y))
                    return true;

            for (var i2 = 0; i2 < i; i2++)
                if (CheckPixel(x, y--))
                    return true;
            i++;

            for (var i2 = 0; i2 < i; i2++)
                if (CheckPixel(x--, y))
                    return true;

            for (var i2 = 0; i2 < i; i2++)
                if (CheckPixel(x, y++))
                    return true;
            i++;
        }

        return false;

        bool CheckPixel(int x, int y)
        {
            if (IsPixelRed(x, y))
            {
                if (
                    IsPixelRed(x + 1, y - 1) &&
                    IsPixelRed(x + 2, y - 2) &&
                    IsPixelRed(x + 3, y - 3) &&
                    IsPixelRed(x + 4, y - 4) &&
                    IsPixelRed(x - 1, y - 1) &&
                    IsPixelRed(x - 2, y - 2) &&
                    IsPixelRed(x - 3, y - 3) &&
                    IsPixelRed(x - 4, y - 4) &&

                    !IsPixelRed(x + 1, y - 2 - 1) &&    
                    !IsPixelRed(x + 2, y - 2 - 2) &&
                    !IsPixelRed(x + 3, y - 2 - 3) &&
                    !IsPixelRed(x + 4, y - 2 - 4) &&
                    !IsPixelRed(x - 1, y - 2 - 1) &&
                    !IsPixelRed(x - 2, y - 2 - 2) &&
                    !IsPixelRed(x - 3, y - 2 - 3) &&
                    !IsPixelRed(x - 4, y - 2 - 4) &&

                    !IsPixelRed(x + 1, y + 3 - 1) &&
                    !IsPixelRed(x + 2, y + 3 - 2) &&
                    !IsPixelRed(x + 3, y + 3 - 3) &&
                    !IsPixelRed(x + 4, y + 3 - 4) &&
                    !IsPixelRed(x - 1, y + 3 - 1) &&
                    !IsPixelRed(x - 2, y + 3 - 2) &&
                    !IsPixelRed(x - 3, y + 3 - 3) &&
                    !IsPixelRed(x - 4, y + 3 - 4)                    
                )
                {
                    *tab = new AimbotTab() { X = x, Y = y };
                    return true;
                }
            }

            return false;
        }

        bool IsPixelRed(int x, int y) => IsColorRed(bitmap[x, y]);
        bool IsColorRed(Pixel pixel) => pixel.R >= 150 && pixel.G <= 80 && pixel.B <= 80;
    }
}