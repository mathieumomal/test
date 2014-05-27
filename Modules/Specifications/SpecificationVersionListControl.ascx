﻿<%@ Control Language="C#" ClassName="SpecificationVersionListControl" AutoEventWireup="true" CodeBehind="SpecificationVersionListControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationVersionListControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

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

    .display_size {
        border: 1px solid gray;
        background-color: #75B91A;
        height: 20px;
        width: 20px;
        border-radius: 6px;
    }
</style>

<script type="text/javascript">
    function GridCreated<%= this.ClientID%>(sender, args) {
        var scrollArea = sender.GridDataDiv;
        var parent = $get("<%= pnlIconStrip.ClientID %>");
        console.log(parent.clientHeight);
        var gridHeader = sender.GridHeaderDiv;
        scrollArea.style.height = parent.clientHeight - gridHeader.clientHeight + "px";
    }

    function openRadWin(specId, relId) {
        var win = radopen("WithdrawMeetingSelectPopUp.aspx?SpecId=" + specId + "&RelId=" + relId, "Withdraw");
        win.setSize(450, 300);
        win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
        win.show();
        return false;
    }
</script>
<asp:Panel runat="server" ID="pnlCover" CssClass="TabContent" Height="100%">
    <asp:Panel runat="server" ID="pnlIconStrip">
        <asp:ImageButton ID="imgUploadVersion" ToolTip="Upload a version" ImageUrl="images/spec_rel-u.png" CssClass="display_size" runat="server" />
        <asp:ImageButton ID="imgInhibitPromote" ToolTip="Inhibit Promote" ImageUrl="images/spec_rel-i.png" CssClass="display_size" runat="server" Visible="false" OnClick="imgInhibitPromote_Click" />
        <asp:ImageButton ID="imgRemoveInhibitPromote" ToolTip="Remove-Inhibit Promote" ImageUrl="images/spec_rel-ri.png" CssClass="display_size" runat="server" Visible="false" OnClick="imgRemoveInhibitPromote_Click" />
        <asp:ImageButton ID="imgForceTransposition" ToolTip="Force transposition" ImageUrl="images/spec_rel-f.png" CssClass="display_size" runat="server" OnClick="imgForceTransposition_Click" />
        <asp:ImageButton ID="imgUnforceTransposition" ToolTip="Unforce transposition" ImageUrl="images/spec_rel-f-crossed.png" CssClass="display_size" runat="server" OnClick="imgUnforceTransposition_Click" />
        <asp:ImageButton ID="imgPromoteSpec" ToolTip="Promote specification to next Release" ImageUrl="images/spec_rel-p.png" CssClass="display_size" runat="server" />
        <asp:ImageButton ID="imgWithdrawSpec" ToolTip="Withdraw specification from Release" ImageUrl="images/spec_rel-w.png" CssClass="display_size" runat="server" />
    </asp:Panel>
    <telerik:RadGrid runat="server" ID="specificationsVersionGrid"
        AllowPaging="false"
        AllowSorting="false"
        AllowFilteringByColumn="false"
        AutoGenerateColumns="false"
        AllowMultiRowEdit="true"
        OnItemDataBound="specificationsVersionGrid_ItemDataBound">
        <ClientSettings>
            <Scrolling AllowScroll="True" ScrollHeight="200px" UseStaticHeaders="true" />
        </ClientSettings>
        <MasterTableView>
            <Columns>
                <telerik:GridTemplateColumn DataField="Meetings" HeaderText="Meetings" UniqueName="Meetings">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px" />
                    <ItemTemplate>
                        <div class="text-center"><%# DataBinder.Eval(Container.DataItem,"ETSI_WKI_ID") %></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Version" UniqueName="Version">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px" />
                    <ItemTemplate>
                        <div class="text-center"><%# DataBinder.Eval(Container.DataItem,"Version")%></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="DocumentUploaded" HeaderText="Document Uploaded" UniqueName="DocumentUploaded">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px" />
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem,"DocumentUploaded", "{0:yyyy-MM-dd}") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="LatestRemark" HeaderText="Comment" UniqueName="LatestRemark">
                    <HeaderStyle HorizontalAlign="Center" Width="40%" Font-Bold="True" />
                    <ItemTemplate>
                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"LatestRemark") %></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Link">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" />
                    <ItemTemplate>
                        <div class="text-left">Links</div>
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
</asp:Panel>