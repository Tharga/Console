using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class ActionAsyncCommandBase : ActionCommandBase, ICommandAsync
    {
        protected ActionAsyncCommandBase(string[] names, string description = null, bool hidden = false)
            : base(names, description, hidden)
        {
        }

        protected ActionAsyncCommandBase(string name, string description = null, bool hidden = false)
            : base(name, description, hidden)
        {
        }

        public abstract Task InvokeAsync(params string[] input);

        public override void Invoke(params string[] input)
        {
            throw new NotSupportedException("For async commands, use InvokeAsync instead.");
        }

        protected async Task<T> QueryParamAsync<T>(string paramName, string autoProvideValue, Func<Task<IDictionary<T, string>>> selectionDelegate)
        {
            List<KeyValuePair<T, string>> selection = null;
            if (selectionDelegate != null)
            {
                OutputInformation($"Loading data for {paramName}...");
                selection = (await selectionDelegate()).ToList();
            }

            var response = QueryParam(paramName, autoProvideValue, selection);
            return response;
        }
    }
}