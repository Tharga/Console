using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tharga.Console.Helpers
{
    public static class ParamExtensions
    {
        private const char Quote = '"';

        /// <summary>
        /// Splits an entry into its parameters. Every space outside quotes separates two parameters.
        /// A quoted part keeps its spaces and loses its quotes, also inside a parameter (a="b c" gives a=b c).
        /// A quote that is not closed runs to the end of the entry.
        /// </summary>
        public static IEnumerable<string> ToInput(this string paramList)
        {
            if (string.IsNullOrEmpty(paramList))
                return new List<string>();

            var parameters = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;
            foreach (var character in paramList)
            {
                if (character == Quote)
                {
                    inQuotes = !inQuotes;
                }
                else if (character == ' ' && !inQuotes)
                {
                    parameters.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(character);
                }
            }

            parameters.Add(current.ToString());
            return parameters;
        }

        public static string ToParamString(this IEnumerable<string> param)
        {
            var p = param.ToArray().ToParamList();
            return string.Join(" ", p);
        }

        private static IEnumerable<string> ToParamList(this string[] param)
        {
            foreach (var i in param)
            {
                if (string.IsNullOrEmpty(i))
                {
                    //yield break;
                }
                else if (i.Contains(" "))
                    yield return $"\"{i}\"";
                else
                    yield return i;
            }
        }
    }
}