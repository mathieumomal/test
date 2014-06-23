<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Etsi.Ultimate.Module.CRs.View" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<div id="crsList">
 <telerik:RadGrid runat="server" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false"  Skin="Ultimate" ID="crsTable" OnItemDataBound="crsTable_ItemDataBound"  
         AllowPaging="false" AllowSorting="true" AllowFilteringByColumn="false" AutoGenerateColumns="false"
         EnableViewState="false">
        <MasterTableView ClientDataKeyNames="Pk_ChangeRequest">
            <SortExpressions>
                <telerik:GridSortExpression FieldName="targetSpecificationNumber" SortOrder="Ascending" />
                <telerik:GridSortExpression FieldName="crNumberAndRevision" SortOrder="None" />                
            </SortExpressions>
            <Columns>
                <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="targetSpecificationNumber" ShowSortIcon="false" HeaderText="Target spec #" UniqueName="targetSpecificationNumber" ></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="crNumberAndRevision" ShowSortIcon="false" HeaderText="CR # & Revision" UniqueName="crNumberAndRevision" ></telerik:GridBoundColumn>

               
                <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="Subject" AllowSorting="false" HeaderText="Title" UniqueName="Subject" ></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="WgMtgShortRef" AllowSorting="false" HeaderText="WG meeting" UniqueName="WgMtgShortRef" ></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="6%" DataField="wgStatus" HeaderText="WG status" UniqueName="wgStatus">
                    <ItemTemplate>
                        <span><%# DataBinder.Eval(Container.DataItem,"Enum_TDocStatusWG.Status")%></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="8%" DataField="WgTDocLink" HeaderText="WG TDoc" UniqueName="WgTDocLink" >
                    <ItemTemplate>
                        <div class="text-left">
                            <asp:HyperLink runat="server" ID="WgTDocLink" Target='<%# DataBinder.Eval(Container.DataItem,"WgTDocLink")%>' ImageUrl="/DesktopModules/CRs/images/TDocLink.png"/>                            
                        </div>   
                    </ItemTemplate> 
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="TsgMtgShortRef" AllowSorting="false" HeaderText="TSG meeting" UniqueName="TsgMtgShortRef" ></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="6%" DataField="tsgStatus" HeaderText="TSG status" UniqueName="tsgStatus">
                    <ItemTemplate>
                        <span><%# DataBinder.Eval(Container.DataItem,"Enum_TDocStatusTSG.Status")%></span>  
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="8%" DataField="TsgTDocLink" HeaderText="TSG TDoc" UniqueName="TsgTDocLink" >
                    <ItemTemplate>
                        <div class="text-left">
                            <asp:HyperLink runat="server" ID="TsgTDocLink" Target='<%# DataBinder.Eval(Container.DataItem,"TsgTDocLink")%>' ImageUrl="/DesktopModules/CRs/images/TDocLink.png"/>
                        </div>   
                    </ItemTemplate> 
                </telerik:GridTemplateColumn>
                 <telerik:GridBoundColumn HeaderStyle-Width="8%" DataField="ImplementationStatus" AllowSorting="false" HeaderText="Implementation status" UniqueName="ImplementationStatus" ></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="8%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" UniqueName="CRAdditionalDetails">
                    <ItemTemplate>
                        <table id="CRAdditionalDetails">
                            <tr>                                
                                <td>                                    
                                    <asp:HyperLink runat="server" ID="CreateRevisionAction" ImageUrl="/DesktopModules/CRs/images/CR_Revision.png" ToolTip="Create revision" Enabled="true"/>
                                </td>
                                <td>
                                    <asp:HyperLink runat="server" ID="CreateTDocAction" ImageUrl="/DesktopModules/CRs/images/CR_TDoc.png" ToolTip="Create TDoc" Enabled="true"/> 
                                </td>
                                    
                                <td>
                                    <asp:HyperLink runat="server" ID="imgErrors" ImageUrl="/DesktopModules/CRs/images/Cr_ImplemError.png" ToolTip="All CRs implementation errors"/>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</div>
