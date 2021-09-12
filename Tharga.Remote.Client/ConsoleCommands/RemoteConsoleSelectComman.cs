using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;

namespace Tharga.Remote.Client.ConsoleCommands
{
    internal class RemoteConsoleSelectComman : AsyncActionCommandBase
    {
        private readonly IClient _client;

        public RemoteConsoleSelectComman(IClient client)
            : base("Select")
        {
            _client = client;
        }

        public override async Task InvokeAsync(string[] param)
        {
            var consoles = await _client.GetListAsync();
            //TODO: Only list consoles that have not been selected.
            var console = QueryParam("Console", param, consoles.ToDictionary(x => x, x => x.Name));

            await _client.SelectAsync(console.Key);
        }
    }

    internal class RemoteConsoleUnselectComman : AsyncActionCommandBase
    {
        private readonly IClient _client;

        public RemoteConsoleUnselectComman(IClient client)
            : base("Unselect")
        {
            _client = client;
        }

        public override async Task InvokeAsync(string[] param)
        {
            var consoles = await _client.GetListAsync();
            //TODO: Only list selected here
            //TODO: Perhaps have an 'All' option.
            var console = QueryParam("Console", param, consoles.ToDictionary(x => x, x => x.Name));

            await _client.UnselectAsync(console.Key);
        }
    }
}