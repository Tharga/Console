using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ServerConsole : ConsoleBase
    {
        private readonly string _eventLogSource;

        public ServerConsole(string eventLogSource = null)
            : base(new ConsoleManager(System.Console.Out, System.Console.In))
        {
            if (string.IsNullOrEmpty(eventLogSource))
                _eventLogSource = Assembly.GetExecutingAssembly().GetName().Name;
            else
                _eventLogSource = eventLogSource;
        }

        protected override void WriteLineEx(string value, OutputLevel level)
        {
            var output = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}: {value}";
            try
            {
                if (!EventLog.SourceExists(_eventLogSource))
                {
                    try
                    {
                        EventLog.CreateEventSource(_eventLogSource, "Application");
                    }
                    catch (SecurityException)
                    {
                        base.WriteLineEx($"Unable to create event source named {_eventLogSource} in the event log.", OutputLevel.Error);
                    }
                }
            }
            catch (SecurityException exception)
            {
                base.WriteLineEx($"Unable to read event source named {_eventLogSource} in the event log.", OutputLevel.Error);
            }

            switch (level)
            {
                case OutputLevel.Default:
                    output = value;
                    if (GetSetting(OutputType.Log, level, false))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Information);
                    if (GetSetting(OutputType.Trace, level, false))
                        Trace.TraceInformation(value);
                    break;
                case OutputLevel.Information:
                    if (GetSetting(OutputType.Log, level, true))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Information);
                    if (GetSetting(OutputType.Trace, level, true))
                        Trace.TraceInformation(value);
                    break;
                case OutputLevel.Warning:
                    if (GetSetting(OutputType.Log, level, true))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Warning);
                    if (GetSetting(OutputType.Trace, level, true))
                        Trace.TraceWarning(value);
                    break;
                case OutputLevel.Error:
                    if (GetSetting(OutputType.Log, level, true))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Error);
                    if (GetSetting(OutputType.Trace, level, true))
                        Trace.TraceError(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown level {0}.", level));
            }

            base.WriteLineEx(output, level);
        }

        private bool GetSetting(OutputType outputType, OutputLevel outputLevel, bool defaultValue)
        {
            var settingName = string.Format("{0}{1}", outputType, outputLevel);
            var data = System.Configuration.ConfigurationManager.AppSettings[settingName];
            bool result;
            return bool.TryParse(data, out result) ? result : defaultValue;
        }
    }

    public enum OutputType { Log, Trace }
}