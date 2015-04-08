using System;

namespace Tharga.Toolkit.Console.Exceptions
{
    internal sealed class CommandAlreadyRegisteredException : SystemException
    {
        public CommandAlreadyRegisteredException(string commandName, string commandGroupName)
            : base("Command has already been added to command group.")
        {
            Data.Add("CommandName", commandName);
            Data.Add("CommandGroupName", commandGroupName);
        }
    }
}