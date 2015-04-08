namespace Tharga.Toolkit.Console.Helper
{
    internal static class StringExtensions
    {
        internal static string PadString(this string value, int length)
        {
            if (length - value.Length < 0) return value;
            return string.Format("{0}{1}", value, new string(' ', length - value.Length));
        }
    }
}