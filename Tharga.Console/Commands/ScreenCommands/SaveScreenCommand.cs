using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Consoles.Base;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class SaveScreenCommand : ActionCommandBase
    {
        private readonly ConsoleBase _consoleBase;

        public SaveScreenCommand(ConsoleBase consoleBase)
            : base("save", "Save the console position to registry.")
        {
            _consoleBase = consoleBase;
            AddName("s");
        }

        public override void Invoke(string[] param)
        {
            var info = _consoleBase.SavePosition();
            OutputInformation(info);
        }
    }
}