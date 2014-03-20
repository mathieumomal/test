<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReleaseSearchControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.ReleaseSearchControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    .RadTreeView_Default .rtPlus, .RadTreeView_Default .rtMinus
    {
        display: none !important;
    }

    .RadTreeView .rtTop, .RadTreeView .rtMid, .RadTreeView .rtBot
    {
        padding: 1px 0 1px 10px;
    }

    input.rcbCheckBox {
        display:none;
    }
</style>

<telerik:RadComboBox ID="rcbReleases" runat="server" CheckBoxes="true" AutoPostBack="false">
    <Itemtemplate>
        <table>
            <tr>
                <td>
                    <telerik:RadButton ID="rbAllReleases" ToggleType="Radio" runat="server" Text="All Releases" GroupName="ReleaseGroup" ButtonType="ToggleButton" AutoPostBack="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadButton ID="rbOpenReleases" ToggleType="Radio" runat="server" Text="Open Releases" GroupName="ReleaseGroup" ButtonType="ToggleButton" AutoPostBack="false" />
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
                            <telerik:RadButton ID="rbCustomReleases" ToggleType="CheckBox" runat="server" Text='<%# DataBinder.Eval(Container, "Text") %>' ButtonType="ToggleButton" AutoPostBack="false" />
                        </NodeTemplate>
                    </telerik:RadTreeView>
                </td>
            </tr>
        </table>
    </Itemtemplate>
    <Items>
        <telerik:RadComboBoxItem />
    </Items>
</telerik:RadComboBox>