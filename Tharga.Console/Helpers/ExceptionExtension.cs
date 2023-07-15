using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Tharga.Console.Helpers
{
    internal static class ExceptionExtension
    {
        public static string ToFormattedString(this Exception exception, bool includeStackTrace, string prefix)
        {
            var data = ToFormattedString(exception, 0, includeStackTrace).TrimEnd(Environment.NewLine.ToCharArray());
            return $"{prefix}{data}";
        }

        private static string ToFormattedString(this Exception exception, int indentationLevel, bool includeStackTrace)
        {
            var sb = new StringBuilder();

            var indentation = new string(' ', indentationLevel * 2);
            sb.AppendLine($"{indentation}{exception.Message}");
            foreach (DictionaryEntry data in exception.Data)
            {
                sb.AppendLine($"{indentation}{data.Key}: {data.Value}");
            }

            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                indentationLevel++;
                foreach (var exp in aggregateException.InnerExceptions)
                {
                    sb.AppendLine(exp.ToFormattedString(indentationLevel, false));
                }
            }
            else if (exception.InnerException != null)
            {
                sb.AppendLine(exception.InnerException.ToFormattedString(++indentationLevel, false));
            }

            if (includeStackTrace)
            {
                try
                {
                    //NOTE: Make it configurable if to include stacktrace or not
                    sb.AppendLine();
                    sb.AppendLine("Stack trace:");
                    var stackTrace = exception.StackTrace.Split(new[] { " at " }, StringSplitOptions.None);
                    foreach (var line in stackTrace.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        var pair = line.Split(new[] { " in " }, StringSplitOptions.None);
                        if (pair.Length == 2)
                        {
                            sb.AppendLine(pair[0].Trim());
                            sb.AppendLine($"  {pair[1].Trim()}");
                        }
                        else
                            sb.AppendLine(line);
                    }
                }
                catch (Exception e)
                {
                    sb.AppendLine($"Oups, internal error. {e.Message}.");
                }
            }

            var result = sb.ToString();
            return result;
        }
    }
}