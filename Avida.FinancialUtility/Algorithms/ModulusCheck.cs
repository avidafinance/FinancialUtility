using System.Linq;
using System.Text.RegularExpressions;

namespace Avida.FinancialUtility.Algorithms
{
    internal class ModulusCheck
    {
        /// <summary>
        /// Returns the check digit for a string of numbers. Use when needed to create a mod-10 valid number, append the check digit last in the number.
        /// </summary>
        /// <param name="value">An arbitrary number without a check digit.</param>
        /// <returns>The mod-10 check digit for the supplied number.</returns>
        public static int GetMod10CheckDigit(string value)
        {
            int sum = 0;
            string rev = new string(value.Reverse().ToArray());

            for (int i = 1; i < value.Length + 1; i++)
            {
                int weight = i % 2 == 0 ? 1 : 2;
                int tmp = (rev[i - 1] - '0') * weight;
                if (tmp > 9) tmp -= 9;
                sum += tmp;
            }

            if ((sum % 10) == 0)
                return 0;

            return 10 - (sum % 10);
        }

        /// <summary>
        /// Makes a mod-10 check on a string of digits.
        /// </summary>
        /// <param name="value">The numerical value to check.</param>
        /// <returns>True if the checksum is valid, else false.</returns>
        public static bool Mod10(string value)
        {
            int sum = 0;
            string rev = new string(value.Reverse().ToArray());

            for (int i = 1; i < value.Length + 1; i++)
            {
                int weight = i % 2 == 0 ? 2 : 1;
                int tmp = (rev[i - 1] - '0') * weight;
                if (tmp > 9) tmp -= 9;
                sum += tmp;
            }

            return (sum % 10) == 0;
        }

        /// <summary>
        /// Makes a mod-11 check on a string of digits.
        /// Any input strings longer than 11 digits will return false.
        /// </summary>
        /// <param name="value">The numerical value to check.</param>
        /// <returns>True if the checksum is valid, else false.</returns>
        public static bool Mod11(string value)
        {
            // An account number that is checked with mod-11 is max 11 digits long.
            if (value.Length > 11) return false;

            int sum = 0;
            int[] weights = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1 };
            string rev = new string(value.Reverse().ToArray());

            for (int i = 1; i - 1 < rev.Length; i++)
            {
                sum += (rev[i - 1] - '0') * weights[i - 1];
            }

            return (sum % 11) == 0;
        }

        /// <summary>
        /// Checks if a Norwegian account number is a valid
        /// </summary>
        /// <param name="value">11 characters long account number</param>
        /// <returns>true if valid, else false</returns>
        public static bool AccountMod11CheckNo(string value)
        {
            // A Norwegian account number that is checked with mod-11 is 11 characters long
            if (value.Length != 11)
                return false;
            //Check for only numbers
            if (!Regex.IsMatch(value, @"^\d+$"))
                return false;

            int sum = 0;
            //https://no.wikipedia.org/wiki/MOD11
            int[] weights = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };


            for (int i = 0; i < value.Length - 1; i++)
            {
                sum += int.Parse(value[i].ToString()) * weights[i];
            }
            return (sum % 11 == 0 ? 0 : 11 - (sum % 11)) == int.Parse(value[value.Length - 1].ToString());
        }
    }
}
