using System;
using Avida.FinancialUtility.Algorithms;
using Avida.FinancialUtility.Extensions;
using Avida.FinancialUtility.NationalIdentification;

namespace Avida.FinancialUtility.Bank.Se
{
    /// <summary>
    /// Provides methods for validating bank accounts and clearing numbers.
    /// </summary>
    internal class AccountNumberValidator
    {
        /// <summary>
        /// Validates an account number. Throws an exception if the account number is not in valid format.
        /// </summary>
        /// <param name="clearingNumber">The clearing number.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="accountNumberType">Type of the account number.</param>
        /// <param name="isSwedbank">true if the current account is belived to be a swedbank account.</param>
        public static void CheckAccountNumber(ref string clearingNumber, ref string accountNumber, AccountNumberType accountNumberType, bool isSwedbank)
        {
            // The stored accounts with clearing 3300 and 11 digits long account number have an incorrect clearing number. The clearing number needs to be changed to
            // the first 4 digits of the account number, the rest of the string is the actual account number. The AccountNumberType should be changed to Type1, from Type3.
            if (clearingNumber.Equals("3300") && accountNumber.Length == 11 && accountNumberType.Equals(AccountNumberType.Type3))
            {
                FixNordeaPersonkonto3300(ref clearingNumber, ref accountNumber, ref accountNumberType);
            }

            switch (accountNumberType)
            {
                case AccountNumberType.Type1:
                    accountNumber = RemoveClearingFromAccountNumber(clearingNumber, accountNumber, 7);
                    ValidateType1(clearingNumber, ref accountNumber);
                    break;

                case AccountNumberType.Type2:
                    accountNumber = RemoveClearingFromAccountNumber(clearingNumber, accountNumber, 7);
                    ValidateType2(clearingNumber, ref accountNumber);
                    break;

                case AccountNumberType.Type3:
                    if (clearingNumber.Equals("3300") && accountNumber.Length == 12)
                        accountNumber = FixNordeaPersonkonto3300(accountNumber);

                    accountNumber = RemoveClearingFromAccountNumber(clearingNumber, accountNumber, 10);
                    ValidateType3(clearingNumber, ref accountNumber);

                    break;

                case AccountNumberType.Type4:
                    accountNumber = RemoveClearingFromAccountNumber(clearingNumber, accountNumber, 9);
                    ValidateType4(clearingNumber, ref accountNumber);
                    break;

                case AccountNumberType.Type5:
                    if (isSwedbank)
                    {
                        FixSwedbankAccount(ref clearingNumber, ref accountNumber);
                    }
                    ValidateType5(clearingNumber, ref accountNumber);
                    break;
            }
        }

