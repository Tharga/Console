using System;
using System.Threading.Tasks;

namespace Tharga.Console.Commands;

internal class ExitCommand : ActionCommandBase
{
    //private readonly Action _stopAction;

    //TODO: Provide an interface with a service that can terminate the application
    public ExitCommand()
        : base("exit", "Exit from the console.")
    {
        //_stopAction = stopAction;
    }

    //public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("This command terminates the application."); } }

    public override async Task Invoke(string[] param)
    {
        Output("Exiting...");
        Environment.Exit(0);
        //_stopAction();
    }
}

//public class ExitCommand : ActionCommandBase
//{
//    public ExitCommand()
//        : base("Exit")
//    {
//    }

//    public override Task Invoke(string[] param)
//    {
//        //TODO: Make the application exit.
//        throw new System.NotImplementedException();
//    }
//}