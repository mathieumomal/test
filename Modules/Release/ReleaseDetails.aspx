<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.Release.ReleaseDetails" culture="auto" meta:resourcekey="ReleaseDetailsResource" uiculture="auto" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="module.css"/>
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon"/>
    <script src="JS/jquery.min.js"></script>    
    <script type="text/javascript">

        function closeAllModals() {
            var manager = GetRadWindowManager();
            manager.closeAll();
        }
    </script>
</head>
<body id="releaseDetailBody">
    <form id="ReleaseDetailsForm" runat="server">
       <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="650px">
       <asp:Panel ID="releaseWarning" runat="server" CssClass="releaseDetailsWarning" Visible="false">
           <span class="releaseDetailsWarningTxt">No data available for the current query.</span>
       </asp:Panel>
       <asp:Panel ID="releaseDetailsBody" runat="server" CssClass="releaseDetailsBody">
            <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
            <telerik:RadTabStrip ID="ReleaseDetailRadTabStrip" runat="server" MultiPageID="ReleaseDetailRadMultiPage" 
            AutoPostBack="false">    
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="ReleaseDetailRadMultiPage" runat="server" Width="100%" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
                <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true">        
                   <table style="width: 100%">
                        <tr>
                            <td class="TabLineLeft" ><asp:Label ID="releaseCodeLbl" runat="server" Text="Release code:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight" id="SecondColreleaseCode"><asp:Label ID="releaseCodeVal" runat="server" ></asp:Label></td>
                        </tr>                              
                        <tr>
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStatusLbl" runat="server"  Text="Status:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseStatusVal" runat="server" ></asp:Label></td>
                        </tr>            
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseNameLbl" runat="server"  Text="Release name:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseNameVal" runat="server"  CssClass="releaseName"></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseDescLbl" runat="server" CssClass="lblTooltipStyleDark" Text="Release description:" ToolTip="URL of Release description document"></asp:Label>  </td>          
                            <td colspan="2" class="TabLineRight">
                                <asp:hyperlink id="lnkReleaseDescription" runat="server" Target="_blank">
                                   <img runat="server" id="ReleaseDescVal" alt="Go to description" src="images/ReleaseDecription.png"/>
                                </asp:hyperlink>
                                <asp:Label ID="MissigDesc" runat="server" Visible="false"/>
                            </td>
            
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseShortNameLbl" runat="server"  Text="Release short name:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseShortNameVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStartDateLbl" runat="server"  Text="Start date:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseStartDateVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="3" class="TabLineRightFreeze">
                                <asp:Panel ID="FreezeStagesPanel" runat="server">
                                    <fieldset id="FreezeFieldset">
                                        <legend><asp:Label ID="FreezeMeetingLbl" runat="server"  Text="Freeze meetings and dates"></asp:Label></legend>
                                        <table style="width: 100%" id="FrezeStagesTable">
                                            <tr>
                                                <td style="display: inline-block; text-align: right" class="FirstColFreezeStageDetails"><asp:Label ID="ReleaseFreezeStage1Lbl" runat="server"  Text="Stage1:"></asp:Label></td>
                                                <td style="text-align: left" class="DetailsColFreezeStage">
                                                    <asp:Label ID="ReleaseFreezeStage1Meeting" runat="server"  CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage1" class="DetailsColFreezeStage">
                                                    <asp:Label ID="ReleaseFreezeStage1Date" runat="server" ></asp:Label>
                                                </td>
                                            </tr>                                        
                                            <tr>
                                                <td style="display: inline-block; text-align: right" class="FirstColFreezeStageDetails"><asp:Label ID="ReleaseFreezeStage2Lbl" runat="server"  Text="Stage2:"></asp:Label></td>
                                                <td style="text-align: left" class="DetailsColFreezeStage">
                                                    <asp:Label ID="ReleaseFreezeStage2Meeting" runat="server"  CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage2" class="DetailsColFreezeStage">
                                                    <asp:Label ID="ReleaseFreezeStage2Date" runat="server" ></asp:Label>
                                                </td>
                                            </tr>                                        
                                            <tr>
                                                <td style="display: inline-block; text-align: right" class="FirstColFreezeStageDetails"><asp:Label ID="ReleaseFreezeStage3Lbl" runat="server"  Text="Stage3:"></asp:Label></td>
                                                <td style="text-align: left" class="DetailsColFreezeStage">
                                                    <asp:Label ID="ReleaseFreezeStage3Meeting" runat="server"  CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage3" class="DetailsColFreezeStage">
                                                    <asp:Label ID="ReleaseFreezeStage3Date" runat="server" ></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                        <div>
                                            
                                        </div>
                                    </fieldset>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseEndDateLbl" runat="server"  Text="End date:"></asp:Label></td>
                            <td class="TabLine2colRight"><asp:Label ID="ReleaseEndDateMeetingVal" runat="server" ></asp:Label></td>
                            <td class="TabLine3colRight"><asp:Label ID="ReleaseEndDateVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseClosureDateLbl" runat="server"  Text="Closure date:"></asp:Label></td>
                            <td class="TabLine2colRight"><asp:Label ID="ReleaseClosureDateMeetingVal" runat="server" ></asp:Label></td>
                            <td class="TabLine3colRight"><asp:Label ID="ReleaseClosureDateVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr style="max-height: 150px; overflow-y: scroll; margin-top:10px"> 
                            <td colspan="3" class="releaseRemarks">
                                <ult:RemarksControl runat="server" ID="releaseRemarks" />   
                            </td>                                                    
                        </tr>
                   </table>
                 </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageAdministration" runat="server" Height="90%">
                    <table class="TabContent" style="width:95%">
                        <tr class="TabLine">
                            <td class="TabLineLeft">                                
                                <asp:Label ID="previousReleaseLbl" runat="server"  Text="Follows Release:" CssClass="TabLabel"></asp:Label>                                
                            </td>
                            <td class="TabLineRight">   
                                <asp:Label ID="previousReleaseVal" runat="server"  CssClass="TabValue"></asp:Label>                                                                                                 
                            </td>
                        </tr>
                        <tr class="TabLine">
                            <td class="TabLineLeft">                                
                                <asp:Label ID="ITURCodeLbl" runat="server" CssClass="lblTooltipStyleDark" Text="ITUR code:" ToolTip="For referencing in appropriate ITU Recommendations"></asp:Label> 
                            </td>
                            <td class="TabLineRight">   
                                <asp:Label ID="ITURCodeVal" runat="server" ></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="TabLineRight">
                                <fieldset>
                                    <legend ><asp:Label ID="ReleaseVersionLbl" runat="server"  Text="Versions"></asp:Label></legend>
                                    <table class="versionFieldsetTab" style="width:100%">                                                                            
                                        <tr>
                                            <td style="width: 30%; padding-left: 10%">
                                                <asp:Label ID="Release2GLbl" runat="server"  Text="2G:"></asp:Label>
                                                <asp:Label ID="Release2GVal" runat="server" ></asp:Label>
                                            </td>                                                                                        
                                            <td style="width: 70%; padding-left: 30%">
                                                <asp:Label ID="Release2GDecimalLbl" runat="server"  Text="2G Decimal:"></asp:Label>
                                                <asp:Label ID="Release2GDecimalVal" runat="server" ></asp:Label>
                                            </td>                                           
                                        </tr>                                                                                                              
                                        <tr>
                                            
                                            <td style="width: 30%; padding-left: 10%">
                                                <asp:Label ID="Release3GLbl" runat="server"  Text="3G:"></asp:Label>
                                                <asp:Label ID="Release3GVal" runat="server" ></asp:Label>
                                            </td>  
                                            <td style="width: 70%; padding-left: 30%">
                                                <asp:Label ID="Release3GDecimalLbl" runat="server"  Text="3G Decimal:"></asp:Label>
                                                <asp:Label ID="Release3GDecimalVal" runat="server" ></asp:Label>
                                            </td>                                                                                       
                                        </tr>
                                    </table>                                    
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="TabLineRight">
                                <fieldset>
                                    <legend><asp:Label ID="WPMCodesLbl" runat="server"  Text="WPM codes" ></asp:Label></legend>
                                    <table class="versionFieldsetTab" style="width:100%">
                                        <tr>
                                            <td style="width: 40%; padding-left: 10%">                              
                                                <asp:Label ID="WPMCodes2GLbl" runat="server"  Text="2G:"></asp:Label>
                                                <asp:Label ID="WPMCodes2GVal" runat="server" ></asp:Label>                                                                
                                            </td>
                                            <td style="width: 60%; padding-left: 30%"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 40%; padding-left: 10%">                                
                                                <asp:Label ID="WPMCodes3GLbl" runat="server"  Text="3G:"></asp:Label>
                                                <asp:Label ID="WPMCodes3GVal" runat="server" ></asp:Label>                                                                
                                            </td>
                                            <td style="width: 60%; padding-left: 30%"></td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                        </tr>
                    </table>
                 </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageHistory" runat="server" Height="90%">
                    <div class="TabContent" style="overflow-y: auto; overflow-x: auto">
                        <ult:HistoryControl runat="server" ID="releaseHistory" />
                    </div>
                 </telerik:RadPageView>
            </telerik:RadMultiPage>     
           <div class="releaseDetailsAction">
                <asp:LinkButton ID="EditBtn" runat="server" Text="Edit" CssClass="btn3GPP-success" Visible="false" OnClick="EditReleaseDetails_Click" />
                <asp:LinkButton ID="FreezeReleaseBtn" runat="server" Text="Freeze Release" CssClass="btn3GPP-success" Visible="false"/>
                <asp:LinkButton ID="CloseReleaseBtn" runat="server" Text="Close Release" CssClass="btn3GPP-success" Visible="false"/>
                <asp:LinkButton ID="ExitBtn" runat="server" Text="Exit" CssClass="btn3GPP-success" OnClientClick=" return closePopUpWindow()"/>
           </div> 
           <script type="text/javascript">
               /* Exit function */
               function closePopUpWindow() {
                   window.close();
               }

               //Freeze a release : display popup
               function freezeRelease() {
                   closeAllModals();
                   window.radopen(null, "RadWindow_FreezeConfirmation");
               }

               //Close release : display popup
               function closeRelease() {
                   closeAllModals();
                   window.radopen(null, "RadWindow_ClosureConfirmation");
               }

               $(document).ready(function () {
                   setTimeout(function () {
                       var releaseName = "Release " + $("#releaseCodeVal").html();
                       document.title = releaseName;                                              
                   }, 200);
               });
        </script>  
       </asp:Panel>
       <telerik:RadAjaxManager ID="RadAjaxManager" runat="server" EnablePageHeadUpdate="false">
       </telerik:RadAjaxManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" >
            <Windows>
                <telerik:RadWindow ID="RadWindow_FreezeConfirmation"  runat="server" Modal="true" Title="Freeze Confirmation" Height="230" Width="400" VisibleStatusbar="false" iconUrl="false">
                    <ContentTemplate>
                        <div class="contentModal" id="divFreezeConfirmation">
                            <div class="header">
                                You are about to freeze the Release.
                            </div>
                            <br />
                            <div class="center">
                                <b>WARNING</b>
                                <br />
                                <asp:Label ID="Freeze_VersionsPendingUpload" runat="server">#</asp:Label> Versions are pending upload.
                                <br />
                                <asp:Label ID="Freeze_CRsNotInFinalStatus" runat="server">#</asp:Label> CRs are not in final status.
                                <br /><br />
                                End meeting: <ult:MeetingControl runat="server" ID="mcFreeze" DisplayLabel="true" />
                            </div>
                            <br />
                            <div class="footer" style="text-align: right">
                                <asp:Button ID="btnConfirmFreeze" CssClass="btn3GPP-success" Text ="Confirm" OnClick="btnConfirmFreeze_Click" runat="server"/>
                                <asp:Button ID="btnCancelFreeze" CssClass="btn3GPP-default" runat="server" Text ="Cancel" />
                            </div>
                        </div>
                    </ContentTemplate>
                </telerik:RadWindow>
                <telerik:RadWindow ID="RadWindow_ClosureConfirmation"  runat="server" Modal="true" Title="Closure Confirmation" Height="280" Width="410" VisibleStatusbar="false" iconUrl="false" Behaviors="Close">
                    <ContentTemplate>
                        <div class="contentModal" id="divClosureConfirmation">
                            <div class="header">
                                You are about to close the release. It will no longer be possible to create change requests on any specification for this release.
                            </div>
                            <br />
                            <div class="center">
                                <div id="divWarnings">
                                <img src="images/warning.png" style="vertical-align:middle"/> <b>WARNING</b>
                                <br />
                                <asp:Label ID="Close_VersionsPendingUpload" runat="server">#</asp:Label> Versions are pending upload.
                                <br />
                                <asp:Label ID="Close_CRsNotInFinalStatus" runat="server">#</asp:Label> CRs are not in final status.
                                <br />
                                <asp:Label ID="Close_TDocsNotInFinalStatus" runat="server">#</asp:Label> TDocs are not in final status.
                                <br /><br />
                                </div>
                                Closure meeting: <ult:MeetingControl runat="server" ID="mcClose" DisplayLabel="true"/>
                            </div>
                            <br />
                            <div class="footer" style="text-align: right">
                                <asp:Button ID="btnConfirmClosure" CssClass="btn3GPP-success" runat="server" Text ="Confirm" OnClick="btnConfirmClosure_Click" />
                                <asp:Button ID="btnCancelClosure" CssClass="btn3GPP-default" runat="server" Text ="Cancel" OnClientClick="return close();" />
                            </div>
                        </div>
                    </ContentTemplate>
                </telerik:RadWindow>
            </Windows>
        </telerik:RadWindowManager>
       </asp:Panel>         
    </form>    
</body>
</html>
