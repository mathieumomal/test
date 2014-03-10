<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseEdition.aspx.cs" Inherits="Etsi.Ultimate.Module.Release.ReleaseEdition" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>  
    <script src="JS/jquery.validate.js"></script>  
    <script type="text/javascript">

        function closeAllModals() {
            var manager = GetRadWindowManager();
            manager.closeAll();
        }

        $().ready(function () {
            

            // validate signup form on keyup and submit
            $("#ReleaseEditionForm").validate({
                rules: {
                    releaseCodeVal: {
                        required: true,
                        maxlength: 10
                    },
                    ReleaseNameVal: {
                        required: true,
                        maxlength: 50
                    },
                    ReleaseDescVal: {
                        maxlength: 200
                    },
                    ReleaseShortNameVal: {
                        required: true,
                        maxlength: 20
                    }
                    /*previousReleaseVal: {
                        required: true,
                        minlength: 2
                    }*/
                },
                messages: {
                    releaseCodeVal: {
                        required: "Please enter the release code",
                        minlength: "10 characters max"
                    },
                    ReleaseNameVal: {
                        required: "Please enter the release name",
                        minlength: "50 characters max"
                    },                   
                    ReleaseShortNameVal: {
                        required: "Please enter the release short name",
                        minlength: "Your password must be at least 5 characters long"
                    },
                    
                    ReleaseDescVal: "200 characters max"
                }
            });            
        });
    </script>
