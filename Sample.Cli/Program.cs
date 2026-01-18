using Microsoft.Extensions.DependencyInjection;
using Sample.Cli;

var builder = new ServiceCollection();
Tharga.Console.ConsoleServiceCollectionExtensions.AddConsole<Tharga.Console.Consoles.ClientConsole>(builder);

var serviceProvider = builder.BuildServiceProvider();

var command = serviceProvider.GetService<Tharga.Console.Commands.RootCommand>();
// var command = new Tharga.Console.Commands.RootCommand(console);
//command.RegisterCommand<SampleCommands>();

var engine = new Tharga.Console.CommandEngine(command);
engine.Start(args);
