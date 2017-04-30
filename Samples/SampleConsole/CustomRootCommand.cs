//using System;
//using System.Collections.Generic;
//using Tharga.Toolkit.Console;
//using Tharga.Toolkit.Console.Commands.Base;
//using Tharga.Toolkit.Console.Commands.Entities;
//using Tharga.Toolkit.Console.Interfaces;

//namespace SampleConsole
//{
//    internal class TextConsole : IOutputConsole
//    {
//        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
//        public event EventHandler<KeyReadEventArgs> KeyReadEvent;

//        public void Dispose()
//        {
//        }

//        //public ConsoleKeyInfo ReadKey(bool intercept)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void Clear()
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputDefault(string message)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputInformation(string message)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputEvent(string message)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputHelp(string message)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputWarning(string message)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputError(Exception exception)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputError(string message)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void Output(string message, OutputLevel outputLevel, bool trunkateSingleLine = false)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        public void Output(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool line)
//        {
//            throw new NotImplementedException();
//        }

//        //public void OutputTable(IEnumerable<string> title, IEnumerable<string[]> data, ConsoleColor? consoleColor = null)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public void OutputTable(string[][] data, ConsoleColor? textColor = null)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        public void NewLine()
//        {
//            throw new NotImplementedException();
//        }

//        public void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor, ConsoleColor? textBackgroundColor)
//        {
//            throw new NotImplementedException();
//        }

//        public void Write(string value)
//        {
//            throw new NotImplementedException();
//        }

//        public int CursorLeft { get; set; }
//        public int CursorTop { get; set; }
//        public int BufferWidth { get; set; }
//        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    internal class ACustomClientConsole : IInteractableConsole
//    {
//        public ACustomClientConsole()
//        {
//            BufferWidth = 2;
//        }

//        public void Dispose()
//        {
//        }

//        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
//        public event EventHandler<KeyReadEventArgs> KeyReadEvent;

//        public ConsoleKeyInfo ReadKey(bool intercept)
//        {
//            return Console.ReadKey();
//        }

//        public void Clear()
//        {
//            Console.Clear();
//        }

//        public void OutputDefault(string message)
//        {
//            Console.WriteLine(message);
//        }

//        public void OutputInformation(string message)
//        {
//            Console.WriteLine(message);
//        }

//        public void OutputEvent(string message)
//        {
//            Console.WriteLine(message);
//        }

//        public void OutputHelp(string message)
//        {
//            Console.WriteLine(message);
//        }

//        public void OutputWarning(string message)
//        {
//            Console.WriteLine(message);
//        }

//        public void OutputError(Exception exception)
//        {
//            Console.WriteLine(exception.Message);
//        }

//        public void OutputError(string message)
//        {
//            Console.WriteLine(message);
//        }

//        public void Output(string message, OutputLevel outputLevel, bool trunkateSingleLine = false)
//        {
//            Console.WriteLine(message);
//        }

//        public void Output(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool line)
//        {
//            Console.WriteLine(message);
//        }

//        public void OutputTable(IEnumerable<string> title, IEnumerable<string[]> data, ConsoleColor? consoleColor = null)
//        {
//            throw new NotImplementedException();
//        }

//        public void OutputTable(string[][] data, ConsoleColor? textColor = null)
//        {
//            throw new NotImplementedException();
//        }

//        public void NewLine()
//        {
//            System.Console.WriteLine();
//        }

//        public void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor, ConsoleColor? textBackgroundColor)
//        {
//            throw new NotImplementedException();
//        }

//        public void Write(string value)
//        {
//            System.Console.Write(value);
//        }

//        public int CursorLeft { get; set; }
//        public int CursorTop { get; set; }
//        public int BufferWidth { get; set; }
//        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
//        {
//        }
//    }

//    internal class BCustomRootCommand : RootCommandBase
//    {
//        public BCustomRootCommand(IConsole console, Action stopAction)
//            : base(console, stopAction)
//        {
//        }
//    }

//    internal class ACustomRootCommand : IRootCommand //RootCommandBase
//    {
//        private readonly IConsole _console;

//        public ACustomRootCommand(IConsole console)
//        {
//            _console = console;
//        }

//        public string Name { get; }
//        public IEnumerable<string> Names { get; }
//        public string Description { get; }

//        public bool CanExecute(out string reasonMessage)
//        {
//            reasonMessage = "?";
//            return true;
//        }

//        public IEnumerable<HelpLine> HelpText { get; }
//        public bool Hidden { get; }
//        public void SetStopAction(Action stop)
//        {
//        }

//        public IConsole Console => _console;

//        public string QueryRootParam()
//        {
//            return System.Console.ReadLine();
//        }

//        public bool Execute(string entry)
//        {
//            return true;
//        }
//    }

//    internal class ACustomCommandEngine : ICommandEngine
//    {
//        public string Title { get; }
//        public string SplashScreen { get; }
//        public bool ShowAssemblyInfo { get; }
//        public bool TopMost { get; }
//        public ConsoleColor BackgroundColor { get; }
//        public ConsoleColor DefaultForegroundColor { get; }
//        public Runner[] Runners { get; }
//        public IConsole Console { get; }

//        public ACustomCommandEngine(ICommand command)
//        {
//        }

//        public void Run(string[] args)
//        {
//            while (true)
//            {
//                System.Console.Write(".");
//                System.Threading.Thread.Sleep(1000);
//            }
//        }

//        public void Stop()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}