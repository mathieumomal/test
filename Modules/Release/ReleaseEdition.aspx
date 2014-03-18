<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseEdition.aspx.cs" Inherits="Etsi.Ultimate.Module.Release.ReleaseEdition" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="MeetingControl" Src="../../controls/Ultimate/MeetingControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
            <telerik:RadTabStrip ID="ReleaseDetailRadTabStrip" runat="server" MultiPageID="ReleaseEditRadMultiPage" 
            AutoPostBack="false">    
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="ReleaseEditRadMultiPage" runat="server" Width="100%" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
                <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true">        
                   <table style="width: 100%">
                        <tr>
                            <td class="TabLineLeft" ><asp:Label ID="releaseCodeLbl" runat="server"  Text="Release code<span class='requiredField'>(*)</span>:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight" id="SecondColreleaseCode"><asp:TextBox ID="releaseCodeVal" runat="server"  ></asp:TextBox></td>
                        </tr>                              
                        <tr>
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStatusLbl" runat="server"  Text="Status:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight">
                                <asp:Label ID="ReleaseStatusVal" runat="server"  Text="Open"></asp:Label>
                            </td>
                        </tr>            
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseNameLbl" runat="server"  Text="Release name<span class='requiredField'>(*)</span>:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:TextBox ID="ReleaseNameVal" runat="server"  CssClass="releaseName"></asp:TextBox></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseDescLbl" runat="server"  Text="Release description:"></asp:Label>  </td>          
                            <td colspan="2" class="TabLineRight">
                                <asp:TextBox runat="server" id="ReleaseDescVal" />
                            </td>
            
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseShortNameLbl" runat="server"  Text="Release short name<span class='requiredField'>(*)</span>:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight"><asp:TextBox ID="ReleaseShortNameVal" runat="server" ></asp:TextBox></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseStartDateLbl" runat="server"  Text="Start date:"></asp:Label></td>
                            <td colspan="2" class="TabLineRight">
                                <telerik:RadDatePicker ID="ReleaseStartDateVal" runat="server" >
                                    <DateInput runat="server" ID="ReleaseStartDateValInput" DateFormat="yyyy-MM-dd"></DateInput>
                                </telerik:RadDatePicker>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" class="TabLineRightFreeze">
                                <asp:Panel ID="FreezeStagesPanel" runat="server">
                                    <fieldset id="FreezeFieldset">
                                        <legend><asp:Label ID="FreezeMeetingLbl" runat="server"  Text="Freeze meetings and dates"></asp:Label></legend>
                                        <table style="width: 100%; vertical-align:middle" id="FrezeStagesTable">
                                            <tr>
                                                <td class="FirstColFreezeStageEdit" id="FirstColFreezeStage1"><asp:Label ID="ReleaseFreezeStage1Lbl" runat="server"  Text="Stage1:"></asp:Label></td>
                                                
                                                <td class="SecndColFreezeStage"><ult:MeetingControl runat="server" ID="FreezeStage1Meeting"/></td>
                                            </tr>                                        
                                            <tr>
                                                <td class="FirstColFreezeStageEdit" id="FirstColFreezeStage2"><asp:Label ID="ReleaseFreezeStage2Lbl" runat="server"  Text="Stage2:"></asp:Label></td>
                                                
                                                <td class="SecndColFreezeStage"><ult:MeetingControl runat="server" ID="FreezeStage2Meeting"/></td>
                                            </tr>                                        
                                            <tr>
                                                <td class="FirstColFreezeStageEdit" id="FirstColFreezeStage3"><asp:Label ID="ReleaseFreezeStage3Lbl" runat="server"  Text="Stage3:"></asp:Label></td>
                                                
                                                <td class="SecndColFreezeStage"><ult:MeetingControl runat="server" ID="FreezeStage3Meeting"/></td>
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
                            
                            <td class="TabLineRight"><ult:MeetingControl runat="server" ID="ReleaseEndMeeting" /></td>
                        </tr>                       
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="ReleaseClosureDateLbl" runat="server"  Text="Closure date:"></asp:Label></td>
                            
                            <td class="TabLineRight"><ult:MeetingControl runat="server" ID="ReleaseClosureMeeting" /></td>
                        </tr>
                        <tr style="max-height: 120px; overflow-y: scroll; margin-top:5px"> 
                            <td colspan="3">
                                <ult:RemarksControl runat="server" ID="releaseRemarks" IsEditMode="true"/>   
                            </td>                                                    
                        </tr>
                   </table>
                 </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageAdministration" runat="server" selected="false">
                    <table class="TabContent" style="width:95%">
                        <tr class="TabLine">
                            <td class="TabLineLeft">                                
                                <asp:Label ID="previousReleaseLbl" runat="server"  Text="Follows Release<span class='requiredField'>(*)</span>:" CssClass="TabLabel"></asp:Label>                                
                            </td>
                            <td class="TabLineRight">   
                                <asp:DropDownList ID="previousReleaseVal" runat="server"  CssClass="TabValue"></asp:DropDownList>                                                                                                 
                            </td>
                        </tr>
                        <tr class="TabLine">
                            <td class="TabLineLeft">                                
                                <asp:Label ID="ITURCodeLbl" runat="server"  Text="ITUR code:"></asp:Label> 
                            </td>
                            <td class="TabLineRight">   
                                <asp:TextBox ID="ITURCodeVal" runat="server" ></asp:TextBox>
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
                                            <td style="width: 70%; padding-left: 25%">
                                                <asp:Label ID="Release2GDecimalLbl" runat="server"  Text="2G Decimal:"></asp:Label>
                                                <asp:TextBox ID="Release2GDecimalVal" runat="server"  ></asp:TextBox>
                                            </td>                                           
                                        </tr>                                                                                                              
                                        <tr>                                            
                                            <td style="width: 30%; padding-left: 10%">
                                                <asp:Label ID="Release3GLbl" runat="server"  Text="3G:"></asp:Label>
                                                <asp:Label ID="Release3GVal" runat="server" ></asp:Label>
                                            </td>  
                                            <td style="width: 70%; padding-left: 25%">
                                                <asp:Label ID="Release3GDecimalLbl" runat="server"  Text="3G Decimal:"></asp:Label>
                                                <asp:TextBox ID="Release3GDecimalVal" runat="server" ></asp:TextBox>
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
                                            <td style="width: 60%; padding-left: 10%">                              
                                                <asp:Label ID="WPMCodes2GLbl" runat="server"  Text="2G:"></asp:Label>
                                                <asp:TextBox ID="WPMCodes2GVal" runat="server"  MaxLength="30"></asp:TextBox>                                                                
                                            </td>
                                            <td style="width: 40%; padding-left: 30%"></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 60%; padding-left: 10%">                                
                                                <asp:Label ID="WPMCodes3GLbl" runat="server"  Text="3G:"></asp:Label>
                                                <asp:TextBox ID="WPMCodes3GVal" runat="server" ></asp:TextBox>                                                                
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
                <asp:LinkButton ID="SaveBtn" runat="server" Text="Save" CssClass="LinkButton" Visible="true" OnClick="SaveEditedRelease_Click"/>
                <asp:LinkButton ID="ExitBtn" runat="server" Text="Cancel" CssClass="LinkButton" OnClick="CloseReleaseDetails_Click"/>
           </div> 
           <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="Release2GDecimalVal">
                        <UpdatedControls>                  
                            <telerik:AjaxUpdatedControl ControlID="Release2GDecimalVal" /> 
                            <telerik:AjaxUpdatedControl ControlID="Release2GVal" />   
                            <telerik:AjaxUpdatedControl ControlID="Release2GDecimalLbl" /> 
                            <telerik:AjaxUpdatedControl ControlID="Release2GLbl" />                                                               
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="Release3GDecimalVal">
                        <UpdatedControls>                  
                            <telerik:AjaxUpdatedControl ControlID="Release3GDecimalVal" /> 
                            <telerik:AjaxUpdatedControl ControlID="Release3GVal" />  
                            <telerik:AjaxUpdatedControl ControlID="Release3GDecimalLbl" /> 
                            <telerik:AjaxUpdatedControl ControlID="Release3GLbl" />                                                         
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
           <script type="text/javascript">
              

               function realPostBack(eventTarget, eventArgument) {
                  $find("<%= RadAjaxManager1.ClientID %>").__doPostBack(eventTarget, eventArgument);
               }

               

               $(document).ready(function () {
                   setTimeout(function () {

                       var releaseName = "Create new Release";
                       if ($("#releaseCodeVal").val() != '')
                           releaseName = " Edit Release " + $("#releaseCodeVal").val();
                       
                       document.title = releaseName;
                   }, 200);

                   $('#FreezeReleaseBtn').click(function (event) {
                       event.preventDefault();
                       closeAllModals();
                       window.radopen(null, "RadWindow_workItemImport");
                   });

                   
                   var errorClassName = 'error';
                   var validator = $("#ReleaseEditionForm").validate({
                       onKeyup: true,                       
                       eachValidField: function () {                           
                           $(this).removeClass('error');
                           
                           if (!$("releaseCodeVal").hasClass(errorClassName) && !$("ReleaseNameVal").hasClass(errorClassName)
                               && !$("ReleaseShortNameVal").hasClass(errorClassName) && !$("previousReleaseVal").hasClass(errorClassName)
                               && !$("Release2GDecimalVal").hasClass(errorClassName) && !$("Release3GDecimalVal").hasClass(errorClassName))
                           {
                               $('#SaveBtn').removeAttr('disabled');
                               $('#SaveBtn').removeClass('disabledLink');
                               $('#SaveBtn').addClass('LinkButton');
                           }
                           
                       },
                       eachInvalidField: function () {
                           $(this).addClass('error');
                           $('#SaveBtn').attr('disabled', 'disabled');
                           $('#SaveBtn').removeClass('LinkButton');
                           $('#SaveBtn').addClass('disabledLink');
                       }                       
	   
                   });
                   
               });

               
               
        </script>  
       </asp:Panel>       
       </div>        
    </form>    
</body>
</html>
