using System.Drawing;
using System.Runtime.InteropServices;

unsafe class MemoryBitmap
{
    public MemoryBitmap(void* pixels, int width, int height)
    {
        Pixels = (Pixel*)pixels;
        (Width, Height) = (width, height);
    }

    public readonly Pixel* Pixels;
    public readonly int Width, Height;

    public Pixel this[int x, int y] => Pixels[y * Width + x];

    public Pixel* GetPixelPointer(int x, int y) => Pixels + (y * Width + x);

    public SlicedMemoryBitmap Slice() => new(this, 0, 0, Width, Height);
    public SlicedMemoryBitmap Slice(int x, int y, int width, int height)
        => new(this, x, y, width, height);

    public Bitmap GetGDIBitmap()
    {
        var bitmap = new Bitmap(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var pixel = this[x, y];
                bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.R, pixel.G, pixel.B));
            }

        return bitmap;
    }

    public void Save(string path)
    {
        using var bitmap = GetGDIBitmap();
        bitmap.Save(path);
    }

    public void DebugSave() => Save(@"C:\a.png");

    public static MemoryBitmap LoadFromFile(string path)
    {
        using var bitmap = (Bitmap)Image.FromFile(path);
        return LoadFromGDIBitmap(bitmap);
    }

    public static MemoryBitmap LoadFromGDIBitmap(Bitmap gdiBitmap)
    {
        var bitmap = new ManagedMemoryBitmap(gdiBitmap.Width, gdiBitmap.Height);
        for (var y = 0; y < gdiBitmap.Height; y++)
            for (var x = 0; x < gdiBitmap.Width; x++)
                *bitmap.GetPixelPointer(x, y) = gdiBitmap.GetPixel(x, y);

        return bitmap;
    }
}

unsafe class SlicedMemoryBitmap
{
    public SlicedMemoryBitmap(MemoryBitmap parent, int x, int y, int width, int height)
    {
        Parent = parent;
        Pixels = Parent.Pixels;
        (ParentWidth, ParentHeight) = (Parent.Width, Parent.Height);
        (X, Y) = (x, y);
        (Width, Height) = (width, height);
    }

    public readonly MemoryBitmap Parent;
    public readonly Pixel* Pixels;
    public readonly int ParentWidth, ParentHeight;
    public readonly int X, Y;
    public readonly int Width, Height;

    public Pixel this[int x, int y]
    {
        get => Pixels[(Y + y) * ParentWidth + X + x];
        set => Pixels[(Y + y) * ParentWidth + X + x] = value;
    }

    public Bitmap GetGDIBitmap()
    {
        var bitmap = new Bitmap(Width, Height);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                //Console.Write($"{x} {y}", 0, 0);

                var pixel = this[x, y];
                bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.R, pixel.G, pixel.B));
            }

        return bitmap;
    }

    public void Save(string path)
    {
        using var bitmap = GetGDIBitmap();
        bitmap.Save(path);
    }

    public void DebugSave() => Save(@"C:\a.png");
}

unsafe class ManagedMemoryBitmap : MemoryBitmap, IDisposable
{
    public ManagedMemoryBitmap(int width, int height) : base(Allocate(width * height * 4), width, height) { }

    static void* Allocate(int size) => (void*)Marshal.AllocCoTaskMem(size);

    bool disposed;
    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;
        GC.SuppressFinalize(this);

        Marshal.FreeCoTaskMem((nint)Pixels);
    }

    ~ManagedMemoryBitmap() => Dispose();
}