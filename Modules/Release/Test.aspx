<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="Etsi.Ultimate.Module.Release.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>TEST PAGE</title>
    <link rel="stylesheet" type="text/css" href="/Portals/_default/Skins/3GPP/mainpage.css"/>
</head>
<body>

    <asp:Panel ID="pnlTestPageMessage" runat="server" Visible="false">
        <asp:Label runat="server" ID="lblTestPage"></asp:Label>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlTestPageBody">
        <form id="form1" runat="server">
            <div>
                TEST PAGE...
            </div>
        </form>
    </asp:Panel>

</body>
</html>
