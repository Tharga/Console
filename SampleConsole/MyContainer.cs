using Tharga.Console.Commands.Base;

namespace SampleConsole;

internal class MyContainer : ContainerCommandBase
{
	public MyContainer()
		: base("container")
	{
		RegisterCommand<MyCommand>();
	}
}