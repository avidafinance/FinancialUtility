using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avida.FinancialUtility.Bank.No
{
    public class ClearingNumberData
    {
        /// <summary>
        /// Constructs an enumerable tuple object based on a bank register data with following structure:
        /// clearingNumber;branchNumber;CheckNumber;BankName
        /// The branchNumber field is not mandatory.
        /// </summary>
        /// <param name="bankRegisterFilePath"></param>
        /// <returns>An enumerable tupe object with bank name, clearing number and checksum number</returns>
        public static IEnumerable<Tuple<string, int, int, AccountNumberType>> GetClearingBankData(IEnumerable<string> bankRegisterLines)
        {
            var retData = new List<Tuple<string, int, int, AccountNumberType>>();
            foreach(var line in bankRegisterLines)
            {
                var lineData = line
                    .Split(new char[] { ';' })
                    .ToList();

                var bankName = lineData[3];
                int clearingNr = -1;
                if (!int.TryParse(lineData[0], out clearingNr))
                    continue;

                int checkNr = -1;
                if (!int.TryParse(lineData[2], out checkNr))
                    continue;

                retData.Add(new Tuple<string, int, int, AccountNumberType>(bankName, clearingNr, checkNr, AccountNumberType.Type1));
            }
            return retData.AsEnumerable();
        }

        public static Tuple<string, AccountNumberType> GetBankAndAccountNumberType(string clearingNumber, IEnumerable<string> bankRegisterLines)
        {
            return GetBankAndAccountNumberType(int.Parse(clearingNumber.Substring(0, 4)), bankRegisterLines);
        }

        private static Tuple<string, AccountNumberType> GetBankAndAccountNumberType(int clearing, IEnumerable<string> bankRegisterLines)
        {
            IEnumerable<Tuple<string, int, int, AccountNumberType>> ranges = GetClearingBankData(bankRegisterLines);
            foreach (var range in ranges.Where(range => clearing == range.Item2))
            {
                return new Tuple<string, AccountNumberType>(range.Item1, range.Item4);
            }
            return new Tuple<string, AccountNumberType>("Unknown bank", AccountNumberType.Type1);
        }
    }
}
