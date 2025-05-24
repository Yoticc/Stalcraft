using System.Drawing;
using System.Runtime.InteropServices;

static unsafe class Gdi32
{
    const string gdi = "gdi32";

    [DllImport(gdi)] public static extern nint CreateRectRgn(int left, int top, int right, int bottom);
    [DllImport(gdi)] public static extern int CombineRgn(nint handleRegionDestination, nint handleRegionSource1, nint handleRegionSource2, int combineMode);
    [DllImport(gdi)] public static extern bool DeleteObject(nint objectHandle);

    public static GdiRegion CreateRectangleRegion(int left, int top, int right, int bottom)
    {
        var rectangle = CreateRectRgn(left, top, right, bottom);
        return *(GdiRegion*)&rectangle;
    }

    public static GdiRegion CreateRegion(Rectangle rectangle) => CreateRectangleRegion(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

    public static void CombineRegions(GdiRegion handleRegionDestination, GdiRegion handleRegionSource1, GdiRegion handleRegionSource2, CombineRegionsStyles combineMode)
        => CombineRgn(*(nint*)&handleRegionDestination, *(nint*)&handleRegionSource1, *(nint*)&handleRegionSource2, (int)combineMode);

    public static void DeleteObject(GdiObject gdiObject) => DeleteObject(*(nint*)&gdiObject);
    public static void DeleteRegion(GdiRegion gdiRegion) => DeleteObject(*(nint*)&gdiRegion);
}