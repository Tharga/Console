using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ClearCommand : ActionCommandBase
    {
        internal ClearCommand(IConsole console)
            : base(console, new[] { "cls", "clear" }, "Clears the display")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            Console.Clear();
            return true;
        }
    }
}