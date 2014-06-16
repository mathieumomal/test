﻿<%@ Control Language="C#" ClassName="ReleaseHeaderControl" AutoEventWireup="true" CodeBehind="ReleaseHeaderControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.ReleaseHeaderControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>

<style>
    .button_position {
        height: 16px;
        width: 16px;
        vertical-align: bottom;
    }

    .hideOverflow {
        height: 25px;
        overflow: hidden;
        display: inline;
    }
</style>

<script type="text/javascript">

    function OpenReleaseHeaderRemarksWindow<%= this.ClientID %>() {
        var radWindowRemarks = $find("<%= rwReleaseHeaderRemarks.ClientID %>");
        radWindowRemarks.show();
    }

</script>
<table id="tblReleaseHeader" style="width: 100%; vertical-align: middle;">
    <tr>
        <td style="width: 50%; text-align: left">
            <asp:Label ID="lblReleaseName" Font-Bold="true" runat="server" /><asp:Label ID="lblStatus" runat="server" />
        </td>
        <td style="text-align: left">
            <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Latest Remark:" />
            <asp:Panel CssClass="hideOverflow" ID="pnlCover" runat="server">
                <asp:Label ID="lblLatestRemark" runat="server" />
            </asp:Panel>
        </td>
        <td style="width: 20px;">
            <asp:ImageButton ID="imgRemarks" ToolTip="Remarks" ImageUrl="images/spec_rel-remarks.png" CssClass="button_position" runat="server" />
        </td>
    </tr>
</table>
<telerik:RadWindowManager ID="rwmSpecReleaseHeader" runat="server">
    <Windows>
        <telerik:RadWindow ID="rwReleaseHeaderRemarks" runat="server" Modal="true" Title="Remarks" Width="550" Height="230" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                    <asp:UpdatePanel ID="upHeaderRemarks" runat="server">
                        <ContentTemplate>
                            <ult:remarkscontrol runat="server" id="releaseRemarks" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>