</head>
<body class="releaseDetailBody">
    <form id="ReleaseEditionForm" runat="server">
       <div class="containerFix">
       <asp:Panel ID="releaseWarning" runat="server" CssClass="releaseDetailsWarning" Visible="false">
           <span class="releaseDetailsWarningTxt">No data available for the current query.</span>
       </asp:Panel>
       <asp:Panel ID="releaseError" runat="server" CssClass="releaseDetailsError" Visible="false">
           <asp:Label Id="ErrorMsg" runat="server" CssClass="releaseDetailsErrorTxt" Text="Bad request."></asp:Label>
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
                            <td class="TabLineLeft" ><asp:Label ID="releaseCodeLbl" runat="server" ControlName="releaseCodeLbl" Text="Release code<span class='requiredField'>(*)</span>:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight" id="SecondColreleaseCode"><asp:TextBox ID="releaseCodeVal" runat="server" ControlName="releaseCodeVal" ></asp:TextBox></td>
                        </tr>                              
                        <tr>
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStatusLbl" runat="server" ControlName="ReleaseStatusLbl" Text="Status:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight">
                                <asp:Label ID="ReleaseStatusVal" runat="server" ControlName="ReleaseStatusVal" Text="Open"></asp:Label>
                            </td>
                        </tr>            
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseNameLbl" runat="server" ControlName="ReleaseNameLbl" Text="Release name<span class='requiredField'>(*)</span>:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:TextBox ID="ReleaseNameVal" runat="server" ControlName="ReleaseNameVal" CssClass="releaseName"></asp:TextBox></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseDescLbl" runat="server" ControlName="ReleaseDescLbl" Text="Release description:"></asp:Label>  </td>          
                            <td colspan="2" class="TabLineRight">
                                <asp:TextBox runat="server" id="ReleaseDescVal" />
                                <asp:Label ID="MissigDesc" runat="server" Visible="false"/>
                            </td>
            
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseShortNameLbl" runat="server" ControlName="ReleaseShortNameLbl" Text="Release short name:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:TextBox ID="ReleaseShortNameVal" runat="server" ControlName="ReleaseShortNameVal"></asp:TextBox></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStartDateLbl" runat="server" ControlName="ReleaseStartDateLbl" Text="Start date:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><telerik:RadDatePicker ID="ReleaseStartDateVal" runat="server" ControlName="ReleaseStartDateVal"></telerik:RadDatePicker></td>
                        </tr>
                        <tr>
                            <td colspan="3" class="TabLineRightFreeze">
                                <asp:Panel ID="FreezeStagesPanel" runat="server">
                                    <fieldset id="FreezeFieldset">
                                        <legend><asp:Label ID="FreezeMeetingLbl" runat="server" ControlName="FreezeMeetingLbl" Text="Freeze meetings and dates"></asp:Label></legend>
                                        <table style="width: 100%; vertical-align:middle" id="FrezeStagesTable">
                                            <tr>
                                                <td style="text-align: right" id="FirstColFreezeStage1"><asp:Label ID="ReleaseFreezeStage1Lbl" runat="server" ControlName="ReleaseFreezeStage1Lbl" Text="Stage1:"></asp:Label></td>
                                                <td style="padding-left: 10px; text-align: left" id="SecndColFreezeStage1">
                                                    <asp:Label ID="ReleaseFreezeStage1Meeting" runat="server" ControlName="ReleaseFreezeStage1Meeting" CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage1">
                                                    <telerik:RadDatePicker ID="ReleaseFreezeStage1Date" runat="server" ControlName="ReleaseFreezeStage1Date"></telerik:RadDatePicker>
                                                </td>
                                            </tr>                                        
                                            <tr>
                                                <td style="text-align: right" id="FirstColFreezeStage2"><asp:Label ID="ReleaseFreezeStage2Lbl" runat="server" ControlName="ReleaseFreezeStage2Lbl" Text="Stage2:"></asp:Label></td>
                                                <td style="padding-left: 10px; text-align: left" id="SecndColFreezeStage2">
                                                    <asp:Label ID="ReleaseFreezeStage2Meeting" runat="server" ControlName="ReleaseFreezeStage2Meeting" CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage2">
                                                    <telerik:RadDatePicker ID="ReleaseFreezeStage2Date" runat="server" ControlName="ReleaseFreezeStage2Date"></telerik:RadDatePicker>
                                                </td>
                                            </tr>                                        
                                            <tr>
                                                <td style="text-align: right" id="FirstColFreezeStage3"><asp:Label ID="ReleaseFreezeStage3Lbl" runat="server" ControlName="ReleaseFreezeStage3Lbl" Text="Stage3:"></asp:Label></td>
                                                <td style="padding-left: 10px; text-align: left" id="SecndColFreezeStage3">
                                                    <asp:Label ID="ReleaseFreezeStage3Meeting" runat="server" ControlName="ReleaseFreezeStage3Meeting" CssClass="SecndColFreezeStageMeeting"></asp:Label>                                                    
                                                </td>
                                                <td id="thirdColFreezeStage3">
                                                    <telerik:RadDatePicker ID="ReleaseFreezeStage3Date" runat="server" ControlName="ReleaseFreezeStage3Date"></telerik:RadDatePicker>
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
                            <td class="TabLine3colRight"><telerik:RadDatePicker ID="ReleaseEndDateVal" runat="server" ControlName="ReleaseEndDateVal"></telerik:RadDatePicker></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseClosureDateLbl" runat="server" ControlName="ReleaseClosureDateLbl" Text="Closure date:"></asp:Label></td>
                            <td><asp:Label ID="ReleaseClosureDateMeetingVal" runat="server" ControlName="ReleaseClosureDateMeetingVal"></asp:Label></td>
                            <td><telerik:RadDatePicker ID="ReleaseClosureDateVal" runat="server" ControlName="ReleaseClosureDateVal"></telerik:RadDatePicker></td>
                        </tr>
                        <tr style="max-height: 150px; overflow-y: scroll; margin-top:5px"> 
                            <td colspan="3">
                                <ult:RemarksControl runat="server" ID="releaseRemarks" />   
                            </td>                                                    
                        </tr>
                   </table>
                 </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageAdministration" runat="server" Height="90%">
                    <table class="TabContent" style="width:95%">
                        <tr class="TabLine">
                            <td class="TabLineLeft">                                
                                <asp:Label ID="previousReleaseLbl" runat="server" ControlName="previousReleaseLbl" Text="Follows Release<span class='requiredField'>(*)</span>:" CssClass="TabLabel"></asp:Label>                                
                            </td>
                            <td class="TabLineRight">   
                                <asp:DropDownList ID="previousReleaseVal" runat="server" ControlName="previousReleaseVal" CssClass="TabValue"></asp:DropDownList>                                                                                                 
                            </td>
                        </tr>
                        <tr class="TabLine">
                            <td class="TabLineLeft">                                
                                <asp:Label ID="ITURCodeLbl" runat="server" ControlName="ITURCodeLbl" Text="ITUR code:"></asp:Label> 
                            </td>
                            <td class="TabLineRight">   
                                <asp:TextBox ID="ITURCodeVal" runat="server" ControlName="ITURCodeVal"></asp:TextBox>
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
                                            <td style="width: 70%; padding-left: 25%">
                                                <asp:Label ID="Release2GDecimalLbl" runat="server" ControlName="Release2GDecimalLbl" Text="2G Decimal:"></asp:Label>
                                                <asp:TextBox ID="Release2GDecimalVal" runat="server" ControlName="Release2GDecimalVal"></asp:TextBox>
                                            </td>                                           
                                        </tr>                                                                                                              
                                        <tr>
                                            
                                            <td style="width: 30%; padding-left: 10%">
                                                <asp:Label ID="Release3GLbl" runat="server" ControlName="Release3GLbl" Text="3G:"></asp:Label>
                                                <asp:Label ID="Release3GVal" runat="server" ControlName="Release3GVal"></asp:Label>
                                            </td>  
                                            <td style="width: 70%; padding-left: 25%">
                                                <asp:Label ID="Release3GDecimalLbl" runat="server" ControlName="Release3GDecimalLbl" Text="3G Decimal:"></asp:Label>
                                                <asp:TextBox ID="Release3GDecimalVal" runat="server" ControlName="Release3GDecimalVal" Numeric="true"></asp:TextBox>
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
                                            <td style="width: 60%; padding-left: 10%">                              
                                                <asp:Label ID="WPMCodes2GLbl" runat="server" ControlName="WPMCodes2GLbl" Text="2G:"></asp:Label>
                                                <asp:TextBox ID="WPMCodes2GVal" runat="server" ControlName="WPMCodes2GVal" MaxLength="30"></asp:TextBox>                                                                
                                            </td>
                                            <td style="width: 40%; padding-left: 30%"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 60%; padding-left: 10%">                                
                                                <asp:Label ID="WPMCodes3GLbl" runat="server" ControlName="WPMCodes3GLbl" Text="3G:"></asp:Label>
                                                <asp:TextBox ID="WPMCodes3GVal" runat="server" ControlName="WPMCodes3GVal"></asp:TextBox>                                                                
                                            </td>
                                            <td style="width: 40%; padding-left: 30%"></td>
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
                <asp:LinkButton ID="SaveBtn" runat="server" Text="Save" CssClass="LinkButton" Visible="true"/>
                <asp:LinkButton ID="ExitBtn" runat="server" Text="Cancel" CssClass="LinkButton" OnClick="CloseReleaseDetails_Click"/>
           </div> 
           <script type="text/javascript">
               function resizeElements() {

                   var calWidth = $("#FreezeStagesPanel").width() - $("#SecondColreleaseCode").width() - ($("#FreezeStagesPanel").width() * 0.03) - 17;

                   $("#thirdColFreezeStage1").width(($("#FrezeStagesTable").width() - calWidth) * 0.5);
                   $("#SecndColFreezeStage1").width(($("#FrezeStagesTable").width() - calWidth) * 0.5 -20);
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
               });
        </script>  
       </asp:Panel>       
       </div>        
    </form>    
</body>
</html>
