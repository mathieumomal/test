<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpecificationDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationDetails" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Etsi.Ultimate.Module.Specifications" Namespace="Etsi.Ultimate.Module.Specifications" TagPrefix="specification" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="specificationDetailsForm" runat="server">
    <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="650px">
        <asp:Panel ID="specificationDetailsBody" runat="server" CssClass="specificationDetailsBody">
            <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
            <telerik:RadTabStrip ID="SpecificationDetailsRadTabStrip" runat="server" MultiPageID="SpecificationDetailsRadMultiPage" 
            AutoPostBack="false">    
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="SpecificationDetailsRadMultiPage" runat="server" Width="100%" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
                <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true"> 
                    <table style="width: 100%">
                        <tr>
                            <td class="TabLineLeft" ><asp:Label ID="referenceLbl" runat="server" Text="Reference:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="referenceVal" runat="server" ></asp:Label></td>
                        </tr>                              
                        <tr>
                            <td class="TabLineLeft"><asp:Label ID="titleLbl" runat="server"  Text="Title:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="titleVal" runat="server" ></asp:Label></td>
                        </tr>            
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="statusLbl" runat="server"  Text="Status:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="statusVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="typeLbl" runat="server"  Text="Type:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="typeVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="stageLbl" runat="server"  Text="Stage:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="stageVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="initialPlannedReleaseLbl" runat="server"  Text="Initial planned Release:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="initialPlannedReleaseVal" runat="server" ></asp:Label></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="internalLbl" runat="server"  Text="Internal:"></asp:Label></td>
                            <td class="TabLineRight"><asp:CheckBox ID="internalVal" runat="server" Enabled="false"></asp:CheckBox></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="commonIMSLbl" runat="server"  Text="Common IMS Specification:"></asp:Label></td>
                            <td class="TabLineRight"><asp:CheckBox ID="commonIMSVal" runat="server" Enabled="false"></asp:CheckBox></td>
                        </tr>
                        <tr>            
                            <td class="TabLineLeft"><asp:Label ID="radioTechnologyLbl" runat="server"  Text="Common IMS Specification:"></asp:Label></td>
                            <td class="TabLineRight"><asp:CheckBoxList ID="radioTechnologyVals" runat="server" Enabled="false"></asp:CheckBoxList></td>
                        </tr>
                        <tr style="max-height: 150px; overflow-y: scroll; margin-top:10px"> 
                            <td colspan="2" class="specificationRemark">
                                <ult:RemarksControl runat="server" ID="specificationRemarks" />   
                            </td>                                                    
                        </tr>
                    </table>  
                </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageResponsibility" runat="server" Selected="true">   
                    <table style="width: 100%">
                        <tr>
                            <td class="TabLineLeft" ><asp:Label ID="PrimaryResponsibleGroupLbl" runat="server" Text="Primary responsible group:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="PrimaryResponsibleGroupVal" runat="server" ></asp:Label></td>
                        </tr>                              
                        <tr>
                            <td class="TabLineLeft"><asp:Label ID="SecondaryResponsibleGroupsLbl" runat="server"  Text="Secondary responsible groups:"></asp:Label></td>
                            <td class="TabLineRight"><asp:Label ID="SecondaryResponsibleGroupsVal" runat="server" ></asp:Label></td>
                        </tr> 
                        <tr style="max-height: 150px; overflow-y: scroll; margin-top:10px"> 
                            <td colspan="2" class="specificationRapporteurs">
                                <!-- ID="specificationRapporteurs"   -->
                            </td>                                                    
                        </tr>
                    </table>                    
                </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageRelated" runat="server" Selected="true">  
                    <asp:Panel ID="RelatedSpecificationsPanel" runat="server">
                        <div style="max-height: 200px; overflow-y: scroll">
                            <fieldset id="ParentFieldset">
                                <legend><asp:Label ID="ParentSpecLbl" runat="server"  Text="Parent Specifications"></asp:Label></legend>
                                <specification:SpecificationsList ID="parentSpecifications" />
                            </fieldset>
                        </div>
                        <div style="max-height: 200px; overflow-y: scroll">
                            <fieldset id="ChildFieldset">
                                <legend><asp:Label ID="ChildSpecLbl" runat="server"  Text="Child Specifications"></asp:Label></legend>
                                <specification:SpecificationsList ID="childSpecifications" />
                            </fieldset>
                        </div>
                        <div style="max-height: 300px; overflow-y: scroll">
                            <fieldset id="WorkItemsFieldSet">
                                <legend><asp:Label ID="RelatedWorkItemsLbl" runat="server"  Text="Related Work Items"></asp:Label></legend>
                                 <!-- work item componenet -->
                            </fieldset>
                        </div>
                    </asp:Panel>                   
                </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageReleases" runat="server" Selected="true">   
                </telerik:RadPageView>
                <telerik:RadPageView ID="RadPageHistory" runat="server" Selected="true">   
                    <div class="TabContent" style="overflow-y: auto; overflow-x: auto">
                        <ult:HistoryControl runat="server" ID="specificationHistory" />
                    </div>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
            <div class="releaseDetailsAction">
                    <asp:LinkButton ID="EditBtn" runat="server" Text="Edit" CssClass="btn3GPP-success" Visible="false" OnClick="EditSpecificationDetails_Click" />
                    <asp:LinkButton ID="WithdrawBtn" runat="server" Text="Definitively withdraw" CssClass="btn3GPP-success" OnClick="WithdrawSpecificatione_Click"/>
                    <asp:LinkButton ID="ExitBtn" runat="server" Text="Exit" CssClass="btn3GPP-success" OnClick="ExitSpecificationDetails_Click"/>
            </div> 
        </asp:Panel>
    </asp:Panel>
    </form>
</body>
</html>
