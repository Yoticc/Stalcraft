using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 4)]
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
unsafe struct Pixel
{
    public Pixel(byte r, byte g, byte b, byte a)
        => (R, G, B, A) = (r, g, b, a);

    public byte B, G, R, A;

    public static Pixel FromGDIColor(System.Drawing.Color color) => new(color.R, color.G, color.B, color.A);

    public static implicit operator Pixel(System.Drawing.Color color) => FromGDIColor(color);
    public static bool operator ==(Pixel left, Pixel right) => *(int*)&left == *(int*)&right;
    public static bool operator !=(Pixel left, Pixel right) => *(int*)&left != *(int*)&right;
}