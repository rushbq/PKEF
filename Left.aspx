<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Left.aspx.cs" Inherits="Left" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>左側選單</title>
    <script src="js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
    <base target="mainFrame" />
</head>
<body class="MenuFirstPage">
    <form id="form1" runat="server">
    <!-- MenuFirst  Start -->
    <div class="MenuFirst">
        <ul>
            <asp:Literal ID="lt_Menu" runat="server"></asp:Literal>
            <li class="End"></li>
        </ul>
    </div>
    <!-- MenuFirst  End -->
    </form>
</body>
</html>
<script language="javascript" type="text/javascript">
    //主選單
    function fmenu(Target_Id, Open, currClass) {
        //移除所有的class=>Menu1Active
        $('li[id*="li_up_"]').removeClass('Menu1Active');
        //子選單元素
        var SubMenu = $("#li_SubMenu_" + Target_Id);
        //判斷是否展開
        if ((SubMenu.is(":hidden")) || (Open == 'Y')) {
            SubMenu.show();
            $("#li_up_" + Target_Id).addClass('Menu1Active');
        } else {
            SubMenu.hide();
        }
    }
    //子選單
    function SubClick(Target_Id) {
        $("li.AcFirst").removeAttr('class');
        if (Target_Id != '') {
            $("#li_" + Target_Id).addClass('AcFirst');
        }
    }
        
</script>
