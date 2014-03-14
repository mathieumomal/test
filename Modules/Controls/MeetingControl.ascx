<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MeetingControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.MeetingControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script language="javascript" type="text/javascript">
    function OnClientSelectedIndexChanged<%=lblEndDate.ClientID%>(sender, eventArgs) {
        var item = eventArgs.get_item();
        var lbl = document.getElementById('<%=lblEndDate.ClientID%>');
        if (lbl != null && typeof(lbl) != "undefined")
            lbl.innerHTML = item.get_value().split("|")[1];
    }
</script>
<telerik:RadComboBox
    id="rcbMeetings"
    runat="server"
    width="200px" AllowCustomText="true"
    EnableLoadOnDemand="True"
    OnItemsRequested="rcbMeetings_ItemsRequested"
    onclientselectedindexchanged="OnClientSelectedIndexChanged"
    OnSelectedIndexChanged="rcbMeetings_SelectedIndexChanged" 
    OnDataBound="rcbMeetings_DataBound">  
</telerik:RadComboBox>

<asp:Panel Style="display:inline;font-weight:bold" ID="pnlEndDate" runat="server">
    &nbsp;&nbsp;
    <asp:Label runat="server" ID="lblEndDate" Text="-" />
</asp:Panel>

