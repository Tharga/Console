using Tharga.Console.Commands;

namespace Sample.Cli;

internal class OutputCommand : ActionCommandBase
{
    public OutputCommand()
        : base("Output")
    {
    }

    public override async Task Invoke(string[] param)
    {
        Output("Some text");
    }
}

internal class InputCommand : ActionCommandBase
{
    public InputCommand()
        : base("Input")
    {
    }

    public override Task Invoke(string[] param)
    {
        //var text = QueryParam<string>("Type som text:");
        //Output(text);
        throw new NotImplementedException();
    }
}