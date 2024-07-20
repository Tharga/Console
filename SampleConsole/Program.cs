using Microsoft.Extensions.DependencyInjection;
using SampleConsole;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

using var console = new ClientConsole();
var command = new RootCommandIoc(console);
command.RegisterCommand<MyContainer>();
command.ServiceCollection.AddTransient<MyService>();
var engine = new CommandEngine(command);
engine.Start(args);