<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VersionRemarksPopUp.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.VersionRemarksPopUp" %>

<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="stylesheet" type="text/css" href="module.css">
    <title>Versions - Remarks</title>
</head>
<body>
    <script type="text/javascript">
        function getRadWindow() {
            var oWindow = null;
            if (window.radWindow)
                oWindow = window.radWindow;
            else if (window.frameElement.radWindow)
                oWindow = window.frameElement.radWindow;
            return oWindow;
        }
    </script>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadAjaxManager ID="RadAjaxManager" runat="server" EnablePageHeadUpdate="false">
        </telerik:RadAjaxManager>
        <div class="specificationDetailsBody">
            <ult:remarkscontrol runat="server" id="rcVersion" />
        </div>
    </form>
</body>
</html>
