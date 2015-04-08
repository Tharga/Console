using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Tharga.Toolkit.Console.Command.Base
{
    internal class VariableStore
    {
        private static readonly Lazy<VariableStore> InstanceLoader = new Lazy<VariableStore>(() => new VariableStore());
        private readonly List<Variable> _variables = new List<Variable>();
        public static VariableStore Instance { get { return InstanceLoader.Value; } }

        private VariableStore()
        {
        }

        public void Add(Variable variable)
        {
            _variables.RemoveAll(x => string.Compare(x.Name, variable.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
            _variables.Add(variable);
        }

        public T Get<T>(string value)
        {
            var var = _variables.SingleOrDefault(x => string.Compare(x.Name, value, StringComparison.InvariantCultureIgnoreCase) == 0);
            var stringVar = var.Value.ToString();
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(stringVar);
        }
    }
}