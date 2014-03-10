<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.WorkItem.View" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>


<telerik:RadButton ID="workItem_import" runat="server" Enabled="true" AutoPostBack="false" OnClientClicked="open_RadWindow_workItemImport" Text="Import work plan"></telerik:RadButton>

<telerik:RadAjaxManager ID="wiRadAjaxManager" runat="server" EnablePageHeadUpdate="false">
    <ClientEvents OnRequestStart="Start" OnResponseEnd="End" />
    <AjaxSettings>
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
    </AjaxSettings>
</telerik:RadAjaxManager>



<telerik:RadWindowManager ID="RadWindowManager1" runat="server" >
    <Windows>
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
    </Windows>
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
        alert('Just csv and zip files are allowed here.');
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