using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Consoles.Base;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
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