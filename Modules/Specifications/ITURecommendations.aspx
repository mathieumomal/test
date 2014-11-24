<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ITURecommendations.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.ITURecommendation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ITU Recommendation</title>
    <link rel="stylesheet" type="text/css" href="/Portals/_default/Skins/3GPP/mainpage.css"/>
    <link rel="stylesheet" type="text/css" href="CSS/ItuRecommendations.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="JS/jquery.min.js"></script>
</head>
<body id="ituRecommendationsContent">
    <form id="formItuRecommendation" runat="server">
        <telerik:RadScriptManager runat="server" ID="rsm" EnableHandlerDetection="false" />
        <asp:Panel ID="pnlMsg" runat="server" Visible="false">
            <asp:Label runat="server" ID="lblMsg"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="pnlItuRecommendations" runat="server">                
            <div class="formItuRec">
                <table>
                    <tr>
                        <td class="TabLineLeft">
                            <asp:Label ID="lblItuRec" runat="server">ITU Recommendation <span class="mandatoryIndicator" title="Mandatory field">*</span>: </asp:Label>
                        </td>
                        <td class="TabLineRight" colspan="3">
                            <telerik:RadComboBox
                                ID="rcbItuRec"
                                runat="server"
                                Width="200"
                                AutoPostBack="False">
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TabLineLeft">
                            <asp:Label ID="lblStartRelease" runat="server">Start Release <span class="mandatoryIndicator" title="Mandatory field">*</span>: </asp:Label>
                        </td>
                        <td class="TabLineRight">
                            <telerik:RadComboBox
                                ID="rcbStartRelease"
                                runat="server"
                                Width="200"
                                AutoPostBack="False">
                            </telerik:RadComboBox>
                        </td>
                        <td class="TabLineLeft">
                            <asp:Label ID="lblEndRelease" runat="server">End Release <span class="mandatoryIndicator" title="Mandatory field">*</span>: </asp:Label>
                        </td>
                        <td class="TabLineRight">
                            <telerik:RadComboBox
                                ID="rcbEndRelease"
                                runat="server"
                                Width="200"
                                AutoPostBack="False">
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TabLineLeft">
                            <asp:Label ID="lblSaMeeting" runat="server">SA-Meeting <span class="mandatoryIndicator" title="Mandatory field">*</span>: </asp:Label>
                        </td>
                        <td class="TabLineRight" colspan="3">
                            <ult:MeetingControl runat="server" ID="RcbSaMeeting" /> 
                        </td>
                    </tr>
                    <tr>
                        <td class="TabLineLeft">
                            <asp:Label ID="lblSeedFile" runat="server">Seed file <span class="mandatoryIndicator" title="Mandatory field">*</span>: </asp:Label>
                        </td>
                        <td class="TabLineRight" colspan="3">
                            <telerik:RadAsyncUpload runat="server" 
                                ID="rdauUploadSeedFile" 
                                AllowedFileExtensions="docx"
                                MultipleFileSelection="Automatic" 
                                inputSize="68"
                                MaxFileInputsCount="1"
                                PostbackTriggers="btnExport"
                                OnClientvalidationFailed="OnClientValidationFailed"
                                OnClientFileUploaded="validateForm"
                                OnClientFileUploadRemoved="validateForm"/>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="btns">
                <asp:Button ID="btnPreliminary" Enabled="False" runat="server" CssClass="btn3GPP-default" Text="Preliminary Q.1741"/>
                <asp:Button ID="btnExport" OnClick="btnExport_OnClick" runat="server" CssClass="btn3GPP-default" Text="Export list"/>
            </div>
        </asp:Panel>
        <telerik:RadAjaxManager ID="rajxItuRec" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="rcbSaMeeting">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="pnlItuRecommendations"/>
                        <telerik:AjaxUpdatedControl ControlID="pnlMsg" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="btnExport">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="pnlItuRecommendations"/>
                        <telerik:AjaxUpdatedControl ControlID="pnlMsg" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
    </form>
    <!--scripts-->
        <script type="text/javascript" src="JS/jquery.min.js"></script>
        <script type="text/javascript" src="JS/jquery-validate.min.js"></script>
        <script type="text/javascript" src="JS/ItuRecommendations.js"></script>
    <!--scripts-->
</body>
</html>
