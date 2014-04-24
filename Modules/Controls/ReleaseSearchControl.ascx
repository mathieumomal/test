<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReleaseSearchControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.ReleaseSearchControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    <%= rcbReleases.ClientID %> .RadTreeView_Default .rtPlus, <%= rcbReleases.ClientID %> .RadTreeView_Default .rtMinus
    {
        display: none !important;
    }

    <%= rcbReleases.ClientID %> .RadTreeView .rtTop, <%= rcbReleases.ClientID %> .RadTreeView .rtMid, <%= rcbReleases.ClientID %> .RadTreeView .rtBot
    {
        padding: 1px 0 1px 10px;
    }

</style>

<script type="text/javascript">

    var resetToAllReleases = false;

    function ResetCheckBoxes<%= this.ClientID%>(sender, args) {
        if (sender.get_checked()) {
            var comboBox = $find("<%= rcbReleases.ClientID %>");
            var rtvReleases = comboBox.get_items().getItem(0).findControl("rtvReleases");
            for (var i = 0; i < rtvReleases.get_nodes().get_count() ; i++) {
                var node = rtvReleases.get_nodes().getNode(i);
                var rbCustomReleases = node.findControl("rbCustomReleases");
                if (rbCustomReleases.get_checked())
                    rbCustomReleases.set_checked(false);
            }
        }
    }

    function ResetToAllReleases<%= this.ClientID%>() {
        resetToAllReleases = true;
    }

    function ResetRadioButtons<%= this.ClientID%>(sender, args) {
        if (sender.get_checked()) {
            var comboBox = $find("<%= rcbReleases.ClientID %>");
            var rbCustomSelection = comboBox.get_items().getItem(0).findControl("rbCustomSelection");
            if (!rbCustomSelection.get_checked())
                rbCustomSelection.set_checked(true);
        }
    }

    function OnClientDropDownClosed<%= this.ClientID%>(sender, eventArgs) {
        SetComboBoxText<%= this.ClientID%>();
    }

    function OnClientDropDownOpened<%= this.ClientID%>(sender, eventArgs) {
        console.log("called");
        $('#<%= rcbReleases.ClientID %>_DropDown .rcbCheckBox').hide();
    }

    function OnClientLoad<%= this.ClientID%>(sender) {
        if (resetToAllReleases) {
            var comboBox = $find("<%= rcbReleases.ClientID %>");
            var allReleases = comboBox.get_items().getItem(0).findControl("rbAllReleases");
            allReleases.set_checked(false);
            var openReleases = comboBox.get_items().getItem(0).findControl("rbOpenReleases");
            openReleases.set_checked(true);
            var customSelectin = comboBox.get_items().getItem(0).findControl("rbCustomSelection");
            customSelectin.set_checked(false);

            var rtvReleases = comboBox.get_items().getItem(0).findControl("rtvReleases");
            for (var i = 0; i < rtvReleases.get_nodes().get_count() ; i++) {
                var node = rtvReleases.get_nodes().getNode(i);
                var rbCustomReleases = node.findControl("rbCustomReleases");
                if (rbCustomReleases.get_checked())
                    rbCustomReleases.set_checked(false);
            }
            resetToAllReleases = false;
        }
        SetComboBoxText<%= this.ClientID%>();
    }

    function SetComboBoxText<%= this.ClientID%>() {
        var comboBox = $find("<%= rcbReleases.ClientID %>");
        var allReleases = comboBox.get_items().getItem(0).findControl("rbAllReleases");
        if (allReleases.get_checked()) {
            comboBox.set_text(allReleases.get_text());
        }
        else {
            var openReleases = comboBox.get_items().getItem(0).findControl("rbOpenReleases");
            if (openReleases.get_checked()) {
                comboBox.set_text(openReleases.get_text());
            }
            else {
                var rtvReleases = comboBox.get_items().getItem(0).findControl("rtvReleases");
                var customRelease = "";
                for (var i = 0; i < rtvReleases.get_nodes().get_count() ; i++) {
                    var node = rtvReleases.get_nodes().getNode(i);
                    var rbCustomReleases = node.findControl("rbCustomReleases");
                    if (rbCustomReleases.get_checked())
                        customRelease = customRelease + node.get_text() + ", ";
                }
                if (customRelease.length > 1) {
                    comboBox.set_text(customRelease.substring(0, customRelease.length - 2));
                }
                else
                    comboBox.set_text("Custom Selection");
            }
        }
    }
</script>

<telerik:RadComboBox ID="rcbReleases" runat="server" CheckBoxes="true" AutoPostBack="false" >
    <ItemTemplate>
        <table>
            <tr>
                <td>
                    <telerik:RadButton ID="rbAllReleases" ToggleType="Radio" runat="server" Text="All Releases" GroupName="ReleaseGroup" ButtonType="ToggleButton" AutoPostBack="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadButton ID="rbOpenReleases" ToggleType="Radio" runat="server" Text="Open Releases" GroupName="ReleaseGroup" ButtonType="ToggleButton" AutoPostBack="false"  />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadButton ID="rbCustomSelection" ToggleType="Radio" runat="server" Text="Custom Selection" GroupName="ReleaseGroup" ButtonType="ToggleButton" AutoPostBack="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadTreeView ID="rtvReleases" runat="server" CheckBoxes="true" ShowLineImages="false">
                        <NodeTemplate>
                            <telerik:RadButton ID="rbCustomReleases" ToggleType="CheckBox" runat="server" Text='<%# DataBinder.Eval(Container, "Text") %>' ButtonType="ToggleButton" AutoPostBack="false"/>
                        </NodeTemplate>
                    </telerik:RadTreeView>
                </td>
            </tr>
        </table>
    </ItemTemplate>
    <Items>
        <telerik:RadComboBoxItem />
    </Items>
</telerik:RadComboBox>
