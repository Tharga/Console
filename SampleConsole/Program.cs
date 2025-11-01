using Microsoft.Extensions.DependencyInjection;
using SampleConsole;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;
using Tharga.Runtime;

var serviceCollection = new ServiceCollection();
_ = AssemblyService.GetTypes<ICommand>().Select(serviceCollection.AddTransient).ToArray();
serviceCollection.AddTransient<MyService>();
var serviceProvider = serviceCollection.BuildServiceProvider();

using var console = new ClientConsole();
var command = new RootCommand(console, new CommandResolver(type => (ICommand)serviceProvider.GetService(type)));
command.RegisterCommand<MyContainer>();
var engine = new CommandEngine(command);
engine.Start(args);

//using Microsoft.Extensions.DependencyInjection;
//using SampleConsole;
//using Tharga.Console;
//using Tharga.Console.Commands;
//using Tharga.Console.Consoles;

//using var console = new ClientConsole();
//var command = new RootCommandIoc(console);
//command.RegisterCommand<MyContainer>();
//command.ServiceCollection.AddTransient<MyService>();
//var engine = new CommandEngine(command);
//engine.Start(args);