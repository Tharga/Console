using Sample.Cli;
using Tharga.Console;

var builder = ConsoleApplication.CreateBuilder(args);
builder.AddCommand<MyCommand>();
builder.AddCommand<MyOtherCommand>();
var app = builder.Build();
app.Run();