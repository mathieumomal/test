﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpecificationDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationDetails" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Etsi.Ultimate.Module.Specifications" Namespace="Etsi.Ultimate.Module.Specifications" TagPrefix="specification" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="RapporteurControl " Src="../../controls/Ultimate/RapporteurControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="RelatedWiControl" Src="../../controls/Ultimate/RelatedWiControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="CommunityHyperlinkControl" Src="../../controls/Ultimate/CommunityHyperlinkControl.ascx" %>
<%@ Register TagPrefix="spec" TagName="SpecificationListControl" Src="SpecificationListControl.ascx" %>
<%@ Register TagPrefix="spec" TagName="SpecificationReleaseControl" Src="SpecificationReleaseControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon"/>
    <script src="JS/jquery.min.js"></script>
    <telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
        <link rel="stylesheet" type="text/css" href="/Portals/_default/Skins/3GPP/mainpage.css?v=<%=ConfigurationManager.AppSettings["AppVersion"] %>"/>
        <link rel="stylesheet" type="text/css" href="module.css?v=<%= ConfigurationManager.AppSettings["AppVersion"] %>"/>
        <script src="JS/CommonScript.js?v=<%=ConfigurationManager.AppSettings["AppVersion"] %>"></script>
    </telerik:RadCodeBlock>
