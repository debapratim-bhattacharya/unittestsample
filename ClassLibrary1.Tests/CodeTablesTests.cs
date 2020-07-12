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
    public class CodeTablesTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;

        private CodeTables codeTables;

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

            codeTables = new CodeTables(mockDataAccess.Object, mockExceptionHandling.Object);
        }

        [Test]
        public void GetCodeTableValues_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "code.prc_GetCode_by_CodeTypeName",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.GetCodeTableValues(
                "someId", DateTime.Now));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetCodeTableValues_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "code.prc_GetCode_by_CodeTypeName",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = codeTables.GetCodeTableValues(
                "someId", DateTime.Now);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "code.prc_GetCode_by_CodeTypeName",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetEndCode_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.GetEndCode("someId"));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetEndCode_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = codeTables.GetEndCode("someId");

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetEndCodebyID_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeByIDGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.GetEndCodebyID(1));

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetEndCodebyID_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeByIDGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act
            var response = codeTables.GetEndCodebyID(1);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeByIDGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetEndCodes_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodesGet",
                    CommandType.StoredProcedure,
                    null))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.GetEndCodes());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetEndCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodesGet",
                    CommandType.StoredProcedure,
                    null))
                .Returns(MockDataTable);

            //Act
            var response = codeTables.GetEndCodes();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISEndCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetHabServicesEndCodes_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISHabServicesEndCodesGet",
                    CommandType.StoredProcedure,
                    null))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.GetHabServicesEndCodes());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetHabServicesEndCodes_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISHabServicesEndCodesGet",
                    CommandType.StoredProcedure,
                    null))
                .Returns(MockDataTable);

            //Act
            var response = codeTables.GetHabServicesEndCodes();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISHabServicesEndCodesGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetSettings_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISSettingsGet",
                    CommandType.StoredProcedure,
                    null))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.GetSettings());

            mockExceptionHandling.Verify();
        }

        [Test]
        public void GetSettings_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISSettingsGet",
                    CommandType.StoredProcedure,
                    null))
                .Returns(MockDataTable);

            //Act
            var response = codeTables.GetSettings();

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAccess.Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISSettingsGet",
                    System.Data.CommandType.StoredProcedure,
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void SaveLastExtractDate_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISLastExtractDateSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SaveLastExtractDate(
                DateTime.Now, 1000));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveLastExtractDate_WhenExecuted_ShouldExecute()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISLastExtractDateSave",
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
            codeTables.SaveLastExtractDate(DateTime.Now, 10);

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISLastExtractDateSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveEndCode_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SaveEndCode("endCode", 10, "desc", "lngdesc", 10));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void SaveEndCode_WhenExecuted_ShouldReturnExpectedOutVariable(
            int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngEndCodeID", expected));
                    }))
                .Returns(1);

            //Act
            var response = codeTables.SaveEndCode("endCode",10,"desc","lngdesc",10);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual($"0|{expected}", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISEndCodeSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveLevelofCareProgram_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISLevelofCareProgramSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SaveLevelofCareProgram(1, 1, 1, 10, 10, 10));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void SaveLevelofCareProgram_WhenExecuted_ShouldReturnExpectedOutVariable(
            int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISLevelofCareProgramSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intLevelCareProgramID", expected));
                    }))
                .Returns(1);

            //Act
            var response = codeTables.SaveLevelofCareProgram(1, 1, 1, 10, 10, 10);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual($"0|{expected}", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISLevelofCareProgramSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SavePACECounties_WhenDataIsEmpty_ShouldNotExecuteAnyStoredProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Verifiable();

            //Act and Assert
            codeTables.SavePACECounties("");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                    Times.Never);
        }

        [Test]
        public void SavePACECounties_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SavePACECounties(
                "someString"));

            mockExceptionHandling.VerifyAll();
        }
                
        [TestCase("country1")]
        [TestCase("country1|country2")]
        public void SavePACECounties_WhenExecuted_ShouldExecute(string data)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesDelete",
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

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesSave",
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
            codeTables.SavePACECounties(data);

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPACECountiesDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISPACECountiesSave",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny),
               Times.Exactly(data.Split('|').Count()));
        }

        [Test]
        public void SaveProgram_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SaveProgram(new Fixture().Create<SaveCodeTableProgramRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void SaveProgram_WhenExecuted_ShouldReturnExpectedOutVariable(
            int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngProgramID", expected));
                    }))
                .Returns(1);

            //Act
            var response = codeTables.SaveProgram(new Fixture().Create<SaveCodeTableProgramRequest>());

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual($"0|{expected}", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISProgramSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveProgramsServices_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPrograms_ServicesSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SaveProgramsServices(
                new Fixture().Create<SaveProgramServiceRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveProgramsServices_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPrograms_ServicesSave",
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
           codeTables.SaveProgramsServices(new Fixture().Create<SaveProgramServiceRequest>());

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPrograms_ServicesSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveService_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SaveService(
                1,"wcode","modifier","serviceName","serviceDescription",1,1,"strTier",10));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void SaveService_WhenExecuted_ShouldReturnExpectedOutVariable(
            int expected)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@lngServiceID", expected));
                    }))
                .Returns(1);

            //Act
            var response = codeTables.SaveService(
                1, "wcode", "modifier", "serviceName", "serviceDescription", 1, 1, "strTier", 10);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual($"0|{expected}", response);

            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISServiceSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void SaveSettings_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSettingsSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => codeTables.SaveSettings(1,"name","value","comment","delete",10));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveSettings_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSettingsSave",
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
            codeTables.SaveSettings(1, "name", "value", "comment", "delete", 10);

            //Assert
            mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSettingsSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }
    }
}
