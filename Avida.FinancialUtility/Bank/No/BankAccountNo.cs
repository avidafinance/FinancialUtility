using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avida.FinancialUtility.Bank.No
{
    public class BankAccountNo : IBankAccount
    {
        private BankAccountNo()
        {

        }

        public AccountNumberType AccountNumberType { get; private set; }
        public string ClearingNumber { get; private set; }
        public string AccountNumber { get; private set; }
        public string Bank { get; private set; }

        public static bool IsvalidBankAccount(string nr, string bankRegisterFilePath)
        {
            BankAccountNo _;
            string __;
            return TryParseBankAccount(nr, out _, out __, bankRegisterFilePath);
        }

        public static bool TryParseBankAccount(string source, out BankAccountNo bankAccount, out string errMsg, string bankRegisterFilePath)
        {
            try
            {
                bankAccount = CreateBankAccount(source, bankRegisterFilePath);
                errMsg = null;
                return true;
            }
            catch(ArgumentException ex)
            {
                errMsg = ex.Message;
                bankAccount = null;
                return false;
            }
        }

        public static BankAccountNo CreateBankAccount(string accountNumber, string bankRegisterFilePath)
        {
            var cleaned = AccountNumberValidator.Clean(accountNumber) ?? "";

            if (cleaned.Length != 11)
            {
                throw new ArgumentException("A complete account number is 11 digits included clearing and check number.");
            }

            BankAccountNo norwegianAccount = null;
            ArgumentException norwegianAccountException = null;
            try
            {
                norwegianAccount = CreateBankAccount(cleaned.Substring(0, 4), cleaned.Substring(4), bankRegisterFilePath);
            }
            catch (ArgumentException ex)
            {
                norwegianAccountException = ex;
            }

            if (norwegianAccountException != null)
                throw norwegianAccountException;

            return norwegianAccount;
        }

        public static BankAccountNo CreateBankAccount(string clearingNumber, string accountNumber, string bankRegisterFilePath)
        {
            if (string.IsNullOrEmpty(clearingNumber)) throw new ArgumentException("clearingNumber must not be null or empty string.");
            if (string.IsNullOrEmpty(accountNumber)) throw new ArgumentException("accountNumber must not be null or empty string.");

            BankAccountNo bankAccount = new BankAccountNo();
            // Remove any redundant characters
            clearingNumber = AccountNumberValidator.Clean(clearingNumber);
            accountNumber = AccountNumberValidator.Clean(accountNumber);

            // Assign clearing number
            AccountNumberValidator.CheckClearingNumber(clearingNumber);

            // Assign account type and bank
            var bankAndAccountNumberType = ClearingNumberData.GetBankAndAccountNumberType(clearingNumber, bankRegisterFilePath);
            if ((bankAccount.AccountNumberType = bankAndAccountNumberType.Item2) == AccountNumberType.Unknown)
                throw new ArgumentException("Unknown clearingNumber. Could not match clearing number to a known bank.");
            bankAccount.Bank = bankAndAccountNumberType.Item1;
            bankAccount.AccountNumberType = bankAndAccountNumberType.Item2;

            // Assign account number
            AccountNumberValidator.CheckAccountNumber(ref clearingNumber, ref accountNumber, bankAndAccountNumberType.Item2);
            bankAccount.AccountNumber = accountNumber;
            bankAccount.ClearingNumber = clearingNumber;

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

        public string GetCanocialStringRepresentation()
        {
            return string.Format("{0}{1}", this.ClearingNumber, this.AccountNumber);
        }
    }
}
