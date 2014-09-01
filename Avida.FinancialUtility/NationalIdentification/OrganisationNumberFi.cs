using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avida.FinancialUtility.Extensions;

namespace Avida.FinancialUtility.NationalIdentification
{
    public class OrganisationNumberFi : NationalIdentificationNumber, IEquatable<OrganisationNumberFi>, IComparable<OrganisationNumberFi>
    {
        /// <summary>
        /// Declare a string which represents a single 9 digit organisationnumber.
        /// </summary>
        private string nr;

        public static bool IsValidOrganisationNumberFi(string s)
        {
            OrganisationNumberFi _;
            string __;
            return TryParse(s, out _, out __);
        }

        public static bool TryParse(string nr, out OrganisationNumberFi result, out string errorMessage)
        {
            result = null;
            nr = new string(nr.Where(Char.IsDigit).ToArray());
            if (nr.Length != 8)
            {
                errorMessage = "Invalid string length";
                return false;
            }

            var checkSum = ComputeChecksum(nr.Left(7));
            if (checkSum != int.Parse(nr.Right(1)))
            {
                errorMessage = "Invalid checksum";
                return false;
            }

            var r = new OrganisationNumberFi();
            r.nr = nr.Left(7) + "-" + nr.Right(1);
            result = r;
            errorMessage = null;
            return true;
        }

        private OrganisationNumberFi()
        {

        }

        public OrganisationNumberFi(string nr)
        {
            string errorMessage;
            OrganisationNumberFi n;
            if (!TryParse(nr, out n, out errorMessage))
                throw new ArgumentException(errorMessage, "nr");

            this.nr = n.nr;
        }

        public string AsitisComparisonForm //Asitis seems to be removing the dash: 12345678
        {
            get { return new string(nr.Where(char.IsDigit).ToArray()); }
        }

        public string DatabaseNormalForm //1234567-8
        {
            get { return nr; }
        }

        /// <summary>
        /// Returns a string representation of the number value of this object.
        /// </summary>
        /// <returns>A System.String representing the number value of this object.</returns>
        public override string ToString()
        {
            return DatabaseNormalForm;
        }

        private static readonly int[] CheckDigitMultipliers = new int[] { 7, 9, 10, 5, 8, 4, 2 };

        /// <summary>
        /// Computes the check sum for a civic registration number.
        /// See: http://www.finlex.fi/sv/laki/ajantasa/2001/20010288
        /// 
        /// Finnish orgnrs are called FO-nummer in swedish (företags/organisationsnummer)
        /// 
        /// Reading the specs they seem to imply that checkdigit = 1 means reserved for testing.
        /// </summary>
        /// <param name="value">The value to compute a check sum for.</param>
        /// <returns>An integer value.</returns>
        public static int ComputeChecksum(string value)
        {
            var m = value
                .Select(x => int.Parse(new string(new[] { x })))
                .Zip(CheckDigitMultipliers, (digit, multiplier) => digit * multiplier)
                .Sum() % 11;

            return m == 0 ? 0 : 11 - m;
        }

        public int CompareTo(OrganisationNumberFi other)
        {
            return DatabaseNormalForm.CompareTo(other.DatabaseNormalForm);
        }

        public override bool Equals(object obj)
        {
            var o = obj as OrganisationNumberFi;
            if (o == null)
                return base.Equals(obj);
            return Equals(o);
        }

        public bool Equals(OrganisationNumberFi other)
        {
            if (other == null)
                return base.Equals(other);

            return DatabaseNormalForm.Equals(other.DatabaseNormalForm);
        }

        public override int GetHashCode()
        {
            return DatabaseNormalForm.GetHashCode();
        }

        public override string NormalForm { get { return this.DatabaseNormalForm; } }
        public override bool IsCompany { get { return true; } }
        public override string CountryTwoLetterIsoCode { get { return "FI"; } }
    }
}
