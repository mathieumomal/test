<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommunityHyperlinkControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.CommunityHyperlinkControl" %>
<asp:Repeater runat="server" ID="rptMultipleCommunities" Visible="False">
    <SeparatorTemplate><span>, </span></SeparatorTemplate>
    <ItemTemplate>
        <span runat="server" Visible='<%# DataBinder.Eval(Container.DataItem, "DetailsUrl") == null || string.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "DetailsUrl").ToString()) %>'>
            <span><%# DataBinder.Eval(Container.DataItem, "Name") %></span>
        </span>
        <span runat="server" Visible='<%# DataBinder.Eval(Container.DataItem, "DetailsUrl") != null && !string.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "DetailsUrl").ToString()) %>'>
            <a href="<%# DataBinder.Eval(Container.DataItem, "DetailsUrl") %>" target="_blank" title="<%# DataBinder.Eval(Container.DataItem, "Tooltip") %>"><%# DataBinder.Eval(Container.DataItem, "Name") %></a>
        </span>
    </ItemTemplate>
</asp:Repeater>

<!-- NO DATA -->
<asp:Label runat="server" ID="lblNoData" Visible="False"><%= NoDataValue %></asp:Label> 

<!-- ERROR -->
<asp:Label runat="server" ID="lblError" Visible="False" ToolTip="An unexpected error occured. Please contact helpdesk if problem persists.">ERROR</asp:Label> 