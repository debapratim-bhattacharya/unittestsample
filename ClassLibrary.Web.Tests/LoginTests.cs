using ClassLibrary1.Interfaces;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using System;
using System.Web.UI.WebControls;

namespace ClassLibrary.Web.Tests
{
    public class LoginMock : Login
    {
        public LoginMock(ILogin login, IConfiguration configuration) : base(login,configuration)
        {
            pnlCPW = new Panel();
            pnlLogin = new Panel();
            txtUserName = new System.Web.UI.WebControls.TextBox();
            tdChangePwd = new System.Web.UI.WebControls.Table();
            tdError = new System.Web.UI.WebControls.Table();
            CurrentPassword = new System.Web.UI.WebControls.TextBox();
            NewPassword = new System.Web.UI.WebControls.TextBox();
            lblChangePwdError = new System.Web.UI.WebControls.Label();
            lblHeaderText = new System.Web.UI.WebControls.Label();
            lblMessage = new System.Web.UI.WebControls.Label();
        }

        public void Arrange(
            ref Panel panelCPW,
            ref Panel panelLogin,
            ref TextBox textUserName,
            ref TextBox textCurrentPassword,
            ref TextBox textNewPassword,
            ref TextBox textLoginName,
            ref TextBox textLoginPassword,
            ref Label labelChangePwdError,
            ref Label labelHeaderText,
            ref Label labelMessage,
            ref Table tableChangePwd,
            ref Table tableError,
            ref HiddenField hiddenUnsuccessCount
            )
        {
            pnlCPW = panelCPW;
            pnlLogin = panelLogin;
            tdError = tableError;
            tdChangePwd = tableChangePwd;
            txtLoginName = textLoginName;
            txtLoginPassword = textLoginPassword;
            txtUserName = textUserName;
            CurrentPassword = textCurrentPassword;
            NewPassword = textNewPassword;
            lblChangePwdError = labelChangePwdError;
            lblHeaderText = labelHeaderText;
            lblMessage = labelMessage;
            hdnUnSuccessCount = hiddenUnsuccessCount;
        }

        public void PageLoad(object sender, EventArgs e)
        {   
            base.Page_Load(sender, e);
        }

        public void ChangePasswordPushButtonClick(object sender, EventArgs e)
        {
            base.ChangePasswordPushButton_Click(sender, e);
        }

        public void ClearButtonClick(object sender, EventArgs e)
        {
            base.ClearButton_Click(sender, e);
        }

        public void LoginButtonClick(object sender, EventArgs e)
        {
            base.LoginButton_Click(sender, e);
        }

        public void ChangePasswordClearClick(object sender, EventArgs e)
        {
            base.ChangePasswordClear_Click(sender, e);           
        }
    }

    [TestFixture(Category = "Unit")]
    public class LoginTests
    {
        private Mock<ILogin> mockLogin;

        private Mock<IConfiguration> mockConfiguration;

        private LoginMock loginMocker;

        Panel pnlCPW = new Panel();
        Panel pnlLogin = new Panel();
        
        Table tdChangePwd = new Table();
        Table tdError = new Table();
        TextBox txtUserName = new TextBox();
        TextBox txtLoginPassword = new TextBox();
        TextBox txtLoginName = new TextBox();
        TextBox currentPassword = new TextBox();
        TextBox newPassword = new TextBox();
        Label lblChangePwdError = new Label();
        Label lblHeaderText = new Label();
        Label lblMessage = new Label();
        HiddenField hdnUnSuccessCount = new HiddenField();

        [SetUp]
        public void SetUp()
        {
            mockLogin = new Mock<ILogin>();

            mockConfiguration = new Mock<IConfiguration>();

            loginMocker = new LoginMock(mockLogin.Object, mockConfiguration.Object);

            ArrangeControls();
        }

        public void ArrangeControls()
        {
            loginMocker.Arrange(
                ref pnlCPW,
                ref pnlLogin,
                ref txtUserName,
                ref currentPassword,
                ref newPassword,
                ref txtLoginName,
                ref txtLoginPassword,
                ref lblChangePwdError,
                ref lblHeaderText,
                ref lblMessage,
                ref tdChangePwd,
                ref tdError,
                ref hdnUnSuccessCount);
        }

        [Test]
        public void ChangePasswordClearClick_WhenExecuted_ShouldSetControlPropertiesAsExpected()
        {
            //Act
            loginMocker.ChangePasswordClearClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("none", pnlLogin.Style["display"]);
            Assert.IsEmpty(txtUserName.Text);
            Assert.IsEmpty(currentPassword.Text);
            Assert.IsEmpty(newPassword.Text);
            Assert.IsFalse(tdChangePwd.Visible);
        }

