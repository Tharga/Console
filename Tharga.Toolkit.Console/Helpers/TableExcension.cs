using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tharga.Toolkit.Console.Helpers
{
    internal static class TableExcension
    {
        public static string ToFormattedString(this IEnumerable<IEnumerable<string>> data)
        {
            var sbO = new StringBuilder();
            var arr = data.Select(x => x.ToArray()).ToArray();
            var columnLength = GetColumnSizes(arr);

            foreach (var line in arr)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < line.Length; i++)
                {
                    sb.AppendFormat("{0}{1}", line[i] ?? string.Empty, new string(' ', columnLength[i] - (line[i]?.Length ?? 0) + 1));
                }

                sbO.AppendLine(sb.ToString().TrimEnd());
            }

            var lineCount = arr.Length - 1;
            if (lineCount < 0) lineCount = 0;
            sbO.Append($"{lineCount} lines.");

            return sbO.ToString();
        }

        private static int[] GetColumnSizes(string[][] data)
        {
            if (data.Length == 0)
                return new int[] { };

            var length = new int[data.Max(y => y.Length)];
            foreach (var line in data)
            {
                for (var i = 0; i < line.Length; i++)
                {
                    if ((line[i]?.Length ?? 0) > length[i])
                    {
                        length[i] = line[i]?.Length ?? 0;
                    }
                }
            }

            return length.ToArray();
        }
    }
}