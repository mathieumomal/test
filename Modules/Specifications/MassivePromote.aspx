<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MassivePromote.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.MassivePromote" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Etsi.Ultimate.Module.Specifications" Namespace="Etsi.Ultimate.Module.Specifications" TagPrefix="specification" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Massive Promote</title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="JS/jquery.min.js"></script>

    <style>
        #tdMassivePromote fieldset {
            margin: 2%;
            padding: 1% !important;
            border: 1px solid grey !important;
            -webkit-border-radius: 4px !important;
            -moz-border-radius: 4px !important;
            border-radius: 4px !important;
        }
    </style>
</head>
<body style="margin-left: 0;">
    <form id="specMassivePromoteForm" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server">
        </telerik:RadWindowManager>
        <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="750px">
            <asp:Panel ID="specificationMessages" runat="server" Visible="false">
                <asp:Label runat="server" ID="specificationMessagesTxt"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="specMassivePromoteBody" runat="server" CssClass="specificationDetailsBody">
                <div class="HeaderText">
                    <asp:Label ID="lblHeaderText" runat="server"></asp:Label>
                </div>
                <table style="width: 100%">
                    <tr>
                        <td class="TabLineLeft">Initial Release:</td>
                        <td class="TabLineRight">
                            <asp:DropDownList ID="ddlInitialRelease" Width="200" runat="server" OnSelectedIndexChanged="ddlInitialRelease_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="TabLineLeft">Target Release:</td>
                        <td class="TabLineRight">
                            <asp:Label ID="lblTargetRelease" runat="server" /></td>
                    </tr>
                    <tr>
                        <td colspan="2" id="tdMassivePromote">
                            <fieldset id="ParentFieldset">
                                <legend>Specifications to promote</legend>
                                <telerik:RadGrid runat="server" ID="rgSpecificationList" AllowPaging="false"
                                    AllowSorting="false"
                                    AllowFilteringByColumn="false"
                                    AutoGenerateColumns="false"
                                    AllowMultiRowEdit="true"
                                    OnNeedDataSource="rgSpecificationList_NeedDataSource"
                                    OnItemDataBound="rgSpecificationList_ItemDataBound">
                                    <ClientSettings>
                                        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
                                    </ClientSettings>
                                    <MasterTableView ClientDataKeyNames="Pk_SpecificationId" Width="100%" AllowNaturalSort="false">
                                        <Columns>
                                            <telerik:GridBoundColumn HeaderStyle-Font-Bold="true" HeaderStyle-Width="60" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="Number" HeaderText="Promote inhibited" UniqueName="PromoteInhibit"></telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn HeaderStyle-Font-Bold="true" HeaderStyle-Width="60" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="Number" HeaderText="Create new version" UniqueName="CreateNewVersion"></telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn HeaderStyle-Font-Bold="true" HeaderStyle-Width="20%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="Number" HeaderText="Specification Number" UniqueName="SpecificationNumber"></telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn HeaderStyle-Font-Bold="true" HeaderStyle-Width="60" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="SpecificationTypeShortName" HeaderText="Type" UniqueName="Type"></telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn HeaderStyle-Font-Bold="true" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" ShowSortIcon="false" DataField="Title" HeaderText="Title" UniqueName="Title"></telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn HeaderStyle-Font-Bold="true" HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="Status" HeaderText="Status" UniqueName="Status"></telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn HeaderStyle-Font-Bold="true" HeaderStyle-Width="12%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="PrimeResponsibleGroupShortName" HeaderText="Prime Responsible" UniqueName="PrimeResponsibleGroupShortName"></telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn HeaderStyle-Width="40" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" UniqueName="SpecificationAdditionalDetails">
                                                <ItemTemplate>
                                                    <table id="specAdditionalDetails">
                                                        <tr>
                                                            <td>
                                                                <img id="imgViewSpecifications" title="See details" class="imgViewSpecifications" alt="See details" src="/DesktopModules/Specifications/images/details.png"
                                                                    onclick="var popUp=window.open('/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 'Specification-<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 'height=690,width=674,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" /></td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                                <br />
                                <asp:HyperLink runat="server" ID="btnExportPromoteList" Target="_blank" NavigateUrl="#" CssClass="float-right">Export as CSV</asp:HyperLink>
                                <br />
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding-left: 14px;">
                            <asp:LinkButton ID="btnPromote" runat="server" Text="Promote" CssClass="btn3GPP-success" OnClick="btnPromote_Click" /></td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
