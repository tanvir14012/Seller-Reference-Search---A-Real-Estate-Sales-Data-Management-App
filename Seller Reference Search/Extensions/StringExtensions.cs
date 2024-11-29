namespace Seller_Reference_Search.Extensions
{
    using System;

    public static class StringExtensions
    {
        public static string ForgivingSubstring(this string str, int startIndex, int length)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (startIndex > str.Length)
            {
                return string.Empty;
            }

            if (length < 0)
            {
                length = 0;
            }

            if (startIndex + length > str.Length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }

        public static string ForgivingSubstring(this string str, int startIndex)
        {
            return ForgivingSubstring(str, startIndex, str.Length - startIndex);
        }
    }

}
