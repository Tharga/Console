using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tharga.Console.Entities;

namespace Tharga.Console.Helpers
{
    internal class VariableStore
    {
        private static readonly Lazy<VariableStore> _instanceLoader = new Lazy<VariableStore>(() => new VariableStore());
        private readonly List<Variable> _variables = new List<Variable>();
        public static VariableStore Instance => _instanceLoader.Value;

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