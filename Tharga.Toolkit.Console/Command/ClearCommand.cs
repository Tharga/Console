using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    class ClearCommand : ActionCommandBase
    {
        internal ClearCommand(IConsole console)
            : base(console, "cls", "Clears the display")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            _console.Clear();
            return true;
        }
    }
}