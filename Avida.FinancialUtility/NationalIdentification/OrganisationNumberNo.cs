using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avida.FinancialUtility.Extensions;

namespace Avida.FinancialUtility.NationalIdentification
{
    public class OrganisationNumberNo : NationalIdentificationNumber, IEquatable<OrganisationNumberNo>, IComparable<OrganisationNumberNo>
    {
        /// <summary>
        /// Declare a string which represents a single 9 digit organisationnumber.
        /// </summary>
        private string nr;

        public static bool IsValidOrganisationNumberNo(string s)
        {
            OrganisationNumberNo _;
            return TryParse(s, out _);
        }

        public static bool TryParse(string nr, out OrganisationNumberNo result)
        {
            //Format: http://www.brreg.no/samordning/organisasjonsnummer.html
            result = null;

            var digits = (nr ?? "").Where(Char.IsDigit).ToArray();
            if (digits.Length != 9)
                return false;

            var digitsStr = new string(digits);
            var n = digits.Select(x => int.Parse(Char.ToString(x))).ToArray();
            var k = GetControlDigit(nr);
            if (k >= 10)
                return false;
            if (k != n[8])
                return false;

            result = new OrganisationNumberNo(digitsStr);
            return true;
        }

        private static int GetControlDigit(string orgnr)
        {
            int sumForMod = 0;
            int controlNumber = 2;
            char[] knr = orgnr.ToCharArray();

            for (int i = knr.Length - 2; i >= 0; i--)
            {
                sumForMod += (int)(char.GetNumericValue(knr[i]) * controlNumber);
                controlNumber++;
                if (controlNumber > 7)
                {
                    controlNumber = 2;
                }
            }
            int calculus = (11 - sumForMod % 11);
            if (calculus == 11)
            {
                return 0;
            }
            return calculus;
        }

        private OrganisationNumberNo(string nr)
        {
            this.nr = nr;
        }
        
        /// <summary>
        /// Returns a string representation of the number value of this object.
        /// </summary>
        /// <returns>A System.String representing the number value of this object.</returns>
        public override string ToString()
        {
            return this.NormalForm;
        }
                
        public int CompareTo(OrganisationNumberNo other)
        {
            return this.NormalForm.CompareTo(other.NormalForm);
        }

        public override bool Equals(object obj)
        {
            var o = obj as OrganisationNumberNo;
            if (o == null)
                return base.Equals(obj);
            return Equals(o);
        }

        public bool Equals(OrganisationNumberNo other)
        {
            if (other == null)
                return base.Equals(other);

            return this.NormalForm.Equals(other.NormalForm);
        }

        public override int GetHashCode()
        {
            return this.NormalForm.GetHashCode();
        }

        public override string NormalForm { get { return this.nr; } }
        public override bool IsCompany { get { return true; } }
        public override string CountryTwoLetterIsoCode { get { return "NO"; } }
    }
}
