using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avida.FinancialUtility.NationalIdentification
{
    public class CivicRegNumberNo : NationalIdentificationNumber, IComparable<CivicRegNumberNo>, IEquatable<CivicRegNumberNo>
    {
        /// <summary>
        /// Declare a string which represents a single 11 digit civic registration number.
        /// </summary>
        private string nr;

        /// <summary>
        /// Century ranges for norwegian individual numbers. 
        /// Based on KITH-standard in Norway (http://kith.no/upload/5588/KITH1001-2010_Identifikatorer-for-personer_v1.pdf)
        /// </summary>
        private static readonly List<(int Start, int End, string Century)> centuryRanges = 
            new List<(int Start, int End, string Century)>
                {
                    (0, 499, "19"),   // individual numbers between years 1900-1999
                    (500, 749, "18"), // individual numbers between years 1854-1899
                    (900, 999, "19"), // individual numbers between years 1940-1999
                    (500, 999, "20")  // individual numbers between years 2000-2039
                };

        public override string ToString()
        {
            return this.nr;
        }

        public int CompareTo(CivicRegNumberNo other)
        {
            return this.nr.CompareTo(other.nr);
        }

        public override bool Equals(object obj)
        {
            var o = obj as CivicRegNumberNo;
            if (o == null)
                return base.Equals(obj);
            return Equals(o);
        }

        public bool Equals(CivicRegNumberNo other)
        {
            if (other == null)
                return base.Equals(other);

            return this.nr.Equals(other.nr);
        }

        public override int GetHashCode()
        {
            return this.nr.GetHashCode();
        }        

        /// <summary>
        /// Not all valid norwegian civic numbers have valid birthdates.
        /// </summary>
        public DateTime? BirthDateIfValid
        {
            get
            {
                return ParseBirthDateFromNr(this.nr);
            }
        }

        public override string NormalForm
        {
            get { return this.nr; }
        }

        public override bool IsCompany
        {
            get { return false; }
        }

        public override string CountryTwoLetterIsoCode
        {
            get { return "NO"; }
        }

        public static bool IsValidCivicRegNumberNo(string s)
        {
            CivicRegNumberNo _;
            return TryParse(s, out _);
        }

        private CivicRegNumberNo(string nr)
        {
            this.nr = nr;
        }

        public static bool TryParse(string nr, out CivicRegNumberNo result, bool forceCorrectBirthDate = false)
        {
            //Structure: DDMMYYIISCC
            //DDMMYY = Birth Date
            //II = Individual number
            //S = Male/Female
            //CC = Check digits
            
            result = null;

            var digits = (nr ?? "").Where(Char.IsDigit).ToArray();
            if (digits.Length != 11)
                return false;

            var digitsStr = new string(digits);
            var n = digits.Select(x => int.Parse(Char.ToString(x))).ToArray();

            if(forceCorrectBirthDate)
            {
                //Unfortunately date valiation cannot be done in general since norway had the brilliant idea of issuing non valid dates to people when the numbers stared running out.
                if (!ParseBirthDateFromNr(digitsStr).HasValue)
                    return false;
            }

            var k1 = 11 - ((3 * n[0] + 7 * n[1] + 6 * n[2] + 1 * n[3] + 8 * n[4] + 9 * n[5] + 4 * n[6] + 5 * n[7] + 2 * n[8]) % 11);
            k1 = k1 == 11 ? 0 : k1;

            var k2 = 11 - ((5*n[0] + 4*n[1] + 3*n[2] + 2*n[3] + 7*n[4] + 6*n[5] + 5*n[6] + 4*n[7] + 3*n[8] + 2*k1) % 11);
            k2 = k2 == 11 ? 0 : k2;
            if(k1 == 10 || k2 == 10)
                return false;

            if (n[9] != k1 || n[10] != k2)
                return false;

            result = new CivicRegNumberNo(digitsStr);
            return true;
        }

        /// <summary>
        /// Returns century of a individual number.
        /// 
        /// ddMMyyiiicc
        ///    ___^^^_____________
        ///    |individual number|
        ///    
        /// 
        /// </summary>
        /// <param name="individualDigits"> The individual digits of the social security number </param>
        /// <returns></returns>
        private static string GetCenturyOfIndividualDigits(string individualDigits)
        {
            int iDig = Int32.Parse(individualDigits);
            for(int i = 0; i < centuryRanges.Count; ++i)
            {
                if(iDig >= centuryRanges[i].Start && iDig <= centuryRanges[i].End)
                {
                    return centuryRanges[i].Century;
                }
            }

            return null;
        }

        private static DateTime? ParseBirthDateFromNr(string nr)
        {
            String century = GetCenturyOfIndividualDigits(nr.Substring(6, 3));
            String birthdate = nr.Substring(0, 4) + century + nr.Substring(4, 2);
            DateTime d;
            
            if (!DateTime.TryParseExact(birthdate, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return null;
            return d;
        }


        public static int GetAgeInYears(CivicRegNumberNo n, DateTime now)
        {
            if (n.BirthDateIfValid.HasValue)
            {
                var dob = n.BirthDateIfValid.Value;
                int currentYear = now.Year;
                int currentMonth = now.Month;
                int currentDay = now.Day;

                if (currentYear < dob.Year)
                    throw new ArgumentException("This date is in the future");
                var age = currentYear - dob.Year;

                if (currentMonth == dob.Month && currentDay < dob.Day)
                {
                    age = age - 1;
                }
                else if (currentMonth < dob.Month)
                {
                    age = age - 1;
                }

                return age;
            }
            else
            {
                throw new Exception("BirthDate is not valid");
            }
        }

        public int AgeInYears
        {
            get
            {
                return GetAgeInYears(this, DateTime.Today);
            }
        }
    }
}
