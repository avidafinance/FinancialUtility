using System;
using System.Collections.Generic;
using System.Linq;

namespace Avida.FinancialUtility.Bank.Se
{
    /// <summary>
    /// Defines the ranges for clearing number series of Swedish Banks.
    /// </summary>
    internal class ClearingNumberRange
    {
        public const string SwebankName = "Swedbank";

        /// <summary>
        /// Returns all valid clearing number ranges, for Swedish banks.
        /// </summary>
        /// <returns>A list of Tuple[BankName, ClearingNumberFrom, ClearingNumberTo, AccountNumberType].</returns>
        public static IEnumerable<Tuple<string, int, int, AccountNumberType>> GetClearingRanges()
        {
            var ranges = new List<Tuple<string, int, int, AccountNumberType>>
                             {
                                 new Tuple<string, int, int, AccountNumberType>("Avanza Bank", 9550, 9569, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("Citibank", 9040, 9049, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("Danske Bank", 1200, 1399, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Danske Bank", 2400, 2499, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Danske Bank", 9180, 9189, AccountNumberType.Type3),
                                 new Tuple<string, int, int, AccountNumberType>("DnB NOR Bank", 9190, 9199, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("DnB NOR Bank", 9260, 9269, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("Forex Bank", 9400, 9449, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Fortis Bank S.A/NV Stocholm Branch", 9470, 9479, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("GE Money Bank", 9460, 9469, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Handelsbanken", 6000, 6999, AccountNumberType.Type4),
                                 new Tuple<string, int, int, AccountNumberType>("ICA Banken AB", 9270, 9279, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("IKANO Bank", 9170, 9179, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Länsförsäkringar Bank", 3400, 3409, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Länsförsäkringar Bank", 9020, 9029, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("Länsförsäkringar Bank", 9060, 9069, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Marginalen Bank", 9230, 9239, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 1100, 1199, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 1400, 2099, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 3000, 3299, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 3300, 3300, AccountNumberType.Type3),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 3301, 3399, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 3410, 3781, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 3782, 3782, AccountNumberType.Type3),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 3783, 3999, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea", 4000, 4999, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea/Plusgirot", 9500, 9549, AccountNumberType.Type5),
                                 new Tuple<string, int, int, AccountNumberType>("Nordea/Plusgirot", 9960, 9969, AccountNumberType.Type5),
                                 new Tuple<string, int, int, AccountNumberType>("Nordnet", 9100, 9109, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("Resurs Bank", 9280, 9289, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Royal Bank of Scotland", 9090, 9099, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("SBAB", 9250, 9259, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("SEB", 5000, 5999, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("SEB", 9120, 9124, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("SEB", 9130, 9149, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>("Skandiabanken", 9150, 9169, AccountNumberType.Type2),
                                 new Tuple<string, int, int, AccountNumberType>("Sparbanken Syd", 9570, 9579, AccountNumberType.Type3),
                                 new Tuple<string, int, int, AccountNumberType>("Sparbanken Öresund AB (fd Spb Finn, fd Spb Gripen)", 9300, 9349, AccountNumberType.Type3),
                                 new Tuple<string, int, int, AccountNumberType>(SwebankName, 7000,7999, AccountNumberType.Type1),
                                 new Tuple<string, int, int, AccountNumberType>(SwebankName, 8000, 8999, AccountNumberType.Type5),
                                 new Tuple<string, int, int, AccountNumberType>("Ålandsbanken Sverige AB", 2300, 2399, AccountNumberType.Type2)
                             };

            return ranges.ToArray();
        }

        /// <summary>
        /// Returns the bank name and account number type from a clearing number. If no bank could be matched to the
        /// clearing number, AccountNumberType.Unkown will be returned.
        /// </summary>
        /// <param name="clearingNumber">The clearing number to match.</param>
        /// <returns>A tuple object with bank name and account number type.</returns>
        public static Tuple<string, AccountNumberType> GetBankAndAccountNumberType(string clearingNumber)
        {
            if (!AccountNumberValidator.IsInteger(clearingNumber)) throw new ArgumentException("clearingNumber must be numeric.");

            // Item1: Bank, Item2: AccountNumberType
            // Swedbank uses 5 digit clearing numbers. Only the 4 first digits are used, the 5th digit is a check digit
            return GetBankAndAccountNumberType(int.Parse(clearingNumber.Substring(0, 4)));
        }

        /// <summary>
        /// Returns the bank name and account number type from a clearing number. If no bank could be matched to the
        /// clearing number, AccountNumberType.Unkown will be returned.
        /// </summary>
        /// <param name="clearing">The clearing number to match.</param>
        /// <returns>A tuple object with bank name and account number type.</returns>
        private static Tuple<string, AccountNumberType> GetBankAndAccountNumberType(int clearing)
        {
            IEnumerable<Tuple<string, int, int, AccountNumberType>> ranges = GetClearingRanges();
            foreach (var range in ranges.Where(range =>
                                               clearing >= range.Item2 && clearing <= range.Item3))
            {
                return new Tuple<string, AccountNumberType>(range.Item1, range.Item4);
            }

            return new Tuple<string, AccountNumberType>("Okänd bank", AccountNumberType.Unknown);
        }
    }
}