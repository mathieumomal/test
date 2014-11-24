<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangeRequestList.ascx.cs" Inherits="Etsi.Ultimate.Module.CRs.ChangeRequestList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<div id="crList">
    <table>
        <tr>
            <td>
                <telerik:RadGrid ID="rgCrList" runat="server"
                                 EnableEmbeddedSkins="false"
                                 EnableEmbeddedBaseStylesheet="false"
                                 Skin="Ultimate"
                                 AllowSorting="true"
                                 AllowPaging="true"
                                 AllowCustomPaging="true"
                                 PageSize="50"
                                 AllowFilteringByColumn="false"
                                 AutoGenerateColumns="false"
                                 OnNeedDataSource="RgCrList_NeedDataSource"
                                 OnItemDataBound="RgCrList_ItemDataBound">
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
                    </ClientSettings>
                    <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" PageButtonCount="10" Position="Top" />
                    <MasterTableView ClientDataKeyNames="ChangeRequestId" Width="100%" AllowNaturalSort="false">
                        <Columns>
                            <telerik:GridHyperLinkColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" HeaderText="Spec #" UniqueName="SpecNumber" Target="_blank"></telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="ChangeRequestNumber" HeaderText="CR #" UniqueName="ChangeRequestNumber"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="Revision" HeaderText="Revision #" UniqueName="Revision"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="ImpactedVersion" HeaderText="Impacted Version" UniqueName="ImpactedVersion"></telerik:GridBoundColumn>
                            <telerik:GridHyperLinkColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" HeaderText="Target Release" UniqueName="TargetRelease" Target="_blank"></telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn HeaderStyle-Width="15%" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ShowSortIcon="false" DataField="Title" HeaderText="Title" UniqueName="Title"></telerik:GridBoundColumn>
                            <telerik:GridHyperLinkColumn HeaderStyle-Width="8%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" HeaderText="WG TDoc #" UniqueName="WgTdocNumber" Target="_blank"></telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="WgStatus" HeaderText="WG" UniqueName="WgStatus"></telerik:GridBoundColumn>
                            <telerik:GridHyperLinkColumn HeaderStyle-Width="8%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" HeaderText="TSG TDoc #" UniqueName="TsgTdocNumber" Target="_blank"></telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="TsgStatus" HeaderText="TSG" UniqueName="TsgStatus"></telerik:GridBoundColumn>
                            <telerik:GridHyperLinkColumn HeaderStyle-Width="7%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" HeaderText="New Version" UniqueName="NewVersion" Target="_blank"></telerik:GridHyperLinkColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            <div style="text-align: center">
                                No Change Request
                            </div>
                        </NoRecordsTemplate>
                    </MasterTableView>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
</div>