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
            <telerik:RadGrid ID="rgSpecificationList" runat="server" 
                EnableEmbeddedSkins="false"
                EnableEmbeddedBaseStylesheet="false" 
                Skin="Ultimate"
                AllowPaging="true"
                PageSize="1"
                AllowFilteringByColumn="false" 
                AutoGenerateColumns="false"                
                OnNeedDataSource="rgSpecificationList_NeedDataSource"
                OnItemDataBound="rgSpecificationList_ItemDataBound">
                <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" PageButtonCount="10"/>
                <MasterTableView ClientDataKeyNames="Pk_SpecificationId">
                    <Columns>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" DataField="Number" HeaderText="Specification Number" UniqueName="Number"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="5%" DataField="SpecificationTypeShortName" HeaderText="Type" UniqueName="Type"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Title" HeaderText="Title" UniqueName="Title"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" DataField="Status" HeaderText="Status" UniqueName="Status"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" DataField="PrimeResponsibleGroupShortName" HeaderText="Primary Responsible Group" UniqueName="PrimeResponsibleGroupShortName"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderStyle-Width="120px" UniqueName="SpecificationAdditionalDetails">
                            <ItemTemplate>
                                <table id="specAdditionalDetails">
                                    <tr>
                                        <td><img id="imgViewSpecifications" title="View Specification" class="imgViewSpecifications" alt="View Specification" src="/DesktopModules/Specifications/images/details.png"
                                                onclick="var popUp=window.open('/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 'height=550,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" /></td>
                                        <td><img id="imgLock" title="Internal" alt="Internal" src="/DesktopModules/Specifications/images/lock.png" style='display: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsForPublication")) ? "none" : "block" %>' /></td>
                                        <td><img id="imgIMS" title="Common IMS" alt="Common IMS" src="/DesktopModules/Specifications/images/ims.png" style='display: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"ComIMS")) ? "block" : "none" %>' /></td>
                                        <td><asp:Image ID="img2G" runat="server" ToolTip="2nd Generation" AlternateText="2nd Generation" ImageUrl="/DesktopModules/Specifications/images/2g.png" /></td>
                                        <td><asp:Image ID="img3G" runat="server" ToolTip="3rd Generation" AlternateText="3rd Generation" ImageUrl="/DesktopModules/Specifications/images/3g.png" /></td>
                                        <td><asp:Image ID="imgLTE" runat="server" ToolTip="Long-Term Evolution" AlternateText="Long-Term Evolution" ImageUrl="/DesktopModules/Specifications/images/lte.png" /></td>
                                        <td><img id="imgCR" title="CR is pending implementation" class="imgCR" alt="CR is pending implementation" src="/DesktopModules/Specifications/images/cr.png" /></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </td>
    </tr>
</table>
