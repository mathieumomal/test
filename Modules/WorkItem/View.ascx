<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.WorkItem.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="ult" TagName="ShareUrlControl" Src="../../controls/Ultimate/ShareUrlControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="FullViewControl" Src="../../controls/Ultimate/FullView.ascx" %>
<%@ Register TagPrefix="ult" TagName="ReleaseSearchControl" Src="../../controls/Ultimate/ReleaseSearchControl.ascx" %>

<style type="text/css">
    div.RadPanelBar .rpRootGroup .rpText {
        text-align: center;
    }

    .modalBackground {
        background-color: Gray;
        filter: alpha(opacity=60);
        opacity: 0.60;
        width: 100%;
        top: 0px;
        left: 0px;
        position: fixed;
        height: 100%;
        z-index: 3000;
    }

    .updateProgress {
        margin: auto;
        font-family: Trebuchet MS;
        filter: alpha(opacity=100);
        opacity: 1;
        font-size: small;
        vertical-align: middle;
        color: #275721;
        text-align: center;
        background-color: White;
        padding: 10px;
        -moz-border-radius: 15px;
        z-index: 3001;
        border-radius: 15px;
    }

        .updateProgress .Fixed {
            top: 45%;
            position: fixed;
            right: 45%;
        }

    .workItem_0 {
        color: red;
        font-weight: bold;
    }

    .workItem_1 {
        color: blue;
        font-weight: bold;
    }

    .workItem_2 {
        color: black;
        font-weight: bold;
    }

    .breakWord {
        word-break: break-all !important;
    }
</style>

<asp:UpdateProgress ID="updateProgressWorkItemsTree" runat="server" DisplayAfter="200">
    <ProgressTemplate>
        <div class="modalBackground">
        </div>
        <div class="updateProgress fixed">
            <asp:Image ID="imgProgress" runat="server" Class="rotating" ImageUrl="~/DesktopModules/WorkItem/images/hourglass.png" Width="45" />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>

