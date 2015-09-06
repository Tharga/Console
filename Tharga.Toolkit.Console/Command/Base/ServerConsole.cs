using System;
using System.Diagnostics;
using System.Reflection;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class ServerConsole : SystemConsoleBase
    {
        private readonly string _eventLogSource;

        public ServerConsole( string eventLogSource = null)
            : base(System.Console.Out)
        {
            if (string.IsNullOrEmpty(eventLogSource))
                _eventLogSource = Assembly.GetExecutingAssembly().GetName().Name;
            else
                _eventLogSource = eventLogSource;
        }

        protected override void WriteLine(string value, OutputLevel level)
        {
            var output = string.Format("{0} {1}: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), value);
            if (!EventLog.SourceExists(_eventLogSource))
            {
                try
                {
                    EventLog.CreateEventSource(_eventLogSource, "Application");
                }
                catch (Exception)
                {
                    base.WriteLine(string.Format("Unable to create event source named {0} in the event log.", _eventLogSource), OutputLevel.Error);
                }
            }

            switch (level)
            {
                case OutputLevel.Default:
                    output = value;
                    if (GetSetting(level, false))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Information);
                    break;
                case OutputLevel.Information:
                    if (GetSetting(level, true))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Information);
                    break;
                case OutputLevel.Warning:
                    if (GetSetting(level, true))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Warning);
                    break;
                case OutputLevel.Error:
                    if (GetSetting(level, true))
                        EventLog.WriteEntry(_eventLogSource, value, EventLogEntryType.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown level {0}.", level));
            }

            base.WriteLine(output, level);
        }

        private bool GetSetting(OutputLevel outputLevel, bool defaultValue)
        {
            var data = System.Configuration.ConfigurationManager.AppSettings[string.Format("Log{0}", outputLevel)];
            bool result;
            return bool.TryParse(data, out result) ? result : defaultValue;
        }
    }
}