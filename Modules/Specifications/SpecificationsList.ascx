<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationsList.ascx.cs" Inherits="Etsi.Ultimate.Module.Specifications.SpecificationsList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!--Import module.css-->
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/Specifications/module.css" />
<!--Import module.css-->

<%@ Register TagPrefix="ult" TagName="ShareUrlControl" Src="../../controls/Ultimate/ShareUrlControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="FullViewControl" Src="../../controls/Ultimate/FullView.ascx" %>
<%@ Register TagPrefix="ult" TagName="ReleaseSearchControl" Src="../../controls/Ultimate/ReleaseSearchControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="CommunityControl" Src="../../controls/Ultimate/CommunityControl.ascx" %>

<div id="componentSpecList">
    <asp:UpdateProgress ID="updateProgressSpecificationGrid" runat="server" DisplayAfter="200">
        <ProgressTemplate>
            <div class="modalBackground">
            </div>
            <div class="updateProgress fixed">
                <asp:Image ID="imgProgress" runat="server" Class="rotating" ImageUrl="~/DesktopModules/WorkItem/images/hourglass.png" Width="45" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="upSpecificationGrid" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table style="width: 100%;">
                <tr>
                    <td style="line-height: 22px;">
                        <asp:LinkButton ID="btnNewSpecification" class="btn3GPP-success" runat="server" OnClientClick="var popUp=window.open('/desktopmodules/Specifications/EditSpecification.aspx?action=create', 'Specification-Create', 'height=690,width=674,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();return false;" Text="New Specification" />                    
                        <span style="float: right; padding-bottom: 2px; white-space: nowrap">
                            <asp:Button ID="imgBtnFTP" Text="FTP" runat="server" CssClass="btn3GPP-success customizePanelButtons" OnClientClick="openFTPConfiguration(); return false;" ToolTip="Manage Manage specifications folders on FTP" />
                            <asp:Button ID="lnkManageITURecommendations" Text="ITU" runat="server" CssClass="btn3GPP-success customizePanelButtons" ToolTip="Manage ITU recommendations" />
                            <asp:ImageButton ID="btnSpecExport" runat="server" CssClass="customizePanelButtons customizeButtonsImages" AlternateText="Export" ImageUrl="/DesktopModules/Specifications/images/excel_export.png" OnClick="btnSpecExport_Click" OnClientClick="removeBg" ToolTip="Download to Excel" />
                            <ult:fullviewcontrol ID="ultFullView" runat="server" />
                            <asp:HiddenField ID="hidSpecAddress" runat="server" Value="" />
                        </span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <telerik:RadPanelBar runat="server" ID="rpbSpecSearch" Width="100%">
                            <Items>
                                <telerik:RadPanelItem runat="server" ID="searchPanel">
                                    <HeaderTemplate>
                                        <table style="width: 100%; vertical-align: middle" class="SpecificationSearchHeader">
                                            <tr>
                                                <td style="width: 20px;">
                                                    <ult:shareurlcontrol id="ultShareUrl" runat="server" />
                                                </td>
                                                <td style="text-align: center">
                                                    <asp:Label ID="lblSearchHeader" runat="server" /></td>
                                                <td style="width: 20px;">
                                                    <a class="rpExpandable">
                                                        <span class="rpExpandHandle"></span>
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlSearchContainer" runat="server" DefaultButton="btnSearch">
                                            <table style="width: 100%; padding: 20px 50px 20px 50px;">
                                                <tr>
                                                    <td style="width: 50%; vertical-align: top;">
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td style="width: 40%;">Title/Specification number</td>
                                                                <td style="width: 60%;">
                                                                    <asp:TextBox ID="txtTitle" Width="196px" runat="server"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td>Series</td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="rcbSeries" runat="server" Width="200px" CheckBoxes="true"></telerik:RadComboBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Type</td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbTechnicalSpecification" runat="server" Text="Technical Specification (TS)"></asp:CheckBox><br />
                                                                    <asp:CheckBox ID="cbTechnicalReport" runat="server" Text="Technical Report (TR)"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Publication</td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbInternal" runat="server" Text="Internal"></asp:CheckBox><br />
                                                                    <asp:CheckBox ID="cbForPublication" runat="server" Text="For Publication"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                            <tr runat="server" id="trNumberNotYetAllocated" visible="false">
                                                                <td>Number not yet allocated</td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbNumNotYetAllocated" runat="server"></asp:CheckBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 50%; vertical-align: top;">
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td style="width: 40%;">Release</td>
                                                                <td style="width: 60%;">
                                                                    <ult:releasesearchcontrol id="ReleaseCtrl" runat="server" width="200" dropdownwidth="200" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Status</td>
                                                                <td>
                                                                    <asp:CheckBox ID="cbDraft" runat="server" Text="Draft"></asp:CheckBox><br />
                                                                    <asp:CheckBox ID="cbUnderCC" runat="server" Text="Under change control"></asp:CheckBox><br />
                                                                    <asp:CheckBox ID="cbWithdrawnBeforeCC" runat="server" Text="Withdrawn before change control"></asp:CheckBox><br />
                                                                    <asp:CheckBox ID="cbWithdrawnAfterCC" runat="server" Text="Withdrawn under change control"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Technology</td>
                                                                <td>
                                                                    <asp:CheckBoxList ID="cblTechnology" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>&nbsp;</td>
                                                                <td>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" style="text-align: right; padding-right: 20px">
                                                                    <asp:Button ID="btnDefault" Visible="false" runat="server" Text="Default" Width="150px" OnClientClick="collapseItem()"></asp:Button>
                                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" Width="150px" OnClick="btnSearch_Click" OnClientClick="collapseItem()"></asp:Button></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>
                    </td>
                </tr>
                <tr>
                    <td>
                        <telerik:RadGrid ID="rgSpecificationList" runat="server"
                            EnableEmbeddedSkins="false"
                            EnableEmbeddedBaseStylesheet="false"
                            Skin="Ultimate"
                            AllowSorting="true"
                            AllowPaging="true"
                            AllowCustomPaging="true"
                            PageSize="50"
                            AllowFilteringByColumn="false"
                            AutoGenerateColumns="false"
                            OnNeedDataSource="rgSpecificationList_NeedDataSource"
                            OnItemDataBound="rgSpecificationList_ItemDataBound">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"></Scrolling>
                            </ClientSettings>
                            <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" PageButtonCount="10" />
                            <MasterTableView ClientDataKeyNames="Pk_SpecificationId" Width="100%" AllowNaturalSort="false">
                                <SortExpressions>
                                    <telerik:GridSortExpression FieldName="Title" SortOrder="None" />
                                    <telerik:GridSortExpression FieldName="Number" SortOrder="Ascending" />
                                </SortExpressions>
                                <Columns>
                                    <telerik:GridBoundColumn HeaderStyle-Width="8%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ShowSortIcon="false" DataField="Number" HeaderText="Specification Number" UniqueName="Number"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="5%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="SpecificationTypeShortName" HeaderText="Type" UniqueName="Type"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="50%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" ShowSortIcon="false" DataField="Title" HeaderText="Title" UniqueName="Title"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="Status" HeaderText="Status" UniqueName="Status"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" AllowSorting="false" DataField="PrimeResponsibleGroupShortName" HeaderText="<span title='TSG or WG which has primary responsibility for the specification' class='helpTooltip'>Primary Responsible Group</span>" UniqueName="PrimeResponsibleGroupShortName"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="17%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" UniqueName="SpecificationAdditionalDetails">
                                        <ItemTemplate>
                                            <table id="specAdditionalDetails">
                                                <tr>
                                                    <td>
                                                        <img id="imgViewSpecifications" title="See details" class="imgViewSpecifications" alt="See details" src="/DesktopModules/Specifications/images/details.png"
                                                            onclick="var popUp=window.open('/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 'Specification-<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 'height=690,width=674,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no'); popUp.focus();" /></td>
                                                    <td>
                                                        <img id="imgLock" title="<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsForPublication")) ? "For publication" : "Internal" %>" alt="Internal" src="/DesktopModules/Specifications/images/lock.png" style='opacity: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsForPublication")) ? "0.1" : "1" %>' /></td>
                                                    <td>
                                                        <img id="imgIMS" title="Common IMS" alt="Common IMS" src="/DesktopModules/Specifications/images/ims.png" style='opacity: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"ComIMS")) ? "1" : "0.1" %>' /></td>
                                                    <td>
                                                        <asp:Image ID="img2G" runat="server" ToolTip="2G" AlternateText="2G" ImageUrl="/DesktopModules/Specifications/images/2g.png" /></td>
                                                    <td>
                                                        <asp:Image ID="img3G" runat="server" ToolTip="3G" AlternateText="3G" ImageUrl="/DesktopModules/Specifications/images/3g.png" /></td>
                                                    <td>
                                                        <asp:Image ID="imgLTE" runat="server" ToolTip="LTE" AlternateText="LTE" ImageUrl="/DesktopModules/Specifications/images/lte.png" /></td>
                                                    <td>
                                                        <img id="imgCR" title="All CRs for this specification" class="imgCR" alt="All CRs for this specification" src="/DesktopModules/Specifications/images/cr.png" style='opacity: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsUnderChangeControl")) ? "1" : "0.1" %>' /></td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <telerik:RadAjaxManager ID="ramVersions" runat="server" EnablePageHeadUpdate="false">
    </telerik:RadAjaxManager>
    <telerik:RadWindowManager ID="rwmVersions" runat="server">
        <Windows>
            <telerik:RadWindow ID="rwFTPConfiguration" runat="server" Modal="true" Behaviors="Close" Title="FTP Configuration" Height="225" Width="400" VisibleStatusbar="false" IconUrl="false">
                <ContentTemplate>
                    <div id="divFTPConfiguration" style="padding: 5px">
                        <div style="margin: 5px 5px 5px 5px">
                            Do you want to create missing hardlinks from '\Specs\<asp:Label ID="lblFolderPath" runat="server"/>' to '\Specs\latest' ?
                            <br />
                            <br />
                            <br />
                            *This is required only when you map 'VersionsLatestFTPFolder' in web.config to an non-empty folder
                        </div>
                        <br />
                        <div class="footer" style="text-align: right">
                            <asp:Button ID="btnConfirm" runat="server" Text="Yes" OnClick="btnConfirm_Click"/>
                            <asp:Button ID="btnCancel" runat="server" Text="No" OnClientClick="return closePopUpWindow();" />
                        </div>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <script type="text/javascript">
        var lastVal = "";
        function checkExport() {
            var hidExport = $("#<%=hidSpecAddress.ClientID %>");
            if (hidExport != null && hidExport.val() != "" && hidExport.val() != lastVal) {
                lastVal = hidExport.val();
                window.location.replace(hidExport.val());
            }
        }

        function removeBg() {
            setTimeout(function () {
                alert('test');
                var greyArea = $("#<%=updateProgressSpecificationGrid.ClientID %>");
                greyArea.css('visibility', 'hidden');
            }, 2000);
        }




        function collapseItem() {
            var panelBar = $find("<%= rpbSpecSearch.ClientID %>");
            var item = panelBar.get_items().getItem(0);
            if (item) {
                item.collapse();
            }
        }

        //Adapt RagGrid height based on the "contentHeight" event (in the mainpage.ascx)
        $("#content").on('contentHeight', function (event, hContent) {
            var specGrid = $('#<%= rgSpecificationList.ClientID %>');
            var gridDiv = specGrid.find(".rgDataDiv")[0];
            if ($('.livetabssouthstreet').length == 0)
                var securityValue = 120;
            else
                var securityValue = 205;

            gridDiv.style.height = (hContent - securityValue) + "px";
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(checkExport);

        function openFTPConfiguration() {
            var radWindowFTPConfiguration = $find("<%= rwFTPConfiguration.ClientID %>");
            radWindowFTPConfiguration.show();
        };

        function closePopUpWindow() {
            var radWindowFTPConfiguration = $find("<%= rwFTPConfiguration.ClientID %>");
            radWindowFTPConfiguration.close();
            return false;
        }

    </script>
</div>

