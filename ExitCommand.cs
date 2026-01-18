using Tharga.Console.Commands;

public class ExitCommand : ActionCommandBase
{
    public override string Name => "exit";
    public override string ShortDescription => "Exits the application.";

    protected override bool OnCanExecute(out string reasonMessage)
    {
        reasonMessage = "";
        return true;
    }

    protected override void OnExecute()
    {
        // Gracefully stop the application
        Environment.Exit(0);
    }
}
