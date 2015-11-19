<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemoveSpecReleasePopUp.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.PopUps.RemoveSpecReleasePopUp" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2013.2.717.40, 
Culture=neutral, PublicKeyToken=121fae78165ba3d4" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="/Portals/_default/Skins/3GPP/mainpage.css"/>
    <link rel="stylesheet" type="text/css" href="../module.css">
    <link rel="SHORTCUT ICON" href="../images/favicon.ico" type="image/x-icon">
    <title>Remove Specification-Release</title>
    <script src="../JS/jquery.min.js"></script>
    <telerik:RadCodeBlock ID="RadCodeBlockVersion" runat="server">
        <script src="../JS/Popup.js?v=<%=ConfigurationManager.AppSettings["AppVersion"] %>"></script>
    </telerik:RadCodeBlock>
</head>
<body>
    <form id="frmRemoveSpecRelease" class="popup" runat="server">
        <asp:Panel ID="pnlMessage" runat="server" Visible="false">
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
        </asp:Panel>
        <asp:Panel runat="server" id="formContent">
            <div class="contentModal">
                <div class="center InfoTxt">
                    Are you sure you want to delete this Specification-Release ? 
                </div>
                <div class="footer" runat="server">
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn3GPP-success" OnClick="btnDelete_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn3GPP-success" OnClientClick="return closeRadWindow()" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
