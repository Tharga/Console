namespace Tharga.Toolkit.Console.Command.Base
{
    internal interface ICommandBase
    {
        void OutputError(string message, params object[] args);
    }
}