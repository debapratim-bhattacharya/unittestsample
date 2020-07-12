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
    public class ProgramRequestTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;

        private ProgramRequest programRequest;

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

        private DataSet MockDataSet()
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(MockDataTable());

            return ds;
        }

        [SetUp]
        public void SetUp()
        {
            mockExceptionHandling = new Mock<IExceptionHandling>();

            mockDataAccess = new Mock<IDataAccess>();

            mockExceptionHandling
                .Setup(x => x.LogException(It.IsAny<Exception>(), ExceptionPolicy.Web_Exception))
                .Verifiable();

            programRequest = new ProgramRequest(mockDataAccess.Object, mockExceptionHandling.Object);
        }

        [Test]
        public void ChangeAidType_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMDSQReferralSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.ChangeAidType(10,1,10));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void ChangeAidType_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMDSQReferralSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                    }))
                .Returns(1);

            //Act
            programRequest.ChangeAidType(10,1,1);

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMDSQReferralSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void ChangeCPAmount_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestChangeCPAmounts",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.ChangeCPAmount(10,1,100,100,DateTime.Now));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(0)]
        public void ChangeCPAmount_WhenExecuted_ShouldReturnExpectedReturnValue(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestChangeCPAmounts",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngProgramRequestID", expected));
                    }))
                .Returns(1);

            //Act
            var response = programRequest.ChangeCPAmount(10, 1, 100, 100, DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestChangeCPAmounts",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void ConsumerEnteringLeavingFacility_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestChangeCPAmounts",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.ConsumerEnteringLeavingFacility(10, 1, DateTime.Now, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void ConsumerEnteringLeavingFacility_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestChangeCPAmounts",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                    }))
                .Returns(1);

            //Act
            programRequest.ConsumerEnteringLeavingFacility(10, 1, DateTime.Now, 1, 1);

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestChangeCPAmounts",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void DeleteProgramRequest_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.DeleteProgramRequest(10, 1, "someString", "someString"));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void DeleteProgramRequest_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                    }))
                .Returns(1);

            //Act
            programRequest.DeleteProgramRequest(10, 1, "someString","someString");

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void GetBeginCodes_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISBeginCodesGet",
                    It.IsAny<CommandType>(),null))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetBeginCodes());

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetBeginCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISBeginCodesGet",
                    It.IsAny<CommandType>(), null))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetBeginCodes();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISBeginCodesGet",
                    It.IsAny<CommandType>(), null),
                    Times.Once);                
        }

        [Test]
        public void GetDCNs_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDCNsGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetDCNs(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetDCNs_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDCNsGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetDCNs(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDCNsGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetProgramRequest_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetProgramRequest(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetProgramRequest_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetProgramRequest(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetProgramRequestDiagnoses_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestDiagnosesGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetProgramRequestDiagnoses(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetProgramRequestDiagnoses_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestDiagnosesGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetProgramRequestDiagnoses(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestDiagnosesGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetProgramRequests_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestsGet",
                    It.IsAny<CommandType>(), 
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetProgramRequests("someString"));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetProgramRequests_WhenExecuted_ShouldReturnDataSet()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestsGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataSet);

            //Act
            var response = programRequest.GetProgramRequests("someString");

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataSet>(response);

            mockDataAccess
                .Verify(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestsGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetProgramRequestTierHistory_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestTierHistoryGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetProgramRequestTierHistory(1,DateTime.Now, DateTime.Now));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetProgramRequestTierHistory_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestTierHistoryGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetProgramRequestTierHistory(1,DateTime.Now, DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestTierHistoryGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetRequestors_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISRequestorsGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetRequestors(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetRequestors_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISRequestorsGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetRequestors(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISRequestorsGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetStatusCodes_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISStatusCodesGet",
                    It.IsAny<CommandType>(), null))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetStatusCodes());

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetStatusCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISStatusCodesGet",
                    It.IsAny<CommandType>(), null))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetStatusCodes();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISStatusCodesGet",
                    It.IsAny<CommandType>(), null),
                    Times.Once);
        }

        [Test]
        public void GetTCMServiceAuthorization_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISTCMServiceAuthorizationGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetTCMServiceAuthorization(1,1,"id",DateTime.Now));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetTCMServiceAuthorization_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISTCMServiceAuthorizationGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetTCMServiceAuthorization(1,1,"idType", DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISTCMServiceAuthorizationGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetTerminationCodes_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISTerminationCodesGet",
                    It.IsAny<CommandType>(), null))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.GetTerminationCodes());

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetTerminationCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISTerminationCodesGet",
                    It.IsAny<CommandType>(), null))
                .Returns(MockDataTable);

            //Act
            var response = programRequest.GetTerminationCodes();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISTerminationCodesGet",
                    It.IsAny<CommandType>(), null),
                    Times.Once);
        }

        [Test]
        public void HospiceNFTransfer_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestHospiceNFTransfer",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.HospiceNFTransfer(10, 1, DateTime.Now));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(0)]
        public void HospiceNFTransfer_WhenExecuted_ShouldReturnExpectedReturnValue(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestHospiceNFTransfer",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngProgramRequestID", expected));
                    }))
                .Returns(1);

            //Act
            var response = programRequest.HospiceNFTransfer(10, 1, DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramRequestHospiceNFTransfer",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveProgramRequest_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => programRequest.SaveProgramRequest(new Fixture().Create<SaveProgramRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveProgramRequest_WhenProviderIdIsFound_ShouldReturnValidateProviderNumber(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intProviderFound", expected));
                    }))
                .Returns(1);

            //Act
            var response = programRequest.SaveProgramRequest(new Fixture().Create<SaveProgramRequest>());

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual("Validate Provider Number", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveProgramRequest_WhenProviderIdIsNot1Or2_ShouldReturnProgramRequestId(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intProgramRequestID", expected));
                    }))
                .Returns(1);

            //Act
            var response = programRequest.SaveProgramRequest(new Fixture().Create<SaveProgramRequest>());

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(expected.ToString(), response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }
    }
}
