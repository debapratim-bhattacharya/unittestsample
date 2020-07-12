using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    /// <summary>
    /// Author: Praveen Vattela  
    /// Crated Date: 06/19/2020
    /// Modified Date: 06/22/2020 /06/24/20(Satish)
    /// Class Name: Milestone
    /// </summary>
    /// <remarks>
    /// Longer comments can be associated with a type or member through
    /// the remarks tag.
    /// </remarks>        

    public class Milestone : IMilestone
    {
        IDataAccess dataAccess;
        IExceptionHandling exceptionHandling;
        ILogin login;
        public Milestone(IDataAccess access, IExceptionHandling exception, ILogin loginDetails)
        {
            dataAccess = access;
            exceptionHandling = exception;
            login = loginDetails;
        }

        /// <summary>
        /// Build Milestone...
        /// </summary>
        /// <param name="workflowPointTemplateId"></param>
        /// <param name="mileStoneType"></param>
        /// <param name="roleId"></param>
        /// <param name="questionId"></param>
        /// <param name="daystoRespond"></param>
        public void BuildMilestone(int workflowPointTemplateId, int mileStoneType, int roleId, int questionId, int daystoRespond)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowPointTemplateID", SqlDbType.Int) { Value = workflowPointTemplateId },
                    new SqlParameter("@lngMilestoneType", SqlDbType.Int) { Value = mileStoneType },
                    new SqlParameter("@lngRoleID", SqlDbType.Int) { Value = roleId },
                    new SqlParameter("@lngQuestionID", SqlDbType.Int) { Value = questionId },
                    new SqlParameter("@lngDaystoRespond", SqlDbType.Int) { Value = daystoRespond}
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneBuild", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Build Predecessor...
        /// </summary>
        /// <param name="workflowPredecessorId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="workflowPointTemplateId"></param>
        /// <param name="andOrInd"></param>
        public void BuildPredecessor(int workflowPredecessorId, int responseTemplateId, int workflowPointTemplateId, int andOrInd)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowPredecessorID", SqlDbType.Int) { Value = workflowPredecessorId },
                    new SqlParameter("@lngResponseTemplateID", SqlDbType.Int) { Value = responseTemplateId },
                    new SqlParameter("@lngWorkflowPointTemplateID", SqlDbType.Int) { Value = workflowPointTemplateId },
                    new SqlParameter("@lngAndORInd", SqlDbType.Int) { Value = andOrInd }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISPredecessorBuild", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// BuildResponse
        /// </summary>
        /// <param name="workflowResponseId"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="response"></param>
        public void BuildResponse(int workflowResponseId, int responseTemplateId, string response)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowResponseID", SqlDbType.Int) { Value = workflowResponseId },
                    new SqlParameter("@lngResponseTemplateID", SqlDbType.Int) { Value = responseTemplateId },
                    new SqlParameter("@strResponse", SqlDbType.VarChar,100) { Value = response }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISResponseBuild", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }


        /// <summary>
        /// Close All Outstanding Milestones for program request.
        /// </summary>
        /// <param name="isisSession"></param>
        /// <param name="programRequestId"></param>
        /// <param name="closureReason"></param>
        public void CloseMilestones(long isisSession, int programRequestId, int closureReason)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intISISSession", SqlDbType.Int) { Value = isisSession },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = programRequestId },
                    new SqlParameter("@intClosureResponse", SqlDbType.Int) { Value = closureReason }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestonesClose", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Delete Workflow
        /// </summary>
        /// <param name="workflowNumber"></param>
        /// <returns></returns>
        public string DeleteWorkflow(int workflowNumber)
        {
            string workflowResult;
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowNumber", SqlDbType.Int) { Value = workflowNumber },
                    new SqlParameter("@intRETURN", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowDelete", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var errCode = 0;
                foreach (var parameter in outputParameters)
                {
                    switch (parameter.ParameterName)
                    {
                        case "@intRETURN":
                            errCode = Convert.ToInt32(parameter.Value);
                            break;
                    }
                }
                workflowResult = errCode.ToString();
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return workflowResult;
        }

        /// <summary>
        /// Get a listing of the all the causes of death.
        /// </summary>
        /// <returns></returns>
        public DataTable GetCausesofDeath()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISCausesofDeathGet", CommandType.StoredProcedure, null);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve milestone information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <returns></returns>
        public DataTable GetMilestoneInfo(int workflowPointProcessId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int) { Value = workflowPointProcessId }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneResponsesGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve all the milestone questions  on the table
        /// </summary>
        /// <param name="milestoneNumber"></param>
        /// <returns></returns>
        public DataTable GetMilestoneQuestion(int milestoneNumber = 0)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneQuestionNumber", SqlDbType.Int) { Value = milestoneNumber }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneQuestionGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// GetMilestoneQuestions...
        /// </summary>
        /// <returns></returns>
        public DataTable GetMilestoneQuestions()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneQuestionsGet", CommandType.StoredProcedure, null);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// .
        /// Retrieve all the services on the table
        /// </summary>
        /// <param name="workflowTemplateId"></param>
        /// <returns></returns>
        public DataTable GetMilestones(int workflowTemplateId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowTemplateID", SqlDbType.Int) { Value = workflowTemplateId }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMilestonesGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve all the milestone questions  on the table
        /// </summary>
        /// <param name="milestoneTypeNumber"></param>
        /// <returns></returns>
        public DataTable GetMilestoneType(int milestoneTypeNumber = 0)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneTypeNumber", SqlDbType.Int) { Value = milestoneTypeNumber }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneTypeGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve all the milestone questions  on the table
        /// </summary>
        /// <returns></returns>
        public DataTable GetMilestoneTypes()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneTypesGet", CommandType.StoredProcedure, null);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Get all predecessor records for a supplied work flow template Id
        /// </summary>
        /// <param name="workflowTemplateId"></param>
        /// <returns></returns>
        public DataTable GetPredecessors(int workflowTemplateId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowTemplateID", SqlDbType.Int) { Value = workflowTemplateId }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISPredecessorsGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Get all Responses records for a supplied work flow templateID
        /// </summary>
        /// <param name="workflowTemplateId"></param>
        /// <returns></returns>
        public DataTable GetResponses(int workflowTemplateId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowTemplateID", SqlDbType.Int) { Value = workflowTemplateId }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISResponsesGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve workflow status..
        ///  Removed Unused "StateId" input parameter.
        /// </summary>
        /// <param name="programRequestId"></param>
        /// <returns></returns>
        public DataTable GetStatus(int programRequestId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequest", SqlDbType.Int) { Value = programRequestId }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowStatusGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Fire a change flow
        /// </summary>
        /// <param name="changeEvent"></param>
        /// <param name="sessionId"></param>
        /// <param name="data"></param>
        /// <param name="customData"></param>
        public void RaiseChangeEvent(string changeEvent, long sessionId, int data, string customData = "")
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strChangeEvent", SqlDbType.VarChar,20) { Value = changeEvent },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = sessionId },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = data },
                    new SqlParameter("@strCustomData", SqlDbType.VarChar,256) { Value = customData }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISChangeEventRaise", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Save Milestone Question
        /// </summary>
        /// <param name="milestoneNumber"></param>
        /// <param name="milestoneWording"></param>
        /// <returns></returns>
        public string SaveMilestoneQuestion(int milestoneNumber, string milestoneWording)
        {
            string workflowResult;
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngMilestoneNumber", SqlDbType.Int) { Value = milestoneNumber },
                    new SqlParameter("@strMilestoneWording", SqlDbType.VarChar,1000) { Value = milestoneWording },
                    new SqlParameter("@lngNewMilestoneNumber", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneQuestionSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var newMilestoneNumber = 0;
                foreach (var parameter in outputParameters)
                {
                    switch (parameter.ParameterName)
                    {
                        case "@lngNewMilestoneNumber":
                            newMilestoneNumber = Convert.ToInt32(parameter.Value);
                            break;
                    }
                }
                workflowResult = newMilestoneNumber.ToString();
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return workflowResult;
        }

        /// <summary>
        /// SaveMilestoneType
        /// </summary>
        /// <param name="milestoneType"></param>
        /// <param name="processingPage"></param>
        /// <returns></returns>
        public string SaveMilestoneType(string milestoneType, string processingPage)
        {
            string workflowResult;
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strDescription", SqlDbType.VarChar,15) { Value = milestoneType },
                    new SqlParameter("@strProcessingPage", SqlDbType.VarChar,50) { Value = processingPage },
                    new SqlParameter("@lngNewMilestoneNumber", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneTypeSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var newMilestoneNumber = 0;
                foreach (var parameter in outputParameters)
                {
                    switch (parameter.ParameterName)
                    {
                        case "@lngNewMilestoneNumber":
                            newMilestoneNumber = Convert.ToInt32(parameter.Value);
                            break;
                    }
                }
                workflowResult = newMilestoneNumber.ToString();
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return workflowResult;
        }

        /// <summary>
        /// SavePredecessors
        /// </summary>
        /// <param name="workflowTemplateId"></param>
        /// <param name="predecessorCount"></param>
        public void SavePredecessors(int workflowTemplateId, int predecessorCount)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowTemplateID", SqlDbType.Int) { Value = workflowTemplateId },
                    new SqlParameter("@lngPredecessorCount", SqlDbType.Int) { Value = predecessorCount }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISPredecessorsSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// SaveResponses
        /// </summary>
        /// <param name="workflowTemplateId"></param>
        /// <param name="responseCount"></param>
        public void SaveResponses(int workflowTemplateId, int responseCount)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowTemplateID", SqlDbType.Int) { Value = workflowTemplateId },
                    new SqlParameter("@lngResponseCount", SqlDbType.Int) { Value = responseCount }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISResponsesSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// SaveWorkflow...
        /// </summary>
        /// <param name="workflowNumber"></param>
        /// <param name="workflowCode"></param>
        /// <param name="milestoneCount"></param>
        /// <param name="workflowName"></param>
        /// <returns></returns>
        public string SaveWorkflow(int workflowNumber, string workflowCode, int milestoneCount, string workflowName)
        {
            string workflowResult;
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngWorkflowNumber", SqlDbType.Int) { Value = workflowNumber },
                    new SqlParameter("@lngWorkflowCode", SqlDbType.VarChar,6) { Value = workflowCode },
                    new SqlParameter("@lngMilestoneCount", SqlDbType.Int) { Value = milestoneCount },
                    new SqlParameter("@strWorkflowName", SqlDbType.VarChar,100) { Value = workflowName },
                    new SqlParameter("@intNewWorkflowTemplateID", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var newWorkflowTemplateId = 0;
                foreach (var parameter in outputParameters)
                {
                    switch (parameter.ParameterName)
                    {
                        case "@intNewWorkflowTemplateID":
                            newWorkflowTemplateId = Convert.ToInt32(parameter.Value);
                            break;
                    }
                }
                workflowResult = newWorkflowTemplateId.ToString();
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return workflowResult;
        }

        /// <summary>
        /// Unlock All Milestones for program request.
        /// </summary>
        /// <param name="programRequestId"></param>
        /// <param name="isisSession"></param>
        public void UnlockMilestones(int programRequestId, long isisSession)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = programRequestId },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = isisSession }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestonesUnlockQA", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public void UnlockMilestone(int workflowPointProcessId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int) { Value = workflowPointProcessId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneUnLock", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public void LockMilestone(int workflowPointProcessId, int workerId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int) { Value = workflowPointProcessId },
                    new SqlParameter("@intWorkerID", SqlDbType.Int) { Value = workerId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneLock", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

    }
}
