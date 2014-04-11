<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FullView.ascx.cs" Inherits="Etsi.Ultimate.Controls.FullView" %>
<style>
.FullViewLnk
{
    padding: 5px 12px;
    background:url("/controls/Ultimate/images/fullview.png");
    background-repeat:no-repeat;
    background-color:white;
    height:24px;
    width:24px;
}

</style>
<asp:HyperLink runat="server" Text="" ID="lnkFullView" Visible="false" Title="Full view" CssClass="FullViewLnk" Target="_blank">
</asp:HyperLink>