﻿<%@ Control Language="C#" ClassName="SpecificationVersionListControl" AutoEventWireup="true" CodeBehind="SpecificationVersionListControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationVersionListControl" %>
<%@ Import Namespace="Etsi.Ultimate.Utils" %>
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

    .remarks_btn
    {
        width: 16px;
        height: 16px;
    }

    .float_right {
        float: right;
    }

    .notAvailable
    {
        opacity: 0.3;
        cursor:default !important;
    }

    .linkStyle
    {
        display:inline-block;
        height: 16px;
    }

    .linkStyle img
    {
        height: 16px;
    }

    a.disabled
    {
        pointer-events: none;
    }
</style>

<script type="text/javascript">
    /*
        Default open popup launcher
    */
    function openRadWin(url, windowId, width, height, title, shouldCallMethodWhenClose) {
        var win = radopen(url, windowId);
        if (shouldCallMethodWhenClose)
            win.add_close(radWinCloseCallBackForReload);

        win.setSize(width, height);
        win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
        win.set_modal(true);
        win.set_visibleStatusbar(false);
        win.set_title(title);
        win.show();
        return false;
    }

    /* 
        Popup Closed : reload event 
    */
    function radWinCloseCallBackForReload(sender, eventArgs) {
        //Refresh page
        var url = window.location.href;
        if (url.indexOf('&selectedTab=Releases') > -1)
            location.reload();
        else
            window.location = window.location + "&selectedTab=Releases";

        //get the transferred arguments
        var arg = eventArgs.get_argument();
        if (arg) {
            var status = arg.status;
            if (status === "success") { }
        }

        //remove RadWindow close callback
        var oWnd = $find("Version");
        if (oWnd) {
            oWnd.remove_close(radWinCloseCallBackForReload);
        }
    }
