using System;

namespace Avida.FinancialUtility.Bank.Se
{
    /// <summary>
    /// Defines the account number types for bank accounts, according to the rules specified by BGC
    /// in the document Bank Account Numbers in Swedish Banks, http://www.bgc.se/upload/Gemensamt/Trycksaker/Manualer/BG910.pdf.
    /// </summary>
    public enum AccountNumberType
    {
        /// <summary>
        /// Unknown account number type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Total: 11 digits (4 clearing digits, 7 account digits)
        /// Weights: 3 clearing digits (the first digit excluded) + 7 calculation digigts
        /// BGC classification: Type 1, Comment 1
        /// </summary>
        Type1,

        /// <summary>
        /// Total: 11 digits (4 clearing digits, 7 account digits)
        /// Weights: 4 clearing digits + 7 calculation digigts
        /// BGC classification: Type 1, Comment 2
        /// </summary>
        Type2,

        /// <summary>
        /// Total: 14 digits (4 clearing digits, 10 account digits)
        /// Weights: Modulus 10 calculation of the 10 account digits
        /// BGC classification: Type 2, Comment 1
        /// </summary>
        Type3,

        /// <summary>
        /// Total: 13 digits (4 clearing digits, 9 account digits)
        /// Weights: Modulus 11 calculation of the 9 account digits
        /// BGC classification: Type 2, Comment 2
        /// </summary>
        Type4,

        /// <summary>
        /// Total: 15 digits (5 clearing digits, 10 account digits)
        /// Weights: Modulus 10 of the 10 account digits
        /// BGC classification: Type 2, Comment 3
        /// </summary>
        Type5
    }
}
