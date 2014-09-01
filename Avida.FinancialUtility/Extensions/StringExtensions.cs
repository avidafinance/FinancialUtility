using System;
using System.Linq;

namespace Avida.FinancialUtility.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsOneOf(this string source, params string[] targets)
        {
            return source != null && targets.Any(source.Equals);
        }

        public static string Left(this string source, int length)
        {
            return source.Substring(0, length);
        }

        public static string RemoveAll(this string source, params string[] toBeRemoved)
        {
            return source == null || toBeRemoved == null
                ? source
                : toBeRemoved.Aggregate(source, (acc, s) => acc.Replace(s, ""));
        }

        public static string Right(this string source, int length)
        {
            return source.Substring(source.Length - length, length);
        }
    }
}