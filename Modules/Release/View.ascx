<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.Release.View" %>
<%@ Import Namespace="System.Drawing" %>
<link id="Link1" href="DesktopModules/Release/skin/skin.css" rel="stylesheet" type="text/css" runat="server" />

<telerik:RadButton runat="server" ID="newRelease" Text="New"></telerik:RadButton>

 <telerik:RadGrid runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" ID="releasesTable" OnItemDataBound="releasesTable_ItemDataBound" OnCol AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false">
    <MasterTableView ClientDataKeyNames="Pk_ReleaseId">
        <Columns>
            <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="Code" HeaderText="Release Code" UniqueName="Code"></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="10%" DataField="Name" HeaderText="Name" UniqueName="Name">
                <ItemTemplate>
                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Name") %></div>  
                </ItemTemplate> 
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="6%" DataField="Status" HeaderText="Status" UniqueName="Status">
                <ItemTemplate>
                    <span class="status <%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.ReleaseStatus").ToString().ToLower() %>"><%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.ReleaseStatus") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="StartDate" HeaderText="Start date" UniqueName="StartDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="Stage1FreezeDate" HeaderText="Stage 1 <br />Freeze date" UniqueName="Stage1FreezeDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"Stage1FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="Stage2FreezeDate" HeaderText="Stage 2 <br />Freeze date" UniqueName="Stage2FreezeDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"Stage2FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="Stage3FreezeDate" HeaderText="Stage 3 <br />Freeze date" UniqueName="Stage3FreezeDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"Stage3FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="EndDate" HeaderText="End date" UniqueName="EndDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"EndDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="ClosureDate" HeaderText="Closure date" UniqueName="ClosureDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"ClosureDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridButtonColumn HeaderStyle-Width="2%" CommandName="see" Text="See details" UniqueName="see" ButtonType="ImageButton" ImageUrl="~/DesktopModules/Release/images/details.png"></telerik:GridButtonColumn>
            <telerik:GridButtonColumn HeaderStyle-width="2%" CommandName="seeSpec" Text="See related specifications" UniqueName="seeSpec" ButtonType="ImageButton" ImageUrl="~/DesktopModules/Release/images/specifications.png"></telerik:GridButtonColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>