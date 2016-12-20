namespace Tharga.Toolkit.Console.Command.Base
{
    public interface ICommandEngine
    {
        void Run(string[] args);
        void Stop();
        string SplashScreen { get; }
        IConsole Console { get; }
    }
}