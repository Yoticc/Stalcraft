unsafe static class GuiDetermination
{
    static (int X, int Y)[] radarBorderPixelCoords = [(1666, 207), (1676, 56), (1717, 241)];
    static Pixel radarBorderPixel = new(0x21, 0x21, 0x21, 0xFF);
    public static bool HasOverlay(MemoryBitmap bitmap)
    {
        foreach (var coord in radarBorderPixelCoords)
            if (bitmap[coord.X, coord.Y] != radarBorderPixel)
                return false;

        return true;
    }

    static (int X, int Y) crateSplitterCoords = (359, 173);
    static Pixel crateSplitterPixel = new(0xB1, 0xB1, 0xB1, 0xFF);
    public static bool IsInCrateGui(MemoryBitmap bitmap)
    {
        for (var x = crateSplitterCoords.X; x < crateSplitterCoords.X + 10; x++)
            if (bitmap[x, crateSplitterCoords.Y] != crateSplitterPixel)
                return false;
        return true;
    }
}