using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tharga.Console.Commands;

//public class HelpCommand : ActionCommand
//{
//    public HelpCommand()
//        : base("Help")
//    {
//    }

//    public override Task Execute()
//    {
//        //Show a tree of all registered commands
//        throw new System.NotImplementedException();
//    }
//}

internal class HelpCommand : ActionCommandBase
{
    private readonly CommandEngine _commandEngine;
    //private readonly List<HelpLine> _helpLines = new List<HelpLine>();

    public HelpCommand() //CommandEngine commandEngine)
        : base("help", "Displays helpt text.")
    {
        //_commandEngine = commandEngine;
    }

    public override Task Invoke(string[] param)
    {
        //foreach (var helpLine in _helpLines)
        //{
        //    _commandEngine.RootCommand.Console.Output(new WriteEventArgs(helpLine.Text, OutputLevel.Help, helpLine.CanExecute() ? helpLine.ForeColor : ConsoleColor.DarkGray));
        //}
        throw new NotImplementedException();
    }

    //internal void AddLine(string text, Func<bool> canExecute = null, ConsoleColor foreColor = ConsoleColor.Gray)
    //{
    //    _helpLines.Add(new HelpLine(text, canExecute ?? (() => true), foreColor));
    //}
}