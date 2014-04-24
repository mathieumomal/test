<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationListControl.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationListControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    .RadGrid_Default th.rgHeader
    {
        background-color: grey;
        border: none;
        border-bottom: 1px solid grey;
        min-height: 50px;
        background-color: #eaeaea !important;
    }

    .RadGrid_Default .rgEditRow td {
        border: none;
    }
</style>

<table style="width: 100%">
    <tr>
        <td colspan="3">
            <telerik:RadGrid runat="server" ID="specificationsGrid" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" AllowPaging="false" 
                                                                 AllowSorting="false" 
                                                                 AllowFilteringByColumn="false" 
                                                                 AutoGenerateColumns="false"
                                                                 AllowMultiRowEdit="true"
                                                                 OnPreRender="specificationsGrid_PreRender"
                                                                 OnNeedDataSource="specificationsGrid_NeedDataSource"                                                                                 
                                                                 style="min-width:400px">
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                    </ClientSettings>
                    <MasterTableView clientdatakeynames="Pk_SpecificationId" EditMode="InPlace">
                        <Columns>
                            <telerik:GridTemplateColumn DataField="Number" HeaderText="Specification Number" UniqueName="Number">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="100px"/> 
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Number") %></div>   
                                </ItemTemplate>                 
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="SpecificationType" HeaderText="Type" UniqueName="SpecificationType">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px"/> 
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"SpecificationType") %></div>  
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="Title" HeaderText="Title" UniqueName="Title">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="140px"/>
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Title") %></div>  
                                </ItemTemplate>                
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="Status" HeaderText="Status" UniqueName="Status">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px"/>
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"Status") %></div>  
                                </ItemTemplate>                
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="PrimeResponsibleGroupShortName" HeaderText="Prime responsible" UniqueName="PrimeResponsibleGroupShortName">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="60px"/>
                                <ItemTemplate>
                                    <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"PrimeResponsibleGroupShortName") %></div>  
                                </ItemTemplate>                
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="SpecificationActions" HeaderText="" UniqueName="SpecificationActions">
                                <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="50px"/>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate> 
                                    <a href="SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>&selectedTab=<%# SelectedTab %>">
                                       <img id="imgViewSpecifications" alt="See details" src="images/details.png"  style="display: block"/>  
                                    </a>                                         
                                </ItemTemplate>
                                <EditItemTemplate>
                                        <img id="imgViewSpecifications" alt="See details" src="images/details.png"  style="display: block"
                                           onclick="var popUp=window.open('SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>&selectedTab=<%# SelectedTab %>',
								           'height=550,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" />                         
                                </EditItemTemplate>                   
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            <div style="text-align:center">
                                No related specifications
                            </div>
                        </NoRecordsTemplate>
                    </MasterTableView>
                </telerik:RadGrid>
        </td>
    </tr>
    <tr>
        <td style="padding-right:5px">
            <asp:Label ID="AddSpecificationLbl" runat="server" Width="100%"></asp:Label>            
        </td>
        <td style="padding-right:5px">
            <asp:TextBox ID="txtAddSpecification" runat="server" Width="100%"></asp:TextBox>
        </td>
        <td>
            <asp:Button ID="btnAddSpecification" runat="server" Text="Add" Width="100%" />
        </td>
    </tr>
</table>