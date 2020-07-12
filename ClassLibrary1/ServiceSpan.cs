using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public interface IServiceSpan
    {
        void DeleteServiceSpan(int servicePlanId, int serviceSpanId, long sessionId);
        void DeleteServiceSpan(int serviceSpanId, long sessionId, string exceptionNbr, string exceptionComment);
        DataSet GetEnhancedServicesSpans(int serviceSpanId, string stateId);
        DataSet GetOdsmmisEligInfo(string stateId, DateTime beginDate);
        DataTable GetServiceSpan(int serviceSpanId);
        DataTable GetServiceSpans(int servicePlanId);
        DataTable GetServiceSpansRequiringPaReview(int servicePlanId);
        int SaveServiceSpan(MaintServiceSpanRequest request);
        int SaveServiceSpan(ServiceSpanRequest request);
        string ValidateSpans(int servicePlanId, DateTime servicePlanStartDate, DateTime servicePlanEndDate, decimal curMonthlyCap, decimal curYearlyCap, decimal cpFirstMonth, decimal cpOngoing, string programName, int programId, int supportBrokerId);
    }

    public class ServiceSpan : IServiceSpan
    {
        IDataAccess dataAccess;
        IExceptionHandling exceptionHandling;
        IServicePlan servicePlan;

        public ServiceSpan(IDataAccess access, IExceptionHandling handling, IServicePlan service)
        {
            dataAccess = access;
            exceptionHandling = handling;
            servicePlan = service;
        }

        /// <summary>
        /// Delete a single service span record
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="serviceSpanId"></param>
        /// <param name="sessionId"></param>
        public void DeleteServiceSpan(int servicePlanId, int serviceSpanId, long sessionId)
        {
            try
            {
                var dtServicePlan = servicePlan.GetServicePlan(servicePlanId);
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServiceSpanID", serviceSpanId),
                    new SqlParameter("@intSessionID", sessionId),
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpanDelete", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                if (dtServicePlan != null && dtServicePlan.Rows.Count > 0)
                {
                    var dr = dtServicePlan.Rows[0];
                    servicePlan.ValidatePlanUpdate(servicePlanId,
                        Convert.ToInt32(dr["pgr_ProgramRequestID"]),
                        Convert.ToDateTime(dr["svp_BeginDate"]),
                        Convert.ToDateTime(dr["svp_EndDate"]),
                        Convert.ToDecimal(dr["svp_MonthlyCap"]),
                        Convert.ToDecimal(dr["svp_YearlyCap"]),
                        Convert.ToDecimal(dr["pgr_ClientPartic_FirstMo"]),
                        Convert.ToDecimal(dr["pgr_ClientPartic_Ongoing"]),
                        Convert.ToString(dr["lvl_Code"]),
                        Convert.ToString(dr["svp_LevelCareEffectiveDate"]),
                        Convert.ToString(sessionId),
                        Convert.ToString(dr["svp_OrigLOCDate"])
                        );
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Delete a single service span record
        /// </summary>
        /// <param name="serviceSpanId"></param>
        /// <param name="sessionId"></param>
        /// <param name="exceptionNbr"></param>
        /// <param name="exceptionComment"></param>
        public void DeleteServiceSpan(int serviceSpanId, long sessionId, string exceptionNbr, string exceptionComment)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServiceSpanID", serviceSpanId),
                    new SqlParameter("@intSessionID", sessionId),
                    new SqlParameter("@strExceptionNumber", exceptionNbr),
                    new SqlParameter("@strExceptionComment", exceptionComment),
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpanDelete_Exception", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
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
        /// <param name="serviceSpanId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public DataSet GetEnhancedServicesSpans(int serviceSpanId, string stateId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@svp_CurrentPlan", serviceSpanId),
                    new SqlParameter("@cmr_StateID", SqlDbType.VarChar,8) { Value= stateId }
                };

                return dataAccess.GetDataSet(ApplicationConstants.IsisConnectionString, "prc_ISISEnhancedServicesSpansGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Get Consumer Info from Recipient_DSS database
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="beginDate"></param>
        /// <returns></returns>
        public DataSet GetOdsmmisEligInfo(string stateId, DateTime beginDate)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@dteBeginDate", beginDate),
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8) { Value= stateId }
                };

                return dataAccess.GetDataSet(ApplicationConstants.DataCentralConnectionString, "isis.prc_ODSMMISEligibilityInfoGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve service span data.
        /// </summary>
        /// <param name="serviceSpanId"></param>
        /// <returns></returns>
        public DataTable GetServiceSpan(int serviceSpanId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServiceSpanID", serviceSpanId)
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpanGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve service span listing for a service plan.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <returns></returns>
        public DataTable GetServiceSpans(int servicePlanId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", servicePlanId)
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpansGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve service span required for PA review data.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <returns></returns>
        public DataTable GetServiceSpansRequiringPaReview(int servicePlanId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intServicePlanID", servicePlanId)
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpansRequiringPAGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Save a single service span record
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int SaveServiceSpan(MaintServiceSpanRequest request)
        {
            try
            {
                //Validate the passed in span.
                ValidateSpan(request);

                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int){ Value = request.ServicePlanId },
                    new SqlParameter("@lngServiceSpanID", SqlDbType.Int){ Value =request.ServiceSpanId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = request.BeginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = request.EndDate },
                    new SqlParameter("@intProgramID", SqlDbType.Int){ Value = request.ProgramId },
                    new SqlParameter("@intServiceID", SqlDbType.Int){ Value = request.ServiceId },
                    new SqlParameter("@strProvNum", SqlDbType.VarChar, 255) { Value = request.ProvNum },
                    new SqlParameter("@strSiteNum", SqlDbType.VarChar, 255) { Value =request.SiteNum },
                    new SqlParameter("@sngRate", SqlDbType.Money) { Value = request.CurRate },
                    new SqlParameter("@intUnits", SqlDbType.Int){ Value = request.Units },
                    new SqlParameter("@sngBillable", SqlDbType.Money) { Value = request.Billable },
                    new SqlParameter("@sngCPFirstMo", SqlDbType.Money) { Value = request.CurCpFirstMo },
                    new SqlParameter("@sngCPOngoing", SqlDbType.Money) { Value = request.CurCpOngoing },
                    new SqlParameter("@intSessionID", SqlDbType.Int){ Value = request.IsisSession },
                    new SqlParameter("@strExceptionNumber", SqlDbType.VarChar, 30) { Value = request.ExceptionNbr },
                    new SqlParameter("@strExceptionComment", SqlDbType.VarChar, 500) { Value = request.ExceptionComment },
                    new SqlParameter("@intException", SqlDbType.Int) { Value = request.Exception },
                    new SqlParameter("@intRequestorID", SqlDbType.Int) { Value = request.RequestorId },
                    new SqlParameter("@intApproved", SqlDbType.Int) { Value = request.Approved }
                };
                return dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpanSave_Exception", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Save a single service span record
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int SaveServiceSpan(ServiceSpanRequest request)
        {
            try
            {
                if (request.SentToFiscalAgent != 1)
                {
                    //Validate the passed in span.
                    var validateSpan = ValidateSpan(request);
                    if (string.IsNullOrWhiteSpace(validateSpan)) return 0;
                }
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int){ Value = request.ServicePlanId },
                    new SqlParameter("@lngServiceSpanID", SqlDbType.Int) { Direction = ParameterDirection.InputOutput,Value=request.ServiceSpanId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = request.BeginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = request.EndDate },
                    new SqlParameter("@intProgramID", SqlDbType.Int){ Value = request.ProgramId },
                    new SqlParameter("@intServiceID", SqlDbType.Int){ Value = request.ServiceId },
                    new SqlParameter("@strProvNum", SqlDbType.VarChar, 255) { Value = request.ProvNum },
                    new SqlParameter("@strSiteNum", SqlDbType.VarChar,255){ Value = request.SiteNum },
                    new SqlParameter("@sngRate", SqlDbType.Money) { Value = request.CurRate },
                    new SqlParameter("@intUnits", SqlDbType.Int){ Value = request.Units },
                    new SqlParameter("@sngBillable", SqlDbType.Money) { Value = request.Billable },
                    new SqlParameter("@sngCPFirstMo", SqlDbType.Money) { Value = request.CurCpFirstMo },
                    new SqlParameter("@sngCPOngoing", SqlDbType.Money) { Value = request.CurCpOngoing },
                    new SqlParameter("@intSessionID", SqlDbType.Int){ Value = request.IsisSession }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpanSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var serviceSpanId = 0;
                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@lngServiceSpanID")
                    {
                        serviceSpanId = Convert.ToInt32(parameter.Value);
                    };
                }

                //Now update plan authorization.
                servicePlan.UpdatePlanAuthorization(request.IsisSession, request.ServicePlanId);

                var dtServicePlan = servicePlan.GetServicePlan(request.ServicePlanId);
                var monthlyCap = Convert.ToDecimal(dtServicePlan.Rows[0]["svp_MonthlyCap"]);
                var yearlyCap = Convert.ToDecimal(dtServicePlan.Rows[0]["svp_YearlyCap"]);
                var levelCode = Convert.ToString(dtServicePlan.Rows[0]["lvl_code"]);
                var reviewDate = Convert.ToString(dtServicePlan.Rows[0]["svp_reviewDate"]);
                var effectiveDate = Convert.ToString(dtServicePlan.Rows[0]["svp_LevelCareEffectiveDate"]);
                //Now update the PlanIsValid indicator.
                servicePlan.ValidatePlanUpdate(request.ServicePlanId, request.ProgramRequestId, request.BeginDate, request.EndDate, monthlyCap, yearlyCap, request.CurCpFirstMo,
                     request.CurCpOngoing, levelCode, reviewDate, request.IsisSession.ToString(), effectiveDate, 0);

                return serviceSpanId;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }


        /// <summary>
        /// To validate a service span against business rules.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="servicePlanStartDate"></param>
        /// <param name="servicePlanEndDate"></param>
        /// <param name="curMonthlyCap"></param>
        /// <param name="curYearlyCap"></param>
        /// <param name="cpFirstMonth"></param>
        /// <param name="cpOngoing"></param>
        /// <param name="programName"></param>
        /// <param name="programId"></param>
        /// <param name="supportBrokerId"></param>
        /// <returns></returns>
        public string ValidateSpans(int servicePlanId, DateTime servicePlanStartDate, DateTime servicePlanEndDate, decimal curMonthlyCap,
            decimal curYearlyCap, decimal cpFirstMonth, decimal cpOngoing, string programName, int programId, int supportBrokerId)
        {
            var curMonthlyTotals = new decimal[12];
            decimal curYearlyTotal = 0;

            var dtServiceSpans = GetServiceSpan(servicePlanId);
            if (dtServiceSpans == null || dtServiceSpans.Rows.Count == 0)
            {
                return "220";
            }
            //Loop through the spans and add up Monthly and Yearly impacts, and CP's
            //If a CCO Service code exists you must have a support broker
            foreach (DataRow serviceRow in dtServiceSpans.Rows)
            {
                var name = Convert.ToString(serviceRow["srg_name"]);
                var startDate = Convert.ToDateTime(serviceRow["svs_RateStartDate"]);
                var endDate = Convert.ToDateTime(serviceRow["svs_RateEndDate"]);

                //Check Begin date against Program Request begin date.
                if (serviceRow["srg_name"].ToString() == "CCO Services")
                {
                    if (supportBrokerId <= 0)
                    {
                        return "277";
                    }
                }

                if (serviceRow["prs_CapType"].ToString() == "M")
                {

                    decimal curBudgetMonthlyCapAmount = 0;
                    var dtConsumer = GetConsumerBudgetTotal(servicePlanId, startDate);
                    if (dtConsumer != null && dtConsumer.Rows.Count > 0)
                    {
                        curBudgetMonthlyCapAmount = Convert.ToDecimal(dtConsumer.Rows[0]["cbt_MonthlyCapAmount"]);
                    }

                    var startDateMonth = startDate.Month;
                    var endDateMonth = endDate.Month;


                    for (var i = startDateMonth; i < endDateMonth; i++)
                    {
                        if ((programId == 10 && (name == "Case Management Services" || name == "HVM Services" || name == "Personal Care-Adaptive Services"))
                            || (programId == 12 && (name == "Case Management Services" || name == "HVM Services"))
                            || ((programId == 13 || programId == 8) && name == "HVM Services") || (programId == 120 && name == "Personal Care-Adaptive Services"))
                        {
                            // Do not add elderly case management or HVM or Personal Care services
                            // Do not add Brain injury case management or HVM services
                            // Do not add Physical Disability HVM services
                            // Do not add Ill&Handicapped HVM services
                            // Do not add CMH Personal Care services
                        }
                        else
                        {
                            if (curMonthlyCap > 0)
                            {
                                if (name == "CCO Services")
                                {
                                    if (Convert.ToInt32(serviceRow["svs_exception"]) == 1)
                                    {
                                        return "";
                                    }

                                    if (curBudgetMonthlyCapAmount > curMonthlyCap)
                                    {
                                        return "205";
                                    }

                                    if (curBudgetMonthlyCapAmount > 0)
                                    {
                                        if (startDate.Month >= Convert.ToDateTime(serviceRow["cbt_BudgetStartDate"]).Month)
                                        {
                                            curMonthlyTotals[i] = curMonthlyTotals[i] + curBudgetMonthlyCapAmount;
                                        }
                                    }
                                }
                                else
                                {
                                    if (startDate.Month >= Convert.ToDateTime(serviceRow["svs_RateStartDate"]).Month)
                                    {
                                        curMonthlyTotals[i] = curMonthlyTotals[i] + Convert.ToDecimal(serviceRow["MonthlyTotalFirstMonth"]);
                                    }
                                    else
                                    {
                                        curMonthlyTotals[i] = curMonthlyTotals[i] + Convert.ToDecimal(serviceRow["MonthlyTotalOngoing"]);
                                    }
                                }
                            }
                        }
                    }

                }
                else
                {
                    if (Convert.ToDecimal(serviceRow["MonthlyTotalFirstMonth"]) > Convert.ToDecimal(serviceRow["MonthlyTotalOngoing"]))
                    {
                        curYearlyTotal += Convert.ToDecimal(serviceRow["MonthlyTotalFirstMonth"]);
                    }
                    else
                    {
                        curYearlyTotal += Convert.ToDecimal(serviceRow["MonthlyTotalOngoing"]);
                    }
                }

                if (Convert.ToDateTime(serviceRow["svs_RateStartDate"]) < servicePlanStartDate)
                {
                    return "221";
                }
                if (Convert.ToDateTime(serviceRow["svs_RateEndDate"]) > servicePlanEndDate)
                {
                    return "222";
                }

                //If Habilitation Services Program(id=126) or Brain Injury Program (id=12) or
                //   Elderly Program (id=10) the service is for Case Management
                //      then perform edit to see if on TCM program too
                if (((programId == 126 || programId == 12 || programId == 10) && name == "Case Management Services"))
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = servicePlanId },
                        new SqlParameter("@dteServiceStartDate", SqlDbType.DateTime) {  Value = startDate },
                        new SqlParameter("@dteServiceEndDate",SqlDbType.DateTime) {  Value = endDate },
                        new SqlParameter("@lngServiceID", SqlDbType.Int) { Value = Convert.ToInt32(serviceRow["svs_ServiceID"]) },
                        new SqlParameter("@lngProgramID", SqlDbType.Int) { Value = programId },
                        new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISOtherProgramswithCaseManagementtoTCMEdit", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var intResult = 0;
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intResult")
                        {
                            intResult = Convert.ToInt32(parameter.Value);
                        };
                    }

                    if (intResult != 0)
                    {
                        return intResult.ToString();
                    }
                }


                //If TCM Program
                //Perform edit to determine if Sevrvice is appropriate for the consumers age
                //And If Services Being added are proper if consumer has open/future end Facility PR
                if (programName == "TCM")
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = servicePlanId },
                        new SqlParameter("@dteServiceStartDate", SqlDbType.DateTime) {  Value = startDate },
                        new SqlParameter("@dteServiceEndDate",SqlDbType.DateTime) {  Value = endDate },
                        new SqlParameter("@intServiceID", SqlDbType.Int) { Value = Convert.ToInt32(serviceRow["svs_ServiceID"]) },
                        new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISTCMServiceChildAgeEdit", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var intResult = 0;
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intResult")
                        {
                            intResult = Convert.ToInt32(parameter.Value);
                        };
                    }

                    switch (intResult)
                    {
                        case 320://Failure
                            return "255";
                        case 321:
                            return "254";
                        case 322:
                            return "256";
                        case 323:
                            return "241";
                    }

                    outputParameters = new List<SqlParameter>();
                    parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = servicePlanId },
                        new SqlParameter("@dteServiceStartDate", SqlDbType.DateTime) {  Value = startDate },
                        new SqlParameter("@dteServiceEndDate",SqlDbType.DateTime) {  Value = endDate },
                        new SqlParameter("@intServiceID", SqlDbType.Int) { Value = Convert.ToInt32(serviceRow["svs_ServiceID"]) },
                        new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISTCMtoOpenFacilityEdit", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    intResult = 0;
                    foreach (var parameter in outputParameters)
                    {
                        if (parameter.ParameterName == "@intResult")
                        {
                            intResult = Convert.ToInt32(parameter.Value);
                        };
                    }

                    switch (intResult)
                    {
                        case 332://Failure
                            return "287";
                        case 333:
                            return "288";
                        case 334:
                            return "289";
                    }
                }

            }

            if (curMonthlyCap > 0)
            {
                for (int i = 0; i <= 12; i++)
                {
                    if (curMonthlyTotals[i] > curMonthlyCap)
                    {
                        return "205";
                    }
                }
            }

            //Now check Service units against max's
            if (CheckServiceUnits(servicePlanId))
            {
                return "217";
            }

            if (curYearlyTotal > 0)
            {
                if (curYearlyTotal > curYearlyCap)
                {
                    return "219";
                }
            }

            var outputParameters1 = new List<SqlParameter>();
            var parameters1 = new List<SqlParameter>
            {
                new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = servicePlanId },
                new SqlParameter("@curCP1stMO", SqlDbType.Money) {  Value = cpFirstMonth },
                new SqlParameter("@curCPOng",SqlDbType.Money) {  Value = cpOngoing },
                new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISClientParticEdit", CommandType.StoredProcedure, parameters1.ToArray(), ref outputParameters1);
            var iResult = 0;
            foreach (var parameter in outputParameters1)
            {
                if (parameter.ParameterName == "@intResult")
                {
                    iResult = Convert.ToInt32(parameter.Value);
                };
            }

            switch (iResult)
            {
                case -100:
                    return "214";
                case -101:
                    return "215";
            }

            return string.Empty;
        }


        public string ValidateSpan(ServiceSpanRequest request)
        {

            bool exceptionExists = servicePlan.CheckForException("ServiceSpan", request.ServiceSpanId);
            if (exceptionExists) return "";

            var dtServicePlan = servicePlan.GetServicePlan(request.ServicePlanId);

            var reference = new Reference(dataAccess, exceptionHandling);
            var dtService = reference.GetService(request.ServiceId, request.ProgramId);

            var provider = new Provider(dataAccess, exceptionHandling);
            var dtProviderService = provider.GetProviderService(request.ProvNum, 0, request.ProgramId, request.ServiceId);
            var dtProviderServiceRates = provider.GetProviderServiceRates(request.ProvNum, request.ServiceId, request.ProgramId);

            //Check Service Span dates against Service Plan dates.
            if (request.BeginDate.Date < Convert.ToDateTime(dtServicePlan.Rows[0]["svp_BeginDate"]).Date || request.EndDate.Date < Convert.ToDateTime(dtServicePlan.Rows[0]["svp_EndDate"]).Date)
            {
                return "301";
            }

            //Check units are less than Procedure Code max units
            if (dtService.Rows.Count > 0 && dtService.Rows[0]["srv_MaxUnits"] != null && Convert.ToInt32(dtService.Rows[0]["srv_MaxUnits"]) > 0)
            {
                if (request.Billable > Convert.ToInt32(dtService.Rows[0]["srv_MaxUnits"]) ||
                    request.Units > Convert.ToInt32(dtService.Rows[0]["srv_MaxUnits"]))
                {
                    return "303";
                }
            }

            if (request.ProgramId != 125)
            {

                if (dtProviderService == null || dtProviderService.Rows.Count == 0)
                {
                    return "306";
                }
                bool provCert = false;
                if (dtProviderServiceRates != null && dtProviderServiceRates.Rows.Count > 0)
                {
                    foreach (DataRow drSrRow in dtProviderServiceRates.Rows)
                    {
                        if (request.BeginDate.Date >= Convert.ToDateTime(drSrRow["psv_RateEffectiveDate"]).Date || string.IsNullOrWhiteSpace(drSrRow["psv_RateEffectiveDate"].ToString()))
                        {
                            if (Convert.ToInt32(drSrRow["psv_Rate"]) > 0)
                            {
                                if (request.CurRate > Convert.ToInt32(drSrRow["psv_Rate"]))
                                    return "305";
                            }
                            else if (Convert.ToInt32(drSrRow["prs_rate"]) > 0)
                            {
                                if (request.CurRate > Convert.ToInt32(drSrRow["prs_rate"]))
                                    return "304";
                            }
                        }
                    }
                }

                foreach (DataRow drServiceRow in dtProviderService.Rows)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(drServiceRow["psv_TerminationDate"])) ||
                        Convert.ToDateTime(drServiceRow["psv_TerminationDate"]) < default(DateTime))
                    {
                        provCert = true;
                    }
                    else
                    {
                        if (request.EndDate.Date < Convert.ToDateTime(drServiceRow["psv_TerminationDate"]).Date)
                        {
                            provCert = true;
                        }
                    }

                    if (!provCert)
                    {
                        return "306";
                    }
                }
            }

            //Check Billable Units vs. Total Units
            if (Convert.ToInt32(dtService.Rows[0]["srv_SNBUVisible"]) > 0)
            {
                if (request.Units < request.Billable)
                {
                    return "307";
                }
            }


            if ((request.ProgramId == 126 || request.ProgramId == 12 || request.ProgramId == 10) && Convert.ToString(dtService.Rows[0]["srg_name"]) == "Case Management Services")
            {
                var outputParametersCase = new List<SqlParameter>();
                var parametersCase = new List<SqlParameter>
                {
                    new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = request.ServicePlanId },
                    new SqlParameter("@dteServiceStartDate", SqlDbType.DateTime) {  Value = request.BeginDate },
                    new SqlParameter("@dteServiceEndDate",SqlDbType.DateTime) {  Value = request.EndDate },
                    new SqlParameter("@lngServiceID", SqlDbType.Int) { Value = request.ServiceId },
                    new SqlParameter("@lngProgramID", SqlDbType.Int) { Value = request.ProgramId },
                    new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISOtherProgramswithCaseManagementtoTCMEdit", CommandType.StoredProcedure, parametersCase.ToArray(), ref outputParametersCase);
                var intResult = 0;
                foreach (var parameter in outputParametersCase)
                {
                    if (parameter.ParameterName == "@intResult")
                    {
                        intResult = Convert.ToInt32(parameter.Value);
                    };
                }
                switch (intResult)
                {
                    case 253://Failure
                        return "319";
                    case 275:
                        return "325";
                    case 276:
                        return "326";
                }
            }

            //If MR(id=11) or MFP(id=127) and the service is W1320 (RBSCL), then consumer had to have been thru an RBSCL approval flow (pgr_RBSCLInd = 1)
            if ((request.ProgramId == 127 || request.ProgramId == 11) && Convert.ToString(dtService.Rows[0]["srg_name"]) == "SCL Services")
            {
                if (Convert.ToInt32(dtServicePlan.Rows[0]["pgr_RBSCLInd"]) != 1)
                {
                    return "327";
                }
            }

            var outputParameters = new List<SqlParameter>();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = request.ServicePlanId },
                new SqlParameter("@intServiceSpanID", SqlDbType.Int) { Value = request.ServiceSpanId },
                new SqlParameter("@dteServiceStartDate", SqlDbType.DateTime) {  Value = request.BeginDate },
                new SqlParameter("@dteServiceEndDate",SqlDbType.DateTime) {  Value = request.EndDate },
                new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISExistingTCMServiceEdit", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            var iResult = 0;
            foreach (var parameter in outputParameters)
            {
                if (parameter.ParameterName == "@intResult")
                {
                    iResult = Convert.ToInt32(parameter.Value);
                };
            }

            if (iResult != 0)
            {
                return iResult.ToString();
            }


            outputParameters = new List<SqlParameter>();
            parameters = new List<SqlParameter>
            {
                new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = request.ServicePlanId },
                new SqlParameter("@dteServiceStartDate", SqlDbType.DateTime) {  Value = request.BeginDate },
                new SqlParameter("@dteServiceEndDate",SqlDbType.DateTime) {  Value = request.EndDate },
                new SqlParameter("@intServiceID", SqlDbType.Int) { Value = request.ServiceId },
                new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISTCMServiceChildAgeEdit", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            iResult = 0;
            foreach (var parameter in outputParameters)
            {
                if (parameter.ParameterName == "@intResult")
                {
                    iResult = Convert.ToInt32(parameter.Value);
                };
            }

            if (iResult != 0)
            {
                return iResult.ToString();
            }

            outputParameters = new List<SqlParameter>();
            parameters = new List<SqlParameter>
            {
                new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = request.ServicePlanId },
                new SqlParameter("@dteServiceStartDate", SqlDbType.DateTime) {  Value = request.BeginDate },
                new SqlParameter("@dteServiceEndDate",SqlDbType.DateTime) {  Value = request.EndDate },
                new SqlParameter("@intServiceID", SqlDbType.Int) { Value = request.ServiceId },
                new SqlParameter("@intResult",SqlDbType.Int) { Direction = ParameterDirection.Output }
            };
            dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, "prc_ISISTCMtoOpenFacilityEdit", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            iResult = 0;
            foreach (var parameter in outputParameters)
            {
                if (parameter.ParameterName == "@intResult")
                {
                    iResult = Convert.ToInt32(parameter.Value);
                };
            }

            if (iResult != 0)
            {
                return iResult.ToString();
            }

            dtService.Dispose();
            dtServicePlan.Dispose();
            dtProviderService.Dispose();
            dtProviderServiceRates.Dispose();

            return string.Empty;
        }


        public string ValidateSpan(MaintServiceSpanRequest request)
        {
            if (request.Exception > 0) return "";

            var dtServicePlan = servicePlan.GetServicePlan(request.ServicePlanId);

            var reference = new Reference(dataAccess, exceptionHandling);
            var dtService = reference.GetService(request.ServiceId, request.ProgramId);

            var provider = new Provider(dataAccess, exceptionHandling);
            var dtProviderService = provider.GetProviderService(request.ProvNum, 0, request.ProgramId, request.ServiceId);
            var dtProviderServiceRates = provider.GetProviderServiceRates(request.ProvNum, request.ServiceId, request.ProgramId);

            //Check Service Span dates against Service Plan dates.
            if (request.BeginDate.Date < Convert.ToDateTime(dtServicePlan.Rows[0]["svp_BeginDate"]).Date || request.EndDate.Date < Convert.ToDateTime(dtServicePlan.Rows[0]["svp_EndDate"]).Date)
            {
                return "301";
            }

            //Check units are less than Procedure Code max units
            if (dtService.Rows.Count > 0 && dtService.Rows[0]["srv_MaxUnits"] != null && Convert.ToInt32(dtService.Rows[0]["srv_MaxUnits"]) > 0)
            {
                if (request.Billable > Convert.ToInt32(dtService.Rows[0]["srv_MaxUnits"]) ||
                    request.Units > Convert.ToInt32(dtService.Rows[0]["srv_MaxUnits"]))
                {
                    return "303";
                }
            }

            if (request.ProgramId != 125)
            {

                if (dtProviderService == null || dtProviderService.Rows.Count == 0)
                {
                    return "306";
                }
                bool provCert = false;
                if (dtProviderServiceRates != null && dtProviderServiceRates.Rows.Count > 0)
                {
                    foreach (DataRow drSrRow in dtProviderServiceRates.Rows)
                    {
                        if (request.BeginDate.Date >= Convert.ToDateTime(drSrRow["psv_RateEffectiveDate"]).Date || string.IsNullOrWhiteSpace(drSrRow["psv_RateEffectiveDate"].ToString()))
                        {
                            if (Convert.ToInt32(drSrRow["psv_Rate"]) > 0)
                            {
                                if (request.CurRate > Convert.ToInt32(drSrRow["psv_Rate"]))
                                    return "305";
                            }
                        }
                    }
                }

                foreach (DataRow drServiceRow in dtProviderService.Rows)
                {
                    if (string.IsNullOrWhiteSpace(Convert.ToString(drServiceRow["psv_TerminationDate"])) ||
                        Convert.ToDateTime(drServiceRow["psv_TerminationDate"]) < default(DateTime))
                    {
                        provCert = true;
                    }
                    else
                    {
                        if (request.EndDate.Date < Convert.ToDateTime(drServiceRow["psv_TerminationDate"]).Date)
                        {
                            provCert = true;
                        }
                    }

                    if (!provCert)
                    {
                        return "306";
                    }
                }
            }

            //Check Billable Units vs. Total Units
            if (Convert.ToInt32(dtService.Rows[0]["srv_SNBUVisible"]) > 0)
            {
                if (request.Units < request.Billable)
                {
                    return "307";
                }
            }

            dtService.Dispose();
            dtServicePlan.Dispose();
            dtProviderService.Dispose();
            dtProviderServiceRates.Dispose();

            return string.Empty;
        }


        DataTable GetConsumerBudgetTotal(int servicePlanId, DateTime serviceStartDate)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID",SqlDbType.Int) { Value = servicePlanId },
                    new SqlParameter("@dteServiceStartDate",SqlDbType.Int) { Value = serviceStartDate }
                };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISCCOConsumerBudgetTotalGet", CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        bool CheckServiceUnits(int servicePlanId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intServicePlanID",SqlDbType.Int) { Value = servicePlanId }
                };

                var dtServiceUnits = dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "prc_ISISServiceSpansUnitsCheck", CommandType.StoredProcedure, parameters.ToArray());
                return dtServiceUnits != null && dtServiceUnits.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

    }
}
