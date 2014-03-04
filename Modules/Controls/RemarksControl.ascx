<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RemarksControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RemarksControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    .RadGrid_Default th.rgHeader
    {
        background-color: grey;
        border: none;
        border-bottom: 1px solid grey;
    }

    .RadGrid_Default .rgEditRow td {
        border: none;
    }
</style>

<fieldset style="padding: 5px;">
    <legend>Remarks</legend>
    <table style="width: 100%">
        <tr>
            <td colspan="2">
                <telerik:RadGrid runat="server" ID="remarksGrid" AllowPaging="false" 
                                                                 AllowSorting="false" 
                                                                 AllowFilteringByColumn="false" 
                                                                 AutoGenerateColumns="false"
                                                                 AllowMultiRowEdit="true"
                                                                 OnPreRender="remarksGrid_PreRender"
                                                                 OnNeedDataSource="remarksGrid_NeedDataSource"
                                                                 OnItemDataBound="remarksGrid_ItemDataBound"
                                                                 style="min-width:400px">
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                    </ClientSettings>
                    <MasterTableView clientdatakeynames="Pk_RemarkId, IsPublic" EditMode="InPlace">
                        <Columns>
                            <telerik:GridTemplateColumn DataField="CreationDate" HeaderText="Creation Date" UniqueName="CreationDate">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px"/> 
                                <ItemTemplate>
                                    <span><%# DataBinder.Eval(Container.DataItem,"CreationDate", "{0:yyyy-MM-dd hh:mm UTC}") %></span>  
                                </ItemTemplate>                 
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="PersonName" HeaderText="Created By" UniqueName="CreatedBy">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px"/> 
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"PersonName") %></div>  
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="RemarkText" HeaderText="Remarks" UniqueName="RemarkText">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True"/>
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"RemarkText") %></div>  
                                </ItemTemplate>                
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="IsPublic" HeaderText="Remark Type" UniqueName="IsPublic">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="100px"/>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>  
                                    <asp:Label runat="server" ID="lblRemarkType" Text='<%# Convert.ToBoolean(Eval("IsPublic")) == true ? "Public" : "Private" %>'>></asp:Label>                  
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="rddlRemarkType" Width="90px" DataValueField="IsPublic" AutoPostBack="true" OnSelectedIndexChanged="rddlRemarkType_SelectedIndexChanged" SelectedValue='<%#Bind("IsPublic") %>'>
                                        <Items>
                                            <telerik:DropDownListItem Text="Public" Value="True"/>  
                                            <telerik:DropDownListItem Text="Private" Value="False"/>                                  
                                            </Items>
                                        </telerik:RadDropDownList>                          
                                </EditItemTemplate>                   
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            <div style="text-align:center">
                                No Remarks Added
                            </div>
                        </NoRecordsTemplate>
                    </MasterTableView>
                </telerik:RadGrid></td>
        </tr>
        <tr>
            <td style="padding-right:5px">
                <asp:TextBox ID="txtAddRemark" runat="server" Width="100%"></asp:TextBox>
                </td>
            <td>
                <asp:Button ID="btnAddRemark" runat="server" Text="Add" Width="100%" OnClick="btnAddRemark_Click" />
            </td>
        </tr>
    </table>
</fieldset>
