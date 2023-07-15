using System;
using Tharga.Console.Commands.Base;

namespace Tharga.Console.Commands.ScreenCommands
{
    internal class UnmuteCommand : ActionCommandBase
    {
        public UnmuteCommand()
            : base("unmute", "Unmute output.", false)
        {
        }

        public override void Invoke(string[] param)
        {
            throw new NotImplementedException("Fire event that unmutes the console.");

            //var type = QueryParam("Type", param, EnumExtensions.GetValues<OutputLevel>().ToDictionary(x => x, x => x.ToString()));

            //((SystemConsoleBase)_console).Unmute(type);
        }
    }
}