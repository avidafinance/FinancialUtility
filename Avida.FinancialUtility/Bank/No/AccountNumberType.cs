using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avida.FinancialUtility.Bank.No
{
    public enum AccountNumberType
    {
        /// <summary>
        /// Unknown account number type.
        /// </summary>
        Unknown,
        /// <summary>
        /// Total: 10 digits (4 clearing digits, 6 account digits)
        /// Wegiths: Modulus 11 calculation of the 10 account digits.
        /// </summary>
        Type1
    }
}
