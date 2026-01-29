using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tharga.Console;

public abstract class CommandGroupBase : CommandBase, ICommandGroup
{
    private readonly List<Type> _commandTypes = new();

    protected CommandGroupBase(string name)
        : base(name)
    {
    }

    public override Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }

    protected void AddCommand<T>() where T : ICommand
    {
        _commandTypes.Add(typeof(T));
    }

    public IEnumerable<Type> GetCommandTypes()
    {
        return _commandTypes;
    }
}
