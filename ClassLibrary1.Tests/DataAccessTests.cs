using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
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
    public class DataAccessTests
    {
        private Mock<IDbConnectionHelper> mockDbConnectionHelper;

        private Mock<IExceptionHandling> mockExceptionHandling;

        private Mock<IDbConnection> mockDbConnection;

        private Mock<IDbCommand> mockDbCommand;

        private Mock<IDbDataAdapter> mockDataAdapter;

        private DataAccess dataAccess;

        [SetUp]
        public void SetUp()
        {
            mockDbConnectionHelper = new Mock<IDbConnectionHelper>();

            mockExceptionHandling = new Mock<IExceptionHandling>();

            mockDbConnection = new Mock<IDbConnection>();

            mockDbCommand = new Mock<IDbCommand>();

            mockDataAdapter = new Mock<IDbDataAdapter>();

            mockExceptionHandling
                .Setup(x => x.LogException(It.IsAny<Exception>(), ExceptionPolicy.Web_Exception))
                .Verifiable();

            mockDbConnectionHelper
                .Setup(x => x.CloseConnection(It.IsAny<IDbConnection>()))
                .Verifiable();

            dataAccess = new DataAccess(mockExceptionHandling.Object, mockDbConnectionHelper.Object);
        }

        private void SetUpDbConnection()
        {
            mockDbConnection
               .Setup(x => x.CreateCommand())
               .Returns(mockDbCommand.Object);

            mockDbConnectionHelper
                .Setup(x => x.GetConnection(It.IsAny<string>()))
                .Returns(mockDbConnection.Object);

            mockDbConnectionHelper
                .Setup(x => x.GetDbDataAdapter(It.IsAny<IDbCommand>()))
                .Returns(mockDataAdapter.Object);
        }

        private void SetUpDbComandForParameters(SqlParameter[] parameters, string commandText, CommandType commandType)
        {
            foreach (var param in parameters)
            {
                mockDbCommand.SetupGet(c => c.Parameters[param.ParameterName]).Returns(param);
            }
        }

        [Test]
        public void GetDataTable_WhenOpenConnectionThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDbConnectionHelper
                .Setup(x => x.GetConnection(It.IsAny<string>()))
                .Throws<Exception>();

            //Act
            Assert.Throws<Exception>(() =>
                dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "someProcName",
                    CommandType.StoredProcedure));
        }

        [Test]
        public void GetDataTable_WhenDataAdapterFillThrowsError_ShouldThrowException()
        {
            //Arrange
            SetUpDbConnection();

            mockDataAdapter
                .Setup(x => x.Fill(It.IsAny<DataSet>()))
                .Throws<Exception>();

            //Act
            Assert.Throws<Exception>(() =>
                dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "someProcName",
                    CommandType.StoredProcedure),
                    "Error in GetDataTable");

            AssertException();
        }

        [Test]
        public void GetDataTable_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            SetUpDbConnection();

            mockDataAdapter
                .Setup(x => x.Fill(It.IsAny<DataSet>()))
                .Callback((DataSet ds) =>
                {
                    GetMockDataSet(ds);
                })
                .Returns(1);

            //Act
            var response = dataAccess.GetDataTable(
                ApplicationConstants.IsisConnectionString,
                "someProcName",
                CommandType.StoredProcedure);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAdapter
                .Verify(x => x.Fill(It.IsAny<DataSet>()), Times.Once);
        }

        [Test]
        public void GetDataTableWithParameter_WhenExecuted_ShouldReturnDataTable()
        {
            //Arrange
            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("paramName", "someValue")
                }.ToArray();            

            SetUpDbConnection();

            SetUpDbComandForParameters(parameters, "someProcName", CommandType.StoredProcedure);

            mockDataAdapter
                .Setup(x => x.Fill(It.IsAny<DataSet>()))
                .Callback((DataSet ds) =>
                {
                    GetMockDataSet(ds);
                })
                .Returns(1);

            //Act
            var response = dataAccess.GetDataTable(
                ApplicationConstants.IsisConnectionString,
                "someProcName",
                CommandType.StoredProcedure,
                parameters);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataTable>(response);

            mockDataAdapter
                .Verify(x => x.Fill(It.IsAny<DataSet>()), Times.Once);

        }

        [Test]
        public void GetDataSet_WhenOpenConnectionThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDbConnectionHelper
                .Setup(x => x.GetConnection(It.IsAny<string>()))
                .Throws<Exception>();

            //Act
            Assert.Throws<Exception>(() =>
                dataAccess.GetDataSet(ApplicationConstants.IsisConnectionString,
                    "someProcName",
                    CommandType.StoredProcedure));
        }

        [Test]
        public void GetDataSet_WhenDataAdapterFillThrowsError_ShouldThrowException()
        {
            //Arrange
            SetUpDbConnection();

            mockDataAdapter
                .Setup(x => x.Fill(It.IsAny<DataSet>()))
                .Throws<Exception>();

            //Act
            Assert.Throws<Exception>(() =>
                dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "someProcName",
                    CommandType.StoredProcedure),
                    "Error in GetDataSet");

            AssertException();
        }

        [Test]
        public void GetDataSet_WhenExecuted_ShouldReturnDataSet()
        {
            //Arrange
            SetUpDbConnection();

            mockDataAdapter
                .Setup(x => x.Fill(It.IsAny<DataSet>()))
                .Callback((DataSet ds) =>
                {
                    GetMockDataSet(ds);
                })
                .Returns(1);

            //Act
            var response = dataAccess.GetDataSet(
                ApplicationConstants.IsisConnectionString,
                "someProcName",
                CommandType.StoredProcedure);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataSet>(response);

            mockDataAdapter
                .Verify(x => x.Fill(It.IsAny<DataSet>()), Times.Once);

        }

        [Test]
        public void GetDataSetWithParameter_WhenExecuted_ShouldReturnDataSet()
        {
            //Arrange
            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("paramName", "someValue")
                }.ToArray();

            SetUpDbConnection();

            SetUpDbComandForParameters(parameters, "someProcName", CommandType.StoredProcedure);

            mockDataAdapter
                .Setup(x => x.Fill(It.IsAny<DataSet>()))
                .Callback((DataSet ds) =>
                {
                    GetMockDataSet(ds);
                })
                .Returns(1);

            //Act
            var response = dataAccess.GetDataSet(
                ApplicationConstants.IsisConnectionString,
                "someProcName",
                CommandType.StoredProcedure,
                parameters);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<DataSet>(response);

            mockDataAdapter
                .Verify(x => x.Fill(It.IsAny<DataSet>()), Times.Once);

        }

        private static void GetMockDataSet(DataSet ds)
        {
            if (ds.Tables["Table"] == null)
            {
                ds.Tables.Add("Table");
                ds.Tables["Table"].Columns.Add(new DataColumn());
            }

            var row = ds.Tables["Table"].NewRow();
            row[0] = "Test";

            ds.Tables["Table"].Rows.Add(row);
        }

        [Test]
        public void ExecuteNonQuery_WhenDatabaseThrowsError_ShouldThrowException()
        {
            //Arrange
            SetUpDbConnection();

            var outParameters = new List<SqlParameter>();

            mockDataAdapter
                .Setup(x => x.Fill(It.IsAny<DataSet>()))
                .Throws<Exception>();

            //Act
            Assert.Throws<Exception>(() =>
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "someProcName",
                    CommandType.StoredProcedure,
                    new SqlParameter[2],
                    ref outParameters),
                    "Error in ExecuteNonQuery");

            AssertException();
        }

        [Test]
        public void ExecuteNonQuery_WhenExecuted_ShouldReturnExpectedValue()
        {
            //Arrange
            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("paramName", "someValue"),
                    new SqlParameter("outParam", "someValue") { Direction = ParameterDirection.Output},
                    new SqlParameter("retParam", "returnValue") { Direction = ParameterDirection.ReturnValue}
                }.ToArray();

            var outParameters = new List<SqlParameter>();

            SetUpDbConnection();

            SetUpDbComandForParameters(parameters, "someProcName", CommandType.StoredProcedure);

            mockDbCommand
                .Setup(x => x.ExecuteNonQuery())                
                .Returns(1);

            //Act
            var response = dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "someProcName",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

            //Assert
            Assert.IsNotNull(response);

            Assert.IsInstanceOf<int>(response);

            Assert.AreEqual(
                parameters.Count(x => x.Direction == ParameterDirection.Output
                    || x.Direction == ParameterDirection.ReturnValue),
                outParameters.Count);

            mockDbCommand
                .Verify(x => x.ExecuteNonQuery(), Times.Once);
        }

        private void AssertException()
        {
            mockDbConnectionHelper
               .Verify(x => x.CloseConnection(It.IsAny<IDbConnection>()), Times.Once);

            mockExceptionHandling.VerifyAll();
        }

    }
}
