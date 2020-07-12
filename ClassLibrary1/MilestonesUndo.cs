using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    /// <summary>
    /// Author: Satish Yeluri  
    /// Crated Date: 06/23/2020   
    /// Modified Date: 06/24/2020
    /// Undo Answered Milestones
    /// </summary>
    public class MilestonesUndo : IMilestonesUndo
    {
        IDataAccess dataAccess;
        IExceptionHandling exceptionHandling;
        public MilestonesUndo(IDataAccess access, IExceptionHandling exception)
        {
            dataAccess = access;
            exceptionHandling = exception;
        }

        /// <summary>
        /// COM: cMilestone.Undo 
        /// Undo Standard Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void Undo(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cMilestone.PACEUndo
        /// Undo the PACEUndo Milestone Information 
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoPace(long workflowPointProcessId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkFlowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestonePACEUnDo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cAllRolesAssignedMilestone.Undo
        /// Undo the AllRolesAssigned Milestone Information
        /// This stored procedure does not exists in database but this method calls from web pages.
        /// This menas when worker Undo RolesAssigned milestone, roles are not going to removed.
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoAllRolesAssigned(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                };

                DataTable MilestoneDetails = dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneInfoGet", CommandType.StoredProcedure, parameters.ToArray());
                string customData = "";
                if (MilestoneDetails.Rows.Count > 0)
                {
                    DataRow dr = MilestoneDetails.Rows[0];
                    customData = Convert.ToString(dr["wpp_CustomData"]);
                }
                var outputParameters = new List<SqlParameter>();
                var roleRecord = customData.Split('|');
                foreach (var item in roleRecord)
                {
                    var roleSplit = item.Split(',');
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = Convert.ToInt64(roleSplit[0])},
                        new SqlParameter("@intRoleID", SqlDbType.Int,4) { Value = Convert.ToInt16(roleSplit[1]) },
                        new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = Convert.ToInt64(roleSplit[2]) },
                        new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneVerifyAllRolesAssignedUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cARODiagLPHAMilestone.Undo
        /// Undo the ARO Diagnosis LPHA Milestone Information
        /// Any open flows that were cancelled with the system response = "9999997" will be undone too.
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <param name="lphaCertDate"></param>
        /// <returns></returns>
        public void UndoARODiagLPHA(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                    long progranRequestId = 0, string lphaCertDate = "01/01/1900")
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                };

                DataTable MilestoneDetails = dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneInfoGet", CommandType.StoredProcedure, parameters.ToArray());
                string customData = "";
                if (MilestoneDetails.Rows.Count > 0)
                {
                    DataRow dr = MilestoneDetails.Rows[0];
                    customData = Convert.ToString(dr["wpp_CustomData"]);
                }
                var outputParameters = new List<SqlParameter>();

                parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId},
                    new SqlParameter("@dteLPHACertDate", SqlDbType.DateTime) { Value = DateTime.Parse (lphaCertDate) },
                    new SqlParameter("@dteOldLPHACertDate", SqlDbType.DateTime ) { Value = DateTime.Parse (customData) },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneARODiagLPHAUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cCancelDecisionMilestone.Undo
        /// Undo the Cancel Decision Milestone Information
        /// Any open flows that were cancelled with the system response = "9999997" will be undone too.
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <returns></returns>
        public void UndoCancelDecision(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                    long progranRequestId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId},
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneCancelDecisionUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cCauseofDeathMilestone.Undo
        /// Undo the Cause of Death Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoCauseofDeath(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cClosureEntryMilestone.Undo
        /// Undo the Closure Reason Entry Milestone Information
        /// Any open flows that were cancelled with the system response = "9999997" will be undone too.
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <returns></returns>
        public void UndoClosureEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                    long progranRequestId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId},
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneClosureReasonUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cCOLSArbitrationMilestone.Undo
        /// Undo the COLS Arbitration Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoCOLSArbitration(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneCOLSArbitrationUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cCOLSMilestone.Undo
        /// Undo the COLS Milestone Information 
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoCOLS(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneCOLSUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cDateEntryMilestone.Undo 
        /// Undo the DateEntry Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <returns></returns>
        public void UndoDateEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                    long progranRequestId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId}
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneDateEntryUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cDCNEntryMilestone.Undo
        /// Undo the DCN Entry Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoClosureEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId},
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneDCNEntryUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cDeleteRolesMilestone.Undo
        /// Undo the Delete Roles Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoDeleteRoles(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                };

                DataTable MilestoneDetails = dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneInfoGet", CommandType.StoredProcedure, parameters.ToArray());
                string customData = "";
                if (MilestoneDetails.Rows.Count > 0)
                {
                    DataRow dr = MilestoneDetails.Rows[0];
                    customData = Convert.ToString(dr["wpp_CustomData"]);
                }
                var outputParameters = new List<SqlParameter>();
                var roleRecord = customData.Split('|');
                foreach (var item in roleRecord)
                {
                    var roleSplit = item.Split(',');
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@@intWorkflowProcessID", SqlDbType.Int,4) { Value = Convert.ToInt64 (roleSplit[0])},
                        new SqlParameter("@intRoleID", SqlDbType.Int,4) { Value =Convert.ToInt16 ( roleSplit[1]) },
                        new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = Convert.ToInt64 (roleSplit[2]) },
                        new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneAcceptDenyUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cDiagEntryMilestone.Undo
        /// Undo the Diagnosis Codes Entry Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <returns></returns>
        public void UndoDiagEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                    long progranRequestId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId},
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneDiagEntryUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cFacilityEntryMilestone.Undo
        /// Undo Facility Entry Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoFacilityEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                };

                DataTable MilestoneDetails = dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneInfoGet", CommandType.StoredProcedure, parameters.ToArray());
                long programRequestId = 0;
                int workflowPointType = 0;
                string providerNumber = "0000000";  
                if (MilestoneDetails.Rows.Count > 0)
                {
                    DataRow dr = MilestoneDetails.Rows[0];
                    programRequestId = Convert.ToInt64(dr["pgr_ProgramRequestID"]);
                    workflowPointType = Convert.ToInt16(dr["wpt_workflowpointtypeid"]);
                }
                var outputParameters = new List<SqlParameter>();

                parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int,4) { Value = programRequestId},
                    new SqlParameter("@strProviderNumber", SqlDbType.VarChar,10) { Value = providerNumber },
                    new SqlParameter("@intWorkflowPointType", SqlDbType.Int,4) { Value = workflowPointType },
                    new SqlParameter("@intISISSession", SqlDbType.Int,4) { Value = sessionId },
                    new SqlParameter("@intResult", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowProviderUpdate", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Commented below due to unused Output parameter value.
                //int intResult = 0;
                //foreach (var parameter in outputParameters)
                //{
                //    if (parameter.ParameterName == "@intResult") intResult = Convert.ToInt16(parameter.Value);
                //}

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cFinalAuthMilestone.Undo
        /// Undo the FinalAuth Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoFinalAuth(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                };

                DataTable MilestoneDetails = dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneInfoGet", CommandType.StoredProcedure, parameters.ToArray());
                string customData = "";
                if (MilestoneDetails.Rows.Count > 0)
                {
                    DataRow dr = MilestoneDetails.Rows[0];
                    customData = Convert.ToString(dr["wpp_CustomData"]);
                }

                if (customData.Length > 0)
                {
                    var outputParameters = new List<SqlParameter>();
                    var roleRecord = customData.Split('|');
                    //Need to test this conversion
                    string servicePlanIds = Convert.ToString(roleRecord[0]);
                    DateTime assessmentDate = DateTime.Parse(roleRecord[1]);
                    var servicePlans = servicePlanIds.Split(',');
                    foreach (var servicePlanId in servicePlans)
                    {
                        parameters = new List<SqlParameter>
                        {
                            new SqlParameter("@intServicePlanID", SqlDbType.Int,4) { Value = servicePlanId},
                            new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId },
                            new SqlParameter("@dteAssessmentDate", SqlDbType.DateTime) { Value = assessmentDate }
                        };
                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneFinalAuthUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    }
                }
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cIntervStrategyMilestone.Undo
        /// Undo the Intervention Strategy Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoInterventionStrategy(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cLOCMilestone.Undo
        /// Undo Level of Care Milestone Information
        /// Gets the old program from archival table
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="sessionId"></param>
        /// <param name="workerId"></param>
        /// <param name="oldServicePlanId"></param>
        /// <param name="archivePlanId"></param>
        /// <param name="oldLevelCareId"></param>
        /// <param name="oldLevelCareEffectiveDate"></param>
        /// <param name="oldCSRDate"></param>
        /// <param name="oldOrigLOCEffectiveDate"></param>
        /// <param name="splitServicePlanId"></param>
        /// <returns></returns>
        public void UndoLOC(long workflowPointProcessId = 0, long sessionId = 0, long workerId = 0,
                                long oldServicePlanId = 0, long archivePlanId = 0, long oldLevelCareId = 0,
                                string oldLevelCareEffectiveDate = "01/01/1900", string oldCSRDate = "01/01/1900",
                                string oldOrigLOCEffectiveDate = "01/01/1900", long splitServicePlanId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId},
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId},
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId},
                    new SqlParameter("@intArchivePlanID", SqlDbType.Int,4) { Value = archivePlanId},
                    new SqlParameter("@intOldLevelCareID", SqlDbType.Int,4) { Value = oldLevelCareId},
                    new SqlParameter("@dteOldLevelCareEffectiveDate", SqlDbType.DateTime) { Value = DateTime.Parse(oldLevelCareEffectiveDate)},
                    new SqlParameter("@dteOldCSRDate", SqlDbType.DateTime) { Value = DateTime.Parse(oldCSRDate)},
                    new SqlParameter("@intOldOrigLOCEffectiveDate", SqlDbType.DateTime) { Value = DateTime.Parse(oldOrigLOCEffectiveDate)},
                    new SqlParameter("@intOldServicePlanID", SqlDbType.Int,4) { Value = oldServicePlanId },
                    new SqlParameter("@intSplitServicePlanID", SqlDbType.Int,4) { Value = splitServicePlanId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneLOCUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cMCareLOCMilestone.Undo
        /// Undo the MCareLOC Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoMCareLOC(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                };

                DataTable MilestoneDetails = dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneInfoGet", CommandType.StoredProcedure, parameters.ToArray());
                string responseDescription = "";
                string programType = "";
                string customData = "";
                if (MilestoneDetails.Rows.Count > 0)
                {
                    DataRow dr = MilestoneDetails.Rows[0];
                    responseDescription = Convert.ToString(dr["wpr_ResponseDescription"]);
                    programType = Convert.ToString(dr["prg_ProgramType"]);
                    customData = Convert.ToString(dr["wpp_CustomData"]);
                }

                if (responseDescription.Equals("Medical Services", StringComparison.CurrentCultureIgnoreCase) ||
                    responseDescription.Equals("Physician Review", StringComparison.CurrentCultureIgnoreCase) ||
                    responseDescription.Equals("IME Medical Services for Mental Health", StringComparison.CurrentCultureIgnoreCase) ||
                    responseDescription.Equals("Medical Services, for members not currently enrolled in Medicaid & Iowa Plan", StringComparison.CurrentCultureIgnoreCase))
                {
                }
                else
                {
                    int roleId = 0;
                    int medicalServicesWorkerId = 0;
                    // For PMIC get the roleid info to put Medical Services role back
                    if (programType == "8")
                    {
                        var roleRecord = customData.Split('|');
                        foreach (var item in roleRecord)
                        {
                            var roleSplit = item.Split(',');
                            if (roleSplit[1] == "14")
                            {
                                roleId = Convert.ToInt32(roleSplit[1]);
                                medicalServicesWorkerId = Convert.ToInt32(roleSplit[2]);
                            }
                        }
                    }
                    var outputParameters = new List<SqlParameter>();
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId},
                        new SqlParameter("@intRoleID", SqlDbType.Int,4) { Value = roleId },
                        new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = medicalServicesWorkerId },
                        new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneMCareLOCUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cMDSQMilestone.Undo
        /// Undo the MDSQ Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoMDSQ(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneMDSQUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cMDSQReasonsMilestone.Undo
        /// Undo the MDSQ Reasons Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoMDSQReasons(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneMDSQReasonsUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cMedAcceptDenyMilestone.Undo
        /// Undo the MedAcceptDeny Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoMedAcceptDeny(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneMedAcceptDenyUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cMembersResponseMilestone.Undo
        /// Undo the MembersResponse Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoMembersResponse(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cOutofStateSkilledMilestone.Undo
        /// Undo the OutofState Skilled Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="programRequestId"></param>
        /// <param name="oldMedicalServicesWorkerID"></param>
        /// <returns></returns>
        public void UndoOutofStateSkilled(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                long programRequestId = 0, long oldMedicalServicesWorkerID = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int,4) { Value = programRequestId},
                    new SqlParameter("@intOutofStateSkilledInd", SqlDbType.Int,4) { Value = 0},
                    new SqlParameter("@intISISSession", SqlDbType.Int,4) { Value = sessionId},
                    new SqlParameter("@intOldMedicalServicesWorkerID", SqlDbType.Int,4) { Value = oldMedicalServicesWorkerID}
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneOutofStateSkilledUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cPACEWithDrawalMilestone.Undo
        /// Undo the PACEWithDrawal Milestone Information
        /// Removed unused "programRequestId" parameter;
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoPACEWithDrawal(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId},
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId},
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId}
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestonePACEWithdrawalUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// COM: cPASRRExemptionMilestone.Undo
        /// Undo the PASRR Exemption Milestone Information
        /// This stored procedure does not exists in database and also NOT calling from webpages.
        /// So, Satish commented this method.
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <returns></returns>
        //public void UndoPASRRExemption(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
        //                            long progranRequestId = 0)
        //{
        //    try
        //    {
        //        var outputParameters = new List<SqlParameter>();
        //        var parameters = new List<SqlParameter>
        //        {
        //            new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId},
        //            new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
        //        };
        //        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestonePASRRExemptionUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

        //        //Perform standard undo
        //        Undo(workflowPointProcessId, workerId, sessionId);
        //    }
        //    catch (Exception ex)
        //    {
        //        exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
        //        throw;
        //    }
        //}

        /// <summary>
        /// COM: cRBSCLApprovalMilestone.Undo
        /// Undo the RBSCL Approval Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <returns></returns>
        public void UndoRBSCLApproval(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                    long progranRequestId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId},
                    new SqlParameter("@intRBSCLInd", SqlDbType.Int,4) { Value = 0},
                    new SqlParameter("@intISISSession", SqlDbType.Int,4) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISRBSCLUpdate", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cRSFinalAuthMilestone.Undo
        /// Undo RSFinalAuth Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoRSFinalAuth(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                };

                DataTable MilestoneDetails = dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISWorkflowMilestoneInfoGet", CommandType.StoredProcedure, parameters.ToArray());
                string customData = "";
                if (MilestoneDetails.Rows.Count > 0)
                {
                    DataRow dr = MilestoneDetails.Rows[0];
                    customData = Convert.ToString(dr["wpp_CustomData"]);
                }

                if (customData.Length > 0)
                {
                    var outputParameters = new List<SqlParameter>();
                    var roleRecord = customData.Split('|');
                    string servicePlanIds = roleRecord[0];

                    //Unused assessmentDate so i commented.
                    // DateTime assessmentDate = DateTime.Parse(roleRecord[1]);

                    //Pull service plans out of string
                    var servicePlans = servicePlanIds.Split(',');
                    foreach (var servicePlanId in servicePlans)
                    {
                        parameters = new List<SqlParameter>
                        {
                            new SqlParameter("@intServicePlanID", SqlDbType.Int,4) { Value = servicePlanId},
                            new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId }
                        };
                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneRSFinalAuthUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    }
                }
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cTCMServiceAuthMilestone.Undo
        /// Undo the TCMService Authorization Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoTCMServiceAuth(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cTierEntryMilestone.Undo
        /// Undo the TierEntry Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <param name="progranRequestId"></param>
        /// <returns></returns>
        public void UndoTierEntry(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0,
                                    long progranRequestId = 0)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intWorkflowPointProcessID", SqlDbType.Int,4) { Value = workflowPointProcessId },
                    new SqlParameter("@intWorkerID", SqlDbType.Int,4) { Value = workerId },
                    new SqlParameter("@intSessionID", SqlDbType.Int,4) { Value = sessionId },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int,4) { Value = progranRequestId}
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneTierEntryUndo", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cUnnaturalDeathMilestone.Undo
        /// Undo the UnNatural Death Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoUnNaturalDeath(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// COM: cWhatTriggeredMilestone.Undo
        /// Undo the WhatTriggered Milestone Information
        /// </summary>
        /// <param name="workflowPointProcessId"></param>
        /// <param name="workerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public void UndoWhatTriggered(long workflowPointProcessId = 0, long workerId = 0, long sessionId = 0)
        {
            try
            {
                //Perform standard undo
                Undo(workflowPointProcessId, workerId, sessionId);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Save key data elements whenever worker undoes a milestone
        /// Creted by Praveen 06/19/20
        /// </summary>
        /// <param name="request"></param>
        public void SaveUndoAudit(UndoAuditRequest request)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int) { Value = request.ProgramRequestId },
                    new SqlParameter("@lngMilestoneID", SqlDbType.Int) { Value = request.MilestoneId },
                    new SqlParameter("@strMilestoneType", SqlDbType.VarChar,15) { Value = request.MilestoneType },
                    new SqlParameter("@lngWorkflowProcessID", SqlDbType.Int) { Value = request.WorkflowProcessId },
                    new SqlParameter("@lngResponseID", SqlDbType.Int) { Value = request.ResponseId },
                    new SqlParameter("@strCustomData", SqlDbType.VarChar,256) { Value = request.CustomData },
                    new SqlParameter("@strNote", SqlDbType.VarChar,800) { Value = request.Note },
                    new SqlParameter("@lngWorkerID", SqlDbType.Int) { Value = request.WorkerId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMilestoneUndoAuditSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

    }
}
