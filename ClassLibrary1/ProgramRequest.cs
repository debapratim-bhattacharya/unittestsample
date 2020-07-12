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
    /// Author: Praveen Vattela  
    /// Crated Date: 06/12/2020
    /// Modified Date: 06/16/2020
    /// Class Name: Service Plan
    /// </summary>

    public class ProgramRequest : IProgramRequest
        {

            IDataAccess dataAccess;
            IExceptionHandling exceptionHandling;
            public ProgramRequest(IDataAccess access, IExceptionHandling exception)
            {
                dataAccess = access;
                exceptionHandling = exception;
            }

            /// <summary>
            /// Save/Change Aid Type
            /// </summary>
            /// <param name="isisSession"></param>
            /// <param name="programRequestID"></param>
            /// <param name="newAidType"></param>
            public void ChangeAidType(long isisSession, int programRequestID, int newAidType)
            {
                try
                {
                    var outParameters = new List<SqlParameter>();

                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intISISSession", SqlDbType.Int, 4) { Value = isisSession },
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int, 4) { Value = programRequestID },
                    new SqlParameter("@lngNewAidType", SqlDbType.Int, 4) { Value = newAidType }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMDSQReferralSave", CommandType.StoredProcedure, parameters.ToArray(), ref outParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Change CP Amount
            /// </summary>
            /// <param name="isisSession"></param>
            /// <param name="programRequestID"></param>
            /// <param name="curCPFirstMonth"></param>
            /// <param name="curCPOngoing"></param>
            /// <param name="splitDate"></param>
            /// <returns></returns>
            public int ChangeCPAmount(long isisSession, int programRequestID, decimal curCPFirstMonth, decimal curCPOngoing, DateTime splitDate)
            {
                var requestId = 0;
                try
                {
                    var outParameters = new List<SqlParameter>();

                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intISISSession", SqlDbType.Int, 4) { Value = isisSession },
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int, 4) { Value = programRequestID },
                    new SqlParameter("@curNewCPFirstMonth", SqlDbType.Money, 8) { Value = curCPFirstMonth },
                    new SqlParameter("@curNewCPOnGoing", SqlDbType.Money, 1) { Value = curCPOngoing },
                    new SqlParameter("@dteSplitDate", SqlDbType.DateTime) { Value = splitDate }
                };


                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestChangeCPAmounts", CommandType.StoredProcedure, parameters.ToArray(), ref outParameters);
                    foreach (var parameter in outParameters)
                    {
                        if (parameter.ParameterName == "@lngProgramRequestID") requestId = Convert.ToInt32(parameter.Value);
                    }
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
                return requestId;
            }

            /// <summary>
            /// Consumer entering leaving facility..
            /// </summary>
            /// <param name="isisSession"></param>
            /// <param name="programRequestID"></param>
            /// <param name="effectiveDate"></param>
            /// <param name="inaFacilityInd"></param>
            /// <param name="roleID"></param>
            public void ConsumerEnteringLeavingFacility(long isisSession, int programRequestID, DateTime effectiveDate, int inaFacilityInd, int roleID)
            {
                try
                {
                    var outParameters = new List<SqlParameter>();

                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intISISSession", SqlDbType.Int, 4) { Value = isisSession },
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int, 4) { Value = programRequestID },
                    new SqlParameter("@dteEffectiveDate", SqlDbType.DateTime) { Value = effectiveDate },
                    new SqlParameter("@intInaFacilityInd", SqlDbType.Int) { Value = inaFacilityInd },
                    new SqlParameter("@intRoleID", SqlDbType.Int) { Value = roleID }
                };

                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestChangeCPAmounts", CommandType.StoredProcedure, parameters.ToArray(), ref outParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Delete program request.  This includes related service plans and workflows.
            /// </summary>
            /// <param name="isisSession"></param>
            /// <param name="programRequestID"></param>
            /// <param name="exceptionNbr"></param>
            /// <param name="exceptionComment"></param>
            public void DeleteProgramRequest(long isisSession, int programRequestID, string exceptionNbr, string exceptionComment)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intISISSession", SqlDbType.Int, 4) { Value = isisSession },
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int, 4) { Value = programRequestID },
                    new SqlParameter("@strExceptionNumber", SqlDbType.VarChar, 30) { Value = exceptionNbr },
                    new SqlParameter("@strExceptionComment", SqlDbType.VarChar, 500) { Value = exceptionComment },
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestDelete_Exception", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve begin codes.
            /// </summary>
            /// <returns></returns>
            public DataTable GetBeginCodes()
            {
                try
                {
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISBeginCodesGet", CommandType.StoredProcedure, null);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Get a listing of the all the dcns for a supplied service plan
            /// </summary>
            /// <param name="servicePlanID"></param>
            /// <returns></returns>
            public DataTable GetDCNs(int servicePlanID)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@dintServicePlanID", SqlDbType.Int) { Value = servicePlanID }
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISDCNsGet", CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Get program request recordset.
            /// </summary>
            /// <param name="programRequestID"></param>
            /// <returns></returns>
            public DataTable GetProgramRequest(int programRequestID)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = programRequestID }
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestGet", CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Get program request diagnoses recordset.
            /// </summary>
            /// <param name="programRequestID"></param>
            /// <returns></returns>
            public DataTable GetProgramRequestDiagnoses(int programRequestID)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = programRequestID }
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestDiagnosesGet", CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Get program request recordset.
            /// </summary>
            /// <param name="stateId"></param>
            /// <returns></returns>
            public DataSet GetProgramRequests(string stateId)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strStateID", SqlDbType.VarChar,10) { Value = stateId },
                    new SqlParameter("@intResult", SqlDbType.Int) { Direction = ParameterDirection.Output },
                };
                    return dataAccess.GetDataSet(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestsGet", CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Get a listing of the Diagnosis Codes.
            /// </summary>
            /// <param name="programRequestID"></param>
            /// <param name="servicePlanBeginDate"></param>
            /// <param name="servicePlanEndDate"></param>
            /// <returns></returns>
            public DataTable GetProgramRequestTierHistory(int programRequestID, DateTime servicePlanBeginDate, DateTime servicePlanEndDate)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = programRequestID },
                    new SqlParameter("@dteServicePlanBegDate", SqlDbType.DateTime) { Value = servicePlanBeginDate },
                    new SqlParameter("@dteServicePlanEndDate", SqlDbType.DateTime) { Value = servicePlanEndDate }
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestTierHistoryGet", CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve Requestor information.
            /// </summary>
            /// <param name="programRequestID"></param>
            /// <returns></returns>
            public DataTable GetRequestors(int programRequestID)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = programRequestID }
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISRequestorsGet", CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve status codes.
            /// </summary>
            /// <returns></returns>
            public DataTable GetStatusCodes()
            {
                try
                {
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISStatusCodesGet", CommandType.StoredProcedure, null);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve TCM Service for specified PR and Assessment date
            /// </summary>
            /// <param name="programRequestID"></param>
            /// <param name="mileStoneId"></param>
            /// <param name="idType"></param>
            /// <param name="responseDate"></param>
            /// <returns></returns>
            public DataTable GetTCMServiceAuthorization(int programRequestID, int mileStoneId, string idType, DateTime responseDate)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value = programRequestID },
                    new SqlParameter("@intMilestoneID", SqlDbType.Int) { Value = mileStoneId },
                    new SqlParameter("@strIDType", SqlDbType.Char, 1) { Value = idType },
                    new SqlParameter("@dteServicePlanEndDate", SqlDbType.DateTime) { Value = responseDate }
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISTCMServiceAuthorizationGet", CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve termination codes.
            /// </summary>
            /// <returns></returns>
            public DataTable GetTerminationCodes()
            {
                try
                {
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISTerminationCodesGet", CommandType.StoredProcedure, null);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Hospice NF Transfer
            /// </summary>
            /// <param name="isisSession"></param>
            /// <param name="programRequestID"></param>
            /// <param name="splitDate"></param>
            /// <returns></returns>
            public int HospiceNFTransfer(long isisSession, int programRequestID, DateTime splitDate)
            {
                var requestId = 0;
                try
                {
                    var outParameters = new List<SqlParameter>();

                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intISISSession", SqlDbType.Int, 4) { Value = isisSession },
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int, 4) { Value = programRequestID },
                    new SqlParameter("@dteSplitDate", SqlDbType.DateTime) { Value = splitDate }
                };


                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISProgramRequestHospiceNFTransfer", CommandType.StoredProcedure, parameters.ToArray(), ref outParameters);
                    foreach (var parameter in outParameters)
                    {
                        if (parameter.ParameterName == "@lngProgramRequestID") requestId = Convert.ToInt32(parameter.Value);
                    }
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
                return requestId;
            }

            /// <summary>
            /// Save or update program request record.
            /// </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            public string SaveProgramRequest(SaveProgramRequest request)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intISISSession", SqlDbType.Int) { Value  = request.IsisSession},
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = request.ProgramRequestId },
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8) { Value = request.StateId },
                    new SqlParameter("@dteBeginDate", SqlDbType.Date) { Value = request.BeginDate },
                    new SqlParameter("@strBeginCode", SqlDbType.VarChar, 3) { Value = request.BeginCode },
                    new SqlParameter("@strStatusCode", SqlDbType.VarChar,3) { Value = request.StatusCode },
                    new SqlParameter("@dteEndDate", SqlDbType.Date) { Value = request.EndDate },
                    new SqlParameter("@strEndCode", SqlDbType.VarChar,3) { Value = request.EndCode },
                    new SqlParameter("@strAidType", SqlDbType.VarChar,3) { Value = request.AidType },
                    new SqlParameter("@intLevelCareID", SqlDbType.Int) { Value = request.LevelCareId },
                    new SqlParameter("@intProgramID", SqlDbType.Int) { Value = request.ProgramId },
                    new SqlParameter("@intCountyID", SqlDbType.Int) { Value = request.CountyId },
                    new SqlParameter("@strProviderNum", SqlDbType.VarChar, 7) { Value = request.ProviderNum },
                    new SqlParameter("@curCPFirstMonth", SqlDbType.Money, 8) { Value = request.CurCpFirstMonth },
                    new SqlParameter("@curCPOnGoing", SqlDbType.Money, 1) { Value = request.CurCpOngoing },
                    new SqlParameter("@dteAssessmentDate", SqlDbType.Date) { Value = request.AssessmentDate },
                    new SqlParameter("@dteApplicationDate", SqlDbType.Date) { Value = request.ApplicationDate },
                    new SqlParameter("@strExceptionNumber", SqlDbType.VarChar, 30) { Value = request.ExceptionNbr },
                    new SqlParameter("@strExceptionComment", SqlDbType.VarChar, 500) { Value = request.ExceptionComment },
                    new SqlParameter("@strCaseID", SqlDbType.VarChar, 50) { Value = request.CaseId },
                    new SqlParameter("@intApproved", SqlDbType.Int) { Value = request.Approved },
                    new SqlParameter("@dteCSRDate", SqlDbType.Date) { Value = request.CsrDate },
                    new SqlParameter("@dteOrigAssessmentDate", SqlDbType.Date) { Value = request.OrigAssessmentDate },
                    new SqlParameter("@intRequestorID",SqlDbType.Int) { Value =  request.RequestorId },
                    new SqlParameter("@strTermCode", SqlDbType.Char,1){Value = request.TermCode},
                    new SqlParameter("@dteTermDate", SqlDbType.Date) { Value = request.TermDate },
                    new SqlParameter("@intProviderFound", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@intOutofStateSkilledInd",SqlDbType.Int) { Value =  request.OutofStateSkilledInd },
                    new SqlParameter("@strHospiceNFProviderNum", SqlDbType.VarChar, 7){Value = request.HospiceNfProviderNum},
                    new SqlParameter("@intHospiceNFProviderFound", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@intRBSCL",SqlDbType.Int) { Value =  request.Rbscl },
                    new SqlParameter("@intInaFacility",SqlDbType.Int) { Value =  request.InaFacility },
                    new SqlParameter("@strTier", SqlDbType.Char,1){Value = request.Tier},
                    new SqlParameter("@dteTierEffectiveDate", SqlDbType.Date) { Value = request.TierEffectiveDate },
                    new SqlParameter("@intWorkerID",SqlDbType.Int) { Value =  request.WorkerId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISServicePlanSave_Exception", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var programRequestId = 0;
                    var providerFoundSwitch = 0;
                    foreach (var parameter in outputParameters)
                    {
                        switch (parameter.ParameterName)
                        {
                            case "@intProgramRequestID":
                                programRequestId = Convert.ToInt32(parameter.Value);
                                break;
                            case "@intProviderFound":
                                providerFoundSwitch = Convert.ToInt32(parameter.Value);
                                break;
                        }
                    }

                    if (providerFoundSwitch == 1 || providerFoundSwitch == 2)
                    {
                        return "Validate Provider Number";
                    }
                    return programRequestId.ToString();
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }
        }

    public interface IProgramRequest
    {
        DataTable GetProgramRequest(int programRequestID);
    }
}




