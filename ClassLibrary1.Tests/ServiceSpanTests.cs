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
    public class ServiceSpanTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;
        private Mock<IServicePlan> mockServicePlan;

        private ServiceSpan serviceSpan;

        delegate void MockRefCallback(
            string connectionString,
            string procName,
            CommandType commandType,
            SqlParameter[] sqlParameters,
            ref List<SqlParameter> outParameters);

        private DataTable MockDataTable(Dictionary<string,object> rows)
        {
            DataTable table = new DataTable();
            foreach(var keyValue in rows)
            {
                table.Columns.Add(keyValue.Key);
            }
            
            DataRow dataRow = table.NewRow();
            foreach(var keyValue in rows)
            {
                dataRow[keyValue.Key] = keyValue.Value;
            }
            
            table.Rows.Add(dataRow);

            return table;
        }

        private DataSet MockDataSet(Dictionary<string, object> rows)
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(MockDataTable(rows));

            return ds;
        }

        [SetUp]
        public void SetUp()
        {
            mockExceptionHandling = new Mock<IExceptionHandling>();

            mockDataAccess = new Mock<IDataAccess>();

            mockServicePlan = new Mock<IServicePlan>();

            mockExceptionHandling
                .Setup(x => x.LogException(It.IsAny<Exception>(), ExceptionPolicy.Web_Exception))
                .Verifiable();

            serviceSpan = new ServiceSpan(mockDataAccess.Object, mockExceptionHandling.Object, mockServicePlan.Object);
        }

        [Test]
        public void DeleteServiceSpan_WhenExceptionOccures_ThrowsException()
        {
            //Arrange
            mockServicePlan
                .Setup(x => x.GetServicePlan(It.IsAny<int>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                serviceSpan.DeleteServiceSpan(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void DeleteServiceSpan_WhenServicePlanIsEmpty_ShouldNotInvokeValidatePlanUpdate()
        {
            //Arrange
            mockServicePlan
                .Setup(x => x.GetServicePlan(It.IsAny<int>()))
                .Returns(new DataTable());

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockServicePlan
                .Setup(x => x.ValidatePlanUpdate(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Verifiable();

            //Act
            serviceSpan.DeleteServiceSpan(1, 1, 1);

            mockServicePlan
                .Verify(x => x.GetServicePlan(It.IsAny<int>()),Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockServicePlan
                .Verify(x => x.ValidatePlanUpdate(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void DeleteServiceSpan_WhenServicePlanHasRows_ShouldInvokeValidatePlanUpdate()
        {
            //Arrange
            mockServicePlan
                .Setup(x => x.GetServicePlan(It.IsAny<int>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                    {
                        {  "pgr_ProgramRequestID", 1 },
                        {  "svp_BeginDate", DateTime.Now },
                        {  "svp_EndDate", DateTime.Now },
                        {  "svp_MonthlyCap", Convert.ToDecimal(1000) },
                        {  "svp_YearlyCap", Convert.ToDecimal(1000) },
                        {  "pgr_ClientPartic_FirstMo", Convert.ToDecimal(1000) },
                        {  "pgr_ClientPartic_Ongoing", Convert.ToDecimal(1000) },
                        {  "lvl_Code", "code" },
                        {  "svp_LevelCareEffectiveDate", "someDate" },
                        {  "svp_OrigLOCDate", "LOCDate" },
                    }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockServicePlan
                .Setup(x => x.ValidatePlanUpdate(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Verifiable();

            //Act
            serviceSpan.DeleteServiceSpan(1, 1, 1);

            mockServicePlan
                .Verify(x => x.GetServicePlan(It.IsAny<int>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockServicePlan
                .Verify(x => x.ValidatePlanUpdate(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void DeleteServiceSpan_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.DeleteServiceSpan(10, 1, "exceptionNumber", "someExceptionComment"));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void DeleteServiceSpan_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngProgramRequestID", 1));
                    }))
                .Returns(1);

            //Act
            serviceSpan.DeleteServiceSpan(10, 1, "exceptionNumber", "someExceptionComment");

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanDelete_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void GetEnhancedServicesSpans_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "prc_ISISEnhancedServicesSpansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.GetEnhancedServicesSpans(1,"stateId"));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetEnhancedServicesSpans_WhenExecuted_ShouldReturnDataSet()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "prc_ISISEnhancedServicesSpansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataSet(new Dictionary<string, object>()));

            //Act
            var response = serviceSpan.GetEnhancedServicesSpans(1, "someString");

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataSet>(response);

            mockDataAccess
                .Verify(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "prc_ISISEnhancedServicesSpansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetOdsmmisEligInfo_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "isis.prc_ODSMMISEligibilityInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.GetOdsmmisEligInfo("stateId", DateTime.Now));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetOdsmmisEligInfo_WhenExecuted_ShouldReturnDataSet()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "isis.prc_ODSMMISEligibilityInfoGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataSet(new Dictionary<string, object>()));

            //Act
            var response = serviceSpan.GetOdsmmisEligInfo("someString", DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataSet>(response);

            mockDataAccess
                .Verify(x => x.GetDataSet(
                    It.IsAny<string>(),
                    "isis.prc_ODSMMISEligibilityInfoGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetServiceSpan_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.GetServiceSpan(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetServiceSpan_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>()));

            //Act
            var response = serviceSpan.GetServiceSpan(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetServiceSpans_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.GetServiceSpans(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetServiceSpans_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>()));

            //Act
            var response = serviceSpan.GetServiceSpans(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void GetServiceSpansRequiringPaReview_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansRequiringPAGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.GetServiceSpansRequiringPaReview(1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetServiceSpansRequiringPaReview_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansRequiringPAGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>()));

            //Act
            var response = serviceSpan.GetServiceSpansRequiringPaReview(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansRequiringPAGet",
                    It.IsAny<CommandType>(), It.IsAny<SqlParameter[]>()),
                    Times.Once);
        }

        [Test]
        public void SaveServiceSpan_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.SaveServiceSpan(new Fixture().Create<MaintServiceSpanRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(0)]
        public void SaveServiceSpan_WhenExecuted_ShouldReturnExpectedReturnValue(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngProgramRequestID", 1));
                    }))
                .Returns(expected);

            //Act
            var response = serviceSpan.SaveServiceSpan(new Fixture().Create<MaintServiceSpanRequest>());

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanSave_Exception",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveServiceSpanServiceRequest_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => serviceSpan.SaveServiceSpan(new Fixture().Create<ServiceSpanRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(0)]
        public void SaveServiceSpanServiceRequest_WhenExecuted_ShouldReturnExpectedReturnValue(int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngServiceSpanID", expected));
                    }))
                .Returns(1);

            //Act
            var response = serviceSpan.SaveServiceSpan(new Fixture().Create<ServiceSpanRequest>());

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }
    }
}
