<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedWiControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RelatedWiControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    .RadGrid_Default th.rgHeader {
        background-color: grey;
        border: none;
        border-bottom: 1px solid grey;
    }

    .RadGrid_Default .rgEditRow td {
        border: none;
    }
    #<%=relatedWiGrid.ClientID %>.RadGrid_Default .rgSelectedRow.rgRow,
    #<%=relatedWiGrid_Edit.ClientID %>.RadGrid_Default .rgSelectedRow.rgRow {
        font-weight:bold;
        background: transparent;
        color: #000;
    }
    #<%=relatedWiGrid.ClientID %>.RadGrid_Default .rgSelectedRow.rgAltRow,
    #<%=relatedWiGrid_Edit.ClientID %>.RadGrid_Default .rgSelectedRow.rgAltRow {
        font-weight:bold;
        background: #f2f2f2;
        color: #000;
    }
    #<%=relatedWiGrid.ClientID %>.RadGrid_Default .rgSelectedRow td,
    #<%=relatedWiGrid_Edit.ClientID %>.RadGrid_Default .rgSelectedRow td {
        border-bottom-color: transparent;
    }
</style>
<script type="text/javascript">
    function closeAllModals() {
        var manager = GetRadWindowManager();
        manager.closeAll();
    }

    function open_RadWindow_workItemEdit(sender, eventArgs) {
        closeAllModals();
        window.radopen(null, "RadWindow_wiEdit");
        $("#<%=txtSearchText.ClientID %>").val('');
    }

    function GetSelectedWis_relatedWiGrid() {
        var selectedWis = $("#<%=hidSelectedWis.ClientID %>").val();

        var grid = $find("<%=relatedWiGrid_Search.ClientID %>");
        var MasterTable = grid.get_masterTableView();
        var selectedRows = MasterTable.get_selectedItems();
        for (var i = 0; i < selectedRows.length; i++) {
            var row = selectedRows[i];
            var cell = MasterTable.getCellByColumnUniqueName(row, "UID")
            selectedWis = selectedWis + cell.innerHTML + ",";
        }

        if (selectedWis.length > 0)
            $("#<%=hidSelectedWis.ClientID %>").val(selectedWis);
    }

    function OnRowSelected_relatedWiGrid_Edit(sender, eventArgs) {
        var MasterTable = eventArgs._tableView;
        var selectedRows = MasterTable.get_selectedItems();

        if (selectedRows.length == 0) {
            $("#<%=hidPrimaryWi.ClientID %>").val("-1");
        }
        else {
            for (var i = 0; i < selectedRows.length; i++) {
                var row = selectedRows[i];
                var cell = MasterTable.getCellByColumnUniqueName(row, "UID")
                $("#<%=hidPrimaryWi.ClientID %>").val(cell.innerHTML);
            }
        }
    }

    function setSearchProgress(flag) {
        if (flag)
            $("#<%=btnSearchWi.ClientID %>").val('Searching...');
    else
        $("#<%=btnSearchWi.ClientID %>").val('Search');
}

function setDeleteProgress(sender) {
    var gifImg = "/controls/Ultimate/images/busy.gif";
    sender.src = gifImg;
}
</script>

<telerik:RadAjaxManagerProxy ID="wiRadAjaxManagerProxy" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="btnSearchWi">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Search" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnAddWisToGrid">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Search" />
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Edit" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnAddToRelatedWiGrid">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Search" />
                <telerik:AjaxUpdatedControl ControlID="txtSearchText" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnRevertChanges">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Search" />
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Edit" />
                <telerik:AjaxUpdatedControl ControlID="txtSearchText" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnRemoveWis">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Edit" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnShowWiEditWindow">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="relatedWiGrid_Edit" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

