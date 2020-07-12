using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{   
        /// <summary>
        /// Author: Satish Yeluri  
        /// Crated Date: 06/22/2020, 6/23/2020 
        /// Modified Date: 06/24/2020
        /// Save Answered Milestones
        /// </summary>   
        public class MilestonesSave : IMilestonesSave
        {
            DateTime deFaultDate = DateTime.ParseExact("01/01/1900", "MM/dd/yyyy", CultureInfo.InvariantCulture);
            IDataAccess dataAccess;
            IExceptionHandling exceptionHandling;
            ILogin login;
            public MilestonesSave(IDataAccess access, IExceptionHandling exception, ILogin loginDetails)
            {
                dataAccess = access;
                exceptionHandling = exception;
                login = loginDetails;
            }

            /// <summary>
            ///  COM: Save cMilestone.Save.
            ///  Save General Milestone Information.
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="data"></param>
            /// <param name="sessionId"></param>
            /// <param name="beginEndCode"></param>
            /// <returns></returns>
            public void SaveMilestone(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                                string comments = "", string data = "", long sessionId = 0, string beginEndCode = "")
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int) { Value = workflowPointProcessID },
                    new SqlParameter("@intWorkflowPointResponseTemplateID", SqlDbType.Int) { Value = workflowPointResponseID },
                    new SqlParameter("@intWorkerID", SqlDbType.Int) { Value = workerID },
                    new SqlParameter("@strComments", SqlDbType.VarChar,800) { Value = comments },
                    new SqlParameter("@strData", SqlDbType.VarChar,256) { Value = data },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = sessionId },
                    new SqlParameter("@strBeginEndCode", SqlDbType.VarChar,3) { Value = beginEndCode }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cAllRolesAssignedMilestone.Save
            /// Save AllRoles Assigned Milestone information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="data"></param>
            /// <param name="responseType"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveAllRolesAssigned(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                                string comments = "", string data = "", int responseType = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();

                    //Verify Role Assignments
                    if (responseType == 1)
                    {
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                        new SqlParameter("@intCount", SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };

                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISWorkflowMilestoneAssignmentCheck", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                        var intCount = 0;
                        foreach (var parameter in outputParameters)
                        {
                            if (parameter.ParameterName == "@intCount") intCount = Convert.ToInt16(parameter.Value);
                        }
                        if (intCount > 0)
                        {
                            var errorDescription = login.GetErrorString(501);
                        }
                    }
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, data, sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cARODiagLPHAMilestone.Save
            /// Save ARO Diag LPHA Milestone information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="programRequestId"></param>
            /// <param name="workerID"></param>
            /// <param name="diagnosisCodes"></param>
            /// <param name="comments"></param>
            /// <param name="lphaCertDate"></param>
            /// <param name="oldLPHACertDate"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveARODiagLPHA(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long programRequestId = 0, long workerID = 0,
                                                string diagnosisCodes = "", string comments = "", string lphaCertDate = "01/01/1900",
                                                string oldLPHACertDate = "01/01/1900", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    //Pull Diagnosis Codes out of string
                    var diagnosisCodesList = new List<string>(diagnosisCodes.Split('|'));
                    //If loopCount = 0=> In stored procedure: Update the PR with LPHA Cert date info and update any Service Plan if present
                    int loopCount = 0;
                    foreach (string diagCode in diagnosisCodesList)
                    {
                        DateTime lphaCertificationDate = DateTime.Parse(lphaCertDate);
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                        new SqlParameter("@intDiagnosisCodeID", SqlDbType.Int,4) { Value = Convert.ToInt32 (diagCode) },
                        new SqlParameter("@dteLPHACertDate", SqlDbType.DateTime) { Value = lphaCertificationDate },
                        new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerID },
                        new SqlParameter("@intLoopCount", SqlDbType.Int,4) { Value = loopCount}
                    };
                        loopCount += 1;
                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneARODiagLPHASave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    }

                    DateTime oldLPHACertificatonDate = DateTime.Parse(oldLPHACertDate);
                    if (oldLPHACertificatonDate > deFaultDate)
                    {
                        SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, oldLPHACertDate, sessionId);
                    }
                    else
                    {
                        SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                    }
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cCancelDecisionMilestone.Save
            /// Save Cancel Decision Milestone information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="accept"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveCancelDecision(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, bool accept = true, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    //Delete Role Assignments
                    if (accept == false)
                    {
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@int_ProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                        new SqlParameter("@int_ResponseTemplateID", SqlDbType.Int,4) { Value = 9999997 },
                        new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                    };

                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISCloseActiveMilestones", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    }

                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cCapIncreaseMilestone.Save
            /// Save CapIncrease Milestone information        
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="monthlyCap"></param>
            /// <param name="beginDate"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public string SaveCapIncrease(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, Decimal monthlyCap = 0, string beginDate = "01/01/1900", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@curCapAmount", SqlDbType.Int,4) { Value = monthlyCap },
                    new SqlParameter("@dteBeginDate", SqlDbType.Int,4) { Value = beginDate },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId },
                    new SqlParameter("@intRETURN", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneCapIncreaseSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var errorCode = 0;
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intRETURN") errorCode = Convert.ToInt16(parameter.Value);
                    }

                    if (errorCode != 0)
                    {
                        return errorCode + "|-1";
                    }
                    else
                    {
                        SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cCauseofDeathMilestone.Save
            /// Save CauseofDeath Milestone Information
            /// This stored procedure does not exists in database. that means this functionality does not work.
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="primaryCauseofDeath"></param>
            /// <param name="secondaryCauseofDeath"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveCauseofDeath(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, int primaryCauseofDeath = 0, int secondaryCauseofDeath = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@intPrimaryCauseofDeath", SqlDbType.Int,4) { Value = primaryCauseofDeath },
                    new SqlParameter("@intSecondaryCauseofDeath", SqlDbType.Int, 4) { Value = secondaryCauseofDeath },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISCauseOfDeathMilestoneSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cClosureEntryMilestone.Save
            /// Save ClosureEntry Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="programRequestId"></param>
            /// <param name="workerID"></param>
            /// <param name="closureReasonID"></param>
            /// <param name="closureReason"></param>
            /// <param name="comments"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveClosureEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long programRequestId = 0, long workerID = 0,
                                           int closureReasonID = 0, string closureReason = "", string comments = "", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@intClosureReasonID", SqlDbType.Int,4) { Value = closureReasonID },
                    new SqlParameter("@intWorkerID", SqlDbType.Int, 4) { Value = workerID }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneClosureReasonSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    closureReason = comments + " ---- Closure Reason: " + closureReason;
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, closureReason, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cCOLSArbitrationMilestone.Save
            /// Save COLS Arbitration Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="countyOfLegalSettlementID"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveCOLSArbitration(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", int countyOfLegalSettlementID = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intOrganizationID", SqlDbType.Int,4) {Direction = ParameterDirection.InputOutput, Value = countyOfLegalSettlementID },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneCOLSArbitrationSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    //Pick up old county ID
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intOrganizationID") countyOfLegalSettlementID = Convert.ToInt16(parameter.Value);
                    }

                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, Convert.ToString(countyOfLegalSettlementID), sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cCOLSMilestone.Save
            /// Save COLS Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="countyOfLegalSettlementID"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveCOLS(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", int countyOfLegalSettlementID = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intOrganizationID", SqlDbType.Int,4) {Direction = ParameterDirection.InputOutput, Value = countyOfLegalSettlementID },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneCOLSSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    //Pick up old county ID
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intOrganizationID") countyOfLegalSettlementID = Convert.ToInt16(parameter.Value);
                    }

                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, Convert.ToString(countyOfLegalSettlementID), sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cDateEntryMilestone.Save
            /// Save DateEntry Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestID"></param>
            /// <param name="dateType"></param>
            /// <param name="captureDate"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveDateEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestID = 0, int dateType = 0,
                                            string captureDate = "01/01/1900", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestID },
                    new SqlParameter("@intDateType", SqlDbType.Int,4) { Value = dateType },
                    new SqlParameter("@dteDate", SqlDbType.DateTime) { Value = DateTime.Parse(captureDate) },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneDateEntrySave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cDCNEntryMilestone.Save
            /// Save DCN Entry Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestID"></param>
            /// <param name="dcn"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveDCNEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestID = 0, long dcn = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestID },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerID },
                    new SqlParameter("@intDCN", SqlDbType.Int,4) { Value = dcn },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneDCNEntrySave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cDeleteRolesMilestone.Save
            /// Save Delete Roles Milestone Information
            /// COM object is using data but programrequestid is passing from asp page. 
            /// Removed unused this parameter.
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public string SaveDeleteRoles(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                };

                    string existingRoles = "";
                    //TODO: Need to findout how to code ADO Getstring in ADO.NET
                    //var strExistingRoles = RS.GetString(adClipString, , ",", "|");
                    IDataReader reader = dataAccess.GetDataReader(ApplicationConstants.IsisConnectionString, "prc_ISISWorkflowMilestoneAssignmentDelete", CommandType.StoredProcedure, parameters.ToArray());
                    StringBuilder stringBuilder = new StringBuilder();

                    string delimiter = "";
                    while (reader.Read())
                    {
                        //wkp_WorkflowProcessID, wpr_RoleID, wpr_WorkerID
                        stringBuilder.Append(delimiter);
                        stringBuilder.Append(reader.GetString(0)); // wkp_WorkflowProcessID
                        stringBuilder.Append("|");
                        stringBuilder.Append(reader.GetString(1)); //wpr_RoleID
                        stringBuilder.Append("|");
                        stringBuilder.Append(reader.GetString(2)); //wpr_WorkerID
                        delimiter = "|";
                    }

                    reader.Close();
                    existingRoles = stringBuilder.ToString();
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, existingRoles, sessionId);
                    return existingRoles;
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cDiagEntryMilestone.Save
            /// Save DiagEntry Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="programRequestId"></param>
            /// <param name="workerID"></param>
            /// <param name="diagnosisCodes"></param>
            /// <param name="comments"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveDiagEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long programRequestId = 0, long workerID = 0,
                                                string diagnosisCodes = "", string comments = "", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    //Pull Diagnosis Codes out of string
                    var diagnosisCodesList = new List<string>(diagnosisCodes.Split('|'));
                    foreach (string diagCode in diagnosisCodesList)
                    {
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                        new SqlParameter("@intDiagnosisCodeID", SqlDbType.Int,4) { Value = Convert.ToInt32 (diagCode) },
                        new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerID }
                    };

                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneDiagEntrySave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    }

                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cFacilityEntryMilestone.Save
            /// Save FacilityEntry Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="providerNumber"></param>
            /// <param name="workflowPointType"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public string SaveFacilityEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, string providerNumber = "", int workflowPointType = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@strProviderNumber", SqlDbType.VarChar, 10) { Value = providerNumber },
                    new SqlParameter("@intWorkflowPointType", SqlDbType.Int,4) { Value = workflowPointType },
                    new SqlParameter("@intISISSession", SqlDbType.Int, 4) { Value = sessionId },
                    new SqlParameter("@intResult", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISWorkflowProviderUpdate", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var errorCode = 0;
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intResult") errorCode = Convert.ToInt16(parameter.Value);
                    }

                    //Something happened
                    if (errorCode != 0)
                    {
                        return errorCode + "|-1";
                    }

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);

                    //SaveWorkerProvider Record was successfully Added/Updated
                    return 0 + "|";
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cFinalAuthMilestone.Save
            /// Save FinalAuth Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workflowApproveDeny"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="servicePlanIds"></param>
            /// <param name="sessionId"></param>
            /// <param name="denialReasonCode"></param>
            /// <param name="assesmentDate"></param>        
            /// <returns></returns>
            public void SaveFinalAuth(long workflowPointProcessID = 0, long workflowPointResponseID = 0, int workflowApproveDeny = 0,
                                        long workerID = 0, string comments = "", string servicePlanIds = "", long sessionId = 0,
                                        int denialReasonCode = 0, string assesmentDate = "01/01/1900")
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intworkflowpointprocessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intFlowopen", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISIsMatchingPlanChangeFlowOpen", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    var result = 0;
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intFlowopen") result = Convert.ToInt16(parameter.Value);
                    }

                    DateTime AssessmentDate = DateTime.Parse(assesmentDate);
                    if (result == 0)
                    {
                        //Pull service plans out of string
                        var servicePlanIdsList = new List<string>(servicePlanIds.Split(','));

                        foreach (string servicePlanId in servicePlanIdsList)
                        {
                            // Now insert parameters                       
                            parameters = new List<SqlParameter>
                        {
                            new SqlParameter("@intServicePlanID", SqlDbType.Int,4) { Value = servicePlanId },
                            new SqlParameter("@intApproveDeny", SqlDbType.Int,4) { Value = workflowApproveDeny },
                            new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId },
                            new SqlParameter("@intDenialReason", SqlDbType.Int,4) { Value = denialReasonCode },
                            new SqlParameter("@dteAssessmentDate", SqlDbType.DateTime) { Value = AssessmentDate}
                        };
                            dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneFinalAuthSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                        }
                    }

                    var customData = servicePlanIds + "|" + AssessmentDate;
                    if (comments != "")
                    {
                        customData = servicePlanIds + "|" + AssessmentDate + "|" + comments;
                    }

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, customData, " ", sessionId);

                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cIntervStrategyMilestone.Save
            /// Save IntervStrategy Milestone Information        
            /// This stored procedure does not exists in database.
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestID"></param>
            /// <param name="askMembertoStopBehavior"></param>
            /// <param name="encourageMemberExpression"></param>
            /// <param name="attemptDistraction"></param>
            /// <param name="offeredOtherChoices"></param>
            /// <param name="changedEnvironment"></param>
            /// <param name="mediatedConflict"></param>
            /// <param name="otherIntervention"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveIntervStrategy(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestID = 0, int askMembertoStopBehavior = 0,
                                            int encourageMemberExpression = 0, int attemptDistraction = 0, int offeredOtherChoices = 0,
                                            int changedEnvironment = 0, int mediatedConflict = 0, string otherIntervention = "", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestID },
                    new SqlParameter("@intAskMembertoStopBehavior", SqlDbType.Int,4) { Value = askMembertoStopBehavior },
                    new SqlParameter("@intEncourageMemberExpression", SqlDbType.Int,4) { Value = encourageMemberExpression },
                    new SqlParameter("@intAttemptDistraction", SqlDbType.Int, 4) { Value = attemptDistraction },
                    new SqlParameter("@intOfferedotherchoices", SqlDbType.Int,4) { Value = offeredOtherChoices },
                    new SqlParameter("@intchangedEnvironment", SqlDbType.Int,4) { Value = changedEnvironment },
                    new SqlParameter("@intMediatedConflict", SqlDbType.Int, 4) { Value = mediatedConflict },
                    new SqlParameter("@strOtherIntervention", SqlDbType.VarChar) { Value = otherIntervention },
                    new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISInterventionStrategyMilestoneSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cLOCMilestone.Save
            /// Save LOC Milestone Information
            /// </summary>
            /// <param name="LOCMilestoneRequest"></param>
            /// <returns></returns>
            public void SaveLOC(LOCMilestoneRequest locRequest)
            {
                try
                {
                    long servicePlanID = 0;
                    long archiveId = 0;
                    DateTime servicePlanStartDate = DateTime.Parse("01/01/1900");
                    DateTime servicePlanEndDate = DateTime.Parse("01/01/1900");
                    long splitServicePlanID = 0;
                    DateTime splitServicePlanStartDate = DateTime.Parse("01/01/1900");
                    DateTime splitServicePlanEndDate = DateTime.Parse("01/01/1900");

                    if (locRequest.ForceServicePlanSplit == false)
                    {
                        var outputParameters = new List<SqlParameter>();
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = locRequest.WorkflowPointProcessID },
                        new SqlParameter("@intWorkflowPointResponseTemplateID", SqlDbType.Int,4) { Value = locRequest.WorkflowPointResponseID },
                        new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = locRequest.WorkerID },
                        new SqlParameter("@intLOC", SqlDbType.Int,4) { Value = locRequest.LevelCareID },
                        new SqlParameter("@dtEffectiveDate", SqlDbType.DateTime ) { Value = locRequest.LevelCareEffectiveDate },
                        new SqlParameter("@dtCSRDate", SqlDbType.DateTime) { Value = locRequest.CsrDate },
                        new SqlParameter("@intServicePlanID", SqlDbType.Int,4) { Direction = ParameterDirection.Output },
                        new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = locRequest.SessionId },
                        new SqlParameter("@intArchiveID", SqlDbType.VarChar) { Direction = ParameterDirection.Output }
                    };

                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneLOCSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                        foreach (var parameter in outputParameters)
                        {
                            if (parameter.ParameterName == "@intServicePlanID") servicePlanID = Convert.ToInt64(parameter.Value);
                            if (parameter.ParameterName == "@intArchiveID") archiveId = Convert.ToInt64(parameter.Value);
                        }
                    }
                    else //Service Plan was split path
                    {
                        var outputParameters = new List<SqlParameter>();
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = locRequest.WorkflowPointProcessID },
                        new SqlParameter("@intWorkflowPointResponseTemplateID", SqlDbType.Int,4) { Value = locRequest.WorkflowPointResponseID },
                        new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = locRequest.WorkerID },
                        new SqlParameter("@intLOC", SqlDbType.Int,4) { Value = locRequest.LevelCareID },
                        new SqlParameter("@dtEffectiveDate", SqlDbType.DateTime ) { Value = locRequest.LevelCareEffectiveDate },
                        new SqlParameter("@dtCSRDate", SqlDbType.DateTime) { Value = locRequest.CsrDate },
                        new SqlParameter("@intServicePlanID", SqlDbType.Int,4) { Direction = ParameterDirection.Output },
                        new SqlParameter("@dtServicePlanStartDate", SqlDbType.DateTime) { Direction = ParameterDirection.Output },
                        new SqlParameter("@dtServicePlanEndDate", SqlDbType.DateTime) { Direction = ParameterDirection.Output },
                        new SqlParameter("@intNewServicePlanID", SqlDbType.Int,4) { Direction = ParameterDirection.Output },
                        new SqlParameter("@dtNewServicePlanStartDate", SqlDbType.DateTime) { Direction = ParameterDirection.Output },
                        new SqlParameter("@dtNewServicePlanEndDate", SqlDbType.DateTime) { Direction = ParameterDirection.Output },
                        new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = locRequest.SessionId },
                        new SqlParameter("@intArchiveID", SqlDbType.Int, 4) { Direction = ParameterDirection.Output }
                    };

                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneLOCSplitPlanSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                        foreach (var parameter in outputParameters)
                        {
                            if (parameter.ParameterName == "@intServicePlanID") servicePlanID = Convert.ToInt64(parameter.Value);
                            if (parameter.ParameterName == "@dteServicePlanStartDate") servicePlanStartDate = Convert.ToDateTime(parameter.Value);
                            if (parameter.ParameterName == "@dteServicePlanEndDate") servicePlanEndDate = Convert.ToDateTime(parameter.Value);
                            if (parameter.ParameterName == "@intNewServicePlanID") splitServicePlanID = Convert.ToInt64(parameter.Value);
                            if (parameter.ParameterName == "@dtNewServicePlanStartDate") splitServicePlanStartDate = Convert.ToDateTime(parameter.Value);
                            if (parameter.ParameterName == "@dtNewServicePlanEndDate") splitServicePlanEndDate = Convert.ToDateTime(parameter.Value);
                            if (parameter.ParameterName == "@intArchiveID") archiveId = Convert.ToInt64(parameter.Value);
                        }

                        //Originally in COM, checks ServicePlanID is NULL; Checked in stored procedure it returns value
                        if (servicePlanID == 0)
                        {
                            outputParameters = new List<SqlParameter>();
                            parameters = new List<SqlParameter>
                        {
                            new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = locRequest.WorkflowPointProcessID },
                            new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = locRequest.WorkerID },
                            new SqlParameter("@intLOC", SqlDbType.Int,4) { Value = locRequest.LevelCareID },
                            new SqlParameter("@dtEffectiveDate", SqlDbType.DateTime ) { Value = locRequest.LevelCareEffectiveDate},
                            new SqlParameter("@dtCSRDate", SqlDbType.DateTime) { Value = locRequest.CsrDate},
                            new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = locRequest.SessionId },
                        };

                            dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneLOCProgramRequestSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                        }
                        else
                        {
                            //Get the Service Spans for the the Plan
                            outputParameters = new List<SqlParameter>();
                            parameters = new List<SqlParameter>
                        {
                            new SqlParameter("@lngServicePlanID", SqlDbType.Int,4) { Value = servicePlanID },
                        };

                            DataTable ServiceSpans = dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpansGet", CommandType.StoredProcedure, parameters.ToArray());

                            foreach (DataRow span in ServiceSpans.Rows)
                            {
                                DateTime serviceSpanStartDate = Convert.ToDateTime(span["svs_RateStartDate"].ToString());
                                DateTime serviceSpanEndDate = Convert.ToDateTime(span["svs_RateEndDate"].ToString());
                                // Check if Span Start and End Dates fall within the Original Plan's new start and end dates
                                if (serviceSpanStartDate <= servicePlanEndDate)
                                {
                                    //Span needs to be assigned to the new plan
                                    if (serviceSpanStartDate >= servicePlanStartDate)
                                    {
                                        parameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@lngServiceSpanID", SqlDbType.Int,4) { Value = Convert.ToInt64(span["svs_ServiceSpanID"].ToString()) },
                                        new SqlParameter("@lngNewServicePlanID", SqlDbType.Int,4) { Value = splitServicePlanID },
                                        new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = locRequest.SessionId}
                                    };
                                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneLOCSpanReAssign", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                                    }
                                    else //Span needs to be Split
                                    {
                                        parameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@intServiceSpanID", SqlDbType.Int,4) { Value = Convert.ToInt64(span["svs_ServiceSpanID"].ToString()) },
                                        new SqlParameter("@intServicePlanID", SqlDbType.Int,4) { Value = servicePlanID },
                                        new SqlParameter("@dtServicePlanEndDate", SqlDbType.DateTime) { Value = servicePlanEndDate },
                                        new SqlParameter("@intNewServicePlanID", SqlDbType.Int,4) { Value = splitServicePlanID},
                                        new SqlParameter("@dtNewServicePlanStartDate", SqlDbType.DateTime) { Value = splitServicePlanStartDate },
                                        new SqlParameter("@dtServiceSpanEndDate", SqlDbType.DateTime) { Value = splitServicePlanEndDate},
                                        new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = locRequest.SessionId}
                                    };
                                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneLOCSplitSpan", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                                    }
                                }
                            }
                        }
                    }

                    string archiveInfo = servicePlanID + "|" + archiveId + "|" + splitServicePlanID + "|" + locRequest.OldLevelCareID + "|" + Convert.ToString(locRequest.OldLevelCareEffectiveDate) + "|" + Convert.ToString(locRequest.OldCSRDate) + "|" + Convert.ToString(locRequest.OldOrigLOCEffectiveDate);

                    // Perform standard save
                    SaveMilestone(locRequest.WorkflowPointProcessID, locRequest.WorkflowPointResponseID, locRequest.WorkerID, locRequest.Comments, archiveInfo, locRequest.SessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cMCareLOCMilestone.Save
            /// Save MCare LOC Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="levelCareID"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public string SavecMCareLOC(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long levelCareID = 0, long sessionId = 0)
            {
                try
                {
                    string existingRoles = "";
                    if (levelCareID > 0)
                    {
                        var outputParameters = new List<SqlParameter>();
                        var parameters = new List<SqlParameter>();
                        if (levelCareID == 9)
                        {
                            parameters = new List<SqlParameter>
                        {
                            new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                            new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                        };

                            dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISWorkflowMilestoneDeleteMedicalServicesRole", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                        }
                        parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                        new SqlParameter("@intLevelCareID", SqlDbType.Int,4) { Value = levelCareID },
                        new SqlParameter("@intSessionID", SqlDbType.Int, 4) { Value = sessionId }
                    };

                        //var existingRoles = RS.GetString(adClipString, , ",", "|");
                        //TODO: need to findout how to relace ADO Getstring: adClipString(Replace logic - remove "|" for fitst and last records
                        IDataReader reader = dataAccess.GetDataReader(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneMCARELOCSave", CommandType.StoredProcedure, parameters.ToArray());

                        var stringBuilder = new StringBuilder();
                        string delimiter = "";
                        //IDataReader does not has definition for .HasRow Using  readerRead() suffice checking for rows
                        while (reader.Read())
                        {
                            stringBuilder.Append(delimiter);
                            stringBuilder.Append(reader.GetString(0));
                            stringBuilder.Append("|");
                            stringBuilder.Append(reader.GetString(1));
                            stringBuilder.Append("|");
                            stringBuilder.Append(reader.GetString(2));
                            delimiter = "|";
                        }

                        reader.Close();
                        existingRoles = stringBuilder.ToString();
                    }
                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, existingRoles, sessionId);
                    return existingRoles;
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cMDSQMilestone.Save
            /// Save MDSQ Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="response"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="data"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveMDSQ(long workflowPointProcessID = 0, long workflowPointResponseID = 0, string response = "", long workerID = 0,
                                     string comments = "", string data = "", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intWorkflowPointResponseTemplateID", SqlDbType.Int,4) { Value = workflowPointResponseID },
                    new SqlParameter("@strResponse", SqlDbType.VarChar, 100) { Value = response },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerID },
                    new SqlParameter("@strComments", SqlDbType.VarChar, 800) { Value = comments },
                    new SqlParameter("@strData", SqlDbType.VarChar,256) { Value = data },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneMDSQSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cMDSQReasonsMilestone.Save
            /// Save MDSQ Reasons Milestone Information
            /// </summary>
            /// <param name="MDSQReasonsMilestoneRequest"></param>    
            /// <returns></returns>
            public void SaveMDSQReasons(MDSQReasonsMilestoneRequest mdsqMilestone)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = mdsqMilestone.WorkflowPointProcessID },
                    new SqlParameter("@intprogramrequestID", SqlDbType.Int,4) { Value = mdsqMilestone.ProgranRequestId },
                    new SqlParameter("@intChangedMindInd", SqlDbType.Int,4) { Value = mdsqMilestone.ChangeMindInd },
                    new SqlParameter("@intServiceUnavailableInd", SqlDbType.Int,4) { Value = mdsqMilestone.ServiceUnavailableInd },
                    new SqlParameter("@intNeedTooHighInd", SqlDbType.Int,4) { Value = mdsqMilestone.NeedTooHighInd },
                    new SqlParameter("@intLackingSupportInd", SqlDbType.Int,4) { Value = mdsqMilestone.LackingSupportInd },
                    new SqlParameter("@intNoFundingInd", SqlDbType.Int,4) { Value = mdsqMilestone.NoFundingInd },
                    new SqlParameter("@intPrevAttemptsFailedInd", SqlDbType.Int,4) { Value = mdsqMilestone.PrevAttemptsFailedInd },
                    new SqlParameter("@intNoGuardianPOAInd", SqlDbType.Int,4) { Value = mdsqMilestone.NoGuardianPOAInd },
                    new SqlParameter("@intCourtCommittedInd", SqlDbType.Int,4) { Value = mdsqMilestone.CourtCommittedInd },
                    new SqlParameter("@intOtherInd", SqlDbType.Int,4) { Value = mdsqMilestone.OtherInd },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = mdsqMilestone.SessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneMDSQReasonsSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Now perform standard save
                    SaveMilestone(mdsqMilestone.WorkflowPointProcessID, mdsqMilestone.WorkflowPointResponseID, mdsqMilestone.WorkerID, mdsqMilestone.Comments, " ", mdsqMilestone.SessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cMedAcceptDenyMilestone.Save
            /// Save MedAcceptDeny Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="data"></param>
            /// <param name="workflowApproveDeny"></param>
            /// <param name="sessionId"></param>
            /// <param name="beginCode"></param>
            /// <returns></returns>
            public void SaveMedAcceptDeny(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "",
                                     string data = " ", int workflowApproveDeny = 0, long sessionId = 0,
                                     string beginCode = "")
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkFlowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intApproveDeny", SqlDbType.TinyInt) { Value = workflowApproveDeny },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId },
                    new SqlParameter("@strBeginCode", SqlDbType.VarChar,3) { Value = beginCode }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneMedAcceptDeny", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, data, sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cMedAcceptDenyMilestone.ISPASRRPresentSave
            /// Save MedAcceptDeny(ISPASRRPresent) Milestone Information
            /// This stored procedure does not exists in database and also not calling from ASP pages.
            /// So i commented this whole method.
            /// </summary>
            /// <param name="programRequestId"></param>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="data"></param>
            /// <param name="workflowApproveDeny"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            //public string SaveISPASRRPresent(long programRequestId = 0, long workflowPointProcessID = 0, long workflowPointResponseID = 0,
            //                            long workerID = 0, string comments = "", string data = " ", int workflowApproveDeny = 0, long sessionId = 0)
            //{
            //    try
            //    {
            //        var outputParameters = new List<SqlParameter>();
            //        var parameters = new List<SqlParameter>
            //        {
            //            new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
            //            new SqlParameter("@intResult", SqlDbType.Int) { Direction = ParameterDirection.Output }
            //        };

            //        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneIsPASRRPresentEdit", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

            //        string errorCode = "0";
            //        foreach (var parameter in outputParameters)
            //        {
            //            if (parameter.ParameterName == "@intResult") errorCode = Convert.ToString(parameter.Value);
            //        }

            //        if (errorCode != "0")
            //        {
            //            return errorCode;
            //        }

            //        parameters = new List<SqlParameter>
            //        {
            //            new SqlParameter("@intWorkFlowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessID },
            //            new SqlParameter("@intApproveDeny", SqlDbType.Int,4) { Value = workflowApproveDeny },
            //            new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
            //        };

            //        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneMedAcceptDeny", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

            //        // Now perform standard save
            //        SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, data, sessionId);
            //        return errorCode;
            //    }
            //    catch (Exception ex)
            //    {
            //        exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
            //        throw;
            //    }
            //}

            /// <summary>
            /// COM: cMembersResponseMilestone.Save
            /// Save Members Response Milestone Information
            /// This stored procedure does not exists in database but this method calls from asp page.
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="gradualincreaseinAgitation"></param>
            /// <param name="explosiveAggressionWithStress"></param>
            /// <param name="explosiveAggressionWithoutProvocation"></param>
            /// <param name="otherResponse"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveMembersResponse(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "",
                                     long programRequestId = 0, int gradualincreaseinAgitation = 0, int explosiveAggressionWithStress = 0,
                                     int explosiveAggressionWithoutProvocation = 0, string otherResponse = "", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@intGradualincreaseinAgitation", SqlDbType.Int,4) { Value = gradualincreaseinAgitation },
                    new SqlParameter("@intExplosiveAggressionwithStress", SqlDbType.Int,4) { Value = explosiveAggressionWithStress },
                    new SqlParameter("@intExplosiveAggressionwithoutProvocation", SqlDbType.Int,4) { Value = explosiveAggressionWithoutProvocation },
                    new SqlParameter("@strOtherResponse", SqlDbType.VarChar) { Value = otherResponse },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMembersResponseMilestoneSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cOutofStateSkilledMilestone.Save
            /// Save OutofState Skilled Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="outofStateInd"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveOutofStateSkilled(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "",
                                     long programRequestId = 0, int outofStateInd = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@intOutofStateSkilledInd", SqlDbType.Int,4) { Value = outofStateInd },
                    new SqlParameter("@intISISSession", SqlDbType.Int,4) { Value = sessionId },
                    new SqlParameter("@intCurrentMedicalServicesWorkerID", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneOutofStateSkilledSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    string currentMedicalServicesWorkerID = "";
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intCurrentMedicalServicesWorkerID") currentMedicalServicesWorkerID = Convert.ToString(parameter.Value);
                    }

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, currentMedicalServicesWorkerID, sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cPACEWithDrawalMilestone.Save
            /// Save PACE WithDrawal Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="withDrawalInd"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SavePACEWithDrawal(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0, string comments = "",
                                     long programRequestId = 0, int withDrawalInd = 0, long sessionId = 0)
            {
                try
                {
                    string archiveID = "";
                    if (withDrawalInd == 1)
                    {
                        var outputParameters = new List<SqlParameter>();
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                        new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId },
                        new SqlParameter("@intArchiveID", SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };

                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestonePACEWithdrawalSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                        foreach (var parameter in outputParameters)
                        {
                            if (parameter.ParameterName == "@intArchiveID") archiveID = Convert.ToString(parameter.Value);
                        }
                    }
                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, archiveID, sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cPASRRExemptionMilestone.Save
            /// Save PASRR Exemption Milestone Information
            /// This stored procedure does not exists in database and also NOT referring this class and method.
            /// So i commented this method.
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="categoryId"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="expirationDate"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            //public void SavePASRRExemption(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
            //    int categoryId = 0, string comments = "", long programRequestId = 0, string expirationDate = "", long sessionId = 0)
            //{
            //    try
            //    {
            //        var outputParameters = new List<SqlParameter>();
            //        var parameters = new List<SqlParameter>
            //        {
            //            new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
            //            new SqlParameter("@dteExpirationDate", SqlDbType.DateTime) { Value = DateTime.Parse (expirationDate) },
            //            new SqlParameter("@intCategoryID", SqlDbType.Int,4) { Value = categoryId },
            //            new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
            //        };

            //        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestonePASRRExemptionSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

            //        // Now perform standard save
            //        SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
            //    }
            //    catch (Exception ex)
            //    {
            //        exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
            //        throw;
            //    }
            //}

            /// <summary>
            /// COM: cRBSCLApprovalMilestone.Save
            /// Save RBSCL Approval Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="rbsclInd"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveRBSCLApproval(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, int rbsclInd = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@intRBSCLInd", SqlDbType.Int,4) { Value = rbsclInd },
                    new SqlParameter("@intISISSession", SqlDbType.Int,4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISRBSCLUpdate", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cRSFinalAuthMilestone.Save
            /// Save RSFinalAuth Milestone Information
            /// </summary>
            /// <param name="serviceSpanID"></param>
            /// <param name="approveDenyInd"></param>
            /// <param name="denialReasonCode"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveRSFinalAuth(long serviceSpanID = 0, int approveDenyInd = 0, long denialReasonCode = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intServiceSpanID", SqlDbType.Int,4) { Value = serviceSpanID },
                    new SqlParameter("@intApproveDeny", SqlDbType.TinyInt) { Value = approveDenyInd },
                    new SqlParameter("@intDenialReason", SqlDbType.Int,4) { Value = denialReasonCode },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneRSFinalAuthSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cTierEntryMilestone.Save
            /// Save Tier Entry Milestone Information
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="tier"></param>
            /// <param name="tierEffectiveDate"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveTierEntry(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, string tier = "", string tierEffectiveDate = "01/01/1900", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@strTier", SqlDbType.VarChar) { Value = tier },
                    new SqlParameter("@dteTierEffectiveDate", SqlDbType.DateTime) { Value = DateTime.Parse(tierEffectiveDate) },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneTierEntrySave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cUnnaturalDeathMilestone.Save
            /// Save Unnatural Death Milestone Information
            /// This stored procedure does not exists in database but calls from asp page.
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="riskFactors"></param>
            /// <param name="actionsTaken"></param>
            /// <param name="actionsTakenforSafetyofOthers"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveUnnaturalDeath(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, string riskFactors = "", string actionsTaken = "",
                                            string actionsTakenforSafetyofOthers = "", long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@strRiskFactors", SqlDbType.VarChar,800) { Value = riskFactors },
                    new SqlParameter("@strActionstaken", SqlDbType.VarChar,800) { Value = actionsTaken },
                    new SqlParameter("@strActionstakenforsafetyofOthers", SqlDbType.VarChar, 800) { Value = actionsTakenforSafetyofOthers },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISUnnaturalDeathMilestoneSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            /// COM: cWhatTriggeredMilestone.Save
            /// Save WhatTriggered Milestone Information
            /// This stored procedure does not exists in database
            /// </summary>
            /// <param name="workflowPointProcessID"></param>
            /// <param name="workflowPointResponseID"></param>
            /// <param name="workerID"></param>
            /// <param name="comments"></param>
            /// <param name="programRequestId"></param>
            /// <param name="environmentalFactors"></param>
            /// <param name="personalCharacteristics"></param>
            /// <param name="consequencesOfEvent"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveWhatTriggered(long workflowPointProcessID = 0, long workflowPointResponseID = 0, long workerID = 0,
                                            string comments = "", long programRequestId = 0, int environmentalFactors = 0, int personalCharacteristics = 0,
                                            int consequencesOfEvent = 0, long sessionId = 0)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneID", SqlDbType.Int,4) { Value = workflowPointProcessID },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = programRequestId },
                    new SqlParameter("@intEnvironmentalFactorsInd", SqlDbType.Int,4) { Value = environmentalFactors },
                    new SqlParameter("@intPersonalCharacteristicsInd", SqlDbType.Int,4) { Value = personalCharacteristics },
                    new SqlParameter("@intConsequencesofEvent", SqlDbType.Int,4) { Value = consequencesOfEvent },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISWhatTriggeredIntervMilestoneSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    // Now perform standard save
                    SaveMilestone(workflowPointProcessID, workflowPointResponseID, workerID, comments, " ", sessionId);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// COM: cTCMServiceAuthMilestone.Save
            /// Save TCM ServiceAuth Milestone Information
            /// </summary>
            /// <param name="TCMMilestone"></param>
            /// <returns></returns>
            public void SaveTCMServiceAuth(TCMMilestoneRequest tcmMilestone)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intMilestoneID", SqlDbType.Int,4) { Value = tcmMilestone.WorkflowPointProcessID},
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = tcmMilestone.ProgramRequestID},
                    new SqlParameter("@dteAssessmentDate", SqlDbType.DateTime) { Value = tcmMilestone.AssessmentDate},
                    new SqlParameter("@strPrimaryDisabilityID", SqlDbType.VarChar,3){ Value = tcmMilestone.PrimaryDisabilityID},
                    new SqlParameter("@intPrimaryPsychDiagCode", SqlDbType.Int,4) { Value = tcmMilestone.PrimaryPsychDiagCode},
                    new SqlParameter("@dteWrittenDocumentationofDisabilityDate", SqlDbType.DateTime) { Value = tcmMilestone.WrittenDocumentationofDisabilityDate},
                    new SqlParameter("@intConsumerinWaiverInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerinWaiverInd},
                    new SqlParameter("@intConsumerNeedsHelpWithSchoolingEducationalServicesInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerNeedsHelpWithSchoolingEtcInd},
                    new SqlParameter("@intConsumerNeedsRespiteInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerNeedsRespiteInd},
                    new SqlParameter("@intConsumerNeedsHealthServicesInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerNeedsHealthServicesInd},
                    new SqlParameter("@intConsumerNeedsDailyLivingSkillsSLCInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerNeedsDailyLivingSkillsSLCInd},
                    new SqlParameter("@intConsumerNeedsDailyLivingSkillsPayeeInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerNeedsDailyLivingSkillsPayeeInd},
                    new SqlParameter("@intConsumerNeedsDailyLivingSkillsMealsonWheelsInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerNeedsDailyLivingSkillsMealsonWheelsInd},
                    new SqlParameter("@intConsumerNeedsDailyLivingSkillsTransInd", SqlDbType.TinyInt) { Value = tcmMilestone.ConsumerNeedsDailyLivingSkillsTransInd},
                    new SqlParameter("@intConsumerNeedsDailyLivingSkillsHomemakerInd", SqlDbType.TinyInt){ Value = tcmMilestone.ConsumerNeedsDailyLivingSkillsHomemakerInd},
                    new SqlParameter("@intConsumerNeedsSupporttoEngageMHTreatmentInd", SqlDbType.TinyInt) { Value =  tcmMilestone.ConsumerNeedsSupportToEngageMHTreatmentInd},
                    new SqlParameter("@intConsumerNeedsAssistInMedsManagementInd", SqlDbType.TinyInt) { Value =  tcmMilestone.ConsumerNeedsAssistInMedsManagementInd},
                    new SqlParameter("@intConsumerNeedsAssistInHousingInd", SqlDbType.TinyInt){ Value = tcmMilestone.ConsumerNeedsAssistInHousingInd},
                    new SqlParameter("@intConsumerNeedsAssisttoAccessEligServicesInd", SqlDbType.TinyInt) { Value =  tcmMilestone.ConsumerNeedsAssisttoAccessEligServicesInd},
                    new SqlParameter("@intAssessmentDemonstrationImpairmentInd", SqlDbType.TinyInt) { Value = tcmMilestone.AssessmentDemonstrationImpairmentInd},
                    new SqlParameter("@intDocumentofLackofAbilitytoAccessServicesInd", SqlDbType.TinyInt){ Value =  tcmMilestone.DocumentofLackofAbilitytoAccessServicesInd},
                    new SqlParameter("@intDocumentofLackofAbilitytoSustainServicesInd", SqlDbType.TinyInt) { Value = tcmMilestone.DocumentofLackofAbilitytoSustainServicesInd},
                    new SqlParameter("@intConsumerResidesinMINoDischargePlan30DaysInd", SqlDbType.VarChar, 1) { Value = tcmMilestone.ConsumerResidesinMINoDischargePlan30DaysInd},
                    new SqlParameter("@intConsumerCurrServicedInAssertiveProgramInd", SqlDbType.VarChar, 1) { Value = tcmMilestone.ConsumerCurrServicedInAssertiveProgramInd},
                    new SqlParameter("@intConsumerNeedsDailyLivingSkillsInd", SqlDbType.TinyInt) { Value =  tcmMilestone.ConsumerNeedsDailyLivingSkillsInd},
                    new SqlParameter("@intConsumerNeedsSocialFunctioningInd", SqlDbType.TinyInt) { Value =  tcmMilestone.ConsumerNeedsSocialFunctioningInd},
                    new SqlParameter("@intConsumerNeedsVocationalorPreVocationalServicesInd", SqlDbType.TinyInt) { Value =  tcmMilestone.ConsumerNeedsVocationalorPreVocationalServicesInd},
                    new SqlParameter("@intConsumerNeedsNonVocationalServicesInd", SqlDbType.TinyInt) { Value =  tcmMilestone.ConsumerNeedsNonVocationalServicesInd},
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value =  tcmMilestone.SessionID},
                    new SqlParameter("@intUserAcknowledgement", SqlDbType.VarChar, 1) { Value =  tcmMilestone.UserAcknowledgement},
                    new SqlParameter("@intDenyReason", SqlDbType.Int,4) { Value = tcmMilestone.DenyReason},
                    new SqlParameter("@intVersionNumber", SqlDbType.Int,4) { Value =  tcmMilestone.VersionNumber }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISTCMServiceAuthorizationSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    string workflowPointResponseId = Convert.ToString(tcmMilestone.WorkflowPointResponseID);

                    //Close any Active Milestones
                    string last3FromResponseId = (workflowPointResponseId.Substring(workflowPointResponseId.Length - 3));
                    if ((last3FromResponseId == "998") || (last3FromResponseId == "999"))
                    {
                        outputParameters = new List<SqlParameter>();
                        parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@int_ProgramRequestID", SqlDbType.Int,4) { Value = tcmMilestone.ProgramRequestID},
                        new SqlParameter("@int_ResponseTemplateID", SqlDbType.Int,4) { Value = tcmMilestone.WorkflowPointResponseID},
                        new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = tcmMilestone.SessionID}
                    };
                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISCloseActiveMilestones", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                    }
                    // Now perform standard save
                    SaveMilestone(tcmMilestone.WorkflowPointProcessID, tcmMilestone.WorkflowPointResponseID, tcmMilestone.WorkerID, tcmMilestone.Comments, " ", tcmMilestone.SessionID);

                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
            /// <summary>
            ///  Update Milestone's Comments
            ///  Created by Praveen 06/19/20
            /// </summary>
            /// <param name="milestoneId"></param>
            /// <param name="comment"></param>
            /// <param name="isisSession"></param>
            public void UpdateComment(int milestoneId, string comment, long isisSession)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();

                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int) { Value = milestoneId },
                    new SqlParameter("@strComment", SqlDbType.VarChar,800) { Value = comment },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = isisSession }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISMilestoneCommentSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

    }
}
