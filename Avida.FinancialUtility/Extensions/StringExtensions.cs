using System;
using System.Linq;

namespace Avida.FinancialUtility.Extensions
{
    internal static class StringExtensions
    {
        public static string RemoveAll(this string source, params string[] toBeRemoved)
        {
            return source == null || toBeRemoved == null
                ? source
                : toBeRemoved.Aggregate(source, (acc, s) => acc.Replace(s, ""));
        }
    }
}