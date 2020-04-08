<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%></title>
    <link href="css/System.css" rel="stylesheet" type="text/css" />
</head>
<body class="SysLogin">
    <form id="form1" runat="server">
    <div class="SysLoginPageCon">
        <div class="SysLoginContent">
            <div class="Info">
                <h2>
                    <%=Application["WEB_NAME"] %></h2>
                <hr />
                <div class="InfoItemCon">
                    <div class="InfoItem">
                        <div class="InfoLable">
                            帳號：</div>
                        <div class="InfoItemEdit">
                            <asp:TextBox ID="tb_UserID" runat="server" MaxLength="5"></asp:TextBox>
                            <br />
                            <span style="display: none" class="InfoItemWarm">*備註文字格式</span>
                            <asp:RequiredFieldValidator ID="rfv_tb_UserID" runat="server" ErrorMessage="請輸入「帳號」!"
                                Display="None" ControlToValidate="tb_UserID"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="InfoItem">
                        <div class="InfoLable">
                            密碼：</div>
                        <div class="InfoItemEdit">
                            <asp:TextBox ID="tb_UserPwd" runat="server" TextMode="Password" MaxLength="50"></asp:TextBox>
                            <br />
                            <asp:RequiredFieldValidator ID="rfv_tb_UserPwd" runat="server" ErrorMessage="請輸入「密碼」!"
                                Display="None" ControlToValidate="tb_UserPwd"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="InfoItem">
                        <div class="InfoLable">
                            請輸入下方驗證碼：</div>
                        <div class="InfoItemEdit" style="text-align: center">
                            <asp:Image ID="img_Verify" runat="server" ImageUrl="Create_ValidImg.aspx" AlternateText="驗證碼"
                                ImageAlign="Middle" />
                        </div>
                        <div class="InfoItemEdit" style="text-align: center">
                            <asp:TextBox ID="tb_VerifyCode" runat="server" Width="80px" MaxLength="5" Style="text-align: center;"></asp:TextBox>
                            <input type="button" onclick="document.getElementById('img_Verify').src='Create_ValidImg.aspx?' + Math.random();"
                                value="R" />
                            <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" ErrorMessage="請輸入「驗證碼」!"
                                Display="None" ControlToValidate="tb_VerifyCode"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="LoginBtnArea">
                        <asp:LinkButton ID="lbtn_Login" runat="server" CssClass="Login" OnClick="lbtn_Login_Click">登入</asp:LinkButton>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true"
                            ShowSummary="false" />
                    </div>
                </div>
            </div>
            <div class="CalendarArea">
                <embed width="131" height="186" name="plugin" src="images/calendar.swf" type="application/x-shockwave-flash"></embed>
            </div>
        </div>
        <div class="Announce">
            <p>
                1. 請輸入網域帳號/密碼 (即電腦登入帳密)。</p>
            <p>
                2. 帳號登入後4小時會自動失效，系統會自動取得目前電腦登入帳戶。</p>
            <p>
                3. <a href="<%=Application["WebUrl"] %>Default.aspx">按此可回首頁並自動登入。</a></p>
            <asp:Literal ID="lt_ErrMsg" runat="server"></asp:Literal>
        </div>
    </div>
    </form>
</body>
</html>
<script language="javascript" type="text/javascript">
    function EnterClick(e) {
        // 這一行讓 ie 的判斷方式和 Firefox 一樣。
        if (window.event) { e = event; e.which = e.keyCode; } else if (!e.which) e.which = e.keyCode;

        if (e.which == 13) {
            __doPostBack('lbtn_Login', '');
            return false;
        }
    }

    document.onkeypress = EnterClick;
</script>
