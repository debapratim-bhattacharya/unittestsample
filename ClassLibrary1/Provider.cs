using ClassLibrary1.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public interface IProvider
    {
        DataTable GetProviderService(int provNumber, int somevariable, int programId, int serviceId);

        DataTable GetProviderServiceRates(int provNum, int serviceId, int programId);
    }


    public class Provider : IProvider
    {
        private readonly IDataAccess dataAccess;
        private readonly IExceptionHandling exceptionHandling;

        public Provider(IDataAccess _dataAccess, IExceptionHandling _exceptionHandling)
        {
            dataAccess = _dataAccess;
            exceptionHandling = _exceptionHandling;
        }

        public DataTable GetProviderService(int provNumber, int somevariable, int programId, int serviceId)
        {
            throw new NotImplementedException();
        }

        public DataTable GetProviderServiceRates(int provNum, int serviceId, int programId)
        {
            throw new NotImplementedException();
        }
    }
}
