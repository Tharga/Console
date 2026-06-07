# Dependency injection

Tharga.Console doesn't pull in an IoC container — you bring your own. The integration point is `ICommandResolver`, a one-method interface that resolves a `Type` to an `ICommand` instance. Any container can satisfy it.

## Microsoft.Extensions.DependencyInjection

The shortest path — register every `ICommand` in the executing assembly via `AssemblyService`, build the provider, hand it to `CommandResolver`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;
using Tharga.Toolkit.TypeService;

var services = new ServiceCollection();
_ = AssemblyService.GetTypes<ICommand>().Select(services.AddTransient).ToArray();
var provider = services.BuildServiceProvider();

using var console = new ClientConsole();
var command = new RootCommand(console, new CommandResolver(type => (ICommand)provider.GetService(type)));
command.RegisterCommand<HelloCommand>();

var engine = new CommandEngine(command);
engine.Start(args);
```

Note the generic `RegisterCommand<T>()` — the resolver instantiates the command via your container, so the command's own constructor dependencies (services, repositories, anything) are resolved automatically.

## Castle Windsor

The pattern is identical — provide a delegate that calls `container.Resolve`:

```csharp
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

var container = new WindsorContainer();
container.Register(Classes.FromAssemblyInThisApplication()
    .IncludeNonPublicTypes()
    .BasedOn<ICommand>()
    .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

var resolver = new CommandResolver(type => (ICommand)container.Resolve(type));

using var console = new ClientConsole();
var command = new RootCommand(console, resolver);
command.RegisterCommand<FooCommand>();

var engine = new CommandEngine(command);
engine.Start(args);
```

The `Classes.FromAssemblyInThisApplication()` line registers every concrete `ICommand` in one go, instead of having to list them out.

## RootCommandIoc — the built-in shortcut

If you want Microsoft.Extensions.DI without writing the assembly-scan glue yourself, `Tharga.Console` ships `RootCommandIoc` — it builds its own internal `ServiceCollection`, scans for `ICommand` implementations, and exposes the same `RegisterCommand<T>()` surface:

```csharp
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

using var console = new ClientConsole();
var command = new RootCommandIoc(console);
command.RegisterCommand<MyCommand>();

var engine = new CommandEngine(command);
engine.Start(args);
```

Use this when your console app doesn't otherwise need a `ServiceProvider` of its own. If you already have one (e.g. you're hosting in a `IHostBuilder`), keep using the plain `RootCommand` + `CommandResolver` so commands share your existing DI container.

## Lifetimes

Commands resolve **once** at engine startup, then live for the process lifetime. That means:

- `Transient` and `Singleton` lifetimes behave the same here. Pick whichever is consistent with the rest of your app.
- Don't inject `Scoped` services directly into commands — there's no ambient scope. Create a scope inside `Invoke` if you need one.

## Resolving services that aren't commands

The resolver is for `ICommand` types only. If a command needs a non-command service (database, HTTP client, config), inject it into the command's constructor and let your container handle it — Tharga.Console's resolver delegates to your container, so any service the container can build is available.
