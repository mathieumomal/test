<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadVersion.aspx.cs" Inherits="Etsi.Ultimate.Module.Versions.UploadVersion" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="JS/jquery.min.js"></script>
</head>
<body>
    <style type="text/css">
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

        .version_0 {
            color: red;
            font-weight: bold;
        }

        .version_1 {
            color: blue;
            font-weight: bold;
        }

        .version_2 {
            color: black;
            font-weight: bold;
        }

        .breakWord {
            word-break: break-all !important;
        }
    </style>
    <form id="VersionUploadForm" runat="server">
        <telerik:RadAjaxManager ID="verionsRadAjaxManager" runat="server" EnablePageHeadUpdate="false">
            <ClientEvents OnRequestStart="Start" OnResponseEnd="End" />
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="UploadBtnDisabled">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="btnConfirmUpload" />
                        <telerik:AjaxUpdatedControl ControlID="lblCountWarningErrors" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="btnConfirmUpload">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="lblSaveStatus" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="500px">
            <asp:Panel ID="versionUploadMessages" runat="server" Visible="false">
                <asp:Label runat="server" ID="specificationMessagesTxt"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="versionUploadBody" runat="server" CssClass="versionUploadBody">
                <div class="contentModal" id="versionUploadScreen">
                    <table class="VersionDetailsTable">
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="SpecNumberLbl" Text="Specification number:" />
                            </td>
                            <td class="TabLineRight">
                                <asp:Label runat="server" ID="SpecNumberVal" />
                            </td>
                            <td>
                                <asp:Label runat="server" ID="ReleaseLbl" Text="Release:" />
                                <asp:Label runat="server" ID="ReleaseVal" />
                            </td>
                        </tr>
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="CurrentVersionLbl" Text="Current version:" />
                            </td>
                            <td colspan="2" class="TabLineRight">
                                <asp:Label runat="server" ID="CurrentVersionVal" />
                            </td>
                        </tr>
                    </table>
                    <table class="VersionUploadTable">
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="FileToUploadLbl" Text="File to upload:" />
                            </td>
                            <td colspan="2" class="TabLineRight2Col">
                                <telerik:RadAsyncUpload runat="server" ID="FileToUploadVal"
                                    MaxFileInputsCount="1"
                                    AllowedFileExtensions="docx,doc,zip"
                                    Localization-Select="Browse"
                                    OnClientFileUploaded="OnClientFileUploaded"
                                    OnClientValidationFailed="OnClientValidationFailed"
                                    OnFileUploaded="AsyncUpload_VersionUpload"
                                    OnClientFileSelected="EnabledButtonUpload"
                                    OnClientFileUploadRemoved="DisabledButtonUpload"
                                    OverwriteExistingFiles="True"
                                    ManualUpload="true"
                                    Visible="true">
                                </telerik:RadAsyncUpload>
                            </td>
                        </tr>
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="NewVersionLbl" Text="New version:" />
                            </td>
                            <td colspan="2" class="TabLineRight2Col">
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionMajorVal" Width="40px" MinValue="0"></telerik:RadNumericTextBox>
                                <span>-</span>
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionTechnicalVal" Width="40px" MaxValue="9" MinValue="0"></telerik:RadNumericTextBox>
                                <span>-</span>
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionEditorialVal" Width="40px" MaxValue="9" MinValue="0"></telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="CommentLbl" Text="Comment:" />
                            </td>
                            <td colspan="2" class="TabLineRight2Col">
                                <asp:TextBox runat="server" ID="CommentVal" EmptyMessage="Your comment" TextMode="MultiLine" Resize="Vertical" Width="195px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="MeetingLbl" Text="Meeting:" />
                            </td>
                            <td colspan="2" class="TabLineRight2Col">
                                <ult:meetingcontrol runat="server" id="UploadMeeting" />
                            </td>
                        </tr>
                    </table>
                    <div class="releaseDetailsAction">
                        <asp:LinkButton ID="UploadBtn" runat="server" Text="Upload" CssClass="btn3GPP-success" OnClientClick="startVersionUploadProcess(); return false;" />
                        <asp:LinkButton ID="AllocateBtn" runat="server" Text="Allocate" CssClass="btn3GPP-success" Visible="false" OnClick="AllocateVersion_Click" />
                        <asp:LinkButton ID="UploadBtnDisabled" runat="server" Text="Upload" CssClass="btn3GPP-default" disabled="disabled" OnClientClick="return false;" />
                        <asp:LinkButton ID="ExitBtn" runat="server" Text="Cancel" CssClass="btn3GPP-success" OnClientClick="  return closePopUpWindow()" />
                    </div>
                </div>
                <div class="contentModal" id="analysis" style="display: none">
                    <div class="wiHeader">
                        Version analysis is in progress.
                    </div>
                    <div class="wiCenter">
                        <asp:Image ID="imgWait" runat="server" Class="rotating" ImageUrl="~/DesktopModules/Versions/images/hourglass.png" Width="45" />
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="analysis_cancel" runat="server" Text="Cancel" OnClientClicked="cancel" AutoPostBack="false"></telerik:RadButton>
                    </div>
                </div>
                <div class="contentModal" id="confirmation" style="display: none">
                    <div class="wiHeader">
                        <div>
                            <asp:Label ID="lblCountWarningErrors" runat="server" Text="Operation timed out" />
                        </div>
                    </div>
                    <div>
                        <h2>Errors and Warnings</h2>
                        <div class="scrollable">
                            <ul>
                                <asp:Repeater runat="server" ID="rptWarningsErrors">
                                    <ItemTemplate>
                                        <li>
                                            <asp:Label ID="lblErrorOrWarning" runat="server"></asp:Label>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>
                    </div>
                    <div class="wiFooter">
                        <span class="updateProgress" id="importProgressIcon" style="visibility: hidden">
                            <asp:Image ID="imgProgressImport" runat="server" Class="rotating" ImageUrl="~/DesktopModules/Versions/images/hourglass.png" Width="45" /></span>
                        <span>
                            <telerik:RadButton ID="btnConfirmUpload" runat="server" Text="Confirm upload" AutoPostBack="true" OnClick="Confirmation_Upload_OnClick" CssClass="WiInline"></telerik:RadButton>
                        </span>
                        <span>
                            <telerik:RadButton ID="Confirmation_cancel" runat="server" Text="Cancel" AutoPostBack="false" OnClientClicked="cancel"></telerik:RadButton>
                        </span>
                    </div>
                    <div>
                        <asp:HiddenField ID="isDraft" runat="server" />
                    </div>
                </div>
                <div class="contentModal" id="state" style="display: none">
                    <div class="wiHeader">
                        <asp:Label runat="server" ID="lblSaveStatus" Text="" />
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="state_confirmation" runat="server" Text="OK" AutoPostBack="false"></telerik:RadButton>
                    </div>
                </div>

                <script type="text/javascript">

                    function startVersionUploadProcess(sender, eventArgs) {
                        //If a Version => Analysis is required
                        var upload = $find('<%= FileToUploadVal.ClientID%>');
                        upload.startUpload();
                    }

                    /*-- TELERIK EVENTS --*/

                    function Start(sender, arguments) {
                        if (arguments.EventTarget == "<%= verionsRadAjaxManager.UniqueID %>") {
                            clearFilesToUpload();
                            if ($("#isDraft").val() != "1") {
                                open_RadWindow_VersionUploadAnalysis();
                            }
                                //If Draft => Perform FTP transfer without analysis
                            else {
                                open_RadWindow_VersionUploadConfirmation();
                            }
                        }

                    }
                    function End(sender, arguments) {
                        if (arguments.EventTarget == "<%= verionsRadAjaxManager.UniqueID %>") {
                              open_RadWindow_VersionUploadConfirmation();
                          }
                          if (arguments.EventTarget == "<%= btnConfirmUpload.UniqueID %>") {
                              open_RadWindow_VersionUploadState();
                          }
                      }

                      function open_RadWindow_VersionUploadAnalysis(sender, eventArgs) {
                          $('#versionUploadScreen').hide();
                          $('#analysis').show();
                      }
                      function open_RadWindow_VersionUploadConfirmation(sender, eventArgs) {
                          $('#analysis').hide();
                          $('#confirmation').show();

                      }
                      function open_RadWindow_VersionUploadState(sender, eventArgs) {
                          $('#confirmation').hide();
                          $('#state').show();
                      }

                      function cancel() {
                          $('#analysis').hide();
                          $('#confirmation').hide();
                          $('#state').hide();
                          $('#versionUploadScreen').show();
                          //Other actions to perform
                      }

                      function clearFilesToUpload() {
                          var upload = $find("<%= FileToUploadVal.ClientID %>");
                        upload.deleteAllFileInputs();
                    }

                    function OnClientValidationFailed() {
                        DisabledButtonUpload();
                        alert('Only doc, docx and zip formats are allowed.');
                    }

                    function EnabledButtonUpload() {
                        $('#UploadBtn').show();
                        $('#UploadBtnDisabled').hide();
                    }
                    function DisabledButtonUpload() {
                        $('#UploadBtnDisabled').show();
                        $('#UploadBtn').hide();
                    }

                    function ShowAllocationResult(result) {
                        if (result == "success") {

                            alert('Allocation of the version succeded');
                            closePopUpWindow();
                        }
                        else {
                            alert('Allocation of the version failed');
                        }
                    }

                    /* Exit function */
                    function closePopUpWindow() {
                        window.close();
                    }

                    $(document).ready(function () {
                        //Update of page title
                        setTimeout(function () {
                            $('#UploadBtn').hide();
                        });
                    });
                </script>
                <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
                    <script type="text/javascript">
                        function OnClientFileUploaded(sender, args) {
                            $find('<%=verionsRadAjaxManager.ClientID %>').ajaxRequest();
                        }
                    </script>
                </telerik:RadScriptBlock>
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