<asp:UpdatePanel ID="upWorkItemsTree" runat="server">
    <ContentTemplate>
        <table style="width: 100%;">
            <tr>
                <td>
                    
                    <asp:LinkButton ID="WorkPlanImport_Btn" runat="server" OnClientClick="open_RadWindow_workItemImport(); return false;" Text="Import work plan" />
                    <span style="float: right; padding-bottom:2px; white-space:nowrap ">
                        <asp:Label Visible="false" ID="lblLatestUpdated" runat="server" />
                        <asp:HyperLink Visible="false" ID="lnkFtpDownload" runat="server" Text="Download from FTP" Target="_blank" />
                        &nbsp; <ult:FullViewControl id="ultFullView" runat="server" />
                    </span>
                    
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadPanelBar runat="server" ID="rpbSearch" Width="100%">
                        <items>
                    <telerik:RadPanelItem runat="server" ID="searchPanel" Expanded="True">
                        <HeaderTemplate>
                            <table style="width:100%;vertical-align:middle" class="WorkItemSearchHeader">
                                <tr>
                                    <td style="width:20px;"><ult:shareurlcontrol runat="server" id="ultShareUrl" /></td>
                                    <td style="text-align:center"><asp:Label ID="lblSearchHeader" runat="server" /></td>
                                    <td style="width: 20px;"><a class="rpExpandable">
                                        <span class="rpExpandHandle"></span>
                                    </a></td>
                                </tr>
                            </table>
                        </HeaderTemplate>
                        <ContentTemplate>
                            <table style="width:100%; padding:20px 50px 20px 50px">
                                <tr>
                                    <td>Release</td>
                                    <td>
                                        <ult:ReleaseSearchControl id="releaseSearchControl" runat="server" Width="200" DropDownWidth="200"/>
                                    </td>
                                    <td>Granularity (Level)</td>
                                    <td>
                                        <telerik:RadDropDownList ID="rddGranularity" runat="server" Width="200" DropDownWidth="200px" AutoPostBack="false">
                                            <Items>
                                                <telerik:DropDownListItem Text="Feature (1st level)" Value="1"/>
                                                <telerik:DropDownListItem Text="Building Block (Up to 2nd level)" Value="2"/>
                                                <telerik:DropDownListItem Text="Working Task (Up to 3rd Level)" Value="3"/>
                                                <telerik:DropDownListItem Text="Up to 4th level" Value="4"/>
                                                <telerik:DropDownListItem Text="Up to 5th level" Value="5"/>
                                            </Items>
                                        </telerik:RadDropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Acronym</td>
                                    <td>
                                        <telerik:RadAutoCompleteBox ID="racAcronym" OnClientTextChanged="racAcronym_TextChanged" runat="server" InputType="Text" AllowCustomEntry="true" Width="200" DropDownWidth="200" DropDownHeight="150" Filter="StartsWith">
                                            <TextSettings SelectionMode="Single" />
                                        </telerik:RadAutoCompleteBox>
                                        <asp:HiddenField ID="hidAcronym" runat="server"/>
                                    </td>
                                    <td>Hide Completed Items</td>
                                    <td>
                                        <telerik:RadButton ID="chkHideCompletedItems" ToggleType="CheckBox" runat="server" ButtonType="ToggleButton" AutoPostBack="false"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Name / UID</td>
                                    <td>
                                        <telerik:RadTextBox ID="txtName" runat="server" Width="200"></telerik:RadTextBox>
                                    </td>
                                    <td/>
                                    <td/>
                                </tr>
                                <tr>
                                    <td colspan="4" style="text-align:right; padding-right:20px">
                                        <asp:Button ID="btnDefault" runat="server" Text="Default" Width="150px" OnClick="btnDefault_Click" OnClientClick="collapseItem()"></asp:Button>                   
                                        <asp:Button ID="btnSearch" runat="server" Text="Search"  Width="150px" OnClick="btnSearch_Click" OnClientClick="collapseItem()"></asp:Button></td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </telerik:RadPanelItem>
                </items>
                    </telerik:RadPanelBar>
                </td>
            </tr>
            <tr>
                <td>

                    <telerik:RadTreeList ID="rtlWorkItems" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" runat="server" OnNeedDataSource="rtlWorkItems_NeedDataSource"
                        ParentDataKeyNames="Fk_ParentWiId" DataKeyNames="Pk_WorkItemUid" AutoGenerateColumns="false" AllowSorting="false" AllowPaging="false" AllowFilteringByColumn="false">
                        <clientsettings>
                            <Scrolling AllowScroll="true" />
                            <ClientEvents OnTreeListCreated="TreeListCreated" />
                        </clientsettings>
                        <columns>
                        <telerik:TreeListTemplateColumn  DataField="Name" UniqueName="Name" HeaderText="Name" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True" Width="20%"/> 
                            <ItemTemplate>
                                <div class="workItem_<%# DataBinder.Eval(Container.DataItem,"WiLevel")%>" style="text-align:left;"><%# DataBinder.Eval(Container.DataItem,"Name")%></div>  
                            </ItemTemplate>
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListBoundColumn DataField="Acronym" UniqueName="Acronym" HeaderText="Acronym" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True"/> 
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListBoundColumn DataField="UID" UniqueName="UID" HeaderText="<span title='Unique Work Item Identifier' class='helpTooltip'>UID</span>"  ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True"/>
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListTemplateColumn DataField="Release" UniqueName="Release" HeaderText="Release" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True"/> 
                            <ItemTemplate>
                                <span><%# DataBinder.Eval(Container.DataItem,"Release.Code") %></span>  
                            </ItemTemplate>      
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListTemplateColumn DataField="StartDate" UniqueName="StartDate"  HeaderText="Start date" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True"/> 
                            <ItemTemplate>
                                <span><%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:yyyy-MM-dd}") %></span>  
                            </ItemTemplate>      
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListTemplateColumn DataField="EndDate" UniqueName="EndDate"  HeaderText="End date" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True"/> 
                            <ItemTemplate>
                                <span><%# DataBinder.Eval(Container.DataItem,"EndDate", "{0:yyyy-MM-dd}") %></span>  
                            </ItemTemplate>   
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListTemplateColumn DataField="Completion" UniqueName="Completion"  HeaderText="Completion rate" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True"/> 
                            <ItemTemplate>
                                <span><%# String.Format("{0:0'%}",  DataBinder.Eval(Container.DataItem,"Completion") )%></span>  
                            </ItemTemplate>     
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListBoundColumn DataField="ResponsibleGroups" UniqueName="ResponsibleGroups" HeaderText="Responsible groups" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True"/> 
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListTemplateColumn  UniqueName="LatestRemark"   HeaderText="Latest remark" ItemStyle-CssClass="breakWord">
                            <HeaderStyle Font-Bold="True" Width="16%"/> 
                            <ItemTemplate>
                                <div style="text-align:left"><%# DataBinder.Eval(Container.DataItem,"ShortLatestRemark")%></div>  
                            </ItemTemplate>
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListTemplateColumn UniqueName="ViewWorkItem">
                            <HeaderStyle Width="50px"/> 
                            <ItemTemplate>
					            <span></span>
                                <img id="imgViewWorkItems" alt="See details" src="/DesktopModules/WorkItem/images/details.png" style='display: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsLevel0Record")) ? "none" : "block" %>'
                                    onclick="var popUp=window.open('/desktopmodules/WorkItem/WorkItemDetails.aspx?workitemId=<%# DataBinder.Eval(Container.DataItem,"Pk_WorkItemUid").ToString() %>',
								            'Rel-<%# DataBinder.Eval(Container.DataItem,"Pk_WorkItemUid").ToString() %>', 'height=550,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" />
                            </ItemTemplate>      
                        </telerik:TreeListTemplateColumn>
                    </columns>
                    </telerik:RadTreeList>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<telerik:RadAjaxManager ID="wiRadAjaxManager" runat="server" EnablePageHeadUpdate="false">
    <clientevents onrequeststart="Start" onresponseend="End" />
    <ajaxsettings>
        <telerik:AjaxSetting AjaxControlID="wiRadAjaxManager">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblExportPath" />
                <telerik:AjaxUpdatedControl ControlID="rptWarningsErrors" />
                <telerik:AjaxUpdatedControl ControlID="btnConfirmImport" />
                <telerik:AjaxUpdatedControl ControlID="lblCountWarningErrors" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnConfirmImport">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rptWarningsErrors" />
                <telerik:AjaxUpdatedControl ControlID="lblExportedPath" />
                <telerik:AjaxUpdatedControl ControlID="lblSaveStatus" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </ajaxsettings>