</head>
<body class="specDetailsBody">
    <form id="specificationDetailsForm" class="specDetailform" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" EnableHandlerDetection="false" />
        <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="750px">
            <asp:Panel ID="specMsg" runat="server" Visible="false">
                <asp:Label runat="server" ID="specMsgTxt"></asp:Label>
            </asp:Panel>
            
            <asp:Panel ID="specificationMessages" runat="server" Visible="false">
                <asp:Label runat="server" ID="specificationMessagesTxt"></asp:Label>
            </asp:Panel>

            <!-- Logo header -->
            <div class="logoHeader">
                <asp:HyperLink runat="server" ID="logoHeaderHpk" NavigateUrl="<%$ AppSettings:PortalUrl %>" Target="_blank" ImageUrl="/Portals/0/BANNER06.jpg" />
            </div>

            <asp:Panel ID="specificationDetailsBody" runat="server" CssClass="specificationDetailsBody" ViewStateMode="Disabled">                
                <div class="HeaderText">
                    <asp:Label ID="lblHeaderText" runat="server"></asp:Label>
                </div>
                <telerik:RadTabStrip ID="SpecificationDetailsRadTabStrip" runat="server" MultiPageID="SpecificationDetailsRadMultiPage"
                    AutoPostBack="false">
                    <Tabs>
                        <telerik:RadTab runat="server" PageViewID="RadPageGeneral" Text="General" Selected="true"></telerik:RadTab>
                        <telerik:RadTab runat="server" PageViewID="RadPageReleases" Text="Versions" Selected="false"></telerik:RadTab>
                        <telerik:RadTab runat="server" PageViewID="RadPageResponsibility" Text="Responsibility" Selected="false"></telerik:RadTab>
                        <telerik:RadTab runat="server" PageViewID="RadPageRelated" Text="Related" Selected="false"></telerik:RadTab>
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="SpecificationDetailsRadMultiPage" runat="server" Width="100%" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px" >
                    <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true" >
                        <table style="width: 100%">
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="referenceLbl" runat="server" Text="Reference:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:Label ID="referenceVal" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="titleLbl" runat="server" Text="Title:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:Label ID="titleVal" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="statusLbl" runat="server" Text="Status:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:Label ID="statusVal" runat="server"></asp:Label>
                                    <asp:HyperLink ID="lnkChangeRequest" runat="server" Target="_blank" Visible="false" CssClass="img" NavigateUrl="#" ImageUrl="images/cr.png" ToolTip="All CRs for this specification"></asp:HyperLink>
                                </td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="typeLbl" runat="server" Text="Type:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:Label ID="typeVal" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="initialPlannedReleaseLbl" runat="server" Text="Initial planned Release:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:Label ID="initialPlannedReleaseVal" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="internalLbl" runat="server" Text="Internal:" ToolTip="Not for SDO transposition" CssClass="lblTooltipStyleDark"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:CheckBox ID="internalVal" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="commonIMSLbl" runat="server" Text="Common IMS Specification:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:CheckBox ID="commonIMSVal" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="radioTechnologyLbl" runat="server" Text="Radio technology:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <asp:CheckBoxList ID="radioTechnologyVals" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td class="TabLineRight moreSpace">
                                    <asp:HyperLink ID="versionFolderHpl" runat="server" target="_blank" Visible="False">Click to see all versions of this specification</asp:HyperLink>
                                </td>
                            </tr>
                            <tr style="max-height: 150px; overflow-y: scroll; margin-top: 10px">
                                <td colspan="2" class="specificationRemarks">
                                    <ult:remarkscontrol runat="server" id="specificationRemarks" ScrollHeight="80"/>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <fieldset class="fsHistory">
                                        <legend>
                                            <asp:Label runat="server">History</asp:Label>
                                        </legend>
                                        <ult:historycontrol runat="server" id="specificationHistory" ScrollHeight="80"/>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageResponsibility" runat="server">
                        <table style="width: 100%">
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="PrimaryResponsibleGroupLbl" runat="server" Text="Primary responsible group:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <ult:CommunityHyperlinkControl runat="server" ID="primaryResponsibleGroup" CommunityNameType="FULL_WITHOUT_3GPP"></ult:CommunityHyperlinkControl>
                                </td>
                            </tr>
                            <tr>
                                <td class="TabLineLeft">
                                    <asp:Label ID="SecondaryResponsibleGroupsLbl" runat="server" Text="Secondary responsible groups:"></asp:Label></td>
                                <td class="TabLineRight">
                                    <ult:CommunityHyperlinkControl runat="server" ID="secondaryResponsibleGroups" CommunityNameType="FULL_WITHOUT_3GPP"></ult:CommunityHyperlinkControl>
                                </td>
                            </tr>
                            <tr style="max-height: 150px; overflow-y: scroll; margin-top: 10px">
                                <td colspan="2" class="specificationRapporteurs">
                                    <ult:RapporteurControl runat="server" id="specificationRapporteurs" />
                                </td>
                            </tr>
                        </table>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageRelated" runat="server">
                        <asp:Panel ID="RelatedSpecificationsPanel" runat="server">
                            <div style="max-height: 160px;">
                                <fieldset id="ParentFieldset">
                                    <legend>
                                        <asp:Label ID="ParentSpecLbl" runat="server" Text="Parent Specifications"></asp:Label></legend>
                                    <spec:SpecificationListControl runat="server" ID="parentSpecifications" />
                                </fieldset>
                            </div>
                            <div style="max-height: 160px;">
                                <fieldset id="ChildFieldset">
                                    <legend>
                                        <asp:Label ID="ChildSpecLbl" runat="server" Text="Child Specifications"></asp:Label></legend>
                                    <spec:SpecificationListControl runat="server" ID="childSpecifications" />
                                </fieldset>
                            </div>
                            <div style="max-height: 220px;">
                                <ult:relatedwicontrol id="SpecificationRelatedWorkItems" runat="server" />
                            </div>
                        </asp:Panel>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageReleases" runat="server" ViewStateMode="Enabled">
                        <asp:Panel ID="notifMsg" runat="server" Visible="false">
                            <asp:Label runat="server" ID="notifMsgTxt"></asp:Label>
                        </asp:Panel>
                        <spec:SpecificationReleaseControl runat="server" ID="SpecificationReleaseControl1" />
                    </telerik:RadPageView>
                </telerik:RadMultiPage>
                <div class="specificationDetailsAction">
                    <asp:LinkButton ID="EditBtn" runat="server" Text="Edit" CssClass="btn3GPP-success" OnClick="EditSpecificationDetails_Click" />
                    <asp:LinkButton ID="WithdrawBtn" runat="server" Text="Definitively withdraw" CssClass="btn3GPP-success"/>
                    <asp:LinkButton ID="ExitBtn" runat="server" Text="Exit" CssClass="btn3GPP-success" OnClientClick="return closePopUpWindow()" />
                    <asp:LinkButton ID="DeleteBtn" runat="server" Text="Delete" CssClass="btn3GPP-delete" OnClientClick="openConfirmDeleteRadWin();return false;" />
                </div>
                <script type="text/javascript">

                    /* Exit function */
                    function closePopUpWindow() {
                        window.close();
                    }

                    $(document).ready(function () {
                        setTimeout(function () {
                            var specificationNumber = "Specification ";
                            if ($("#referenceVal").html() != "undefined" && $("#referenceVal").html() != "" && $("#referenceVal").html() != "-") {
                                specificationNumber += "# " + $("#referenceVal").html();
                            }
                            document.title = specificationNumber;
                        }, 200);

                        setTimeout(function () {
                            $("#notifMsg").fadeOut("slow", null);
                        }, 7000);
                    });
                </script>
            </asp:Panel>
        </asp:Panel>
        <script type="text/javascript">
            //<![CDATA[
            // Open popup for confirm delete
            function openConfirmDeleteRadWin() {
                var win = $find("<%= ConfirmDeleteWindow.ClientID %>");
                win.setSize(420, 140);
                win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
                win.set_modal(true);
                win.set_visibleStatusbar(false);
                win.show();
                return false;
            }
            // Open popup for definitve withdrawal
            function openDefinitiveWithdrawlRadWin() {
                var win = $find("<%= WithdrawRadWindow.ClientID %>");
                win.setSize(450, 220);
                win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
                win.set_modal(true);
                win.set_visibleStatusbar(false);
                win.show();
                win.add_close(OnClientClose);
                return false;
            }
            // On popup closure, sheow result popup 
            function OnClientClose(sender, eventArgs) {
                var arg = eventArgs.get_argument();
                if (arg) {
                    var operationResult = arg.OperationResult;
                    if (operationResult == "True") {
                        window.opener.location.reload(true);
                        var successWin = $find("<%= WithdrawSuccessRadWindow.ClientID %>");
                        successWin.setSize(290, 120);
                        successWin.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
                        successWin.set_modal(true);
                        successWin.set_visibleStatusbar(false);
                        successWin.show();

                    }
                    else {
                        var failureWin = $find("<%= WithdrawFailureRadWindow.ClientID %>");
                        failureWin.setSize(290, 120);
                        failureWin.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
                        failureWin.set_modal(true);
                        failureWin.set_visibleStatusbar(false);
                        failureWin.show();
                    }
                }
            }
            // Refresh window in case the operation succede
            function refreshWindow() {
                cancel();
                window.location.reload(true);
            }

            // Close all modal windows
            function closeAllModals() {
                var manager = GetRadWindowManager();
                manager.closeAll();
            }

            function cancelDeleteAction() {
                $find("ConfirmDeleteWindow").close();
            }

            function cancel() {
                closeAllModals();
            }
            //]]>
        </script> 
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
            <windows>      
                <telerik:RadWindow ID="WithdrawRadWindow" runat="server" Behaviors="Close" 
                    NavigateUrl="DefinitiveWithdrawlMeetingSelectPopUp.aspx"> </telerik:RadWindow>
                <telerik:RadWindow ID="WithdrawSuccessRadWindow" runat="server" Behaviors="Close"> 
                    <ContentTemplate>
                        <br />
                        <div class="header">               
                            Specification has been successfully withdrawn <br />
                        </div>
                        <div class="footer" style="position:absolute; bottom:10px; right:20px;">                            
                            <asp:Button id="btnCancelSuccess" runat="server" Text ="OK" OnClientClick="refreshWindow(); return false;"/>
                        </div>
                    </ContentTemplate>
                </telerik:RadWindow> 
                <telerik:RadWindow ID="WithdrawFailureRadWindow" runat="server" Behaviors="Close"> 
                    <ContentTemplate>
                        <br />
                        <div class="header">               
                            Specification withdrawal has failed <br />
                        </div>
                        <div class="footer" style="position:absolute; bottom:10px; right:20px;">                            
                            <asp:Button id="btnCancelFailure" runat="server" Text ="OK" OnClientClicked="cancel"/>
                        </div>
                    </ContentTemplate>
                </telerik:RadWindow>   
                <telerik:RadWindow ID="ConfirmDeleteWindow" CssClass="confirmDeleteWindow" runat="server" Modal="true" Title="Confirm delete" 
            VisibleStatusbar="false" Behaviors="Close">
                    <ContentTemplate>
                        <div class="header">
                            Are you sure you want delete specification ?<br />
                        </div>
                        <div class="footer">                            
                            <asp:Button CssClass="btn3GPP-success" id="btnConfirmDelete" runat="server" Text ="Confirm delete" OnClick="btnConfirmDelete_Click"/>
                            <asp:Button id="btnCancelDelete" CssClass="btn3GPP-success" runat="server" Text ="Cancel" OnClientClick="cancelDeleteAction(); return false;"/>
                        </div>
                    </ContentTemplate>
                </telerik:RadWindow>                   
            </windows>
        </telerik:RadWindowManager>
               
    </form>
</body>
</html>
