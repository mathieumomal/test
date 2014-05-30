<%@ Control Language="C#" ClassName="SpecificationVersionListControl" AutoEventWireup="true" CodeBehind="SpecificationVersionListControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationVersionListControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>

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

    .grid_btn {
        height: 16px;
        width: 16px;
    }

    .float_right {
        float: right;
    }
</style>

<script type="text/javascript">
    function openRadWin(specId, relId) {
        var win = radopen("WithdrawMeetingSelectPopUp.aspx?SpecId=" + specId + "&RelId=" + relId, "Withdraw");
        win.setSize(450, 220);
        win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
        win.set_modal(true);
        win.set_visibleStatusbar(false);
        win.show();
        return false;
    }

    function openRadWinRemarks(specVerId, isEditMode) {
        var win = radopen("VersionRemarksPopUp.aspx?specVerId=" + specVerId + "&isEditMode=" + isEditMode, "Remarks");
        win.setSize(500, 250);
        win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
        win.set_modal(true);
        win.set_visibleStatusbar(false);
        win.show();
        return false;
    }

    function openRadWinVersion(specVerId, action) {
        var win = radopen("/desktopmodules/Versions/UploadVersion.aspx?versionId=" + specVerId + "&action=" + action, "Version");
        var height = (action == 'upload') ? 310 : 270;
        win.setSize(440, height);
        win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
        win.set_modal(true);
        win.set_visibleStatusbar(false);
        win.show();
        return false;
    }


</script>
<asp:Panel runat="server" ID="pnlCover" CssClass="TabContent" Height="100%">
    <asp:Panel runat="server" ID="pnlIconStrip">
        <asp:ImageButton ID="imgUploadVersion" ToolTip="Upload a version" ImageUrl="images/spec_rel-u.png" CssClass="display_size" runat="server" />
        <asp:ImageButton ID="imgAllocateVersion" ToolTip="Allocate a version" ImageUrl="images/spec_rel-a.png" CssClass="display_size" runat="server" />
        <asp:ImageButton ID="imgInhibitPromote" ToolTip="Inhibit Promote" ImageUrl="images/spec_rel-i.png" CssClass="display_size" runat="server" Visible="false" OnClick="imgInhibitPromote_Click" />
        <asp:ImageButton ID="imgRemoveInhibitPromote" ToolTip="Remove-Inhibit Promote" ImageUrl="images/spec_rel-ri.png" CssClass="display_size" runat="server" Visible="false" OnClick="imgRemoveInhibitPromote_Click" />
        <asp:ImageButton ID="imgForceTransposition" ToolTip="Force transposition" ImageUrl="images/spec_rel-f.png" CssClass="display_size" runat="server" OnClick="imgForceTransposition_Click" />
        <asp:ImageButton ID="imgUnforceTransposition" ToolTip="Unforce transposition" ImageUrl="images/spec_rel-f-crossed.png" CssClass="display_size" runat="server" OnClick="imgUnforceTransposition_Click" />
        <asp:ImageButton ID="imgPromoteSpec" ToolTip="Promote specification to next Release" ImageUrl="images/spec_rel-p.png" CssClass="display_size" runat="server" OnClick="imgPromoteSpec_Click" />
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
            <Scrolling AllowScroll="True" UseStaticHeaders="true" />
        </ClientSettings>
        <MasterTableView>
            <Columns>
                <telerik:GridTemplateColumn DataField="Meetings" HeaderText="Meetings" UniqueName="Meetings">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px" />
                    <ItemTemplate>
                        <div class="text-center">
                            <asp:HyperLink runat="server" ID="lnkMeetings" Target="_blank" />
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Version" UniqueName="Version">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px" />
                    <ItemTemplate>
                        <div class="text-center"><%# DataBinder.Eval(Container.DataItem,"Version")%></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="DocumentUploaded" HeaderText="Upload date" UniqueName="DocumentUploaded">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px" />
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem,"DocumentUploaded", "{0:yyyy-MM-dd}") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="LatestRemark" HeaderText="Comment" UniqueName="LatestRemark">
                    <HeaderStyle HorizontalAlign="Center" Width="40%" Font-Bold="True" />
                    <ItemTemplate>
                        <div class="text-left">
                            <asp:Label ID="lblRemarkText" Text='<%# DataBinder.Eval(Container.DataItem,"LatestRemark") %>' runat="server" />
                            <asp:ImageButton ID="imgVersionRemarks" ImageUrl="images/spec_rel-remarks.png" runat="server" CssClass="float_right grid_btn"/>
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Link">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" />
                    <ItemTemplate>
                        <asp:ImageButton ID="imgTransposedSpec" CssClass="grid_btn" ImageUrl="images/spec_rel-tranSpec.png" runat="server" OnClientClick="return false;" />
                        <asp:ImageButton ID="imgRelatedTDocs" CssClass="grid_btn" ImageUrl="images/spec_rel-TDocs.png" runat="server" OnClientClick="return false;" />
                        <asp:ImageButton ID="imgRelatedCRs" CssClass="grid_btn" ImageUrl="images/spec_rel-CRs.png" runat="server" OnClientClick="return false;" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Source" UniqueName="Source" Display="false"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="MtgShortRef" UniqueName="MtgShortRef" Display="false"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Pk_VersionId" UniqueName="Pk_VersionId" Display="false"></telerik:GridBoundColumn>

            </Columns>
            <NoRecordsTemplate>
                <div style="text-align: center">
                    No related specifications
                </div>
            </NoRecordsTemplate>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Panel>
<telerik:RadWindowManager ID="rwmVersionReleaseHeader" runat="server">
    <Windows>
        <telerik:RadWindow ID="rwVersionRemarks" runat="server" Modal="true" Title="Remarks" Width="550" Height="230" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                <ult:remarkscontrol runat="server" id="versionRemarks" />
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>
