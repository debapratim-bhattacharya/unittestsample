using ClassLibrary1.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public class DataAccess : IDataAccess
    {
        #region "Constructor"
        private readonly IExceptionHandling exceptionHandling;
        private readonly IDbConnectionHelper dbConnectionHelper;
        protected IDbTransaction transaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">Exception handling class to log exceptions</param>
        /// <param name="connectionHelper">Connection helper to abstract connection provider</param>
        public DataAccess(IExceptionHandling exception, IDbConnectionHelper connectionHelper)
        {
            exceptionHandling = exception;
            dbConnectionHelper = connectionHelper;
        }

        #endregion

        #region Transaction Members

        /// <summary>
        /// Begins a transaction
        /// Starts/Fetch a transaction from the SQL server
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IDbConnection connection)
        {
            transaction = connection.BeginTransaction();
            return transaction;
        }

        /// <summary>
        /// Commits a transaction to the SQL server
        /// </summary>
        public void CommitTransaction()
        {
            if(transaction != null)
            {
                //Commit the transaction
                transaction.Commit();

                //Close/Dispose the connection
                transaction.Dispose();
            }
        }

        /// <summary>
        /// Rollback a transaction to the SQL server
        /// </summary>
        public void RollbackTransaction()
        {
            if(transaction != null)
            {
                //Rollback the transaction
                transaction.Commit();

                //Close/Dispose the connection
                transaction.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string connectionName, string commandText, CommandType commandType)
        {
            return GetDataTable(connectionName, commandText, commandType, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string connectionName, string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        var dataSet = new DataSet();
                        var adapter = dbConnectionHelper.GetDbDataAdapter(command);
                        adapter.Fill(dataSet);

                        return dataSet.Tables[0];
                    }
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(GetDataTable)}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string connectionName, string commandText, CommandType commandType)
        {
            return GetDataSet(connectionName, commandText, commandType, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string connectionName, string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        var dataSet = new DataSet();
                        var adapter = dbConnectionHelper.GetDbDataAdapter(command);
                        adapter.Fill(dataSet);

                        return dataSet;
                    }
                }
                catch(Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception("Error in GetDataSet()");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IDataReader GetDataReader(
            string connectionName, 
            string commandText, 
            CommandType commandType, 
            SqlParameter[] parameters)
        {
            IDataReader reader = null;
            var connection = dbConnectionHelper.GetConnection(connectionName);
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = commandText;
                command.CommandType = commandType;
                if(parameters != null)
                {
                    foreach(var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                reader = command.ExecuteReader();
            }
            catch(Exception ex)
            {
                dbConnectionHelper.CloseConnection(connection);
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw new Exception($"Error in {nameof(GetDataReader)}");
            }
            return reader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public void Delete(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters)
        {

            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(Delete)}");
                }                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public void Insert(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters)
        {

            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(Insert)}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="lastId"></param>
        /// <returns></returns>
        public long Insert(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters,
            out long lastId)
        {
            lastId = 0;
            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = commandType;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    
                    //Not sure why we are returning the same value in return as well as out parameter
                    object newId = command.ExecuteScalar();
                    lastId = Convert.ToInt64(newId);
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(Insert)}");
                }
                return lastId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public void InsertWithTransaction(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters)
        {

            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    BeginTransaction(connection);

                    using(var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        try
                        {
                            command.ExecuteNonQuery();
                            CommitTransaction();
                        }
                        catch(Exception)
                        {
                            RollbackTransaction();
                        }
                        finally
                        {
                            dbConnectionHelper.CloseConnection(connection);
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(InsertWithTransaction)}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Update(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters)
        {
            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        return command.ExecuteNonQuery();
                    }                        
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(Update)}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        public void UpdateWithTransaction(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters)
        {

            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    BeginTransaction(connection);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        try
                        {
                            command.ExecuteNonQuery();
                            CommitTransaction();
                        }
                        catch (Exception)
                        {
                            RollbackTransaction();
                        }
                        finally
                        {
                            dbConnectionHelper.CloseConnection(connection);
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(UpdateWithTransaction)}");
                }
            }
        }

        /// <summary>
        /// Executes a stored procedure that returns a single value
        /// </summary>
        /// <param name="connectionName">Connection string key</param>
        /// <param name="commandText">Command text - sql query or stored procedure name</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameters">Any number of sqlparameters to be passed as arguments to database</param>
        /// <returns></returns>
        public object GetScalarValue(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters)
        {
            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        return command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(GetScalarValue)}");
                }
            }
        }

        /// <summary>
        /// Executes a stored procedure that returns a single value
        /// </summary>
        /// <param name="connectionName">Connection string key</param>
        /// <param name="commandText">Command text - sql query or stored procedure name</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameters">Any number of sqlparameters to be passed as arguments to database</param>
        /// <param name="outParameters">Returns parameters with direction set to output</param>
        /// <returns></returns>
        public int ExecuteNonQuery(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters,
            ref List<SqlParameter> outParameters)
        {
            using (var connection = dbConnectionHelper.GetConnection(connectionName))
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.CommandType = commandType;
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }
                        int result = command.ExecuteNonQuery();

                        outParameters = new List<SqlParameter>();
                        foreach(var parameter in parameters)
                        {
                            switch(parameter.Direction)
                            {
                                case ParameterDirection.Output:
                                case ParameterDirection.ReturnValue:
                                    {
                                        outParameters.Add(
                                            new SqlParameter(parameter.ParameterName,
                                                ((SqlParameter)command.Parameters[parameter.ParameterName]).Value));
                                        break;
                                    }                                
                                default:
                                    break;
                            }
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    dbConnectionHelper.CloseConnection(connection);
                    exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                    throw new Exception($"Error in {nameof(ExecuteNonQuery)}: {ex.Message}");
                }
            }
        }
    }
}
