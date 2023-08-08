using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tharga.Console.Commands.Base;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands
{
    public class QueryInput : QueryParamBase
    {
        private int _paramIndex;

        public QueryInput(IConsole console, CancellationToken cancellationToken = new CancellationToken())
        {
            InputManager = new InputManager(console);
            CancellationToken = cancellationToken;
        }

        protected override IInputManager InputManager { get; }
        protected override CancellationToken CancellationToken { get; }

        protected override string GetNextParam(IEnumerable<string> param)
        {
            return GetParam(param, _paramIndex);
        }

        private string GetParam(IEnumerable<string> param, int index)
        {
            if (param == null) return null;
            var enumerable = param as string[] ?? param.ToArray();
            if (!enumerable.Any()) return null;
            if (enumerable.Length <= index) return null;
            _paramIndex++;
            return enumerable[index];
        }
    }
}