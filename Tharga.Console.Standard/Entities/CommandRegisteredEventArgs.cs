using Tharga.Console.Commands;

namespace Tharga.Console.Entities
{
    public class CommandRegisteredEventArgs
    {
        public ICommand Command { get; }

        public CommandRegisteredEventArgs(ICommand command)
        {
            Command = command;
        }
    }
}