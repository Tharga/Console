Tharga Console
==============

Tharga Toolkit Console is used to simplify construction of advanced console applications.

The development platform is .NET C#.

NuGet
-----

To get started you can download the prebuilt [NuGet package](https://www.nuget.org/packages/Tharga.Toolkit.Console/).

Sample
------

A good sample to look at can be found here.
[Sample code](https://github.com/poxet/tharga-console/blob/master/Samples/SampleConsole/Program.cs)

Arguments
------

You can send arguments to the console application having it execute commands  right when it starts. Each *command* is sent using quotation marks (ie "some list" "some item A"). When sending parameters the console will exit automatically. If you want the console to stay open send the switch /c.

The switch /e will make the console stay open if something goes wrong, otherwise it will close.

####Theese examples will make the console stay open when completed
- "status success" /c
- "status fail" /c
- "status exception" /c
- "status fail" /e
- "status exception" /e

####Theese examples will make the console close when completed
- "status success"
- "status fail"
- "status exception"
- "status success" /e

It is also possible to provide commands using a textfile. Use the command "exec file myCommandFile.txt" as a parameter and the console will execute each line in the file as a separate command. I usually use this method during development when I want to set up data or testing.

Clients
------
There are several different type of consoles that can be used.
- ClientConsole - Regular console used for normal console applications.
- VoiceConsole - With this client you can use voice commands to control the application.
- ServerConsole - This console outputs time information and writes to the event log. Great when hosting services.

#####ServerConsole
The default behaviour is that output of type information, warnings and errors are written to the event log (not the default type).
This can be configured by adding items with the key LogError, LogWarning, LogInformation and LogDefault to appSettings with a boolean value of true or false.

Commands
------

There are two types of commands; container commands and action commands. The container commands is used to group other commands together and the action commands to execute stuff.
When executing commands from the console the names are to be typed in one flow. Say for instance that you have a container command named "some" and an action command named "item".
The command is executed by typing *some item*.

####Sending parapeters
Many times you want to query the user for input. This can be done inside a command like this.
Ex: *var id = QueryParam<string>("Some Id", GetParam(paramList, 0));*
When executing the command by typing *some item*, the user will be queried for *Some Id*.

You can also send the parameter directly by typing *some item A*. This will automatically send the parameter value A as the first parameter for the command. (The part *GetParam(paramList, 0)* will feed the *QueryParam<T>* function with the fist value provided)

####Query users in different ways
The simplest way of querying is just to use the generic *QueryParam<T>* function.
Ex: *var id = QueryParam<string>("Some Id", GetParam(paramList, 0));*

If you want the user to have options to choose from you can provide a list of possible selections as a dictionary. The key is what will be returned and the value (string) what will be displayed.
Ex: *var answer = QueryParam<bool>("Are you sure?", GetParam(paramList, 0), new Dictionary<bool, string> { { true, "Yes" }, { false, "No" } });*

There are also async versions that takes functions of possible selections.

Color
------
There are four types of output, the colors for theese can be configured using the appSettings part of the config file
- InformationColor - Default Green
- WarningColor - Default Yellow
- ErrorColor - Default Red
- EventColor - Default Cyan

License
------
Tharga Console goes under The MIT License (MIT).