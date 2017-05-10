using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands
{
    internal class HelpCommand : ActionCommandBase
    {
        private readonly CommandEngine _commandEngine;
        private readonly List<HelpLine> _helpLines = new List<HelpLine>();

        internal HelpCommand(CommandEngine commandEngine)
            : base("help", "Displays helpt text.", false)
        {
            _commandEngine = commandEngine;
        }

        public override void Invoke(params string[] param)
        {
            foreach (var helpLine in _helpLines)
            {
                _commandEngine.RootCommand.Console.Output(new WriteEventArgs(helpLine.Text, OutputLevel.Help, helpLine.CanExecute() ? helpLine.ForeColor : ConsoleColor.DarkGray));
            }
        }

        internal void AddLine(string text, Func<bool> canExecute = null, ConsoleColor foreColor = ConsoleColor.Gray)
        {
            _helpLines.Add(new HelpLine(text, canExecute ?? (() => true), foreColor));
        }
    }
}