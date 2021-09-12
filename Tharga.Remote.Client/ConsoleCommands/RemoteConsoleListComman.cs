using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;

namespace Tharga.Remote.Client.ConsoleCommands
{
    internal class RemoteConsoleListComman : AsyncActionCommandBase
    {
        private readonly IClient _client;

        public RemoteConsoleListComman(IClient client)
            : base("List")
        {
            _client = client;
        }

        public override async Task InvokeAsync(string[] param)
        {
            var items = await _client.GetListAsync();
            OutputTable(new[]
            {
                "Name",
                "Status",
                "Selected"
            }, items.Select(x => new[]
            {
                x.Name,
                x.Status?.Message,
                "?"
            }));
        }
    }
}