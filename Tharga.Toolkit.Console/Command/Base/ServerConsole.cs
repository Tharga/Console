using System;
using System.Diagnostics;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class ServerConsole : SystemConsoleBase
    {
        private readonly string _logSource;

        public ServerConsole(string logSource)
        {
            _logSource = logSource;
        }

        protected override void WriteLine(string value)
        {
            var output = string.Format("{0} {1}: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), value);
            System.Console.WriteLine(output);
        }

        public override void WriteLine(string value, OutputLevel level)
        {
            base.WriteLine(value, level);

            switch (level)
            {
                case OutputLevel.Default:
                    if (GetSetting(level, false))
                        EventLog.WriteEntry(_logSource, value, EventLogEntryType.Information);
                    break;
                case OutputLevel.Information:
                    if (GetSetting(level, true))
                        EventLog.WriteEntry(_logSource, value, EventLogEntryType.Information);
                    break;
                case OutputLevel.Warning:
                    if (GetSetting(level, true))
                        EventLog.WriteEntry(_logSource, value, EventLogEntryType.Warning);
                    break;
                case OutputLevel.Error:
                    if (GetSetting(level, true))
                        EventLog.WriteEntry(_logSource, value, EventLogEntryType.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown level {0}.", level));
            }            
        }

        private bool GetSetting(OutputLevel outputLevel, bool defaultValue)
        {
            var data = System.Configuration.ConfigurationManager.AppSettings[string.Format("Log{0}", outputLevel)];
            bool result;
            return bool.TryParse(data, out result) ? result : defaultValue;
        }
    }
}