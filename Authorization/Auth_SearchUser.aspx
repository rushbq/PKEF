<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_SearchUser.aspx.cs"
    Inherits="Auth_SearchUser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <%-- fancybox End --%>
    <script type="text/javascript">
        $(function () {
            //資料展開/收合
            $(".DTtoggle").click(function () {
                //取得此物件的rel屬性值 (#xx)
                var _this = $(this).attr("rel");
                var _img = $(this).attr("imgrel");
                //判斷指定元素是否隱藏
                if ($(_this).css("display") == "none") {
                    $(_this).show();
                    $(_img).attr("src", "../images/icon_top.png").attr("title", "收合");
                } else {
                    $(_this).hide();
                    $(_img).attr("src", "../images/icon_down.png").attr("title", "展開");
                }
                return false;
            });

            //fancybox - 檢視
            $(".infoBox").fancybox({
                type: 'iframe',
                fitToView: true,
                autoSize: true,
                closeClick: false,
                openEffect: 'elastic', // 'elastic', 'fade' or 'none'
                closeEffect: 'none'
            });
        });
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
            <li><a href="Auth_Search.aspx" style="cursor: pointer;">by 權限表</a></li>
            <li class="TabAc"><a href="Auth_SearchUser.aspx" style="cursor: pointer;">by 使用者</a></li>
        </ul>
    </div>
    <div>
        <table class="TableModify">
            <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
        </table>
    </div>
    </form>
</body>
</html>
