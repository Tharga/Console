using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console
{
    public class Runner
    {
        private readonly Action<CancellationToken> _action;
        private readonly CancellationTokenSource _cancellationToken;
        private Task _task;

        public Runner(Action<CancellationToken> action)
        {
            _action = action;
            _cancellationToken = new CancellationTokenSource();
        }

        public void Start()
        {
            _task = Task.Run(() =>
            {
                _action(_cancellationToken.Token);
            }, _cancellationToken.Token);
        }

        public void Close()
        {
            _cancellationToken.Cancel();
            _task.Wait();
        }
    }
}