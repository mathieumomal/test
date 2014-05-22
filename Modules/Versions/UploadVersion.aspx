<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadVersion.aspx.cs" Inherits="Etsi.Ultimate.Module.Versions.UploadVersion" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="JS/jquery.min.js"></script>
</head>
<body>
    <form id="VersionUploadForm" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="500px">
            <asp:Panel ID="versionUploadMessages" runat="server" Visible="false">
                <asp:Label runat="server" ID="specificationMessagesTxt"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="versionUploadBody" runat="server" CssClass="versionUploadBody">
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
                            <telerik:RadAsyncUpload runat="server" ID="FileToUploadVal" MaxFileInputsCount="1" AllowedFileExtensions="docx,doc,zip" PostbackTriggers="UploadBtn" />
                        </td>
                    </tr>
                    <tr>
                        <td class="TabLineLeft">
                            <asp:Label runat="server" ID="NewVersionLbl" Text="New version:" />
                        </td>
                        <td colspan="2" class="TabLineRight2Col">
                            <telerik:RadTextBox  runat="server" ID="NewVersionMajorVal" InputType ="Number" Tooltip="Major" Width="40px"/> 
                            <span>-</span>
                            <telerik:RadTextBox  runat="server" ID="NewVersionTechnicalVal" InputType ="Number" Tooltip="Technical" Width="40px"/> 
                            <span>-</span>
                            <telerik:RadTextBox  runat="server" ID="NewVersionEditorialVal" InputType ="Number" Tooltip="Editorial" Width="40px"/> 
                        </td>
                    </tr>
                    <tr>
                        <td class="TabLineLeft">
                            <asp:Label runat="server" ID="CommentLbl" Text="Comment:"/>
                        </td>
                        <td colspan="2" class="TabLineRight2Col">
                            <asp:TextBox runat="server" ID="CommentVal" EmptyMessage="Your comment" TextMode="MultiLine" Resize="Vertical" Width="195px"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="TabLineLeft">
                            <asp:Label runat="server" ID="MeetingLbl" Text="File to upload:" />
                        </td>
                        <td colspan="2" class="TabLineRight2Col">
                             <ult:MeetingControl runat="server" ID="UploadMeeting" />
                        </td>
                    </tr>
                </table>
                <div class="releaseDetailsAction">
                    <asp:LinkButton ID="UploadBtn" runat="server" Text="Upload" CssClass="btn3GPP-success" Visible="false"/>
                    <asp:LinkButton ID="UploadBtnDisabled" runat="server" Text="Upload" CssClass="btn3GPP-default" disabled="disabled" OnClientClick="return false;" />
                    <asp:LinkButton ID="ExitBtn" runat="server" Text="Cancel" CssClass="btn3GPP-success" OnClientClick="  return closePopUpWindow()"/>                        
                </div>
                <script type="text/javascript">

                    /* Exit function */
                    function closePopUpWindow() {
                        window.close();
                    }                    
                </script>
            </asp:Panel>            
        </asp:Panel>
    </form>
</body>
</html>
