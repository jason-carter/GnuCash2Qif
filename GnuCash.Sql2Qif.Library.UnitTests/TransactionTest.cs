using GnuCash.Sql2Qif.Library.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GnuCash.Sql2Qif.Library.UnitTests
{
    [TestClass]
    public class TransactionTest
    {
        [TestMethod]
        public void ToStringTest()
        {
            // Arrange
            string transactionGuid = Guid.NewGuid().ToString();
            string accountGuid = Guid.NewGuid().ToString();
            string accountName = "Test Account Name";
            DateTime datePosted = new DateTime(2020, 1, 2);
            string reference = "Test Reference";
            string description = "Test Description";
            string memo = "Test Memo";
            string isReconciled = "True";
            decimal trxValue = 0;

            // Act
            Transaction trx = new Transaction(transactionGuid,
                                                accountGuid,
                                                accountName,
                                                datePosted,
                                                reference,
                                                description,
                                                memo,
                                                isReconciled,
                                                trxValue);

            // Assert
            Assert.AreEqual(trx.ToString(), "2020-01-02 / Test Description / Test Memo");
        }
    }
}
