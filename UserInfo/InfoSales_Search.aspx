<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoSales_Search.aspx.cs"
    Inherits="InfoSales_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
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
        $(document).ready(function () {
            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("select#ddl_Dept")[0].selectedIndex = 0;
                $("#tb_Keyword").val('');
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
                <%=Application["WebName"]%></a>&gt;<a>基本資料維護</a>&gt;<span>資料維護</span>
        </div>
        <div class="h2Head">
            <h2>資料維護</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>部門：
                <asp:DropDownListGP ID="ddl_Dept" runat="server">
                </asp:DropDownListGP>
                </li>
                <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="200px" placeholder="查詢工號, 人名, NickName" autocomplete="off"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                </li>
            </ul>
        </div>
        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
            <LayoutTemplate>
                <table class="List1" width="100%">
                    <tr class="tdHead">
                        <td width="60px">區域
                        </td>
                        <td width="100px">部門
                        </td>
                        <td width="340px">人員</td>
                        <td width="80px">分機
                        </td>
                        <td>Email</td>
                        <td width="120px">功能選項
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="trItem" runat="server">
                    <td align="center" class="styleGreen Font13">
                        <%#Eval("SName")%>
                    </td>
                    <td align="center" class="Font12">
                        <%#Eval("DeptName")%>
                    </td>
                    <td>(<span><%# Eval("Account_Name")%></span>)
                        <a href="InfoSales_View.aspx?UserID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("Account_Name").ToString()))%>"
                            class="L2MainHead infoBox" title="人員明細: <%# Eval("Display_Name")%>">
                            <%# Eval("Display_Name")%>&nbsp;<span class="Font12">(<%# Eval("NickName")%>)</span>
                            <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                    </td>
                    <td align="center" class="styleBlue B Font16">
                        <%# Eval("Tel_Ext")%>
                    </td>
                    <td class="Font12">
                        <%# Eval("Email")%>
                    </td>
                    <td align="center">
                        <a class="BtnFour infoBox" href="InfoSales_Corp_Edit.aspx?UserID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("Account_Name").ToString()))%>">設定公司別</a><br />
                        <a class="BtnFour infoBox" href="InfoSales_Edit.aspx?UserID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("Account_Name").ToString()))%>">修改資料</a>
                    </td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div style="padding: 120px 0px 120px 0px; text-align: center">
                    <span style="color: #FD590B; font-size: 12px">未新增或無任何符合資料！</span>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>
        <asp:Panel ID="pl_Page" runat="server" CssClass="PagesArea" Visible="false">
            <div class="PageControlCon">
                <div class="PageControl">
                    <asp:Literal ID="lt_Page_Link" runat="server" EnableViewState="False"></asp:Literal>
                    <span class="PageSet">轉頁至
                    <asp:DropDownList ID="ddl_Page_List" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_Page_List_SelectedIndexChanged">
                    </asp:DropDownList>
                        /
                    <asp:Literal ID="lt_TotalPage" runat="server" EnableViewState="False"></asp:Literal>
                        頁</span>
                </div>
                <div class="PageAccount">
                    <asp:Literal ID="lt_Page_DataCntInfo" runat="server" EnableViewState="False"></asp:Literal>
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
<script language="javascript" type="text/javascript">
    function EnterClick(e) {
        // 這一行讓 ie 的判斷方式和 Firefox 一樣。
        if (window.event) { e = event; e.which = e.keyCode; } else if (!e.which) e.which = e.keyCode;

        if (e.which == 13) {
            // Submit按鈕
            __doPostBack('btn_Search', '');
            return false;
        }
    }

    document.onkeypress = EnterClick;
</script>
