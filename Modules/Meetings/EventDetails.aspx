<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EventDetails.aspx.cs" Inherits="Etsi.Ultimate.Module.Meetings.EventDetails" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>View event</title>
        <link rel="stylesheet" type="text/css" href="module.css"/>
        <link rel="stylesheet" type="text/css" href="/Portals/_default/Skins/3GPP/mainpage.css"/>
        <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon"/>
    </head>
    <body id="eventDetailsContener" class="eventWebFormStyle">
        <h1>View event</h1>

        <form id="detailsEventForm" runat="server">
            <telerik:RadScriptManager runat="server" ID="rsmEventDetails" />
            <table>
                <tr>
                    <td class="LeftColumn">Reference:</td>
                    <td class="RightColumn"><asp:Label ID="lblReference" runat="server" Text="-" /></td>
                </tr>
                <tr class="">
                    <td class="LeftColumn">IT Support:</td>
                    <td class="RightColumn">
                        <asp:LinkButton ID="lblITSupportPerson" runat="server" Text="-" />
                    </td>
                </tr>
                <tr>
                    <td class="LeftColumn">
                        Satisfaction survey URL:
                    </td>
                    <td class="RightColumn">
                        <asp:LinkButton ID="lblSatisfactionSurveyUrl" runat="server" Text="-" />
                    </td>
                </tr>
                <tr>
                    <td class="LeftColumn"></td>
                    <td class="RightColumn">
                        <asp:LinkButton ID="linkDownloadListParticipants" runat="server" Text="-" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />
                        <br />
                        <div>Additional event informations:</div>
                        <div><asp:TextBox id="txtBoxInformations" TextMode="multiline" Columns="50" Rows="5" runat="server" Enabled="false" /></div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />
                        <br />
                        <telerik:RadGrid 
                            runat="server" 
                            EnableEmbeddedSkins="false" 
                            EnableEmbeddedBaseStylesheet="false" 
                            ID="rdgMeetings" 
                            AllowPaging="false" 
                            AllowSorting="false" 
                            AllowFilteringByColumn="false" 
                            AutoGenerateColumns="false"
                            EnableViewState="false"
                            Height="180px">
                            <MasterTableView ClientDataKeyNames="Pk_MeetingId">
                                <Columns>
                                    <telerik:GridTemplateColumn DataField="meeting" HeaderText="Meeting" UniqueName="meeting" >
                                        <ItemTemplate>
                                            <span>a</span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="startDate" HeaderText="Start date" UniqueName="startDate" >
                                        <ItemTemplate>
                                            <span>a</span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="endDate" HeaderText="End date" UniqueName="endDate" >
                                        <ItemTemplate>
                                            <span>a</span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="location" HeaderText="Location" UniqueName="location" >
                                        <ItemTemplate>
                                            <span>a</span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="registeredParticipants" HeaderText="# registered participants" UniqueName="registeredParticipants" >
                                        <ItemTemplate>
                                            <span>a</span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                                <NoRecordsTemplate>
                                    <div style="text-align:center">
                                        No meetings founded
                                    </div>
                                </NoRecordsTemplate>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />
                        <br />
                        <asp:Button ID="btnCancel" runat="server" Text="Exit" CssClass="btn3GPP-success" />
                    </td>
                </tr>
            </table>
        </form>
    </body>
</html>
