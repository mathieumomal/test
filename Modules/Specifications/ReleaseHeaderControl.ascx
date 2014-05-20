<%@ Control Language="C#" ClassName="ReleaseHeaderControl" AutoEventWireup="true" CodeBehind="ReleaseHeaderControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.ReleaseHeaderControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<table style="width: 100%; vertical-align: middle;">
    <tr>
        <td style="width: 50%; text-align: left">
            <asp:Label ID="lblReleaseName" Font-Bold="true" runat="server" /><asp:Label ID="lblStatus" runat="server" />
        </td>
        <td style="text-align: left">
            <b>Latest Remark: </b>
            <asp:Label ID="lblLatestRemark" runat="server" />
        </td>
        <td style="width: 20px;">
            [::]
        </td>
        <td style="width: 20px;">
            <a class="rpExpandable">
                <span class="rpExpandHandle"></span>
            </a>
        </td>
    </tr>
</table>
