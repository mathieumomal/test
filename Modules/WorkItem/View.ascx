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
</style>
<asp:UpdatePanel ID="upWorkItemsTree" runat="server">
    <ContentTemplate>
        <table style="width: 100%;">
            <tr>
                <td>
                    <ult:fullviewcontrol id="ultFullView" runat="server" />
                    <telerik:RadButton ID="workItem_import" runat="server" Enabled="true" AutoPostBack="false" OnClientClicked="open_RadWindow_workItemImport" Text="Import work plan"></telerik:RadButton>                    
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
                                        <telerik:RadAutoCompleteBox ID="racAcronym" runat="server" InputType="Text" AllowCustomEntry="true" Width="200" DropDownWidth="200" DropDownHeight="150" Filter="StartsWith">
                                            <TextSettings SelectionMode="Single" />
                                        </telerik:RadAutoCompleteBox>
                                    </td>
                                    <td>Hide Completed Items</td>
                                    <td>
                                        <telerik:RadButton ID="chkHideCompletedItems" ToggleType="CheckBox" runat="server" ButtonType="ToggleButton" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Name</td>
                                    <td>
                                        <telerik:RadTextBox ID="txtName" runat="server" Width="200"></telerik:RadTextBox>
                                    </td>
                                    <td/>
                                    <td/>
                                </tr>
                                <tr>
                                    <td colspan="4" style="text-align:right; padding-right:20px">
                                        <asp:Button ID="btnDefault" runat="server" Text="Default" Width="150px" OnClick="btnDefault_Click"></asp:Button>                   
                                        <asp:Button ID="btnSearch" runat="server" Text="Search"  Width="150px" OnClick="btnSearch_Click"></asp:Button></td>
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
                        ParentDataKeyNames="Fk_ParentWiId" DataKeyNames="Pk_WorkItemUid" AutoGenerateColumns="false" AllowSorting="false"  AllowPaging="false"  AllowFilteringByColumn="false">
                        <columns>
                        <telerik:TreeListBoundColumn DataField="Name" UniqueName="Name" HeaderText="Name">
                            <HeaderStyle Font-Bold="True" Width="300px"/> 
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListBoundColumn DataField="Acronym" UniqueName="Acronym" HeaderText="Acronym">
                            <HeaderStyle Font-Bold="True" Width="70px"/> 
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListBoundColumn DataField="UID" UniqueName="UID"  HeaderText="UID">
                            <HeaderStyle Font-Bold="True" Width="50px"/> 
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListTemplateColumn DataField="Release" UniqueName="Release" HeaderText="Release">
                            <HeaderStyle Font-Bold="True" Width="50px"/> 
                            <ItemTemplate>
                                <span><%# DataBinder.Eval(Container.DataItem,"Release.Code") %></span>  
                            </ItemTemplate>      
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListTemplateColumn DataField="StartDate" UniqueName="StartDate"  HeaderText="Start date">
                            <HeaderStyle Font-Bold="True" Width="140px"/> 
                            <ItemTemplate>
                                <span><%# DataBinder.Eval(Container.DataItem,"StartDate", "{0:yyyy-mm-dd hh:mm UTC}") %></span>  
                            </ItemTemplate>      
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListTemplateColumn DataField="EndDate" UniqueName="EndDate"  HeaderText="End date">
                            <HeaderStyle Font-Bold="True" Width="140px"/> 
                            <ItemTemplate>
                                <span><%# DataBinder.Eval(Container.DataItem,"EndDate", "{0:yyyy-mm-dd hh:mm UTC}") %></span>  
                            </ItemTemplate>   
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListTemplateColumn DataField="Completion" UniqueName="Completion"  HeaderText="Completion rate">
                            <HeaderStyle Font-Bold="True" Width="90px"/> 
                            <ItemTemplate>
                                <span><%# String.Format("{0:0'%}",  DataBinder.Eval(Container.DataItem,"Completion") )%></span>  
                            </ItemTemplate>     
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListBoundColumn DataField="ResponsibleGroups" UniqueName="ResponsibleGroups" HeaderText="Responsible groups">
                            <HeaderStyle Font-Bold="True"  Width="90px"/> 
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListBoundColumn  DataField="LatestRemark" UniqueName="LatestRemark"   HeaderText="Latest remark">
                            <HeaderStyle Font-Bold="True" Width="200px"/> 
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListTemplateColumn UniqueName="ViewWorkItem">
                            <HeaderStyle Width="50px"/> 
                            <ItemTemplate>
					            <span></span>
                                <img id="imgViewWorkItems" alt="See details" src="/DesktopModules/WorkItem/images/details.png" 
                                    onclick="var popUp=window.open('/desktopmodules/WorkItem/WorkItemDetails.aspx?workitemId=<%# DataBinder.Eval(Container.DataItem,"Pk_WorkItemUid").ToString() %>',
								            'Rel-<%# DataBinder.Eval(Container.DataItem,"Pk_WorkItemUid").ToString() %>', 'height=550,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" />
                            </ItemTemplate>      
                        </telerik:TreeListTemplateColumn>
                        <telerik:TreeListBoundColumn DataField="Display" Visible="false" UniqueName="Display" >
                        </telerik:TreeListBoundColumn>
                        <telerik:TreeListBoundColumn DataField="wiLevel" Visible="false" UniqueName="wiLevel" >
                        </telerik:TreeListBoundColumn>
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
                        <div><asp:Label ID="lblCountWarningErrors" runat="server" Text="Test" /></div>
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
                        <asp:Label runat="server" ID="lblSaveStatus" Text="Work plan was successfully imported.<br/>Word and Excel version of the work plan are available at:" />
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

        <telerik:RadWindow ID="RadWindow_workItemCount" runat="server" Modal="true" Title="Warning.." Width="400" Height="180" VisibleStatusbar="false" Behaviors="Close">
            <ContentTemplate>
                <div class="contentModal" id="wiCount">
                    <div class="wiHeader">
                        Query might take a long time to perform..<br />
                        Do you want to refine the search?
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="rbworkItemCountOk" runat="server" Text="Yes" OnClick="rbWorkItemCountOk_Click"></telerik:RadButton>
                        <telerik:RadButton ID="rbworkItemCountCancel" runat="server" Text="No" OnClientClicked="cancel" AutoPostBack="false"></telerik:RadButton>
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </windows>
</telerik:RadWindowManager>

