using System.Data;

namespace ClassLibrary1.Interfaces
{
    public interface ILogin
    {
        DataTable GetOptionsInfo();

        string GetSplashText(int splashTextID);

        string LoginDetails(string userID, string password, string iP);

        void Logout(long sessionID);

        string GetErrorBlock(int errorBlockID);

        string GetErrorString(int errorCodeID);

        int SetPassword(string userID, string oldPassword, string newPassword, long sessionID);

        string SessionVariable(long sessionID, string variableName);

        int SaveSessionVariable(long sessionID, string variableName, string variableValue);

        int GetSecurity(long sessionID, string iP, string webPageID);
    }
}
