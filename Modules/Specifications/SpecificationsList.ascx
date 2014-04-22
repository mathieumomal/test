<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationsList.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationsList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="ShareUrlControl" Src="../../controls/Ultimate/ShareUrlControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="FullViewControl" Src="../../controls/Ultimate/FullView.ascx" %>

<table style="width: 100%;">
    <tr>
        <td>
            <telerik:RadPanelBar runat="server" ID="rpbSpecSearch" Width="100%">
                <Items>
                    <telerik:RadPanelItem runat="server" ID="searchPanel" Expanded="True">
                        <HeaderTemplate>
                            <table style="width: 100%; vertical-align: middle" class="SpecificationSearchHeader">
                                <tr>
                                    <td style="width: 20px;">
                                        <ult:ShareUrlControl ID="ultShareUrl" runat="server" />
                                    </td>
                                    <td style="text-align: center">
                                        <asp:Label ID="lblSearchHeader" runat="server" /></td>
                                    <td style="width: 20px;">
                                        <a class="rpExpandable">
                                            <span class="rpExpandHandle"></span>
                                        </a>
                                    </td>
                                </tr>
                            </table>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table style="width: 100%; padding: 20px 50px 20px 50px">
                                <tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </Items>
            </telerik:RadPanelBar>
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadButton ID="rbNewSpecification" runat="server" Text="New Specification" />
            <span style="float: right; padding-bottom: 2px; white-space: nowrap">
                <asp:HyperLink ID="lnkManageITURecommendations" runat="server" Text="Manage ITU Recommendations" Target="_blank" />
                <ult:fullviewcontrol id="ultFullView" runat="server" />
            </span>
        </td>
    </tr>
    <tr>
        <td>
            <telerik:RadGrid ID="rgSpecificationList" runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" AllowPaging="true" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false">
                <MasterTableView ClientDataKeyNames="Pk_SpecificationId">
                    <Columns>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" DataField="Number" HeaderText="Specification Number" UniqueName="Number"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="5%" DataField="Type" HeaderText="Type" UniqueName="Type"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="40%" DataField="Title" HeaderText="Title" UniqueName="Title"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" DataField="IsActive" HeaderText="Status" UniqueName="IsActive"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" DataField="ITU_Description" HeaderText="Primary Responsible Group" UniqueName="ITU_Description"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderStyle-Width="25%" UniqueName="SpecificationDetails">
                            <ItemTemplate>
                                <img class="imgViewSpecifications" title="See details" alt="See details" src="/DesktopModules/Specifications/images/details.png"
                                    onclick="var popUp=window.open('/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 
                            'height=690,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </td>
    </tr>
</table>
