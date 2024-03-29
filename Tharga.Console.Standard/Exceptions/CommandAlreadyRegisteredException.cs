﻿using System;

namespace Tharga.Console
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