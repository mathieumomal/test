<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.Release.ReleaseDetails" culture="auto" meta:resourcekey="ReleaseDetailsResource" uiculture="auto" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>    
</head>
<body>
    <form id="ReleaseDetailsForm" runat="server">
       <asp:Panel ID="releaseWarning" runat="server" CssClass="releaseDetailsWarning" Visible="false" Height="100%">
           <span class="releaseDetailsWarningTxt">No data available for the current query.</span>
       </asp:Panel>
       <asp:Panel ID="releaseDetailsBody" runat="server" CssClass="releaseDetailsBody">
            <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
            <telerik:RadTabStrip ID="ReleaseDetailRadTabStrip" runat="server" MultiPageID="ReleaseDetailRadMultiPage" 
            AutoPostBack="false">    
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="ReleaseDetailRadMultiPage" runat="server" Height="550px" Width="100%" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
                <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true">        
                   <table style="width: 100%">
                        <tr>
                            <td class="TabLineLeft" ><asp:Label ID="releaseCodeLbl" runat="server" ControlName="releaseCodeLbl" Text="Release code:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight" id="SecondColreleaseCode"><asp:Label ID="releaseCodeVal" runat="server" ControlName="releaseCodeVal" ></asp:Label></td>
                        </tr>                              
                        <tr>
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStatusLbl" runat="server" ControlName="ReleaseStatusLbl" Text="Status:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseStatusVal" runat="server" ControlName="ReleaseStatusVal"></asp:Label></td>
                        </tr>            
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseNameLbl" runat="server" ControlName="ReleaseNameLbl" Text="Release name:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseNameVal" runat="server" ControlName="ReleaseNameVal" CssClass="releaseName"></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseDescLbl" runat="server" ControlName="ReleaseDescLbl" Text="Release description:"></asp:Label>  </td>          
                            <td colspan="2" class="TabLineRight">
                                <img runat="server" id="ReleaseDescVal" alt="Go to description" src="images/ReleaseDecription.png"/>
                                <asp:Label ID="MissigDesc" runat="server" Visible="false"/>
                            </td>
            
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseShortNameLbl" runat="server" ControlName="ReleaseShortNameLbl" Text="Release short name:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseShortNameVal" runat="server" ControlName="ReleaseShortNameVal"></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStartDateLbl" runat="server" ControlName="ReleaseStartDateLbl" Text="Start date:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:Label ID="ReleaseStartDateVal" runat="server" ControlName="ReleaseStartDateVal"></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="3" class="TabLineRightFreeze">
                                <asp:Panel ID="FreezeStagesPanel" runat="server">
                                    <fieldset id="FreezeFieldset">
                                        <legend><asp:Label ID="FreezeMeetingLbl" runat="server" ControlName="FreezeMeetingLbl" Text="Freeze meetings and dates"></asp:Label></legend>
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="display: inline-block; text-align: right" id="FirstColFreezeStage1"><asp:Label ID="ReleaseFreezeStage1Lbl" runat="server" ControlName="ReleaseFreezeStage1Lbl" Text="Stage1:"></asp:Label></td>
                                                <td style="text-align: left" id="SecndColFreezeStage1"><asp:Label ID="ReleaseFreezeStage1Meeting" runat="server" ControlName="ReleaseFreezeStage1Meeting"></asp:Label></td>
                                                <td ><asp:Label ID="ReleaseFreezeStage1Date" runat="server" ControlName="ReleaseFreezeStage1Date"></asp:Label></td>
                                            </tr>                                        
                                            <tr>
                                                <td style="display: inline-block; text-align: right" id="FirstColFreezeStage2"><asp:Label ID="ReleaseFreezeStage2Lbl" runat="server" ControlName="ReleaseFreezeStage2Lbl" Text="Stage2:"></asp:Label></td>
                                                <td style="text-align: left" id="SecndColFreezeStage2"><asp:Label ID="ReleaseFreezeStage2Meeting" runat="server" ControlName="ReleaseFreezeStage2Meeting"></asp:Label></td>
                                                <td ><asp:Label ID="ReleaseFreezeStage2Date" runat="server" ControlName="ReleaseFreezeStage2Date"></asp:Label></td>
                                            </tr>                                        
                                            <tr>
                                                <td style="display: inline-block; text-align: right" id="FirstColFreezeStage3"><asp:Label ID="ReleaseFreezeStage3Lbl" runat="server" ControlName="ReleaseFreezeStage3Lbl" Text="Stage3:"></asp:Label></td>
                                                <td style="text-align: left" id="SecndColFreezeStage3"><asp:Label ID="ReleaseFreezeStage3Meeting" runat="server" ControlName="ReleaseFreezeStage3Meeting"></asp:Label></td>
                                                <td><asp:Label ID="ReleaseFreezeStage3Date" runat="server" ControlName="ReleaseFreezeStage3Date"></asp:Label></td>
                                            </tr>
                                        </table>
                                        <div>
                                            
                                        </div>
                                    </fieldset>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseEndDateLbl" runat="server" ControlName="ReleaseEndDateLbl" Text="End date:"></asp:Label></td>
                            <td class="TabLine3colRight"><asp:Label ID="ReleaseEndDateMeetingVal" runat="server" ControlName="ReleaseEndDateMeetingVal"></asp:Label></td>
                            <td class="TabLine3colRight"><asp:Label ID="ReleaseEndDateVal" runat="server" ControlName="ReleaseEndDateVal"></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseClosureDateLbl" runat="server" ControlName="ReleaseClosureDateLbl" Text="Closure date:"></asp:Label></td>
                            <td><asp:Label ID="ReleaseClosureDateMeetingVal" runat="server" ControlName="ReleaseClosureDateMeetingVal"></asp:Label></td>
                            <td><asp:Label ID="ReleaseClosureDateVal" runat="server" ControlName="ReleaseClosureDateVal"></asp:Label></td>
                        </tr>
                        <tr style="max-height: 150px; overflow-y: scroll"> 
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
                                <asp:Label ID="previousReleaseLbl" runat="server" ControlName="previousReleaseLbl" Text="Follows release:" CssClass="TabLabel"></asp:Label>                                
                            </td>
                            <td class="TabLineRight">   
                                <asp:Label ID="previousReleaseVal" runat="server" ControlName="previousReleaseVal" CssClass="TabValue"></asp:Label>                                                                                                 
                            </td>
                        </tr>
                        <tr class="TabLine">
                            <td class="TabLineLeft">                                
                                <asp:Label ID="ITURCodeLbl" runat="server" ControlName="ITURCodeLbl" Text="ITUR code:"></asp:Label> 
                            </td>
                            <td class="TabLineRight">   
                                <asp:Label ID="ITURCodeVal" runat="server" ControlName="ITURCodeVal"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="TabLineRight">
                                <fieldset>
                                    <legend ><asp:Label ID="ReleaseVersionLbl" runat="server" ControlName="ReleaseVersionLbl" Text="Versions"></asp:Label></legend>
                                    <table class="versionFieldsetTab" style="width:100%">                                                                            
                                        <tr>
                                            <td style="width: 30%; padding-left: 10%">
                                                <asp:Label ID="Release2GLbl" runat="server" ControlName="Release2GLbl" Text="2G:"></asp:Label>
                                                <asp:Label ID="Release2GVal" runat="server" ControlName="Release2GVal"></asp:Label>
                                            </td>                                                                                        
                                            <td style="width: 70%; padding-left: 30%">
                                                <asp:Label ID="Release2GDecimalLbl" runat="server" ControlName="Release2GDecimalLbl" Text="2G Decimal:"></asp:Label>
                                                <asp:Label ID="Release2GDecimalVal" runat="server" ControlName="Release2GDecimalVal"></asp:Label>
                                            </td>                                           
                                        </tr>                                                                                                              
                                        <tr>
                                            
                                            <td style="width: 30%; padding-left: 10%">
                                                <asp:Label ID="Release3GLbl" runat="server" ControlName="Release3GLbl" Text="3G:"></asp:Label>
                                                <asp:Label ID="Release3GVal" runat="server" ControlName="Release3GVal"></asp:Label>
                                            </td>  
                                            <td style="width: 70%; padding-left: 30%">
                                                <asp:Label ID="Release3GDecimalLbl" runat="server" ControlName="Release3GDecimalLbl" Text="3G Decimal:"></asp:Label>
                                                <asp:Label ID="Release3GDecimalVal" runat="server" ControlName="Release3GDecimalVal"></asp:Label>
                                            </td>                                                                                       
                                        </tr>
                                    </table>                                    
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" class="TabLineRight">
                                <fieldset>
                                    <legend><asp:Label ID="WPMCodesLbl" runat="server" ControlName="WPMCodesLbl" Text="WPM codes" ></asp:Label></legend>
                                    <table class="versionFieldsetTab" style="width:100%">
                                        <tr>
                                            <td style="width: 40%; padding-left: 10%">                              
                                                <asp:Label ID="WPMCodes2GLbl" runat="server" ControlName="WPMCodes2GLbl" Text="2G:"></asp:Label>
                                                <asp:Label ID="WPMCodes2GVal" runat="server" ControlName="WPMCodes2GVal"></asp:Label>                                                                
                                            </td>
                                            <td style="width: 60%; padding-left: 30%"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 40%; padding-left: 10%">                                
                                                <asp:Label ID="WPMCodes3GLbl" runat="server" ControlName="WPMCodes3GLbl" Text="3G:"></asp:Label>
                                                <asp:Label ID="WPMCodes3GVal" runat="server" ControlName="WPMCodes3GVal"></asp:Label>                                                                
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
                <asp:LinkButton ID="EditBtn" runat="server" Text="Edit" CssClass="LinkButton" Visible="false"/>
                <asp:LinkButton ID="FreezeReleaseBtn" runat="server" Text="Freeze Release" CssClass="LinkButton" Visible="false"/>
                <asp:LinkButton ID="CloseReleaseBtn" runat="server" Text="Close Release" CssClass="LinkButton" Visible="false"/>
                <asp:LinkButton ID="ExitBtn" runat="server" Text="Exit" CssClass="LinkButton"/>
           </div> 
           <script type="text/javascript">
               function resizeElements() {
                   //window.resizeTo(640, 480)
                   //var winSize = $(window).height() - $("#FreezeStagesPanel".height());
                   //alert(winSize);
                   //$(window).height(winSize);
                   var calWidth = $("#FreezeStagesPanel").width() - $("#SecondColreleaseCode").width() - ($("#FreezeStagesPanel").width() * 0.03) -15;
                   $("#FirstColFreezeStage1").width(calWidth); //SecndColFreezeStage1
                   $("#SecndColFreezeStage1").width(($("#FreezeStagesPanel").width()-calWidth)*0.45);
                   $("#FirstColFreezeStage2").width(calWidth);
                   $("#SecndColFreezeStage2").width(($("#FreezeStagesPanel").width() - calWidth) * 0.45);
                   $("#FirstColFreezeStage3").width(calWidth);
                   $("#SecndColFreezeStage3").width(($("#FreezeStagesPanel").width() - calWidth) * 0.45);
               }
               $(document).ready(function () {                  
                   setTimeout(function () {
                       var releaseName = "Release " + $("#releaseCodeVal").html();
                       document.title = releaseName;                       
                       resizeElements();
                   }, 200);

                   $(window).resize(function () {                       
                       resizeElements();                       
                   });
               });
        </script>  
       </asp:Panel>
                
    </form>    
</body>
</html>
