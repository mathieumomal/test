<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RemarksControl.ascx.cs" Inherits="Etsi.Ultimate.Controls.RemarksControl" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<style type="text/css">
    #remarkControlContent .RadGrid_Default th.rgHeader
    {
        background-color: grey;
        border: none;
        border-bottom: 1px solid grey;
    }

    #remarkControlContent .RadGrid_Default .rgEditRow td
    {
        border: none;
    }

    #remarkControlContent .legendLabel{
        font-family: "Segoe UI",Arial,Helvetica,sans-serif;
        font-size: 14px;
        color: rgb(97, 97, 97);
    }

    #fieldSetMask.hide {
		opacity:0.3;
	}

    #remarkControlContent .loader {
		position:absolute;
		top: 75px;
        left: 315px;
    }
</style>
<script type="text/javascript">
    
    function startAjaxRequest() {
        $("#fieldSetMask").addClass('hide');
        $(".loader").css('display', 'block');
    }

    function SetAddRemarkState<%= this.ClientID %>() {
        if ($("#<%=txtAddRemark.ClientID %>").val().trim().length > 0)
            $("#<%=btnAddRemark.ClientID %>").removeAttr('disabled');
        else
            $("#<%=btnAddRemark.ClientID %>").attr('disabled', 'disabled');
    };


    function setAddingProgress<%= this.ClientID %>(flag) {
        if (flag) {
            startAjaxRequest();
            $("#<%=btnAddRemark.ClientID %>").val('Adding...');
        } else {
            $("#<%=btnAddRemark.ClientID %>").val('Add');
        }
    }

</script>
<div id="remarkControlContent">
     <img src="./images/loader.gif" alt="Loading..." class="loader" style="display: none;"/>
    <fieldset style="padding: 5px;" id="fieldSetMask">
        <legend>
            <asp:Label runat="server" ID="legendLabel" CssClass="legendLabel"></asp:Label>
        </legend>
        <table style="width: 100%">
            <tr>
                <td colspan="2">
                    <telerik:RadGrid runat="server" ID="remarksGrid" AllowPaging="false"
                        AllowSorting="false"
                        AllowFilteringByColumn="false"
                        AutoGenerateColumns="false"
                        AllowMultiRowEdit="true"
                        OnPreRender="remarksGrid_PreRender"
                        OnNeedDataSource="remarksGrid_NeedDataSource"
                        style="min-width: 400px">
                        <clientsettings>
                            <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                        </clientsettings>
                        <mastertableview clientdatakeynames="Pk_RemarkId, IsPublic, RemarkText, CreationDate" editmode="InPlace">
                            <Columns>
                                <telerik:GridTemplateColumn DataField="CreationDate" HeaderText="Creation date" UniqueName="CreationDate">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px"/> 
                                    <ItemTemplate>
                                        <span><%# DataBinder.Eval(Container.DataItem,"CreationDate", "{0:yyyy-MM-dd HH:mm UTC}") %></span>  
                                    </ItemTemplate>                 
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="PersonName" HeaderText="Author" UniqueName="CreatedBy">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="150px"/> 
                                    <ItemTemplate>
                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"PersonName") %></div>  
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="RemarkText" HeaderText="Remark" UniqueName="RemarkText">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True"/>
                                    <ItemTemplate>
                                        <div class="text-left"><%# DataBinder.Eval(Container.DataItem,"RemarkText") %></div>  
                                    </ItemTemplate>                
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn DataField="IsPublic" HeaderText="Remark Type" UniqueName="IsPublic">
                                    <HeaderStyle HorizontalAlign="Center" Font-Bold="True" Width="100px"/>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    <ItemTemplate>  
                                        <asp:Label runat="server" ID="lblRemarkType" Text='<%# Convert.ToBoolean(Eval("IsPublic")) == true ? "Public" : "Private" %>'>></asp:Label>                  
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadDropDownList runat="server" ID="rddlRemarkType" 
                                            Width="90px" 
                                            DataValueField="IsPublic" 
                                            AutoPostBack="true" 
                                            OnSelectedIndexChanged="rddlRemarkType_SelectedIndexChanged" 
                                            SelectedValue='<%#Bind("IsPublic") %>' 
                                            OnClientItemSelected="startAjaxRequest">
                                            <Items>
                                                <telerik:DropDownListItem Text="Public" Value="True"/>  
                                                <telerik:DropDownListItem Text="Private" Value="False"/>                                  
                                                </Items>
                                            </telerik:RadDropDownList>                          
                                    </EditItemTemplate>                   
                                </telerik:GridTemplateColumn>
                            </Columns>
                            <NoRecordsTemplate>
                                <div style="text-align:center">
                                    No Remarks Added
                                </div>
                            </NoRecordsTemplate>
                        </mastertableview>
                    </telerik:RadGrid></td>
            </tr>
            <tr>
                <td style="padding-right: 5px">
                    <asp:TextBox ID="txtAddRemark" runat="server" Width="100%" AutoComplete="off" />
                </td>
                <td>
                    <asp:Button ID="btnAddRemark" runat="server" Text="Add" Width="100%" OnClick="btnAddRemark_Click"/>
                </td>
            </tr>
        </table>
    </fieldset>
</div>
