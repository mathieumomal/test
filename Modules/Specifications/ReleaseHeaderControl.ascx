<%@ Control Language="C#" ClassName="ReleaseHeaderControl" AutoEventWireup="true" CodeBehind="ReleaseHeaderControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.ReleaseHeaderControl" %>
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

    function openRemarksPopup(remarksModule, remarksModulePrimaryKey, isEditMode, title) {
        var win = radopen("/desktopmodules/Specifications/RemarksPopup.aspx?remarksModule=" + remarksModule + "&remarksModulePrimaryKey=" + remarksModulePrimaryKey + "&isEditMode=" + isEditMode, "Remarks");
        var height = 200;
        if (isEditMode)
            height = height + 85;
        win.setSize(650, height);
        win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
        win.set_autoSize(true);
        win.set_autoSizeBehaviors(Telerik.Web.UI.WindowAutoSizeBehaviors.Height);
        win.set_modal(true);
        win.set_visibleStatusbar(false);
        win.set_title(title);
        win.show();
        return false;
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
