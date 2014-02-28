<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.HistoryControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<telerik:RadGrid runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" ID="historyTable" AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false">
    <mastertableview clientdatakeynames="Pk_HistoryId">
        <Columns>
            <telerik:GridTemplateColumn HeaderStyle-Width="25%" DataField="CreatedBy" HeaderText="Created By" UniqueName="CreatedBy">
                <ItemTemplate>
                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Fk_PersonId") %></div>  
                </ItemTemplate> 
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="25%" DataField="CreationDate" HeaderText="Creation Date" UniqueName="CreationDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"CreationDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="50%" DataField="HistoryText" HeaderText="History Text" UniqueName="HistoryText">
                <ItemTemplate>
                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"HistoryText") %></div>  
                </ItemTemplate> 
            </telerik:GridTemplateColumn>
        </Columns>
    </mastertableview>
</telerik:RadGrid>
