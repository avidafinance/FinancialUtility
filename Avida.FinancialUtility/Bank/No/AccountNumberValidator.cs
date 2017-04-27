using Avida.FinancialUtility.Algorithms;
using Avida.FinancialUtility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avida.FinancialUtility.Bank.No
{
    public static class AccountNumberValidator
    {
        public static void CheckAccountNumber(ref string clearingNumber, ref string accountNumber, AccountNumberType accountNumberType)
        {
            switch(accountNumberType)
            {
                case AccountNumberType.Type1:
                    accountNumber = RemoveClearingFromAccountNumber(clearingNumber, accountNumber, 7);
                    ValidateType1(clearingNumber, ref accountNumber);
                    break;
            }
        }

        private static string RemoveClearingFromAccountNumber(string clearingNumber, string accountNumber, int accountLength)
        {
            if (accountNumber.Length > accountLength)
            {
                string tmp = accountNumber.Substring(0, clearingNumber.Length);
                if (tmp.Equals(clearingNumber))
                {
                    return accountNumber.Substring(clearingNumber.Length, accountNumber.Length - clearingNumber.Length);
                }
            }

            return accountNumber;
        }

        private static void ValidateType1(string clearingNumber, ref string accountNumber)
        {
            // Clearing number should be 4 digits
            if (clearingNumber.Length != 4)
                throw new ArgumentException(
                    string.Format("clearingNumber is {0} characters long. Expected 4 characters.",
                                  clearingNumber.Length));

            // Acount number should be 7 digits
            if (accountNumber.Length != 7)
                throw new ArgumentException(
                    string.Format("accountNumber is {0} characters long. Expected 7 characters.",
                                  accountNumber.Length));

            string checkValue = string.Concat(clearingNumber, accountNumber);

            if (!ModulusCheck.Mod11(checkValue))
                throw new ArgumentException(string.Format("accountNumber has an invalid checksum (Type 1)."));
        }

        /// <summary>
        /// Validates the format of a clearing number. Throws an exception if the clearing number is not in valid format.
        /// </summary>
        /// <param name="clearingNumber">The clearing number to validate.</param>
        public static void CheckClearingNumber(string clearingNumber)
        {
            if (!IsInteger(clearingNumber)) throw new ArgumentException("clearingNumber must be numeric.");

            if (clearingNumber.Length != 4)
                throw new ArgumentException(string.Format("clearingNumber is {0} characters long. Expected length is 4 characters.", clearingNumber.Length));
        }

        /// <summary>
        /// Returnes a string cleaned from characters used in account number strings that has no significance to its value, such as commas, spaces, etc.
        /// </summary>
        /// <param name="value">The string to clean from unsignificant characters.</param>
        /// <returns>A string cleaned from unsignificant characters.</returns>
        public static string Clean(string value)
        {
            return value.RemoveAll("-", " ", ",", ".");
        }

        /// <summary>
        /// Returns true if a given string is has an integer value.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if the givens string is an integer, else false.</returns>
        public static bool IsInteger(string value)
        {
            int tmp;
            return int.TryParse(value, out tmp);
        }
    }
}
