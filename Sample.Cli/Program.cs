using Sample.Cli;
using Tharga.Console;

var builder = ConsoleApplication.CreateBuilder(args);
builder.AddCommand<BaseCommand>();
var app = builder.Build();
app.Run();
