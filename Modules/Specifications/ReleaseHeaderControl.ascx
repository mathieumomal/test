<%@ Control Language="C#" ClassName="ReleaseHeaderControl" AutoEventWireup="true" CodeBehind="ReleaseHeaderControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.ReleaseHeaderControl" %>

<table class="tblReleaseHeader" style="width: 100%; vertical-align: middle;">
    <tr>
        <td style="width: 50%; text-align: left">
            <asp:Label ID="lblReleaseName" Font-Bold="true" runat="server" /><asp:Label ID="lblStatus" runat="server" />
        </td>
        <td class="remark">
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
