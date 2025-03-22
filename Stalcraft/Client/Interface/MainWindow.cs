class MainWindow : Window
{
    public MainWindow() : base("Stalcraft client")
    {
        AddControls(
            new BorderedPanel(
                location: new(1, 1),
                size: new(10, 10),
                fillBackground: true,
                borderColor: ConsoleColor.Red,
                borderStyle: BorderStyle.ASCII,
                controls:[
                    new Label(
                        location: new(0, 8),
                        text: "test text"
                    )
                ]
            )
        );
    }
}