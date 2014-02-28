<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RemarksControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RemarksControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    .RadGrid_Default th.rgHeader
    {
        background-color: grey;
        border: none;
        border-bottom: 1px solid grey;
    }
</style>

<fieldset style="border: 1px solid grey; padding: 5px;">
    <legend>Remarks</legend>
    <table style="width: 100%">
        <tr>
            <td colspan="2">
                <telerik:RadGrid runat="server" ID="releaseDetailTable" AllowPaging="false" 
                                                                        AllowSorting="false" 
                                                                        AllowFilteringByColumn="false" 
                                                                        AutoGenerateColumns="false"
                                                                        style="min-width:400px">
<%--                    <clientsettings>
                         <Scrolling AllowScroll="true" UseStaticHeaders="true" />
                    </clientsettings>--%>
                    <MasterTableView clientdatakeynames="Pk_RemarkId" EditMode="InPlace">
                        <Columns>
                             <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn">
                             </telerik:GridEditCommandColumn>
                            <telerik:GridTemplateColumn DataField="CreationDate" HeaderText="Creation Date" UniqueName="CreationDate">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="125px"/> 
                                <ItemTemplate>
                                    <span><%# DataBinder.Eval(Container.DataItem,"CreationDate", "{0:yyyy-MM-dd hh:mm}") %></span>  
                                </ItemTemplate>                    
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="CreatedBy" HeaderText="Created By" UniqueName="CreatedBy">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="125px"/> 
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Fk_PersonId") %></div>  
                                </ItemTemplate> 
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="RemarkText" HeaderText="Remarks" UniqueName="RemarkText">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True"/>
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"RemarkText") %></div>  
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <div class="text-left"><asp:TextBox ID="txtRemark" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "RemarkText") %>'></asp:TextBox></div>  
                                </EditItemTemplate>                     
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="IsPublic" HeaderText="Remark Type" UniqueName="IsPublic">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="100px"/>
                                <EditItemTemplate>
                                    <div class="text-left">Remark Type</div>  
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
