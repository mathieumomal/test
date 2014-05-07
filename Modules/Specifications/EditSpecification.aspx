<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSpecification.aspx.cs" Inherits="Etsi.Ultimate.Module.Specifications.EditSpecification" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Etsi.Ultimate.Module.Specifications" Namespace="Etsi.Ultimate.Module.Specifications" TagPrefix="specification" %>
<%@ Register TagPrefix="ult" TagName="RemarksControl" Src="../../controls/Ultimate/RemarksControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="HistoryControl" Src="../../controls/Ultimate/HistoryControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="RapporteurControl " Src="../../controls/Ultimate/RapporteurControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="RelatedWiControl" Src="../../controls/Ultimate/RelatedWiControl.ascx" %>
<%@ Register TagPrefix="ult" TagName="CommunityControl" Src="../../controls/Ultimate/CommunityControl.ascx" %>
<%@ Register TagPrefix="spec" TagName="SpecificationListControl" Src="SpecificationListControl.ascx" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Specification</title>
    <link rel="stylesheet" type="text/css" href="module.css">
    <link rel="SHORTCUT ICON" href="images/favicon.ico" type="image/x-icon">
    <script src="JS/jquery.min.js"></script>
    <script src="JS/jquery-validate.min.js"></script>
    <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
</head>
<body>
    <form id="specEditForm" runat="server">
        <asp:Panel runat="server" ID="fixContainer" CssClass="containerFix" Width="750px" Height="600">
            <asp:Panel ID="specMsg" runat="server" Visible="false">
                <asp:Label runat="server" ID="specMsgTxt"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="specBody" runat="server" CssClass="specificationDetailsBody">
                <telerik:RadScriptManager runat="server" ID="rsmSpecificationEdit" />
                <telerik:RadAjaxManager ID="wiRadAjaxManager" runat="server" EnablePageHeadUpdate="false" UpdatePanelsRenderMode="Inline">
                </telerik:RadAjaxManager>
                <div class="HeaderText">
                    <asp:Label ID="lblHeaderText" runat="server"></asp:Label>
                </div>
                <telerik:RadTabStrip ID="rtsSpecEdit" runat="server" MultiPageID="rmpSpecEdit"
                    AutoPostBack="false">
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="rmpSpecEdit" runat="server" Width="100%" Height="560" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
                    <telerik:RadPageView ID="RadPageGeneral" runat="server" Selected="true">
                        <table style="width: 100%">
                            <tr>
                                <td class="LeftColumn">Reference</td>
                                <td class="RightColumn">
                                    <asp:TextBox ID="txtReference" Width="198" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="LeftColumn">Title<span class='requiredField'>(*)</span></td>
                                <td class="RightColumn">
                                    <asp:TextBox ID="txtTitle" Width="350" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="LeftColumn">Status:</td>
                                <td class="RightColumn">
                                    <asp:Label ID="lblStatus" runat="server">-</asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="LeftColumn">Type</td>
                                <td class="RightColumn">
                                    <asp:DropDownList ID="ddlType" Width="200" runat="server">
                                        <asp:ListItem Selected="True" Text="Technical Specification (TS)" Value="true" />
                                        <asp:ListItem Text="Technical Report (TR)" Value="false" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="LeftColumn">Initial planned Release
                                </td>
                                <td class="RightColumn">
                                    <asp:DropDownList ID="ddlPlannedRelease" Width="200" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="LeftColumn">Internal</td>
                                <td class="RightColumn">
                                    <asp:CheckBox ID="chkInternal" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="LeftColumn">Common IMS Specification</td>
                                <td class="RightColumn">
                                    <asp:CheckBox ID="chkCommonIMSSpec" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="LeftColumn">Radio technology</td>
                                <td class="RightColumn">
                                    <asp:CheckBoxList ID="cblRadioTechnology" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList></td>
                            </tr>
                            <tr style="max-height: 150px; overflow-y: scroll; margin-top: 10px">
                                <td colspan="2" class="specificationRemarks">
                                    <asp:UpdatePanel ID="upSpecRemarks" runat="server">
                                        <ContentTemplate>
                                            <ult:remarkscontrol runat="server" id="specificationRemarks" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageResponsibility" runat="server">
                        <asp:UpdatePanel ID="upSpecResponsibility" runat="server">
                            <ContentTemplate>
                                <table style="width: 100%">
                                    <tr>
                                        <td class="LeftColumn">Primary responsible group</td>
                                        <td class="RightColumn">
                                            <ult:communitycontrol id="PrimaryResGrpCtrl" width="200" issingleselection="true" iseditmode="true" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="LeftColumn">Secondary responsible groups</td>
                                        <td class="RightColumn">
                                            <ult:communitycontrol id="SecondaryResGrpCtrl" width="200" issingleselection="false" iseditmode="true" runat="server" />
                                        </td>
                                    </tr>
                                    <tr style="max-height: 150px; overflow-y: scroll; margin-top: 10px">
                                        <td colspan="2" class="specificationRapporteurs">
                                            <ult:rapporteurcontrol runat="server" id="specificationRapporteurs" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageRelated" runat="server">
                        <asp:Panel ID="RelatedSpecificationsPanel" runat="server">
                            <div style="max-height: 160px;">
                                <fieldset id="ParentFieldset">
                                    <legend>
                                        <asp:Label ID="ParentSpecLbl" runat="server" Text="Parent Specifications"></asp:Label></legend>
                                    <spec:SpecificationListControl runat="server" ID="parentSpecifications" />
                                </fieldset>
                            </div>
                            <br />
                            <div style="max-height: 160px;">
                                <fieldset id="ChildFieldset">
                                    <legend>
                                        <asp:Label ID="ChildSpecLbl" runat="server" Text="Child Specifications"></asp:Label></legend>
                                    <spec:SpecificationListControl runat="server" ID="childSpecifications" />
                                </fieldset>
                            </div>
                            <br />
                            <div style="max-height: 220px;">
                                <ult:relatedwicontrol id="SpecificationRelatedWorkItems" runat="server" />
                            </div>
                        </asp:Panel>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageReleases" runat="server">
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="RadPageHistory" runat="server">
                        <div class="TabContent" style="overflow-y: auto; overflow-x: auto">
                            <ult:historycontrol runat="server" id="specificationHistory" />
                        </div>
                    </telerik:RadPageView>
                </telerik:RadMultiPage>
                <div class="specificationDetailsAction">
                    <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn3GPP-success" OnClick="SaveSpec_Click" />
                    <asp:LinkButton ID="btnSaveDisabled" runat="server" Text="Save" CssClass="btn3GPP-default" disabled="disabled" OnClientClick="return false;" />
                    <asp:LinkButton ID="btnExit" runat="server" Text="Exit" CssClass="btn3GPP-success" OnClick="ExitSpecEdit_Click" />
                </div>

                <script type="text/javascript">


                    //Check if the inserted value is a valid URL */
                    function isURLValid(element) {
                        var controlSelector = "#" + element
                        var urlregex = '^(http:\/\/|https:\/\/|ftp:\/\/){1}([0-9A-Za-z]+\.)'
                        if ($(controlSelector).val() == "") {
                            return true;
                        }
                        if (!$(controlSelector).val().match(urlregex)) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    }

                    //Check if the inserted value is not empty or composed by spaces
                    function isEmpty(element) {
                        var controlSelector = "#" + element;
                        $(controlSelector).removeClass('required');
                        if ($.trim($(controlSelector).val()).length == 0) {
                            $(controlSelector).addClass('required');
                            return true;
                        }
                        return false;
                    }

                    function validateformTrim() {
                        isEmpty("txtTitle");
                    }

                    /* Check if all form's field are valid */
                    function validateform(errorClassName) {
                        validateformTrim();
                        if (!$("#txtTitle").hasClass(errorClassName)) {
                            $('#btnSave').show();
                            $('#btnSaveDisabled').hide();
                        }
                        else {
                            $('#btnSaveDisabled').show();
                            $('#btnSave').hide();
                        }
                    }

                    //Perform a form validation on each field keyUp
                    function formValidator() {
                        var errorClassName = 'required';
                        var validator = $("#specEditForm").validate({
                            errorClass: "required",
                            onsubmit: true,
                            onKeyup: true,
                            eachValidField: function () {
                                $(this).removeClass('required');
                                validateform(errorClassName);
                                validateformTrim();
                            },
                            eachInvalidField: function () {
                                $(this).addClass('required');
                                $('#btnSave').hide();
                                $('#btnSaveDisabled').show();
                            }
                        });
                    }

                    $(document).ready(function () {

                        // Used for creation to tell jquery validate that those fields are non valid
                        if ($('#txtTitle').val() == "") $('#txtTitle').addClass('required');

                        //Validate form
                        formValidator();


                    });
                </script>
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
