# Tharga Console

Tharga Console is used to simplify construction of advanced console applications.

Perfect for hosting local services where you want to be able to perform some extra features.

## NuGet

Nuget packages can be downloaded from here...
- [.NET Standard](https://www.nuget.org/packages/Tharga.Console.Standard/)
- [.NET](https://www.nuget.org/packages/Tharga.Console/)
- [.NET with Speech](https://www.nuget.org/packages/Tharga.Console.Speech/)

## Engine, Command and Console

Tharga console has an engine that runs the program. The engine needs a root command and a console to run.
Here are some basic examples on how to get started.

#### Simplest core application (top-level statement)
```
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

using var console = new ClientConsole();
var command = new RootCommand(console);
var engine = new CommandEngine(command);
engine.Start(args);
```

#### DependencyInjection

Add nuget package [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) to your project

```
using Microsoft.Extensions.DependencyInjection;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

var serviceCollection = new ServiceCollection();
var serviceProvider = serviceCollection.BuildServiceProvider();

using var console = new ClientConsole();
var command = new RootCommand(console, new CommandResolver(type => (ICommand)serviceProvider.GetService(type)));
var engine = new CommandEngine(command);
engine.Start(args);
```

## Previous versions

#### Simplest core application
```
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using (var console = new ClientConsole())
        {
            var command = new RootCommand(console);
            var engine = new CommandEngine(command);
            engine.Start(args);
        }
    }
}
```

#### Adding custom commands
```
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using (var console = new ClientConsole())
        {
            var command = new RootCommand(console);
            command.RegisterCommand(new FooCommand());
            var engine = new CommandEngine(command);
            engine.Start(args);
        }
    }
}

public class FooCommand : ContainerCommandBase
{
    public FooCommand()
        : base("Foo")
    {
        RegisterCommand(new BarCommand());
    }
}

public class BarCommand : ActionCommandBase
{
    public BarCommand()
        : base("Bar")
    {
    }

    public override void Invoke(string[] param)
    {
        OutputInformation("Foo Bar command executed successfully.");
    }
}
```

#### Adding custom commands using IOC container
You can use any IOC container by using the generic *CommandResolver* class provided or implement your own based by *ICommandResolver* and inject it in the RootCommand constructor.
When registering the console command, just use the generic *RegisterCommand<>* function and provide the command class type (or interface). The IOC will then resolve dependencies for the command.
You can use interfaces for the commands and dependencies, if you do not want to register concrete classes. You can also provide dependencies other than console commands, for the console commands to use.
The commands are resolved and instantiated once when the program starts.
In this example we are using *castle windsor*.
```
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var container = new WindsorContainer();
        container.Register(Component.For<FooCommand>().LifestyleTransient());
        container.Register(Component.For<BarCommand>().LifestyleTransient());

        var commandResolver = new CommandResolver(type => (ICommand)container.Resolve(type));

        using (var console = new ClientConsole())
        {
            var command = new RootCommand(console, commandResolver);
            command.RegisterCommand<FooCommand>();
            var engine = new CommandEngine(command);
            engine.Start(args);
        }
    }
}

public class FooCommand : ContainerCommandBase
{
    public FooCommand()
        : base("Foo")
    {
        RegisterCommand<BarCommand>();
    }
}

public class BarCommand : ActionCommandBase
{
    public BarCommand()
        : base("Bar")
    {
    }

    public override void Invoke(string[] param)
    {
        OutputInformation("Foo Bar command executed successfully.");
    }
}
```

You can also use this to register all commands at once.
```
container.Register(Classes.FromAssemblyInThisApplication()
    .IncludeNonPublicTypes()
    .BasedOn<ICommand>()
    .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
```


#### Adding stuff that should start automatically and run in background
```
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using (var console = new ClientConsole())
        {
            var command = new RootCommand(console);
            var engine = new CommandEngine(command)
            {
                TaskRunners = new[]
                {
                    new TaskRunner((c, a) =>
                    {
                        while (!c.IsCancellationRequested)
                        {
                            console.OutputInformation("Running...");
                            Thread.Sleep(3000);
                        }
                    }),
                },
            };

            engine.Start(args);
        }
    }
}
```

#### Basic text input
In this example we start by registering a command named *Input*. Start the program and type input to test.

You can also provide commands in a single line by typeing *Input ABC 123 qwerty*.

The third way is to start the console program with a provided parameter *"Input ABC 123 qwerty" /c*. (The /c parameter will keep the console open after execution.)
```
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using (var console = new ClientConsole())
        {
            var command = new RootCommand(console);
            command.RegisterCommand(new InputCommand());
            var engine = new CommandEngine(command);

            engine.Start(args);
        }
    }
}

internal class InputCommand : ActionCommandBase
{
    public InputCommand()
        : base("Input")
    {
    }

    public override void Invoke(string[] param)
    {
        //Query the user for input
        var stringInput = QueryParam<string>("Enter a string", param);
        var intInput = QueryParam<int>("Enter an int", param);
        var passwordInput = QueryPassword("Enter a password", param);

        //Show the result
        OutputInformation(stringInput);
        OutputInformation(intInput.ToString());
        OutputInformation(passwordInput);
    }
}
```

#### Input with options
In this example the user can choose between three inputs using the *tab*-key.
It is also possible to manually enter (or provide) any of the named options *First*, *Second* or *Third*, ither as a parameter (Try to type *Input First*) to the command, or to the program (*"Input First" /c*)
```
internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using (var console = new ClientConsole())
        {
            var command = new RootCommand(console);
            command.RegisterCommand(new InputCommand());
            var engine = new CommandEngine(command);

            engine.Start(args);
        }
    }
}

internal class InputCommand : ActionCommandBase
{
    public InputCommand()
        : base("Input")
    {
    }

    public override void Invoke(string[] param)
    {
        //Prepare the selection to choose from
        var selection = new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), "First" },
            { Guid.NewGuid(), "Second" },
            { Guid.NewGuid(), "Third" }
        };

        //Query the user for input
        var someKey = QueryParam("Enter a key", param, selection);

        //Show the result
        OutputInformation(someKey.ToString());
    }
}
```

## Sample

A good sample to look at can be found here.
[Sample code](https://github.com/Tharga/Console/blob/master/SampleCoreConsole/Program.cs)

## Arguments

You can send arguments to the console application having it execute commands right when it starts. Each *command* is sent using quotation marks (ie "some list" "some item A"). When sending parameters the console will exit automatically.

#### /c
If you want the console to stay open send the switch /c.

#### /e
The switch /e will make the console stay open if something goes wrong, otherwise it will close.

#### /r
Resets configuration.

### Theese examples will make the console stay open when completed
- "status success" /c
- "status fail" /c
- "status exception" /c
- "status fail" /e
- "status exception" /e

### Theese examples will make the console close when completed
- "status success"
- "status fail"
- "status exception"
- "status success" /e

It is also possible to provide commands using a textfile. Use the command "exec file myCommandFile.txt" as a parameter and the console will execute each line in the file as a separate command. I usually use this method during development when I want to set up data or testing.

## Console Clients
There are several different type of consoles to choose from that can be used for different purposes.

When building a service that is hosted as a console in development, a good idea is to use the *ClientConsole* when running in development and use the *EventConsole* when running as a service.

### Client inheritance tree
- IOutputConsole
    - IConsole
        - ConsoleBase (*Abstract*)
			- ClientConsole
				- VoiceConsole (In nuget *Tharga.Console.Speech*)
			- NullConsole
			- EventConsole
			- ActionConsole
			- AggregateConsole
	- OutputConsoleBase (*Planned*)
		- EventLogConsole (*Planned*)
		- FileLogConsole (*Planned*)

### ClientConsole
Regular console used for normal console applications.

This console have got several built in commands that can be used for manageing, testing and probing. There are several commands that are normally hidden. Type *help* to se the full description of the commands and what they do.

### EventConsole
Fires off an event on each console output.

### ActionConsole
Fires off a function on each console output.

### NullConsole
Swallows all inputs and outputs.

### AggregateConsole
Merge serveral consoles together and use them all. For instance *ClientConsole* and *EventConsole* in combination.

### VoiceConsole (*Under development*)
Use voice commands to control the application.


## Building Console Commands
There are two types of command classes; container commands and action commands. The container commands is used to group other commands together and the action commands to execute stuff.
When executing commands from the console the names are to be typed in one flow. Say for instance that you have a container command named "some" and an action command named "item".
The command is executed by typing *some item*.

### Sending parapeters
Many times you want to query the user for input. This can be done inside a command like this.
Ex: *var id = QueryParam<string>("Some Id", GetParam(paramList, 0));*
When executing the command by typing *some item*, the user will be queried for *Some Id*.

You can also send the parameter directly by typing *some item A*. This will automatically send the parameter value A as the first parameter for the command. (The part *GetParam(paramList, 0)* will feed the *QueryParam<T>* function with the fist value provided)

### Query input in different ways
The simplest way of querying is just to use the generic *QueryParam<T>* function. The parameter *param* is an enumerable string. The *QueryParam* function will pick the first value from *parm* the first time it is called, and the second value next time and so on. If there are not enough values in *param* the user will be queried for the input.
Ex: *var id = QueryParam<string>("Some Id", param);*

If you want the user to have options to choose from you can provide a list of possible selections as a dictionary. The key is what will be returned and the value (string) what will be displayed.
Ex: *var answer = QueryParam<bool>("Are you sure?", param, new Dictionary<bool, string> { { true, "Yes" }, { false, "No" } });*

There are also async versions that takes functions of possible selections, when using the base class *ActionAsyncCommandBase*.

### Tab
Using the Tab-key, it is possible to cycle over all possible inputs. You can also type the first letters of a command and have the Tab-cycle start from there.
Also combination of command and sub commands can be Tab-Cycled.

Example.
```
Type 's' and press Tab. Then 'screen' will be seen in the prompt. Then press ' ' (space) and Tab again.
Now you are cycling inside the 'screen' command for sub-commands to execute.
```

When in a parameter query input the Tab-key is used to cycle through the alternative parameters provided to the command.

### Arrow up and down
Using the arrow up and down keys, it is possible to cycle over all previously used commands.
This works in the main command mode as well as for each parameter query input, where each entry have its own *memory*.

## Help texts
Type your command followed by -? to get help. Or just use the keyword help.

Override the command classes you crate with the property *HelpText* and write your own help text for each command you create.

## Color
There are four types of output, the colors for theese can be configured using the appSettings part of the config file
- InformationColor - Default Green
- WarningColor - Default Yellow
- ErrorColor - Default Red
- EventColor - Default Cyan

## License
Tharga Console goes under The MIT License (MIT).