<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RapporteurControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RapporteurControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!--Import module.css-->
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/controls/Ultimate/module.css" />
<!--Import module.css-->

<div id="divRapporteurControl">
    <asp:Panel runat="server" ID="panelRapporteurControl" CssClass="">
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
                                                                                AllowMultiRowSelection="False"
                                                                                OnSelectedIndexChanged="RdGridRapporteurs_SelectedIndexChanged"  
                                                                                Height="140px">
                            <ClientSettings EnableRowHoverStyle="False" EnablePostBackOnRowClick="False">
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                                <Selecting AllowRowSelect="True"></Selecting>
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="PERSON_ID"> 
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="selectable" Visible="false" HeaderStyle-CssClass="selectStyleColumn" ItemStyle-CssClass="selectStyleColumn">
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn DataField="nameHyperLink" HeaderText="Name" UniqueName="nameHyperLink" Visible="false">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" CssClass="hyperlinkStyleColumn"/> 
                                        <ItemStyle CssClass="hyperlinkStyleColumn" />
                                        <ItemTemplate>
                                            <div><%# DataBinder.Eval(Container.DataItem,"RapporteurDetailsAddress") %></div>  
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="name" HeaderText="Name" UniqueName="name">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" CssClass="nameStyleColumn"/> 
                                        <ItemStyle CssClass="nameStyleColumn" />
                                        <ItemTemplate>
                                            <div><%# DataBinder.Eval(Container.DataItem,"FIRSTNAME") %> <%# DataBinder.Eval(Container.DataItem,"LASTNAME") %></div>  
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="company" HeaderText="Company" UniqueName="company">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" CssClass="companyStyleColumn"/>
                                        <ItemStyle CssClass="companyStyleColumn" />
                                        <ItemTemplate>
                                            <div><%# DataBinder.Eval(Container.DataItem,"ORGA_SHORT") %></div>  
                                        </ItemTemplate>                
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="email" HeaderText="Email" UniqueName="email">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" CssClass="emailStyleColumn"/>
                                        <ItemStyle CssClass="emailStyleColumn" />
                                        <ItemTemplate>
                                            <div><%# DataBinder.Eval(Container.DataItem,"Email") %></div>  
                                        </ItemTemplate>                
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridButtonColumn CommandName="Delete" HeaderText="Delete" UniqueName="delete" ImageUrl="~/images/delete.gif" ButtonType="ImageButton" HeaderStyle-CssClass="deleteStyleColumn" ItemStyle-CssClass="deleteStyleColumn" />
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
                            Width="98%"
                            OnItemsRequested="RdcbRapporteurs_ItemsRequested"
                            CssClass="rdcbRapporteurStyle"
                            OnSelectedIndexChanged="RdcbRapporteurs_SelectedIndexChanged"
                            AutoPostBack="true">  
                        </telerik:RadComboBox>
                    </td>
                    <td>
                        <asp:Button ID="btnAddRapporteur" OnClick="BtnAddRapporteur_onClick" runat="server" Text="Add" Width="95%" CssClass="center" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:Panel>
    <telerik:RadWindowManager ID="RadWindowAlert" runat="server" />
    
</div>