<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShareUrlControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.ShareUrlControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style>
#divContentModalShareUrl{padding: 0px 5px;}
#divContentModalShareUrl {
	width: 260px;
	height: 134px;
	border: solid 1px rgb(201, 201, 201);
	position: absolute;
	z-index: 10000;
	background: white;
	-webkit-border-radius: 5px;
	-moz-border-radius: 5px;
	border-radius: 5px;
	-webkit-box-shadow: 2px 2px 3px 0px rgba(50, 50, 50, 0.23);
	-moz-box-shadow: 2px 2px 3px 0px rgba(50, 50, 50, 0.23);
	box-shadow: 2px 2px 3px 0px rgba(50, 50, 50, 0.23);
}
#divContentModalShareUrl .close{
	cursor:pointer;
	float:right;
	font-size: 16px;
	color: #000;
	text-shadow: 0 1px 0 #fff;
	opacity: 0.4;
	border:none;
	background:none;
}
#divContentModalShareUrl .close:hover{opacity: 0.8;}
#divContentModalShareUrl .suHeader{margin: 5px 0 10px 0;}
#divContentModalShareUrl .suFooter
{text-align: right;margin-top: 10px;padding: 0 5px;}
#divContentModalShareUrl input[type=checkbox]{vertical-align:bottom;}
#divContentModalShareUrl table{width: 100%;padding: 0 15px;}
#divContentModalShareUrl input[type=text]{width:99%;}
#divContentModalShareUrl.hidden{
	display:none;
}
</style>


<telerik:RadButton ID="btnShareUrl" 
    runat="server" 
    Enabled="true" 
    AutoPostBack="false" 
    OnClientClicked="open_divContentModalShareUrl"
    Text="Share url">
    <Icon PrimaryIconUrl="images/share.png" PrimaryIconLeft="4" PrimaryIconTop="4"></Icon>
</telerik:RadButton>

<telerik:RadAjaxManagerProxy ID="RadAjaxMngShareUrl" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="CheckBoxShortUrl">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="txtShareUrl" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy >



<telerik:RadToolTip runat="server" ID="RadToolTip3" HideEvent="FromCode" Position="MiddleRight"
    Width="150px" 
    Height="70px" 
    Animation="Fade" 
    ShowEvent="OnClick" 
    ShowDelay="0"
    RelativeTo="Element" 
    TargetControlID="btnShareUrl">
    <div class="suHeader">
        <table>
            <tr>
                <td>
                    <asp:CheckBox ID="CheckBox1" runat="server" Text="Short URL" OnCheckedChanged="CheckBoxShortUrl_CheckedChanged"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="TextBox1" runat="server" Text="monUrl"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div class="suFooter">
        <telerik:RadButton ID="RadButton1" runat="server" Text="Copy to clipboard" AutoPostBack="false" OnClientClicked="ClipBoard" ></telerik:RadButton>
    </div>
</telerik:RadToolTip>




<telerik:RadScriptBlock runat="server">
	<script type="text/javascript">
	    function open_divContentModalShareUrl() {
	        var divContentModalShareUrl = $("#divContentModalShareUrl");
	        divContentModalShareUrl.removeClass('hidden');
	    }
	    function ClipBoard() {
	        var txtShareUrl = $("#txtShareUrl").html();
	        console.log(txtShareUrl);
	        return false;
	    }

	    $(document).ready(function () {
	        $('#divContentModalShareUrl').find('.close').click(function (e) {
	            e.preventDefault();
	            $(this).parent().addClass('hidden');
	        });
	    });
	</script>
</telerik:RadScriptBlock>
