<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.Release.ReleaseDetails" culture="auto" meta:resourcekey="ReleaseDetailsResource" uiculture="auto" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="ReleaseDetailsForm" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadTabStrip ID="ReleaseDetailRadTabStrip" runat="server" MultiPageID="ReleaseDetailRadMultiPage" Width="100%" Align="Justify"
        OnTabClick="ReleaseDetailRadTabStrip_Click" AutoPostBack="true">    
        </telerik:RadTabStrip>
        <telerik:RadMultiPage ID="ReleaseDetailRadMultiPage" runat="server" Width="100%" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
            <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true">        
                <div>                                
                    <asp:Label ID="releaseCodeLbl" runat="server" ControlName="releaseCodeLbl" Text="Release code:"></asp:Label>
                    <asp:Label ID="releaseCodeVal" runat="server" ControlName="releaseCodeVal"></asp:Label>
                </div>
                <div>            
                    <asp:Label ID="ReleaseStatusLbl" runat="server" ControlName="ReleaseStatusLbl" Text="Status:"></asp:Label>
                    <asp:Label ID="ReleaseStatusVal" runat="server" ControlName="ReleaseStatusVal"></asp:Label>
                </div>
                <div>            
                    <asp:Label ID="ReleaseNameLbl" runat="server" ControlName="ReleaseNameLbl" Text="Release name:"></asp:Label>
                    <asp:Label ID="ReleaseNameVal" runat="server" ControlName="ReleaseNameVal"></asp:Label>
                </div>
                <div>            
                    <asp:Label ID="ReleaseDescLbl" runat="server" ControlName="ReleaseDescLbl" Text="Release description:"></asp:Label>            
                    <img runat="server" id="ReleaseDescVal" alt="Go to description" />
            
                </div>
                <div>            
                    <asp:Label ID="ReleaseShortNameLbl" runat="server" ControlName="ReleaseShortNameLbl" Text="Release short name:"></asp:Label>
                    <asp:Label ID="ReleaseShortNameVal" runat="server" ControlName="ReleaseShortNameVal"></asp:Label>
                </div>
                <div>            
                    <asp:Label ID="ReleaseStartDateLbl" runat="server" ControlName="ReleaseStartDateLbl" Text="Start date:"></asp:Label>
                    <asp:Label ID="ReleaseStartDateVal" runat="server" ControlName="ReleaseStartDateVal"></asp:Label>
                </div>
                <div>
                    <fieldset>
                        <legend><asp:Label ID="FreezeMeetingLbl" runat="server" ControlName="FreezeMeetingLbl" Text="Freeze meetings and dates"></asp:Label></legend>
                        <div>
                            <asp:Label ID="ReleaseFreezeStage1Lbl" runat="server" ControlName="ReleaseFreezeStage1Lbl" Text="Stage1:"></asp:Label>
                            <asp:Label ID="ReleaseFreezeStage1Meeting" runat="server" ControlName="ReleaseFreezeStage1Meeting"></asp:Label>
                            <asp:Label ID="ReleaseFreezeStage1Date" runat="server" ControlName="ReleaseFreezeStage1Date"></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="ReleaseFreezeStage2Lbl" runat="server" ControlName="ReleaseFreezeStage2Lbl" Text="Stage1:"></asp:Label>
                            <asp:Label ID="ReleaseFreezeStage2Meeting" runat="server" ControlName="ReleaseFreezeStage2Meeting"></asp:Label>
                            <asp:Label ID="ReleaseFreezeStage2Date" runat="server" ControlName="ReleaseFreezeStage2Date"></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="ReleaseFreezeStage3Lbl" runat="server" ControlName="ReleaseFreezeStage3Lbl" Text="Stage3:"></asp:Label>
                            <asp:Label ID="ReleaseFreezeStage3Meeting" runat="server" ControlName="ReleaseFreezeStage3Meeting"></asp:Label>
                            <asp:Label ID="ReleaseFreezeStage3Date" runat="server" ControlName="ReleaseFreezeStage3Date"></asp:Label>
                        </div>
                    </fieldset>
                </div>
                <div>            
                    <asp:Label ID="ReleaseEndDateLbl" runat="server" ControlName="ReleaseEndDateLbl" Text="End date:"></asp:Label>
                    <asp:Label ID="ReleaseEndDateVal" runat="server" ControlName="ReleaseEndDateVal"></asp:Label>
                </div>
                <div>            
                    <asp:Label ID="ReleaseClosureDateLbl" runat="server" ControlName="ReleaseClosureDateLbl" Text="Closure Date"></asp:Label>
                    <asp:Label ID="ReleaseClosureDateVal" runat="server" ControlName="ReleaseClosureDateVal"></asp:Label>
                </div>
                <div>
                    <fieldset>
                        <legend><asp:Label ID="RemarksLbl" runat="server" ControlName="RemarksLbl" Text="Remarks"></asp:Label></legend>
                        <!-- Remarks control -->
                    </fieldset>
                </div>
             </telerik:RadPageView>
            <telerik:RadPageView ID="RadPageAdministration" runat="server" Selected="true">
             </telerik:RadPageView>
            <telerik:RadPageView ID="RadPageHistory" runat="server" Selected="true">
             </telerik:RadPageView>
        </telerik:RadMultiPage>        
        <asp:LinkButton ID="EditBtn" runat="server" Text="Edit"/>
        <asp:LinkButton ID="FreezeReleaseBtn" runat="server" Text="Freeze"/>
        <asp:LinkButton ID="CloseReleaseBtn" runat="server" Text="Close"/>
        <asp:LinkButton ID="ExitBtn" runat="server" Text="Exit"/>
    </form>
</body>
</html>
