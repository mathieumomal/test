<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VersionsSettings.ascx.cs" Inherits="Etsi.Ultimate.Module.Versions.Settings" %>


<!-- uncomment the code below to start using the DNN Form pattern to create and update settings -->


<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblUploadPath" runat="server" Text="Upload path :" /> 
 
             <asp:TextBox ID="txtUploadPath" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblFTPBase" runat="server" Text="FTP base path :" /> 
 
             <asp:TextBox ID="txtFtpBasePath" runat="server" />
        </div>        
    </fieldset>


