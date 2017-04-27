using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avida.FinancialUtility.Bank
{
    public interface IBankAccount
    {
        /// <summary>
        /// A banks clearing number.
        /// </summary>
        string ClearingNumber { get; }
        /// <summary>
        /// A customers unique bank account number (excluding clearing number).
        /// </summary>
        string AccountNumber { get;  }
        /// <summary>
        /// The bank that the account belongs to.
        /// </summary>
        string Bank { get;  }
        string GetCanocialStringRepresentation();
    }
}