        [Test]
        public void ClearButtonClick_WhenExecuted_ShouldSetControlPropertiesAsExpected()
        {
            //Act
            loginMocker.ClearButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("none", pnlCPW.Style["display"]);
            Assert.IsEmpty(txtLoginName.Text);
            Assert.IsEmpty(txtLoginPassword.Text);
            Assert.IsFalse(tdError.Visible);
        }

        [Test]
        public void ChangePasswordPushButtonClick_WhenUserNameEmpty_ShouldSetProperLblMessage()
        {
            //Act
            loginMocker.ChangePasswordPushButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("Please enter username", lblMessage.Text);
            Assert.IsTrue(tdChangePwd.Visible);
        }

        [Test]
        public void ChangePasswordPushButtonClick_WhenCurrentPasswordEmpty_ShouldSetProperLblMessage()
        {
            txtUserName.Text = "someUsername";
            currentPassword.Text = "currentPassword";

            //Act
            loginMocker.ChangePasswordPushButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("Please enter new password", lblMessage.Text);
            Assert.IsTrue(tdChangePwd.Visible);
        }

        [Test]
        public void ChangePasswordPushButtonClick_WhenChangePasswordIsSuccess_ShouldSetProperLblMessage()
        {
            txtUserName.Text = "someUsername";
            currentPassword.Text = "currentPassword";
            newPassword.Text = "currentPassword";

            mockLogin
                .Setup(x => x.SetPassword(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    0))
                .Returns(1);

            //Act
            loginMocker.ChangePasswordPushButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("Password Changed Successfully.", lblChangePwdError.Text);
            Assert.IsTrue(tdChangePwd.Visible);
        }

        [Test]
        public void ChangePasswordPushButtonClick_WhenChangePasswordIsFail_ShouldSetProperLblMessage()
        {
            txtUserName.Text = "someUsername";
            currentPassword.Text = "currentPassword";
            newPassword.Text = "currentPassword";

            int setPasswordResponse = 0;
            string expectedMessage = "someMessage";

            mockLogin
                .Setup(x => x.SetPassword(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    0))
                .Returns(setPasswordResponse);

            mockLogin
                .Setup(x => x.GetErrorString(setPasswordResponse))
                .Returns(expectedMessage);

            //Act
            loginMocker.ChangePasswordPushButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual(expectedMessage, lblChangePwdError.Text);
            Assert.IsTrue(tdChangePwd.Visible);
        }

        [Test]
        public void LoginButtonClick_WhenLoginNameEmpty_ShouldSetProperLblMessage()
        {
            //Act
            loginMocker.LoginButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("Please enter username", lblMessage.Text);
            Assert.IsTrue(tdError.Visible);
        }

        [Test]
        public void LoginButtonClick_WhenLoginPasswordEmpty_ShouldSetProperLblMessage()
        {
            txtLoginName.Text = "someUsername";
            
            //Act
            loginMocker.LoginButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("Please enter password", lblMessage.Text);
            Assert.IsTrue(tdError.Visible);
        }

        [Test]
        public void LoginButtonClick_WhenUsernameMoreThan20Chars_ShouldSetProperLblMessage()
        {
            txtLoginName.Text = "someUsername+SomeMoreInformation";
            txtLoginPassword.Text = "somePassword";

            //Act
            loginMocker.LoginButtonClick(new object(), new EventArgs());

            //Assert
            Assert.AreEqual("Username cannot exceed 20 characters", lblMessage.Text);
            Assert.IsFalse(tdError.Visible);
        }

        [Test]
        public void LoginButtonClick_WhenLoginResponseIsEmpty_ShouldNotExecuteAnyOtherLogic()
        {
            txtLoginName.Text = "someUsername";
            txtLoginPassword.Text = "somePassword";

            mockLogin
                .Setup(x => x.LoginDetails(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(string.Empty);

            //Act
            loginMocker.LoginButtonClick(new object(), new EventArgs());

            //Assert            
            Assert.IsFalse(tdError.Visible);
        }

        [TestCase(1,"Expected Message")]
        [TestCase(3, "You have exceeded maximum number of attempts.")]
        public void LoginButtonClick_WhenLoginResponseValidateUserIsFalse_ShouldSetLoginErrorString(int unsuccessCount, string expectedMessage)
        {
            txtLoginName.Text = "someUsername";
            txtLoginPassword.Text = "somePassword";
            hdnUnSuccessCount.Value = unsuccessCount.ToString();

            mockLogin
                .Setup(x => x.LoginDetails(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns("0|1");

            mockLogin
                .Setup(x => x.GetErrorString(
                    It.IsAny<int>()))
                .Returns(expectedMessage);

            mockConfiguration
                .Setup(x => x.GetAppSetting("MaxLoginAttempts"))
                .Returns("2");

            //Act
            loginMocker.LoginButtonClick(new object(), new EventArgs());

            //Assert            
            Assert.IsTrue(tdError.Visible);

            Assert.AreEqual(expectedMessage, lblMessage.Text);
        }
    }
}
