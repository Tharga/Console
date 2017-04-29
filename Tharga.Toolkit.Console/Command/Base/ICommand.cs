using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public interface ICommand
    {
        string Name { get; }
        IEnumerable<string> Names { get; }
        string Description { get; }
        //bool CanExecute();
        bool CanExecute(out string reasonMessage);
        IEnumerable<HelpLine> HelpText { get; }

        //TODO: Try to hide theese from the interface
        void AttachConsole(IConsole console);
        Task<bool> InvokeAsync(string paramList);
        Task<bool> InvokeWithCanExecuteCheckAsync(string paramList);
    }
}