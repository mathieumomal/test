<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RemarksControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RemarksControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<fieldset style="border: 1px solid grey; padding: 5px;">
    <legend>Remarks</legend>
    <table style="width: 100%">
        <tr>
            <td>
                <telerik:RadGrid runat="server" ID="releaseDetailTable" AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false">
                    <mastertableview clientdatakeynames="Pk_RemarkId">
        <Columns>
            <telerik:GridTemplateColumn HeaderStyle-Width="25%" DataField="CreationDate" HeaderText="Creation Date" UniqueName="CreationDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"CreationDate", "{0:yyyy-MM-dd hh:mm}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="25%" DataField="CreatedBy" HeaderText="Created By" UniqueName="CreatedBy">
                <ItemTemplate>
                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Fk_PersonId") %></div>  
                </ItemTemplate> 
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="50%" DataField="RemarkText" HeaderText="Remarks" UniqueName="RemarkText">
                <ItemTemplate>
                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"RemarkText") %></div>  
                </ItemTemplate>
<%--                <EditItemTemplate>
                    <div class="text-left"><asp:TextBox ID="txtRemark" runat="server" Text='<%# Eval("RemarkText") %>'></asp:TextBox></div>  
                </EditItemTemplate>   --%>                  
            </telerik:GridTemplateColumn>
        </Columns>
    </mastertableview>
                </telerik:RadGrid></td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtAddRemark" runat="server" Width="75%"></asp:TextBox>
                <asp:Button ID="btnAddRemark" runat="server" Text="Add" Width="75px" OnClick="btnAddRemark_Click" />
            </td>
        </tr>
    </table>
</fieldset>
