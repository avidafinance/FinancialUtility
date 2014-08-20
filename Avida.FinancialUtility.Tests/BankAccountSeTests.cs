using System;
using Avida.FinancialUtility.Bank.Se;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Avida.FinancialUtility.Tests
{
    [TestClass]
    public class BankAccountSeTests
    {
        //
        // Valid Accounts
        //

        [TestMethod]
        public void CanCreateValidHandelsbankenBankAccount()
        {
            BankAccountSe account = BankAccountSe.CreateBankAccount("6789123456789");
            Assert.AreEqual("Handelsbanken", account.Bank);
            Assert.AreEqual("6789", account.ClearingNumber);
            Assert.AreEqual("123456789", account.AccountNumber);
            Assert.AreEqual(AccountNumberType.Type4, account.AccountNumberType);
        }

        [TestMethod]
        public void CanCreateValidNordeaBankAccount()
        {
            BankAccountSe account = BankAccountSe.CreateBankAccount("3300192208319232");
            Assert.AreEqual("Nordea", account.Bank);
            Assert.AreEqual("3300", account.ClearingNumber);
            Assert.AreEqual("2208319232", account.AccountNumber);
            Assert.AreEqual(AccountNumberType.Type3, account.AccountNumberType);
        }

        [TestMethod]
        public void CanCreateValidSebBankAccount()
        {
            BankAccountSe account = BankAccountSe.CreateBankAccount("50001234560");
            Assert.AreEqual("SEB", account.Bank);
            Assert.AreEqual("5000", account.ClearingNumber);
            Assert.AreEqual("1234560", account.AccountNumber);
            Assert.AreEqual(AccountNumberType.Type1, account.AccountNumberType);
        }

        [TestMethod]
        public void CanCreateValidSwedbankBankAccount()
        {
            BankAccountSe account = BankAccountSe.CreateBankAccount("888812345674");
            Assert.AreEqual("Swedbank", account.Bank);
            Assert.AreEqual("8888", account.ClearingNumber);
            Assert.AreEqual("12345674", account.AccountNumber);
            Assert.AreEqual(AccountNumberType.Type5, account.AccountNumberType);
        }

        //
        // Invalid Accounts
        //

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreatingHandelsbankenAccountWithInvalidChecksumThrowsExcption()
        {
            var account = BankAccountSe.CreateBankAccount("6789123456780");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreatingNordeaAccountWithInvalidChecksumThrowsExcption()
        {
            var account = BankAccountSe.CreateBankAccount("3300192208319231");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreatingSebAccountWithInvalidChecksumThrowsExcption()
        {
            var account = BankAccountSe.CreateBankAccount("50001234561");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreatingSwedbankAccountWithInvalidChecksumThrowsExcption()
        {
            var account = BankAccountSe.CreateBankAccount("888812345675");
        }
    }
}
