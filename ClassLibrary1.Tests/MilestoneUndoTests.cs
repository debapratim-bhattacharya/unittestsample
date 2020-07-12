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
    public class MilestoneUndoTests
    {
        private Mock<IExceptionHandling> mockExceptionHandling;
        private Mock<IDataAccess> mockDataAccess;

        private MilestonesUndo milestonesUndo;

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

        [SetUp]
        public void SetUp()
        {
            mockExceptionHandling = new Mock<IExceptionHandling>();

            mockDataAccess = new Mock<IDataAccess>();

            mockExceptionHandling
                .Setup(x => x.LogException(It.IsAny<Exception>(), ExceptionPolicy.Web_Exception))
                .Verifiable();

            milestonesUndo = new MilestonesUndo(mockDataAccess.Object, mockExceptionHandling.Object);
        }

        [Test]
        public void Undo_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.Undo(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0,1,1)]
        [TestCase(0, 1, null)]
        public void Undo_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.Undo(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoPace_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestonePACEUnDo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoPace(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1)]
        [TestCase(0, null)]
        public void UndoPace_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEUnDo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoPace(workerPointProcessId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEUnDo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoAllRolesAssigned_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoAllRolesAssigned(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1, "1,2,3")]
        [TestCase(0, 1, null, "1,2,3|1,2,3")]
        public void UndoAllRolesAssigned_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId, string wppCustomData)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"wpp_CustomData",  wppCustomData}
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneVerifyAllRolesAssignedUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
                milestonesUndo.UndoAllRolesAssigned(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneVerifyAllRolesAssignedUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Exactly(wppCustomData.Split('|').Count()));

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoARODiagLPHA_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoARODiagLPHA(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1, "01/01/1900")]
        [TestCase(0, 1, null, "01/12/2020")]
        public void UndoARODiagLPHA_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId, string wppCustomData)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"wpp_CustomData",  wppCustomData}
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneARODiagLPHAUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoARODiagLPHA(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneARODiagLPHAUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoCancelDecision_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneCancelDecisionUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoCancelDecision(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoCancelDecision_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCancelDecisionUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoCancelDecision(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCancelDecisionUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoCauseofDeath_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoCauseofDeath(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoCauseofDeath_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoCauseofDeath(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoClosureEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneClosureReasonUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoClosureEntry(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoClosureEntry_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneClosureReasonUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoClosureEntry(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneClosureReasonUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoCOLSArbitration_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneCOLSArbitrationUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoCOLSArbitration(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoCOLSArbitration_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSArbitrationUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoCOLSArbitration(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSArbitrationUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoCOLS_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneCOLSUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoCOLS(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoCOLS_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoCOLS(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneCOLSUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoDateEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneDateEntryUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoDateEntry(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoDateEntry_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDateEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoDateEntry(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDateEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoClosureEntryOverload_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneDCNEntryUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoClosureEntry(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoClosureEntryOverload_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDCNEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoClosureEntry(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDCNEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoDeleteRoles_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoDeleteRoles(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1, "1,2,3")]
        [TestCase(0, 1, null, "1,2,3|1,2,3")]
        public void UndoDeleteRoles_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId, string wppCustomData)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"wpp_CustomData",  wppCustomData}
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneAcceptDenyUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoDeleteRoles(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneAcceptDenyUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Exactly(wppCustomData.Split('|').Count()));

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoDiagEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneDiagEntryUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoDiagEntry(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoDiagEntry_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDiagEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoDiagEntry(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneDiagEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }


        [Test]
        public void UndoFacilityEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoFacilityEntry(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoFacilityEntry_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"pgr_ProgramRequestID",  1},
                    {"wpt_workflowpointtypeid",  2}
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowProviderUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
               .Setup(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISWorkflowMilestoneUndo",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny))
               .Returns(1);

            //Act
            milestonesUndo.UndoFacilityEntry(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowProviderUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny),Times.Once);

            mockDataAccess
               .Verify(x => x.ExecuteNonQuery(
                   It.IsAny<string>(),
                   "prc_ISISWorkflowMilestoneUndo",
                   It.IsAny<CommandType>(),
                   It.IsAny<SqlParameter[]>(),
                   ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoFinalAuth_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoFinalAuth(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1, "1|01/01/1900")]
        [TestCase(0, 1, null, "1,2,3|01/01/1900")]
        public void UndoFinalAuth_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId, string wppCustomData)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"wpp_CustomData",  wppCustomData}
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneFinalAuthUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoFinalAuth(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneFinalAuthUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Exactly(wppCustomData.Split(',').Count()));

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoInterventionStrategy_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoInterventionStrategy(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoInterventionStrategy_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoInterventionStrategy(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoLOC_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoLOC(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UndoLOC_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoLOC(1,1,1,1,1,1,"01/01/2000", "01/01/2000", "01/01/2000",1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneLOCUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoMCareLOC_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoMCareLOC(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase("Medical Services")]
        [TestCase("Physician Review")]
        [TestCase("IME Medical Services for Mental Health")]
        [TestCase("Medical Services, for members not currently enrolled in Medicaid & Iowa Plan")]
        public void UndoMCareLOC_WhenWprResponseMatch_ShouldExecuteOnlyUndo(
            string wpr_responseDescription)
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Returns(MockDataTable(new Dictionary<string, object>
                 {
                     { "wpr_ResponseDescription", wpr_responseDescription },
                     { "prg_ProgramType", "some program type"},
                     { "wpp_CustomData", "some program type"}
                 }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoMCareLOC(1,1,1);

            mockDataAccess
                 .Verify(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [TestCase("something different","8", "14,14,1|12,1,1")]
        [TestCase("something different", "8", "1,14,1|14,10,10")]
        public void UndoMCareLOC_WhenWprResponseDoesntMatch_ShouldExecuteMCareLOCUndo(
            string wpr_responseDescription, string programType, string wpp_CustomData)
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Returns(MockDataTable(new Dictionary<string, object>
                 {
                     { "wpr_ResponseDescription", wpr_responseDescription },
                     { "prg_ProgramType", programType },
                     { "wpp_CustomData", wpp_CustomData }
                 }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMCareLOCUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoMCareLOC(1, 1, 1);

            mockDataAccess
                 .Verify(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMCareLOCUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoMDSQ_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneMDSQUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoMDSQ(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoMDSQ_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoMDSQ(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoMDSQReasons_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneMDSQReasonsUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoMDSQReasons(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UndoMDSQReasons_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQReasonsUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoMDSQReasons(1, 1, 1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMDSQReasonsUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoMedAcceptDeny_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneMedAcceptDenyUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoMedAcceptDeny(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UndoMedAcceptDeny_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMedAcceptDenyUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoMedAcceptDeny(1, 1, 1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneMedAcceptDenyUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoMembersResponse_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoMembersResponse(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoMembersResponse_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoMembersResponse(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoOutofStateSkilled_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneOutofStateSkilledUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoOutofStateSkilled(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UndoOutofStateSkilled_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneOutofStateSkilledUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoOutofStateSkilled(1, 1, 1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneOutofStateSkilledUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoPACEWithDrawal_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestonePACEWithdrawalUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoPACEWithDrawal(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UndoPACEWithDrawal_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoPACEWithDrawal(1, 1, 1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestonePACEWithdrawalUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoRBSCLApproval_WhenDbThrowsError_ShouldThrowException()
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
                milestonesUndo.UndoRBSCLApproval(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UndoRBSCLApproval_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISRBSCLUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoRBSCLApproval(1, 1, 1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISRBSCLUpdate",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoRSFinalAuth_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.GetDataTable(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneInfoGet",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>()))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoAllRolesAssigned(1, 1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1, "1,2,3")]
        [TestCase(0, 1, null, "1,2,3|1,2,3")]
        public void UndoRSFinalAuth_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId, string wppCustomData)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()))
                .Returns(MockDataTable(new Dictionary<string, object>
                {
                    {"wpp_CustomData",  wppCustomData}
                }));

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneRSFinalAuthUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoRSFinalAuth(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.GetDataTable(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneInfoGet",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>()), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneRSFinalAuthUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Exactly(wppCustomData.Split('|').Count()));

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoTCMServiceAuth_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoTCMServiceAuth(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoTCMServiceAuth_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoTCMServiceAuth(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoTierEntry_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneTierEntryUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoTierEntry(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void UndoTierEntry_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneTierEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoTierEntry(1, 1, 1);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneTierEntryUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoUnNaturalDeath_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoUnNaturalDeath(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoUnNaturalDeath_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoUnNaturalDeath(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void UndoWhatTriggered_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISWorkflowMilestoneUndo",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.UndoWhatTriggered(1, 1));

            mockExceptionHandling.VerifyAll();
        }

        [TestCase(0, 1, 1)]
        [TestCase(0, 1, null)]
        public void UndoWhatTriggered_WhenExecuted_ShouldExecuteProcedure(
            long workerPointProcessId, long workerId, long sessionId)
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.UndoWhatTriggered(workerPointProcessId, workerId, sessionId);

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISWorkflowMilestoneUndo",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }

        [Test]
        public void SaveUndoAudit_WhenDbThrowsError_ShouldThrowException()
        {
            //Arrange
            mockDataAccess
                 .Setup(x => x.ExecuteNonQuery(
                     It.IsAny<string>(),
                     "prc_ISISMilestoneUndoAuditSave",
                     It.IsAny<CommandType>(),
                     It.IsAny<SqlParameter[]>(),
                     ref It.Ref<List<SqlParameter>>.IsAny))
                 .Throws<Exception>();

            //Act and Assert
            Assert.Throws<Exception>(() =>
                milestonesUndo.SaveUndoAudit(new Fixture().Create<UndoAuditRequest>()));

            mockExceptionHandling.VerifyAll();
        }

        [Test]
        public void SaveUndoAudit_WhenExecuted_ShouldExecuteProcedure()
        {
            //Arrange
            mockDataAccess
                .Setup(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneUndoAuditSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny))
                .Returns(1);

            //Act
            milestonesUndo.SaveUndoAudit(new Fixture().Create<UndoAuditRequest>());

            mockDataAccess
                .Verify(x => x.ExecuteNonQuery(
                    It.IsAny<string>(),
                    "prc_ISISMilestoneUndoAuditSave",
                    It.IsAny<CommandType>(),
                    It.IsAny<SqlParameter[]>(),
                    ref It.Ref<List<SqlParameter>>.IsAny), Times.Once);
        }
    }
}
