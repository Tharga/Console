using Microsoft.Extensions.Logging;
using Tharga.Console.Entities;

namespace Tharga.Console
{
	public static class OutputLevelExtensions
	{
		public static OutputLevel ToOutputLevel(this LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Trace:
					return OutputLevel.Default;
				case LogLevel.Debug:
					return OutputLevel.Default;
				case LogLevel.Information:
					return OutputLevel.Information;
				case LogLevel.Warning:
					return OutputLevel.Warning;
				case LogLevel.Error:
					return OutputLevel.Error;
				case LogLevel.Critical:
					return OutputLevel.Error;
				default:
					return OutputLevel.Default;
			}
		}

		public static LogLevel ToLogLevel(this OutputLevel outputLevel)
		{
			switch (outputLevel)
			{
				case OutputLevel.Default:
					return LogLevel.Information;
				case OutputLevel.Information:
					return LogLevel.Information;
				case OutputLevel.Warning:
					return LogLevel.Warning;
				case OutputLevel.Error:
					return LogLevel.Error;
				case OutputLevel.Event:
					return LogLevel.Information;
				case OutputLevel.Help:
					return LogLevel.Information;
				case OutputLevel.Title:
					return LogLevel.Information;
				default:
					return LogLevel.Information;
			}
		}
	}
}