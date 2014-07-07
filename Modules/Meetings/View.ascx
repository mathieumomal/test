<!-- 
    Take care : this module must follow new design css rules :
    - All the css in the module.css file
    - Consistency naming
    - use global design components (buttons, messagebox, ...)
-->

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.Meetings.View" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!--Import module.css-->
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/Specifications/module.css" />
<!--Import module.css-->


<!-- This global contener should be stay here and its id should be use as prefix of all css style in the module.css file -->
<div id="meetingsView">
    <h1>Meetings module</h1>


</div>
<!-- no code anymore -->
