<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShareUrlControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.ShareUrlControl" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style>
.suHeader{margin: 5px 0 10px 0;}
.suFooter
{text-align: right;margin-top: 10px;padding: 0 5px;}
#suModal input[type=checkbox]{vertical-align:middle;}
#suModal table{width: 100%;padding: 0 5px;}
#suModal input[type=text]{width:100%;}
.btnShare
{
    width: 20px;
    height: 20px;
    vertical-align: top;
    margin:2px;
}
</style>


<telerik:RadButton ID="btnShareUrl" runat="server"
    Enabled="true" AutoPostBack="false" OnClientClicked="open_suModal" CssClass="btnShare">
    <Image ImageUrl="images/share.png" DisabledImageUrl="images/share.png" />
</telerik:RadButton>

<telerik:RadAjaxManagerProxy ID="RadAjaxMngShareUrl" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="CheckBoxGetShortUrl">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="txtLink" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy >



<telerik:RadToolTip runat="server" ID="radTooltipShareUrl" HideEvent="LeaveTargetAndToolTip" 	
    Position="TopCenter"
    Width="250px" 
    Height="60px"  
    ShowDelay="0"
    Animation="Fade" 
    enableshadow="true"
    TargetControlID="btnShareUrl"
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
					    <ToggleStates>
					     <telerik:RadButtonToggleState Text="Short URL" PrimaryIconCssClass="rbToggleCheckbox" />
					     <telerik:RadButtonToggleState Text="Short URL" PrimaryIconCssClass="rbToggleCheckboxChecked" Selected="true"/>
					    </ToggleStates>
					</telerik:RadButton>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLink" runat="server" Text="http://3gpp.org/..." />
                </td>
            </tr>
        </table>
    </div>
</telerik:RadToolTip>




<telerik:RadScriptBlock runat="server">
	<script type="text/javascript">
	    function open_suModal() {
	        var suModal = $("#suModal");
	        suModal.removeClass('hidden');
	    }

	    $("#<%=txtLink.ClientID %>").click(function () {
	        this.select();
	    });

	    $(document).ready(function () {
	        $('#suModal').find('.close').click(function (e) {
	            e.preventDefault();
	            $(this).parent().addClass('hidden');
	        });
	    });
	</script>
</telerik:RadScriptBlock>
