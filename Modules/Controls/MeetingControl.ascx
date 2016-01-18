<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MeetingControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.MeetingControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script language="javascript" type="text/javascript">
    function OnClientSelectedIndexChanged<%=lblEndDate.ClientID%>(sender, eventArgs) {
        var item = eventArgs.get_item();
        var lbl = document.getElementById('<%=lblEndDate.ClientID%>');
        if (lbl != null && typeof(lbl) != "undefined")
            lbl.innerHTML = item.get_value().split("|")[1];
    }
    function OnClientTextChanged<%=lblEndDate.ClientID%>(sender, eventArgs) {
        var text = sender.get_text();

        var combo = $find("<%= rcbMeetings.ClientID %>");
        var item = combo.findItemByText(" ");
        if (item) {
            item.select();
        }

    
    }
</script>
<telerik:RadComboBox
    id="rcbMeetings"
    runat="server"
    width="320px" AllowCustomText="true"
    EnableLoadOnDemand="True"
    OnItemsRequested="rcbMeetings_ItemsRequested"
    onclientselectedindexchanged="OnClientSelectedIndexChanged"
    OnSelectedIndexChanged="rcbMeetings_SelectedIndexChanged"
    OnClientTextChange="OnClientTextChanged" 
    OnDataBound="rcbMeetings_DataBound"
    MarkFirstMatch="True"
>  
</telerik:RadComboBox>

<asp:Panel Style="display:inline;font-weight:bold" ID="pnlEndDate" runat="server">
    &nbsp;&nbsp;
    <asp:Label runat="server" ID="lblEndDate" Text="-" />
</asp:Panel>

