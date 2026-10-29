namespace Tharga.Console
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

        /// <summary>
        /// Truncates the value to the buffer width of <see cref="System.Console"/>.
        /// A console writes through its own console manager, so it truncates with <see cref="Truncate(string, int)"/> and its own width.
        /// </summary>
        public static string Truncate(this string value)
        {
            return value.Truncate(System.Console.BufferWidth);
        }

        /// <summary>
        /// Truncates the value to at most <paramref name="maxLength"/> characters.
        /// </summary>
        /// <param name="value">The value to truncate.</param>
        /// <param name="maxLength">The largest number of characters to keep, for example the buffer width of the console in use.</param>
        public static string Truncate(this string value, int maxLength)
        {
            if (value.Length > maxLength)
                value = value.Substring(0, maxLength);
            return value;
        }
    }
}