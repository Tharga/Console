using System;
using System.Collections;
using System.Text;

namespace Tharga.Toolkit.Console.Helpers
{
    internal static class ExceptionExtension
    {
        public static string ToFormattedString(this Exception exception)
        {
            return ToFormattedString(exception, 0);
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

            return sb.ToString().TrimEnd('\n');
        }
    }
}