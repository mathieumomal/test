<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkItemDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.WorkItem.WorkItemDetails" Culture="auto" meta:resourcekey="WorkItemDetailsResource" UICulture="auto" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="JS/jquery.min.js"></script>
    <script type="text/javascript">
        function closeAllModals() {
            var manager = GetRadWindowManager();
            manager.closeAll();
        }
    </script>
</head>
<body class="wiDetailBody">
    <form id="wiDetailsForm" runat="server">
        <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="650px">
            <asp:Panel ID="wiWarning" runat="server" CssClass="wiDetailsWarning" Visible="false">
                <span class="wiDetailsWarningTxt">No data available for the current query.</span>
            </asp:Panel>
            <asp:Panel ID="wiDetailsBody" runat="server" CssClass="wiDetailsBody">
                <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
                <telerik:RadTabStrip ID="wiDetailRadTabStrip" runat="server" MultiPageID="WiDetailRadMultiPage"
                    AutoPostBack="false">
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="wiDetailRadMultiPage" runat="server" Width="100%" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
                    <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true">
                        <table style="width: 100%">
                            <tr>
                                <td class="TabLineLeft">Name:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblName" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Acronym:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblAcronym" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">WI Level:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblWiLevel" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Type:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblType" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Type:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblStatus" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Release:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblRelease" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Start date:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblStartDate" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">End date:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblEndDate" runat="server"></asp:Label></td>
                            </tr>
                            <tr style="max-height: 150px; overflow-y: scroll; margin-top: 10px;">
                                <td colspan="2" class="wiRemarks">
                                    <ult:remarkscontrol runat="server" id="wiRemarks" />
                                </td>
                            </tr>
                        </table>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageRelated" runat="server" Height="90%">
                        <table style="width: 100%">
                            <tr>
                                <td class="TabLineLeft">Parent Work Item:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblParentWorkItem" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Child Work Items:</td>
                                <td class="TabLineRight"></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Responsible group(s):</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblResponsibleGroups" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Rapporteur(s):</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblRapporteur" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">Latest WID version:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblWiVersion" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">TSG Approval meeting:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblTsgMtg" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">PCG Approval meeting:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblPcgMtg" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">TSG Stopped meeting:</td>
                                <td class="TabLineRight">
                                    <asp:Label ID="lblTsgStpMtg" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="2">See Specifications specifically resulting from this Work Item</td>
                            </tr>
                            <tr>
                                <td colspan="2">See all related Change Requests related to this Work Item (all specifications)</td>
                            </tr>
                        </table>
                    </telerik:RadPageView>
                </telerik:RadMultiPage>
                <div class="wiDetailsAction">
                    <asp:LinkButton ID="ExitBtn" runat="server" Text="Exit" CssClass="LinkButton" OnClick="CloseWorkItemDetails_Click" />
                </div>
                <script type="text/javascript">
                    $(document).ready(function () {
                        setTimeout(function () {
                            var strTitle = "WI #" + 0//$("#releaseCodeVal").html();
                            document.title = strTitle;
                        }, 200);
                    });
                </script>
            </asp:Panel>
            <telerik:RadAjaxManager ID="RadAjaxManager" runat="server" EnablePageHeadUpdate="false">
            </telerik:RadAjaxManager>
        </asp:Panel>
    </form>
</body>
</html>
