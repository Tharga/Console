using Tharga.Console.Commands.Base;
using Tharga.Console.Consoles.Base;

namespace Tharga.Console.Commands.ScreenCommands
{
    internal class InfoScreenCommand : ActionCommandBase
    {
        private readonly ConsoleBase _consoleBase;

        public InfoScreenCommand(ConsoleBase consoleBase)
            : base("info", "Show information aabout the console.")
        {
            _consoleBase = consoleBase;
            AddName("i");
        }

        public override void Invoke(string[] param)
        {
            var info = _consoleBase.GetInfo();
            OutputInformation(info);
        }
    }
}