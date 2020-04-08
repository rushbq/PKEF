<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WR_Class_Edit.aspx.cs" Inherits="WR_Class_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                類別維護<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <!-- 資料設定 Start -->
        <tbody>
            <tr class="Must">
                <td class="TableModifyTdHead" style="width: 100px">
                    <em>(*)</em> 部門
                </td>
                <td class="TableModifyTd styleBlue">
                    <asp:DropDownListGP ID="ddl_Dept" runat="server">
                    </asp:DropDownListGP>
                    <asp:RequiredFieldValidator ID="rfv_ddl_Dept" runat="server" ErrorMessage="-&gt; 請填入「部門」!"
                        ControlToValidate="ddl_Dept" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr class="Must">
                <td class="TableModifyTdHead">
                    <em>(*)</em> 類別名稱
                </td>
                <td class="TableModifyTd">
                    <asp:TextBox ID="tb_ClassName" runat="server" MaxLength="50" Width="250px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfv_tb_ClassName" runat="server" ErrorMessage="-&gt; 請填入「類別名稱」!"
                        ControlToValidate="tb_ClassName" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead">
                    是否顯示
                </td>
                <td class="TableModifyTd">
                    <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
                        <asp:ListItem Value="Y" Selected="True">是&nbsp;</asp:ListItem>
                        <asp:ListItem Value="N">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr class="Must">
                <td class="TableModifyTdHead">
                    <em>(*)</em>排序
                </td>
                <td class="TableModifyTd">
                    <asp:TextBox ID="tb_Sort" runat="server" Width="50px" MaxLength="3" Style="text-align: center;">999</asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="-&gt; 請輸入「排序」"
                        Display="Dynamic" ControlToValidate="tb_Sort" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="-&gt; 請輸入1 ~ 999 的數字"
                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                        ForeColor="Red" ValidationGroup="Add"></asp:RangeValidator>
                </td>
            </tr>
        </tbody>
        <!-- 資料設定 End -->
    </table>
    <div class="SubmitAreaS">
        <asp:Button ID="btn_Save" runat="server" Text="存檔" Width="90px" CssClass="btnBlock colorBlue"
            ValidationGroup="Add" OnClick="btn_Save_Click" />
        <input type="button" value="關閉視窗" onclick="parent.$.fancybox.close();" style="width: 90px"
            class="btnBlock colorGray" />
        <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
        <asp:HiddenField ID="hf_UID" runat="server" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
            ShowMessageBox="true" ValidationGroup="Add" />
    </div>
    <div style="height: 40px">
    </div>
    </form>
</body>
</html>
