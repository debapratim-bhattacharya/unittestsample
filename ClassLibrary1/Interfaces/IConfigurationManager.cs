using System.Configuration;

namespace ClassLibrary1.Interfaces
{
    public interface IConfiguration
    {
        string GetAppSetting(string key);

        string GetConnectionString(string connectionName);
    }

    public class Configuration : IConfiguration
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public string GetConnectionString(string connectionName)
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }
    }
}
