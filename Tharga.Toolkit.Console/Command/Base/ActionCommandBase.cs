using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class ActionCommandBase : CommandBase //, ICommand
    {
        private Func<bool> _canExecute;

        public override IEnumerable<HelpLine> HelpText { get { yield break; } }

        protected ActionCommandBase(string name, string description)
            : this(null, name, description, null)
        {
        }

        protected ActionCommandBase(string[] names, string description)
           : this(null, names, description, null)
        {
        }

        internal ActionCommandBase(IConsole console, string name, string description)
            : this(console, name, description, null)
        {
        }

        internal ActionCommandBase(IConsole console, string[] names, string description)
            : this(console, names, description, null)
        {
        }

        private ActionCommandBase(IConsole console, string name, string description, string helpText)
            : base(console, name, description)
        {
            //HelpText = helpText ?? $"There is no detailed help for command {name}.";
        }

        private ActionCommandBase(IConsole console, string[] names, string description, string helpText)
            : base(console, names, description)
        {
            //HelpText = helpText ?? $"There is no detailed help for command {names[0]}.";
        }

        protected override ICommand GetHelpCommand(string paramList)
        {
            var helpCommand = new HelpCommand(Console);
            if (HelpText.Any())
            {
                helpCommand.AddLine($"Help for command {Name}.", foreColor: ConsoleColor.DarkCyan);
                foreach (var helpText in HelpText)
                {
                    helpCommand.AddLine(helpText.Text, foreColor: helpText.ForeColor);
                }
                helpCommand.AddLine(string.Empty);
            }

            return helpCommand;
        }

        public void SetCanExecute(Func<bool> canExecute)
        {
            _canExecute = canExecute;
        }

        //public override bool CanExecute(out string reasonMessage)
        //{
        //    return base.CanExecute(out reasonMessage);
        //    //throw new NotImplementedException();
        //    ////reasonMessage = "";
        //    ////if (_canExecute == null)
        //    ////{
        //    ////    return CanExecute();
        //    ////}
        //    ////return _canExecute();
        //}

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
                var variable = new Variable(pair[0]);
                var entitySource = pair[1];

                var property = entity.GetType().GetProperty(entitySource);
                variable.Value = property.GetValue(entity);

                VariableStore.Instance.Add(variable);
            }
        }
    }
}