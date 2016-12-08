using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public interface ICommand
    {
        string Name { get; }
        IEnumerable<string> Names { get; }
        string Description { get; }
        void CommandRegistered(IConsole console);
        bool CanExecute(out string reasonMessage);
        Task<bool> InvokeAsync(string paramList);
        Task<bool> InvokeWithCanExecuteCheckAsync(string paramList);
        IEnumerable<HelpLine> HelpText { get; }
    }
}