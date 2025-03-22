class ClientProgram
{
    public void Main()
    {
        const int ConsoleWidth = 90;
        const int ConsoleHeight = 25;

        var app = new ConsoleApplication(ConsoleWidth, ConsoleHeight);
        app.Run<MainWindow>();
    }
}