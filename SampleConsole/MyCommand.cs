using Tharga.Console.Commands.Base;
using InvalidOperationException = System.InvalidOperationException;

namespace SampleConsole;

internal class MyCommand : AsyncActionCommandBase
{
	private readonly MyService _myService;

	public MyCommand(MyService myService)
		: base("my")
	{
		_myService = myService;
	}

	public override async Task InvokeAsync(string[] param)
	{
		OutputInformation("Yeee!");
		await SomeAction();
		OutputInformation("Yooo!");
	}

	private async Task SomeAction()
	{
		throw new InvalidOperationException("Some issue.");
	}
}