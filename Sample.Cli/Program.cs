using Microsoft.Extensions.DependencyInjection;
using Sample.Cli;

var builder = new ServiceCollection();
builder.AddConsole();

var serviceProvider = builder.Build();

var console = serviceProvider.GetService<IConsole>();
var command = new RootCommand(console);
command.RegisterCommand<SampleCommands>();

var engine = new CommandEngine(command);
engine.Start(args);
