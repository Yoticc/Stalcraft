struct AimbotTab
{
    public AimbotTab(int x, int y) => (X, Y) = (x, y);

    public int X;
    public int Y;

    public bool IsValid => X != 0 && Y != 0;
}