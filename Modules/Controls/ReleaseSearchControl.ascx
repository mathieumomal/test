<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReleaseSearchControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.ReleaseSearchControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    .RadTreeView_Default .rtPlus, .RadTreeView_Default .rtMinus
    {
        display: none !important;
    }

    .RadTreeView .rtTop, .RadTreeView .rtMid, .RadTreeView .rtBot
    {
        padding: 1px 0 1px 0;
    }

    input.rcbCheckBox {
        display:none;
    }
</style>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        $(function () {
            $('[name$="$ReleaseGroup"]').attr("name", $('[name$="$ReleaseGroup"]').attr("name"));

            $('[name$="$ReleaseGroup"]').click(function () {
                //set name for all to name of clicked 
                $('[name$="$ReleaseGroup"]').attr("name", $(this).attr("name"));
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadComboBox ID="rcbReleases" runat="server" Width="200" DropDownWidth="200" CheckBoxes="true">
    <ItemTemplate>
        <telerik:RadTreeView ID="rtvReleases" runat="server" CheckBoxes="true" ShowLineImages="false">
            <Nodes>
                <telerik:RadTreeNode Checkable="false">
                    <NodeTemplate>
                        <asp:RadioButton ID="rbAllReleases" runat="server" GroupName="ReleaseGroup" Text="All Releases" />
                    </NodeTemplate>
                </telerik:RadTreeNode>
                <telerik:RadTreeNode Checkable="false">
                    <NodeTemplate>
                        <asp:RadioButton ID="rbOpenReleases" runat="server" GroupName="ReleaseGroup" Text="Open Releases" />
                    </NodeTemplate>
                </telerik:RadTreeNode>
                <telerik:RadTreeNode Checkable="false" Expanded="true" ExpandMode="ServerSide">
                    <NodeTemplate>
                        <asp:RadioButton ID="rbCustomSelection" runat="server" GroupName="ReleaseGroup" Text="Custom Selection" />
                    </NodeTemplate>
                    <Nodes>
                        <telerik:RadTreeNode Text="Rel - 1" />
                        <telerik:RadTreeNode Text="Rel - 2" />
                        <telerik:RadTreeNode Text="Rel - 3" />
                        <telerik:RadTreeNode Text="Rel - 4" />
                        <telerik:RadTreeNode Text="Rel - 5" />
                    </Nodes>
                </telerik:RadTreeNode>
            </Nodes>
        </telerik:RadTreeView>
    </ItemTemplate>
    <Items>
        <telerik:RadComboBoxItem />
    </Items>
</telerik:RadComboBox>