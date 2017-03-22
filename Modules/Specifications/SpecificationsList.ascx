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
<%@ Register TagPrefix="ult" TagName="CommunityHyperlinkControl" Src="../../controls/Ultimate/CommunityHyperlinkControl.ascx" %>

<telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
    <script src="desktopmodules/specifications/JS/CommonScript.js?v=<%=ConfigurationManager.AppSettings["AppVersion"] %>"></script>
</telerik:RadCodeBlock>

<asp:Panel ID="componentSpecList" runat="server" Visible="false" ClientIDMode="Static">
    <asp:UpdateProgress ID="updateProgressSpecificationGrid" runat="server" DisplayAfter="200">
        <ProgressTemplate>
            <div class="modalBackground">
            </div>
            <div class="updateProgress">
                <asp:Image ID="imgProgress" runat="server" Class="rotating" ImageUrl="~/DesktopModules/WorkItem/images/hourglass.png" Width="45" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="upSpecificationGrid" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table style="width: 100%;">
                <tr>
                    <td style="line-height: 22px;">
                        <asp:LinkButton ID="btnNewSpecification" class="btn3GPP-success" runat="server" OnClientClick="OpenSpecDetailsPage('/desktopmodules/Specifications/EditSpecification.aspx?action=create', 'Specification-Create');return false;" Text="New Specification" />                    
                        <span style="float: right; padding-bottom: 2px; white-space: nowrap">
                            <asp:Button ID="imgBtnFTP" Text="Manage meeting folder" runat="server" CssClass="btn3GPP-success customizePanelButtons" OnClientClick="openFTPConfiguration(); return false;" ToolTip="Manage specifications folders on FTP" />
                            <asp:Button ID="lnkManageITURecommendations" Text="ITU" runat="server" CssClass="btn3GPP-success customizePanelButtons" ToolTip="Manage ITU recommendations" OnClientClick="return openITURecommendationPopUp();" />
                            <asp:ImageButton ID="btnSpecExport" runat="server" CssClass="customizePanelButtons customizeButtonsImages" AlternateText="Export" ImageUrl="/DesktopModules/Specifications/images/excel_export.png" OnClick="btnSpecExport_Click" OnClientClick="removeBg" ToolTip="Download to Excel" />
                            <ult:fullviewcontrol ID="ultFullView" runat="server" />
                            <asp:HiddenField ID="hidSpecAddress" runat="server" Value="" />
                        </span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <telerik:RadPanelBar runat="server" ID="rpbSpecSearch" Width="100%" OnClientItemClicking="PreventCollapse" >
                            <Items>
                                <telerik:RadPanelItem runat="server" ID="searchPanel" Expanded="True">
                                    <HeaderTemplate>
                                        <table style="width: 100%; vertical-align: middle" class="SpecificationSearchHeader">
                                            <tr>
                                                <td style="width: 20px;">
                                                    <ult:shareurlcontrol id="ultShareUrl" runat="server" />
                                                </td>
                                                <td style="text-align: center" class="openCloseRadPanelBar">
                                                    <asp:Label ID="lblSearchHeader" runat="server" CssClass="openCloseRadPanelBar"/>
                                                </td>
                                                <td style="width: 180px;">
                                                    <span>Items per page</span>
                                                    <telerik:RadComboBox ID="SelectPageSize" runat="server" Width="80" OnSelectedIndexChanged="SelectPageSize_OnSelectedIndexChanged" AutoPostBack="True">
                                                    </telerik:RadComboBox>
                                                </td>
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
                                            <table style="width: 100%; padding: 5px 5px 5px 5px;">
                                                <tr>
                                                    <!-- Column 1 -->
                                                    <td style="width: 33%; vertical-align: top;">
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
                                                        </table>
                                                    </td>
                                                    <!-- Column 2 -->
                                                    <td style="width: 33%; vertical-align: top;">
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td style="width: 40%;">Release</td>
                                                                <td style="width: 60%;">
                                                                    <ult:releasesearchcontrol id="ReleaseCtrl" runat="server" width="200" dropdownwidth="200" />
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
                                                            <tr>
                                                                <td>Technology</td>
                                                                <td>
                                                                    <asp:CheckBoxList ID="cblTechnology" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <!-- Column 3 -->
                                                    <td style="width: 33%; vertical-align: top;">
                                                        <table style="width: 100%;">
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
                                                                <td>&nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" style="text-align: right; padding-right: 20px">
                                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" Width="150px" OnClick="btnSearch_Click"></asp:Button></td>
                                                                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="150px" OnClick="btnRefresh_Click" CssClass="btnHidden"></asp:Button>
                                                                </td>
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
                             <PagerStyle AlwaysVisible="true" Mode="NextPrevAndNumeric" Position="Top" PageButtonCount="10" PagerTextFormat="{4} {5} specifications found, displaying {2} to {3}" />
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
                                    <telerik:GridTemplateColumn HeaderStyle-Width="10%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderText="<span title='TSG or WG which has primary responsibility for the specification' class='helpTooltip'>Primary Responsible Group</span>" UniqueName="PrimeResponsibleGroupShortName">
                                        <ItemTemplate>
                                            <ult:CommunityHyperlinkControl runat="server" ID="primaryResponsibleGroup"></ult:CommunityHyperlinkControl>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="17%" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" UniqueName="SpecificationAdditionalDetails">
                                        <ItemTemplate>
                                            <table id="specAdditionalDetails">
                                                <tr>
                                                    <td>
                                                        <img id="imgViewSpecifications" title="See details" class="imgViewSpecifications" alt="See details" src="/DesktopModules/Specifications/images/details.png"
                                                            onclick="OpenSpecDetailsPage('/desktopmodules/Specifications/SpecificationDetails.aspx?specificationId=<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>', 'Specification-<%# DataBinder.Eval(Container.DataItem,"Pk_SpecificationId").ToString() %>');" /></td>
                                                    <td>
                                                        <img id="imgLock" title="<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsForPublication")) ? "For publication" : "Internal" %>" alt="Internal" src="/DesktopModules/Specifications/images/lock.png" style='opacity: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"IsForPublication")) ? "0.1" : "1" %>' /></td>
                                                    <td>
                                                        <img id="imgIMS" title="Common IMS" alt="Common IMS" src="/DesktopModules/Specifications/images/ims.png" style='opacity: <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem,"ComIMS")) ? "1" : "0.1" %>' />
                                                    </td>
                                                    <td>
                                                        <asp:Image ID="img2G" runat="server" ToolTip="2G" AlternateText="2G" ImageUrl="/DesktopModules/Specifications/images/2g.png" />
                                                    </td>
                                                    <td>
                                                        <asp:Image ID="img3G" runat="server" ToolTip="3G" AlternateText="3G" ImageUrl="/DesktopModules/Specifications/images/3g.png" />
                                                    </td>
                                                    <td>
                                                        <asp:Image ID="imgLTE" runat="server" ToolTip="LTE" AlternateText="LTE" ImageUrl="/DesktopModules/Specifications/images/lte.png" />
                                                    </td>
                                                    <td>
                                                        <asp:Image ID="img5G" runat="server" ToolTip="5G" AlternateText="5G" ImageUrl="/DesktopModules/Specifications/images/5g.png" />
                                                    </td>
                                                    <td>
                                                        <asp:HyperLink runat="server" ID="lnkCr" Target="_blank" CssClass="img imgCR" ImageUrl="/DesktopModules/Specifications/images/cr.png" ToolTip="All CRs for this specification" NavigateUrl="#"></asp:HyperLink>
                                                    </td>
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
    <telerik:RadAjaxManager ID="ramVersions" runat="server" EnablePageHeadUpdate="false" OnAjaxRequest="ramVersions_AjaxRequest">
        <ajaxsettings>
                <telerik:AjaxSetting AjaxControlID="ramVersions">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="FtpStatusLabel"></telerik:AjaxUpdatedControl>
                        <telerik:AjaxUpdatedControl ControlID="progressBarContent"></telerik:AjaxUpdatedControl>
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </ajaxsettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindowManager ID="rwmVersions" runat="server">
        <Windows>
            <telerik:RadWindow ID="rwFTPConfiguration" runat="server" Modal="true" Behaviors="Close"
                Title="Manage Latest Folder" Height="280" Width="510" VisibleStatusbar="false" IconUrl="false" ReloadOnShow="true" 
                ShowContentDuringLoad="false" OnClientBeforeClose="OnClientBeforeClose">
                <ContentTemplate>
                    <div id="divFTPConfiguration">
                        <div style="margin: 5px 5px 5px 5px">
                            <asp:Label ID="FtpStatusLabel" runat="server" CssClass="hide"/>
                            <br /><span class="legend">Current meeting folder: </span><asp:Label runat="server" ID="currentFolderLabel" CssClass="legend"></asp:Label>
                            <br />In order to create new meeting folder, please provide its name: 
                            <asp:TextBox runat="server"  ID="txtFTPFolderName"></asp:TextBox><span class="mandatory">*</span>
                        </div>
                        <!-- FTP Progress Bar -->
                        <asp:Literal runat="server" ID="progressBarContent"></asp:Literal>
                        <div class="footer" style="text-align: right">
                            <asp:Button ID="btnCreateFolder" CssClass="btn3GPP-default" runat="server" Text="Create" OnClientClick="CreateLatestFolder();return false;"/>
                            <asp:Button ID="btnCancel" CssClass="btn3GPP-success" runat="server" Text="Close" OnClientClick="return closePopUpWindow();" />
                        </div>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    <script type="text/javascript">
        /** open or close RadPanelBar only if element has openCloseRadPanelBar class **/
        function PreventCollapse(sender, eventArgs) {
            if (eventArgs.get_domEvent().target.className != "openCloseRadPanelBar") {
                eventArgs.set_cancel(true);
            }
        }

        var lastVal = "";

        /** Function used after each query **/
        function processEndRequest() {
            checkExport();
            adaptContentHeight();
        }

        function checkExport() {
            var hidExport = $("#<%=hidSpecAddress.ClientID %>");
		    if (hidExport != null && hidExport.val() != "" && hidExport.val() != lastVal) {
		        lastVal = hidExport.val();
		        window.location.replace(hidExport.val());
		    }
		    adaptContentHeight();
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

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(processEndRequest);

        function openFTPConfiguration() {
            var radWindowFTPConfiguration = $find("<%= rwFTPConfiguration.ClientID %>");
            radWindowFTPConfiguration.show();
        };

        function OnClientBeforeClose(sender, args)
        {
            $("#<%= txtFTPFolderName.ClientID %>").attr("disabled", false).val(''); /* clear text box */
    		$("#<%= btnCreateFolder.ClientID %>").removeClass().addClass('btn3GPP-default').attr("disabled", true); /* init create button */
    		$("#<%= FtpStatusLabel.ClientID %>").removeClass().addClass('hide');
    		$("#<%= updateProgressSpecificationGrid.ClientID %>").removeClass('forceHidden'); /* show Grid update progress */
        }

    	function closePopUpWindow() {
    		var radWindowFTPConfiguration = $find("<%= rwFTPConfiguration.ClientID %>");
            radWindowFTPConfiguration.close();
            return false;
        }

        // Function called by outside page to trigger a click on the search button.
        function refreshSpecList() {
            var searchButton = $("#<%= btnRefresh.ClientID %>");
            if (searchButton != null) {
                searchButton.click();
            }
        }
        function openITURecommendationPopUp() {
            var popUp = window.open('/desktopmodules/Specifications/ITURecommendations.aspx', 'ituRecPopup', 'height=340,width=674,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no');
            popUp.focus();
            return false;
        }

        function CreateLatestFolder() {
            $("#<%= updateProgressSpecificationGrid.ClientID %>").addClass('forceHidden'); /* hide Grid update progress */

            $("#<%= FtpStatusLabel.ClientID %>").text('Please wait...');
            $("#<%= btnCreateFolder.ClientID %>").attr('disabled', 'disabled').removeClass("btn3GPP-success").addClass("btn3GPP-default");
            var txtFTPFolderName = $("#<%= txtFTPFolderName.ClientID %>");
            txtFTPFolderName.attr('disabled', 'disabled');

            var ajaxManager = $find("<%= ramVersions.ClientID %>");
            ajaxManager.ajaxRequest("CreateLatestFolder");
        }

        function RefreshCreateLatestFolderStatus() {
            var ajaxManager = $find("<%= ramVersions.ClientID %>");
            ajaxManager.ajaxRequest("RefreshCreateLatestFolderStatus");
        }

        $(document).ready(function () {
            $("#divFTPConfiguration input[type=text]").keyup(function () {
                var textBox = $("#divFTPConfiguration input[type=text]");
                var btnCreateFolder = $("#<%= btnCreateFolder.ClientID %>");
                btnCreateFolder.removeClass('btn3GPP-default').removeClass('btn3GPP-success');

                if (textBox.val() != '') {
                    btnCreateFolder.addClass('btn3GPP-success').attr("disabled", false);
                } else {
                    btnCreateFolder.addClass('btn3GPP-default').attr("disabled", true);
                }
            });
        });

    </script>
</asp:Panel>

