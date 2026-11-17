using System;
using System.Collections.Generic;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;

namespace Tharga.Console.Commands
{
    internal class HelpCommand : ActionCommandBase
    {
        private readonly CommandEngine _commandEngine;
        private readonly List<HelpLine> _helpLines = new List<HelpLine>();
        private readonly HashSet<HelpLine> _titleLines = new HashSet<HelpLine>();

        internal HelpCommand(CommandEngine commandEngine)
            : base("help", "Displays helpt text.")
        {
            _commandEngine = commandEngine;
        }

        public override void Invoke(string[] param)
        {
            foreach (var helpLine in _helpLines)
            {
                if (_titleLines.Contains(helpLine))
                {
                    _commandEngine.RootCommand.Console.Output(new WriteEventArgs(helpLine.Text, OutputLevel.Title));
                    continue;
                }

                _commandEngine.RootCommand.Console.Output(new WriteEventArgs(helpLine.Text, OutputLevel.Help, helpLine.CanExecute() ? helpLine.ForeColor : ConsoleColor.DarkGray));
            }
        }

        internal void AddLine(string text, Func<bool> canExecute = null, ConsoleColor foreColor = ConsoleColor.Gray)
        {
            _helpLines.Add(new HelpLine(text, canExecute ?? (() => true), foreColor));
        }

        internal void AddTitle(string text)
        {
            var helpLine = new HelpLine(text, () => true, ConsoleColor.Gray);
            _helpLines.Add(helpLine);
            _titleLines.Add(helpLine);
        }
    }
}
