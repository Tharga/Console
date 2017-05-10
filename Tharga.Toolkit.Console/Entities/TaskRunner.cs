using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Entities
{
    public class TaskRunner
    {
        private readonly Action<CancellationToken, AutoResetEvent> _action;
        private readonly CancellationTokenSource _cancellationToken;
        private readonly AutoResetEvent _autoResetEvent;
        private Task _task;

        private TaskRunner()
        {
            _cancellationToken = new CancellationTokenSource();
            _autoResetEvent = new AutoResetEvent(false);
        }

        public TaskRunner(Action action)
            : this()
        {
            _action = (c, a) => { action(); };
        }

        public TaskRunner(Action<CancellationToken> action)
            : this()
        {
            _action = (c, a) => { action(c); };
        }

        public TaskRunner(Action<AutoResetEvent> action)
            : this()
        {
            _action = (c, a) => { action(a); };
        }

        public TaskRunner(Action<CancellationToken, AutoResetEvent> action)
            : this()
        {
            _action = action;
        }

        public void Start()
        {
            _task = Task.Run(() => { _action(_cancellationToken.Token, _autoResetEvent); }, _cancellationToken.Token);
        }

        public void Close()
        {
            _cancellationToken.Cancel();
            _autoResetEvent.Set();
            _task.Wait();
        }
    }
}