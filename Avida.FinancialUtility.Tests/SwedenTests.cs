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
    public class SwedenTests
    {
        [TestMethod]
        public void TestAge()
        {
            var nr = new CivicRegNumberSe("19790717-9191");
            var age1 = CivicRegNumberSe.GetAgeInYears(nr, new DateTime(2015, 7, 16));
            Assert.AreEqual(35, age1);

            var age2 = CivicRegNumberSe.GetAgeInYears(nr, new DateTime(2015, 7, 17));
            Assert.AreEqual(36, age2);

            Assert.AreEqual(nr.DateOfBirth, new DateTime(1979, 7, 17));

        }
    }
}
