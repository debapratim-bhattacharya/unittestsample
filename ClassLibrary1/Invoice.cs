using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using ClassLibrary1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public class Invoice : IInvoice
    {
        IDataAccess dataAccess;
        IExceptionHandling exceptionHandling;

        public Invoice(IDataAccess access, IExceptionHandling exception)
        {
            dataAccess = access;
            exceptionHandling = exception;
        }

        public string DeleteInvoice(int invoiceId, long sessionId)
        {
            try
            {
                var outParameters = new List<SqlParameter>();

                var parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@lngInvoiceID", invoiceId);
                parameters[1] = new SqlParameter("@lngSessionID", sessionId);                
                parameters[2] = new SqlParameter("@intReturn", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
               

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoiceDelete",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                var intReturn = string.Empty;
                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intReturn")
                        intReturn = Convert.ToString(parameter.Value);
                }
                return intReturn;
            }
            catch(Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetInvoiceByInvoiceId(int invoiceId)
        {
            try
            {
                var outParameters = new List<SqlParameter>();


                var parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@lngInvoiceID", invoiceId);
                
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoiceByInvoiceIDGet",
                    CommandType.StoredProcedure,
                    parameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetInvoiceByServicePlanId(int servicePlanId, string fiscalYearInd)
        {
            try
            {
                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@lngServicePlanID", servicePlanId);
                parameters[1] = new SqlParameter("@strFiscalYearInd", fiscalYearInd);

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoiceByServicePlanIDGet",
                    CommandType.StoredProcedure,
                    parameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetInvoiceByLineItems(int invoiceId, int iSisServicePlanId, DateTime fiscalYearBeginDate)
        {
            try
            {
                var parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@lngInvoiceID", invoiceId);
                parameters[1] = new SqlParameter("@lngISISServicePlanID", iSisServicePlanId);
                parameters[2] = new SqlParameter("@dteFiscalYearBeginDate", SqlDbType.Date) { Value = fiscalYearBeginDate };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoiceLineItemsGet",
                    CommandType.StoredProcedure,
                    parameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetInvoiceTotalCapAmountForMonth(int invoiceId, int servicePlanId, DateTime dteCurrentMonth)
        {
            try
            {
                var parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@lngInvoiceID", invoiceId);
                parameters[1] = new SqlParameter("@lngServicePlanID", servicePlanId);
                parameters[2] = new SqlParameter("@dteCurrentMonth", SqlDbType.Date) { Value = dteCurrentMonth };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoicesCapAmountForMonth",
                    CommandType.StoredProcedure,
                    parameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetServiceSpansAlreadyonInvoice(int serviceSpanId, DateTime billDate)
        {
            try
            {
                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@lngServiceSpanID", serviceSpanId);
                parameters[1] = new SqlParameter("@dteBillDate", SqlDbType.Date) { Value = billDate };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_ServiceSpansAlreadyOnInvoiceGet",
                    CommandType.StoredProcedure,
                    parameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetServiceSpansforInvoice(int serviceSpanId, DateTime fiscalYearBeginDate)
        {
            try
            {
                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@lngServiceSpanID", serviceSpanId);
                parameters[1] = new SqlParameter("@dteFiscalYearBeginDate", SqlDbType.Date) { Value = fiscalYearBeginDate };

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_ServiceSpansforInvoiceGet",
                    CommandType.StoredProcedure,
                    parameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public string SaveInvoice(InvoiceRequest invoiceRequest)
        {
            string resultMessage;
            try
            {
                var outParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@lngISISServiceSpanID", SqlDbType.Int,4){Value = invoiceRequest.IsisServicePlanId},
                    new SqlParameter("@intVoidInd", SqlDbType.Int,4){Value = invoiceRequest.VoidInd},
                    new SqlParameter("@dteInvoiceDate", SqlDbType.Date){Value = invoiceRequest.InvoiceDate},
                    new SqlParameter("@dteScheduleDate", SqlDbType.Date){Value = invoiceRequest.ScheduleDate},
                    new SqlParameter("@strStateID", SqlDbType.VarChar,8){Value = invoiceRequest.StateId},
                    new SqlParameter("@strSSN", SqlDbType.VarChar,9){Value = invoiceRequest.SsN},
                    new SqlParameter("@strMemberCustomerID", SqlDbType.VarChar,9){Value = invoiceRequest.MemberCustomerId},
                    new SqlParameter("@strFiscalYear", SqlDbType.VarChar,9){Value = invoiceRequest.FiscalYear},
                    new SqlParameter("@intSessionID", SqlDbType.Int){Value = invoiceRequest.SessionId},
                    new SqlParameter("@lngInvoiceID", SqlDbType.Int,20){Value = invoiceRequest.InvoiceId, Direction = ParameterDirection.InputOutput},
                    new SqlParameter("@intReturn", SqlDbType.Int,10){Direction = ParameterDirection.Output},
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoiceSave",
                    CommandType.StoredProcedure,
                    parameters.ToArray(),
                    ref outParameters);

                var intReturn = 0;
                var invoiceId = 0;
                foreach (var parameter in outParameters)
                {
                    switch(parameter.ParameterName)
                    {
                        case "@intReturn":
                            intReturn = Convert.ToInt32(parameter.Value);
                            break;
                        case "@lngInvoiceID":
                            invoiceId = Convert.ToInt32(parameter.Value);
                            break;
                    }
                }

                resultMessage = intReturn != 0 ? intReturn + "|-1" : intReturn + "|" + invoiceId;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
            return resultMessage;
        }

        public string SaveInvoiceLineItems(InvoiceLineItemRequest invoiceLineItemRequest)
        {
            string resultMessage;
            try
            {
                var outParameters = new List<SqlParameter>();

                var parameters = new List<SqlParameter>()
                {
                    new SqlParameter("@lngInvoiceID", SqlDbType.Int,4){Value = invoiceLineItemRequest.InvoiceId},
                    new SqlParameter("@lngISISServiceSpanID", SqlDbType.Int,4){Value = invoiceLineItemRequest.IsisServicePlanId},
                    new SqlParameter("@lngProviderID", SqlDbType.Int,4){Value = invoiceLineItemRequest.ProviderId},
                    new SqlParameter("@dteServiceSpanBeginDate", SqlDbType.Date){Value = invoiceLineItemRequest.ServiceSpanBeginDate},
                    new SqlParameter("@dteServiceSpanEndDate", SqlDbType.Date){Value = invoiceLineItemRequest.ServiceSpanEndDate},
                    new SqlParameter("@strWcode", SqlDbType.VarChar,5){Value = invoiceLineItemRequest.Wcode},
                    new SqlParameter("@strModifier", SqlDbType.VarChar,2){Value = invoiceLineItemRequest.Modifier},
                    new SqlParameter("@lngUnits", SqlDbType.Int){Value = invoiceLineItemRequest.Units},
                    new SqlParameter("@lngUnitCost", SqlDbType.Money){Value = invoiceLineItemRequest.UnitCost},
                    new SqlParameter("@lngTotalCost", SqlDbType.Money){Value = invoiceLineItemRequest.TotalCost},
                    new SqlParameter("@lngCPAmount", SqlDbType.Money){Value = invoiceLineItemRequest.CpAmount},
                    new SqlParameter("@lngFeees", SqlDbType.Money){Value = invoiceLineItemRequest.Fees},
                    new SqlParameter("@lngCredits", SqlDbType.Money){Value = invoiceLineItemRequest.Credits},
                    new SqlParameter("@lngNetCost", SqlDbType.Money){Value = invoiceLineItemRequest.NetCost},
                    new SqlParameter("@intException", SqlDbType.Int){Value = invoiceLineItemRequest.Exception},
                    new SqlParameter("@lngDeleteInvoiceLineItemInd", SqlDbType.Int){Value = invoiceLineItemRequest.DeleteInvoiceLineItemInd},
                    new SqlParameter("@intSessionID", SqlDbType.Int){Value = invoiceLineItemRequest.SessionId},
                    new SqlParameter("@intReturn", SqlDbType.Int,10){Direction = ParameterDirection.Output},
                };
                
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoiceLineItemsSave",
                    CommandType.StoredProcedure,
                    parameters.ToArray(),
                    ref outParameters);

                var intReturn = 0;
                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intReturn")
                        intReturn = Convert.ToInt32(parameter.Value);
                }

                resultMessage = intReturn == -1 ? intReturn + "|-1" : intReturn + "|0";
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
            return resultMessage;
        }

        public string VoidInvoice(int invoiceId, long sessionId)
        {
            try
            {
                var outParameters = new List<SqlParameter>();

                var parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@lngInvoiceID", invoiceId);
                parameters[1] = new SqlParameter("@lngSessionID", sessionId);
                parameters[2] = new SqlParameter("@intReturn", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };


                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "ihhrc.prc_InvoiceVoid",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                var intReturn = string.Empty;
                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intReturn")
                        intReturn = Convert.ToString(parameter.Value);
                }
                return intReturn;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
    }
}
