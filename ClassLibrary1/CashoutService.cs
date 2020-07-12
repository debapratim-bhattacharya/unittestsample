using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public class CashoutService
    {
        IDataAccess dataAccess;
        IExceptionHandling exceptionHandling;
        public CashoutService(IDataAccess access, IExceptionHandling exception)
        {
            dataAccess = access;
            exceptionHandling = exception;
        }

        /// <summary>
        /// End each service passed.  These have been cashed out and need to be ended
        /// </summary>
        /// <param name="request"></param>
        public void CashOutServices(CashoutServiceRequest request)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) {  Value = request.ServicePlanId },
                    new SqlParameter("@lngServiceSpanID", SqlDbType.Int) { Value = request.ServiceSpanId },
                    new SqlParameter("@lngCCOServiceSpanID", SqlDbType.Int) { Value = request.CcoServiceSpanId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = request.BeginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = request.EndDate },
                    new SqlParameter("@intCashDiscountPct", SqlDbType.Money) { Value = request.DiscountPercent },
                    new SqlParameter("@intMonthlyBudgetAmt", SqlDbType.Money) { Value = request.MonthlyBudgetAmount },
                    new SqlParameter("@intMonthlyCapCalculationAmt", SqlDbType.Money) { Value = request.MonthlyCapCalcAmount },
                    new SqlParameter("@intProgramServiceID", SqlDbType.Int) {  Value = request.ProgramServiceId },
                    new SqlParameter("@intServiceID", SqlDbType.Int) { Value = request.ServiceId },
                    new SqlParameter("@mneCashOutRate", SqlDbType.Money) { Value = request.CashOutRate },
                    new SqlParameter("@intUnits", SqlDbType.Int) { Value = request.Units },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = request.SessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISServiceSpanCashOutSave", CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Edit to insure that CCO Services do not overlap with existing CCO Services and if no overlap insert the CCO Cash Out budget
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string CreateCashOutCCO(CashoutCCORequest request)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = request.ServicePlanId },
                    new SqlParameter("@intCCOServiceSpanID", SqlDbType.Int) {  Value = request.CcoServiceSpanId },
                    new SqlParameter("@dteCCOServiceStartDate", SqlDbType.DateTime) { Value = request.BeginDate },
                    new SqlParameter("@dteCCOServiceEndDate", SqlDbType.DateTime) { Value = request.EndDate },
                    new SqlParameter("@intReturn", SqlDbType.Int) { Direction=ParameterDirection.InputOutput },
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISCCOOverlappingServicesEdit",
                    CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var intReturn = 0;
                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@intReturn")
                    {
                        intReturn = Convert.ToInt32(parameter.Value);
                    };
                }
                if (intReturn != -1)
                {
                    return intReturn + "|-1";
                }
                outputParameters = new List<SqlParameter>();
                parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramRequestID", SqlDbType.Int) {  Value = request.ProgramRequestId },
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Value = request.ServicePlanId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = request.BeginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = request.EndDate },
                    new SqlParameter("@intProviderID", SqlDbType.Int) { Value = request.ProviderId },
                    new SqlParameter("@mneRate", SqlDbType.Money) { Value = request.CashOutRate },
                    new SqlParameter("@mneMonthlyCapAmount", SqlDbType.Money) { Value = request.MonthlyBudgetAmount },
                    new SqlParameter("@intSessionID", SqlDbType.Int) {  Value = request.SessionId },
                    new SqlParameter("@intApprovedInd", SqlDbType.Int) { Value = request.ApprovedInd },
                    new SqlParameter("@intCCOServiceSpanID", SqlDbType.Int) { Direction=ParameterDirection.Output },
                    new SqlParameter("@intExceptionInd", SqlDbType.Int) { Value = request.ExceptionInd },
                    new SqlParameter("@intServiceID", SqlDbType.Int) { Value = request.CcoServiceId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISCCOCashOutServiceSave",
                    CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var idReturned = 0;
                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@intCCOServiceSpanID")
                    {
                        idReturned = Convert.ToInt32(parameter.Value);
                    }
                }
                return "0|" + idReturned;
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
        /// <param name="ccoServiceSpanId"></param>
        /// <returns></returns>
        public DataTable GetCCOServicesCashOut(int ccoServiceSpanId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngCCOServiceSpanID", SqlDbType.Int) { Value = ccoServiceSpanId },
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISCCOServicesCashOutGet",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve Transactions for a StateId
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable GetCCOTransactionsByStateID(string stateId, DateTime beginDate, DateTime endDate)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strStateID", SqlDbType.Int) { Value = stateId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = beginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = endDate },
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISCCOTransactionsGetByStateID",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Get Consumer Budget record set.
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataTable GetConsumerBudgets(int consumerId, int value)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intConsumerID", SqlDbType.Int) { Value = consumerId },
                    new SqlParameter("@intValue", SqlDbType.Int) { Value = value },
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISCCOBudgetsGet",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Get Consumer Budget record set.
        /// </summary>
        /// <param name="consumerId"></param>
        /// <returns></returns>
        public DataTable GetConsumerBudgetTotals(int consumerId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Value = consumerId }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISCCOConsumerBudgetTotal",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve MFP Budget Item data.
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        public DataTable GetMFPBudget(int budgetId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngBudgetID", SqlDbType.Int) { Value = budgetId }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMFPBudgetGet",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve MFP Budgets.
        /// </summary>
        /// <param name="consumerTotalBudgetId"></param>
        /// <returns></returns>
        public DataTable GetMFPBudgets(int consumerTotalBudgetId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngConsumerTotalBudgetID", SqlDbType.Int) { Value = consumerTotalBudgetId }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMFPBudgetsGet",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve MFP Budget data.
        /// </summary>
        /// <param name="servicePlanId"></param>
        /// <param name="mfpServiceId"></param>
        /// <param name="serviceSpanId"></param>
        /// <param name="beginDate"></param>
        /// <returns></returns>
        public DataTable GetMFPConsumerBudgetTotal(int servicePlanId, int mfpServiceId, int serviceSpanId, DateTime beginDate)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServicePlanID", SqlDbType.Int) { Value = servicePlanId },
                    new SqlParameter("@intMFPServiceID", SqlDbType.Int) { Value = mfpServiceId },
                    new SqlParameter("@lngServiceSpanID", SqlDbType.Int) { Value = serviceSpanId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = beginDate }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMFPConsumerBudgetTotalGet",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve Transactions for a supplied Budget Item (budget id)
        /// </summary>
        /// <param name="budgetId"></param>
        /// <returns></returns>
        public DataTable GetMFPTransactions(int budgetId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intConsumerId", SqlDbType.Int) { Value = 0 },
                    new SqlParameter("@dteFromDate", SqlDbType.DateTime) { Value = DBNull.Value },
                    new SqlParameter("@dteToDate", SqlDbType.DateTime) { Value = DBNull.Value },
                    new SqlParameter("@intBudgetID", SqlDbType.Int) { Value = budgetId }
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISMFPTransactionsGet",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Retrieve MFP Categories.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public DataTable GetNMCategories(int programId, int serviceId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramID", SqlDbType.Int) { Value = programId },
                    new SqlParameter("@intServiceID", SqlDbType.Int) { Value = serviceId },
                };
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISNMCategoriesGet",
                    CommandType.StoredProcedure, parameters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Saves the MFP Budget Item record
        /// </summary>
        /// <param name="budgetId"></param>
        /// <param name="stateId"></param>
        /// <param name="consumerTotalBudgetId"></param>
        /// <param name="categoryId"></param>
        /// <param name="budgetDescription"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="budgetAmount"></param>
        /// <param name="sessionId"></param>
        public void SaveMFPBudget(int budgetId, string stateId, int consumerTotalBudgetId, int categoryId,
            string budgetDescription, DateTime beginDate, DateTime endDate, decimal budgetAmount, long sessionId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intBudgetID", SqlDbType.Int) {  Value = budgetId },
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8) { Value = stateId },
                    new SqlParameter("@intConsumerTotalBudgetID", SqlDbType.Int) { Value = consumerTotalBudgetId },
                    new SqlParameter("@intCategoryID", SqlDbType.Int) { Value = categoryId },
                    new SqlParameter("@strBudgetDescription", SqlDbType.VarChar,250) { Value = budgetDescription },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = beginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = endDate },
                    new SqlParameter("@curBudgetAmount", SqlDbType.Money) { Value = budgetAmount },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMFPBudgetSave",
                    CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Saves the MFP Budget record..
        /// </summary>
        /// <param name="consumerTotalBudgetId"></param>
        /// <param name="servicePlanId"></param>
        /// <param name="serviceId"></param>
        /// <param name="stateId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="budgetAmount"></param>
        /// <param name="approvedInd"></param>
        /// <param name="mfpServiceSpan"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public int SaveMFPConsumerBudgetTotal(int consumerTotalBudgetId, int servicePlanId, int serviceId,
            string stateId, DateTime beginDate, DateTime endDate, decimal budgetAmount, int approvedInd, int mfpServiceSpan, long sessionId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intConsumerBudgetTotalID", SqlDbType.Int) {  Direction=ParameterDirection.InputOutput, Value = consumerTotalBudgetId },
                    new SqlParameter("@intServicePlanID", SqlDbType.Int) { Value = servicePlanId },
                    new SqlParameter("@intServiceID", SqlDbType.Int) { Value = serviceId },
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8) { Value = stateId },
                    new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = beginDate },
                    new SqlParameter("@dteEndDate", SqlDbType.DateTime) { Value = endDate },
                    new SqlParameter("@curTotalBudgetAmount", SqlDbType.Money) { Value = budgetAmount },
                    new SqlParameter("@intApprovedInd", SqlDbType.Int) { Value = approvedInd },
                    new SqlParameter("@intServiceSpanID", SqlDbType.Int) { Value = mfpServiceSpan },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISMFPConsumerBudgetTotalSave",
                    CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                var idReturned = 0;
                foreach (var parameter in outputParameters)
                {
                    if (parameter.ParameterName == "@intConsumerBudgetTotalID")
                    {
                        idReturned = Convert.ToInt32(parameter.Value);
                    }
                }

                return idReturned;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        /// <summary>
        /// Saves the Non-Medicaid Transaction
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="budgetId"></param>
        /// <param name="stateId"></param>
        /// <param name="paidDate"></param>
        /// <param name="paidAmount"></param>
        /// <param name="description"></param>
        /// <param name="sessionId"></param>
        public void SaveNMTransaction(int transactionId, int budgetId, string stateId, DateTime paidDate,
            decimal paidAmount, string description, long sessionId)
        {
            try
            {
                var outputParameters = new List<SqlParameter>();
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intTransactionID", SqlDbType.Int) {  Value = transactionId },
                    new SqlParameter("@intBudgetId", SqlDbType.Int) { Value = budgetId },
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8) { Value = stateId },
                    new SqlParameter("@dtePaidDate", SqlDbType.DateTime) { Value = paidDate },
                    new SqlParameter("@mnePaidAmount", SqlDbType.Money) { Value = paidAmount },
                    new SqlParameter("@strdescription", SqlDbType.VarChar,250) { Value = description },
                    new SqlParameter("@intSessionID", SqlDbType.Int) { Value = sessionId }
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISNMTransactionSave",
                    CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

    }

    public class CashoutServiceRequest
    {
        public int ServicePlanId { get; set; }
        public int ServiceSpanId { get; set; }
        public int CcoServiceSpanId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public double DiscountPercent { get; set; }
        public decimal MonthlyBudgetAmount { get; set; }
        public decimal MonthlyCapCalcAmount { get; set; }
        public int ProgramServiceId { get; set; }
        public int ServiceId { get; set; }
        public double CashOutRate { get; set; }
        public int Units { get; set; }
        public long SessionId { get; set; }
    }

    public class CashoutCCORequest
    {
        public int ProgramRequestId { get; set; }
        public int ServicePlanId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProviderId { get; set; }
        public decimal CashOutRate { get; set; }
        public decimal MonthlyBudgetAmount { get; set; }
        public long SessionId { get; set; }
        public int CcoServiceSpanId { get; set; }
        public int CcoServiceId { get; set; }
        public int ApprovedInd { get; set; }
        public int ExceptionInd { get; set; }
    }
    
}