</script>
<asp:Panel runat="server" ID="pnlCover" CssClass="TabContent" Height="100%">    
    <asp:Panel runat="server" ID="pnlIconStrip">        
        <asp:ImageButton ID="imgUploadVersion" ToolTip="Upload a version" ImageUrl="images/spec_rel-u.png" CssClass="icon_display_size" runat="server" />
        <asp:ImageButton ID="imgAllocateVersion" ToolTip="Allocate a version" ImageUrl="images/spec_rel-a.png" CssClass="icon_display_size" runat="server" />
        <asp:ImageButton ID="imgInhibitPromote" ToolTip="Inhibit Promote" ImageUrl="images/spec_rel-i.png" CssClass="icon_display_size" runat="server" Visible="false" OnClick="imgInhibitPromote_Click" />
        <asp:ImageButton ID="imgRemoveInhibitPromote" ToolTip="Remove-Inhibit Promote" ImageUrl="images/spec_rel-ri.png" CssClass="icon_display_size" runat="server" Visible="false" OnClick="imgRemoveInhibitPromote_Click" />
        <asp:ImageButton ID="imgForceTransposition" ToolTip="Force transposition" ImageUrl="images/spec_rel-f.png" CssClass="icon_display_size" runat="server" OnClick="imgForceTransposition_Click" />
        <asp:ImageButton ID="imgUnforceTransposition" ToolTip="Unforce transposition" ImageUrl="images/spec_rel-f-crossed.png" CssClass="icon_display_size" runat="server" OnClick="imgUnforceTransposition_Click" />
        <asp:ImageButton ID="imgPromoteSpec" ToolTip="Promote specification to next Release" ImageUrl="images/spec_rel-p.png" CssClass="icon_display_size" runat="server" OnClick="imgPromoteSpec_Click" />
        <asp:ImageButton ID="imgDemoteSpec" ToolTip="Demote specification to previous Release" ImageUrl="images/spec_rel-d.png" CssClass="icon_display_size" runat="server" OnClick="imgDemoteSpec_Click"/>
        <asp:ImageButton ID="imgWithdrawSpec" ToolTip="Withdraw specification from Release" ImageUrl="images/spec_rel-w.png" CssClass="icon_display_size" runat="server" />
        <asp:ImageButton ID="imgUnWithdrawnSpec" ToolTip="Unwithdrawn specification from Release" ImageUrl="images/spec_rel-rw.png" CssClass="icon_display_size" runat="server" OnClick="imgUnWithdrawnSpec_Click" />
        <asp:ImageButton ID="imgDeleteSpecRelease" ImageUrl="images/delete.png" CssClass="icon_display_size_remove" runat="server"/>
    </asp:Panel>
    <telerik:RadGrid runat="server" ID="specificationsVersionGrid"
        AllowPaging="false"
        AllowSorting="false"
        AllowFilteringByColumn="false"
        AutoGenerateColumns="false"
        AllowMultiRowEdit="true"
        OnItemDataBound="specificationsVersionGrid_ItemDataBound" ViewStateMode="Disabled">
        <ClientSettings>
            <Scrolling AllowScroll="True" UseStaticHeaders="true" />
        </ClientSettings>
        <MasterTableView DataKeyNames="Pk_VersionId">
            <Columns>
                <telerik:GridTemplateColumn DataField="Meetings" HeaderText="Meetings" UniqueName="Meetings">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="100px" />
                    <ItemTemplate>
                        <div class="text-center">
                            <asp:HyperLink runat="server" ID="lnkMeetings" 
                                Target="_blank" 
                                Text='<%# string.IsNullOrEmpty((string)DataBinder.Eval(Container.DataItem,"MtgShortRef")) ? "-" : DataBinder.Eval(Container.DataItem,"MtgShortRef")%>' 
                                NavigateUrl='<%# (DataBinder.Eval(Container.DataItem,"Source") == null || int.Parse(DataBinder.Eval(Container.DataItem,"Source").ToString()) == 0) ? "" : ConfigVariables.MeetingDetailsAddress + DataBinder.Eval(Container.DataItem,"Source")%>'
                                ToolTip='<%# (DataBinder.Eval(Container.DataItem,"Source") == null || int.Parse(DataBinder.Eval(Container.DataItem,"Source").ToString()) == 0) ? "" : "Click to show meeting details"%> '/>
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Version" UniqueName="Version">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px" />
                    <ItemTemplate>
                        <div class="text-center">                            
                            <asp:HyperLink ID="lnkFtpDownload" runat="server" 
                                Text='<%# DataBinder.Eval(Container.DataItem,"Version")%>' 
                                NavigateUrl='<%# DataBinder.Eval(Container.DataItem,"Location")%>' 
                                ToolTip='<%# string.IsNullOrEmpty((string)DataBinder.Eval(Container.DataItem,"Location")) ? "" : "Click to download this version" %>'/>
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="DocumentUploaded" HeaderText="Upload date" UniqueName="DocumentUploaded">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="110px" />
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem,"DocumentUploaded", "{0:yyyy-MM-dd}") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="LatestRemark" HeaderText="Comment" UniqueName="LatestRemark">
                    <HeaderStyle HorizontalAlign="Center" Width="40%" Font-Bold="True" />
                    <ItemTemplate>
                        <div class="text-left">
                            <asp:Label ID="lblRemarkText" Text='<%# DataBinder.Eval(Container.DataItem,"LatestRemark") %>' runat="server" />
                            <asp:ImageButton ID="imgVersionDetails" ImageUrl="images/details.png" runat="server" ToolTip="See version details" CssClass="float_right"/>
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Link">
                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="100px" />
                    <ItemTemplate>
                        <a 
                            target="_blank" 
                            class="linkStyle <%# DataBinder.Eval(Container.DataItem, "TranspositionReferenceAndDate") == "" ? "disabled" : "" %>"
                            href="<%# DataBinder.Eval(Container.DataItem,"WIReportPath")%>"
                            <%# DataBinder.Eval(Container.DataItem, "TranspositionReferenceAndDate") == "" ? "disabled='disabled'" : "" %> 
                            >
                            <img border="0" 
                                src="images/spec_rel-tranSpec.png?v=<%=ConfigurationManager.AppSettings["AppVersion"] %>" 
                                class='linkStyle <%# DataBinder.Eval(Container.DataItem, "TranspositionReferenceAndDate") == "" ? "notAvailable" : ""%>'
                                title='<%# DataBinder.Eval(Container.DataItem,"TranspositionReferenceAndDate")%>' 
                                />
                        </a>
                        <asp:HyperLink ID="imgRelatedTDocs" runat="server" ImageUrl="images/spec_rel-TDocs.png" Target="_blank"/>
                        <asp:HyperLink ID="imgRelatedCRs" runat="server" ImageUrl="images/spec_rel-CRs.png" Target="_blank"/>
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