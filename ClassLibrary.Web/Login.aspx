<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ClassLibrary.Web.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="pnlCPW" runat="server" />

            <asp:Panel ID="pnlLogin" runat="server" />

            <asp:Label ID="lblHeaderText" runat="server" />

            <asp:Label ID="lblMessage" runat="server" />

            <asp:Label ID="lblChangePwdError" runat="server" />

            <asp:Label ID="Label2" runat="server" />

            <asp:TextBox ID="txtUserName" runat="server" />

            <asp:TextBox ID="txtLoginName" runat="server" />

            <asp:TextBox ID="txtLoginPassword" runat="server" />

            <asp:TextBox ID="CurrentPassword" runat="server" />

            <asp:TextBox ID="NewPassword" runat="server" />

            <asp:Table ID="tdError" runat="server" />

            <asp:Table ID="tdChangePwd" runat="server" />

            <asp:HiddenField ID="hdnUnSuccessCount" runat="server" />
        </div>
    </form>
</body>
</html>
