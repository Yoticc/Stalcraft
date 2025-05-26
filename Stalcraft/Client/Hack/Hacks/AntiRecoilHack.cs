class AntiRecoilHack : Hack
{
    public AntiRecoilHack() : base("anti recoil") => new Thread(ThreadBody).Start();

    void ThreadBody()
    {
        while (true)
        {
            if (!IsEnabled)
            {
                Thread.Sleep(100);
                continue;
            }

            if (!StalcraftWindow.IsActive)
            {
                Thread.Sleep(75);
                continue;
            }

            if (Interception.IsLeftMouseDown && Interception.IsRightMouseDown)
            {
                Interception.MouseMove(0, 4);
            }
        }
    }
}