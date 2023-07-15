using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class ActionCommandBase : CommandBase, IActionCommand
    {
        private readonly List<Func<string[], KeyValuePair<string, object>>> _registeredQuery = new List<Func<string[], KeyValuePair<string, object>>>();
        private readonly List<Func<string[]>> _selectionDelegate = new List<Func<string[]>>();
        private Dictionary<string, object> _param = new Dictionary<string, object>();
        private Func<string> _canExecute;

        public override IEnumerable<HelpLine> HelpText { get { yield break; } }

        protected ActionCommandBase(string name, string description = null, bool hidden = false)
            : base(name, description, hidden)
        {
        }

        protected void RegisterQuery<T>(string key, string paramName, Func<IEnumerable<KeyValuePair<T, string>>> selectionDelegate)
        {
            _registeredQuery.Add((param) =>
            {
                var result = QueryParam(paramName, param, selectionDelegate());
                return new KeyValuePair<string, object>(key, result);
            });
            _selectionDelegate.Add(() =>
            {
                return selectionDelegate().Select(x => x.Value).ToArray();
            });
        }

        protected T GetParam<T>(string key)
        {
            return (T)_param[key];
        }

        internal override void InvokeEx(string[] param)
        {
            ParamIndex = 0;
            _param = new Dictionary<string, object>();
            foreach (var query in _registeredQuery)
            {
                var result = query.Invoke(param);
                _param.Add(result.Key, result.Value);
            }
            Invoke(param);
        }

        protected override ICommand GetHelpCommand(string paramList)
        {
            var helpCommand = new HelpCommand(RootCommand.CommandEngine);
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

        public IEnumerable<IEnumerable<string>> GetOptionList()
        {
            var l = new List<string>();
            foreach (var x in _selectionDelegate)
            {
                var r = x.Invoke().ToArray();
                foreach (var v in r)
                {
                    l.Add(v);
                }
            }
            yield return l;
        }
    }
}