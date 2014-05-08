<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationsSettings.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.Settings" %>


<!-- uncomment the code below to start using the DNN Form pattern to create and update settings -->
  

<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
    <fieldset>
            <div class="dnnFormItem">
            <dnn:label ID="lblExportPath" runat="server" Text="Export path :" />
            <asp:TextBox ID="txtExportPath" runat="server" />
        </div>
    </fieldset>


