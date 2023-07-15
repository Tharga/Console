using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Entities
{
    public class TaskRunner
    {
        private readonly Func<CancellationToken, AutoResetEvent, Task> _action;
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
            _action = (c, a) =>
            {
                action();
                return Task.CompletedTask;
            };
        }

        public TaskRunner(Action<CancellationToken> action)
            : this()
        {
            _action = (c, a) =>
            {
                action(c);
                return Task.CompletedTask;
            };
        }

        public TaskRunner(Action<AutoResetEvent> action)
            : this()
        {
            _action = (c, a) =>
            {
                action(a);
                return Task.CompletedTask;
            };
        }

        public TaskRunner(Action<CancellationToken, AutoResetEvent> action)
            : this()
        {
            _action = (c, a) =>
            {
                action(c, a);
                return Task.CompletedTask;
            };
        }

        public TaskRunner(Func<CancellationToken, AutoResetEvent, Task> action)
            : this()
        {
            _action = action;
        }

        public void Start()
        {
            _task = Task.Run(() =>
            {
                try
                {
                    _action(_cancellationToken.Token, _autoResetEvent);
                }
                catch (Exception exception)
                {
                    Instance.Console.OutputError(exception);
                    throw;
                }
            }, _cancellationToken.Token);
        }

        public void Close()
        {
            _cancellationToken.Cancel();
            _autoResetEvent.Set();
            _task.Wait();
        }
    }
}