<asp:UpdatePanel ID="upRelatedWis" runat="server">
    <ContentTemplate>
        <fieldset style="padding: 5px;">
            <legend>
                <asp:Label runat="server" ID="legendLabel"></asp:Label>
            </legend>
            <table style="width: 100%">
                <tr>
                    <td colspan="2">
                        <telerik:RadGrid runat="server" ID="relatedWiGrid" AllowPaging="false"
                            AllowSorting="false"
                            AllowFilteringByColumn="false"
                            AutoGenerateColumns="false"
                            OnNeedDataSource="relatedWiGrid_NeedDataSource"
                            OnItemDataBound="relatedWiGrid_ItemDataBound"
                            Style="min-width: 400px">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                            </ClientSettings>
                            <MasterTableView ClientDataKeyNames="UID" EditMode="InPlace">
                                <Columns>
                                    <telerik:GridTemplateColumn DataField="Select" HeaderText="Select" UniqueName="Select" Visible="false">
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="40px" />
                                        <ItemTemplate>
                                            <span><%# DataBinder.Eval(Container.DataItem,"UID") %></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="UID" HeaderText="UID" UniqueName="UID">
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="20%" />
                                        <ItemTemplate>
                                            <span><%# DataBinder.Eval(Container.DataItem,"UID") %></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="Acronym" HeaderText="Acronym" UniqueName="Acronym">
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="20%" />
                                        <ItemTemplate>
                                            <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Acronym") %></div>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="Name" HeaderText="Title" UniqueName="Name">
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="40%" />
                                        <ItemTemplate>
                                            <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Name") %></div>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="ResponsibleGroups" HeaderText="Responsible" UniqueName="ResponsibleGroups">
                                        <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="20%" />
                                        <ItemTemplate>
                                            <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"ResponsibleGroups") %></div>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="ViewWorkItem">
                                        <HeaderStyle Width="30px" />
                                        <ItemTemplate>
                                            <span></span>
                                            <img id="imgViewWorkItems" alt="See details" src="/DesktopModules/WorkItem/images/details.png" style='display: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsLevel0Record")) ? "none" : "block" %>'
                                                onclick="var popUp=window.open('/desktopmodules/WorkItem/WorkItemDetails.aspx?workitemId=<%# DataBinder.Eval(Container.DataItem,"Pk_WorkItemUid").ToString() %>',
								            'RelWi-<%# DataBinder.Eval(Container.DataItem,"Pk_WorkItemUid").ToString() %>', 'height=550,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn Display="false" DataField="IsPrimary" UniqueName="IsPrimary">
                                    </telerik:GridBoundColumn>
                                </Columns>
                                <NoRecordsTemplate>
                                    <div style="text-align: center">
                                        No related Work Items
                                    </div>
                                </NoRecordsTemplate>
                            </MasterTableView>
                        </telerik:RadGrid></td>
                </tr>
                <tr>
                    <td colspan="2" style="padding-right: 5px; float: right">
                        <asp:Button ID="btnShowWiEditWindow" runat="server" Text="Add" OnClientClick="open_RadWindow_workItemEdit()" />
                        <asp:HiddenField runat="server" ID="hidSelectedWis" />
                        <asp:HiddenField runat="server" ID="hidPrimaryWi" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </ContentTemplate>
