using System;
using System.Collections.Generic;

namespace Tharga.Console.Commands;

public abstract class CommandGroup : CommandBase
{
    protected readonly List<Type> _commandTypes = new();

    protected CommandGroup(string name, string description = default)
        : base(name, description)
    {
    }

    internal IEnumerable<Type> GetCommandTypes() => _commandTypes;

    public void RegisterCommand<T>()
    {
        //var instance = Activator.CreateInstance(typeof(T)) as ICommand;
        //if (instance == null) throw new InvalidOperationException($"Cannot create instance of {typeof(T).Name}.");
        //_commands.Add(instance.Name, typeof(T));
        _commandTypes.Add(typeof(T));
    }
}
