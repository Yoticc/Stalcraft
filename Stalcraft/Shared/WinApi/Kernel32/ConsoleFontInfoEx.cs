using System.Drawing;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
unsafe struct ConsoleFontInfoEx
{
    public int Size;
    public uint Font;
    public Point FontSize;
    public ushort FontFamily;
    public ushort FontWeight;
    public fixed char FaceName[32];
}