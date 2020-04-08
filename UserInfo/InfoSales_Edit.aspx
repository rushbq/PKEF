<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoSales_Edit.aspx.cs" Inherits="InfoSales_Edit" %>

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
                <td colspan="4">資料維護<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <!-- 資料設定 Start -->
            <tbody>
                <tr class="Must">
                    <td class="TableModifyTdHead" style="width: 120px">人員
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_UserID" runat="server" CssClass="styleBluelight"></asp:Label>
                        , 
                        <asp:Label ID="lb_UserName" runat="server" CssClass="styleRed B"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">E-Mail
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_Email" runat="server" CssClass="styleCafe"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> NickName
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_NickName" runat="server" MaxLength="50" Width="150px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfv_tb_NickName" runat="server" ErrorMessage="-&gt; 請填入「NickName」!"
                            ControlToValidate="tb_NickName" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> ERP登入代號
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_ERP_LoginID" runat="server" MaxLength="10" Width="150px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfv_tb_ERP_LoginID" runat="server" ErrorMessage="-&gt; 請填入「ERP登入代號」!"
                            ControlToValidate="tb_ERP_LoginID" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="rev_tb_ERP_LoginID" runat="server" ErrorMessage="-&gt; 請填入1~10碼英文或數字"
                            ControlToValidate="tb_ERP_LoginID" Display="Dynamic" ValidationExpression="[0-9A-Za-z\s-_.]{1,10}" ForeColor="Red" ValidationGroup="Add"></asp:RegularExpressionValidator>

                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> ERP員工代號
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_ERP_UserID" runat="server" MaxLength="10" Width="150px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfv_tb_ERP_UserID" runat="server" ErrorMessage="-&gt; 請填入「ERP員工代號」!"
                            ControlToValidate="tb_ERP_UserID" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator ID="rev_tb_ERP_UserID" runat="server" ErrorMessage="-&gt; 請填入1~10碼英文或數字"
                            ControlToValidate="tb_ERP_UserID" Display="Dynamic" ValidationExpression="[0-9A-Za-z\s-_.]{1,10}" ForeColor="Red" ValidationGroup="Add"></asp:RegularExpressionValidator>

                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">電話
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_Tel" runat="server" MaxLength="20" Width="150px"></asp:TextBox>&nbsp;
                        分機:
                        <asp:TextBox ID="tb_TelExt" runat="server" MaxLength="5" Width="80px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">行動電話
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_Mobile" runat="server" MaxLength="20" Width="150px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">Skype
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_IM_Skype" runat="server" MaxLength="100" Width="300px"></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <td class="TableModifyTdHead">Line
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_IM_Line" runat="server" MaxLength="100" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">QQ
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_IM_QQ" runat="server" MaxLength="100" Width="300px"></asp:TextBox>
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
