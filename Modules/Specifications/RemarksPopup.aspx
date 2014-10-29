﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemarksPopup.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.RemarksPopup" %>

<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />    
    <title>Remarks</title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="JS/jquery.min.js"></script>
    <script src="JS/jquery-validate.min.js"></script>
    <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
</head>
<body>
    <form id="frmRemarksPopup" runat="server">
            <telerik:RadScriptManager runat="server" ID="rsmRemarksPopup" EnableHandlerDetection="false" />
            <telerik:RadAjaxManager ID="ramRemarksPopup" runat="server" EnablePageHeadUpdate="false" UpdatePanelsRenderMode="Inline">
            </telerik:RadAjaxManager>
            <asp:Panel ID="pnlErrorMessage" runat="server" Visible="false" CssClass="Spec_Edit_Error">
                <asp:Label runat="server" ID="lblErrorMessage" CssClass="ErrorTxt"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlSuccessMessage" runat="server" Visible="false" CssClass="Spec_Edit_Info">
                <asp:Label runat="server" ID="lblSuccessMessage" CssClass="InfoTxt"></asp:Label>
            </asp:Panel>
            <div class="contentModal" id="divRemarksPopup" style="padding: 5px;">
                <div class="center">
                    <asp:UpdatePanel ID="upContentRemarks" runat="server">
                        <ContentTemplate>
                            <ult:remarkscontrol runat="server" id="remarksControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="footer" id="remarksFooter" runat="server" style="position: absolute; bottom: 10px; right: 20px;">
                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn3GPP-success" OnClick="btnSave_Click"/>
                    <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="btn3GPP-success" OnClientClick="return window.close();" />
                </div>
            </div>
    </form>
</body>
</html>
