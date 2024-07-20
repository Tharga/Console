using System;

namespace Tharga.Console.Commands
{
    public interface ICommandResolver
    {
        ICommand Resolve(Type type);
    }
}