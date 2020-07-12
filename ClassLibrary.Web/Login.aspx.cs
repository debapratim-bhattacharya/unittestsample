using ClassLibrary1.Interfaces;
using System;

namespace ClassLibrary.Web
{
    public partial class Login : System.Web.UI.Page
    {
        public ILogin login;

        public IConfiguration configurationManagerHelper;

        public Login(ILogin _login, IConfiguration configurationManager)
        {
            login = _login;
            configurationManagerHelper = configurationManager;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //removed this as dependency injection is working now
            //var eHlper = new EnterpriseLibraryHelper();
            //var exceptionHandler = new ExceptionHandling(eHlper, new HttpContextWrapper(HttpContext.Current));
            //var dbConnectionHelper = new DbConnectionHelper(exceptionHandler, new ConfigurationHelper());

            //var dataAccess = new DataAccess(exceptionHandler, dbConnectionHelper);
            //login = new Iowa.Dhs.Isis.Business.Login(dataAccess, exceptionHandler, new ConfigurationHelper());

            //This method cannot be unit tested as IsPostback cannot be mocked
            if (!IsPostBack)
            {
                lblHeaderText.Text = login.GetSplashText(3) + login.GetSplashText(4);
            }
        }

        protected void ChangePasswordPushButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                lblMessage.Text = "Please enter username";
                tdChangePwd.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentPassword.Text))
            {
                lblMessage.Text = "Please enter current password";
                tdChangePwd.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword.Text))
            {
                lblMessage.Text = "Please enter new password";
                tdChangePwd.Visible = true;
                return;
            }
            var changePassword = login.SetPassword(txtUserName.Text, CurrentPassword.Text, NewPassword.Text, 0);
            if (changePassword == 1)
            {
                lblChangePwdError.Text = "Password Changed Successfully.";
                tdChangePwd.Visible = true;
            }
            else
            {
                lblChangePwdError.Text = login.GetErrorString(changePassword);
                tdChangePwd.Visible = true;
                //lblMessage.Text = "Error occured while changing the password.";
            }
            pnlCPW.Style.Remove("display");
            pnlLogin.Style["display"] = "none";
            txtUserName.Text = string.Empty; CurrentPassword.Text = string.Empty; NewPassword.Text = string.Empty;
        }

        protected void ClearButton_Click(object sender, EventArgs e)
        {
            txtLoginName.Text = "";
            txtLoginPassword.Text = "";
            pnlCPW.Style["display"] = "none";
            tdError.Visible = false;
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string ipAddress = string.Empty;
            tdError.Visible = false;
            if (string.IsNullOrWhiteSpace(txtLoginName.Text))
            {
                lblMessage.Text = "Please enter username";
                tdError.Visible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLoginPassword.Text))
            {
                lblMessage.Text = "Please enter password";
                tdError.Visible = true;
                return;
            }

            if (txtLoginName.Text.Length >= 20)
            {
                lblMessage.Text = "Username cannot exceed 20 characters";
            }

            var loginDetails = login.LoginDetails(txtLoginName.Text, txtLoginPassword.Text, ipAddress);
            if (loginDetails != null && !string.IsNullOrWhiteSpace(loginDetails))
            {
                var validateUser = Convert.ToInt32(loginDetails.Split('|')[0]);
                var daysTillPasswordExpires = Convert.ToInt32(loginDetails.Split('|')[1]);
                if (validateUser > 0)
                {
                    //This section of the method cannot be unit tested as we cannot mock Session and Response.Redirect
                    Session["ISISSession"] = validateUser;
                    if (daysTillPasswordExpires > 0 && daysTillPasswordExpires < 8)
                    {
                        login.SaveSessionVariable(validateUser, "PasswordAboutToExpire", loginDetails.Split('|')[1]);
                        lblMessage.Text = "password is getting expired in the next 8 days";
                        tdError.Visible = true;
                    }
                    hdnUnSuccessCount.Value = "0";
                    Response.Redirect("~/Consumer/ConsumerRoles.aspx", false);
                }
                else
                {
                    //This section has an issue hdnUnSuccessCount.Value will through exception first time
                    hdnUnSuccessCount.Value = string.IsNullOrEmpty(hdnUnSuccessCount.Value) ? "0" : hdnUnSuccessCount.Value;
                    hdnUnSuccessCount.Value = $"{Convert.ToInt32(hdnUnSuccessCount.Value) + 1}";
                    lblMessage.Text = login.GetErrorString(validateUser);
                    tdError.Visible = true;
                }
                if (Convert.ToInt32(hdnUnSuccessCount.Value) > Convert.ToInt32(configurationManagerHelper.GetAppSetting("MaxLoginAttempts")))
                {
                    lblMessage.Text = "You have exceeded maximum number of attempts.";
                    tdError.Visible = true;
                }
            }
        }

        protected void ChangePasswordClear_Click(object sender, EventArgs e)
        {

            pnlCPW.Style.Remove("display");
            pnlLogin.Style["display"] = "none";
            txtUserName.Text = string.Empty; CurrentPassword.Text = string.Empty; NewPassword.Text = string.Empty;
            tdChangePwd.Visible = false;
        }
    }
}