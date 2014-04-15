<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RapporteurControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RapporteurControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!--Import module.css-->
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/controls/Ultimate/module.css" />
<!--Import module.css-->

<div id="divRapporteurControl">
    <fieldset style="padding: 5px;">
        <legend>
            <asp:Label runat="server" ID="lblLegendRapporteur">Rapporteurs</asp:Label>
        </legend>
        <table style="width: 100%">
            <tr>
                <td colspan="3">
                    <telerik:RadGrid runat="server" ID="rdGridRapporteurs"  AllowPaging="false" 
                                                                            AllowSorting="false" 
                                                                            AllowFilteringByColumn="false" 
                                                                            AutoGenerateColumns="false"
                                                                            OnDeleteCommand="RdGridRapporteurs_DeleteCommand"
                                                                            AllowAutomaticDeletes="true"
                                                                            Height="140px">
                        <ClientSettings>
                            <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                        </ClientSettings>
                        <MasterTableView AutoGenerateColumns="False" DataKeyNames="PERSON_ID"> 
                            <Columns>
                                <telerik:GridTemplateColumn DataField="name" HeaderText="Name" UniqueName="name">
                                    <HeaderStyle HorizontalAlign="Left" Font-Bold="True" Width="30%"/> 
                                    <ItemTemplate>
                                        <div><%# DataBinder.Eval(Container.DataItem,"FIRSTNAME") %></div>  
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="company" HeaderText="Company" UniqueName="company">
                                    <HeaderStyle HorizontalAlign="Left" Font-Bold="True" Width="20%"/>
                                    <ItemTemplate>
                                        <div><%# DataBinder.Eval(Container.DataItem,"ORGA_NAME") %></div>  
                                    </ItemTemplate>                
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="email" HeaderText="Email" UniqueName="email">
                                    <HeaderStyle HorizontalAlign="Left" Font-Bold="True" Width="40%"/>
                                    <ItemTemplate>
                                        <div><%# DataBinder.Eval(Container.DataItem,"Email") %></div>  
                                    </ItemTemplate>                
                                </telerik:GridTemplateColumn>
                                <telerik:GridButtonColumn CommandName="Delete" Text="Delete" UniqueName="DeleteColumn" />
                            </Columns>
                            
                            <NoRecordsTemplate>
                                <div style="text-align:center">
                                    No Rapporteur Added
                                </div>
                            </NoRecordsTemplate>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
            <tr class="searchBar">
                <td class="center">
                    <asp:Label ID="lblAddRapporteur" runat="server" Text="Add rapporteur"/>
                </td>
                <td>
                    <telerik:RadComboBox
                        id="rdcbRapporteurs"
                        runat="server"
                        AllowCustomText="true"
                        EnableLoadOnDemand="True"
                        Width="95%"
                        OnItemsRequested="RdcbRapporteurs_ItemsRequested"
                        CssClass="center">  
                    </telerik:RadComboBox>
                </td>
                <td>
                    <asp:Button ID="btnAddRapporteur" OnClick="BtnAddRapporteur_onClick" runat="server" Text="Add" Width="95%" CssClass="center" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>
<telerik:RadWindowManager ID="RadWindowAlert" runat="server" />