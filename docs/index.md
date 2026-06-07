---
_layout: landing
---

# Tharga.Console

A toolkit for building **advanced interactive console applications** in .NET. Hosts a command engine, a pluggable console abstraction, and helpers for input handling — useful when you want a console application that does more than read a single line and exit.

Perfect for hosting local services where you want extra features (admin commands, status output, scripted automation) on top of the running process.

## Packages

| Package | Target | What it adds |
|---|---|---|
| [Tharga.Console.Standard](https://www.nuget.org/packages/Tharga.Console.Standard) | netstandard2.0 | Command base classes, command engine, input helpers, console abstractions. Drop into any project. |
| [Tharga.Console](https://www.nuget.org/packages/Tharga.Console) | net8.0 / net9.0 / net10.0 | Modern .NET entry point. References `Tharga.Console.Standard` and adds `ClientConsole`, startup commands, and DI-friendly `RootCommandIoc`. |
| [Tharga.Console.Speech](https://www.nuget.org/packages/Tharga.Console.Speech) | net8.0 / net9.0 / net10.0 | Optional voice input integration via `System.Speech` for hands-free console interaction. |

## Quick start

```
dotnet add package Tharga.Console
```

```csharp
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

using var console = new ClientConsole();
var command = new RootCommand(console);
var engine = new CommandEngine(command);
engine.Start(args);
```

See [Getting started](articles/getting-started.md) for the full setup walkthrough.

## What's in the box

- **Command engine** — `RootCommand` + `CommandEngine` parse the input line, dispatch to the right command, and loop until the user exits. See [Commands](articles/commands.md).
- **Command types** — `ContainerCommandBase` for namespaced sub-trees (e.g. `screen clear`), `ActionCommandBase` for leaf commands, `AsyncActionCommandBase` for async work. See [Commands](articles/commands.md).
- **Console abstraction** — `ClientConsole` is the default; `IConsole` lets you plug in alternative renderers (event-driven, tests, voice). See [Consoles](articles/consoles.md).
- **Dependency injection** — bring your own IoC container (Microsoft.Extensions.DI, Castle Windsor, anything) via `ICommandResolver`. See [Dependency injection](articles/dependency-injection.md).

## Repo

[github.com/Tharga/Console](https://github.com/Tharga/Console) — source, issues, releases.
