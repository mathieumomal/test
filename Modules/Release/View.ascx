﻿<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="FullView" Src="../../controls/Ultimate/FullView.ascx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.Release.View" %>
<%@ Import Namespace="System.Drawing" %>

<div class="headerBtnRelease">
    <div style="float:right"><ult:FullView ID="ultFullView" runat="server" /></div>
    <asp:LinkButton runat="server" ID="newRelease" Text="New" CssClass="btn3GPP-success"></asp:LinkButton>
    <asp:LinkButton ID="btnMassivePromote" class="btn3GPP-success" runat="server" OnClientClick="var popUp=window.open('/desktopmodules/Specifications/Massivepromote.aspx', 'Specification-Massive-Promote', 'height=590,width=1024,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();return false;" Text="Massive Promote" />
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
<div id="releaseList">
     <telerik:RadGrid runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" ID="releasesTable" OnItemDataBound="releasesTable_ItemDataBound"  
         AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false" AutoGenerateColumns="false"
         EnableViewState="false">
        <MasterTableView ClientDataKeyNames="Pk_ReleaseId">
            <Columns>
                <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="Code" HeaderText="Release Code" UniqueName="Code" ></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="10%" DataField="Name" HeaderText="Name" UniqueName="Name">
                    <ItemTemplate>
                        <div class="text-left"><asp:HyperLink runat="server" ID="lnkReleaseDescription" CssClass="linkRelease" Visible="false" />
                            <asp:Label runat="server" ID="lblReleaseName" />
                        </div>  
                    </ItemTemplate> 
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="6%" DataField="Status" HeaderText="Status" UniqueName="Status" HeaderTooltip="Status of the Release (see 3GPP TR 21.900)">
                    <ItemTemplate>
                        <span class="status <%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.Description").ToString().ToLower() %>"><%# DataBinder.Eval(Container.DataItem,"Enum_ReleaseStatus.Description") %></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="StartDate" HeaderText="Start date" UniqueName="StartDate" HeaderTooltip="Start of earliest work item in this Release">
                    <ItemTemplate>
                        <span><%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:yyyy-MM-dd}") %></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="Stage1FreezeDate" HeaderText="Stage 1 <br />Freeze date" UniqueName="Stage1FreezeDate" HeaderTooltip="Stage 1 Feeze date (see 3GPP TR 21.900)">
                    <ItemTemplate>
                        <span><%# DataBinder.Eval(Container.DataItem,"Stage1FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="Stage2FreezeDate" HeaderText="Stage 2 <br />Freeze date" UniqueName="Stage2FreezeDate" HeaderTooltip="Stage 2 Feeze date (see 3GPP TR 21.900)">
                    <ItemTemplate>
                        <span><%# DataBinder.Eval(Container.DataItem,"Stage2FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="Stage3FreezeDate" HeaderText="Stage 3 <br />Freeze date" UniqueName="Stage3FreezeDate" HeaderTooltip="Stage 3 Feeze date (see 3GPP TR 21.900)">
                    <ItemTemplate>
                        <span><%# DataBinder.Eval(Container.DataItem,"Stage3FreezeDate", "{0:yyyy-MM-dd}") %></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="EndDate" HeaderText="End date" UniqueName="EndDate" HeaderTooltip="Protocols stable">
                    <ItemTemplate>
                        <span><%# DataBinder.Eval(Container.DataItem,"EndDate", "{0:yyyy-MM-dd}") %></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="12%" DataField="ClosureDate" HeaderText="Closure date" UniqueName="ClosureDate" HeaderTooltip="Date after which specifications are no longer maitained for the corresponding Release (see 3GPP TR 21.900)">
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
                <telerik:GridTemplateColumn HeaderStyle-Width="2%" UniqueName="seeSpec">
                    <ItemTemplate>
                        <a target="_blank" href="./Specifications.aspx?q=1&releases=<%# DataBinder.Eval(Container.DataItem, "Pk_ReleaseId").ToString() %>"><img border="0" src="/DesktopModules/Release/images/specifications.jpg" title="See related specifications" /></a>
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</div>

    
 