<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShareUrlControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.ShareUrlControl" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style>
    .suHeader {
        margin: 5px 0 10px 0;
    }

    .suFooter {
        text-align: right;
        margin-top: 10px;
        padding: 0 5px;
    }

    #suModal input[type=checkbox] {
        vertical-align: middle;
    }

    #suModal table {
        width: 100%;
        padding: 0 5px;
    }

    #suModal input[type=text] {
        width: 100%;
    }

    .btnShare {
        width: 20px;
        height: 20px;
        vertical-align: top;
        margin: 2px;
    }
</style>
<asp:UpdatePanel ID="upShareUrl" runat="server">
    <ContentTemplate>
        <telerik:RadToolTip runat="server" ID="radTooltipShareUrl" HideEvent="LeaveTargetAndToolTip"
            Position="TopRight"
            Width="250px"
            Height="60px"
            ShowDelay="0"
            enableshadow="true"
            TargetControlID="btnShareUrl"
            OnClientShow="clientShow"
            Skin="Silk">
            <div id="suModal" class="suHeader">

                <table>
                    <tr>
                        <td>
                            <telerik:RadButton ID="CheckBoxGetShortUrl"
                                runat="server"
                                ToggleType="CheckBox"
                                ButtonType="ToggleButton"
                                AutoPostBack="true"
                                OnClick="CheckBoxGetShortUrl_CheckedChanged"
                                Skin="Silk">
                                <togglestates>
					     <telerik:RadButtonToggleState Text="Short URL" PrimaryIconCssClass="rbToggleCheckbox" />
					     <telerik:RadButtonToggleState Text="Short URL" PrimaryIconCssClass="rbToggleCheckboxChecked" Selected="true"/>
					    </togglestates>
                            </telerik:RadButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtLink" runat="server" Text="http://3gpp.org/..." onclick="this.setSelectionRange(0, this.value.length)" />
                        </td>
                    </tr>
                </table>
            </div>
        </telerik:RadToolTip>
        <telerik:RadButton ID="btnShareUrl" runat="server"
            Enabled="true" AutoPostBack="false" OnClientClicked="open_suModal" CssClass="btnShare">
            <image imageurl="images/share.png" disabledimageurl="images/share.png" />
        </telerik:RadButton>
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    function open_suModal() {
        var suModal = $("#suModal");
        suModal.removeClass('hidden');
    }

    $(document).ready(function () {
        $('#suModal').find('.close').click(function (e) {
            e.preventDefault();
            $(this).parent().addClass('hidden');
        });
    });

    function clientShow(sender, eventArgs) {
        $("#<%=txtLink.ClientID %>").focus().select();
    }
</script>
