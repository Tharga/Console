using System.Threading.Tasks;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class AsyncActionCommandBase : ActionCommandBase, ICommandAsync
    {
        protected AsyncActionCommandBase(string name, string description = null, bool hidden = false)
            : base(name, description, hidden)
        {
        }

        public abstract Task InvokeAsync(string[] param);

        public override void Invoke(string[] param)
        {
            InvokeAsync(param).GetAwaiter().GetResult();
        }
    }
}