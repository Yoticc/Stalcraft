class ClientProgram
{
    public void Main()
    {
        var macro = new InterceptionManager();
        HackManager.SetMacro(macro);

        var app = new ConsoleApplication(macro);
        app.Run<MainWindow>();
    }
}