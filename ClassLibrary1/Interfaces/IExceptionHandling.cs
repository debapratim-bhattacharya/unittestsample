using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClassLibrary1.Interfaces
{
    public interface IExceptionHandling
    {
        bool LogException(Exception ex, ExceptionPolicy exceptionPolicy);

        void OutToDatabase(string loggedinuser, int eventID, string titleCategory, TraceEventType severity = TraceEventType.Error, IDictionary<string, object> extendedProperties = null);
    }
}
