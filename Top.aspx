<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Top.aspx.cs" Inherits="Top" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.7.2.min.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function hideMenu() {
            parent.document.getElementById('DownFrame').cols = '0,*';
        }

        function ShowMenu() {
            parent.document.getElementById('DownFrame').cols = '176,*';
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="Header">
        <div class="SystemLogo">
            <a href="<%=Application["WebUrl"] %>" target="_top">
                <%=Application["WebName"]%></a></div>
        <!--快速選單-->
        <div class="TopFastLink">
            <ul>
                <li class="Fast1"><a href="http://productcenter.prokits.com.tw/" target="_blank">產品中心</a></li>
                <li class="Fast2"><a href="http://report.prokits.com.tw/" target="_blank">報表中心</a></li>
                <li class="Fast3"><a href="http://pk711.prokits.com.tw/" target="_blank">專案服務區</a></li>
                <li>&nbsp;</li>
            </ul>
        </div>
        <div class="SysInfoControlCon">
            <div class="MenuFirstControl">
                <a class="Menu12Hide" href="javascript:hideMenu();" title="點此按鈕可隱藏左側選單">隱藏選單</a>
                <a class="Menu12Show" href="javascript:ShowMenu();" title="點此按鈕可展開左側選單">展開選單</a>
            </div>
          <%--  <div  class="SystemFastNews">
                <span class="font13_green">Tip：請妥善保存您的帳號 & 密碼。</span> <span class="font13_green">Tip：您可以使用的功能將由系統管理員設定。</span></div>--%>
            <div class="FastPageBack">
                <a href="Main.aspx" target="mainFrame">回首頁</a>&nbsp;|&nbsp;<asp:LinkButton ID="lbtn_Logout"
                    runat="server" CausesValidation="false" CssClass="B" OnClick="lbtn_Logout_Click"
                    ToolTip="執行登出，可使用其他帳號登入">登出</asp:LinkButton>&nbsp;|&nbsp;<%=fn_Params.UserAccount %> (<%=fn_Params.UserAccountName %>)
            </div>
        </div>
    </div>
    </form>
</body>
</html>
