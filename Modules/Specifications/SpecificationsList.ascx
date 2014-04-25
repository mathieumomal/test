<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationsList.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationsList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="ShareUrlControl" Src="../../controls/Ultimate/ShareUrlControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="FullViewControl" Src="../../controls/Ultimate/FullView.ascx" %>
<%@ Register TagPrefix="ult" TagName="ReleaseSearchControl" Src="../../controls/Ultimate/ReleaseSearchControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="CommunityControl" Src="../../controls/Ultimate/CommunityControl.ascx" %>

<style>
    td {
        vertical-align: top;
        text-align: left;
    }

    .modalBackground {
        background-color: Gray;
        filter: alpha(opacity=60);
        opacity: 0.60;
        width: 100%;
        top: 0px;
        left: 0px;
        position: fixed;
        height: 100%;
        z-index: 3000;
    }

    .updateProgress {
        margin: auto;
        font-family: Trebuchet MS;
        filter: alpha(opacity=100);
        opacity: 1;
        font-size: small;
        vertical-align: middle;
        color: #275721;
        text-align: center;
        background-color: White;
        padding: 10px;
        -moz-border-radius: 15px;
        z-index: 3001;
        border-radius: 15px;
    }

    .updateProgress .Fixed {
        top: 45%;
        position: fixed;
        right: 45%;
    }

</style>

<asp:UpdateProgress ID="updateProgressSpecificationGrid" runat="server" DisplayAfter="200">
    <ProgressTemplate>
        <div class="modalBackground">
        </div>
        <div class="updateProgress fixed">
            <asp:Image ID="imgProgress" runat="server" Class="rotating" ImageUrl="~/DesktopModules/WorkItem/images/hourglass.png" Width="45" />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>

<asp:UpdatePanel ID="upSpecificationGrid" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <table style="width: 100%;">
            <tr>
                <td style="line-height:22px;">
                    <telerik:RadButton ID="rbNewSpecification" runat="server" Text="New Specification" />
                    <span style="float: right; padding-bottom: 2px; white-space: nowrap">
                        <asp:HyperLink ID="lnkManageITURecommendations" runat="server" Text="Manage ITU Recommendations" Target="_blank" />
                        <ult:fullviewcontrol id="ultFullView" runat="server" />
                    </span>
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadPanelBar runat="server" ID="rpbSpecSearch" Width="100%">
                        <items>
                    <telerik:RadPanelItem runat="server" ID="searchPanel">
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
                            <table style="width:100%;padding: 20px 50px 20px 50px;">
                                <tr>
                                    <td style="width:50%;">
                                        <table style="width:100%;">
                                            <tr>
                                                <td style="width:40%;">Title/Specification number</td>
                                                <td style="width:60%;"><asp:TextBox ID="txtTitle" Width="196px" runat="server"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>Series</td>
                                                <td><telerik:RadComboBox ID="rcbSeries" runat="server" Width="200px" CheckBoxes="true"></telerik:RadComboBox></td>
                                            </tr>
                                            <tr>
                                                <td>Type</td>
                                                <td><asp:CheckBox ID="cbTechnicalSpecification" runat="server" Text="Technical Specification (TS)"></asp:CheckBox><br />
                                                    <asp:CheckBox ID="cbTechnicalReport" runat="server" Text="Technical Report (TR)"></asp:CheckBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Number not yet allocated</td>
                                                <td><asp:CheckBox ID="cbNumNotYetAllocated" runat="server"></asp:CheckBox></td>
                                            </tr>
                                            <tr>
                                                <td>Primary responsible group(s)</td>
                                                <td><ult:CommunityControl id="CommunityCtrl" Width="200" IsSingleSelection="false" IsEditMode="true" runat="server"/></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="width:50%;">
                                         <table style="width:100%;">
                                            <tr>
                                                <td style="width:40%;">Release</td>
                                                <td style="width:60%;"><ult:ReleaseSearchControl id="ReleaseCtrl" runat="server" Width="200" DropDownWidth="200"/></td>
                                            </tr>
                                            <tr>
                                                <td>Status</td>
                                                <td><asp:CheckBox ID="cbDraft" runat="server" Text="Draft"></asp:CheckBox><br />
                                                    <asp:CheckBox ID="cbUnderCC" runat="server" Text="Under change control"></asp:CheckBox><br />
                                                    <asp:CheckBox ID="cbWithdrawnBeforeCC" runat="server" Text="Withdrawn before change control"></asp:CheckBox><br />
                                                    <asp:CheckBox ID="cbWithdrawnAfterCC" runat="server" Text="Withdrawn under change control"></asp:CheckBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Publication</td>
                                                <td><asp:CheckBox ID="cbInternal" runat="server" Text="Internal"></asp:CheckBox><br />
                                                    <asp:CheckBox ID="cbForPublication" runat="server" Text="For Publication"></asp:CheckBox>
                                                </td>
                                            </tr>
                                             <tr>
                                                 <td>Technology</td>
                                                <td><asp:CheckBoxList ID="cblTechnology" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                                </td>
                                             </tr>
                                             <tr>
                                                <td colspan="2" style="text-align:right; padding-right:20px">
                                                <asp:Button ID="btnDefault" Visible="false" runat="server" Text="Default" Width="150px" OnClientClick="collapseItem()"></asp:Button>
                                                <asp:Button ID="btnSearch" runat="server" Text="Search"  Width="150px" OnClick="btnSearch_Click" OnClientClick="collapseItem()"></asp:Button></td>
                                             </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </items>
                    </telerik:RadPanelBar>
                </td>
            </tr>
            <tr>
                <td>
            <telerik:RadGrid ID="rgSpecificationList" runat="server" 
                EnableEmbeddedSkins="false"
                EnableEmbeddedBaseStylesheet="false" 
                Skin="Ultimate"
                AllowSorting="true"
                AllowPaging="true"
                PageSize="50"
                AllowFilteringByColumn="false" 
                AutoGenerateColumns="false"                
                OnNeedDataSource="rgSpecificationList_NeedDataSource"
                OnItemDataBound="rgSpecificationList_ItemDataBound">
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
                </ClientSettings>
                <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" PageButtonCount="10"/>
                        <MasterTableView ClientDataKeyNames="Pk_SpecificationId" Width="100%">
                            <SortExpressions>
                                <telerik:GridSortExpression FieldName="Number" SortOrder="Ascending" />
                                <telerik:GridSortExpression FieldName="Title" SortOrder="Ascending" />
                            </SortExpressions>
                    <Columns>
                        <telerik:GridBoundColumn HeaderStyle-Width="8%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" DataField="Number" HeaderText="Specification Number" UniqueName="Number"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="SpecificationTypeShortName" HeaderText="Type" UniqueName="Type"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="50%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" DataField="Title"  HeaderText="Title" UniqueName="Title"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="Status" HeaderText="Status" UniqueName="Status"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="PrimeResponsibleGroupShortName" HeaderText="Primary Responsible Group" UniqueName="PrimeResponsibleGroupShortName"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderStyle-Width="17%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" UniqueName="SpecificationAdditionalDetails">
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
    </ContentTemplate>
</asp:UpdatePanel>
<script type="text/javascript">
    function collapseItem() {
        var panelBar = $find("<%= rpbSpecSearch.ClientID %>");
        var item = panelBar.get_items().getItem(0);
        if (item) {
            item.collapse();
        }
    }
</script>