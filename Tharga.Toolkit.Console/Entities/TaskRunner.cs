using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Entities
{
    public class TaskRunner
    {
        private readonly Action<CancellationToken> _action;
        private readonly CancellationTokenSource _cancellationToken;
        private Task _task;

        public TaskRunner(Action action)
            : this(e => { action(); })
        {
        }

        public TaskRunner(Action<CancellationToken> action)
        {
            _cancellationToken = new CancellationTokenSource();
            _action = action;
        }

        public void Start()
        {
            _task = Task.Run(() => { _action(_cancellationToken.Token); }, _cancellationToken.Token);
        }

        public void Close()
        {
            _cancellationToken.Cancel();
            _task.Wait();
        }
    }
}