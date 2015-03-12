<%@ Control Language="C#" ClassName="SpecificationReleaseControl" AutoEventWireup="true" CodeBehind="SpecificationReleaseControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationReleaseControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="spec" TagName="SpecificationVersionListControl" Src="SpecificationVersionListControl.ascx" %>

<asp:Panel CssClass="TabContent" ID="ReleasesSpecificationsPanel" runat="server">
    <telerik:RadPanelBar runat="server" ID="rpbReleases" Width="100%" ExpandMode="SingleExpandedItem">
    </telerik:RadPanelBar>
</asp:Panel>
