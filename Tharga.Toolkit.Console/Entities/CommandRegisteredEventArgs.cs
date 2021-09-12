using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Entities
{
    public class CommandRegisteredEventArgs
    {
        public CommandRegisteredEventArgs(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; }
    }
}