        /// <summary>
        /// Validates the format of a clearing number. Throws an exception if the clearing number is not in valid format.
        /// </summary>
        /// <param name="clearingNumber">The clearing number to validate.</param>
        public static void CheckClearingNumber(string clearingNumber)
        {
            if (!IsInteger(clearingNumber)) throw new ArgumentException("clearingNumber must be numeric.");

            if (clearingNumber.Length > 5 || clearingNumber.Length < 4)
                throw new ArgumentException(string.Format("clearingNumber is {0} characters long. Expected length is 4-5 characters.", clearingNumber.Length));
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
        /// Validates the account number as a Type 1 account. Will try to fix any errors in the account number. If not being able to
        /// fix an incomplete account number an exception will be thrown.
        /// </summary>
        /// <param name="clearingNumber">The clearing number should be 4 digits.</param>
        /// <param name="accountNumber">The account number should be max 7 digits. Will be leftpadded with zeroes if shorter.</param>
        private static void ValidateType1(string clearingNumber, ref string accountNumber)
        {
            // Def. Type 1 Account (BGC: Type 1, Remark 1)
            // (1) The clearing number should be 4 digits.
            // (2) The account number should be 7 digits (if less than 7 characters, leftpad with zeroes).
            // (3) Checksum calculation is made on the clearing number with the exception of the first digit, and seven digits of the actual account number.

            // Clearing number should be 4 digits
            if (clearingNumber.Length != 4)
                throw new ArgumentException(
                    string.Format("clearingNumber is {0} characters long. Expected 4 characters.",
                                  clearingNumber.Length));

            // Account numbers with less than 7 digits should be leftpadded with zeroes.
            accountNumber = accountNumber.PadLeft(7, '0');

            // Acount number should be 7 digits
            if (accountNumber.Length != 7)
                throw new ArgumentException(
                    string.Format("accountNumber is {0} characters long. Expected 7 characters.",
                                  accountNumber.Length));

            // Checksum calculation is made on the clearing number with the exception of the first digit,
            // and seven digits of the actual account number.
            string checkValue = string.Concat(clearingNumber.Substring(1, 3), accountNumber);

            if (!ModulusCheck.Mod11(checkValue))
                throw new ArgumentException(string.Format("accountNumber has an invalid checksum (Type 1)."));
        }

        /// <summary>
        /// Validates the account number as a Type 2 account. Will try to fix any errors in the account number. If not being able to
        /// fix an incomplete account number an exception will be thrown.
        /// </summary>
        /// <param name="clearingNumber">The clearing number should be 4 digits.</param>
        /// <param name="accountNumber">The account number should be max 7 digits. Will be leftpadded with zeroes if shorter.</param>
        private static void ValidateType2(string clearingNumber, ref string accountNumber)
        {
            // Def. Type 2 Account (BGC: Type 1, Remark 2)
            // (1) The clearing number should be 4 digits.
            // (2) The account number should be 7 digits (if less then 7 characters, leftpad with zeroes).
            // (3) Checksum calculation is made on the entire clearing number, and seven digits of the actual account number.

            // Clearing number should be 4 digits
            if (clearingNumber.Length != 4)
                throw new ArgumentException(
                    string.Format("clearingNumber is {0} characters long. Expected 4 characters.",
                                  clearingNumber.Length));

            // Account numbers with less than 7 digits should be leftpadded with zeroes.
            accountNumber = accountNumber.PadLeft(7, '0');

            // Acount number should be 7 digits
            if (accountNumber.Length != 7)
                throw new ArgumentException(
                    string.Format("accountNumber is {0} characters long. Expected 7 characters.",
                                  accountNumber.Length));

            // Checksum calculation is made on the entire clearing number, and seven digits of the actual account number.
            string checkValue = string.Concat(clearingNumber, accountNumber);

            if (!ModulusCheck.Mod11(checkValue))
                throw new ArgumentException(string.Format("accountNumber has an invalid checksum (Type 2)."));
        }

        /// <summary>
        /// Validates the account number as a Type 3 account. Will try to fix any errors in the account number. If not being able to
        /// fix an incomplete account number an exception will be thrown.
        /// </summary>
        /// <param name="clearingNumber">The clearing number should be 4 digits.</param>
        /// <param name="accountNumber">The account number should be max 10 digits. Will be leftpadded with zeroes if shorter.</param>
        private static void ValidateType3(string clearingNumber, ref string accountNumber)
        {
            // Def. Type 3 Account (BGC: Type 2, Remark 1)
            // (1) The clearing number should be 4 digits, and is not a part of the account number.
            // (2) The account number should be 10 digits (if less than 10 charcters, leftpaded with zeroes).
            // (3) Checksum calculation is made on the last 10 digits in the account number (modulus 10 check).

            // Clearing number should be 4 digits
            if (clearingNumber.Length != 4)
                throw new ArgumentException(
                    string.Format("clearingNumber is {0} characters long. Expected 4 characters.",
                                  clearingNumber.Length));

            // Account numbers with less than 10 digits should be leftpadded with zeroes.
            accountNumber = accountNumber.PadLeft(10, '0');

            // Account number should be 10 digits
            if (accountNumber.Length != 10)
                throw new ArgumentException(
                    string.Format("accountNumber is {0} characters long. Expected 10 characters.",
                                  accountNumber.Length));

            if (!ModulusCheck.Mod10(accountNumber))
                throw new ArgumentException(string.Format("accountNumber has an invalid checksum (Type 3)."));
        }

        /// <summary>
        /// Validates the account number as a Type 4 account. Will try to fix any errors in the account number. If not being able to
        /// fix an incomplete account number an exception will be thrown.
        /// </summary>
        /// <param name="clearingNumber">The clearing number shoukld be 4 digits.</param>
        /// <param name="accountNumber">The account number should be max 9 characters. Will be leftpadded with zeroes if shorter.</param>
        private static void ValidateType4(string clearingNumber, ref string accountNumber)
        {
            // Def. Type 4 Account (BGC: Type 2, Remark 2)
            // (1) The clearing number should be 4 digits, and is not a part of the account number.
            // (2) The account number should be 9 digits (if less than 9 characters, leftpaded with zeroes).
            // (3) Checksum calculations is made on the last 9 digits in the account number (modulus 11 check).

            // Clearing number should be 4 digits
            if (clearingNumber.Length != 4)
                throw new ArgumentException(
                    string.Format("clearingNumber is {0} characters long. Expected 4 characters.",
                                  clearingNumber.Length));

            // Account numbers with less than 9 digits should be leftpadded with zeroes.
            accountNumber = accountNumber.PadLeft(9, '0');
            if (accountNumber.Length != 9)
                throw new ArgumentException(
                    string.Format("accountNumber is {0} characters long. Expected 9 characters.",
                                  accountNumber.Length));

            if (!ModulusCheck.Mod11(accountNumber))
                throw new ArgumentException(string.Format("accountNumber has an invalid checksum (Type 4)."));
        }

        /// <summary>
        /// Validates the account number as a Type 5 account. Will try to fix any errors in the account number. If not being able to
        /// fix an incomplete account number an exception will be thrown.
        /// </summary>
        /// <param name="clearingNumber"></param>
        /// <param name="accountNumber"></param>
        private static void ValidateType5(string clearingNumber, ref string accountNumber)
        {
            // Def. Type 5 Account (BGC: Type 2, Remark 3)
            // (1) The clearing number should be 4-5 digits, and is not a part of the account number.
            // (2) The account number should be 7-10 digits (no padding).
            // (3) Checksum calculations is made on the last 7-10 digits in the account number (modulus 10 check).

            if (accountNumber.Length > 10 || accountNumber.Length < 7)
                throw new ArgumentException(
                    string.Format("accountNumber is {0} characters long. Expected 7-10 characters.",
                                  accountNumber.Length));

            if (!ModulusCheck.Mod10(accountNumber))
                throw new ArgumentException(string.Format("accountNumber has an invalid checksum (Type 5)."));
        }

        /// <summary>
        /// Fixes a Nordea personkonto account that is formatted as a 12-digit personnumer, to a 10-digit account.
        /// </summary>
        /// <param name="accountNumber">The account number to fix.</param>
        /// <returns>A 10 digit account number.</returns>
        private static string FixNordeaPersonkonto3300(string accountNumber)
        {
            if (accountNumber.Length == 12)
            {
                try
                {
                    new CivicRegNumberSe(accountNumber);
                }
                catch (Exception)
                {
                    // The account number is not a in the format of a "personnummer".
                    return accountNumber;
                }

                // The account number is in the format of a "personnummer" with 12 digits
                // Nordea-accounts is formatted without the intial 2 digits
                return accountNumber.Substring(2, 10);
            }

            return accountNumber;
        }

        private static void FixNordeaPersonkonto3300(ref string clearingNumber, ref string accountNumber, ref AccountNumberType accountNumberType)
        {
            if (accountNumber.Length == 11 && clearingNumber.Equals("3300"))
            {
                clearingNumber = accountNumber.Substring(0, 4);
                accountNumber = accountNumber.Substring(4, 7);
                accountNumberType = AccountNumberType.Type2;
            }
        }

        private static void FixSwedbankAccount(ref string clearingNumber, ref string accountNumber)
        {
            if (!string.IsNullOrEmpty(clearingNumber) && !string.IsNullOrEmpty(accountNumber))
            {
                if (clearingNumber.Length == 4 && accountNumber.Length == 11)
                {
                    clearingNumber = string.Concat(clearingNumber, accountNumber.Substring(0, 1));
                    accountNumber = accountNumber.Substring(1, 10);
                }
                else if (clearingNumber.Length == 5 && accountNumber.Length == 15)
                {
                    if (accountNumber.Substring(0, 5).Equals(clearingNumber))
                    {
                        accountNumber = accountNumber.Substring(5, 10);
                    }
                }
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