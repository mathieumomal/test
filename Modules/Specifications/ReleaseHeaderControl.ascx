<%@ Control Language="C#" ClassName="ReleaseHeaderControl" AutoEventWireup="true" CodeBehind="ReleaseHeaderControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.ReleaseHeaderControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style>
    .button_position {
        height: 16px;
        width: 16px;
        vertical-align: bottom;
    }
</style>
<table id="tblReleaseHeader" style="width: 100%; vertical-align: middle;">
    <tr>

        <td style="width: 50%; text-align: left">
            <asp:Label ID="lblReleaseName" Font-Bold="true" runat="server" /><asp:Label ID="lblStatus" runat="server" />
        </td>
        <td style="text-align: left">
            <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Latest Remark:" />
            <asp:Label ID="lblLatestRemark" runat="server" />
        </td>
        <td style="width: 20px;">
            <asp:ImageButton ID="imgRemarks" ToolTip="Remarks" ImageUrl="images/spec_rel-remarks.png" OnClientClick="return false;" CssClass="button_position" runat="server" />
        </td>
        <td style="width: 20px;">
            <a class="rpExpandable">
                <span class="rpExpandHandle"></span>
            </a>
        </td>
    </tr>
</table>
