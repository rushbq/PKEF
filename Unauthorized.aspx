<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Unauthorized.aspx.cs" Inherits="Unauthorized" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Unauthorized</title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <table class="TableModify">
        <tr class="ModifyHead" id="PicList">
            <td colspan="2">
                帳號未授權或無相關使用權限<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tr>
            <td class="TableModifyTdHead" width="100px">
                Tips
            </td>
            <td class="TableModifyTd styleBlue">
                <ul style="list-style-type: decimal">
                    <li>請確認所屬部門是否有使用權限。</li>
                    <li>若確認有使用權限，請嘗試關閉瀏覽器，重新進入網站。</li>
                    <li>若要新增權限，請洽 系統管理員</li>
                    <li>若上述方式皆無法解決，請洽 系統管理員</li>
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
        <input type="button" onclick="parent.location.href='default.aspx'" value="回首頁" />
    </div>
    </form>
</body>
</html>
