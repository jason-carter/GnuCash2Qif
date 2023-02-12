using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GnuCash.Sql2Qif.Library.DTO.Tests
{
    [TestClass()]
    public class AccountTests
    {
        [TestMethod()]
        [DataRow("45AE4CF7-D3D4-4C52-BCCA-FEF031DFD365", "Asset Account Name", "Asset Account Description", "Asset")]
        [DataRow("45AE4CF7-D3D4-4C52-BCCA-FEF031DFD365", "Credit Account Name", "Credit Account Description", "Credit")]
        [DataRow("45AE4CF7-D3D4-4C52-BCCA-FEF031DFD365", "Bank Account Name", "Bank Account Description", "Bank")]
        public void IsAccountTest(string guid, string name, string description, string accountType)
        {
            // Arrange + Act
            Account acc = new(guid, name, description, accountType);

            // Assert
            Assert.AreEqual(acc.IsAccount, true);
        }

        [TestMethod()]
        [DataRow("45AE4CF7-D3D4-4C52-BCCA-FEF031DFD365", "Expense Category Name", "Expense Category Description", "Expense")]
        [DataRow("45AE4CF7-D3D4-4C52-BCCA-FEF031DFD365", "Income Category Name", "Income Category Description", "Income")]
        public void IsCategoryTest(string guid, string name, string description, string accountType)
        {
            // Arrange + Act
            Account acc = new(guid, name, description, accountType);

            // Assert
            Assert.AreEqual(acc.IsCategory, true);
        }

        [TestMethod()]
        [DataRow("45AE4CF7-D3D4-4C52-BCCA-FEF031DFD365", "Asset Account Name", "Asset Account Description", "Asset")]
        public void ToStringTest(string guid, string name, string description, string accountType)
        {
            // Arrange + Act
            Account acc = new(guid, name, description, accountType);

            // Assert
            Assert.AreEqual(acc.ToString(), $"{accountType} / {name}");
        }
    }
}
