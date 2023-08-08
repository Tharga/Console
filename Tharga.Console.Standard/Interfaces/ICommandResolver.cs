using System;

namespace Tharga.Console.Interfaces
{
    public interface ICommandResolver
    {
        ICommand Resolve(Type type);
    }
}