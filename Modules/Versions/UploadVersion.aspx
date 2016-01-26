<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadVersion.aspx.cs" Inherits="Etsi.Ultimate.Module.Versions.UploadVersion" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Portals/_default/Skins/3GPP/mainpage.css"/>
    <link rel="stylesheet" type="text/css" href="module.css"/>
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon"/>
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
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" EnableHandlerDetection="false" />
        <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="500px">
            <asp:Panel ID="versionUploadMessages" runat="server" Visible="false">
                <asp:Label runat="server" ID="specificationMessagesTxt"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="versionUploadBody" runat="server" CssClass="versionUploadBody">
                <asp:Panel runat="server" CssClass="contentModal" ID="preVersionUploadScreen">
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
                                    OnClientValidationFailed="OnClientValidationFailed"
                                    OnFileUploaded="AsyncUpload_VersionUpload"
                                    OnClientFileSelected="EnabledButtonUpload"
                                    OnClientFileUploadRemoved="DisabledButtonUpload"
                                    OverwriteExistingFiles="True"
                                    PostbackTriggers="UploadBtn"
                                    Visible="true">
                                </telerik:RadAsyncUpload>
                            </td>
                        </tr>
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="NewVersionLbl" Text="New version:" />
                            </td>
                            <td colspan="2" class="TabLineRight2Col">
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionMajorVal" Width="40px" MinValue="0" ToolTip="Major"></telerik:RadNumericTextBox>
                                <span>.</span>
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionTechnicalVal" Width="40px"  MinValue="0" ToolTip="Technical"></telerik:RadNumericTextBox>
                                <span>.</span>
                                <telerik:RadNumericTextBox AutoComplete="off" IncrementSettings-InterceptArrowKeys="true" NumberFormat-DecimalDigits="0" IncrementSettings-InterceptMouseWheel="true" runat="server" ID="NewVersionEditorialVal" Width="40px"  MinValue="0" ToolTip="Editorial"></telerik:RadNumericTextBox>
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
                                <ult:meetingcontrol runat="server" ID="UploadMeeting" />
                                <asp:HiddenField ID="hidIsRequired" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="TabLineLeft">
                                <asp:Label runat="server" ID="QualityChecksLbl" Text="Avoid quality checks:" />
                            </td>
                            <td colspan="2" class="TabLineRight2Col">
                                <asp:CheckBox runat="server" ID="AvoidQualityChecksChkBox" Checked="False" Visible="false"/>
                            </td>
                        </tr>
                    </table>
                    <div class="releaseDetailsAction">
                        <span id="meetingRequiredMsg" style="display:none" class='requiredField'>*Meeting is required</span>
                        <asp:LinkButton ID="UploadBtn" runat="server" Text="Upload" CssClass="btn3GPP-success" OnClientClick="return PerformValidations();" OnClick="UploadVersionBtn_Click"/>
                        <asp:LinkButton ID="AllocateBtn" runat="server" Text="Allocate" CssClass="btn3GPP-success" Visible="false" OnClick="AllocateVersionBtn_Click" />
                        <asp:LinkButton ID="UploadBtnDisabled" runat="server" Text="Upload" CssClass="btn3GPP-default" disabled="disabled" OnClientClick="return false;" />
                        <asp:LinkButton ID="ExitBtn" runat="server" Text="Cancel" CssClass="btn3GPP-success" OnClientClick="  return closePopUpWindow()" />
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" CssClass="contentModal" ID="analysis">
                    <div class="wiHeader">
                        Version analysis is in progress.
                    </div>
                    <div class="wiCenter">
                        <asp:Image ID="imgWait" runat="server" Class="rotating" ImageUrl="~/DesktopModules/Versions/images/hourglass.png" Width="45" />
                    </div>
                    <div class="wiFooter">
                        <telerik:RadButton ID="analysis_cancel" runat="server" Text="Cancel" OnClick="Cancel_Click"></telerik:RadButton>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" CssClass="contentModal" ID="confirmation">
                    <span style="font-size:15px; font-weight:bold">Errors and Warnings</span>
                    <br />
                    <asp:Label ID="lblCountWarningErrors" runat="server" />
                    <div class="scrollable">
                        <ul>
                            <asp:Repeater runat="server" ID="rptWarningsErrors"  OnItemDataBound="rptWarningsErrors_ItemDataBound">
                                <ItemTemplate>
                                    <li><asp:Label ID="lblErrorOrWarning" runat="server"></asp:Label></li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                    <div class="wiFooter" style="float: right; margin-top:10px">
                        <span>
                            <telerik:RadButton ID="btnConfirmUpload" runat="server" Text="Confirm upload" AutoPostBack="true" OnClick="Confirmation_Upload_OnClick" CssClass="WiInline"></telerik:RadButton>
                        </span>
                        <span>
                            <telerik:RadButton ID="Confirmation_cancel" runat="server" Text="Cancel" OnClick="Cancel_Click"></telerik:RadButton>
                        </span>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" CssClass="contentModal ConfirmationWindow" ID="state">
                    <div class="VersionCentered">
                        <asp:Label runat="server" ID="lblSaveStatus" Text="label" />
                    </div><br /><br />
                    <div class="VersionCentered">
                        <telerik:RadButton ID="state_confirmation" runat="server" Text="Close" OnClick="Cancel_Click"></telerik:RadButton>
                    </div>
                </asp:Panel>

                <script type="text/javascript">

                    /*-- TELERIK EVENTS --*/

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

                    function GetRadWindow() {
                        var oWindow = null;
                        if (window.radWindow) oWindow = window.radWindow;
                        else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                        return oWindow;
                    }

                    function closeRadWindow()
                    {
                        var oArg = new Object();
                        oArg.status = "success";

                        var oWnd = GetRadWindow();
                        oWnd.close(oArg);
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

                    /* Validations */
                    function PerformValidations() {
                        var isValid = true;
                        var hidIsRequiredValue = $('#hidIsRequired').val() == "True";
                        if (hidIsRequiredValue) {
                            var meetingControl = $find('<%= UploadMeeting.ClientID %>');
                            var selectedMeetingID = meetingControl.get_value().split("|")[0];
                            if (selectedMeetingID <= 0) {
                                isValid = false;
                                $('#meetingRequiredMsg').show();
                            }
                        }
                        return isValid;
                    }
                </script>
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
