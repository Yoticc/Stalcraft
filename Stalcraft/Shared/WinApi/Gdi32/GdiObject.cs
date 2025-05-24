struct GdiObject : IDisposable
{
    public readonly nint Value;

    public void Dispose() => Gdi32.DeleteObject(this);
}

struct GdiRegion : IDisposable
{
    public readonly GdiObject Object;

    public static readonly GdiRegion Empty = new GdiRegion();

    public void Dispose() => Gdi32.DeleteRegion(this);
}