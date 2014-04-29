<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommunityControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.CommunityControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<script type="text/javascript">
    //Open Community Selector
    function openCommunitySelector<%= this.ClientID%>(sender, eventArgs) {
        var radWindowCommunity = $find("<%= rwCommunity.ClientID %>");
        radWindowCommunity.show();
    }

    //Close Community Selector
    function closeCommunitySelector<%= this.ClientID%>(sender, eventArgs) {
        var radWindowCommunity = $find("<%= rwCommunity.ClientID %>");
        radWindowCommunity.close();
    }

    //If tree node checked/unchecked, then update the parent & child records as follows
    //Node Checked   - Parent => Should be checked if all siblings checked
    //               - Childs => All children should be checked
    //Node UnChecked - Parent => Should be unchecked
    //               - Childs => All children should be unchecked
    function clientNodeChecked<%= this.ClientID%>(sender, eventArgs) {
        var childNodes = eventArgs.get_node().get_nodes();
        var isChecked = eventArgs.get_node().get_checked();
        UpdateAllChildren<%= this.ClientID%>(childNodes, isChecked);
        UpdateParent<%= this.ClientID%>(eventArgs.get_node(), isChecked);
    }

    //Node Checked   - All children should be checked
    //Node UnChecked - All children should be unchecked
    function UpdateAllChildren<%= this.ClientID%>(nodes, checked) {
        for (var i = 0; i < nodes.get_count() ; i++) {
            if (checked) {
                nodes.getNode(i).check();
            }
            else {
                nodes.getNode(i).set_checked(false);
            }

            if (nodes.getNode(i).get_nodes().get_count() > 0) {
                UpdateAllChildren<%= this.ClientID%>(nodes.getNode(i).get_nodes(), checked);
            }
        }
    }

    //Node Checked   - Parent should be checked if all siblings checked
    //Node UnChecked - Parent should be unchecked
    function UpdateParent<%= this.ClientID%>(node, checked) {
        if (node.get_parent() != node.get_treeView()) {
            if (checked) {
                var siblings = node.get_parent().get_nodes();
                var checkedCount = 0;
                for (var i = 0; i < siblings.get_count() ; i++) {
                    if (siblings.getNode(i).get_checked())
                        checkedCount++;
                }

                if (siblings.get_count() == checkedCount)
                    node.get_parent().check();
            }
            else {
                node.get_parent().set_checked(false);
            }
            UpdateParent<%= this.ClientID%>(node.get_parent(), checked);
        }
    }

    //Update all nodes with checked/unchecked
    function UpdateNodes<%= this.ClientID%>(checked) {
        var tree = $find("<%= rtvCommunitySelector.ClientID %>");
        for (var i = 0; i < tree.get_allNodes().length; i++) {
            tree.get_allNodes()[i].set_checked(checked);
        }
    }

    //Set Default TBs checked
    function SetDefaultItems<%= this.ClientID%>(sender, eventArgs) {
        UpdateNodes<%= this.ClientID%>(false);
        var defaultTBIds = $("#<%= defaultTbIds.ClientID %>").val();
        var tbIDsList = defaultTBIds.split(",");

        var tree = $find("<%= rtvCommunitySelector.ClientID %>");
        for (var i = 0; i < tree.get_allNodes().length; i++) {
            var node = tree.get_allNodes()[i];

            if (node.get_nodes().get_count() == 0) {
                var nodeValue = node.get_value();
                for (var j = 0; j < tbIDsList.length; j++) {
                    if (nodeValue == tbIDsList[j]) {
                        node.set_checked(true);
                        UpdateParent<%= this.ClientID%>(node, true);
                    }
                }
            }
        }
    }

</script>
<style>
    .hideOverflow {
        height: 25px;
        overflow: hidden;
    }
</style>
<div>
    <telerik:RadComboBox
        ID="rcbCommunity"
        runat="server"
        AllowCustomText="false" />
    <asp:Panel CssClass="hideOverflow" ID="pnlCover" runat="server">
        <asp:ImageButton ID="imgBtnCommunity" runat="server" ImageUrl="images/edit_16X16.png"/>
        <asp:Label ID="lblCommunity" runat="server" />
    </asp:Panel>
</div>

<telerik:RadWindowManager ID="rwmCommunity" runat="server">
    <Windows>
        <telerik:RadWindow ID="rwCommunity" runat="server" Width="400" Height="500" Behaviors="None" Modal="true" VisibleStatusbar="false" Title="Responsible group(s)">
            <ContentTemplate>
                <asp:UpdatePanel ID="upCommunityPanel" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <table style="width: 350px">
                            <tr>
                                <td>Select the Responsible group(s)</td>
                            </tr>
                            <tr>
                                <td>
                                    <telerik:RadTreeView ID="rtvCommunitySelector" runat="server" CheckBoxes="True">
                                        <DataBindings>
                                            <telerik:RadTreeNodeBinding Expanded="True"></telerik:RadTreeNodeBinding>
                                        </DataBindings>
                                    </telerik:RadTreeView>
                                </td>
                            </tr>
                            <tr style="height: 35px"></tr>
                        </table>
                        <div style="position: absolute; bottom: 0; height: 30px; width: 350px; padding-top: 5px; padding-left: 15px; margin-bottom: 8px; background-color: white;">
                            <telerik:RadButton ID="btnConfirm" runat="server" Text="Confirm" Width="60" OnClick="btnConfirm_Click"/>
                            <telerik:RadButton ID="btnAll" runat="server" Text="All" Width="60" AutoPostBack="false" />
                            <telerik:RadButton ID="btnDefault" runat="server" Text="Default" Width="60"  AutoPostBack="false"/>
                            <telerik:RadButton ID="btnClear" runat="server" Text="Clear" Width="60" AutoPostBack="false" />
                            <telerik:RadButton ID="btnCancel" runat="server" Text="Cancel" Width="60" AutoPostBack="false" />
                            <asp:HiddenField ID="defaultTbIds" runat="server" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>
