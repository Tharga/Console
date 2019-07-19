using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles.Base
{
    public abstract class ConsoleBase2 : IConsole
    {
        internal readonly IConsoleManager ConsoleManager;
        private readonly List<OutputLevel> _mutedTypes = new List<OutputLevel>();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Dictionary<string, Location> _tagLocalLocation = new Dictionary<string, Location>();
        private IRootCommand RootCommand;

        protected ConsoleBase2(IConsoleManager consoleManager)
        {
            ConsoleManager = consoleManager;
            ConsoleManager.Intercept(this);
            
            if (Instance.Console == null)
            {
                Instance.Setup(this, _cancellationTokenSource.Token);
            }
        }

        public virtual void Output(IOutput output)
        {
            if (output.LineFeed)
                ConsoleManager.WriteLine(output.Message);
            else
                ConsoleManager.Write(output.Message);
        }

        public void OutputError(Exception exception, bool includeStackTrace = false)
        {
            throw new NotImplementedException();
        }

        public void OutputTable(IEnumerable<IEnumerable<string>> data)
        {
            throw new NotImplementedException();
        }

        public void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<PushBufferDownEventArgs> PushBufferDownEvent;
        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        public int CursorLeft => ConsoleManager.CursorLeft;
        public int CursorTop => ConsoleManager.CursorTop;
        public virtual bool SupportsInput => true;
        public int BufferWidth => ConsoleManager.BufferWidth;
        public int BufferHeight => ConsoleManager.BufferHeight;
        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            var consoleKeyInfo = ConsoleManager.KeyInputEngine.ReadKey(cancellationToken);
            OnKeyReadEvent(new KeyReadEventArgs(consoleKeyInfo));
            return consoleKeyInfo;
        }

        public event EventHandler<KeyReadEventArgs> KeyReadEvent;

        protected virtual void OnKeyReadEvent(KeyReadEventArgs e)
        {
            KeyReadEvent?.Invoke(this, e);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void SetCursorPosition(int left, int top)
        {
            throw new NotImplementedException();
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            throw new NotImplementedException();
        }

        public void Attach(IRootCommand rootCommand)
        {
            if (rootCommand == null) throw new ArgumentNullException(nameof(rootCommand), "No rootCommand provided");
            if (RootCommand != null) throw new InvalidOperationException("The command is already attached.");

            RootCommand = rootCommand;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}