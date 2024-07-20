using Microsoft.Extensions.Logging;
using Tharga.Console.Consoles.Base;

namespace Tharga.Console;

public static class ConsoleBaseExtensions
{
	public static void Output(this ConsoleBase item, string message, LogLevel logLevel)
	{
		item.Output(message, logLevel.ToOutputLevel());
	}
}