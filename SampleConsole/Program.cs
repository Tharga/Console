using Microsoft.Extensions.DependencyInjection;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

var serviceCollection = new ServiceCollection();
var serviceProvider = serviceCollection.BuildServiceProvider();

using var console = new ClientConsole();
var command = new RootCommand(console, new CommandResolver(type => (ICommand)serviceProvider.GetService(type)));
var engine = new CommandEngine(command);
engine.Start(args);