using ClassLibrary1.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1.Tests
{
    [TestFixture(Category ="Unit")]
    public class ReferenceTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;

        private Reference reference;

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

            reference = new Reference(mockDataAccess.Object, mockExceptionHandling.Object);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetAidTypes_WhenDbErrorsOut_ShouldThrowException(int providerId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISAidTypesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetAidTypes(providerId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetAidTypes_WhenExecuted_ShouldReturnDataTable(int providerId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISAidTypesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetAidTypes(providerId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISAidTypesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()), 
                Times.Once);
        }

        [Test]
        public void GetCertificationTypes_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCertificationTypesGet",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetCertificationTypes());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetCertificationTypes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCertificationTypesGet",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetCertificationTypes();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCertificationTypesGet",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCounties_WhenDbErrorsOut_ShouldThrowException(int workerId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCountiesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetCounties(workerId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCounties_WhenExecuted_ShouldReturnDataTable(int workerId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCountiesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetCounties(workerId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCountiesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCountiesAndStates_WhenDbErrorsOut_ShouldThrowException(int workerId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCountiesAndStatesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetCountiesAndStates(workerId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCountiesAndStates_WhenExecuted_ShouldReturnDataTable(int workerId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCountiesAndStatesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetCountiesAndStates(workerId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISCountiesAndStatesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCounty_WhenDbErrorsOut_ShouldThrowException(int organizationId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCountyGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetCounty(organizationId));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetCounty_WhenExecuted_ShouldReturnString(int organizationId)
        {
            //Arrange
            var expected = "someOutput";

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCountyGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@strOrgName", expected));
                    }))
                .Returns(1);

            //Act
            var response = reference.GetCounty(organizationId);

            //Assert
            Assert.IsInstanceOf<string>(response);

            Assert.AreEqual(expected, response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCountyGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetDenialReasons_WhenDbErrorsOut_ShouldThrowException(int programId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDenialReasonsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetDenialReasons(programId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetDenialReasons_WhenExecuted_ShouldReturnDataTable(int programId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDenialReasonsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetDenialReasons(programId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDenialReasonsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetDiagCodes_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDiagCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetDiagCodes(0,DateTime.Now.Date.ToString(),"Abc",1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetDiagCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDiagCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetDiagCodes(0, DateTime.Now.Date.ToString(), "Abc", 1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISDiagCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetFacilitiesPrograms_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGetFacilities",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetFacilitiesPrograms());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetFacilitiesPrograms_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGetFacilities",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetFacilitiesPrograms();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGetFacilities",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }

        [Test]
        public void GetLevelOfCares_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISLevelCaresGet",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetLevelOfCares());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetLevelOfCares_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISLevelCaresGet",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetLevelOfCares();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISLevelCaresGet",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetLevelCares_WhenDbErrorsOut_ShouldThrowException(int programRequestId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISLevelCareGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetLevelCares(programRequestId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetLevelCares_WhenExecuted_ShouldReturnDataTable(int programRequestId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISLevelCareGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetLevelCares(programRequestId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISLevelCareGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetPACECounties_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesGet",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetPACECounties());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetPACECounties_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesGet",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetPACECounties();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesGet",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }

        [Test]
        public void GetProcedureCodes_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProcedureCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => 
            reference.GetProcedureCodes(
                0, 0, DateTime.Now.Date.ToString(), DateTime.Now.AddDays(-2).Date.ToString()));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetProcedureCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProcedureCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = 
                reference.GetProcedureCodes(
                    0, 0, DateTime.Now.Date.ToString(), DateTime.Now.AddDays(-2).Date.ToString());

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProcedureCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetProgram_WhenDbErrorsOut_ShouldThrowException(long programId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetProgram(programId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetProgram_WhenExecuted_ShouldReturnDataTable(long programId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetProgram(programId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetPrograms_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGet",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetPrograms());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetPrograms_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGet",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetPrograms();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsGet",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetProgramsService_WhenDbErrorsOut_ShouldThrowException(int serviceId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsServiceGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetProgramsService(serviceId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GetProgramsService_WhenExecuted_ShouldReturnDataTable(int serviceId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsServiceGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetProgramsService(serviceId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISProgramsServiceGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [TestCase(0,1)]
        [TestCase(1,1)]
        public void GetService_WhenDbErrorsOut_ShouldThrowException(int serviceId, int programId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetService(serviceId,programId));

            mockExceptionHandling.Verify();
        }

        [TestCase(0,1)]
        [TestCase(1,1)]
        public void GetProgramsService_WhenExecuted_ShouldReturnDataTable(int serviceId, int programId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetService(serviceId,programId);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetServices_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicesGet",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetServices());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetServices_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicesGet",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetServices();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServicesGet",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }

        [Test]
        public void GetStates_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISStateGet",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetStates());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetStates_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISStateGet",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetStates();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISStateGet",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }

        [Test]
        public void GetUnitTypes_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISUnitTypesGet",
                    System.Data.CommandType.StoredProcedure))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => reference.GetUnitTypes());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetUnitTypes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISUnitTypesGet",
                    System.Data.CommandType.StoredProcedure))
                .Returns(MockDataTable);

            //Act
            var response = reference.GetUnitTypes();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISUnitTypesGet",
                    System.Data.CommandType.StoredProcedure),
                Times.Once);
        }
    }
}
