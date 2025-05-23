using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
struct ConsoleTextStyles
{
    const byte 
        NoneConst = 0,
        InverseConst = 7,
        BoldConst = 1,
        ItalicConst = 3,
        UnderlineConst = 4,
        StrikethroughConst = 9,
        AwaitingConst = 5;

    ConsoleTextStyles(long value) => this.value = value;

    public ConsoleTextStyles(
        ConsoleForegroundColor foregroundColor = ConsoleForegroundColor.None,
        ConsoleBackgroundColor backgroundColor = ConsoleBackgroundColor.None,
        bool isInversed = false,
        bool isBold = false,
        bool isItalic = false,
        bool isUnderlined = false,
        bool isStruckthrough = false,
        bool isAwaiting = false
    ) => (ForegroundColor, BackgroundColor, IsInversed, IsBold, IsItalic, IsUnderlined, IsStruckthrough, IsAwaiting) = 
         (foregroundColor, backgroundColor, isInversed, isBold, isItalic, isUnderlined, isStruckthrough, isAwaiting);

    [FieldOffset(0x00)] long value;
    [FieldOffset(0x00)] byte foregroundColor;
    [FieldOffset(0x01)] byte backgroundColor;
    [FieldOffset(0x02)] byte inverseValue;
    [FieldOffset(0x03)] byte boldValue;
    [FieldOffset(0x04)] byte italicValue;
    [FieldOffset(0x05)] byte underlineValue;
    [FieldOffset(0x06)] byte strikethroughValue;
    [FieldOffset(0x07)] byte awaitingValue;

    public long Value => value;
    
    public ConsoleForegroundColor ForegroundColor { get => (ConsoleForegroundColor)foregroundColor; set => foregroundColor = (byte)value; }
    public ConsoleBackgroundColor BackgroundColor { get => (ConsoleBackgroundColor)backgroundColor; set => backgroundColor = (byte)value; }

    public bool IsInversed      { get => inverseValue       == InverseConst;       set => inverseValue       = value ? InverseConst       : NoneConst; }
    public bool IsBold          { get => boldValue          == BoldConst;          set => boldValue          = value ? BoldConst          : NoneConst; }
    public bool IsItalic        { get => italicValue        == ItalicConst;        set => italicValue        = value ? ItalicConst        : NoneConst; }
    public bool IsUnderlined    { get => underlineValue     == UnderlineConst;     set => underlineValue     = value ? UnderlineConst     : NoneConst; }
    public bool IsStruckthrough { get => strikethroughValue == StrikethroughConst; set => strikethroughValue = value ? StrikethroughConst : NoneConst; }
    public bool IsAwaiting      { get => awaitingValue      == AwaitingConst;      set => awaitingValue      = value ? AwaitingConst      : NoneConst; }

    public static readonly ConsoleTextStyles 
        Inverse       = new((long)InverseConst       << 0x10),
        Bold          = new((long)BoldConst          << 0x18),
        Italic        = new((long)ItalicConst        << 0x20),
        Underline     = new((long)UnderlineConst     << 0x28),
        Strikethrough = new((long)StrikethroughConst << 0x30),
        Awaiting      = new((long)AwaitingConst      << 0x38);

    public static implicit operator long(ConsoleTextStyles self) => self.value;
    public static implicit operator ConsoleTextStyles(ConsoleBackgroundColor color) => default(ConsoleTextStyles) | color;
    public static implicit operator ConsoleTextStyles(ConsoleForegroundColor color) => default(ConsoleTextStyles) | color;
    public static ConsoleTextStyles operator | (ConsoleBackgroundColor left, ConsoleTextStyles      right) => new((long)left | (long)right);
    public static ConsoleTextStyles operator | (ConsoleForegroundColor left, ConsoleTextStyles      right) => new((long)left | (long)right);
    public static ConsoleTextStyles operator | (ConsoleTextStyles      left, ConsoleBackgroundColor right) => new((long)left | (long)right);
    public static ConsoleTextStyles operator | (ConsoleTextStyles      left, ConsoleForegroundColor right) => new((long)left | (long)right);
    public static ConsoleTextStyles operator | (ConsoleTextStyles      left, ConsoleTextStyles      right) => new((long)left | (long)right);
    public static ConsoleTextStyles operator | (ConsoleTextStyles      left, long                   right) => new((long)left | right);
    public static ConsoleTextStyles operator & (ConsoleTextStyles      left, ConsoleTextStyles      right) => new((long)left & (long)right);
    public static ConsoleTextStyles operator & (ConsoleTextStyles      left, long                   right) => new((long)left & right);
    public static ConsoleTextStyles operator <<(ConsoleTextStyles      left, int                    shift) => new((long)left << shift);
    public static ConsoleTextStyles operator >>(ConsoleTextStyles      left, int                    shift) => new((long)left >> shift);
    public static ConsoleTextStyles operator ~ (ConsoleTextStyles      value                             ) => new(~(long)value);
}