<%@ Control Language="C#" ClassName="SpecificationReleaseControl" AutoEventWireup="true" CodeBehind="SpecificationReleaseControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationReleaseControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Panel CssClass="TabContent pnlReleases" ID="ReleasesSpecificationsPanel" runat="server">
    <telerik:RadPanelBar runat="server" ID="rpbReleases" Width="100%" ExpandMode="MultipleExpandedItems" OnClientItemClicking="OnClientItemClicking">
    </telerik:RadPanelBar>
</asp:Panel>
<script>
    function OnClientItemClicking(sender, args) {
        console.log(args.get_domEvent().target.type);
        console.log(args.get_domEvent().target);
        if (args.get_domEvent().target.type == "text") {
            args.set_cancel(true);
        }
    }
</script>