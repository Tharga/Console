using System;

namespace Tharga.Console.Commands
{
    public class CommandResolver : ICommandResolver
    {
        private readonly Func<Type, ICommand> _resolver;

        public CommandResolver(Func<Type, ICommand> resolver)
        {
            _resolver = resolver;
        }

        public ICommand Resolve(Type type)
        {
            return _resolver(type);
        }
    }
}