using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Avida.FinancialUtility.Extensions;
using Avida.FinancialUtility.Utilities;

namespace Avida.FinancialUtility.NationalIdentification
{
    /// <summary>
    /// Represents a Finish civic registration number.
    /// http://www.maistraatti.fi/sv/Tjanster/hemkommun_och_befolkningsuppgifter/Personbeteckning/
    /// 
    /// Example: 131052-308T
    /// 
    /// Structure:
    /// ddmmyytsssc
    /// [birthdate][centurymarker][serialno][checkdigit]
    /// 
    /// birthdate: ddmmyy
    /// centurymarker: '-' = 1900, '+' = 1800, 'A' = 2000
    /// serialno: To differentiate between people born on the same day. Odd for men, even for women.
    /// checkdigit: See algorithm
    /// 
    /// 
    /// </summary>
    public class CivicRegNumberFi : NationalIdentificationNumber, IComparable<CivicRegNumberFi>, IEquatable<CivicRegNumberFi>
    {
        /// <summary>
        /// Declare a string which represents a single 12 digit civic registration number.
        /// </summary>
        private string nr;

        public static bool IsValidCivicRegNumberFi(string s)
        {
            CivicRegNumberFi _;
            string __;
            return TryParse(s, out _, out __);
        }

        public static bool TryParse(string nr, out CivicRegNumberFi result, out string errorMessage)
        {
            result = null;
            nr = (nr ?? "").RemoveAll(" ");
            if (nr.Length != 11)
            {
                errorMessage = "Invalid string length";
                return false;
            }

            //ddmmyytsssc
            var datePart = nr.Substring(0, 6);
            var centuryMarker = nr.Substring(6, 1);
            var serialNo = nr.Substring(7, 3);
            char checkDigit = nr.Substring(10, 1)[0];

            if (!centuryMarker.IsOneOf("A", "-", "+"))
            {
                errorMessage = "Invalid century marker. Must be one of '+', '-', 'A'";
                return false;
            }

            var supposedDate = datePart.Substring(0, 4) + (centuryMarker == "-" ? "19" : (centuryMarker == "A" ? "20" : "18")) + datePart.Substring(4, 2);
            if (!DateTimes.ParseDateExact(supposedDate, "ddMMyyyy").HasValue)
            {
                errorMessage = "Invalid birth date";
                return false;
            }

            if (!serialNo.All(char.IsDigit))
            {
                errorMessage = "Invalid serial no";
                return false;
            }

            var computedCheckDigit = ComputeCheckDigit(datePart, serialNo);

            if (checkDigit != computedCheckDigit)
            {
                errorMessage = "Invalid check digit";
                return false;
            }

            result = new CivicRegNumberFi();
            result.nr = nr;
            errorMessage = null;
            return true;
        }

        private static readonly Dictionary<long, char> CheckDigitTable = new Dictionary<long, char>{
            {0, '0'},{1, '1'},{2, '2'},{3, '3'},{4, '4'},{5, '5'},{6, '6'},{7, '7'},{8, '8'},{9, '9'},
            {10, 'A'},{11, 'B'},{12, 'C'},{13, 'D'},{14, 'E'},{15, 'F'},{16, 'H'},{17, 'J'},{18, 'K'},
            {19, 'L'},{20, 'M'},{21, 'N'},{22, 'P'},{23, 'R'},{24, 'S'},{25, 'T'},{26, 'U'},{27, 'V'},
            {28, 'W'},{29, 'X'},{30, 'Y'}};

        private static char ComputeCheckDigit(string datePart, string serialNoPart)
        {
            //rem(ddmmyysss / 31) -> check table for 'digit'
            long reminder;
            Math.DivRem(long.Parse(datePart + serialNoPart), 31L, out reminder);
            return CheckDigitTable[reminder];
        }

        private CivicRegNumberFi()
        {

        }

        public CivicRegNumberFi(string nr)
        {
            string errorMessage;
            CivicRegNumberFi n;
            if (!TryParse(nr, out n, out errorMessage))
                throw new ArgumentException(errorMessage, "nr");

            this.nr = n.nr;
        }

        public override string ToString()
        {
            return this.nr;
        }

        public int CompareTo(CivicRegNumberFi other)
        {
            return this.nr.CompareTo(other.nr);
        }

        public override bool Equals(object obj)
        {
            var o = obj as CivicRegNumberFi;
            if (o == null)
                return base.Equals(obj);
            return Equals(o);
        }

        public bool Equals(CivicRegNumberFi other)
        {
            if (other == null)
                return base.Equals(other);

            return this.nr.Equals(other.nr);
        }

        public override int GetHashCode()
        {
            return this.nr.GetHashCode();
        }

        public override string NormalForm { get { return this.nr; } }
        public override bool IsCompany { get { return false; } }
        public override string CountryTwoLetterIsoCode { get { return "FI"; } }

        public DateTime BirthDate
        {
            get
            {
                var datePart = this.nr.Substring(0, 6);
                var centuryMarker = nr.Substring(6, 1);

                return DateTimes.ParseDateExact(datePart.Substring(0, 4) + (centuryMarker == "-" ? "19" : (centuryMarker == "A" ? "20" : "18")) + datePart.Substring(4, 2), "ddMMyyyy").Value;
            }
        }
    }
}