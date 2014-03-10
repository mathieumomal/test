﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.Release.ReleaseDetails" culture="auto" meta:resourcekey="ReleaseDetailsResource" uiculture="auto" %>
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
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>    
    <script type="text/javascript">

        function closeAllModals() {
            var manager = GetRadWindowManager();
            manager.closeAll();
        }
    </script>
</head>
<body class="releaseDetailBody">
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
                                        <table style="width: 100%" id="FrezeStagesTable">
                                            <tr>
                                                <td style="display: inline-block; text-align: right" id="FirstColFreezeStage1"><asp:Label ID="ReleaseFreezeStage1Lbl" runat="server" ControlName="ReleaseFreezeStage1Lbl" Text="Stage1:"></asp:Label></td>
                                                <td style="padding-left: 10px; text-align: left" id="SecndColFreezeStage1">
                                                    <asp:Label ID="ReleaseFreezeStage1Meeting" runat="server" ControlName="ReleaseFreezeStage1Meeting" CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage1">
                                                    <asp:Label ID="ReleaseFreezeStage1Date" runat="server" ControlName="ReleaseFreezeStage1Date"></asp:Label>
                                                </td>
                                            </tr>                                        
                                            <tr>
                                                <td style="display: inline-block; text-align: right" id="FirstColFreezeStage2"><asp:Label ID="ReleaseFreezeStage2Lbl" runat="server" ControlName="ReleaseFreezeStage2Lbl" Text="Stage2:"></asp:Label></td>
                                                <td style="padding-left: 10px; text-align: left" id="SecndColFreezeStage2">
                                                    <asp:Label ID="ReleaseFreezeStage2Meeting" runat="server" ControlName="ReleaseFreezeStage2Meeting" CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage2">
                                                    <asp:Label ID="ReleaseFreezeStage2Date" runat="server" ControlName="ReleaseFreezeStage2Date"></asp:Label>
                                                </td>
                                            </tr>                                        
                                            <tr>
                                                <td style="display: inline-block; text-align: right" id="FirstColFreezeStage3"><asp:Label ID="ReleaseFreezeStage3Lbl" runat="server" ControlName="ReleaseFreezeStage3Lbl" Text="Stage3:"></asp:Label></td>
                                                <td style="padding-left: 10px; text-align: left" id="SecndColFreezeStage3">
                                                    <asp:Label ID="ReleaseFreezeStage3Meeting" runat="server" ControlName="ReleaseFreezeStage3Meeting" CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage3">
                                                    <asp:Label ID="ReleaseFreezeStage3Date" runat="server" ControlName="ReleaseFreezeStage3Date"></asp:Label>
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
                            <td class="TabLineLeft"><asp:Label ID="ReleaseEndDateLbl" runat="server" ControlName="ReleaseEndDateLbl" Text="End date:"></asp:Label></td>
                            <td class="TabLine3colRight"><asp:Label ID="ReleaseEndDateMeetingVal" runat="server" ControlName="ReleaseEndDateMeetingVal"></asp:Label></td>
                            <td class="TabLine3colRight"><asp:Label ID="ReleaseEndDateVal" runat="server" ControlName="ReleaseEndDateVal"></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseClosureDateLbl" runat="server" ControlName="ReleaseClosureDateLbl" Text="Closure date:"></asp:Label></td>
                            <td><asp:Label ID="ReleaseClosureDateMeetingVal" runat="server" ControlName="ReleaseClosureDateMeetingVal"></asp:Label></td>
                            <td><asp:Label ID="ReleaseClosureDateVal" runat="server" ControlName="ReleaseClosureDateVal"></asp:Label></td>
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
                                <asp:Label ID="previousReleaseLbl" runat="server" ControlName="previousReleaseLbl" Text="Follows Release:" CssClass="TabLabel"></asp:Label>                                
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
                <asp:LinkButton ID="EditBtn" runat="server" Text="Edit" CssClass="LinkButton" Visible="true" OnClick="EditReleaseDetails_Click"/>
                <asp:LinkButton ID="FreezeReleaseBtn" runat="server" Text="Freeze Release" CssClass="LinkButton" Visible="false"/>
                <asp:LinkButton ID="CloseReleaseBtn" runat="server" Text="Close Release" CssClass="LinkButton" Visible="false"/>
                <asp:LinkButton ID="ExitBtn" runat="server" Text="Exit" CssClass="LinkButton" OnClick="CloseReleaseDetails_Click"/>
           </div> 
           <script type="text/javascript">
               function resizeElements() {
                  
                   var calWidth = $("#FreezeStagesPanel").width() - $("#SecondColreleaseCode").width() - ($("#FreezeStagesPanel").width() * 0.03) - 17;
                   
                   $("#thirdColFreezeStage1").width(($("#FrezeStagesTable").width() - calWidth) * 0.5);
                   $("#SecndColFreezeStage1").width(($("#FrezeStagesTable").width() - calWidth) * 0.5 - 20);
                   $("#FirstColFreezeStage1").width(calWidth); //SecndColFreezeStage1

                   $("#thirdColFreezeStage2").width(($("#FrezeStagesTable").width() - calWidth) * 0.5);
                   $("#SecndColFreezeStage2").width(($("#FrezeStagesTable").width() - calWidth) * 0.5 - 20);
                   $("#FirstColFreezeStage2").width(calWidth);

                   $("#thirdColFreezeStage3").width(($("#FrezeStagesTable").width() - calWidth) * 0.5);
                   $("#SecndColFreezeStage3").width(($("#FrezeStagesTable").width() - calWidth) * 0.5 - 20);
                   $("#FirstColFreezeStage3").width(calWidth);                   
               }
               $(document).ready(function () {
                   resizeElements();
                   setTimeout(function () {
                       var releaseName = "Release " + $("#releaseCodeVal").html();
                       document.title = releaseName;                                              
                   }, 200);

                   $(window).resize(function () {                       
                       resizeElements();                       
                   });
                   $('#FreezeReleaseBtn').click(function (event) {
                       event.preventDefault();
                       closeAllModals();
                       window.radopen(null, "RadWindow_workItemImport");
                   });
                   $('#CloseReleaseBtn').click(function (event) {
                       event.preventDefault();
                       closeAllModals();
                       window.radopen(null, "RadWindow_ClosureConfirmation");
                   });
               });
        </script>  
       </asp:Panel>
       <telerik:RadAjaxManager ID="RadAjaxManager" runat="server" EnablePageHeadUpdate="false">
       </telerik:RadAjaxManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" >
            <Windows>
                <telerik:RadWindow ID="RadWindow_workItemImport"  runat="server" Modal="true" Title="Freeze Confirmation" Height="210" Width="400" VisibleStatusbar="false" iconUrl="false">
                    <ContentTemplate>
                        <div class="contentModal" id="import" style="padding:5px;">
                            <div class="header">
                                You are about to freeze the Release.
                            </div>
                            <br />
                            <div class="center">
                                <b>WARNING</b>
                                <br />
                                # Versions are pending upload.<br />
                                # CRs are not in final status.<br /><br />
                                Freeze stage 3 : <asp:DropDownList  runat="server"><asp:ListItem>Meeting 4</asp:ListItem></asp:DropDownList> 
                                <telerik:RadDatePicker ID="RadDatePicker1"  Width="100" runat="server" MinDate="1900-01-01" AutoPostBack="false">
                                    <Calendar ID="Calendar1" RangeMinDate="1900-01-01" runat="server">
                                    </Calendar>
                                </telerik:RadDatePicker>
                            </div>
                            <br />
                            <div class="footer" style="text-align: right">
                                <asp:Button ID="btnConfirmFreeze" Text ="Confirm" OnClick="btnConfirmFreeze_Click" runat="server"/><asp:Button id="btnCancelFreeze" runat="server" Text ="Cancel" />
                            </div>
                        </div>
                    </ContentTemplate>
                </telerik:RadWindow>
                <telerik:RadWindow ID="RadWindow_ClosureConfirmation"  runat="server" Modal="true" Title="Closure Confirmation" Height="260" Width="400" VisibleStatusbar="false" iconUrl="false" Behaviors="Close">
                    <ContentTemplate>
                        <div class="contentModal" id="divClosureConfirmation" style="padding:5px;">
                            <div class="header">
                                You are about to close the release. It will no longer be possible to create change requests on any specification for this release.
                            </div>
                            <br />
                            <div class="center">
                                <div id="divWarnings">
                                <img src="images/warning.png" style="vertical-align:middle"/> <b>WARNING</b>
                                <br />
                                # Versions are pending upload.<br />
                                # CRs are not in final status.<br />
                                # TDocs are not in final status.<br /><br />
                                </div>
                                Closure <asp:DropDownList ID="ddlClosureMeeting"  runat="server">
                                              <asp:ListItem>Meeting 4</asp:ListItem>
                                         </asp:DropDownList> 
                                <telerik:RadDatePicker ID="RadDatePickerClosureDate"  Width="100" runat="server" MinDate="1900-01-01" SelectedDate = "2014-03-10" AutoPostBack="false">
                                    <Calendar ID="radCalendar" RangeMinDate="1900-01-01" runat="server" />
                                    <DateInput DateFormat="yyyy-MM-dd" />
                                </telerik:RadDatePicker>
                            </div>
                            <br />
                            <div class="footer" style="text-align: right">
                                <asp:Button ID="btnConfirmClosure" runat="server" Text ="Confirm" OnClick="btnConfirmClosure_Click" />
                                <asp:Button id="btnCancelClosure" runat="server" Text ="Cancel" OnClientClick="return close();" />
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
