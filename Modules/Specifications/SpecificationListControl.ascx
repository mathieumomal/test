<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationListControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationListControl" %>
<%@ Import Namespace="Etsi.Ultimate.DomainClasses" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<%@ Register TagPrefix="ult" TagName="CommunityHyperlinkControl" Src="../../controls/Ultimate/CommunityHyperlinkControl.ascx" %>

<telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
    <script src="/desktopmodules/specifications/JS/CommonScript.js?v=<%=ConfigurationManager.AppSettings["AppVersion"] %>"></script>
</telerik:RadCodeBlock>

<style type="text/css">
    .RadGrid_Default th.rgHeader {
        background-color: grey;
        border: none;
        border-bottom: 1px solid grey;
    }

    .RadGrid_Default .rgEditRow td {
        border: none;
    }

    .display_inline {
        display: inline;
        padding-right: 3px;
    }
</style>
<script type="text/javascript">
    function showProgress<%=this.ClientID%>() {
        $('#<%=btnAddSpecification.ClientID%>').next().show();
        $('#<%=btnAddSpecification.ClientID%>').hide();
    }
</script>
<table style="width: 100%">
    <tr>
        <td colspan="3">
            <telerik:RadGrid runat="server" ID="specificationsGrid" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" AllowPaging="false"
                AllowSorting="false"
                AllowFilteringByColumn="false"
                AutoGenerateColumns="false"
                AllowMultiRowEdit="true"
                OnNeedDataSource="specificationsGrid_NeedDataSource"
                OnItemDataBound="specificationsGrid_ItemDataBound"
                Style="min-width: 400px">
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                </ClientSettings>
                <MasterTableView ClientDataKeyNames="Pk_SpecificationId" EditMode="InPlace">
                    <Columns>
                        <telerik:GridTemplateColumn DataField="Number" HeaderText=" Spec #" UniqueName="Number">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="70px" />
                            <ItemTemplate>
                                <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Number") %></div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="SpecificationType" HeaderText="Type" UniqueName="SpecificationType">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="30px" />
                            <ItemTemplate>
                                <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"SpecificationType") %></div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="Title" HeaderText="Title" UniqueName="Title">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="200px" />
                            <ItemTemplate>
                                <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Title") %></div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="Status" HeaderText="Status" UniqueName="Status">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px" />
                            <ItemTemplate>
                                <div class="text-left">
                                    <span title="<%# DataBinder.Eval(Container.DataItem,"Status") %>"><%# DataBinder.Eval(Container.DataItem,"ShortStatus") %></span>
                                </div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn DataField="PrimeResponsibleGroupShortName" HeaderText="Prime grp" UniqueName="PrimeResponsibleGroupShortName">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="70px" />
                            <ItemTemplate>
                                <div class="text-left">
                                    <ult:CommunityHyperlinkControl runat="server" ID="primaryResponsibleGroup"></ult:CommunityHyperlinkControl>
                                </div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="SpecificationActions">
                            <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px" />
                            <ItemTemplate>
                                <img id="imgViewSpecifications" title="See details" class="imgViewSpecifications" alt="See details" src="/DesktopModules/Specifications/images/details.png"
                                                        onclick="OpenSpecDetailsPage('/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 'Specification-<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>');" />
                                <asp:ImageButton Visible="false" CssClass="display_inline" ID="btnRemoveSpec" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId") %>' ImageUrl="/controls/Ultimate/images/delete.png" runat="server" OnClientClick="setDeleteProgress(this)" OnClick="btnRemoveSpec_Click" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <NoRecordsTemplate>
                        <div style="text-align: center">
                            No related specifications
                        </div>
                    </NoRecordsTemplate>
                </MasterTableView>
            </telerik:RadGrid>
        </td>
    </tr>
    <tr>
        <td style="padding-right: 5px">
            <asp:Label ID="lblAddSpecification" runat="server" Width="100%" Text="Add specification"></asp:Label>
        </td>
        <td style="padding-right: 5px">
            <telerik:RadComboBox
                ID="rcbAddSpecification"
                runat="server"
                AllowCustomText="true"
                EnableLoadOnDemand="True"
                Width="100%"
                OnItemsRequested="rcbAddSpecification_ItemsRequested"
                CssClass="rcbAddSpecificationStyle"
                AutoPostBack="true"
                EmptyMessage="Search Specifications...">
            </telerik:RadComboBox>
        </td>
        <td>
            <asp:Button ID="btnAddSpecification" OnClick="btnAddSpecification_Click" runat="server" Text="Add" Width="100%" />
            <img src="/desktopmodules/Specifications/images/busy.gif" alt="progress" style="display:none" />
        </td>
    </tr>
</table>
<telerik:RadWindowManager ID="RadWindowAlert" runat="server" />