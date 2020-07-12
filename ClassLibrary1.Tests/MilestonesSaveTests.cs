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
    public class MilestonesSaveTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;
        private Mock<ILogin> mockLogin;

        private MilestonesSave milestonesSave;

        delegate void MockRefCallback(
            string connectionString,
            string procName,
            CommandType commandType,
            SqlParameter[] sqlParameters,
            ref List<SqlParameter> outParameters);

        private DataTable MockDataTable(Dictionary<string, object> rows)
        {
            DataTable table = new DataTable();
            foreach (var keyValue in rows)
            {
                table.Columns.Add(keyValue.Key);
            }

            DataRow dataRow = table.NewRow();
            foreach (var keyValue in rows)
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

            mockLogin = new Mock<ILogin>();

            mockExceptionHandling
                .Setup(x => x.LogException(It.IsAny<Exception>(), ExceptionPolicy.Web_Exception))
                .Verifiable();

            milestonesSave = new MilestonesSave(mockDataAccess.Object, mockExceptionHandling.Object, mockLogin.Object);
        }

        [Test]
        public void SaveMilestone_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveMilestone(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveMilestone_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveMilestone(1,1,1,"comments","data",1,"beginCode");

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveAllRolesAssigned_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveAllRolesAssigned(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0,0)]
        [TestCase(1, 1)]
        public void SaveAllRolesAssigned_WhenExecuted_ShouldExecuteProcedure(int responseType, int intCount)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneAssignmentCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intCount", intCount));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockLogin
                .Setup(x => x.GetErrorString(It.IsAny<int>()))
                .Verifiable();

            //Act
            milestonesSave.SaveAllRolesAssigned(1, 1, 1, "comments", "data", responseType, 1);

            if(responseType == 1)
            {
                mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneAssignmentCheck",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
            }

            if(intCount > 0)
            {
                mockLogin
                    .Verify(x => x.GetErrorString(It.IsAny<int>()));
            }

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveARODiagLPHA_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneARODiagLPHASave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveARODiagLPHA(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase("1", 0, "01/01/1900")]
        [TestCase("1|2", 1,"01/01/2020")]
        public void SaveARODiagLPHA_WhenExecuted_ShouldExecuteProcedure(string diagnosisCodes, int intCount, string oldPHACertDate)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneARODiagLPHASave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intCount", intCount));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveARODiagLPHA(1, 1, 1, 1, diagnosisCodes, "", "01/01/1900", oldPHACertDate,1);

                mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneARODiagLPHASave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Exactly(diagnosisCodes.Split('|').Count()));

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveCancelDecision_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISCloseActiveMilestones",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveCancelDecision(1, 1, 1, "", 1, false, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SaveCancelDecision_WhenExecuted_ShouldExecuteProcedure(bool accept)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCloseActiveMilestones",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveCancelDecision(1, 1, 1, "Comments",1,accept,1);

            if (!accept)
            {
                mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCloseActiveMilestones",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
            }

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveCapIncrease_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneCapIncreaseSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveCapIncrease(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveCapIncrease_WhenErrorCodeIsNotZero_ShouldNotInvokeSaveMilestone(int intRETURN)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCapIncreaseSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intRETURN", intRETURN));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            var response = milestonesSave.SaveCapIncrease(1, 1, 1,"",1,1,"01/01/2020",1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, $"{intRETURN}|-1");

            mockDataAccess
            .Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                "prc_ISISMilestoneCapIncreaseSave",
                It.IsAny<CommandType>(),
                It.IsAny<SqlParameter[]>(),
                ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
            

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Never);
        }

        [Test]
        public void SaveCapIncrease_WhenErrorCodeIsZero_ShouldNotInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCapIncreaseSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intRETURN", 0));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            var response = milestonesSave.SaveCapIncrease(1, 1, 1, "", 1, 1, "01/01/2020", 1);

            //Assert
            Assert.IsEmpty(response);

            mockDataAccess
            .Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                "prc_ISISMilestoneCapIncreaseSave",
                It.IsAny<CommandType>(),
                It.IsAny<SqlParameter[]>(),
                ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveCauseofDeath_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISCauseOfDeathMilestoneSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveCauseofDeath(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveCauseofDeath_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCauseOfDeathMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))                
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveCauseofDeath(1, 1, 1,"",1,1,1,1);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISCauseOfDeathMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveClosureEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneClosureReasonSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveClosureEntry(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveClosureEntry_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneClosureReasonSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveClosureEntry(1, 1, 1,1,1,"","",1);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneClosureReasonSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveCOLSArbitration_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneCOLSArbitrationSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveCOLSArbitration(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveCOLSArbitration_WhenExecuted_ShouldInvokeSaveMilestone(int intOrganizationID)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSArbitrationSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intOrganizationID", intOrganizationID));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveCOLSArbitration(1,1,1,"",1,1);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSArbitrationSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveCOLS_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneCOLSSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveCOLS(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveCOLS_WhenExecuted_ShouldInvokeSaveMilestone(int intOrganizationID)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intOrganizationID", intOrganizationID));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveCOLS(1,1,1,"",1,1);

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveDateEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneDateEntrySave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveDateEntry(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveDateEntry_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDateEntrySave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveDateEntry(1, 1, 1, "", 1, 1);

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDateEntrySave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveDCNEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneDCNEntrySave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveDCNEntry(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveDCNEntryy_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDCNEntrySave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveDCNEntry(1, 1, 1, "", 1, 1);

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDCNEntrySave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        [Ignore("SaveDiag code must be modified")]
        public void SaveDiagEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneDiagEntrySave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveDiagEntry(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase("1", 0)]
        [TestCase("1|2", 1)]
        public void SaveDiagEntry_WhenExecuted_ShouldExecuteProcedure(string diagnosisCodes, int intCount)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDiagEntrySave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intCount", intCount));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveDiagEntry(1, 1, 1, 1, diagnosisCodes, "", 1);

            mockDataAccess
            .Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                "prc_ISISMilestoneDiagEntrySave",
                It.IsAny<CommandType>(),
                It.IsAny<SqlParameter[]>(),
                ref It.Ref<List<SqlParameter>>.IsAny), Times.Exactly(diagnosisCodes.Split('|').Count()));

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveFacilityEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowProviderUpdate",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveFacilityEntry(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveFacilityEntry_WhenErrorCodeIsNotZero_ShouldNotInvokeSaveMilestone(int intResult)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowProviderUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intResult", intResult));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            var response = milestonesSave.SaveFacilityEntry(1, 1, 1, "", 1, "",1, 1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual(response, $"{intResult}|-1");

            mockDataAccess
            .Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                "prc_ISISWorkflowProviderUpdate",
                It.IsAny<CommandType>(),
                It.IsAny<SqlParameter[]>(),
                ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Never);
        }

        [Test]
        public void SaveFacilityEntry_WhenErrorCodeIsZero_ShouldNotInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowProviderUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intResult", 0));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            var response = milestonesSave.SaveFacilityEntry(1, 1, 1, "", 1, "", 1, 1);

            //Assert
            Assert.IsNotEmpty(response);

            Assert.AreEqual("0|",response);

            mockDataAccess
            .Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                "prc_ISISWorkflowProviderUpdate",
                It.IsAny<CommandType>(),
                It.IsAny<SqlParameter[]>(),
                ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveFinalAuth_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISIsMatchingPlanChangeFlowOpen",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveFinalAuth(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveFinalAuth_WhenErrorCodeIsNotZero_ShouldNotInvokeMilestoneFinalAuthSave(int intFlowopen)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISIsMatchingPlanChangeFlowOpen",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intFlowopen", intFlowopen));
                    }))
                .Returns(1);

            mockDataAccess
               .Setup(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISMilestoneFinalAuthSave",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny))
               .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveFinalAuth(1, 1, 1, 1, "", "", 1, 1);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISIsMatchingPlanChangeFlowOpen",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
               .Verify(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISMilestoneFinalAuthSave",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny), Times.Never);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveFinalAuth_WhenErrorCodeIsZero_ShouldInvokeMilestoneFinalAuthSave()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISIsMatchingPlanChangeFlowOpen",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intFlowopen", 0));
                    }))
                .Returns(1);

            mockDataAccess
               .Setup(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISMilestoneFinalAuthSave",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny))
               .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveFinalAuth(1, 1, 1, 1, "", "", 1, 1);

            //Assert
            
            mockDataAccess
            .Verify(x => x.ExecuteNonQuery(
                It.IsAny<string>(),
                "prc_ISISIsMatchingPlanChangeFlowOpen",
                It.IsAny<CommandType>(),
                It.IsAny<SqlParameter[]>(),
                ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
               .Verify(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISMilestoneFinalAuthSave",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveIntervStrategy_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISInterventionStrategyMilestoneSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveIntervStrategy(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveIntervStrategy_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISInterventionStrategyMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveIntervStrategy(1, 1, 1, "", 1, 1);

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISInterventionStrategyMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveMDSQ_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneMDSQSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveMDSQ(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveMDSQ_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveMDSQ(1, 1);

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

        }

        [Test]
        public void SaveMDSQReasons_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneMDSQReasonsSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveMDSQReasons(new Fixture().Create<MDSQReasonsMilestoneRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveMDSQReasons_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQReasonsSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveMDSQReasons(new Fixture().Create<MDSQReasonsMilestoneRequest>());

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQReasonsSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

        }

        [Test]
        public void SaveMedAcceptDeny_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneMedAcceptDeny",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveMedAcceptDeny(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveMedAcceptDeny_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMedAcceptDeny",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveMedAcceptDeny(1, 1, 1, "");

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMedAcceptDeny",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveMembersResponse_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMembersResponseMilestoneSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveMembersResponse(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveMembersResponse_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMembersResponseMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveMembersResponse(1, 1, 1, "");

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMembersResponseMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveOutofStateSkilled_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneOutofStateSkilledSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveOutofStateSkilled(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SaveOutofStateSkilled_WhenExecuted_ShouldInvokeSaveMilestone(int intCurrentMedicalServicesWorkerID)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneOutofStateSkilledSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intCurrentMedicalServicesWorkerID", intCurrentMedicalServicesWorkerID));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveOutofStateSkilled(1, 1, 1, "", 1, 1);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneOutofStateSkilledSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SavePACEWithDrawal_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestonePACEWithdrawalSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SavePACEWithDrawal(1, 1, 1, "", 1, 1, 0));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1)]
        [TestCase(2)]
        public void SavePACEWithDrawal_WhenExecuted_ShouldNotInvokePACEWithdrawalSave(int intCurrentMedicalServicesWorkerID)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intCurrentMedicalServicesWorkerID", intCurrentMedicalServicesWorkerID));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SavePACEWithDrawal(1, 1, 1, "",1, 0);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Never);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [TestCase(1,1)]
        [TestCase(1,2)]
        public void SavePACEWithDrawal_WhenExecuted_ShouldInvokePACEWithdrawalSave(int withDrawalInd, int intCurrentMedicalServicesWorkerID)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intCurrentMedicalServicesWorkerID", intCurrentMedicalServicesWorkerID));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SavePACEWithDrawal(1,1,1,"",1,withDrawalInd,1);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);


            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveRBSCLApproval_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISRBSCLUpdate",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveRBSCLApproval(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveRBSCLApproval_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISRBSCLUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveRBSCLApproval(1, 1, 1, "");

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISRBSCLUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveRSFinalAuth_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneRSFinalAuthSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveRSFinalAuth(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveRSFinalAuth_WhenExecuted_ShouldExecuteProc()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneRSFinalAuthSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveRSFinalAuth(1, 1, 1);

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneRSFinalAuthSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

        }

        [Test]
        public void SaveTierEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneTierEntrySave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveTierEntry(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveTierEntry_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneTierEntrySave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveTierEntry(1, 1, 1, "");

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneTierEntrySave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveUnnaturalDeath_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISUnnaturalDeathMilestoneSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveUnnaturalDeath(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveUnnaturalDeath_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISUnnaturalDeathMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveUnnaturalDeath(1, 1, 1, "");

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISUnnaturalDeathMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveWhatTriggered_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWhatTriggeredIntervMilestoneSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveWhatTriggered(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveWhatTriggered_WhenExecuted_ShouldInvokeSaveMilestone()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWhatTriggeredIntervMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.SaveWhatTriggered(1, 1, 1, "");

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWhatTriggeredIntervMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UpdateComment_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneCommentSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.UpdateComment(1, "", 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UpdateComment_WhenExecuted_ShouldExecuteProc()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCommentSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesSave.UpdateComment(1, "", 1);

            //Assert

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCommentSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

        }

        [Test]
        public void SaveTCMServiceAuth_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISTCMServiceAuthorizationSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesSave.SaveTCMServiceAuth(new Fixture().Create<TCMMilestoneRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(1234998)]
        [TestCase(1234999)]
        public void SaveTCMServiceAuth_WhenExecuted_ShouldInvokeCloseActiveMileStoneAsPerWorkflowPointResponseId(
            long workflowPointResponseID)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISTCMServiceAuthorizationSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))                
                .Returns(1);

            mockDataAccess
               .Setup(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISCloseActiveMilestones",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny))
               .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            var request = new Fixture().Create<TCMMilestoneRequest>();
            request.WorkflowPointResponseID = workflowPointResponseID;

            //Act
            milestonesSave.SaveTCMServiceAuth(request);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Never);

            mockDataAccess
               .Verify(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISCloseActiveMilestones",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [TestCase(1234900)]
        [TestCase(1234000)]
        public void SaveTCMServiceAuth_WhenExecuted_ShouldNotInvokeCloseActiveMileStoneAsPerWorkflowPointResponseId(
            long workflowPointResponseID)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISTCMServiceAuthorizationSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
               .Setup(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISCloseActiveMilestones",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny))
               .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            var request = new Fixture().Create<TCMMilestoneRequest>();
            request.WorkflowPointResponseID = workflowPointResponseID;

            //Act
            milestonesSave.SaveTCMServiceAuth(request);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Never);

            mockDataAccess
               .Verify(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISCloseActiveMilestones",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny), Times.Never);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveLOC_WhenDBThrowsException_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();


            var request = new Fixture().Create<LOCMilestoneRequest>();
            request.ForceServicePlanSplit = false;

            //Act
            Assert.Throws<Exception>(()=> milestonesSave.SaveLOC(request));

            //Assert
            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveLOC_WhenForceServicePlanSplitIsFalse_ShouldExecuteMilestoneLOCSave()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intServicePlanID", 1));
                        target.Add(new SqlParameter("@intArchiveID", 1));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            var request = new Fixture().Create<LOCMilestoneRequest>();
            request.ForceServicePlanSplit = false;

            //Act
            milestonesSave.SaveLOC(request);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveLOC_WhenForceServicePlanSplitIsTrue_ShouldExecuteMilestoneLOCSplitPlanSave()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitPlanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intServicePlanID", 0));
                        target.Add(new SqlParameter("@dteServicePlanStartDate", DateTime.Now));
                        target.Add(new SqlParameter("@dteServicePlanEndDate", DateTime.Now));
                        target.Add(new SqlParameter("@intNewServicePlanID", 1));
                        target.Add(new SqlParameter("@dtNewServicePlanStartDate", DateTime.Now));
                        target.Add(new SqlParameter("@dtNewServicePlanEndDate", DateTime.Now));
                        target.Add(new SqlParameter("@intArchiveID", 1));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCProgramRequestSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            var request = new Fixture().Create<LOCMilestoneRequest>();
            request.ForceServicePlanSplit = true;

            //Act
            milestonesSave.SaveLOC(request);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitPlanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCProgramRequestSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveLOC_WhenServicePlanIDIsNotZero_ShouldExecuteISISServiceSpansGet()
        {
            //Arrange
            var servicePlanStartDate = DateTime.Now.AddDays(-1);
            var servicePlanEndDate = DateTime.Now.AddDays(5);
            var serviceSpanStartDate = DateTime.Now.AddDays(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitPlanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intServicePlanID", 1));
                        target.Add(new SqlParameter("@dteServicePlanStartDate", servicePlanStartDate));
                        target.Add(new SqlParameter("@dteServicePlanEndDate", servicePlanEndDate));
                        target.Add(new SqlParameter("@intNewServicePlanID", 1));
                        target.Add(new SqlParameter("@dtNewServicePlanStartDate", DateTime.Now));
                        target.Add(new SqlParameter("@dtNewServicePlanEndDate", DateTime.Now));
                        target.Add(new SqlParameter("@intArchiveID", 1));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"svs_RateStartDate", serviceSpanStartDate },
                    {"svs_RateEndDate", DateTime.Now.AddDays(1) },
                    {"svs_ServiceSpanID", 11 }
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSpanReAssign",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            var request = new Fixture().Create<LOCMilestoneRequest>();
            request.ForceServicePlanSplit = true;

            //Act
            milestonesSave.SaveLOC(request);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitPlanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

           
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSpanReAssign",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveLOC_WhenServiceSpanStartGreaterThanServicePlanStart_ShouldExecuteIMilestoneLOCSplitPlan()
        {
            //Arrange
            var servicePlanStartDate = DateTime.Now.AddDays(2);
            var servicePlanEndDate = DateTime.Now.AddDays(5);
            var serviceSpanStartDate = DateTime.Now.AddDays(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitPlanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Callback(new MockRefCallback((string connectionString,
                    string procName,
                    CommandType commandType,
                    SqlParameter[] sqlParams,
                    ref List<SqlParameter> target) => {
                        target = new List<SqlParameter>();
                        target.Add(new SqlParameter("@intServicePlanID", 1));
                        target.Add(new SqlParameter("@dteServicePlanStartDate", servicePlanStartDate));
                        target.Add(new SqlParameter("@dteServicePlanEndDate", servicePlanEndDate));
                        target.Add(new SqlParameter("@intNewServicePlanID", 1));
                        target.Add(new SqlParameter("@dtNewServicePlanStartDate", DateTime.Now));
                        target.Add(new SqlParameter("@dtNewServicePlanEndDate", DateTime.Now));
                        target.Add(new SqlParameter("@intArchiveID", 1));
                    }))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"svs_RateStartDate", serviceSpanStartDate },
                    {"svs_RateEndDate", DateTime.Now.AddDays(1) },
                    {"svs_ServiceSpanID", 11 }
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitSpan",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            var request = new Fixture().Create<LOCMilestoneRequest>();
            request.ForceServicePlanSplit = true;

            //Act
            milestonesSave.SaveLOC(request);

            //Assert
            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitPlanSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISServiceSpansGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCSplitSpan",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]        
        public void SavecMCareLOC_WhenDBThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneDeleteMedicalServicesRole",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Throws<Exception>();

            //Act and assert
            Assert.Throws<Exception>(() => milestonesSave.SavecMCareLOC(1, 1, 1, "", 9, 1));

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneDeleteMedicalServicesRole",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(9)]
        [TestCase(1)]
        [TestCase(0)]
        public void SavecMCareLOC_WhenExecutes_ShouldExecuteProcedure(int levelCareId)
        {
            //Arrange
            var dataReader = new Mock<IDataReader>();
            dataReader.Setup(m => m.FieldCount).Returns(1);

            dataReader.Setup(m => m.GetString(0)).Returns("First"); 
            dataReader.Setup(m => m.GetString(1)).Returns("Second");
            dataReader.Setup(m => m.GetString(2)).Returns("Three");

            dataReader.Setup(m => m.GetFieldType(0)).Returns(typeof(string));
            dataReader.Setup(m => m.GetFieldType(1)).Returns(typeof(string));
            dataReader.Setup(m => m.GetFieldType(2)).Returns(typeof(string));

            dataReader
                .SetupSequence(x => x.Read())
                .Returns(true)
                .Returns(false);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneDeleteMedicalServicesRole",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.GetDataReader(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMCARELOCSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(dataReader.Object);           

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            var response = milestonesSave.SavecMCareLOC(1, 1, 1, "", levelCareId, 1);

            //Assert
            if (levelCareId > 0)
            {
                if (levelCareId == 9)
                {
                    mockDataAccess
                        .Verify(x => x.ExecuteNonQuery(
                            It.IsAny<string>(),
                            "prc_ISISWorkflowMilestoneDeleteMedicalServicesRole",
                            It.IsAny<CommandType>(),
                            It.IsAny<SqlParameter[]>(),
                            ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
                }

                mockDataAccess
                    .Verify(x => x.GetDataReader(
                        It.IsAny<string>(),
                        "prc_ISISMilestoneMCARELOCSave",
                        It.IsAny<CommandType>(),
                        It.IsAny<SqlParameter[]>()), Times.Once);

                Assert.IsNotEmpty(response);
            }
            else
            {
                Assert.IsEmpty(response);
            }

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SavecDeleteRoles_WhenDBThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataReader(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneAssignmentDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws<Exception>();

            //Act and assert
            Assert.Throws<Exception>(() => milestonesSave.SaveDeleteRoles(1,1,1,"",1));

            mockDataAccess
                .Verify(x => x.GetDataReader(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneAssignmentDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SavecDeleteRoles_WhenExecutes_ShouldExecuteProcedure()
        {
            //Arrange
            var dataReader = new Mock<IDataReader>();
            dataReader.Setup(m => m.FieldCount).Returns(1);

            dataReader.Setup(m => m.GetString(0)).Returns("First");
            dataReader.Setup(m => m.GetString(1)).Returns("Second");
            dataReader.Setup(m => m.GetString(2)).Returns("Three");

            dataReader.Setup(m => m.GetFieldType(0)).Returns(typeof(string));
            dataReader.Setup(m => m.GetFieldType(1)).Returns(typeof(string));
            dataReader.Setup(m => m.GetFieldType(2)).Returns(typeof(string));

            dataReader
                .SetupSequence(x => x.Read())
                .Returns(true)
                .Returns(false);

           
            mockDataAccess
                .Setup(x => x.GetDataReader(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneAssignmentDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(dataReader.Object);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            var response = milestonesSave.SaveDeleteRoles(1, 1, 1, "", 1);

            //Assert
            Assert.IsNotEmpty(response);
           
            mockDataAccess
                .Verify(x => x.GetDataReader(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneAssignmentDelete",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }
    }
}
