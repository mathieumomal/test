<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FullView.ascx.cs" Inherits="Etsi.Ultimate.Controls.FullView" %>
<style>
.FullViewLnk
{
	background-position: 0% 0%;
    text-decoration: none;
    padding: 2px 15px;
    color: #FFF;
    -webkit-border-radius: 4px;
    -moz-border-radius: 4px;
    border-radius: 4px;
    border: solid 1px #20538D;
    text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.4);
    /*-webkit-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);
    -moz-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);*/

    vertical-align: middle;
	-webkit-transition-duration: 0.2s;
    -moz-transition-duration: 0.2s;
    transition-duration: 0.2s;
    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;

/* IE6-9 */

    background-color: #75B91A;
    background-repeat: repeat;
    background-attachment: scroll;
}

</style>
<asp:HyperLink runat="server" Text="Full view" ID="lnkFullView" Visible="false" CssClass="FullViewLnk">
</asp:HyperLink>

