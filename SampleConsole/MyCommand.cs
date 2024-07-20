using Tharga.Console.Commands.Base;

namespace SampleConsole;

internal class MyCommand : ActionCommandBase
{
	private readonly MyService _myService;

	public MyCommand(MyService myService)
		: base("my")
	{
		_myService = myService;
	}

	public override void Invoke(string[] param)
	{
		OutputInformation("Yeee!");
	}
}