using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Commands.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class ActionCommandBase : CommandBase, IActionCommand
    {
        private Func<string> _canExecute;

        public override IEnumerable<HelpLine> HelpText { get { yield break; } }

        protected ActionCommandBase(string[] names, string description = null, bool hidden = false)
           : base(names, description, hidden)
        {
        }

        protected ActionCommandBase(string name, string description = null, bool hidden = false)
            : this(new [] { name }, description, hidden)
        {
        }

        protected override ICommand GetHelpCommand(string paramList)
        {
            var helpCommand = new HelpCommand();
            if (HelpText.Any())
            {
                //TODO: Get the help color
                //var c = ((SystemConsoleBase)((CommandBase)this).Console).GetConsoleColor(OutputLevel.Title);
                //var col = ((SystemConsoleBase)Console).GetConsoleColor(OutputLevel.Title);
                helpCommand.AddLine($"Help for command {Name}.", foreColor: ConsoleColor.DarkCyan);
                foreach (var helpText in HelpText)
                {
                    helpCommand.AddLine(helpText.Text, foreColor: helpText.ForeColor);
                }
                helpCommand.AddLine(string.Empty);
            }

            return helpCommand;
        }

        public void SetCanExecute(Func<string> canExecute)
        {
            _canExecute = canExecute;
        }

        public override bool CanExecute(out string reasonMessage)
        {
            if (_canExecute != null)
            {
                reasonMessage = _canExecute();
                return string.IsNullOrEmpty(reasonMessage);
            }

            return base.CanExecute(out reasonMessage);
        }

        //public override bool CanExecute()
        //{
        //    throw new NotImplementedException();
        //    //reasonMessage = "";
        //    //if (_canExecute == null)
        //    //{
        //    //    //return CanExecute();
        //    //}
        //    //return _canExecute();
        //}

        protected static Func<List<KeyValuePair<string, string>>> SelectionTrueFalse()
        {
            return () => new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(true.ToString(CultureInfo.InvariantCulture), true.ToString(CultureInfo.InvariantCulture)), new KeyValuePair<string, string>(false.ToString(CultureInfo.InvariantCulture), false.ToString(CultureInfo.InvariantCulture)) };
        }

        protected void AssignVariables(object entity, string paramList)
        {
            if (paramList == null || !paramList.Contains(">")) return;

            var variablePart = paramList.Substring(paramList.IndexOf(">", StringComparison.Ordinal) + 1);
            var variablePairs = variablePart.Split(',');
            foreach (var variablePair in variablePairs)
            {
                var pair = variablePair.Replace(" ", string.Empty).Split('=');
                var entitySource = pair[1];

                var property = entity.GetType().GetProperty(entitySource);
                var val = property.GetValue(entity);

                VariableStore.Instance.Add(new Variable(pair[0], val));
            }
        }
    }
}