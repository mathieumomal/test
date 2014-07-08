<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RapporteurControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RapporteurControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!--Import module.css-->
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/controls/Ultimate/module.css" />
<!--Import module.css-->


<script type="text/javascript">
    function setAddingRapporteurProgress(flag) {
        if (flag)
            $("#<%=btnAddRapporteur.ClientID %>").val('Adding...');
        else
            $("#<%=btnAddRapporteur.ClientID %>").val('Add');
    }

    function setAddingChairmanProgress(flag) {
        if (flag)
            $("#<%=btnAddChairman.ClientID %>").val('Adding...');
        else
            $("#<%=btnAddChairman.ClientID %>").val('Add');
    }
</script>
<div id="divRapporteurControl">
    <asp:Panel runat="server" ID="panelRapporteurControl" CssClass="">
        <fieldset style="padding: 5px;">
            <legend>
                <asp:Label runat="server" ID="lblLegendRapporteur">Rapporteurs</asp:Label>
            </legend>
            <table style="width: 100%">
                <tr>
                    <td colspan="4">
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
                                <ClientEvents OnDataBound="setAddingRapporteurProgress(false)" />
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                                <Selecting AllowRowSelect="True"></Selecting>
                            </ClientSettings>
                            <MasterTableView AutoGenerateColumns="False" DataKeyNames="PERSON_ID"> 
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="selectable" Visible="false" HeaderStyle-CssClass="selectStyleColumn" ItemStyle-CssClass="selectStyleColumn">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" CssClass="nameStyleColumn" Width="50px"/> 
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn DataField="nameHyperLink" HeaderText="Name" UniqueName="nameHyperLink" Visible="false">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" CssClass="hyperlinkStyleColumn"/> 
                                        <ItemStyle CssClass="hyperlinkStyleColumn" />
                                        <ItemTemplate>
                                            <a href="<%# PersonLinkBaseAddress %><%# DataBinder.Eval(Container.DataItem,"PERSON_ID")  %>" target="_blank">
                                            <%# DataBinder.Eval(Container.DataItem,"FIRSTNAME") %> <%# DataBinder.Eval(Container.DataItem,"LASTNAME") %> </a>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="name" HeaderText="Name" UniqueName="name">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" Width="30%" CssClass="nameStyleColumn"/> 
                                        <ItemStyle CssClass="nameStyleColumn" />
                                        <ItemTemplate>
                                            <div><%# DataBinder.Eval(Container.DataItem,"FIRSTNAME") %> <%# DataBinder.Eval(Container.DataItem,"LASTNAME") %></div>  
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn DataField="company" HeaderText="Company" UniqueName="company">
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" Width="25%" CssClass="companyStyleColumn"/>
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
                                    <telerik:GridButtonColumn CommandName="Delete" HeaderText="Delete" UniqueName="delete" ImageUrl="/controls/Ultimate/images/delete.png" ButtonType="ImageButton" HeaderStyle-CssClass="deleteStyleColumn" ItemStyle-CssClass="deleteStyleColumn" >
                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="True" Width="50px"/>
                                    </telerik:GridButtonColumn>
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
                    <td class="center" style="width:15%">
                        <asp:Label ID="lblAddRapporteur" runat="server" Text="Add rapporteur"/>
                    </td>
                    <td style="width:70%">
                        <telerik:RadComboBox
                            id="rdcbRapporteurs"
                            runat="server"
                            AllowCustomText="true"
                            EnableLoadOnDemand="True"
                            Width="95%"
                            OnItemsRequested="RdcbRapporteurs_ItemsRequested"
                            CssClass="rdcbRapporteurStyle"
                            OnSelectedIndexChanged="RdcbRapporteurs_SelectedIndexChanged"
                            AutoPostBack="true"
                            EmptyMessage="Search Rapporteurs...">  
                        </telerik:RadComboBox>
                    </td>
                    <td style="width:10%">
                        <asp:Button ID="btnAddRapporteur" OnClientClick="setAddingRapporteurProgress(true)" OnClick="BtnAddRapporteur_onClick" runat="server" Text="Add" Width="95%" CssClass="center btn3GPP-success" />
                    </td>
                    <td style="width:5%">
                        <asp:ImageButton ID="btnAddChairman" OnClientClick="setAddingChairmanProgress(true)" OnClick="BtnAddChairman_onClick" runat="server" ToolTip="Add Chairman" ImageUrl="/controls/Ultimate/images/addChairman.png" Width="20px" CssClass="verticalAlignMiddle center" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:Panel>
    <telerik:RadWindowManager ID="RadWindowAlert" runat="server" />
    
</div>