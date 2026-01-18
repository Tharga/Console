using System.Threading.Tasks;

namespace Tharga.Console.Commands;

public abstract class ActionCommandBase : CommandBase
{
    protected ActionCommandBase(string name, string description = default)
        : base(name, description)
    {
    }

    public abstract Task Invoke(string[] param);

    protected void Output(string text)
    {
        //TODO: Use the selected console to output text.
    }
}