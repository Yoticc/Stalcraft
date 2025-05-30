using Interception;
using System.Windows.Forms.Design;

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

            if (InterceptionImpl.IsLeftMouseDown && InterceptionImpl.IsRightMouseDown)
            {
                InterceptionImpl.MoveMouse(0, 4);

                Thread.Sleep(4);
            }
        }
    }
}