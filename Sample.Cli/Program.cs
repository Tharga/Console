using Microsoft.Extensions.DependencyInjection;
using Sample.Cli;
using Tharga.Console;
using Tharga.Console.Consoles;

var builder = ConsoleApplication.CreateBuilder(args);

//builder.UseInput(new NoInputConsole());

builder.Services.AddTransient<IMyService, MyService>();

builder.AddCommand<BaseCommand>();

var app = builder.Build();

app.Run();
