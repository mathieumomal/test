<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="FullView" Src="../../controls/Ultimate/FullView.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.Release.View" %>
<%@ Import Namespace="System.Drawing" %>


<div class="headerBtnRelease">
    <div style="float:right"><ult:FullView ID="ultFullView" runat="server" /></div>
    <asp:LinkButton runat="server" ID="newRelease" Text="New" CssClass="LinkButton"></asp:LinkButton>
</div>

<script type="text/javascript">
    $(document).ready(function () {

        var button = $("#<%= newRelease.ClientID %>");
        button.click(openCreatePopUp);

        function openCreatePopUp() {
            var popUp = window.open('/desktopmodules/Release/ReleaseEdition.aspx?action=Creation',
                                        'Release_Creation', 'height=690,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no');
            popUp.focus();
            return false;
        }
    });
    
    
</script>


<div style="clear:both"></div>
 <telerik:RadGrid runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" ID="releasesTable" OnItemDataBound="releasesTable_ItemDataBound"  AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false">
    <MasterTableView ClientDataKeyNames="Pk_ReleaseId">
        <Columns>
            <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="Code" HeaderText="Release Code" UniqueName="Code" ></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="10%" DataField="Name" HeaderText="Name" UniqueName="Name" HeaderTooltip="Release name" >
                <ItemTemplate>
                    <div class="text-left"><a class="linkRelease" href="<%# DataBinder.Eval(Container.DataItem,"Description") %>"><%# DataBinder.Eval(Container.DataItem,"Name") %></a></div>  
                </ItemTemplate> 
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="6%" DataField="Status" HeaderText="Status" UniqueName="Status">
                <ItemTemplate>
                    <span class="status <%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.Description").ToString().ToLower() %>"><%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.Description") %></span>  
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
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="EndDate" HeaderText="End date" UniqueName="EndDate" HeaderTooltip="End date of ...">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"EndDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="ClosureDate" HeaderText="Closure date" UniqueName="ClosureDate">
                <ItemTemplate>
                    <span><%# DataBinder.Eval(Container.DataItem,"ClosureDate", "{0:yyyy-MM-dd}") %></span>  
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="2%" UniqueName="ReleaseDetails">
                <ItemTemplate>
					<span></span>
                    <img class="imgViewReleases" title="See details" alt="See details" src="/DesktopModules/Release/images/details.png" 
                        onclick="var popUp=window.open('/desktopmodules/Release/ReleaseDetails.aspx?releaseId=<%# DataBinder.Eval(Container.DataItem,"Pk_ReleaseId").ToString() %>',
								'Rel-<%# DataBinder.Eval(Container.DataItem,"Pk_ReleaseId").ToString() %>', 'height=690,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" />
                </ItemTemplate>                    
            </telerik:GridTemplateColumn>
            <telerik:GridButtonColumn HeaderStyle-width="2%" CommandName="seeSpec" Text="See related specifications" UniqueName="seeSpec" ButtonType="ImageButton" ImageUrl="~/DesktopModules/Release/images/specifications.jpg"></telerik:GridButtonColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

<telerik:RadToolTipManager ID="rdTooltipHeader" runat="server"  RelativeTo="Element" Position="TopCenter">
</telerik:RadToolTipManager>