</telerik:RadAjaxManager>



<telerik:RadWindowManager ID="RadWindowManager1" runat="server">
    <windows>
        <telerik:RadWindow ID="RadWindow_workItemImport" runat="server" Modal="true" Title="Work Plan Import" Height="180" Width="500" VisibleStatusbar="false" iconUrl="false" Behaviors="Close">
            <ContentTemplate>
                <div class="contentModal" id="import">
                    <div class="wiHeader">
                        You are about to update the Work Items database.<br/>Please select the work plan file to upload.
                    </div>
                    <div class="wiCenter">
                        <telerik:RadAsyncUpload ID="RdAsyncUpload" runat="server" 
                            AllowedFileExtensions="csv,zip" 
                            Localization-Select="Browse" 
                            MaxFileInputsCount="1" 
                            OnClientFileUploaded="OnClientFileUploaded" 
                            OnClientValidationFailed="OnClientValidationFailed" 
                            OnFileUploaded="AsyncUpload_FileImport" 
                            OnClientFileSelected="EnabledButtonImport" 
                            OnClientFileUploadRemoved="DisabledButtonImport" 
                            OverwriteExistingFiles="True"
                            ManualUpload="true">
                        </telerik:RadAsyncUpload>
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="importButton" runat="server" Text="Import" OnClientClicked="startImport" AutoPostBack="false" Enabled="false" ></telerik:RadButton>
                        <telerik:RadButton ID="import_cancel" runat="server" Text="Cancel" OnClientClicked="cancel" AutoPostBack="false" ></telerik:RadButton>
                    </div>
                    
                </div>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow ID="RadWindow_workItemAnalysis" runat="server" Modal="true" Title="Work Plan analysis - Processing ..." Width="400" Height="180" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                <div class="contentModal" id="analysis">
                    <div class="wiHeader">
                        Work plan analysis is in progress.
                    </div>
                    <div class="wiCenter">
                        <asp:Image ID="imgWait" runat="server" Class="rotating" ImageUrl="~/DesktopModules/WorkItem/images/hourglass.png" width="45"/>
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="analysis_cancel" runat="server" Text="Cancel" OnClientClicked="cancel" AutoPostBack="false"></telerik:RadButton>
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow ID="RadWindow_workItemConfirmation" runat="server" Modal="true" Title="Import confirmation" Width="700" Height="375" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                <div class="contentModal" id="confirmation">
                    <div class="wiHeader">
                        <div><asp:Label ID="lblCountWarningErrors" runat="server" Text="Operation timed out" /></div>
                    </div>
                    <div>
                        <h2>Errors and Warnings</h2>
                        <div class="scrollable">
                            <ul>
                                <asp:Repeater runat="server" ID="rptWarningsErrors" OnItemDataBound="rptErrorsWarning_ItemDataBound">
                                    <ItemTemplate>
                                        <li> <asp:Label ID="lblErrorOrWarning" runat="server"></asp:Label> </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>
                        <div>
                            <asp:Label runat="server" ID="lblExportPath" Text=""/>
                        </div>
                    </div>
                    <div class="wiFooter">
                        <span class="updateProgress" id="importProgressIcon" style="visibility:hidden"><asp:Image ID="imgProgressImport"  runat="server" Class="rotating" ImageUrl="~/DesktopModules/WorkItem/images/hourglass.png" width="45"/></span>
                        <span><telerik:RadButton ID="btnConfirmImport" runat="server" Text="Confirm import" AutoPostBack="true" OnClick="Confirmation_import_OnClick" CssClass="WiInline" ></telerik:RadButton></span>
                        <span><telerik:RadButton ID="Confirmation_cancel" runat="server" Text="Cancel" AutoPostBack="false" OnClientClicked="cancel" ></telerik:RadButton></span>
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow ID="RadWindow_workItemState" runat="server" Modal="true" Title="Work plan import and export successful" Width="450" Height="170" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                <div class="contentModal" id="state">
                    <div class="wiHeader">
                        <asp:Label runat="server" ID="lblSaveStatus" Text="" />
                    </div>
                    <div>
                        <asp:Label id="lblExportedPath" runat="server" Text="" />
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="state_confirmation" runat="server" Text="OK" OnClientClicked="closeAllModals" AutoPostBack="false"></telerik:RadButton>
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>

        <telerik:RadWindow ID="RadWindow_workItemCount" runat="server" Modal="true" Title="Warning." Width="400" Height="180" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                <asp:UpdatePanel ID="upWorkItemCount" runat="server" UpdateMode="Always">
                <ContentTemplate>
                <div class="contentModal" id="wiCount">
                    <div class="wiHeader">
                        Query will return many records and might therefore be long to display.
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="rbworkItemCountOk" runat="server" Text="Confirm" OnClientClicked="closeAllModals" OnClick="rbWorkItemCountOk_Click"></telerik:RadButton>
                        <telerik:RadButton ID="rbworkItemCountCancel" runat="server" Text="Cancel" OnClientClicked="cancel" AutoPostBack="false"></telerik:RadButton>
                    </div>
                </div>
                            </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </windows>
