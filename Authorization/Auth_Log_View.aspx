<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_Log_View.aspx.cs" Inherits="Auth_SetGroup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
    <script type="text/javascript">
        $(function () {
            //樹狀選單
            $("#TreeView_Old").treeview({
                collapsed: false,
                animated: "medium",
                control: "#sidetreecontrol"
            });
            $("#TreeView_New").treeview({
                collapsed: false,
                animated: "medium",
                control: "#sidetreecontrol"
            });
        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                異動明細<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tbody>
            <tr class="Must">
                <td class="TableModifyTdHead" style="width: 10%">
                    異動帳戶
                </td>
                <td class="TableModifyTd styleGreen B" style="width: 40%">
                    <asp:Literal ID="lt_ProcAccount" runat="server"></asp:Literal>
                </td>
                <td class="TableModifyTdHead" style="width: 10%">
                    異動時間
                </td>
                <td class="TableModifyTd styleGraylight" style="width: 40%">
                    <asp:Literal ID="lt_ProcTime" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr class="Must">
                <td class="TableModifyTdHead">
                    異動動作
                </td>
                <td class="TableModifyTd styleEarth B">
                    <asp:Literal ID="lt_ProcAction" runat="server"></asp:Literal>
                </td>
                <td class="TableModifyTdHead">
                    異動處理者
                </td>
                <td class="TableModifyTd styleBlue">
                    <asp:Literal ID="lt_ProcWho" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="TableModifyTdHead B">
                    原權限
                </td>
                <td class="TableModifyTd" valign="top">
                    <div class="MenuSecond">
                        <asp:Literal ID="lt_OldAuthProg" runat="server"></asp:Literal>
                    </div>
                </td>
                <td class="TableModifyTdHead styleRed B">
                    新權限
                </td>
                <td class="TableModifyTd" valign="top">
                    <div class="MenuSecond">
                        <asp:Literal ID="lt_NewAuthProg" runat="server"></asp:Literal>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
    </form>
</body>
</html>
