using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles
{
    public class AggregateConsole : ConsoleBase
    {
        private readonly ConsoleBase[] _consoles;
        private readonly ConsoleBase _mainConsole;

        public AggregateConsole(ConsoleBase mainConsole, params ConsoleBase[] consoles)
            : this(new ConsoleManager(System.Console.Out, System.Console.In), mainConsole, consoles)
        {
        }

        public AggregateConsole(IConsoleManager consoleManager, ConsoleBase mainConsole, params ConsoleBase[] consoles)
            : base(consoleManager)
        {
            _mainConsole = mainConsole;
            _consoles = consoles;
        }

        public override ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return _mainConsole.ReadKey(cancellationToken);
        }

        public override void Initiate(IEnumerable<string> commandKeys)
        {
            var enumerable = commandKeys as string[] ?? commandKeys.ToArray();
            foreach (var console in _consoles)
                console.Initiate(enumerable);
        }

        public override void Output(IOutput output)
        {
            foreach (var console in _consoles)
                console.Output(output);
        }
    }
}