using Microsoft.Extensions.DependencyInjection;
using Sample.Cli;
using Tharga.Console;

var builder = ConsoleApplication.CreateBuilder(args);
builder.Services.AddTransient<IMyService, MyService>();
builder.AddCommand<BaseCommand>();

var app = builder.Build();
app.Run();
