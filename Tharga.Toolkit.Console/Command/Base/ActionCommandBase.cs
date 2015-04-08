using System;
using System.Collections.Generic;
using System.Globalization;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class ActionCommandBase : CommandBase
    {
        private Func<bool> _canExecute;

        public string HelpText { get; set; }

        protected ActionCommandBase(string name, string description)
            : this(null, name, description, null)
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
            HelpText = helpText ?? string.Format("There is no detailed help for command {0}.", name);
        }

        private ActionCommandBase(IConsole console, string[] names, string description, string helpText)
            : base(console, names, description)
        {
            HelpText = helpText ?? string.Format("There is no detailed help for command {0}.", names[0]);
        }

        protected override CommandBase GetHelpCommand()
        {
            if (HelpCommand == null)
            {
                HelpCommand = new HelpCommand(Console);
                HelpCommand.AddLine(string.Format("Help for command {0}. {1}", Name, Description));
                HelpCommand.AddLine(HelpText, CanExecute);
            }

            return HelpCommand;
        }

        public void SetCanExecute(Func<bool> canExecute)
        {
            _canExecute = canExecute;
        }

        public override bool CanExecute()
        {
            return _canExecute == null || _canExecute();
        }

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