# Getting started

## Install

For a typical .NET 8 / 9 / 10 console app:

```
dotnet add package Tharga.Console
```

If you only need the cross-platform core (no `ClientConsole`, no startup commands) — e.g. in a netstandard2.0 library or test project — use `Tharga.Console.Standard` instead:

```
dotnet add package Tharga.Console.Standard
```

For voice input, add `Tharga.Console.Speech` on top of `Tharga.Console`:

```
dotnet add package Tharga.Console.Speech
```

## Namespaces

The core types live across three small namespaces — import them all at the top of `Program.cs`:

```csharp
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;
```

- `Tharga.Console` — `CommandEngine`, the program entry point.
- `Tharga.Console.Commands` — `RootCommand`, `RootCommandIoc`, and the command base classes.
- `Tharga.Console.Consoles` — `ClientConsole` and friends.

## Minimal example

The smallest useful program is three lines: a `ClientConsole`, a `RootCommand`, and a `CommandEngine`. Top-level statements work fine:

```csharp
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

using var console = new ClientConsole();
var command = new RootCommand(console);
var engine = new CommandEngine(command);
engine.Start(args);
```

Run it and you get an interactive prompt with the built-in commands (`help`, `screen`, `startup`, `posh`, etc.). Type `help` to see them.

## Add your own command

Inherit from `ActionCommandBase` for a leaf command:

```csharp
public class HelloCommand : ActionCommandBase
{
    public HelloCommand() : base("hello", "Says hello.") {}

    public override void Invoke(string[] param)
    {
        OutputInformation("Hello!");
    }
}
```

Register it on the root command before starting the engine:

```csharp
using var console = new ClientConsole();
var command = new RootCommand(console);
command.RegisterCommand(new HelloCommand());
var engine = new CommandEngine(command);
engine.Start(args);
```

Typing `hello` at the prompt now writes `Hello!`.

## Pass parameters via the command line

`engine.Start(args)` accepts the host process's `args`. You can run a single command without entering interactive mode by passing it directly:

```
MyApp.exe hello
```

To run a command and *keep* the interactive prompt open, append `/c`:

```
MyApp.exe "hello" /c
```

## Next steps

- [Commands](commands.md) — container / action / async command base classes, naming, visibility.
- [Consoles](consoles.md) — the `IConsole` abstraction and the built-in console types.
- [Dependency injection](dependency-injection.md) — use Microsoft.Extensions.DI or Castle Windsor to resolve your commands.
