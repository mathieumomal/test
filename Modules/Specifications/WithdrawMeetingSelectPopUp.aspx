<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WithdrawMeetingSelectPopUp.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.WithdrawMeetingSelectPopUp" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="stylesheet" type="text/css" href="module.css">
    <title>Withdraw specification</title>
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

        // Reload parent page, on release tab with correct version listed.
        function refreshParentPage(selectedTab, relId) {
            var parentLocation = getRadWindow().BrowserWindow.location.href;
            var finalAddress="";
            if (selectedTab !== null) {
                var str = parentLocation.split("&");
                for (var i = 0; i < str.length; ++i) {
                    if (str[i].indexOf("selectedTab") == -1 && (relId == null || str[i].indexOf("Rel") == -1)) {
                        finalAddress += str[i];
                    }
                }
                finalAddress += "&selectedTab=" + selectedTab;
                if (relId !== null)
                    finalAddress += "&Rel=" + relId;
            }
            else
                finalAddress = parentLocation;
            getRadWindow().BrowserWindow.location.href = finalAddress;
            getRadWindow().close();
        }
    </script>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" EnableHandlerDetection="false" />
        <telerik:RadAjaxManager ID="RadAjaxManager" runat="server" EnablePageHeadUpdate="false">
       </telerik:RadAjaxManager>
    <div class="specificationDetailsBody">
        <div class="contentModal" id="divWithdrawalMtgSelect" style="padding:5px;">
            <div class="header">
                <asp:Label runat="server" ID="lblWithdrawForRelease">You are about to withdraw specification for release <%= SelectedRelease %>. 
                    All versions of the specification concerning this release will be withdrawn. Continue? </asp:Label>
            </div>
            <br />
            <div class="center">
                <br />
                Withdrawal meeting <span style="color:red">*</span>: <ult:MeetingControl runat="server" ID="mcWithdrawal" DisplayLabel="false" />
                <br />
                <asp:Label ID="lblError" runat="server" CssClass="MeetingError" Visible="false"> Meeting is mandatory.</asp:Label>
            </div>
            <br />
            <div class="footer" style="position:absolute; bottom:10px; right:20px;">
                <asp:Button ID="btnConfirmWithdraw" Text ="Confirm" OnClick="btnConfirmWithdraw_Click" runat="server"/>
                <asp:Button id="btnCancel" runat="server" Text ="Cancel" OnClientClick="return window.close();" />
            </div>
        </div>       
    </div>
    </form>
</body>
</html>
