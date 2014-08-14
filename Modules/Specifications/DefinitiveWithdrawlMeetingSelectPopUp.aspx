<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefinitiveWithdrawlMeetingSelectPopUp.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.DefinitiveWithdrawlMeetingSelectPopUp" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="stylesheet" type="text/css" href="module.css">
    <title>Withdrawal confirmation</title>
</head>
<body>
    
    <form id="form2" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnableHandlerDetection="false" />
        
        <script type="text/javascript">            
            function getRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            // Returned to the parent page with the operation resulat as argument
            function returnToParent(operationResult) {
                //create the argument that will be returned to the parent page
                var oArg = new Object();

                //set the argument value      
                oArg.OperationResult = operationResult;

                //Close the RadWindow and send the argument to the parent page                    
                getRadWindow().close(oArg);
            }           
        </script>
        <div class="specificationDetailsBody">
            <div class="contentModal" id="divWithdrawalMtgSelect" style="padding:5px;">
                <div class="header">               
                    You are about to withraw definitively this specification. All versions of the specification will be withdrawn independently of the release. <br/> <br/> Please select withdrawal meeting.                 
                </div>
                <div class="center">
                    <br />
                        <asp:Label ID="withrawalMeetinfLbl" runat="server" Text="Withdrawal meeting <span class='requiredField'>(*)</span>:"/>
                        <ult:MeetingControl runat="server" ID="withrawalMeetinfVal" />     
                    <br />
                     <asp:Label ID="lblError" runat="server" CssClass="MeetingError" Visible="false"> Meeting is mandatory.</asp:Label>
                </div>
                <br />
                <div class="footer" style="position:absolute; bottom:10px; right:20px;">
                    <asp:Button ID="btnConfirmWithdraw" Text ="Confirm" OnClick="btnConfirmWithdraw_Click" runat="server"/>
                    <asp:Button id="btnCancel" runat="server" Text ="Cancel" OnClientClick="return window.close();" />
                </div>
            </div>       
        </div>                
    </form>
</body>
</html>