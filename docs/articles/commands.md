# Commands

A command is one node in the interactive tree. The user types a verb, optionally followed by parameters, and the engine routes it to the matching command's `Invoke`. There are three base classes — pick the one that matches what your command actually does.

## ActionCommandBase

A **leaf command** that runs some work and returns. Use this for verbs like `clear`, `save`, `restart`, `hello`.

```csharp
public class GreetCommand : ActionCommandBase
{
    public GreetCommand() : base("greet", "Greets the user.") {}

    public override void Invoke(string[] param)
    {
        var name = QueryParam<string>("Name", GetParam(param, 0));
        OutputInformation($"Hello, {name}!");
    }
}
```

- `QueryParam<T>(label, suppliedValue)` returns the param if the user already typed it, or prompts for it interactively if not. It handles type conversion (`int`, `bool`, `enum`, etc.).
- `Output*` methods write through the active `IConsole` with the right severity colors (`OutputInformation`, `OutputWarning`, `OutputError`, `OutputEvent`, `OutputHelp`, `OutputDefault`).

## AsyncActionCommandBase

Same as `ActionCommandBase`, but you override `InvokeAsync` instead of `Invoke`. Use this whenever the command awaits I/O — the engine handles the sync-over-async bridging.

```csharp
public class FetchCommand : AsyncActionCommandBase
{
    public FetchCommand() : base("fetch", "Fetches data from a URL.") {}

    public override async Task InvokeAsync(string[] param)
    {
        var url = QueryParam<string>("Url", GetParam(param, 0));
        using var http = new HttpClient();
        var body = await http.GetStringAsync(url);
        OutputInformation(body);
    }
}
```

## ContainerCommandBase

A **namespaced sub-tree**. The container itself doesn't do work — it groups related verbs. Built-ins like `screen` (containing `screen clear`, `screen reset`, etc.) use this.

```csharp
public class DbCommand : ContainerCommandBase
{
    public DbCommand() : base("db", "Database operations.")
    {
        RegisterCommand(new DbBackupCommand());
        RegisterCommand(new DbRestoreCommand());
    }
}
```

At the prompt: `db backup` or `db restore`. Typing just `db` lists the sub-commands.

## Naming and aliases

The first ctor argument is the canonical name (lowercased for matching, displayed in its original case). Add aliases inside the ctor with `AddName`:

```csharp
public class HelloCommand : ActionCommandBase
{
    public HelloCommand() : base("hello", "Says hello.")
    {
        AddName("hi");
        AddName("hey");
    }

    public override void Invoke(string[] param) => OutputInformation("Hello!");
}
```

All three names route to the same command.

## Visibility (`IsVisible`)

> **4.0 rename:** Before 4.0 this property was `IsHidden` with inverted semantics. The constructor parameter was `bool hidden = false`. From 4.0 onward use `bool visible = true`. Subclasses passing a positional `true` for hidden need to pass `false` to preserve "hidden" behavior.

A command can be hidden from the default `help` listing — useful for built-ins that are noisy or only relevant in special situations. Pass `false` as the third constructor argument:

```csharp
public class DiagnosticsCommand : ActionCommandBase
{
    public DiagnosticsCommand() : base("diag", "Dump diagnostics.", visible: false) {}

    public override void Invoke(string[] param) => OutputInformation("...");
}
```

Hidden commands still execute normally; they just don't show up in `help` unless the user runs `help -all`.

The built-in `screen`, `startup`, `posh`, `run`, `exec`, and `cmd` commands are all hidden by default for this reason.

## Registering commands

Register on the `RootCommand` (or on a parent `ContainerCommandBase`) before starting the engine:

```csharp
using var console = new ClientConsole();
var command = new RootCommand(console);
command.RegisterCommand(new HelloCommand());
command.RegisterCommand(new DbCommand());
var engine = new CommandEngine(command);
engine.Start(args);
```

To register every `ICommand` in the assembly at once, use the generic overload with an IoC container — see [Dependency injection](dependency-injection.md).

## Can-execute guard

Override `CanExecute` on any command to gate it at runtime — handy for "only available when connected" semantics:

```csharp
public override bool CanExecute(out string reasonMessage)
{
    if (_session.IsConnected)
    {
        reasonMessage = null;
        return true;
    }
    reasonMessage = "Not connected to a server.";
    return false;
}
```

The `help` listing greys out disabled commands and shows the reason.
