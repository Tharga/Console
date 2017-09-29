using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Tharga.Toolkit.Console.Helpers
{
    internal static class ExceptionExtension
    {
        public static string ToFormattedString(this Exception exception)
        {
            return ToFormattedString(exception, 0).TrimEnd(Environment.NewLine.ToCharArray());
        }

        private static string ToFormattedString(this Exception exception, int indentationLevel)
        {
            var sb = new StringBuilder();

            var indentation = new string(' ', indentationLevel * 2);
            sb.AppendLine($"{indentation}{exception.Message}");
            foreach (DictionaryEntry data in exception.Data)
            {
                sb.AppendLine($"{indentation}{data.Key}: {data.Value}");
            }

            if (exception.InnerException != null)
            {
                sb.AppendLine(exception.InnerException.ToFormattedString(++indentationLevel));
            }

            //NOTE: Make it configurable if to include stacktrace or not
            sb.AppendLine();
            sb.AppendLine("Stack trace:");
            var stackTrace = exception.StackTrace.Split(new[] { " at " }, StringSplitOptions.None);
            foreach (var line in stackTrace.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var pair = line.Split(new[] { " in " }, StringSplitOptions.None);
                sb.AppendLine(pair[0].Trim());
                sb.AppendLine($"  {pair[1].Trim()}");
            }

            var result = sb.ToString();
            return result;
        }
    }
}