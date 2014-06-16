<%@ Control Language="C#" ClassName="VersionRemarksControl" AutoEventWireup="true" CodeBehind="VersionRemarksControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.VersionRemarksControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>

<script type="text/javascript">

    function OpenVersionRemarksWindow<%= this.ClientID %>() {
        var radWindowRemarks = $find("<%= rwReleaseContentRemarks.ClientID %>");
        radWindowRemarks.show();
    }

</script>

<asp:ImageButton ID="imgVersionRemarks" ImageUrl="images/spec_rel-remarks.png" runat="server" CssClass="float_right grid_btn" />

<telerik:RadWindowManager ID="rwmSpecReleaseContent" runat="server">
    <Windows>
        <telerik:RadWindow ID="rwReleaseContentRemarks" runat="server" Modal="true" Title="Remarks" Width="550" Height="230" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                    <asp:UpdatePanel ID="upContentRemarks" runat="server">
                        <ContentTemplate>
                            <ult:remarkscontrol runat="server" id="versionRemarks" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>
