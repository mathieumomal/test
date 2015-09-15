﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeRequestList.ascx.cs" Inherits="Etsi.Ultimate.Module.CRs.ChangeRequestList" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="ShareUrlControl" Src="../../controls/Ultimate/ShareUrlControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="FullViewControl" Src="../../controls/Ultimate/FullView.ascx" %>
<%@ Register TagPrefix="ult" TagName="ReleaseSearchControl" Src="../../controls/Ultimate/ReleaseSearchControl.ascx" %>

<telerik:RadWindowManager ID="RadWindowMgr" runat="server"></telerik:RadWindowManager>
<telerik:RadAjaxManager ID="ramCrs" runat="server" EnablePageHeadUpdate="false"></telerik:RadAjaxManager>
<script type="text/javascript" src="/DesktopModules/CRs/JS/crScript.js"></script>

<asp:Panel ID="crList" runat="server" Visible="false" ClientIDMode="Static">
    <asp:UpdateProgress ID="upCrSearch" runat="server" DisplayAfter="200">
        <ProgressTemplate>
            <div class="modalBackground">
            </div>
            <div class="updateProgress fixed">
                <asp:Image ID="imgProgress" runat="server" Class="rotating" ImageUrl="images/hourglass.png" Width="45" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="upCrSearchGrid" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table style="width: 100%;">
                <tr>
                    <td class="sendCrsToCrPackTd">
                        <telerik:RadComboBox
                            id="rdcbCrPack"
                            CssClass="rdcbCrPackStyle"
                            runat="server"
                            AllowCustomText="False"
                            OnItemsRequested="RdcbCrPack_ItemsRequested"
                            EnableLoadOnDemand="True"
                            Width="200"
                            AutoPostBack="True"
                            EmptyMessage="Search CR-Pack...">  
                        </telerik:RadComboBox>
                        <asp:Button ID="SendToCrPackBtn" runat="server" CssClass="btn3GPP-success" OnClick="SendToCrPackBtn_OnClick" Text="Send to CR-Pack" Enabled="True"/>
                    </td>
                    <td class="moduleHeaderIcon">
                        <ult:fullviewcontrol id="CrFullView" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <telerik:RadPanelBar runat="server" ID="rpbCrSearch" Width="100%" OnClientItemClicking="PreventCrSearchCollapse">
                            <Items>
                                <telerik:RadPanelItem runat="server" ID="rpiCrSearch">
                                    <HeaderTemplate>
                                        <table class="crSearchHeader">
                                            <tr>
                                                <td style="width: 20px;">
                                                    <ult:shareurlcontrol id="crShareUrl" runat="server" />
                                                </td>
                                                <td style="text-align: center" class="openCloseRadPanelBar">
                                                    <asp:Label ID="lblCrSearchHeader" runat="server" CssClass="openCloseRadPanelBar" />
                                                </td>
                                                <td style="width: 20px;">
                                                    <a class="rpExpandable">
                                                        <span class="rpExpandHandle"></span>
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlCrSearchContainer" runat="server" DefaultButton="btnSearch">
                                            <table style="width: 100%; padding: 20px 50px 20px 50px;">
                                                <tr>
                                                    <td style="width: 100%; vertical-align: top;">
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td style="width: 125px;">Specification number</td>
                                                                <td>
                                                                    <asp:TextBox ID="txtSpecificationNumber" Width="296px" MaxLength="20" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>WG Status</td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="rcbWgStatus" runat="server" Width="300" AutoPostBack="false" CheckBoxes="true"></telerik:RadComboBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Target Release</td>
                                                                <td>
                                                                    <ult:ReleaseSearchControl id="releaseSearchControl" runat="server" Width="300" DropDownWidth="300"/>
                                                                </td>
                                                                <td>TSG Status</td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="rcbTsgStatus" runat="server" Width="300" AutoPostBack="false" CheckBoxes="true"></telerik:RadComboBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Meeting</td>
                                                                <td>
                                                                   <telerik:RadAutoCompleteBox ID="racMeeting" runat="server" InputType="Token" Width="300" DropDownWidth="300" DropDownHeight="150" Filter="Contains">
                                                                        <TextSettings SelectionMode="Multiple" />
                                                                       <WebServiceSettings Method="GetMeetings" Path="~/DesktopModules/CRs/GetWiMtgData.asmx" />
                                                                    </telerik:RadAutoCompleteBox>
                                                                </td>
                                                                <td>Work Item</td>
                                                                <td>
                                                                    <telerik:RadAutoCompleteBox ID="racWorkItem" runat="server" InputType="Token" Width="300" DropDownWidth="300" DropDownHeight="150" Filter="Contains">
                                                                        <TextSettings SelectionMode="Multiple"  />
                                                                        <WebServiceSettings Method="GetWorkItems" Path="~/DesktopModules/CRs/GetWiMtgData.asmx" />
                                                                    </telerik:RadAutoCompleteBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="3" />
                                                                <td style="text-align: right; padding-right: 0px">
                                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" Width="150px" OnClick="btnSearch_Click" OnClientClick="collapseCrPanelItem()"></asp:Button>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <telerik:RadGrid ID="rgCrList" runat="server"
                            ClientIDMode="Static"
                            EnableEmbeddedSkins="false"
                            EnableEmbeddedBaseStylesheet="false"
                            Skin="Ultimate"
                            AllowPaging="true"
                            AllowCustomPaging="true"
                            AllowFilteringByColumn="false"
                            AutoGenerateColumns="false"
                            AllowMultiRowSelection="True"
                            OnNeedDataSource="RgCrList_NeedDataSource"
                            OnItemDataBound="RgCrList_ItemDataBound">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
                            </ClientSettings>
                            <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" PageButtonCount="10" Position="Bottom" />
                            <MasterTableView ClientDataKeyNames="ChangeRequestId" Width="100%" AllowNaturalSort="false">
                                <Columns>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="3%" UniqueName="CrSelection">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="CrSelectionCheckBox" runat="server"
                                            AutoPostBack="False"/>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridHyperLinkColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="Spec #" UniqueName="SpecNumber"></telerik:GridHyperLinkColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" DataField="ChangeRequestNumber" HeaderText="CR #" UniqueName="ChangeRequestNumber"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" DataField="Revision" HeaderText="Revision #" UniqueName="Revision"></telerik:GridBoundColumn>
                                    <telerik:GridHyperLinkColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="Impacted Version" UniqueName="ImpactedVersion"></telerik:GridHyperLinkColumn>
                                    <telerik:GridHyperLinkColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="Target Release" UniqueName="TargetRelease" Target="_blank"></telerik:GridHyperLinkColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" DataField="Title" HeaderText="Title" UniqueName="Title"></telerik:GridBoundColumn>
                                    <telerik:GridHyperLinkColumn HeaderStyle-Width="8%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="WG TDoc #" UniqueName="WgTdocNumber" Target="_blank"></telerik:GridHyperLinkColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" DataField="WgStatus" HeaderText="WG status" UniqueName="WgStatus"></telerik:GridBoundColumn>
                                    <telerik:GridHyperLinkColumn HeaderStyle-Width="8%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="TSG TDoc #" UniqueName="TsgTdocNumber" Target="_blank"></telerik:GridHyperLinkColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" DataField="TsgStatus" HeaderText="TSG status" UniqueName="TsgStatus"></telerik:GridBoundColumn>
                                    <telerik:GridHyperLinkColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="New Version" UniqueName="NewVersion" Target="_blank"></telerik:GridHyperLinkColumn>
                                </Columns>
                                <NoRecordsTemplate>
                                    <div style="text-align: center">
                                        No Change Request found.
                                    </div>
                                </NoRecordsTemplate>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function collapseCrPanelItem() {
            var panelBar = $find('<%= rpbCrSearch.ClientID %>');
            var item = panelBar.get_items().getItem(0);
            if (item) {
                item.collapse();
            }
        }
        function sendAdaptContentEvent() {
            adaptContentHeight();
        }
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(sendAdaptContentEvent);
    </script>

</asp:Panel>
