<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_Search.aspx.cs" Inherits="Auth_Search" %>

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
    <%-- treeview Start --%>
    <script src="../js/jquery.treeview/jquery.treeview.min.js" type="text/javascript"></script>
    <link href="../js/jquery.treeview/jquery.treeview.css" rel="stylesheet" type="text/css" />
    <%-- treeview End --%>
    <script type="text/javascript">
        $(function () {
            //樹狀選單
            $("#TreeView").treeview({
                collapsed: true,
                animated: "medium",
                control: "#sidetreecontrol"
            });

            $(".AuthSearch").click(function () {
                //顯示讀取中
                $("#Loading").show();
                //載入ajax
                UserSearch($(this).attr('dataId'));
            });
        });

        //功能使用人員查詢
        function UserSearch(dataId) {
            var showResult = $("div#Result");
            var imgLoading = $("#Loading");

            var request = $.ajax({
                url: 'Auth_Search_Ajax.aspx?' + new Date().getTime(),
                data: { dataId: dataId },
                type: "POST",
                dataType: "html"
            });

            request.done(function (response) {
                //載入回傳的Html
                showResult.html(response);

                imgLoading.hide();
            });

            request.fail(function (jqXHR, textStatus) {
                //alert("Request failed: " + textStatus);
                showResult.html("發生錯誤，請聯絡系統人員。");

                imgLoading.hide();
            });
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>權限管理</a>&gt;<span>權限查詢</span>
    </div>
    <div class="h2Head">
        <h2>
            權限查詢</h2>
    </div>
     <div class="SysTab">
        <ul>
            <li class="TabAc"><a href="Auth_Search.aspx" style="cursor: pointer;">by 權限表</a></li>
            <li><a href="Auth_SearchUser.aspx" style="cursor: pointer;">by 使用者</a></li>
        </ul>
    </div>
    <table class="TableModify">
        <tr class="ModifyHead">
            <td colspan="4">
                權限表<em class="TableModifyTitleIcon"></em>
            </td>
        </tr>
        <tbody id="dt1">
            <tr>
                <td class="TableModifyTd" style="width: 40%">
                    <div id="sidetreecontrol" class="MenuSecControl">
                        <a href="?#">收合</a> | <a href="?#">展開</a> &nbsp;<span class="SiftLight">(點選下方清單項目，查詢有哪些人員使用該功能)</span>
                    </div>
                    <hr class="MenuSecondHr" />
                    <div class="MenuSecond">
                        <asp:Literal ID="lt_AuthProg" runat="server"></asp:Literal>
                    </div>
                </td>
                <td class="TableModifyTd" style="width: 60%; vertical-align: top;">
                    <div id="Result">
                    </div>
                    <div id="Loading" style="padding: 50px 50px 50px 50px; display: none;">
                        <img src="../images/Load.gif" /></div>
                </td>
            </tr>
        </tbody>
    </table>
    </form>
</body>
</html>
