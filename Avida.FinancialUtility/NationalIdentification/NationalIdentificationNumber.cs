using System;

namespace Avida.FinancialUtility.NationalIdentification
{
    public abstract class NationalIdentificationNumber
    {
        public abstract string NormalForm { get; }
        public abstract bool IsCompany { get; }
        public abstract string CountryTwoLetterIsoCode { get; }
    }
}
