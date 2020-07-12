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
    /// Crated Date: 06/17/2020
    /// Modified Date: 
    /// Class Name: Code Tables.
    /// </summary>
    /// <remarks>
    /// Longer comments can be associated with a type or member through
    /// the remarks tag.
    /// </remarks>   

    public class CodeTables
        {
            IDataAccess dataAccess;
            IExceptionHandling exceptionHandling;
            public CodeTables(IDataAccess access, IExceptionHandling exception)
            {
                dataAccess = access;
                exceptionHandling = exception;
            }


            /// <summary>
            /// Get Values from Code Tables.
            /// </summary>
            /// <param name="typeName"></param>
            /// <param name="effectiveDate"></param>
            /// <returns></returns>
            public DataTable GetCodeTableValues(string typeName, DateTime effectiveDate)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strTypeName", SqlDbType.VarChar, 30) { Value = typeName },
                    new SqlParameter("@dteEffectiveDate", SqlDbType.DateTime) { Value = effectiveDate },
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "code.prc_GetCode_by_CodeTypeName",
                        CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }

            }

            /// <summary>
            /// Retrieve end code.
            /// </summary>
            /// <param name="endCode"></param>
            /// <returns></returns>
            /// 
            public DataTable GetEndCode(string endCode)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strEndCode", SqlDbType.VarChar, 3) { Value = endCode },
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISEndCodeGet",
                        CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve end code.
            /// </summary>
            /// <param name="endCodeId"></param>
            /// <returns></returns>
            /// 
            public DataTable GetEndCodebyID(int endCodeId)
            {
                try
                {
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngEndCodeID", SqlDbType.Int) { Value = endCodeId },
                };
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISEndCodeByIDGet",
                        CommandType.StoredProcedure, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve end codes.
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            /// 
            public DataTable GetEndCodes()
            {
                try
                {
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISEndCodesGet", CommandType.StoredProcedure, null);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Retrieve end codes.
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            /// 
            public DataTable GetHabServicesEndCodes()
            {
                try
                {
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISHabServicesEndCodesGet",
                        CommandType.StoredProcedure, null);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Get a listing of the Settings.
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            /// 
            public DataTable GetSettings()
            {
                try
                {
                    return dataAccess.GetDataTable(ApplicationConstants.IsisConnectinString, "prc_ISISSettingsGet", CommandType.StoredProcedure, null);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Saves an End Code Record
            /// </summary>
            /// <param name="endCode"></param>
            /// <param name="endCodeId"></param>
            /// <param name="description"></param>
            /// <param name="longDescription"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public string SaveEndCode(string endCode, long endCodeId, string description, string longDescription, long sessionId)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@strEndCode", SqlDbType.VarChar,3) { Value = endCode },
                    new SqlParameter("@lngEndCodeID", SqlDbType.Int) { Direction=ParameterDirection.InputOutput, Value = endCodeId },
                    new SqlParameter("@strDescription", SqlDbType.VarChar,200) { Value = description },
                    new SqlParameter("@strDescriptionLong", SqlDbType.VarChar,2480) { Value = longDescription },
                    new SqlParameter("@lngSessionID", SqlDbType.Int) { Value = sessionId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISEndCodeSave",
                        CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var endCodeIdValue = 0;
                    foreach (var parameter in outputParameters)
                    {
                        switch (parameter.ParameterName)
                        {
                            case "@lngEndCodeID":
                                endCodeIdValue = Convert.ToInt32(parameter.Value);
                                break;
                        }
                    }
                    return "0|" + endCodeIdValue;
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Saves Last Extract Date
            /// </summary>
            /// <param name="lastExtractDate"></param>
            /// <param name="sessionId"></param>
            public void SaveLastExtractDate(DateTime lastExtractDate, long sessionId)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@dteLastExtractDate", SqlDbType.DateTime) { Value = lastExtractDate },
                    new SqlParameter("@lngSessionID", SqlDbType.Int) { Value = sessionId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISLastExtractDateSave",
                        CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Save Level of Care Program..
            /// </summary>
            /// <param name="levelCareProgramId"></param>
            /// <param name="levelCareId"></param>
            /// <param name="programId"></param>
            /// <param name="monthlyCap"></param>
            /// <param name="yearlyCap"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public string SaveLevelofCareProgram(int levelCareProgramId, int levelCareId, int programId, decimal monthlyCap, decimal yearlyCap, long sessionId)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@intLevelCareProgramID", SqlDbType.Int) { Direction=ParameterDirection.InputOutput, Value = levelCareProgramId },
                    new SqlParameter("@intLevelCareID", SqlDbType.Int) { Value = levelCareId },
                    new SqlParameter("@intProgramID", SqlDbType.Int) { Value = programId },
                    new SqlParameter("@curMonthlyCap", SqlDbType.Money) { Value = monthlyCap },
                    new SqlParameter("@curYearlyCap", SqlDbType.Money) { Value = yearlyCap },
                    new SqlParameter("@lngSessionID", SqlDbType.Int) { Value = sessionId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISLevelofCareProgramSave",
                        CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var levelCareProgramIdValue = 0;
                    foreach (var parameter in outputParameters)
                    {
                        switch (parameter.ParameterName)
                        {
                            case "@intLevelCareProgramID":
                                levelCareProgramIdValue = Convert.ToInt32(parameter.Value);
                                break;
                        }
                    }
                    return "0|" + levelCareProgramIdValue;
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Save PACE counties.
            /// </summary>
            /// <param name="data"></param>
            public void SavePACECounties(string data)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();

                    if (string.IsNullOrWhiteSpace(data)) return;


                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISPACECountiesDelete",
                        CommandType.StoredProcedure, null, ref outputParameters);

                    var dataSplit = data.Split('|');
                    foreach (var item in dataSplit)
                    {
                        var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@strCounty", SqlDbType.Int) { Value = item }
                    };
                        dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISPACECountiesSave",
                            CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    }
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            public string SaveProgram(SaveCodeTableProgramRequest request)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramID", SqlDbType.Int) { Direction=ParameterDirection.InputOutput, Value = request.ProgramId },
                    new SqlParameter("@strProgramName", SqlDbType.VarChar,50) { Value = request.ProgramName },
                    new SqlParameter("@strProgramType", SqlDbType.Char) { Value = request.ProgramType },
                    new SqlParameter("@intMonthlyUnitCap", SqlDbType.Int) { Value = request.MonthlyUnitCap },
                    new SqlParameter("@mneMonthlyCap", SqlDbType.Money) { Value = request.MonthlyCap },
                    new SqlParameter("@intYearlyUnitCap", SqlDbType.Int) { Value = request.YearlyUnitCap },
                    new SqlParameter("@mneYearlyCap", SqlDbType.Money) { Value = request.YearlyCap },
                    new SqlParameter("@strProgramDisplayName", SqlDbType.VarChar,20) { Value = request.ProgramDisplayName },
                    new SqlParameter("@strStatePlanEnhancedInd", SqlDbType.Char) { Value = request.StatePlanEnhancedInd },
                    new SqlParameter("@strProgramManager", SqlDbType.VarChar,50) { Value = request.ProgramManager },
                    new SqlParameter("@lngSessionID", SqlDbType.Int) { Value = request.SessionId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISProgramSave",
                        CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var programId = 0;
                    foreach (var parameter in outputParameters)
                    {
                        switch (parameter.ParameterName)
                        {
                            case "@lngProgramID":
                                programId = Convert.ToInt32(parameter.Value);
                                break;
                        }
                    }
                    return "0|" + programId;
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// SaveProgramsServices
            /// </summary>
            /// <param name="request"></param>
            public void SaveProgramsServices(SaveProgramServiceRequest request)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngProgramsServiceID", SqlDbType.Int) { Value = request.ProgramsServiceId },
                    new SqlParameter("@lngProgramID", SqlDbType.Int) { Value = request.ProgramId },
                    new SqlParameter("@lngServiceID", SqlDbType.Int) { Value = request.ServiceId },
                    new SqlParameter("@curRate", SqlDbType.Money) { Value = request.CurRate },
                    new SqlParameter("@strCapType", SqlDbType.Money) { Value = request.CapType },
                    new SqlParameter("@strAgeEdit", SqlDbType.VarChar,8) { Value = request.AgeEdit },
                    new SqlParameter("@strCashOut", SqlDbType.Char) { Value = request.CashOut },
                    new SqlParameter("@curCashOutAmount", SqlDbType.Money) { Value = request.CurCashOutAmount },
                    new SqlParameter("@curAverageRate", SqlDbType.Money) { Value = request.CurAverageRate },
                    new SqlParameter("@intDiscountPercent", SqlDbType.Int) { Value = request.DiscountPercent },
                    new SqlParameter("@strRateAutoFill", SqlDbType.Char) { Value = request.RateAutoFill },
                    new SqlParameter("@strPAMeasureType", SqlDbType.Char) { Value = request.PaMeasureType },
                    new SqlParameter("@intPAMeasureValue", SqlDbType.Int) { Value = request.PaMeasureValue },
                    new SqlParameter("@strPAMeasureActive", SqlDbType.Char) { Value = request.PaMeasureActive },
                    new SqlParameter("@dtePAMeasureBegDate", SqlDbType.DateTime) { Value = request.PaMeasureBegDate },
                    new SqlParameter("@dtePAMeasureEndDate", SqlDbType.DateTime) { Value = request.PaMeasureEndDate },
                    new SqlParameter("@lngSessionID", SqlDbType.Int) { Value = request.SessionId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISPrograms_ServicesSave",
                        CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// Saves a Service record
            /// </summary>
            /// <param name="serviceId"></param>
            /// <param name="wCode"></param>
            /// <param name="modifier"></param>
            /// <param name="serviceName"></param>
            /// <param name="serviceDescription"></param>
            /// <param name="unitTypeId"></param>
            /// <param name="unitCount"></param>
            /// <param name="strTier"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public string SaveService(int serviceId, string wCode, string modifier, string serviceName,
                string serviceDescription, int unitTypeId, int unitCount, string strTier, long sessionId)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngServiceID", SqlDbType.Int) { Direction=ParameterDirection.InputOutput, Value = serviceId },
                    new SqlParameter("@strWCode", SqlDbType.VarChar,5) { Value = wCode },
                    new SqlParameter("@strModifier", SqlDbType.VarChar,2) { Value = modifier },
                    new SqlParameter("@strServiceName", SqlDbType.VarChar,250) { Value = serviceName },
                    new SqlParameter("@strServiceDescription", SqlDbType.VarChar,250) { Value = serviceDescription },
                    new SqlParameter("@intUnitTypeID", SqlDbType.Int) { Value = unitTypeId },
                    new SqlParameter("@intUnitCount", SqlDbType.Int) { Value = unitCount },
                    new SqlParameter("@lngSessionID", SqlDbType.Int) { Value = sessionId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISServiceSave",
                        CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                    var serviceIdValue = 0;
                    foreach (var parameter in outputParameters)
                    {
                        switch (parameter.ParameterName)
                        {
                            case "@lngServiceID":
                                serviceIdValue = Convert.ToInt32(parameter.Value);
                                break;
                        }
                    }
                    return "0|" + serviceIdValue;
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="settingId"></param>
            /// <param name="name"></param>
            /// <param name="value"></param>
            /// <param name="comment"></param>
            /// <param name="delete"></param>
            /// <param name="sessionId"></param>
            /// <returns></returns>
            public void SaveSettings(int settingId, string name, string value, string comment, string delete, long sessionId)
            {
                try
                {
                    var outputParameters = new List<SqlParameter>();
                    var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@lngSettingID", SqlDbType.Int) { Value = settingId },
                    new SqlParameter("@strName", SqlDbType.VarChar,128) { Value = name },
                    new SqlParameter("@strValue", SqlDbType.VarChar,256) { Value = value },
                    new SqlParameter("@strComment", SqlDbType.VarChar,256) { Value = comment },
                    new SqlParameter("@strDelete", SqlDbType.VarChar,3) { Value = delete },
                    new SqlParameter("@lngSessionID", SqlDbType.Int) { Value = sessionId }
                };
                    dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectinString, "prc_ISISSettingsSave",
                        CommandType.StoredProcedure, parameters.ToArray(), ref outputParameters);
                }
                catch (Exception ex)
                {
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw;
                }
            }

        }
    
}




