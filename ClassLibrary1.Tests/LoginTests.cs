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
    public class LoginTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IDataAccess> _mockDataAccess;
        private Mock<IExceptionHandling> _mockExceptionHandling;

        private Login _login;

        private long sessionId = 100000;

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
            _mockConfiguration = new Mock<IConfiguration>();
            _mockDataAccess = new Mock<IDataAccess>();
            _mockExceptionHandling = new Mock<IExceptionHandling>();

            //Setting up default logic for ConfigurationManger.AppSetting["Key"] call
            _mockConfiguration
                .Setup(x => x.GetAppSetting(It.IsAny<string>()))
                .Returns("1");

            //Setting up default logic for exception handling LogException method call
            _mockExceptionHandling
                .Setup(x=> x.LogException(It.IsAny<Exception>(),ExceptionPolicy.Web_Exception))
                .Verifiable();

            _login = new Login(_mockDataAccess.Object,
                _mockExceptionHandling.Object,
                _mockConfiguration.Object);
        }

        [Test]
        public void GetOptionsInfo_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISOptionsInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.GetOptionsInfo());

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetOptionsInfo_WhenExecuted_ShouldReturnDatatable()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISOptionsInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(new DataTable());

            //Act
            var response = _login.GetOptionsInfo();

            //Assert
            Assert.IsInstanceOf<DataTable>(response);

            _mockDataAccess.Verify(x=> x.GetDataTable(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()),
                Times.Once);
        }

        [Test]
        public void GetSplashText_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSplashTextGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.GetSplashText(8));

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetSplashText_WhenExecuted_ShouldReturnString()
        {
            //Arrange
            var expected = "someOutput";

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSplashTextGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@strSplashText", expected));
                    }))
                .Returns(1);

            //Act
            var response = _login.GetSplashText(10);

            //Assert
            Assert.IsInstanceOf<string>(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void LoginDetails_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.LoginDetails("someUsername","somePassword","127.0.0.1"));

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void LoginDetails_WhenAuthenticationIsSuccess_ShouldReturnLoginSuccessResultCode()
        {
            //Arrange
            var expected = 1;
            var daysTillPasswordExpires = 30;

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intResult", expected));
                        target.Add(new SqlParameter("@intDaysTillPasswordExpires", daysTillPasswordExpires));
                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSessionCreate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intResult", expected));
                    }))
                .Returns(1);

            //Act
            var response = _login.LoginDetails("someUsername","somePassword", "someIp");

            //Assert
            Assert.IsInstanceOf<string>(response);

            Assert.AreEqual(expected+"|"+daysTillPasswordExpires, response);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSessionCreate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void LoginDetails_WhenAuthenticationIsNotSuccess_ShouldReturnLoginFailResultCode()
        {
            //Arrange
            var expected = -100;
            var daysTillPasswordExpires = 30;

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intResult", expected));
                        target.Add(new SqlParameter("@intDaysTillPasswordExpires", daysTillPasswordExpires));
                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityCounterIncrement",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intResult", expected));
                    }))
                .Returns(1);

            //Act
            var response = _login.LoginDetails("someUsername", "somePassword", "someIp");

            //Assert
            Assert.IsInstanceOf<string>(response);

            Assert.AreEqual(expected + "|" + daysTillPasswordExpires, response);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityCounterIncrement",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
                Times.Once);
        }

        [Test]
        public void Logout_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.Delete(
                    It.IsAny<string>(),
                    "prc_ISISSecurityLogout",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.Logout(sessionId));

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void Logout_WhenSessionExist_ShouldExecuteDeleteCmdInDb()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.Delete(
                    It.IsAny<string>(),
                    "prc_ISISSecurityLogout",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Verifiable();

            //Act and Assert
           _login.Logout(sessionId);

            _mockDataAccess.VerifyAll();
        }

        [Test]
        public void GetErrorBlock_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.GetErrorBlock(100));

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetErrorBlock_WhenDataTableIsEmpty_ShouldReturnEmptyString()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(new DataTable());

            //Act and Assert
            var response =  _login.GetErrorBlock(100);

            Assert.IsTrue(string.IsNullOrWhiteSpace(response));

            _mockDataAccess.VerifyAll();
        }

        [Test]
        public void GetErrorBlock_WhenDataTableIsHavingDataRows_ShouldReturnValidString()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable);

            //Act and Assert
            var response = _login.GetErrorBlock(100);

            Assert.IsFalse(string.IsNullOrWhiteSpace(response));

            Assert.IsTrue(response.StartsWith("Const ERRdr"));

            _mockDataAccess.VerifyAll();
        }

        [Test]
        public void GetErrorString_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.GetErrorString(100));

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void GetErrorString_WhenErrorIdIsCorrect_ShouldReturnExpectedErrorString()
        {
            //Arrange
            var expected = "some error string";

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISErrorStringGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@strErrorString", expected));
                    }))
                .Returns(1);

            //Act
            var response = _login.GetErrorString(10);

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.VerifyAll();
        }

        [Test]
        public void SaveSessionVariable_WhenExecuted_ShouldSaveAndReturnExecuteNonQueryResult()
        {
            //Arrange
            var expected = 1;
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSessionVariableSave",
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
            var response = _login.SaveSessionVariable(sessionId, "someVariableName", "someVariableValue");

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.VerifyAll();
        }

        [Test]
        public void SaveSessionVariable_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSessionVariableSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act
            Assert.Throws<Exception>(() => _login.SaveSessionVariable(sessionId, "someVariableName", "someVariableValue"));

            //Assert
            _mockDataAccess.VerifyAll();
        }

        [Test]
        public void SessionVariable_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSessionVariableGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.SessionVariable(sessionId, "someVariableName"));

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SessionVariable_WhenExecuted_ShouldReturnExpectedString()
        {
            //Arrange
            var expected = "sessionStringValue";

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSessionVariableGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString, 
                    string procName,                                              
                    CommandType commandType,                                              
                    SqlParameter[] sqlParams,                                              
                    ref List<SqlParameter> target) =>{ 
                                              
                        target = new List<SqlParameter>();
                                              
                        target.Add(new SqlParameter("@strVariableValue", expected));
                                              
                    }))
                .Returns(1);

            //Act
            var response = _login.SessionVariable(sessionId, "someVariableName");

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.VerifyAll();
        }

        [Test]
        public void SetPassword_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.SetPassword("someUsername", "somePassword", "someNewPassword", sessionId));

            _mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SetPassword_WhenPasswordValidationFails_ShouldReturnExecuteNonQueryInvalidationValue()
        {
            //Arrange
            var expected = 2;

            var intDaysTillPasswordExpires = 30;

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", expected));
                        target.Add(new SqlParameter("@intDaysTillPasswordExpires", intDaysTillPasswordExpires));

                    }))
                .Returns(1);

            //Act
            var response = _login.SetPassword("someUsername", "somePassword", "someNewPassword", sessionId);

            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.Verify(x=> x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);
        }

        [Test]
        public void SetPassword_WhenPasswordHistoryDontExist_ShouldReturnExpectedResponse()
        {
            //Arrange
            var expected = -106;

            var intDaysTillPasswordExpires = 30;

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 1));
                        target.Add(new SqlParameter("@intDaysTillPasswordExpires", intDaysTillPasswordExpires));

                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 1));
                    }))
                .Returns(1);

            //Act
            var response = _login.SetPassword("someUsername", "somePassword", "someNewPassword", sessionId);

            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);
        }

        [Test]
        public void SetPassword_WhenPasswordDictionaryMatchFails_ShouldReturnExpectedResponse()
        {
            //Arrange
            var expected = -107;

            var intDaysTillPasswordExpires = 30;

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 1));
                        target.Add(new SqlParameter("@intDaysTillPasswordExpires", intDaysTillPasswordExpires));

                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 0));
                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordDictionaryCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 0));
                    }))
                .Returns(1);

            //Act
            var response = _login.SetPassword("someUsername", "somePassword", "someNewPassword", sessionId);

            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordDictionaryCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);
        }

        [Test]
        public void SetPassword_WhenSetPasswordCheckPass_ShouldInvokePasswordSetProc()
        {
            //Arrange
            var expected = 1;

            var intDaysTillPasswordExpires = 30;

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 1));
                        target.Add(new SqlParameter("@intDaysTillPasswordExpires", intDaysTillPasswordExpires));

                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 0));
                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordDictionaryCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 1));
                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordSet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", expected));
                    }))
                .Returns(1);

            //Act
            var response = _login.SetPassword("someUsername", "somePassword", "someNewPassword", sessionId);

            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordDictionaryCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordSet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void SetPassword_WhenSetPasswordFail_ShouldReturnIntErrorCode(int responseCode)
        {
            //Arrange
            var expected = -100;

            var intDaysTillPasswordExpires = 30;

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 1));
                        target.Add(new SqlParameter("@intDaysTillPasswordExpires", intDaysTillPasswordExpires));

                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 0));
                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordDictionaryCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", 1));
                    }))
                .Returns(1);

            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordSet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {

                        target = new List<SqlParameter>();

                        target.Add(new SqlParameter("@intResult", responseCode));
                    }))
                .Returns(1);

            //Act
            var response = _login.SetPassword("someUsername", "somePassword", "someNewPassword", sessionId);

            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityAuthenticate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordHistoryGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordDictionaryCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);

            _mockDataAccess.Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISPasswordSet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),
            Times.Once);
        }

        [Test]
        public void GetSecurity_WhenDbErrorsOut_ShouldThrowException()
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() => _login.GetSecurity(sessionId, "127.0.0.1", "someWebPageId"));

            _mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(-1)]
        public void GetSecurity_WhenExecuted_ShouldReturnExpectedIntHash(int expected)
        {
            //Arrange
            _mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback(
                    (string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intHash", expected));
                    }))
                .Returns(1);

            //Act 
            var response = _login.GetSecurity(sessionId, "127.0.0.1", "someWebPageId");

            //Assert
            Assert.IsNotNull(response);

            Assert.AreEqual(expected, response);

            _mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISSecurityGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }
    }
}