</asp:UpdatePanel>
<telerik:RadWindowManager ID="relatedWi_RadWindowManager" runat="server">
    <Windows>
        <telerik:RadWindow ID="RadWindow_wiEdit" runat="server" Modal="true" Title="Related Work Items" Width="550" Height="525" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                <div>
                    <fieldset style="padding: 5px;">
                        <legend>Assigned Work Items
                        </legend>
                        <table style="width: 100%">
                            <tr>
                                <td colspan="2">
                                    <telerik:RadGrid runat="server" ID="relatedWiGrid_Edit" AllowPaging="false"
                                        AllowSorting="false"
                                        AllowFilteringByColumn="false"
                                        AutoGenerateColumns="false"
                                        OnItemDataBound="relatedWiGrid_Edit_ItemDataBound"
                                        Style="min-width: 400px">
                                        <ClientSettings>
                                            <ClientEvents OnRowDeselected="OnRowSelected_relatedWiGrid_Edit" OnRowSelected="OnRowSelected_relatedWiGrid_Edit" />
                                            <Scrolling AllowScroll="True" UseStaticHeaders="true" ScrollHeight="130px" />
                                            <Selecting AllowRowSelect="True" />
                                        </ClientSettings>
                                        <MasterTableView ClientDataKeyNames="UID" EditMode="InPlace">
                                            <Columns>
                                                <telerik:GridClientSelectColumn HeaderText="Prime WI" UniqueName="PrimeWI">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="75px" />
                                                </telerik:GridClientSelectColumn>
                                                <telerik:GridBoundColumn DataField="UID" HeaderText="UID" UniqueName="UID">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px" />
                                                </telerik:GridBoundColumn>
                                                <telerik:GridTemplateColumn DataField="Acronym" HeaderText="Acronym" UniqueName="Acronym">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px" />
                                                    <ItemTemplate>
                                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Acronym") %></div>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn DataField="Name" HeaderText="Title" UniqueName="Name">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" />
                                                    <ItemTemplate>
                                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Name") %></div>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn DataField="ResponsibleGroups" HeaderText="Responsible" UniqueName="ResponsibleGroups">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="80px" />
                                                    <ItemTemplate>
                                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"ResponsibleGroups") %></div>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Delete">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px" />
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnRemoveWis" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"Pk_WorkItemUid") %>' ImageUrl="/controls/Ultimate/images/delete.png" runat="server" OnClientClick="setDeleteProgress(this)" OnClick="btnRemoveWis_Click" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridBoundColumn Display="false" DataField="IsPrimary" UniqueName="IsPrimary">
                                                </telerik:GridBoundColumn>
                                            </Columns>
                                            <NoRecordsTemplate>
                                                <div style="text-align: center">
                                                    No related Work Items
                                                </div>
                                            </NoRecordsTemplate>
                                        </MasterTableView>
                                    </telerik:RadGrid></td>
                            </tr>
                        </table>
                    </fieldset>

                    <fieldset style="padding: 5px;">
                        <legend>Find Work Items
                        </legend>
                        <table style="width: 100%">
                            <tr>
                                <td style="padding-right: 5px">Search Work Items<asp:TextBox Style="width: 250px; margin: 0px 0px 0px 10px;" ID="txtSearchText" runat="server" Width="50%"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="btnSearchWi" runat="server" Text="Search" OnClientClick="setSearchProgress(true)" OnClick="btnSearchWi_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <telerik:RadGrid runat="server" ID="relatedWiGrid_Search" AllowPaging="false"
                                        AllowSorting="false"
                                        AllowFilteringByColumn="false"
                                        AutoGenerateColumns="false"
                                        AllowMultiRowSelection="True"
                                        Style="min-width: 400px">
                                        <ClientSettings>
                                            <ClientEvents OnDataBound="setSearchProgress(false)" />
                                            <Scrolling AllowScroll="True" UseStaticHeaders="true" ScrollHeight="140px" />
                                            <Selecting AllowRowSelect="True" />
                                        </ClientSettings>
                                        <MasterTableView ClientDataKeyNames="UID">
                                            <Columns>
                                                <telerik:GridClientSelectColumn UniqueName="MyClientSelectColumn">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="30px" />
                                                </telerik:GridClientSelectColumn>
                                                <telerik:GridBoundColumn DataField="UID" HeaderText="UID" UniqueName="UID">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px" />
                                                </telerik:GridBoundColumn>
                                                <telerik:GridTemplateColumn DataField="Acronym" HeaderText="Acronym" UniqueName="Acronym">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px" />
                                                    <ItemTemplate>
                                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Acronym") %></div>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn DataField="Name" HeaderText="Title" UniqueName="Name">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50%" />
                                                    <ItemTemplate>
                                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Name") %></div>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn DataField="ResponsibleGroups" HeaderText="Responsible" UniqueName="ResponsibleGroups">
                                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" />
                                                    <ItemTemplate>
                                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"ResponsibleGroups") %></div>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                            <NoRecordsTemplate>
                                                <div style="text-align: center">
                                                    No related Work Items
                                                </div>
                                            </NoRecordsTemplate>
                                        </MasterTableView>
                                    </telerik:RadGrid>

                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div style="float: right">
                                        <asp:Button ID="btnAddWisToGrid" runat="server" Text="Add" OnClientClick="GetSelectedWis_relatedWiGrid();" OnClick="btnAddWisToGrid_Click" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </fieldset>

                    <asp:Button ID="btnAddToRelatedWiGrid" runat="server" Text="Done" OnClientClick="closeAllModals();" OnClick="btnAddToRelatedWiGrid_Click" />
                    <asp:Button ID="btnRevertChanges" runat="server" Text="Cancel" OnClientClick="closeAllModals();" OnClick="btnRevertChanges_Click" />
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>
