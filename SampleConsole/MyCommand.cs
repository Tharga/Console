using Tharga.Console.Commands.Base;
using InvalidOperationException = System.InvalidOperationException;

namespace SampleConsole;

internal class MyCommand : AsyncActionCommandBase
{
	// The sample only shows constructor injection; the service itself is not used.
	// ReSharper disable once NotAccessedField.Local
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