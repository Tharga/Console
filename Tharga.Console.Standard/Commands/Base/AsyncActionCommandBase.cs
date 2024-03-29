using System.Threading.Tasks;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.Base
{
    public abstract class AsyncActionCommandBase : ActionCommandBase, ICommandAsync
    {
        protected AsyncActionCommandBase(string name, string description = null, bool hidden = false)
            : base(name, description, hidden)
        {
        }

        public override void Invoke(string[] param)
        {
            InvokeAsync(param).GetAwaiter().GetResult();
        }

        public abstract Task InvokeAsync(string[] param);
    }
}