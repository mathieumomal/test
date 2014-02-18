<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/Release/css/release.css" />
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.Release.View" %>
<%@ Import Namespace="System.Drawing" %>
<style>
    #releasesTable {
    border: solid 2px black;
    width: 100%;
    }
    .status{
        font-weight: bold;
    }
    .open{
        color : rgb(93, 175, 52);
    }
    .frozen{
        color : rgb(39, 116, 0);
    }
    .closed{
        color : rgb(128, 4, 4);
    }
</style>

<h1>---RELEASE MODULE---</h1>
<telerik:RadButton runat="server" ID="newRelease" Text="New"></telerik:RadButton>

 <telerik:RadGrid runat="server" ID="releasesTable" OnItemDataBound="releasesTable_ItemDataBound" AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false">
    <MasterTableView ClientDataKeyNames="Pk_ReleaseId">
        <Columns>
            <telerik:GridBoundColumn DataField="Pk_ReleaseId" HeaderText="Release Code" UniqueName="Pk_ReleaseId"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Name" HeaderText="Name" UniqueName="name"></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn DataField="Status" HeaderText="Status" UniqueName="Status">
                <ItemTemplate>
                    <span class="status <%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.ReleaseStatus").ToString().ToLower() %>"><%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.ReleaseStatus") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn DataField="StartDate" HeaderText="Start date" UniqueName="StartDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>

            <telerik:GridTemplateColumn DataField="Stage1FreezeDate" HeaderText="Stage 1 Freeze date" UniqueName="Stage1FreezeDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"Stage1FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn DataField="Stage2FreezeDate" HeaderText="Stage 2 Freeze date" UniqueName="Stage2FreezeDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"Stage2FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn DataField="Stage3FreezeDate" HeaderText="Stage 3 Freeze date" UniqueName="Stage3FreezeDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"Stage3FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>

            <telerik:GridTemplateColumn DataField="EndDate" HeaderText="End date" UniqueName="EndDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"EndDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn DataField="ClosureDate" HeaderText="Closure date" UniqueName="ClosureDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"ClosureDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            

            <%--
            <telerik:GridBoundColumn DataField="StartDate" HeaderText="Start date" DataType="System.DateTime" DataFormatString="{0:yyyy-MM-dd}"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Stage1FreezeDate" HeaderText="Stage 1 Freeze date" DataType="System.DateTime" DataFormatString="{0:yyyy-MM-dd}"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Stage2FreezeDate" HeaderText="Stage 2 Freeze date" DataType="System.DateTime" DataFormatString="{0:yyyy-MM-dd}"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Stage3FreezeDate" HeaderText="Stage 3 Freeze date" DataType="System.DateTime" DataFormatString="{0:yyyy-MM-dd}"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="EndDate" HeaderText="End date" DataType="System.DateTime" DataFormatString="{0:yyyy-MM-dd}"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ClosureDate" HeaderText="Closure date" DataType="System.DateTime" DataFormatString="{0:yyyy-MM-dd}"></telerik:GridBoundColumn>--%>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>