using System.IO;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace Tharga.Toolkit.Console.Log4Net
{
    public class ThargaConsoleAppender : AppenderSkeleton
    {
        public ThargaConsoleAppender()
        {
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.ExceptionObject != null)
                Instance.WriteLine(loggingEvent.ExceptionObject);
            else
            {
                var sb = new StringBuilder();
                using (var sr = new StringWriter(sb))
                    Layout.Format(sr, loggingEvent);
                var msg = sb.ToString();
                Instance.WriteLine(msg, loggingEvent.Level.ToLevel());
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
        }
    }
}