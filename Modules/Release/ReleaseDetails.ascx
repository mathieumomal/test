﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReleaseDetails.ascx.cs" Inherits="Etsi.Ultimate.Module.Release.ReleaseDetails" %>

<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>

<table style="width:100%">
    <tr>
        <td style="text-align:right;width:50%">Release Code:</td>
        <td style="text-align:left;width:50%"><asp:Label ID="lblReleaseCode" runat="server"></asp:Label></td>
        
    </tr>
    <tr>
        <td style="text-align:right">Status:</td>
        <td style="text-align:left"><asp:Label ID="lblReleaseStatus" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td style="text-align:right">Release name:</td>
        <td style="text-align:left"><asp:Label ID="lblReleaseName" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td style="text-align:right">Release description:</td>
        <td style="text-align:left"><asp:Label ID="lblReleaseDescription" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td style="text-align:right">Release short name:</td>
        <td style="text-align:left"><asp:Label ID="lblReleaseShortName" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td style="text-align:right">Start date:</td>
        <td style="text-align:left"><asp:Label ID="lblStartDate" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td style="text-align:right">End date:</td>
        <td style="text-align:left"><asp:Label ID="lblEndDate" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td style="text-align:right">Closure date:</td>
        <td style="text-align:left"><asp:Label ID="lblClosureDate" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td colspan="2">
            <ult:remarkscontrol id="RemarksControlComponent" runat="server" IsEditMode="false"/>
        </td>
    </tr>
     <tr>
        <td colspan="2" style="text-align:left">
            History
        </td>
    </tr>
   <tr>
        <td colspan="2">
            <ult:HistoryControl id="HistoryControlComponent" runat="server" />
        </td>
    </tr>
</table>
