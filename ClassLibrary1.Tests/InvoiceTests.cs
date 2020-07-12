using AutoFixture;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1.Tests
{
    [TestFixture(Category = "Unit")]
    public class InvoiceTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;

        private Invoice invoice;

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

            invoice = new Invoice(mockDataAccess.Object, mockExceptionHandling.Object);
        }

        [TestCase(0,1)]
        [TestCase(1,2)]
        public void DeleteInvoice_WhenDbErrorsOut_ShouldThrowException(int invoiceId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.DeleteInvoice(invoiceId, sessionId));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        public void DeleteInvoice_WhenExecuted_ShouldReturnString(int invoiceId, long sessionId)
        {
            //Arrange
            var expected = "someOutput";

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceDelete",
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
            var response = invoice.DeleteInvoice(invoiceId, sessionId);

            //Assert
            Assert.IsInstanceOf<string>(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetInvoiceByInvoiceId_WhenDbErrorsOut_ShouldThrowException(int invoiceId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceByInvoiceIDGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.GetInvoiceByInvoiceId(invoiceId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetInvoiceByInvoiceId_WhenExecuted_ShouldReturnDataTable(int invoiceId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceByInvoiceIDGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = invoice.GetInvoiceByInvoiceId(invoiceId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceByInvoiceIDGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0, "06/06/2019")]
        [TestCase(1, "SomeFiscalYear")]
        public void GetInvoiceByServicePlanId_WhenDbErrorsOut_ShouldThrowException(int servicePlanId, string fiscalYearInd)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceByServicePlanIDGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.GetInvoiceByServicePlanId(servicePlanId, fiscalYearInd));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, "06/06/2019")]
        [TestCase(1, "SomeFiscalYear")]
        public void GetInvoiceByServicePlanId_WhenExecuted_ShouldReturnDataTable(int servicePlanId, string fiscalYearInd)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceByServicePlanIDGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = invoice.GetInvoiceByServicePlanId(servicePlanId, fiscalYearInd);

            //Assert
            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceByServicePlanIDGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        public void GetInvoiceByLineItems_WhenDbErrorsOut_ShouldThrowException(
            int invoiceId, int iSisServicePlanId)
        {
            //Arrange
            DateTime fiscalYearBeginDate = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceLineItemsGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.GetInvoiceByLineItems(invoiceId, iSisServicePlanId, fiscalYearBeginDate));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        public void GetInvoiceByLineItems_WhenExecuted_ShouldReturnDataTable(
            int invoiceId, int iSisServicePlanId)
        {
            //Arrange
            DateTime fiscalYearBeginDate = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceLineItemsGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = invoice.GetInvoiceByLineItems(invoiceId, iSisServicePlanId, fiscalYearBeginDate);

            //Assert
            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceLineItemsGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        public void GetInvoiceTotalCapAmountForMonth_WhenDbErrorsOut_ShouldThrowException(
            int invoiceId, int servicePlanId)
        {
            //Arrange
            DateTime dteCurrentMonth = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoicesCapAmountForMonth",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.GetInvoiceTotalCapAmountForMonth(invoiceId, servicePlanId, dteCurrentMonth));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        public void GetInvoiceTotalCapAmountForMonth_WhenExecuted_ShouldReturnDataTable(
            int invoiceId, int servicePlanId)
        {
            //Arrange
            DateTime dteCurrentMonth = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoicesCapAmountForMonth",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = invoice.GetInvoiceTotalCapAmountForMonth(invoiceId, servicePlanId, dteCurrentMonth);

            //Assert
            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoicesCapAmountForMonth",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetServiceSpansAlreadyonInvoice_WhenDbErrorsOut_ShouldThrowException(
            int serviceSpanId)
        {
            //Arrange
            DateTime billDate = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_ServiceSpansAlreadyOnInvoiceGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.GetServiceSpansAlreadyonInvoice(serviceSpanId, billDate));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetServiceSpansAlreadyonInvoice_WhenExecuted_ShouldReturnDataTable(
            int serviceSpanId)
        {
            //Arrange
            DateTime billDate = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_ServiceSpansAlreadyOnInvoiceGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = invoice.GetServiceSpansAlreadyonInvoice(serviceSpanId, billDate);

            //Assert
            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_ServiceSpansAlreadyOnInvoiceGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetServiceSpansforInvoice_WhenDbErrorsOut_ShouldThrowException(
            int serviceSpanId)
        {
            //Arrange
            DateTime fiscalYearBeginDate = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_ServiceSpansforInvoiceGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.GetServiceSpansforInvoice(serviceSpanId, fiscalYearBeginDate));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetServiceSpansforInvoice_WhenExecuted_ShouldReturnDataTable(
            int serviceSpanId)
        {
            //Arrange
            DateTime fiscalYearBeginDate = DateTime.Now;

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_ServiceSpansforInvoiceGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = invoice.GetServiceSpansforInvoice(serviceSpanId, fiscalYearBeginDate);

            //Assert
            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "ihhrc.prc_ServiceSpansforInvoiceGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        public void VoidInvoice_WhenDbErrorsOut_ShouldThrowException(int invoiceId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceVoid",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.VoidInvoice(invoiceId, sessionId));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        public void VoidInvoice_WhenExecuted_ShouldReturnString(int invoiceId, long sessionId)
        {
            //Arrange
            var expected = "someOutput";

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceVoid",
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
            var response = invoice.VoidInvoice(invoiceId, sessionId);

            //Assert
            Assert.IsInstanceOf<string>(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceVoid",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveInvoice_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.SaveInvoice(new Fixture().Create<InvoiceRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void SaveInvoice_WhenExecuted_ShouldReturnString(int intReturn)
        {
            //Arrange
            var expectedInvoiceId = 100;

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intReturn", intReturn));
                        target.Add(new SqlParameter("@lngInvoiceID", expectedInvoiceId));
                    }))
                .Returns(1);

            //Act
            var response = invoice.SaveInvoice(new Fixture().Create<InvoiceRequest>());

            //Assert
            Assert.IsInstanceOf<string>(response);

            if(intReturn != 0)
                Assert.AreEqual(intReturn + "|-1", response);
            else
                Assert.AreEqual(intReturn + "|" + expectedInvoiceId, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveInvoiceLineItems_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceLineItemsSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => invoice.SaveInvoiceLineItems(new Fixture().Create<InvoiceLineItemRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        public void SaveInvoiceLineItems_WhenExecuted_ShouldReturnString(int intReturn)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceLineItemsSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intReturn", intReturn));
                    }))
                .Returns(1);

            //Act
            var response = invoice.SaveInvoiceLineItems(new Fixture().Create<InvoiceLineItemRequest>());

            //Assert
            Assert.IsInstanceOf<string>(response);

            if (intReturn == -1)
                Assert.AreEqual(intReturn + "|-1", response);
            else
                Assert.AreEqual(intReturn + "|0", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "ihhrc.prc_InvoiceLineItemsSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }
    }
}