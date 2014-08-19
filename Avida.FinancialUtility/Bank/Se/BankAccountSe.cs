using System;

namespace Avida.FinancialUtility.Bank.Se
{
    public class BankAccountSe
    {
        private BankAccountSe()
        {
        }

        public string ClearingNumber { get; private set; }
        public string AccountNumber { get; private set; }

        /// <summary>
        /// The account number type, which specifies how to calculate the check sum.
        /// </summary>
        public AccountNumberType AccountNumberType { get; private set; }

        /// <summary>
        /// The bank that the account belongs to.
        /// </summary>
        public string Bank { get; private set; }

        public string GetCanonicalStringRepresentation()
        {
            return string.Format("{0}{1}", this.ClearingNumber, this.AccountNumber);
        }

        /// <summary>
        /// 4 digits clearing number + 12 digits account number
        /// according to various arcane rules (http://www.bgc.se/upload/Gemensamt/Trycksaker/Manualer/BG910.pdf)
        /// </summary>
        public string GetBankFileRepresentation()
        {
            if (!(this.ClearingNumber.Length == 4 || this.ClearingNumber.Length == 5))
                throw new Exception("Clearingnumber must be length 4 or 5 but was '" + this.ClearingNumber + "'");

            var cl = this.ClearingNumber.Substring(0, 4);
            var accountNo = this.AccountNumber.PadLeft(12, '0');

            if (accountNo.Length > 12)
                throw new Exception("Account number cannot be longer than 12");

            return string.Concat(cl, accountNo);
        }

        public static bool IsValidBankAccount(string nr)
        {
            BankAccountSe _;
            string __;
            return TryParseBankAccount(nr, out _, out __);
        }

        public static bool TryParseBankAccount(string source, out BankAccountSe bankAccount, out string errMsg)
        {
            try
            {
                bankAccount = CreateBankAccount(source);
                errMsg = null;
                return true;
            }
            catch (ArgumentException ex)
            {
                errMsg = ex.Message;
                bankAccount = null;
                return false;
            }
        }

        //TODO: Get rid of the exceptions used for logic and add tryparse instead
        public static BankAccountSe CreateBankAccount(string accountNumber)
        {
            var cleaned = AccountNumberValidator.Clean(accountNumber) ?? "";

            if (cleaned.Length < 6)
            {
                throw new ArgumentException("An account number is at least 6 digits");
            }
            BankAccountSe a4 = null;
            ArgumentException a4Exception = null;
            try
            {
                a4 = CreateBankAccount(cleaned.Substring(0, 4), cleaned.Substring(4));
            }
            catch (ArgumentException ex)
            {
                a4Exception = ex;
            }
            BankAccountSe a5 = null;
            ArgumentException a5Exception = null;
            try
            {
                //Assuming clearingnumber of length 5
                a5 = CreateBankAccount(cleaned.Substring(0, 5), cleaned.Substring(5));
                if (a5.Bank != ClearingNumberRange.SwebankName)
                    throw new ArgumentException("5 digits clearing number are only allowed for Swedbank.");
            }
            catch (ArgumentException ex)
            {
                a5Exception = ex;
            }

            //This should be impossible. Both length 4 and length 5 seem valid. This is an error
            if (a4Exception == null && a5Exception == null)
            {
                if (a4.Bank == ClearingNumberRange.SwebankName)
                {
                    //Both are valid and we have swedbank. We want a5 in this case
                    return a5;
                }
                else
                {
                    throw new Exception("Bank account '" + accountNumber + "' seems to be valid both with 4 and 5 clearing digits. This is an error in the parsing logic.");
                }
            }

            //This is not a valid account number by either method. We pick one of the exceptions at random (may be improved by checking length or similar)
            if (a4Exception != null && a5Exception != null)
            {
                throw a4Exception;
            }

            return a4Exception != null ? a5 : a4;
        }

        /// <summary>
        /// Creates a BankAccount object. Throws an argument exception if the clearing number or account number is malformatted.
        /// </summary>
        /// <param name="clearingNumber">The clearing number part of the bank account.</param>
        /// <param name="accountNumber">The account number part of the bank account.</param>
        /// <returns>A BankAccount object.</returns>
        public static BankAccountSe CreateBankAccount(string clearingNumber, string accountNumber)
        {
            if (string.IsNullOrEmpty(clearingNumber)) throw new ArgumentException("clearingNumber must not be null or empty string.");
            if (string.IsNullOrEmpty(accountNumber)) throw new ArgumentException("accountNumber must not be null or empty string.");

            BankAccountSe bankAccount = new BankAccountSe();

            // Remove any redundant characters
            clearingNumber = AccountNumberValidator.Clean(clearingNumber);
            accountNumber = AccountNumberValidator.Clean(accountNumber);
            accountNumber = accountNumber.TrimStart('0');

            // Assign clearing number
            AccountNumberValidator.CheckClearingNumber(clearingNumber);
            bankAccount.ClearingNumber = clearingNumber;

            // Assign account type and bank
            var bankAndAccountNumberType = ClearingNumberRange.GetBankAndAccountNumberType(clearingNumber);
            if ((bankAccount.AccountNumberType = bankAndAccountNumberType.Item2) == AccountNumberType.Unknown)
                throw new ArgumentException("Unknown clearingNumber. Could not match clearingNumber to a known bank.");
            bankAccount.Bank = bankAndAccountNumberType.Item1;

            // Assign account number
            AccountNumberValidator.CheckAccountNumber(ref clearingNumber, ref accountNumber, bankAndAccountNumberType.Item2, bankAccount.Bank == ClearingNumberRange.SwebankName);
            bankAccount.AccountNumber = accountNumber;

            if (bankAccount.AccountNumberType == AccountNumberType.Type5 && bankAccount.Bank == ClearingNumberRange.SwebankName && bankAccount.ClearingNumber.Length == 5)
            {
                bankAccount.ClearingNumber = bankAccount.ClearingNumber.Substring(0, 4);
            }

            return bankAccount;
        }

        /// <summary>
        /// Returns a System.String representation of this object.
        /// </summary>
        /// <returns>A System.String reprsentation of this object.</returns>
        public override string ToString()
        {
            return string.Format("Bank: {0}, Clearing: {1}, Account: {2}", this.Bank, this.ClearingNumber, this.AccountNumber);
        }
    }
}