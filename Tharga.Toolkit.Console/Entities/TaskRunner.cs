using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Entities
{
    public class TaskRunner
    {
        private readonly Action<CancellationToken, AutoResetEvent> _action;
        private readonly Func<CancellationToken, AutoResetEvent, Task> _asyncAction;
        private readonly AutoResetEvent _autoResetEvent;
        private readonly CancellationTokenSource _cancellationToken;
        private Task _task;

        private TaskRunner()
        {
            _cancellationToken = new CancellationTokenSource();
            _autoResetEvent = new AutoResetEvent(false);
        }

        //public TaskRunner(Action action)
        //    : this()
        //{
        //    _action = (c, a) => { action(); };
        //}

        //public TaskRunner(Action<CancellationToken> action)
        //    : this()
        //{
        //    _action = (c, a) => { action(c); };
        //}

        //public TaskRunner(Action<AutoResetEvent> action)
        //    : this()
        //{
        //    _action = (c, a) => { action(a); };
        //}

        public TaskRunner(Action<CancellationToken, AutoResetEvent> action)
            : this()
        {
            _action = action;
        }

        public TaskRunner(Func<CancellationToken, AutoResetEvent, Task> action)
            : this()
        {
            _asyncAction = action;
        }

        public void Start()
        {
            _task = Task.Run(async () =>
            {
                try
                {
                    if (_action != null)
                        _action(_cancellationToken.Token, _autoResetEvent);
                    else
                        await _asyncAction(_cancellationToken.Token, _autoResetEvent);
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