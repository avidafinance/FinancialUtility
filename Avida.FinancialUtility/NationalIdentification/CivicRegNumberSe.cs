using System;
using System.Globalization;
using System.Linq;
using Avida.FinancialUtility.Extensions;

namespace Avida.FinancialUtility.NationalIdentification
{
    /// <summary>
    /// Represents a Swedish civic registration number.
    /// </summary>
    internal class CivicRegNumberSe : NationalIdentificationNumber, IComparable<CivicRegNumberSe>, IEquatable<CivicRegNumberSe>
    {
        /// <summary>
        /// Declare a string which represents a single 12 digit civic registration number.
        /// </summary>
        private string nr;

        private CivicRegNumberSe()
        {
        }

        public static bool IsValidCivicRegNumberSe(string s, bool guessYearWhenTenDigits = false)
        {
            CivicRegNumberSe _;
            string __;
            return TryParse(s, out _, out __, guessYearWhenTenDigits: guessYearWhenTenDigits);
        }

        public static bool TryParse(string nr, out CivicRegNumberSe result, out string errorMessage, bool guessYearWhenTenDigits = false)
        {
            result = null;
            nr = nr.RemoveAll(" ", "-") ?? "";
            if (!(nr.Length == 12 || (guessYearWhenTenDigits && nr.Length == 10)))
            {
                errorMessage = "Invalid string length";
                return false;
            }

            if (!IsNumeric(nr))
            {
                errorMessage = "Invalid character(s) in value";
                return false;
            }
            var personNoPart = nr.Length == 12 ? nr.Substring(2) : nr;
            if (!IsValidChecksum(personNoPart))
            {
                errorMessage = "Invalid checksum";
                return false;
            }

            string dateString;
            if (nr.Length == 12)
            {
                var year = nr.Substring(0, 4);
                dateString = String.Format("{0}-{1}-{2}", year, nr.Substring(4, 2), nr.Substring(6, 2));
                DateTime d;

                if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.GetCultureInfo("sv-SE"), DateTimeStyles.None, out d))
                {
                    errorMessage = "Invalid date";
                    return false;
                }

                if (int.Parse(year) < 1900 || int.Parse(year) > 2100)
                {
                    errorMessage = "Invalid year";
                    return false;
                }
            }
            else
            {
                var dateString19 = String.Format("19{0}-{1}-{2}", nr.Substring(0, 2), nr.Substring(2, 2), nr.Substring(4, 2));
                var dateString20 = String.Format("20{0}-{1}-{2}", nr.Substring(0, 2), nr.Substring(2, 2), nr.Substring(4, 2));
                DateTime d19;
                DateTime d20;
                var b19 = DateTime.TryParseExact(dateString19, "yyyy-MM-dd", CultureInfo.GetCultureInfo("sv-SE"), DateTimeStyles.None, out d19);
                var b20 = DateTime.TryParseExact(dateString20, "yyyy-MM-dd", CultureInfo.GetCultureInfo("sv-SE"), DateTimeStyles.None, out d20);

                if (!b19 && !b20)
                {
                    errorMessage = "Invalid date";
                    return false;
                }
                else if (b19 && b20)
                {
                    if (d20 > DateTime.Today)
                        nr = "19" + nr;
                    else
                        nr = "20" + nr;
                }
                else if (b19)
                {
                    nr = "19" + nr;
                }
                else
                {
                    nr = "20" + nr;
                }
            }

            var r = new CivicRegNumberSe();
            r.nr = nr;
            result = r;
            errorMessage = null;
            return true;
        }

        public CivicRegNumberSe(string nr, bool guessYearWhenTenDigits = false)
        {
            string errorMessage;
            CivicRegNumberSe n;
            if (!TryParse(nr, out n, out errorMessage, guessYearWhenTenDigits: guessYearWhenTenDigits))
                throw new ArgumentException(errorMessage, "nr");

            this.nr = n.nr;
        }

        public string TenDigitNormalForm
        {
            get { return nr.Substring(2); }
        }

        public string TwelveDigitNormalForm
        {
            get { return nr; }
        }

        /// <summary>
        /// Returns a string representation of the number value of this object.
        /// </summary>
        /// <returns>A System.String representing the number value of this object.</returns>
        public override string ToString()
        {
            return TwelveDigitNormalForm;
        }

        /// <summary>
        /// Checks if the supplied value is numeric.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is numeric, else false.</returns>
        private static bool IsNumeric(string value)
        {
            return !value.Any(c => !Char.IsDigit(c));
        }

        /// <summary>
        /// Checks if the check sum is valid.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the check sum is valid, else false.</returns>
        private static bool IsValidChecksum(string value)
        {
            return ComputeChecksum(value) == 0;
        }

        /// <summary>
        /// Computes the check sum for a civic registration number.
        /// </summary>
        /// <param name="value">The value to compute a check sum for.</param>
        /// <returns>An integer value.</returns>
        private static int ComputeChecksum(string value)
        {
            return value
                .Reverse()
                .SelectMany((c, i) => ((c - '0') << (i & 1)).ToString())
                .Sum(c => c - '0') % 10;
        }

        public int CompareTo(CivicRegNumberSe other)
        {
            return TwelveDigitNormalForm.CompareTo(other.TwelveDigitNormalForm);
        }

        public override bool Equals(object obj)
        {
            var o = obj as CivicRegNumberSe;
            if (o == null)
                return base.Equals(obj);
            return Equals(o);
        }

        public bool Equals(CivicRegNumberSe other)
        {
            if (other == null)
                return base.Equals(other);

            return TwelveDigitNormalForm.Equals(other.TwelveDigitNormalForm);
        }

        public override int GetHashCode()
        {
            return TwelveDigitNormalForm.GetHashCode();
        }

        public override string NormalForm { get { return this.TwelveDigitNormalForm; } }
        public override bool IsCompany { get { return false; } }
        public override string CountryTwoLetterIsoCode { get { return "SE"; } }
    }
}