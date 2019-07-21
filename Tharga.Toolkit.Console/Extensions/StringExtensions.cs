namespace Tharga.Toolkit.Console
{
    public static class StringExtensions
    {
        public static string PadStringAfter(this string value, int totalMinLength, char padValue = ' ')
        {
            if (totalMinLength - value.Length < 0) return value;
            return $"{value}{new string(padValue, totalMinLength - value.Length)}";
        }

        public static string PadStringBefore(this string value, int totalMinLength, char padValue = ' ')
        {
            if (totalMinLength - value.Length < 0) return value;
            return $"{new string(padValue, totalMinLength - value.Length)}{value}";
        }

        public static string Truncate(this string value)
        {
            var maxLength = System.Console.BufferWidth; //TODO: Access through Console Manager
            if (value.Length > maxLength)
            {
                value = value.Substring(0, maxLength);
            }

            return value;
        }
    }
}