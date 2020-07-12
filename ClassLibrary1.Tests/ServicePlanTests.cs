using AutoFixture;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ClassLibrary1.Tests
{
    [TestFixture(Category = "Unit")]
    public class ServicePlanTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;
        private Mock<IServiceSpan> mockServiceSpan;
        private Mock<IProgramRequest> mockProgramRequest;

        private ServicePlan servicePlan;

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

        private DataTable MockDataTable(Dictionary<string, object> rows)
        {
            DataTable table = new DataTable();
            foreach (var keyValue in rows)
            {
                table.Columns.Add(keyValue.Key);
            }

            if (rows.Any())
            {

                DataRow dataRow = table.NewRow();
                foreach (var keyValue in rows)
                {
                    dataRow[keyValue.Key] = keyValue.Value;
                }

                table.Rows.Add(dataRow);
            }

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

            mockServiceSpan = new Mock<IServiceSpan>();

            mockProgramRequest = new Mock<IProgramRequest>();

            mockExceptionHandling
                .Setup(x => x.LogException(It.IsAny<Exception>(), ExceptionPolicy.Web_Exception))
                .Verifiable();

            servicePlan = new ServicePlan(
                mockDataAccess.Object, 
                mockExceptionHandling.Object, 
                mockServiceSpan.Object, 
                mockProgramRequest.Object);
        }

        [Test]
        public void DeleteServicePlan_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISServicePlanDelete",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.DeleteServicePlan(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void DeleteServicePlan_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            servicePlan.DeleteServicePlan(1, 1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);            
        }

        [Test]
        public void DeleteServicePlanException_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISServicePlanDelete_Exception",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.DeleteServicePlan(1, 1, "exceptionNumber", "exceptionComment"));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void DeleteServicePlanException_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            servicePlan.DeleteServicePlan(1, 1,"exceptionNumber","exceptionComment");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void GetActiveServicePlan_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISServicePlanActiveGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.GetActiveServicePlan(1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void GetActiveServicePlan_WhenExecuted_ShouldReturnExpected(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanActiveGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngResult", expected));
                    }))
                .Returns(1);

            //Act
            var response = servicePlan.GetActiveServicePlan(1);

            //Assert
            Assert.AreEqual(expected, response);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanActiveGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void GetEnhancedServicesSpans_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEnhancedServicesSpansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetEnhancedServicesSpans(1,""));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetEnhancedServicesSpans_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEnhancedServicesSpansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetEnhancedServicesSpans(1,"");

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEnhancedServicesSpansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetPlanEnhancedPrograms_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGetPlanEnhanced",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetPlanEnhancedPrograms());

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetPlanEnhancedPrograms_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGetPlanEnhanced",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetPlanEnhancedPrograms();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGetPlanEnhanced",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetRsDiagnosis_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISRSDiagnosisGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetRsDiagnosis(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetRsDiagnosis_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISRSDiagnosisGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetRsDiagnosis(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISRSDiagnosisGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetSecondaryDiagnosisCodes_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISSecondaryDiagnosisCodesGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetSecondaryDiagnosisCodes(1,1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetSecondaryDiagnosisCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISSecondaryDiagnosisCodesGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetSecondaryDiagnosisCodes(1,1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISSecondaryDiagnosisCodesGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetServicePlan_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetServicePlan(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetServicePlan_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetServicePlan(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetServicePlans_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetServicePlans(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetServicePlans_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetServicePlans(1, 1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetSupportBroker_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokerGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetSupportBroker("stateId", DateTime.Now, DateTime.Now));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetSupportBroker_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokerGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetSupportBroker("stateId", DateTime.Now, DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokerGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetSupportBrokers_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokersGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => servicePlan.GetSupportBrokers());

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetSupportBrokers_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokersGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = servicePlan.GetSupportBrokers();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokersGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void SaveRsDiagnosis_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISRSDiagnosisSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.SaveRsDiagnosis(new Fixture().Create<RsDiagnosisRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveRsDiagnosis_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISRSDiagnosisSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            servicePlan.SaveRsDiagnosis(new Fixture().Create<RsDiagnosisRequest>());

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISRSDiagnosisSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveSecondaryDiagnosisCodes_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISSecondaryDiagnosisCodesDelete",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.SaveSecondaryDiagnosisCodes(1, "some", "somestring"));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase("oneValue")]
        [TestCase("oneValue|doubleValue")]
        public void SaveSecondaryDiagnosisCodes_WhenExecuted_ShouldReturnExpected(string axisIDiagnosisCodes)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecondaryDiagnosisCodesDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))               
                .Returns(1);

            //Act
            servicePlan.SaveSecondaryDiagnosisCodes(1,axisIDiagnosisCodes,"somestring");

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecondaryDiagnosisCodesDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Exactly(axisIDiagnosisCodes.Split('|').Count()*2 + 1));
        }
        
        [Test]
        public void SaveSupportBroker_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISCCOSupportBrokerSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.SaveSupportBroker("stateId",10,DateTime.Now, DateTime.Now, 10));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveSupportBroker_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokerSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            servicePlan.SaveSupportBroker("stateId", 10, DateTime.Now, DateTime.Now, 10);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCCOSupportBrokerSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [TestCase("ServicePlan")]
        [TestCase("ServiceSpan")]
        public void CheckForException_WhenExceptionOccures_ThrowsException(string tableName)
        {
            //Arrange
            var procName = string.Empty;
            if (tableName.Equals("ServicePlan")) procName = "prc_ISISServicePlanExceptionCheck";
            if (tableName.Equals("ServiceSpan")) procName = "prc_ISISServiceSpanExceptionCheck";

            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     procName,
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.CheckForException(tableName, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1,"ServicePlan")]
        [TestCase(2, "ServiceSpan")]
        public void CheckForException_WhenExecuted_ShouldReturnExpected(int expected, string tableName)
        {
            //Arrange
            var procName = string.Empty;
            if (tableName.Equals("ServicePlan")) procName = "prc_ISISServicePlanExceptionCheck";
            if (tableName.Equals("ServiceSpan")) procName = "prc_ISISServiceSpanExceptionCheck";

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    procName,
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intException", expected));
                    }))
                .Returns(1);

            //Act
            var response = servicePlan.CheckForException(tableName, 1);

            //Assert
            Assert.AreEqual(expected == 1, response);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    procName,
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UpdatePlanAuthorization_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISPlanAuthorizationUpdate",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.UpdatePlanAuthorization(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void UpdatePlanAuthorization_WhenExecuted_ShouldReturnExpected(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPlanAuthorizationUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intUpdated", expected));
                    }))
                .Returns(1);

            //Act
            var response = servicePlan.UpdatePlanAuthorization(1, 1);

            //Assert
            Assert.AreEqual(expected == 1, response);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPlanAuthorizationUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void ValidatePlanMsg_WhenDbErrors_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISServicePlanExceptionCheck",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.ValidatePlanMsg(1,1,DateTime.Now, DateTime.Now,1,1,1,1,""));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void ValidatePlanMsg_WhenExceptionExist_ReturnEmptyString()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intException", 1));
                    }))
                .Returns(1);

            //Act
            var response = servicePlan.ValidatePlanMsg(1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "");

            //Assert
            Assert.IsEmpty(response);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void ValidatePlanMsg_WhenBeginDateIsLessThanPrBeginDate_Return204()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(1) }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "");

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "204");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("ARO")]
        [TestCase("Remedial Services")]
        [TestCase("Habilitation Services")]
        public void ValidatePlanMsg_WhenProgramNameMatchAndLevelCodeEmpty_Return201(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-1) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "");

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "201");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("ARO")]
        [TestCase("Remedial Services")]
        [TestCase("Habilitation Services")]
        public void ValidatePlanMsg_WhenLocEffectiveDateSpanMoreThan365Days_Return243(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-1) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now,1,1,1,1,"1",
                DateTime.Now.AddDays(366).ToString(), 
                DateTime.Now.AddDays(366).ToString(), 
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "243");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("ARO")]
        [TestCase("Remedial Services")]
        [TestCase("Habilitation Services")]
        public void ValidatePlanMsg_WhenOrigLocDateGreaterThanPrgBegDate_Return258(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-1) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "1",
                DateTime.Now.AddDays(300).ToString(),
                DateTime.Now.AddDays(366).ToString(),
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "258");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("Habilitation Services")]
        [Ignore("Invalid test case scenario. Month subtraction can never be greater than 13")]
        public void ValidatePlanMsg_WhenOrigLocDateGreaterThanPrgBegDate_Return282(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddMonths(-14) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "1",
                DateTime.Now.AddMonths(9).ToString(),
                "",
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "282");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("SomeProgramName")]
        public void ValidatePlanMsg_WhenScenarioMatches_Return202(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-10) },
                    {"pgr_applicationdate", DateTime.Now.AddDays(10) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "1",
                DateTime.Now.ToString(),
                DateTime.Now.AddDays(366).ToString(),
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "202");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("SomeProgramName")]
        public void ValidatePlanMsg_WhenScenarioMatches_Return203(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"svp_serviceplanID",  1}
                }));

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-10) },
                    {"pgr_applicationdate", DateTime.Now.AddDays(10) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "1",
                "",
                DateTime.Now.AddDays(366).ToString(),
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "203");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("SomeProgramName")]
        [Ignore("Invalid test case scenario. Month subtraction can never be greater than 13")]
        public void ValidatePlanMsg_WhenScenarioMatches_Return282(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"svp_serviceplanID",  1}
                }));

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-10) },
                    {"pgr_applicationdate", DateTime.Now.AddDays(10) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "1",
                "",
                DateTime.Now.AddDays(366).ToString(),
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "282");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("MFP")]
        public void ValidatePlanMsg_WhenScenarioMatches_Return207(string programName)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"svp_serviceplanID",  2},
                    {"svp_begindate",  DateTime.Now.AddDays(-2)},
                    {"svp_enddate",  DateTime.Now.AddDays(2)}
                }));

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-10) },
                    {"pgr_applicationdate", DateTime.Now.AddDays(10) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "1",
                DateTime.Now.ToString(),
                DateTime.Now.AddDays(366).ToString(),
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, "207");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [TestCase("MFP")]
        public void ValidatePlanMsg_WhenScenarioMatches_ReturnServiceSpanValidateMsg(string programName)
        {
            //Arrange
            var expectedMessage = "someMessage";

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) =>
                {
                    target = new List<SqlParameter>();
                    target.Add(new SqlParameter("@intException", 10));
                }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>()));

            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_begdate", DateTime.Now.AddDays(-10) },
                    {"pgr_applicationdate", DateTime.Now.AddDays(10) },
                    {"prg_name", programName },
                    {"prg_programID", 1 }
                }));

            mockServiceSpan
                .Setup(x => x.ValidateSpans(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(expectedMessage);

            //Act
            var response = servicePlan.ValidatePlanMsg(
                1, 1, DateTime.Now, DateTime.Now, 1, 1, 1, 1, "1",
                DateTime.Now.ToString(),
                DateTime.Now.AddDays(366).ToString(),
                1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, expectedMessage);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockProgramRequest
                .Verify(x => x.GetProgramRequest(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void SaveServicePlan_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                servicePlan.SaveServicePlan(new Fixture().Create<ServicePlanRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveServicePlan_WhenExecuted_ShouldReturnExpected(int expected)
        {
            //Arrange
            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>()));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngServicePlanID", expected));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPlanAuthorizationUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intUpdated", 1));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanExceptionCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procedureName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intException", 1));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPlanIsValidSet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Verifiable();

            //Act
            var response = servicePlan.SaveServicePlan(new Fixture().Create<ServicePlanRequest>());

            //Assert
            Assert.AreEqual(expected, response);

            mockDataAccess.VerifyAll();
        }

        [Test]
        [Ignore("Unreachable if condition here -if (dtProgramRequest.Rows[0][pgr_Begdate] == null && Convert.ToDateTime(dtProgramRequest.Rows[0][pgr_Begdate]) < default(DateTime))")]
        public void SaveServicePlan_WhenExecuted_ShouldReturn223()
        {
            //Arrange
            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"somerow","someValue" },
                    {"pgr_Begdate", null },
                    { "prg_name", "Not Remedial Service" }
                }));

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"svp_begindate", DateTime.Now }
                }));

            var request = new Fixture().Create<ServicePlanRequest>();
            request.ServicePlanId = 10;
            request.BeginDate = DateTime.Now.AddMonths(-1);
            //Act
            var response = servicePlan.SaveServicePlan(request);

            //Assert
            Assert.AreEqual(223, response);

            mockDataAccess.VerifyAll();
        }

        [Test]
        public void SaveServicePlan_WhenExecuted_ShouldReturn224()
        {
            //Arrange
            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"somerow","someValue" },
                    {"pgr_Begdate", null },
                    { "prg_name", "Not Remedial Service" }
                }));

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    { "svp_begindate", DateTime.Now },
                    {"svp_Enddate", DateTime.Now }
                }));

            var request = new Fixture().Create<ServicePlanRequest>();
            request.ServicePlanId = 0;
            request.BeginDate = DateTime.Now.AddMonths(-1);
            request.EndDate = DateTime.Now.AddDays(-2);

            //Act
            var response = servicePlan.SaveServicePlan(request);

            //Assert
            Assert.AreEqual(224, response);
        }

        [Test]
        public void SaveServicePlan_WhenExecuted_ShouldReturn231()
        {
            //Arrange
            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"somerow","someValue" },
                    {"pgr_Begdate", null },
                    { "prg_name", "Not Remedial Service" }
                }));

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    { "svp_begindate", DateTime.Now },
                    {"svp_Enddate", DateTime.Now }
                }));

            var request = new Fixture().Create<ServicePlanRequest>();
            request.ServicePlanId = 0;
            request.BeginDate = DateTime.Now.AddMonths(1);
            request.EndDate = Convert.ToDateTime($"{DateTime.Now.Month}/01/{DateTime.Now.Year}").AddDays(-2);

            //Act
            var response = servicePlan.SaveServicePlan(request);

            //Assert
            Assert.AreEqual(231, response);
        }

        [Test]
        public void SaveServicePlan_WhenRequestDateEqualSvpBeginDate_ShouldReturn231()
        {
            //Arrange
            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"somerow","someValue" },
                    {"pgr_Begdate", null },
                    { "prg_name", "Not Remedial Service" }
                }));

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    { "svp_begindate", DateTime.Now.Date },
                    {"svp_Enddate", DateTime.Now.Date }
                }));

            var request = new Fixture().Create<ServicePlanRequest>();
            request.ServicePlanId = 10;
            request.BeginDate = DateTime.Now.Date;
            request.EndDate = Convert.ToDateTime($"{DateTime.Now.Month}/01/{DateTime.Now.Year}").AddDays(-2);

            //Act
            var response = servicePlan.SaveServicePlan(request);

            //Assert
            Assert.AreEqual(231, response);
        }

        [Test]
        public void SaveServicePlan_WhenRequestDateEqualSvpBeginDate_ShouldReturn234()
        {
            //Arrange
            mockProgramRequest
                .Setup(x => x.GetProgramRequest(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"somerow","someValue" },
                    {"pgr_Begdate", null },
                    { "prg_name", "TCM" },
                    {"pgr_Enddate", Convert.ToDateTime($"{DateTime.Now.Month}/01/{DateTime.Now.Year}") }
                }));

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    { "svp_begindate", DateTime.Now.Date },
                    {"svp_Enddate", DateTime.Now.Date }
                }));

            var request = new Fixture().Create<ServicePlanRequest>();
            request.ServicePlanId = 10;
            request.BeginDate = DateTime.Now.Date;
            request.EndDate = Convert.ToDateTime($"{DateTime.Now.Month}/01/{DateTime.Now.Year}").AddDays(-2);

            //Act
            var response = servicePlan.SaveServicePlan(request);

            //Assert
            Assert.AreEqual(234, response);
        }

        [Test]
        public void SaveServicePlanMaintServiceRequest_WhenExceptionOccures_ThrowsException()
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
            Assert.Throws<Exception>(() =>
                servicePlan.SaveServicePlan(new Fixture().Create<MaintServicePlanRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveServicePlanMaintServiceRequest_WhenExecuted_ShouldReturnExpected(int expected)
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
                        target.Add(new SqlParameter("@lngServicePlanID", expected));
                    }))
                .Returns(1);

            //Act
            var response = servicePlan.SaveServicePlan(new Fixture().Create<MaintServicePlanRequest>());

            //Assert
            Assert.AreEqual(expected, response);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServicePlanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }
    }
}
