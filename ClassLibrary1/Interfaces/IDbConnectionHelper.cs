using System.Data;

namespace ClassLibrary1.Interfaces
{
    public interface IDbConnectionHelper
    {
        IDbConnection GetConnection(string connectionName);

        void CloseConnection(IDbConnection connection);

        IDbDataAdapter GetDbDataAdapter(IDbCommand command);
    }
}
