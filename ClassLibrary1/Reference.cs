using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public class Reference
    {
        IDataAccess dataAccess;
        IExceptionHandling exceptionHandling;

        public Reference(IDataAccess access, IExceptionHandling exception)
        {
            dataAccess = access;
            exceptionHandling = exception;
        }

        public DataTable GetAidTypes(int programID = 0)
        {
            try
            {
                var paramters = new List<SqlParameter>();

                if (programID > 0)
                    paramters.Add(new SqlParameter("@intProgramID", SqlDbType.Int, 4) { Value = programID });
                else
                    paramters.Add(new SqlParameter("@intProgramID", SqlDbType.Int, 4) { Value = DBNull.Value });


                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                   "prc_ISISAidTypesGet",
                   CommandType.StoredProcedure,
                   paramters.ToArray());
            }
            catch(Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetCertificationTypes()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                   "prc_ISISCertificationTypesGet",
                   CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetCounties(int workerID = 0)
        {
            try
            {
                var paramters = new List<SqlParameter>();

                if (workerID > 0)
                    paramters.Add(new SqlParameter("@intWorkerID", SqlDbType.Int, 4) { Value = workerID });
                else
                    paramters.Add(new SqlParameter("@intWorkerID", SqlDbType.Int, 4) { Value = DBNull.Value });


                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                   "prc_ISISCountiesGet",
                   CommandType.StoredProcedure,
                   paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetCountiesAndStates(int workerID = 0)
        {
            try
            {
                var paramters = new List<SqlParameter>();

                if(workerID > 0)
                    paramters.Add(new SqlParameter("@intWorkerID", SqlDbType.Int, 4) { Value = workerID });
                else
                    paramters.Add(new SqlParameter("@intWorkerID", SqlDbType.Int, 4) { Value = DBNull.Value });


                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                   "prc_ISISCountiesAndStatesGet",
                   CommandType.StoredProcedure,
                   paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public string GetCounty(int organizationID)
        {
            var orgName = string.Empty;
            try
            {
                var outParameters = new List<SqlParameter>();

                var paramters = new List<SqlParameter>();
                paramters.Add(new SqlParameter("@lngOrganizationID", SqlDbType.Int, 4) { Value = organizationID });
                paramters.Add(new SqlParameter("@strOrgName", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output });

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                   "prc_ISISCountyGet",
                   CommandType.StoredProcedure,
                   paramters.ToArray(),
                   ref outParameters);

                foreach(var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@strOrgName")
                        orgName = Convert.ToString(parameter.Value);

                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return orgName;
        }

        public DataTable GetDenialReasons(int programID)
        {
            try
            {
                var paramters = new List<SqlParameter>();
                paramters.Add(new SqlParameter("@intProgramID", SqlDbType.Int, 4) { Value = programID });

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                   "prc_ISISDenialReasonsGet",
                   CommandType.StoredProcedure,
                   paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetDiagCodes(int programID, string birthDate = "01/01/1900", string primaryDisability = "", int axisType = 0)
        {
            try
            {
                var paramters = new List<SqlParameter>();
                
                paramters.Add(new SqlParameter("@intProgramID", SqlDbType.Int, 4) { Value = programID });
                paramters.Add(new SqlParameter("@dtebirthDay", SqlDbType.DateTime) { Value = birthDate });
                paramters.Add(new SqlParameter("@strprimarydiagnosis", SqlDbType.Char,3) { Value = primaryDisability });
                paramters.Add(new SqlParameter("@intAxisType", SqlDbType.Int, 4) { Value = axisType });

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISDiagCodesGet",
                    CommandType.StoredProcedure,
                    paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetFacilitiesPrograms()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISProgramsGetFacilities",
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetLevelCares(int programRequestID)
        {
            try
            {
                var paramters = new List<SqlParameter>();
                paramters.Add(new SqlParameter("@intProgramRequestID", SqlDbType.Int, 4) { Value = programRequestID });

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISLevelCareGet",
                    CommandType.StoredProcedure,
                    paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetLevelOfCares()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISLevelCaresGet",
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetPACECounties()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISPACECountiesGet",
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetProcedureCodes(int programRequestID = 0, int programID = 0, 
            string beginDate = "01/01/1900", string birthDate = "01/01/1900")
        {
            try
            {
                var paramters = new List<SqlParameter>();
                paramters.Add(new SqlParameter("@lngProgramRequestID", SqlDbType.Int, 4) { Value = programRequestID });
                paramters.Add(new SqlParameter("@lngProgramID", SqlDbType.Int, 4) { Value = programID });
                paramters.Add(new SqlParameter("@dteBeginDate", SqlDbType.DateTime) { Value = beginDate });
                paramters.Add(new SqlParameter("@dteBirthDate", SqlDbType.DateTime) { Value = birthDate });

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISProcedureCodesGet",
                    CommandType.StoredProcedure,
                    paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetProgram(long programID)
        {
            try
            {
                var paramters = new List<SqlParameter>();                
                paramters.Add(new SqlParameter("@intProgramID", SqlDbType.Int, 4) { Value = programID });

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISProgramGet",
                    CommandType.StoredProcedure,
                    paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetPrograms()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISProgramsGet",
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetProgramsService(int serviceID)
        {
            try
            {
                var paramters = new List<SqlParameter>();
                paramters.Add(new SqlParameter("@intServiceID", SqlDbType.Int, 4) { Value = serviceID });

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISProgramsServiceGet",
                    CommandType.StoredProcedure,
                    paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetService(int serviceID, int programID = 0)
        {
            try
            {
                var paramters = new List<SqlParameter>();
                paramters.Add(new SqlParameter("@intServiceID", SqlDbType.Int, 4) { Value = serviceID });
                paramters.Add(new SqlParameter("@intProgramID", SqlDbType.Int, 4) { Value = programID });

                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISServiceGet",
                    CommandType.StoredProcedure,
                    paramters.ToArray());
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetServices()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISServicesGet",
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetStates()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISStateGet",
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public DataTable GetUnitTypes()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString,
                    "prc_ISISUnitTypesGet",
                    CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }
    }
}
