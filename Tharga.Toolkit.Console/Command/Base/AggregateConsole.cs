using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class AggregateConsole : SystemConsoleBase
    {
        private readonly SystemConsoleBase[] _consoles;

        public AggregateConsole(params SystemConsoleBase[] consoles)
            : base(consoles.First().ConsoleWriter)
        {
            _consoles = consoles;
        }

        public override string ReadLine()
        {
            return _consoles.First().ReadLine();
        }

        public override ConsoleKeyInfo ReadKey()
        {
            return _consoles.First().ReadKey();
        }

        public override ConsoleKeyInfo ReadKey(bool intercept)
        {
            return _consoles.First().ReadKey(intercept);
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