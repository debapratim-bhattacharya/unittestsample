using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public interface IServicePlan
    {
        bool CheckForException(string tableName, int tablePK);
        void DeleteServicePlan(int servicePlanId, long sessionId);
        void DeleteServicePlan(int servicePlanId, long sessionId, string exceptionNbr, string exceptionComment);
        int GetActiveServicePlan(int programRequestId);
        DataTable GetEnhancedServicesSpans(int servicePlanId, string stateId);
        DataTable GetPlanEnhancedPrograms();
        DataTable GetRsDiagnosis(int servicePlanId);
        DataTable GetSecondaryDiagnosisCodes(int servicePlanId, int axisType);
        DataTable GetServicePlan(int servicePlanId);
        DataTable GetServicePlans(int programRequestId, int filterFlag = 0);
        DataTable GetSupportBroker(string stateId, DateTime servicePlanBeginDate, DateTime servicePlanEndDate);
        DataTable GetSupportBrokers();
        void SaveRsDiagnosis(RsDiagnosisRequest request);
        void SaveSecondaryDiagnosisCodes(int servicePlanId, string axisIDiagnosisCodes, string axisIiDiagnosisCodes);
        int SaveServicePlan(ServicePlanRequest request);
        void SaveSupportBroker(string stateId, long supportBrokerWorkerId, DateTime servicePlanBeginDate, DateTime servicePlanEndDate, long sessionId);
        bool UpdatePlanAuthorization(int sessionId, int servicePlanID);
        string ValidatePlanMsg(int servicePlanID, int programRequestID, DateTime beginDate,
                         DateTime endDate, decimal monthlyCap, decimal yearlyCap, decimal cpFirstMonth,
                         decimal cpOngoing, string levelCode = "",
                         string locEffDate = "", string origLOCDate = "", int supportBrokerID = 0);
        string ValidatePlanUpdate(int servicePlanID, int programRequestID, DateTime beginDate,
                 DateTime endDate, decimal monthlyCap, decimal yearlyCap, decimal cpFirstMonth,
                 decimal cpOngoing, string levelCode = "", string locEffDate = "",
                 string sessionId = "", string origLOCDate = "", int supportBrokerID = 0);
        
    }

    /// <summary>
    /// Author: Praveen Vattela  
    /// Crated Date: 06/12/2020
    /// Modified Date: 06/17/2020
    /// Class Name: Service Plan
    /// </summary>
    public class ServicePlan : IServicePlan
    {
        IDataAccess dataAccess;
        IExceptionHandling exceptionHandling;
        IServiceSpan serviceSpan;
        IProgramRequest programRequest;

        public ServicePlan(IDataAccess access, IExceptionHandling exception, IServiceSpan span, IProgramRequest pr)
        {
            dataAccess = access;
            exceptionHandling = exception;
            serviceSpan = span;
            programRequest = pr;
        }

        /// <summary>
        ///  Delete a single service plan record
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="sessionId"></param>
        public void DeleteServicePlan(int servicePlanId, long sessionId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int){ Value =  servicePlanId },
                    new SqlParameter("@intSessionID", SqlDbType.Int){ Value =  sessionId },
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServicePlanDelete", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Delete a single service plan record
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="sessionId"></param>
        /// <param name="exceptionNbr"></param>
        /// <param name="exceptionComment"></param>
        public void DeleteServicePlan(int servicePlanId, long sessionId, string exceptionNbr, string exceptionComment)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int){ Value =  servicePlanId },
                    new SqlParameter("@intSessionID", SqlDbType.Int){ Value =  sessionId },
                    new SqlParameter("@strExceptionNumber",  SqlDbType.VarChar, 30) { Value = exceptionNbr },
                    new SqlParameter("@strExceptionComment", SqlDbType.VarChar, 500) { Value = exceptionComment },
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServicePlanDelete_Exception", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve service plan id for the active service plan.
        /// </summary>
        /// <param name="programRequestId"></param>
        /// <returns></returns>
        public int GetActiveServicePlan(int programRequestId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int){ Value =  programRequestId },
                    new SqlParameter("@lngResult", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServicePlanActiveGet", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var servicePlanId = 0;
                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@lngResult")
                    {
                        servicePlanId = Convert.ToInt32(parameter.Value);
                    };
                }
                return servicePlanId;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve service span listing for Enhanced programs.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public DataTable GetEnhancedServicesSpans(int servicePlanId, string stateId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@svp_CurrentPlan", SqlDbType.Int){ Value =  servicePlanId },
                    new SqlParameter("@cmr_StateID", SqlDbType.VarChar,8) { Value= stateId }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISEnhancedServicesSpansGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Get a listing of the State Plan Enhanced Programs.
        /// </summary>
        /// <returns></returns>
        public DataTable GetPlanEnhancedPrograms()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISProgramsGetPlanEnhanced", CommandType.StoredProcedure, null);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve service plan data.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <returns></returns>
        public DataTable GetRsDiagnosis(int servicePlanId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Value =  servicePlanId }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISRSDiagnosisGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve secondary diagnosis codes
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="axisType"></param>
        /// <returns></returns>
        public DataTable GetSecondaryDiagnosisCodes(int servicePlanId, int axisType)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Value =   servicePlanId },
                    new SqlParameter("@intAxisType",SqlDbType.Int) { Value =   axisType }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISSecondaryDiagnosisCodesGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve service plan data.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <returns></returns>
        public DataTable GetServicePlan(int servicePlanId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int){ Value =  servicePlanId }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISServicePlanGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve a listing of service plans.
        /// </summary>
        /// <param name="programRequestId"></param>
        /// <param name="filterFlag"></param>
        /// <returns></returns>
        public DataTable GetServicePlans(int programRequestId, int filterFlag = 0)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int){ Value =  programRequestId },
                    new SqlParameter("@intFilterFlag", SqlDbType.Int){ Value =  filterFlag }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISServicePlansGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve Support Broker for a given consumer.
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="servicePlanBeginDate"></param>
        /// <param name="servicePlanEndDate"></param>
        /// <returns></returns>
        public DataTable GetSupportBroker(string stateId, DateTime servicePlanBeginDate, DateTime servicePlanEndDate)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8) { Value= stateId },
                    new SqlParameter("@dteServicePlanBeginDate", SqlDbType.DateTime) { Value = servicePlanBeginDate },
                    new SqlParameter("@dteServicePlanEndDate", SqlDbType.DateTime) { Value = servicePlanEndDate }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISCCOSupportBrokerGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Get a listing of the Programs.
        /// </summary>
        /// <returns></returns>
        public DataTable GetSupportBrokers()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISCCOSupportBrokersGet", CommandType.StoredProcedure, null);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        ///  Save record into Diagnosis..
        /// </summary>
        /// <param name="request"></param>
        public void SaveRsDiagnosis(RsDiagnosisRequest request)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Value =  request.ServicePlanId },
                    new SqlParameter("@intAxis1ID", SqlDbType.Int) { Value =  request.Axis1DiagId },
                    new SqlParameter("@intAxis2ID", SqlDbType.Int) { Value =  request.Axis2DiagId },
                    new SqlParameter("@strAxis3", SqlDbType.VarChar, 300) { Value = request.Axis3 },
                    new SqlParameter("@strAxis4", SqlDbType.VarChar, 350) { Value = request.Axis4 },
                    new SqlParameter("@strAxis5PastYear", SqlDbType.VarChar, 2) { Value = request.Axis5PastYear },
                    new SqlParameter("@strAxis5Session", SqlDbType.VarChar, 2) { Value = request.Axis5Session },
                    new SqlParameter("@strAxis5Current", SqlDbType.VarChar, 2) { Value = request.Axis5Current },
                    new SqlParameter("@strLPHA", SqlDbType.VarChar, 100) { Value = request.Lpha },
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISRSDiagnosisSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Save Secondary Diagnosis Codes.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="axisIDiagnosisCodes"></param>
        /// <param name="axisIiDiagnosisCodes"></param>
        public void SaveSecondaryDiagnosisCodes(int servicePlanId, string axisIDiagnosisCodes, string axisIIDiagnosisCodes)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intServicePlanID", SqlDbType.Int){ Value =  servicePlanId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISSecondaryDiagnosisCodesDelete", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                var diagnosisCodes = axisIDiagnosisCodes.Split('|').ToArray();
                foreach (var diagnosisCode in diagnosisCodes)
                {
                    outputParameters = new List<SqlParameter>();
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intServicePlanID",SqlDbType.Int){ Value =   servicePlanId },
                        new SqlParameter("@intDiagnosisID", SqlDbType.Int){ Value =  diagnosisCode },
                        new SqlParameter("@intAxisType", SqlDbType.Int){ Value =  1 }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISSecondaryDiagnosisCodesSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }

                var iiDiagnosisCodes = axisIIDiagnosisCodes.Split('|').ToArray();
                foreach (var diagnosisCode in iiDiagnosisCodes)
                {
                    outputParameters = new List<SqlParameter>();
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intServicePlanID",SqlDbType.Int) { Value =   servicePlanId },
                        new SqlParameter("@intDiagnosisID", SqlDbType.Int){ Value =  diagnosisCode },
                        new SqlParameter("@intAxisType", SqlDbType.Int){ Value =  1 }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISSecondaryDiagnosisCodesSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Save service plan record for Maintenance..
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int SaveServicePlan(MaintServicePlanRequest request)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = request.ServicePlanId },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value =   request.ProgramRequestId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = request.BeginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = request.EndDate },
                    new SqlParameter("@strLevelCare", SqlDbType.VarChar, 255) { Value = request.LevelCareCode },
                    new SqlParameter("@dteLOCEffDate", SqlDbType.DateTime) { Value = request.LocEffDate != default ? (object) request.LocEffDate : DBNull.Value },
                    new SqlParameter("@dtePlanRevDate", SqlDbType.DateTime) { Value = request.PlanRevDate != default ? (object) request.PlanRevDate : DBNull.Value },
                    new SqlParameter("@intApproved", SqlDbType.TinyInt) { Value =  request.Approved },
                    new SqlParameter("@dteApprovedDate", SqlDbType.DateTime) { Value = request.ApprovedDate != default ? (object) request.ApprovedDate : DBNull.Value },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value =  request.IsisSession },
                    new SqlParameter("@strExceptionNumber", SqlDbType.VarChar, 30) { Value = request.ExceptionNbr },
                    new SqlParameter("@strExceptionComment", SqlDbType.VarChar, 500) { Value = request.ExceptionComments },
                    new SqlParameter("@intArchiveID", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@intException", SqlDbType.TinyInt) { Value =  request.Exception },
                    new SqlParameter("@monMonthlyCap", SqlDbType.Money) { Value = request.MonthlyCap },
                    new SqlParameter("@monYearlyCap", SqlDbType.Money) { Value = request.YearlyCap },
                    new SqlParameter("@dteOrigLOCDate", SqlDbType.DateTime) { Value = request.OrigLocDate != default ? (object) request.OrigLocDate : DBNull.Value },
                    new SqlParameter("@intRequestorID", SqlDbType.Int) { Value =  request.RequestorId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServicePlanSave_Exception", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var servicePlanId = 0;
                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@lngServicePlanID")
                    {
                        servicePlanId = Convert.ToInt32(parameter.Value);
                    };

                }
                return servicePlanId;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Save service plan record ..
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int SaveServicePlan(ServicePlanRequest request)
        {
            try
            {
                //TODO: Removed direct dependency to use injected dependency
                //var programRequest = new ProgramRequest(dataAccess, exceptionHandling);
                var dtProgramRequest = programRequest.GetProgramRequest(request.ProgramRequestId);

                if (dtProgramRequest != null && dtProgramRequest.Rows.Count > 0)
                {
                    var dateConstruct = Convert.ToDateTime($"{DateTime.Now.Month}/01/{DateTime.Now.Year}");
                    if (request.ServicePlanId > 0)
                    {
                        var dtServicePlan = GetServicePlan(request.ServicePlanId);
                        if (dtServicePlan != null && dtServicePlan.Rows.Count > 0)
                        {

                            if (request.BeginDate != Convert.ToDateTime(dtServicePlan.Rows[0]["svp_begindate"]))
                            {
                                //This entire code is unreachable
                                if (dtProgramRequest.Rows[0]["pgr_Begdate"] == null &&
                                    Convert.ToDateTime(dtProgramRequest.Rows[0]["pgr_Begdate"]) < default(DateTime))
                                {
                                    //Program Request is still pending...Validate dates on 31 day rolling window.
                                    //Check Begin date against 31 day rolling window.
                                    if (dtProgramRequest.Rows[0]["prg_name"].ToString() != "Remedial Services")
                                    {
                                        if (request.BeginDate.Date < DateTime.Now.AddDays(-31).Date)
                                        {
                                            return 223;
                                        }
                                    }

                                    if (!(dtProgramRequest.Rows[0]["prg_name"].ToString() == "TCM" || dtProgramRequest.Rows[0]["prg_name"].ToString() == "Remedial Services"))
                                    {
                                        //Validate the Service Begin Date is not prior to current month.

                                        if (request.BeginDate.Date < dateConstruct.Date)
                                        {
                                            return 224;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                //Validate the Service End Date is not prior to current month - This edit does not apply to TCM.
                                if (dtProgramRequest.Rows[0]["prg_name"].ToString() != "TCM")
                                {
                                    if (request.EndDate.Date <
                                        Convert.ToDateTime(dtServicePlan.Rows[0]["svp_Enddate"]).Date)
                                    {
                                        if (request.EndDate.Date < dateConstruct.AddDays(-1).Date)
                                        {
                                            return 231;
                                        }
                                    }
                                }

                                //Validate the Service Plan End Date is not after the program request end date.
                                if (request.EndDate.Date <
                                    Convert.ToDateTime(dtProgramRequest.Rows[0]["pgr_Enddate"]).Date)
                                {
                                    return 234;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Perform Date edits for new service plan
                        if (dtProgramRequest.Rows[0]["pgr_Begdate"] == null &&
                            Convert.ToDateTime(dtProgramRequest.Rows[0]["pgr_Begdate"]) < default(DateTime))
                        {
                            //Program Request is still pending...Validate dates on 31 day rolling window.
                            //Check Begin date against 31 day rolling window.
                            if (dtProgramRequest.Rows[0]["prg_name"].ToString() != "Remedial Services")
                            {
                                if (request.BeginDate < DateTime.Now.AddDays(-31))
                                {
                                    return 223;
                                }
                            }
                        }
                        else
                        {
                            if (!(dtProgramRequest.Rows[0]["prg_name"].ToString() == "TCM" || dtProgramRequest.Rows[0]["prg_name"].ToString() == "Remedial Services"))
                            {
                                //Validate the Service Begin Date is not prior to current month.
                                if (request.BeginDate.Date < dateConstruct.Date)
                                {
                                    return 224;
                                }

                                if (request.EndDate.Date < dateConstruct.AddDays(-1).Date)
                                {
                                    return 231;
                                }
                            }

                            //Validate the Service Plan End Date is not after the program request end date.
                            if (request.EndDate.Date < Convert.ToDateTime(dtProgramRequest.Rows[0]["pgr_Enddate"]).Date)
                            {
                                return 234;
                            }
                        }
                    }
                }

                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = request.ServicePlanId },
                    new SqlParameter("@intProgramRequestID", SqlDbType.Int) { Value =   request.ProgramRequestId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = request.BeginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = request.EndDate },
                    new SqlParameter("@strLevelCare", SqlDbType.VarChar, 255) { Value = request.LevelCareCode },
                    new SqlParameter("@dteLOCEffDate", SqlDbType.DateTime) { Value = request.LocEffDate != default ? (object) request.LocEffDate : DBNull.Value },
                    new SqlParameter("@dtePlanRevDate", SqlDbType.DateTime) { Value = request.PlanRevDate != default ? (object) request.PlanRevDate : DBNull.Value },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value =  request.IsisSession },
                    new SqlParameter("@intArchiveID", SqlDbType.Int, 10) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServicePlanSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var servicePlanId = 0;
                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@lngServicePlanID")
                    {
                        servicePlanId = Convert.ToInt32(parameter.Value);
                    };
                }

                //Now update plan authorization.
                UpdatePlanAuthorization(request.IsisSession, request.ServicePlanId);

                //Now update the PlanIsValid indicator.
                ValidatePlanUpdate(servicePlanId, request.ProgramRequestId, request.BeginDate, request.EndDate, request.MonthlyCap, request.YearlyCap, request.CpFirstMonth, request.CpOngoing,
                    request.LevelCareCode, request.LocEffDate.ToShortDateString(), request.IsisSession.ToString(), request.OrigLocDate.ToShortDateString(), request.SupportBrokerId);

                return servicePlanId;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }


        /// <summary>
        /// Update the PlanIsValid bit.
        /// </summary>
        /// Satish: Removed reviewDate parameter.
        public string ValidatePlanUpdate(int servicePlanID, int programRequestID, DateTime beginDate,
                 DateTime endDate, decimal monthlyCap, decimal yearlyCap, decimal cpFirstMonth,
                 decimal cpOngoing, string levelCode = "", string locEffDate = "",
                 string sessionId = "", string origLOCDate = "", int supportBrokerID = 0)
        {
            try
            {
                string returnMessage = ValidatePlanMsg(servicePlanID, programRequestID, beginDate, endDate,
                             monthlyCap, yearlyCap, cpFirstMonth, cpOngoing, levelCode, locEffDate,
                             origLOCDate, supportBrokerID);

                int planIsValid = returnMessage == "" ? 1 : 0;
                var outParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int, 4) { Value = servicePlanID },
                    new SqlParameter("@tntPlanIsValid", SqlDbType.Int, 4) { Value = planIsValid },
                    new SqlParameter("@intSessionID", SqlDbType.DateTime) { Value = sessionId }
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISPlanIsValidSet", CommandType.StoredProcedure, parameters.ToArray(), ref outParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
            return "";
        }

        /// <summary>
        /// Update the authorization bit and fire change flow if necessary.
        /// </summary>
        /// /// Satish: Removed unused passing parameters ProgramRequestID and ProgramID
        /// <param name="sessionId"></param>
        /// <param name="servicePlanID"></param>
        /// <returns></returns>
        public bool UpdatePlanAuthorization(int sessionId, int servicePlanID)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Value = servicePlanID },
                    new SqlParameter("@intUpdated", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = sessionId },
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISPlanAuthorizationUpdate", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@intUpdated")
                    {
                        return (Convert.ToInt16(parameter.Value) == 1) ? true : false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
        /// <summary>
        /// Check any exception exists in corresponding table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tablePK"></param>
        /// <returns></returns>
        public bool CheckForException(string tableName, int tablePK)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intTablePK", SqlDbType.Int) { Value = tablePK },
                    new SqlParameter("@intException", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                string storedProcedureName = "";
                if (tableName == "ServicePlan") storedProcedureName = "prc_ISISServicePlanExceptionCheck";
                if (tableName == "ServiceSpan") storedProcedureName = "prc_ISISServiceSpanExceptionCheck";

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, storedProcedureName, CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);

                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@intException")
                    {
                        return (Convert.ToInt16(parameter.Value) == 1) ? true : false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// This method validates Service Plan
        /// Business Rules: 
        ///     (Reason to add Busines rule validations in business layer instead of doing in Codebehind is: 
        ///     We need to validate passing values with saved/approved service plan and servicespans)
        ///     Error 201:  LOC and LOC Effective date must be present
        ///     Error 202:  Level of Care effective date must be on or after the app date
        ///     Error 203:  Level of Care effective date for 1st plan must be on or before Program Request begin date
        ///     Error 204:  Service Plan begin date must be on or after Program Request begin date 
        ///     Error 207:  Verify plan does not overlap any existing plans. 
        ///     Error 214:  Verify Sum of CP's on Services(first month) do not exceed CP's on plan
        ///     Error 215:   Verify Sum of CP's on Services(ongoing) do not exceed CP's on plan
        /// </summary>
        /// <param name=""></param>
        /// varReviewDate - Unused in this function; so i(satish) removed
        /// <returns></returns>
        public string ValidatePlanMsg(int servicePlanID, int programRequestID, DateTime beginDate,
                         DateTime endDate, decimal monthlyCap, decimal yearlyCap, decimal cpFirstMonth,
                         decimal cpOngoing, string levelCode = "",
                         string locEffDate = "", string origLOCDate = "", int supportBrokerID = 0)
        {
            string programName;
            int programID;
            DateTime locEffectiveDate;

            bool exceptionExists = CheckForException("ServicePlan", servicePlanID);
            if (exceptionExists) return "";

            //Injecting programRequest dependency instead of resolving it directly here
            //var programRequest = new ProgramRequest(dataAccess, exceptionHandling);
            DataTable dtProgranRequest = programRequest.GetProgramRequest(programRequestID);
            DataTable dtServicePlans = GetServicePlans(programRequestID);

            foreach (DataRow programRequestRow in dtProgranRequest.Rows)
            {
                //Check Begin date against Program Request begin date.
                DateTime pgrBeginDate = Convert.ToDateTime(programRequestRow["pgr_begdate"].ToString());
                if (beginDate < pgrBeginDate)
                {
                    return "204";
                    // return Err.Raise ERR_BASE +204, "Validate Service Plan", ""
                }
                programName = programRequestRow["prg_name"].ToString();
                programID = Convert.ToInt32(programRequestRow["prg_programID"].ToString());

                //If TCM bypase edits, if ARO do ARO base edits, else other edits for all other programs.
                if (programName.Equals("TCM", StringComparison.CurrentCultureIgnoreCase))
                {
                }
                else
                {
                    if (programName.Equals("ARO", StringComparison.CurrentCultureIgnoreCase) ||
                            programName.Equals("Remedial Services", StringComparison.CurrentCultureIgnoreCase) ||
                            programName.Equals("Habilitation Services", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //Validate that all required values are present. (LPHA date must be present)
                        if (string.IsNullOrWhiteSpace(levelCode) || string.IsNullOrWhiteSpace(locEffDate))
                        {
                            return "201";
                            //return  ERR_BASE +201, "Validate Service Plan", "";
                        }

                        //LPHA certification date must be no more than 12 months prior to the start of the service plan.
                        locEffectiveDate = Convert.ToDateTime(locEffDate);
                        TimeSpan getDays = locEffectiveDate - beginDate;
                        if (getDays.TotalDays > 365)
                        {
                            return "243";
                            //return Err.Raise ERR_BASE +243, "PreValidate Service Plan", ""
                        }

                        if (!string.IsNullOrWhiteSpace(origLOCDate))
                        {
                            //Original LPHA date is after the program request begin date
                            DateTime originalLOCDate = Convert.ToDateTime(origLOCDate);
                            if (originalLOCDate > Convert.ToDateTime(programRequestRow["pgr_begdate"].ToString()))
                            {
                                return "258";
                                //return Err.Raise ERR_BASE +258, "Validate Service Plan", "";
                            }
                        }

                        if (programName.Equals("Habilitation Services", StringComparison.CurrentCultureIgnoreCase))
                        {
                            //Sandeep - Is this scenario possible ?
                            //LOC Effective Date can not be more than 13 calendar months old.
                            int checkLOCDifferenceMonths = (locEffectiveDate.Month - DateTime.Now.Month);
                            int checkEnddateMonths = (endDate.Month - DateTime.Now.Month);
                            if (checkLOCDifferenceMonths > 13 && checkEnddateMonths < 1)
                            {
                                return "282";
                                //return Err.Raise ERR_BASE +282, "Validate Service Plan", ""
                            }
                        }
                    }
                    else
                    {
                        // MFP: Do not perform edit #s 202& 203 for MFP per csr# 2979 - 8/28/2008 - BT
                        if (programName.Equals("MFP", StringComparison.CurrentCultureIgnoreCase))
                        {
                        }
                        else
                        {
                            //Validate LOC Effective Date against Application Date.
                            if (!string.IsNullOrWhiteSpace(locEffDate))
                            {
                                locEffectiveDate = Convert.ToDateTime(locEffDate);
                                DateTime pgrApplicationDate = Convert.ToDateTime(programRequestRow["pgr_applicationdate"].ToString());
                                if (locEffectiveDate < pgrApplicationDate)
                                {
                                    return "202";
                                    //Err.Raise ERR_BASE +202, "Validate Service Plan", ""
                                }
                            }

                            //Validate LOC Effective Date against Program Request begin date.
                            //LOC Effective Date must be on or before Program Request begin date
                            //for first service plan only...
                            //Get First ServicePlanID of that ProgramRequestID
                            int firstServicePlanID = 0;
                            foreach (DataRow servicePlanRow in dtServicePlans.Rows)
                            {
                                firstServicePlanID = Convert.ToInt32(servicePlanRow["svp_serviceplanID"].ToString());
                                break;
                            }
                            if (servicePlanID == firstServicePlanID)
                            {
                                //This is the first Service Plan so verify dates...
                                if (!string.IsNullOrWhiteSpace(origLOCDate))
                                {
                                    DateTime originalLOCDate = Convert.ToDateTime(origLOCDate);
                                    DateTime programRequestBeginDate = Convert.ToDateTime(programRequestRow["pgr_begdate"].ToString());
                                    if (originalLOCDate > programRequestBeginDate)
                                    {
                                        return "203";
                                        // return Err.Raise ERR_BASE +203, "Validate Service Plan", ""
                                    }
                                }
                            }
                        }

                        //LOC Effective Date can not be more than 13 calendar months old.
                        locEffectiveDate = Convert.ToDateTime(locEffDate);
                        int checkLOCDifferenceMonths = (locEffectiveDate.Month - DateTime.Now.Month);
                        int checkEnddateMonths = (endDate.Month - DateTime.Now.Month);
                        if (checkLOCDifferenceMonths > 13 && checkEnddateMonths < 1)
                        {
                            return "282";
                            //return Err.Raise ERR_BASE +282, "Validate Service Plan", ""
                        }
                    }
                }

                //Validate Service Plans do not overlap.
                foreach (DataRow servicePlanRow in dtServicePlans.Rows)
                {
                    int servicePlanRowID = Convert.ToInt32(servicePlanRow["svp_serviceplanID"].ToString());
                    DateTime servicePlanBeginDate = Convert.ToDateTime(servicePlanRow["svp_begindate"].ToString());
                    DateTime servicePlanEndDate = Convert.ToDateTime(servicePlanRow["svp_enddate"].ToString());
                    if (servicePlanID != servicePlanRowID)
                    {
                        //Check to make sure the dates don't overlap...
                        if ((beginDate >= servicePlanBeginDate && beginDate <= servicePlanEndDate) ||
                            (endDate >= servicePlanBeginDate && endDate <= servicePlanEndDate))
                        {
                            return "207";
                            // return Err.Raise ERR_BASE +207, "Validate Service Plan", ""
                        }
                    }
                }

                if (servicePlanID > 0)
                {

                    return serviceSpan.ValidateSpans(servicePlanID, beginDate, endDate, monthlyCap, yearlyCap, cpFirstMonth,
                                       cpOngoing, programName, programID, supportBrokerID);
                }
            }
            return "";
        }
        /// <summary>
        /// Save Support Brokers...
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="supportBrokerWorkerId"></param>
        /// <param name="servicePlanBeginDate"></param>
        /// <param name="servicePlanEndDate"></param>
        /// <param name="sessionId"></param>
        public void SaveSupportBroker(string stateId, long supportBrokerWorkerId, DateTime servicePlanBeginDate, DateTime servicePlanEndDate, long sessionId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8) { Value= stateId },
                    new SqlParameter("@intSupportBrokerWorkerID", SqlDbType.Int) { Value= supportBrokerWorkerId },
                    new SqlParameter("@dteServicePlanBeginDate", SqlDbType.Date) { Value = servicePlanBeginDate },
                    new SqlParameter("@dteServicePlanEndDate", SqlDbType.Date) { Value = servicePlanEndDate },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value= sessionId },
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISCCOSupportBrokerSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

    }
}
