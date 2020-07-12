using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace ClassLibrary1
{
    public enum ExceptionPolicy
    {
        Database_Exception,
        Web_Exception,
        Audit_Log
    }

    public interface IEnterpriseLibrary
    {
        bool HandleException(Exception exceptionToHandle, string policyName);

        void WriteLog(LogEntry log);
    }

    public class EnterpriseLibraryHelper : IEnterpriseLibrary
    {
        public bool HandleException(Exception exceptionToHandle, string policyName)
        {
            return Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionPolicy.HandleException(exceptionToHandle, policyName);
        }

        public void WriteLog(LogEntry log)
        {
            Logger.Write(log);
        }
    }

    public class ExceptionHandling
    {
        private readonly IEnterpriseLibrary _enterpriseLibrary;

        private readonly HttpContextBase _httpContextWrapper;

        public ExceptionHandling(IEnterpriseLibrary enterpriseLibrary,
            HttpContextBase httpContextWrapper)
        {
            _enterpriseLibrary = enterpriseLibrary;
            _httpContextWrapper = httpContextWrapper;
        }

        public bool LogException(Exception ex, ExceptionPolicy exceptionPolicy)
        {
            bool rethrow = false;

            switch(exceptionPolicy)
            {
                case ExceptionPolicy.Database_Exception:
                    SqlException exception = (SqlException)ex;
                    rethrow = _enterpriseLibrary.HandleException(exception, nameof(ExceptionPolicy.Database_Exception));
                    break;

                case ExceptionPolicy.Web_Exception:
                    LogPageKeys(ex);
                    rethrow = _enterpriseLibrary.HandleException(ex, nameof(ExceptionPolicy.Web_Exception));
                    break;

                case ExceptionPolicy.Audit_Log:
                    break;
            }

            return rethrow; 
        }

        public void OutToDatabase(string loggedinuser, int eventID, string titleCategory, TraceEventType severity = TraceEventType.Error, IDictionary<string,object> extendedProperties = null)
        {
            LogEntry logEntry = new LogEntry();
            logEntry.Severity = severity;
            logEntry.EventId = eventID;
            logEntry.Priority = 1;
            logEntry.AppDomainName = "ISISWeb";
            logEntry.Title = titleCategory;
            logEntry.Message = loggedinuser;
            logEntry.Categories.Add("Audit_Trail");
            if(extendedProperties?.Count > 0)
                logEntry.ExtendedProperties = extendedProperties;

            _enterpriseLibrary.WriteLog(logEntry);
        }

        private void LogPageKeys(Exception ex)
        {
            var username = string.IsNullOrWhiteSpace(_httpContextWrapper.User?.Identity?.Name) 
                ? "Unauthenticated User" : HttpContext.Current.User?.Identity?.Name;

            var dictionaryValues = new Dictionary<string, object>();
            
            if((_httpContextWrapper.Request.QueryString?.Keys?.Count ?? 0) > 0)
            {
                foreach (string key in _httpContextWrapper.Request.QueryString.Keys)
                {
                    dictionaryValues.Add(key, _httpContextWrapper.Request.QueryString[key].ToString());
                }

                OutToDatabase(username, 900, "Querystring", TraceEventType.Information, dictionaryValues);
                dictionaryValues.Clear();
            }

            if ((_httpContextWrapper.Session?.Keys?.Count ?? 0) > 0)
            {
                foreach (string key in _httpContextWrapper.Session.Keys)
                {
                    dictionaryValues.Add(key, _httpContextWrapper.Session[key].ToString());
                }

                OutToDatabase(username, 902, "Session", TraceEventType.Information, dictionaryValues);
                dictionaryValues.Clear();
            }

            dictionaryValues.Add("1", FormatExceptionForLogging(ex));
            OutToDatabase(username, 901, "General Exception", TraceEventType.Information, dictionaryValues);
            dictionaryValues.Clear();
        }

        private string FormatExceptionForLogging(Exception ex)
        {
            var messageBuilder = new StringBuilder("Message: " + ex.Message);
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("Result: " + ex.HResult.ToString());
            if (!string.IsNullOrEmpty(ex.Source)) messageBuilder.AppendLine("Source: " + ex.Source);
            if (!string.IsNullOrWhiteSpace(ex.TargetSite?.Name)) messageBuilder.AppendLine("Method Name: " + ex.TargetSite.Name);
            if (!string.IsNullOrWhiteSpace(ex.StackTrace)) messageBuilder.AppendLine("Stack trace: " + ex.StackTrace);
            if(ex.Data.Count > 0)
            {
                var dataBuilder = new StringBuilder();
                foreach(DictionaryEntry entry in ex.Data)
                {
                    dataBuilder.AppendFormat("{0} : {1}\n", entry.Key, entry.Value);
                }

                messageBuilder.AppendLine("Data: " + dataBuilder.ToString());

            }

            return messageBuilder.ToString();
        }
    }
}
