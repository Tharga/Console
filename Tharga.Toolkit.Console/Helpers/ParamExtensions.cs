using System;
using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public static class ParamExtensions
    {
        public static IEnumerable<string> ToInput(this string paramList)
        {
            if (string.IsNullOrEmpty(paramList))
                return new List<string>();

            GetDelimiteredVerbs(ref paramList, '\"');

            var paramArray = paramList.Split(' ');
            return paramArray;
        }

        public static string ToParamString(this string[] input)
        {
            var p = input.ToParamList();
            return string.Join(" ", p);
        }

        private static IEnumerable<string> ToParamList(this string[] input)
        {
            foreach (var i in input)
            {
                if (string.IsNullOrEmpty(i))
                {
                    //yield break;
                }
                else if (i.Contains(" "))
                    yield return "\"" + i + "\"";
                else
                    yield return i;
            }
        }

        internal static Dictionary<string, string> GetDelimiteredVerbs(ref string paramList, char delimiter)
        {
            var verbs = new Dictionary<string, string>();

            var pos = paramList.IndexOf(delimiter, 0);
            while (pos != -1)
            {
                var endPos = paramList.IndexOf(delimiter, pos + 1);
                var data = paramList.Substring(pos + 1, endPos - pos - 1);
                var key = Guid.NewGuid().ToString();
                verbs.Add(key, data);

                paramList = paramList.Substring(0, pos) + key + paramList.Substring(endPos + 1);

                pos = paramList.IndexOf(delimiter, pos + 1);
            }

            return verbs;
        }
    }
}