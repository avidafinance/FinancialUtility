using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avida.FinancialUtility.Extensions;

namespace Avida.FinancialUtility.NationalIdentification
{
    public class IndividualDateOfBirth : NationalIdentificationNumber
    {
        public override string NormalForm { get { return this.SixDigitDateOfBirth; } }
        public override bool IsCompany { get { return false; } }
        public override string CountryTwoLetterIsoCode { get { return "SE"; } }
        public string SixDigitDateOfBirth
        {
            get { return nr; }
        }

        private string nr;
        private IndividualDateOfBirth()
        {
        }

        public static bool TryParse(string nr, out IndividualDateOfBirth result, out string errorMessage)
        {
            result = null;
            nr = nr.RemoveAll(" ", "-") ?? "";

            if (nr.Length != 6)
            {
                errorMessage = "Invalid string length";
                return false;
            }

            if (!IsNumeric(nr))
            {
                errorMessage = "Date of birth does not consist of only numeric values";
                return false;
            }


            var dob = new IndividualDateOfBirth();
            dob.nr = nr;
            result = dob;
            errorMessage = null;
            return true;
        }

        private static bool IsNumeric(string value)
        {
            return !value.Any(c => !Char.IsDigit(c));
        }
    }
}
