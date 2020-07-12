using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ClassLibrary1.Interfaces
{
    public interface IDataAccess
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        DataTable GetDataTable(
            string connectionName, string commandText, CommandType commandType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataTable GetDataTable(
            string connectionName, string commandText, CommandType commandType, SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        DataSet GetDataSet(string connectionName, string commandText, CommandType commandType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        DataSet GetDataSet(string connectionName, string commandText, CommandType commandType, SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        IDataReader GetDataReader(string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="outParameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery(
            string connectionName, string commandText, CommandType commandType, 
            SqlParameter[] parameters, ref List<SqlParameter> outParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        void Delete(string connectionName, string commandText, CommandType commandType, SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        void Insert(string connectionName, string commandText, CommandType commandType, SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <param name="lastId"></param>
        /// <returns></returns>
        long Insert(string connectionName, string commandText, 
            CommandType commandType, SqlParameter[] parameters, out long lastId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        void InsertWithTransaction(
            string connectionName, string commandText,
            CommandType commandType, SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int Update(string connectionName, string commandText, CommandType commandType, SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        void UpdateWithTransaction(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object GetScalarValue(
            string connectionName,
            string commandText,
            CommandType commandType,
            SqlParameter[] parameters);
    }
}
