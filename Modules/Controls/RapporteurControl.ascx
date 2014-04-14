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
                                                                            OnPreRender="rdGridRapporteurs_PreRender"
                                                                            OnNeedDataSource="rdGridRapporteurs_NeedDataSource">
                        <ClientSettings>
                            <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                        </ClientSettings>
                        <MasterTableView>
                            <Columns>
                                <telerik:GridTemplateColumn DataField="name" HeaderText="Name" UniqueName="name">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px"/> 
                                    <ItemTemplate>
                                        <div><%# DataBinder.Eval(Container.DataItem,"LASTNAME") %></div>  
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="company" HeaderText="Company" UniqueName="company">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True"/>
                                    <ItemTemplate>
                                        <div><%# DataBinder.Eval(Container.DataItem,"ORGA_NAME") %></div>  
                                    </ItemTemplate>                
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="email" HeaderText="Email" UniqueName="email">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True"/>
                                    <ItemTemplate>
                                        <div><%# DataBinder.Eval(Container.DataItem,"Email") %></div>  
                                    </ItemTemplate>                
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="delete" HeaderText="Delete" UniqueName="delete">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True"/>
                                    <ItemTemplate>
                                        <div>
                                            <asp:Button runat="server" Text="Delete"/>
                                        </div>  
                                    </ItemTemplate>                
                                </telerik:GridTemplateColumn>
                            </Columns>
                            <NoRecordsTemplate>
                                <div style="text-align:center">
                                    No Remarks Added
                                </div>
                            </NoRecordsTemplate>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblAddRapporteur" runat="server" Text="Add rapporteur"/>
                </td>
                <td>
                    <asp:TextBox ID="txtAddRemark" runat="server" Width="100%"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnAddRapporteur" runat="server" Text="Add" Width="100%" />
                </td>
            </tr>
        </table>
    </fieldset>
</div>