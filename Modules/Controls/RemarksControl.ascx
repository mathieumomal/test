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

<fieldset style="border: 1px solid grey; padding: 5px;">
    <legend>Remarks</legend>
    <table style="width: 100%">
        <tr>
            <td colspan="2">
                <telerik:RadGrid runat="server" ID="releaseDetailGrid"  AllowPaging="false" 
                                                                        AllowSorting="false" 
                                                                        AllowFilteringByColumn="false" 
                                                                        AutoGenerateColumns="false"
                                                                        OnNeedDataSource="releaseDetailGrid_NeedDataSource"
                                                                        OnUpdateCommand="releaseDetailGrid_UpdateCommand"
                                                                        style="min-width:400px">
                    <MasterTableView clientdatakeynames="Pk_RemarkId" EditMode="InPlace">
                        <Columns>
                            <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" ButtonType="ImageButton">
                            </telerik:GridEditCommandColumn>
                            <telerik:GridTemplateColumn DataField="CreationDate" HeaderText="Creation Date" UniqueName="CreationDate">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px"/> 
                                <ItemTemplate>
                                    <span><%# DataBinder.Eval(Container.DataItem,"CreationDate", "{0:yyyy-MM-dd hh:mm}") %></span>  
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
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtRemarkText" Width="100%" Text='<%# Bind("RemarkText") %>'></asp:TextBox>
                                </EditItemTemplate>                     
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="IsPublic" HeaderText="Remark Type" UniqueName="IsPublic">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="90px"/>
                                <ItemTemplate>  
                                    <asp:Label runat="server" ID="lblRemarkType" Text='<%# Convert.ToBoolean(Eval("IsPublic")) == true ? "Public" : "Private" %>'>></asp:Label>                  
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadDropDownList runat="server" ID="rddlRemarkType" Width="90px" DataValueField="IsPublic" SelectedValue='<%#Bind("IsPublic") %>'>
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
                        <EditFormSettings>
                          <EditColumn UniqueName="EditCommandColumn" ButtonType="ImageButton" />
                        </EditFormSettings>
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
