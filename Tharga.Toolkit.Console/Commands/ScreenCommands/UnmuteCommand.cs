using System;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class UnmuteCommand : ActionCommandBase
    {
        public UnmuteCommand()
            : base(new[] { "unmute" }, "Unmute output.", false)
        {
        }

        public override void Invoke(params string[] input)
        {
            InvokeAsync(input.ToParamString()).Wait();
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var type = QueryParam("Type", GetParam(paramList, index++), EnumExtensions.GetValues<OutputLevel>().ToDictionary(x => x, x => x.ToString()));

            throw new NotImplementedException("Fire event that unmutes the console.");
            //((SystemConsoleBase)_console).Unmute(type);

            return true;
        }
    }
}