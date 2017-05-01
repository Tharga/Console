using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Consoles
{
    public class AggregateConsole : SystemConsoleBase
    {
        private readonly SystemConsoleBase[] _consoles;

        public AggregateConsole(params SystemConsoleBase[] consoles)
            : base(consoles.First().ConsoleWriter)
        {
            _consoles = consoles;
        }

        public override ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return _consoles.First().ReadKey(cancellationToken);
        }

        public override void Initiate(IEnumerable<string> commandKeys)
        {
            var enumerable = commandKeys as string[] ?? commandKeys.ToArray();
            foreach (var console in _consoles)
            {
                console.Initiate(enumerable);
            }
        }

        protected internal override void WriteLineEx(string value, OutputLevel level)
        {
            foreach (var console in _consoles)
            {
                console.WriteLineEx(value, level);
            }
        }
    }
}