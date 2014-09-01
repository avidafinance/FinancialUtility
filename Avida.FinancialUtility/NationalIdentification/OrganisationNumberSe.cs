using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avida.FinancialUtility.Extensions;

namespace Avida.FinancialUtility.NationalIdentification
{
    public class OrganisationNumberSe : NationalIdentificationNumber, IEquatable<OrganisationNumberSe>, IComparable<OrganisationNumberSe>
    {
        /// <summary>
        /// Declare a string which represents a single 10 digit organisationnumber.
        /// </summary>
        private string nr;

        public static bool IsValidOrganisationNumberSe(string s)
        {
            OrganisationNumberSe _;
            string __;
            return TryParse(s, out _, out __);
        }

        public static bool TryParse(string nr, out OrganisationNumberSe result, out string errorMessage)
        {
            result = null;
            nr = nr.RemoveAll(" ", "-") ?? "";
            if (nr.Length != 10)
            {
                errorMessage = "Invalid string length";
                return false;
            }

            if (!IsNumeric(nr))
            {
                errorMessage = "Invalid character(s) in value";
                return false;
            }
            if (!IsValidChecksum(nr))
            {
                errorMessage = "Invalid checksum";
                return false;
            }

            //NOTE: We cant force the month to be >=20 since there are companies owned by single people where the orgnr = civic regnr.

            var r = new OrganisationNumberSe();
            r.nr = nr;
            result = r;
            errorMessage = null;
            return true;
        }

        private OrganisationNumberSe()
        {

        }

        public OrganisationNumberSe(string nr)
        {
            string errorMessage;
            OrganisationNumberSe n;
            if (!TryParse(nr, out n, out errorMessage))
                throw new ArgumentException(errorMessage, "nr");

            this.nr = n.nr;
        }

        public string TenDigitNormalForm
        {
            get { return nr; }
        }

        public string TaxOfficeNormalForm
        {
            //When registered in the same database as civic reg numbers the year is ofthen set to 16 for orgnrs.
            get { return "16" + nr; }
        }

        /// <summary>
        /// Returns a string representation of the number value of this object.
        /// </summary>
        /// <returns>A System.String representing the number value of this object.</returns>
        public override string ToString()
        {
            return TenDigitNormalForm;
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

        public int CompareTo(OrganisationNumberSe other)
        {
            return TenDigitNormalForm.CompareTo(other.TenDigitNormalForm);
        }

        public override bool Equals(object obj)
        {
            var o = obj as OrganisationNumberSe;
            if (o == null)
                return base.Equals(obj);
            return Equals(o);
        }

        public bool Equals(OrganisationNumberSe other)
        {
            if (other == null)
                return base.Equals(other);

            return TenDigitNormalForm.Equals(other.TenDigitNormalForm);
        }

        public override int GetHashCode()
        {
            return TenDigitNormalForm.GetHashCode();
        }

        public override string NormalForm { get { return this.TenDigitNormalForm; } }
        public override bool IsCompany { get { return true; } }
        public override string CountryTwoLetterIsoCode { get { return "SE"; } }
    }
}
