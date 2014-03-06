<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.HistoryControl" %>
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

<telerik:RadGrid runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" ID="historyTable" AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false" >
    <ClientSettings>
       <Scrolling AllowScroll="True" UseStaticHeaders="true" ScrollHeight="510px"/>
    </ClientSettings>
    <mastertableview clientdatakeynames="Pk_HistoryId">
        <Columns>
            <telerik:GridTemplateColumn DataField="CreationDate" HeaderText="Action date" UniqueName="CreationDate">
                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="140px"/> 
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"CreationDate", "{0:yyyy-mm-dd hh:mm UTC}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn DataField="HistoryText" HeaderText="Action" UniqueName="HistoryText">
                <HeaderStyle HorizontalAlign="Center" Font-Bold="True"/> 
                <ItemTemplate>
                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"HistoryText") %></div>  
                </ItemTemplate> 
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn DataField="PersonName" HeaderText="Author" UniqueName="PersonName">
                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="140px"/> 
                <ItemTemplate>
                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"PersonName") %></div>  
                </ItemTemplate> 
            </telerik:GridTemplateColumn>
        </Columns>
    </mastertableview>
</telerik:RadGrid>
