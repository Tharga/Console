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

It is also possible to provide commands using a textfile. Use the command "exec file myCommandFile.txt" as a parameter and the console will execute each line in the file as a separate command. I usually use this method during development when I want to set up data or testing.

Future
------

A main goal would be to have better test coverage.

It would be nice to be able to use output from one command as input to another, and perhaps support to have it as a powershell plugin.

Please feel free to contribute if you have other ideas of how the project can be improved.

License
------
Tharga Console goes under The MIT License (MIT).