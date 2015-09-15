<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VersionPopup.aspx.cs" Inherits="Etsi.Ultimate.Module.Versions.VersionPopup" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Version details</title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <link rel="stylesheet" type="text/css" href="/Portals/_default/Skins/3GPP/mainpage.css"/>
    <script src="JS/jquery.min.js"></script>
    <script src="JS/jquery-validate.min.js"></script>
    <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
    <script src="JS/VersionPopup.js"></script>
</head>
<body>
    <form id="frmVersionPopup" runat="server">
        <telerik:RadScriptManager runat="server" ID="rsmVersionPopup" EnableHandlerDetection="false" EnablePartialRendering="False"/>
        <telerik:RadAjaxManager ID="ramVersionPopup" runat="server" EnablePageHeadUpdate="false" UpdatePanelsRenderMode="Inline">
        </telerik:RadAjaxManager>

        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="messageBox error">
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="divVersionPopup" runat="server" >
            <div class="center">
                <table>
                    <tr>
                        <td colspan="2" class="versionIdentifier">
                            <asp:Label runat="server" ID="SpecAndTitle"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>Version <span class="mandatoryIndicator" title="my mandatory field">*</span>:</td>
                        <td>
                            <!-- EDIT MODE -->
                            <asp:Panel runat="server" ID="versionInEditMode">
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionMajorVal" Width="40px" MinValue="0" ToolTip="Major"></telerik:RadNumericTextBox>
                                <span>.</span>
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionTechnicalVal" Width="40px"  MinValue="0" ToolTip="Technical"></telerik:RadNumericTextBox>
                                <span>.</span>
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionEditorialVal" Width="40px"  MinValue="0" ToolTip="Editorial"></telerik:RadNumericTextBox>
                            </asp:Panel>
                            <!-- VIEW MODE -->
                            <asp:Panel runat="server" ID="versionInViewMode">
                                <asp:HyperLink runat="server" ID="lkVersion"></asp:HyperLink>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>Suppress from SDO transposition:</td>
                        <td>
                            <asp:CheckBox runat="server" ID="chckboxSdo"/>
                        </td>
                    </tr>
                    <tr>
                        <td>Suppressed from missing:</td>
                        <td>
                            <asp:CheckBox runat="server" ID="chckboxMissing"/>
                        </td>
                    </tr>
                    <tr>
                        <td>Meeting <span class="mandatoryIndicator" title="my mandatory field">*</span>:</td>
                        <td>
                            <!-- EDIT MODE -->
                            <ult:meetingcontrol runat="server" ID="meetingCtrl" class="test" />
                            <!-- VIEW MODE -->
                            <asp:Label runat="server" ID="lblMeeting"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="upContentRemarks" runat="server">
                    <ContentTemplate>
                        <ult:RemarksControl runat="server" id="remarksCtrl" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="footer" id="versionFooter">
                <!-- EDIT MODE -->
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn3GPP-success" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn3GPP-success" OnClick="btnCancel_Click" />
                <!-- VIEW MODE -->
                <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn3GPP-success" OnClick="btnEdit_Click" />
                <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="btn3GPP-success" OnClientClick="return closeRadWindow()" />
            </div>
        </asp:Panel>
    </form>
</body>
</html>