<script type="text/javascript">
    //Documentation telerik -> JS method to modify radWindow : http://www.telerik.com/help/aspnet-ajax/window-programming-radwindow-methods.html

    /*--- MODALS ---*/

    function open_RadWindow_workItemImport(sender, eventArgs) {
        closeAllModals();
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
    }
    function clearFilesToUpload() {
        var upload = $find("<%= RdAsyncUpload.ClientID %>");
        upload.deleteAllFileInputs();
    }
    function cancel() {
        closeAllModals();
    }

    /*-- TELERIK EVENTS --*/

    function Start(sender, arguments) {
        if (arguments.EventTarget == "<%= wiRadAjaxManager.UniqueID %>") {
            clearFilesToUpload();
            open_RadWindow_workItemAnalysis();
        }
        if (arguments.EventTarget == "<%= btnConfirmImport.UniqueID %>") {

        }
    }
    function End(sender, arguments) {
        if (arguments.EventTarget == "<%= wiRadAjaxManager.UniqueID %>") {
            open_RadWindow_workItemConfirmation();
        } if (arguments.EventTarget == "<%= btnConfirmImport.UniqueID %>") {
            open_RadWindow_workItemState();
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



</script>


<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function OnClientFileUploaded(sender, args) {
            $find('<%=wiRadAjaxManager.ClientID %>').ajaxRequest();
        }
    </script>
</telerik:RadScriptBlock>
