<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login_Fail.aspx.cs" Inherits="Login_Fail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Fail</title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <table class="TableModify">
        <tr class="ModifyHead" id="PicList">
            <td colspan="2">
                登入失敗<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead" width="100px">
                Tips
            </td>
            <td class="TableModifyTd styleBlue">
                <ul style="list-style-type: decimal">
                    <li>確認是否已加入網域。</li>
                    <li>確認是否登入公司電腦。</li>
                    <li>確認帳號是否正確。</li>
                    <li>若上述方式皆無法解決，請洽....</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead" width="100px">
                錯誤訊息
            </td>
            <td class="TableModifyTd styleEarth B">
                <asp:Literal ID="lt_ErrMsg" runat="server"></asp:Literal>
            </td>
        </tr>
    </table>
    <div class="SubmitAreaS">
        <input type="button" onclick="location.href='main.aspx'" value="回首頁" />
    </div>
    </form>
</body>
</html>
