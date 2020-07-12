using ClassLibrary1.Constants;
using ClassLibrary1.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public class Login : ILogin
    {
        private readonly IDataAccess dataAccess;
        private readonly IExceptionHandling exceptionHandling;
        private readonly IConfiguration configurationManager;

        public Login(IDataAccess _dataAccess, 
            IExceptionHandling _exceptionHandling, 
            IConfiguration _configurationManager)
        {
            dataAccess = _dataAccess;
            exceptionHandling = _exceptionHandling;
            configurationManager = _configurationManager;
        }        

        public DataTable GetOptionsInfo()
        {
            try
            {
                return dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, 
                    "prc_ISISOptionsInfoGet", 
                    CommandType.StoredProcedure, 
                    null);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public string GetSplashText(int splashTextID)
        {
            var splashText = string.Empty;
            try
            {
                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@intSplashTextID", splashTextID);
                parameters[1] = new SqlParameter("@strSplashText", SqlDbType.VarChar, 150)
                {
                    Direction = ParameterDirection.Output
                };

                var outParameters = new List<SqlParameter>();

                var cmd = dataAccess.ExecuteNonQuery(
                    ApplicationConstants.IsisConnectionString,
                    "prc_ISISSplashTextGet",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                foreach(var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@strSplashText")
                        splashText = Convert.ToString(parameter.Value);
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return splashText;
        }

        public string LoginDetails(string userID, string password, string iP)
        {
            try
            {
                var outParameters = new List<SqlParameter>();

                var intMaxLoginAttempts = Convert.ToInt32(configurationManager.GetAppSetting("MaxLoginAttempts"));
                var intPwdExpiration = Convert.ToInt32(configurationManager.GetAppSetting("PasswordExpiration"));

                var parameters = new SqlParameter[5];
                parameters[0] = new SqlParameter("@strUserID", userID);
                parameters[1] = new SqlParameter("@strPassword", password);
                parameters[2] = new SqlParameter("@intPwdExpiration", intPwdExpiration);
                parameters[3] = new SqlParameter("@intResult", SqlDbType.Int, 100)
                {
                    Direction = ParameterDirection.Output
                };
                parameters[4] = new SqlParameter("@intDaysTillPasswordExpires", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString, 
                    "prc_ISISSecurityAuthenticate", 
                    CommandType.StoredProcedure, 
                    parameters, 
                    ref outParameters);

                var intResult = 0;
                var daysTillPasswordExpires = string.Empty;

                foreach(var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intResult")
                        intResult = Convert.ToInt32(parameter.Value);

                    else if (parameter.ParameterName == "@intDaysTillPasswordExpires")
                        daysTillPasswordExpires = Convert.ToString(parameter.Value);
                }

                switch(intResult)
                {
                    //Authentication passed
                    case 1:
                        parameters = new SqlParameter[3];
                        parameters[0] = new SqlParameter("@strUserID", userID);
                        parameters[0] = new SqlParameter("@strIP", iP);
                        parameters[0] = new SqlParameter("@intResult", SqlDbType.Int, 100)
                        {
                            Direction = ParameterDirection.Output
                        };
                        outParameters = new List<SqlParameter>();
                        var cmdSessionCreate = dataAccess.ExecuteNonQuery(
                            ApplicationConstants.IsisConnectionString,
                            "prc_ISISSessionCreate",
                            CommandType.StoredProcedure,
                            parameters,
                            ref outParameters);

                        foreach(var parameter in outParameters)
                        {
                            if (parameter.ParameterName == "@intResult")
                                intResult = Convert.ToInt32(parameter.Value);
                        }
                        break;

                    case -100:
                        parameters = new SqlParameter[3];
                        parameters[0] = new SqlParameter("@strUserID", userID);
                        parameters[0] = new SqlParameter("@intMaxTries", intMaxLoginAttempts);
                        parameters[0] = new SqlParameter("@intResult", SqlDbType.Int, 100)
                        {
                            Direction = ParameterDirection.Output
                        };
                        outParameters = new List<SqlParameter>();
                        var cmdIncrement = dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                            "prc_ISISSecurityCounterIncrement",
                            CommandType.StoredProcedure,
                            parameters,
                            ref outParameters);

                        foreach (var parameter in outParameters)
                        {
                            if (parameter.ParameterName == "@intResult")
                                intResult = Convert.ToInt32(parameter.Value);
                        }
                        break;
                }

                return intResult + "|" + daysTillPasswordExpires;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public void Logout(long sessionID)
        {
            try
            {
                var parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@lngSessionID", sessionID);
                dataAccess.Delete(ApplicationConstants.IsisConnectionString, "prc_ISISSecurityLogout", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public string GetErrorBlock(int errorBlockID)
        {
            var strErrorBlock = string.Empty;
            try
            {
                var parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@intErrBlock", errorBlockID);

                var dt = dataAccess.GetDataTable(ApplicationConstants.IsisConnectionString, "", CommandType.StoredProcedure, parameters);
                if(dt != null && dt.Rows.Count > 0)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        strErrorBlock += "Const ERRdr" + dr["err_Number"] + "=" + "\"" + $"{dr["err_description"]}" + Environment.NewLine;                    
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return strErrorBlock;
        }

        public string GetErrorString(int errorCodeID)
        {
            var strErrorBlock = string.Empty;
            try
            {
                var outParameters = new List<SqlParameter>();

                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@intError", errorCodeID);
                parameters[1] = new SqlParameter("@strErrorString", SqlDbType.VarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISErrorStringGet", 
                    CommandType.StoredProcedure, 
                    parameters, 
                    ref outParameters);

                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@strErrorString")
                        strErrorBlock = Convert.ToString(parameter.Value);
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return strErrorBlock;
        }

        public int SaveSessionVariable(long sessionID, string variableName, string variableValue)
        {
            var intReturn = 0;
            try
            {
                var outParameters = new List<SqlParameter>();

                var parameters = new SqlParameter[4];
                parameters[0] = new SqlParameter("@lngSessionID", sessionID);
                parameters[1] = new SqlParameter("@strVariableName", SqlDbType.VarChar, 255)
                {
                    Value = variableName
                };
                parameters[2] = new SqlParameter("@strVariableValue", SqlDbType.VarChar, 4000)
                {
                    Value = variableValue
                };
                parameters[3] = new SqlParameter("@intReturn", SqlDbType.Int, 4000)
                {
                    Direction = ParameterDirection.Output
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISSessionVariableSave",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intReturn")
                        intReturn = Convert.ToInt32(parameter.Value);
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return intReturn;
        }

        public string SessionVariable(long sessionID, string variableName)
        {
            var sessionVariable = string.Empty;
            try
            {
                var outParameters = new List<SqlParameter>();

                var timeout = configurationManager.GetAppSetting("Timeout");

                var parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@lngSessionID", sessionID);
                parameters[1] = new SqlParameter("@strVariableName", SqlDbType.VarBinary, 255)
                { 
                    Value = variableName
                };
                parameters[2] = new SqlParameter("@strVariableValue", SqlDbType.VarBinary, 4000) 
                { 
                    Direction = ParameterDirection.Output
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISSessionVariableGet",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@strVariableValue")
                        sessionVariable = Convert.ToString(parameter.Value);
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return sessionVariable;
        }

        public int SetPassword(string userID, string oldPassword, string newPassword, long sessionID)
        {
            try
            {
                var outParameters = new List<SqlParameter>();

                var intMaxLoginAttempts = configurationManager.GetAppSetting("MaxLoginAttempts");
                var intPwdExpiration = configurationManager.GetAppSetting("PasswordExpiration");

                var parameters = new SqlParameter[5];
                parameters[0] = new SqlParameter("@strUserID", userID);
                parameters[1] = new SqlParameter("@strPassword", oldPassword);
                parameters[2] = new SqlParameter("@intPwdExpiration", intPwdExpiration);
                parameters[3] = new SqlParameter("@intResult", SqlDbType.Int, 100)
                {
                    Direction = ParameterDirection.Output
                };
                parameters[4] = new SqlParameter("@intDaysTillPasswordExpires", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISSecurityAuthenticate",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                var intResult = 0;
                var daysTillPasswordExpires = string.Empty;

                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intResult")
                        intResult = Convert.ToInt32(parameter.Value);

                    else if (parameter.ParameterName == "@intDaysTillPasswordExpires")
                        daysTillPasswordExpires = Convert.ToString(parameter.Value);
                }

                if (intResult != 1)
                    return intResult;

                //Check new password versus history
                if (!CheckPasswordHistory(userID, newPassword))
                    return -106;

                //Check new password versus word dictionary
                if (!CheckPasswordDictionary(newPassword))
                    return -107;

                //Set new password
                outParameters = new List<SqlParameter>();
                parameters = new SqlParameter[4];
                parameters[0] = new SqlParameter("@strUserID", userID);
                parameters[1] = new SqlParameter("@strNewPassword", newPassword);
                parameters[2] = new SqlParameter("@intSessionID", sessionID);
                parameters[3] = new SqlParameter("@intResult", SqlDbType.Int, 100)
                {
                    Direction = ParameterDirection.Output
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISPasswordSet",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intResult")
                        intResult = Convert.ToInt32(parameter.Value);
                }

                if (intResult == 1)
                    return 1;
                else
                    return -100;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }
        }

        public int GetSecurity(long sessionID, string iP, string webPageID)
        {
            var intHash = 0;
            try
            {
                var outParameters = new List<SqlParameter>();

                var timeout = configurationManager.GetAppSetting("Timeout");

                var parameters = new SqlParameter[5];
                parameters[0] = new SqlParameter("@lngSessionID", sessionID);
                parameters[1] = new SqlParameter("@strIP", iP);
                parameters[2] = new SqlParameter("@intWebPageID", webPageID);
                parameters[3] = new SqlParameter("@intTimeout", timeout);
                parameters[4] = new SqlParameter("@intHash", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                parameters[4] = new SqlParameter("@intDaysTillPasswordExpires", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISSecurityGet",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intHash")
                        intHash = Convert.ToInt32(parameter.Value);
                }
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return intHash;
        }

        #region "Private methods"
        private bool CheckPasswordHistory(string userID, string password)
        {
            bool isPasswordExits;
            try
            {
                var outParameters = new List<SqlParameter>();

                var passwordHistory = Convert.ToInt32(configurationManager.GetAppSetting("PasswordHistory"));

                var parameters = new SqlParameter[5];
                parameters[0] = new SqlParameter("@strUserID", userID);
                parameters[1] = new SqlParameter("@strPassword", password);
                parameters[2] = new SqlParameter("@strPasswordHistory", passwordHistory);
                parameters[1] = new SqlParameter("@intResult", SqlDbType.Int, 100)
                {
                    Direction = ParameterDirection.Output
                };

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISPasswordHistoryGet",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);

                var intResult = 0;
                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intResult")
                        intResult = Convert.ToInt32(parameter.Value);
                }

                isPasswordExits = intResult == 0;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return isPasswordExits;
        }

        private bool CheckPasswordDictionary(string password)
        {
            bool isPasswordExits;
            try
            {
                var outParameters = new List<SqlParameter>();

                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@strPassword", password);
                parameters[1] = new SqlParameter("@intResult", SqlDbType.Int,100)
                {
                    Direction = ParameterDirection.Output
                };               

                dataAccess.ExecuteNonQuery(ApplicationConstants.IsisConnectionString,
                    "prc_ISISPasswordDictionaryCheck",
                    CommandType.StoredProcedure,
                    parameters,
                    ref outParameters);
                var intResult = 0;
                foreach (var parameter in outParameters)
                {
                    if (parameter.ParameterName == "@intResult")
                        intResult = Convert.ToInt32(parameter.Value);
                }

                isPasswordExits = intResult == 1;
            }
            catch (Exception ex)
            {
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw;
            }

            return isPasswordExits;
        }
        #endregion
    }
}
