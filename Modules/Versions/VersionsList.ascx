<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VersionsList.ascx.cs" Inherits="Etsi.Ultimate.Module.Versions.View" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:ImageButton ID="btnConfigLatestFTPPath"
    runat="server"
    ImageAlign="Top"
    AlternateText="Configure Latest FTP Path"
    ImageUrl="/DesktopModules/Versions/images/ftp.png"
    OnClientClick="openFTPConfiguration(); return false;" />

<telerik:RadAjaxManager ID="ramVersions" runat="server" EnablePageHeadUpdate="false">
</telerik:RadAjaxManager>
<telerik:RadWindowManager ID="rwmVersions" runat="server">
    <Windows>
        <telerik:RadWindow ID="rwFTPConfiguration" runat="server" Modal="true" Behaviors="Close" Title="FTP Configuration" Height="225" Width="400" VisibleStatusbar="false" IconUrl="false">
            <ContentTemplate>
                <div id="divFTPConfiguration" style="padding: 5px">
                    <div>
                        <table>
                            <tr>
                                <td>Latest Versions Path</td>
                                <td><asp:Label ID="lblLatestVersionsPath" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>Current Mapping</td>
                                <td><asp:Label ID="lblCurrentPath" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>New Mapping</td>
                                <td><asp:Label ID="lblNewPath" runat="server"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>Select New Path</td>
                                <td><telerik:RadComboBox ID="rcbConfigFTP" runat="server" OnClientSelectedIndexChanged="OnClientSelectedIndexChanged" /></td>
                            </tr>
                        </table>
                    </div>
                    <br />
                    <div class="footer" style="text-align: right">
                        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" OnClick="btnConfirm_Click"/>
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="closePopUpWindow();" />
                    </div>
                </div>
            </ContentTemplate>
        </telerik:RadWindow>
    </Windows>
</telerik:RadWindowManager>

<telerik:RadCodeBlock ID="rcbVersions" runat="server">
    <script type="text/javascript">

        function OnClientSelectedIndexChanged(sender, args) {
            $('#<%= lblNewPath.ClientID %>').text(args.get_item().get_value());
        }

        function openFTPConfiguration() {
            var radWindowFTPConfiguration = $find("<%= rwFTPConfiguration.ClientID %>");
            radWindowFTPConfiguration.show();
        };

        function closePopUpWindow() {
            var radWindowFTPConfiguration = $find("<%= rwFTPConfiguration.ClientID %>");
            radWindowFTPConfiguration.close();
        }

    </script>
</telerik:RadCodeBlock>