</telerik:RadWindowManager>

<script type="text/javascript">
    //Documentation telerik -> JS method to modify radWindow : http://www.telerik.com/help/aspnet-ajax/window-programming-radwindow-methods.html

    /*--- MODALS ---*/

    function open_RadWindow_workItemImport(sender, eventArgs) {
        window.radopen(null, "RadWindow_workItemImport");
    }
    function open_RadWindow_workItemAnalysis(sender, eventArgs) {
        //Reset file to upload
        closeAllModals();
        window.radopen(null, "RadWindow_workItemAnalysis");
    }
    function open_RadWindow_workItemConfirmation(sender, eventArgs) {
        closeAllModals();
        window.radopen(null, "RadWindow_workItemConfirmation");
    }
    function open_RadWindow_workItemState(sender, eventArgs) {
        closeAllModals();
        window.radopen(null, "RadWindow_workItemState");
    }

    /*--- MODALS ---*/


    /*--- EVENTS ---*/

    function startImport() {
        var upload = $find('<%= RdAsyncUpload.ClientID%>');
        upload.startUpload();
    }
    function closeAllModals() {
        var manager = GetRadWindowManager();
        manager.closeAll();

        clearTimeout(timeout);
    }
    function clearFilesToUpload() {
        var upload = $find("<%= RdAsyncUpload.ClientID %>");
        upload.deleteAllFileInputs();
    }
    function cancel() {
        closeAllModals();
        var panelBar = $find("<%= rpbSearch.ClientID %>");
        var item = panelBar.get_items().getItem(0);
        if (item) {
            item.expand();
        }
    }

    /*-- TELERIK EVENTS --*/

    function Start(sender, arguments) {
        $(".modalBackground").hide();
        $(".updateProgress").hide();
        if (arguments.EventTarget == "<%= wiRadAjaxManager.UniqueID %>") {
            clearFilesToUpload();
            open_RadWindow_workItemAnalysis();
        }
        if (arguments.EventTarget == "<%= btnConfirmImport.UniqueID %>") {
            $("#importProgressIcon").show();
            $("#importProgressIcon").css("visibility", "visible");
        }
        

    }
    function End(sender, arguments) {
        $(".modalBackground").show();
        $(".updateProgress").show();
        if (arguments.EventTarget == "<%= wiRadAjaxManager.UniqueID %>") {
            open_RadWindow_workItemConfirmation();
        }
        if (arguments.EventTarget == "<%= btnConfirmImport.UniqueID %>") {
            open_RadWindow_workItemState();
            $("#importProgressIcon").hide();
            $("#importProgressIcon").css("visibility", "hidden");
        }
    }
    function OnClientValidationFailed() {
        alert('Only csv and zip formats are allowed.');
    }

    function EnabledButtonImport() {
        var but = $find("<%=importButton.ClientID %>");
        but.set_enabled(true);
    }
    function DisabledButtonImport() {
        var but = $find("<%=importButton.ClientID %>");
        but.set_enabled(false);
    }

    /*-- TELERIK EVENTS --*/

    /*--- EVENTS ---*/
    function collapseItem() {
        var panelBar = $find("<%= rpbSearch.ClientID %>");
        var item = panelBar.get_items().getItem(0);
        if (item) {
            item.collapse();
        }
    }

    var timeout;

    function autoConfirmSearch() {
        timeout = window.setTimeout(function () { $('#<%=rbworkItemCountOk.ClientID %>').click(); }, 10000)
    }

    function racAcronym_TextChanged(sender, eventArgs){
        $('#<%=hidAcronym.ClientID %>').val(eventArgs.get_text());
    }

    //--- Adapt workItem's list height

    function TreeListCreated(sender, eventArgs) {
        adaptContentHeight();
    }

    //Catch the "contentHeight" event (in the mainpage.ascx)
    $("#content").on('contentHeight', function (event, hContent) {
        resizeTree(hContent);
    });

    function resizeTree(hContent) {
        var tree = $('#<%= rtlWorkItems.ClientID %>');
        /*console.log(tree);*/
        var treeDiv = tree.find(".rtlDataDiv")[0];
        /*console.log(treeDiv);*/
        /*console.log(hContent);*/
        if ($('.livetabssouthstreet ').length == 0) {
            var securityValue = 120;
        } else {
            var securityValue = 205;
        }

        treeDiv.style.height = (hContent - securityValue) + "px";
        /*console.log(hContent+"-"+treeDiv.style.height);*/
    }

    //--- Adapt workItem's list height
</script>


<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function OnClientFileUploaded(sender, args) {
            $find('<%=wiRadAjaxManager.ClientID %>').ajaxRequest();
        }
    </script>
</telerik:RadScriptBlock>
