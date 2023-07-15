using Tharga.Console.Commands.Base;
using Tharga.Console.Consoles.Base;

namespace Tharga.Console.Commands.ScreenCommands
{
    internal class ResetScreenCommand : ActionCommandBase
    {
        private readonly ConsoleBase _consoleBase;

        public ResetScreenCommand(ConsoleBase consoleBase)
            : base("reset", "Resets current and stored settings back to default.")
        {
            AddName("r");

            _consoleBase = consoleBase;
        }

        public override void Invoke(string[] param)
        {
            _consoleBase.Reset();
            //OutputInformation("All settings have been removed.");
        }
    }
}