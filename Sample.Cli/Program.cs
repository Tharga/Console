using Sample.Cli;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

var console = new ClientConsole();

var command = new RootCommand(console);
command.RegisterCommand<SampleCommands>();

var engine = new CommandEngine(command);
engine.Start(args);