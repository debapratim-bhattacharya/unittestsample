using AutoFixture;
using ClassLibrary1.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1.Tests
{
    [TestFixture(Category = "Unit")]
    public class CashoutServiceTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;

        private CashoutService cashoutService;

        delegate void MockRefCallback(
            string connectionString,
            string procName,
            CommandType commandType,
            SqlParameter[] sqlParameters,
            ref List<SqlParameter> outParameters);

        private DataTable MockDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("err_Number", typeof(string));
            table.Columns.Add("err_description", typeof(string));
            DataRow dataRow = table.NewRow();
            dataRow["err_Number"] = "Some error number";
            dataRow["err_description"] = "Some error description";
            table.Rows.Add(dataRow);

            return table;
        }

        [SetUp]
        public void SetUp()
        {
            mockExceptionHandling = new Mock<IExceptionHandling>();

            mockDataAccess = new Mock<IDataAccess>();

            mockExceptionHandling
                .Setup(x => x.LogException(It.IsAny<Exception>(), ExceptionPolicy.Web_Exception))
                .Verifiable();

            cashoutService = new CashoutService(mockDataAccess.Object, mockExceptionHandling.Object);
        }

        [Test]
        public void CashOutServices_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanCashOutSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.CashOutServices(
                new Fixture().Create<CashoutServiceRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void CashOutServices_WhenExecuted_ShouldExecute()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanCashOutSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intReturn", 1));
                    }))
                .Returns(1);

            //Act
            cashoutService.CashOutServices(
                new Fixture().Create<CashoutServiceRequest>());

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanCashOutSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void CreateCashOutCCO_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOOverlappingServicesEdit",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.CreateCashOutCCO(
                new Fixture().Create<CashoutCCORequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void CreateCashOutCCO_WhenOverlappingServiceEditReturnIsNotMinusOne_ShouldReturnResponseWithoutInvokingCashoutCCOSave(
            int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOOverlappingServicesEdit",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intReturn", expected));
                    }))
                .Returns(1);

            //Act
            var response = cashoutService.CreateCashOutCCO(
                new Fixture().Create<CashoutCCORequest>());

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual($"{expected}|{-1}", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOOverlappingServicesEdit",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOCashOutServiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Never);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void CreateCashOutCCO_WhenOverlappingServiceEditReturnIsMinusOne_ShouldInvokeCashoutCCOSave(
            int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOOverlappingServicesEdit",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intReturn", -1));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOCashOutServiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intCCOServiceSpanID", expected));
                    }))
                .Returns(1);

            //Act
            var response = cashoutService.CreateCashOutCCO(
                new Fixture().Create<CashoutCCORequest>());

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual($"{0}|{expected}", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOOverlappingServicesEdit",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOCashOutServiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCCOServicesCashOut_WhenDbErrorsOut_ShouldThrowException(int ccoServiceSpanId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOServicesCashOutGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetCCOServicesCashOut(ccoServiceSpanId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCCOServicesCashOut_WhenExecuted_ShouldReturnDataTable(int ccoServiceSpanId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOServicesCashOutGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetCCOServicesCashOut(ccoServiceSpanId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOServicesCashOutGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetCCOTransactionsByStateID_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOTransactionsGetByStateID",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetCCOTransactionsByStateID(
                "someId", DateTime.Now, DateTime.Now));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetCCOTransactionsByStateID_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOTransactionsGetByStateID",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetCCOTransactionsByStateID(
                "someId", DateTime.Now, DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOTransactionsGetByStateID",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetConsumerBudgets_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOBudgetsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetConsumerBudgets(1,1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetConsumerBudgets_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOBudgetsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetConsumerBudgets(1,1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOBudgetsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetConsumerBudgetTotals_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOConsumerBudgetTotal",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetConsumerBudgetTotals(1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetConsumerBudgetTotals_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOConsumerBudgetTotal",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetConsumerBudgetTotals(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOConsumerBudgetTotal",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetMFPBudgets_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetMFPBudgets(1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetMFPBudgets_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetMFPBudgets(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetMFPBudget_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetMFPBudget(1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetMFPBudget_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetMFPBudget(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetMFPConsumerBudgetTotal_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPConsumerBudgetTotalGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetMFPConsumerBudgetTotal(1,1,1,DateTime.Now));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetMFPConsumerBudgetTotal_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPConsumerBudgetTotalGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetMFPConsumerBudgetTotal(1, 1, 1, DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPConsumerBudgetTotalGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetMFPTransactions_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPTransactionsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetMFPTransactions(1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetMFPTransactions_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPTransactionsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetMFPTransactions(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISMFPTransactionsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetNMCategories_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISNMCategoriesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.GetNMCategories(1,1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetNMCategories_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISNMCategoriesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = cashoutService.GetNMCategories(1,1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISNMCategoriesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void SaveMFPBudget_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.SaveMFPBudget(
                1, "Some", 1, 1, "some", DateTime.Now, DateTime.Now, 10, 1000));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveMFPBudget_WhenExecuted_ShouldExecute()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intReturn", 1));
                    }))
                .Returns(1);

            //Act
            cashoutService.SaveMFPBudget(1, "Some", 1, 1, "some", DateTime.Now, DateTime.Now, 10, 1000);

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMFPBudgetSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveMFPConsumerBudgetTotal_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMFPConsumerBudgetTotalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.SaveMFPConsumerBudgetTotal(1,1,1,"stateId",DateTime.Now, DateTime.Now,10,1,1,10));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void SaveMFPConsumerBudgetTotal_WhenExecuted_ShouldReturnExpectedOutVariable(
            int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMFPConsumerBudgetTotalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intConsumerBudgetTotalID", expected));
                    }))
                .Returns(1);

            //Act
            var response = cashoutService.SaveMFPConsumerBudgetTotal(1, 1, 1, "stateId", DateTime.Now, DateTime.Now, 10, 1, 1, 10);

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMFPConsumerBudgetTotalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveNMTransactiont_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISNMTransactionSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => cashoutService.SaveNMTransaction(1, 1, "stateId", DateTime.Now, 10, "desc", 10));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveNMTransaction_WhenExecuted_ShouldExecute()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISNMTransactionSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intReturn", 1));
                    }))
                .Returns(1);

            //Act
            cashoutService.SaveNMTransaction(1, 1, "stateId", DateTime.Now, 10, "desc", 10);

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISNMTransactionSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }
    }
}
