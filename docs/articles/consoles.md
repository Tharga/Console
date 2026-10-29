# Consoles

Tharga.Console writes through an `IConsole` abstraction instead of `System.Console` directly. That makes it easy to swap rendering — interactive terminal, event stream, test sink, voice, or even multiple targets at once.

## The built-in implementations

| Console | Package | When to use |
|---|---|---|
| `ClientConsole` | `Tharga.Console` | The default — regular interactive use. Wraps `System.Console`. |
| `EventConsole` | `Tharga.Console.Standard` | Raise `OutputEvent` events instead of writing to a stream. Useful for forwarding to a UI or logger. |
| `ActionConsole` | `Tharga.Console.Standard` | Invoke a delegate (`Action<IActionConsoleOutput>`) for every output. The minimal "plug your own sink" option. A configuration passed to it is applied as for `ClientConsole`. |
| `AggregateConsole` | `Tharga.Console.Standard` | Compose two or more consoles; every write goes to all of them. Pair `ClientConsole` with `EventConsole` to render *and* forward. |
| `NullConsole` | `Tharga.Console.Standard` | Drops every write, and has no keys to read: reading a key waits until the read is cancelled. Useful in tests where you don't want to mock anything. |
| `VoiceConsole` | `Tharga.Console.Speech` | Extends `ClientConsole` with `System.Speech` voice input. Windows only. |

Changed in 4.3: reading a key from `NullConsole` waits until the cancellation token passed to `ReadKey` is cancelled, then throws `OperationCanceledException`, so a query on it ends with an empty entry when the engine stops. It used to return a space at once, and a query on it never ended.

## Choosing one

`ClientConsole` is the right default. The others are useful when you need something specific:

```csharp
// Forward all output to an event subscriber AND render it on screen.
using var ui = new ClientConsole();
using var stream = new EventConsole();
using var console = new AggregateConsole(ui, stream);

stream.OutputEvent += (sender, e) => _telemetry.Track(e.Message, e.Level);

var command = new RootCommand(console);
var engine = new CommandEngine(command);
engine.Start(args);
```

## Output methods

Every `ConsoleBase` (and therefore every implementation above) exposes the same set of methods:

```csharp
console.OutputInformation("Started.");
console.OutputWarning("Cache miss — fetching from upstream.");
console.OutputError("Connection refused.");
console.OutputEvent("User signed in.");
console.OutputHelp("Type 'help' for the command list.");
console.OutputDefault("Plain text, no level.");
```

The `OutputLevel` enum (passed via `WriteEventArgs` to event subscribers) is the same enum the colored output uses, so subscribers can branch on severity.

## Text written to System.Console

The consoles that wrap `System.Console` (all of the above except `NullConsole`) take over its standard output and standard error. Text written with `Console.Write`, `Console.WriteLine` or `Console.Error.WriteLine`, by your code or by a library, goes through the console like any other output instead of breaking into the prompt. Standard error is shown at the `Error` level.

## Redirected standard input

The consoles that wrap `System.Console` read keys with `Console.ReadKey`, which cannot read from redirected standard input (piped input, many CI jobs and services). When keys cannot be read, the prompt or query ends with an error that says so, and `CommandEngine.Start` writes it and returns. Pass the commands as program arguments in that case.

Changed in 4.3: the console used to wait at its prompt without an error when keys could not be read.

## Disposing

All console implementations implement `IDisposable`. Wrap them in `using` (top-level statement) or `using (var ...)` (classic) — `Dispose` clears handlers, restores console colors, and releases speech resources where applicable.

## Plugging in your own

Implement `IConsole` and the matching `IConsoleManager` if you need full control. The simpler path is to subclass `ConsoleBase` — it already implements the public surface, and you only need to wire the underlying input/output. The in-repo `TestConsole` (in `Tharga.Console.Tests`) is a 15-line example you can copy.
