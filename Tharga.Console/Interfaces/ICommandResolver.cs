using System;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface ICommandResolver
    {
        ICommand Resolve(Type type);
    }
}