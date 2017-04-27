using Avida.FinancialUtility.Bank.No;
using Avida.FinancialUtility.NationalIdentification;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avida.FinancialUtility.Tests
{
    [TestClass]
    public class NorwayTests
    {
        [TestMethod]
        public void TestWikipediaExamplePerson()
        {
            //From: http://no.wikipedia.org/wiki/F%C3%B8dselsnummer
            CivicRegNumberNo n1;
            Assert.IsTrue(CivicRegNumberNo.TryParse("11077941012", out n1, forceCorrectBirthDate:true));
            Assert.AreEqual(new DateTime(1979, 7, 11), n1.BirthDateIfValid.Value);
        }

        [TestMethod]
        public void TestWikipediaExampleCompany()
        {
            //From: http://www.brreg.no/samordning/organisasjonsnummer.html
            OrganisationNumberNo n1;
            Assert.IsTrue(OrganisationNumberNo.TryParse("123456785", out n1));            
        }
    }
}