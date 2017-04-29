using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console
{
    public class Runner
    {
        private readonly Action<CancellationToken> _action;
        private readonly CancellationTokenSource _cancellationTokenS;
        private Task _task;

        public Runner(Action<CancellationToken> action)
        {
            _action = action;
            _cancellationTokenS = new CancellationTokenSource();
        }

        public void Start()
        {
            _task = Task.Run(() =>
            {
                _action(_cancellationTokenS.Token);
            }, _cancellationTokenS.Token);
        }

        public void Close()
        {
            _cancellationTokenS.Cancel();
            _task.Wait();
        }
    }
}