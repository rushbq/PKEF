<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SupplierDelivery_tw.aspx.cs"
    Inherits="SupplierDelivery_tw" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_BgSupID").val("");
                $("#tb_EdSupID").val("");
                $("#tb_Keyword").val("");
                $("select#ddl_Employee")[0].selectedIndex = 0;
            });
        });

    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>生產採購</a>&gt;<span>供應商交期(台灣)</span>
    </div>
    <div class="h2Head">
        <h2>
            供應商交期(台灣)</h2>
    </div>
    <div class="Sift">
        <ul>
            <li>供應商代號：
                <asp:TextBox ID="tb_BgSupID" runat="server" MaxLength="10" Width="80px"></asp:TextBox>
                ~
                <asp:TextBox ID="tb_EdSupID" runat="server" MaxLength="10" Width="80px"></asp:TextBox>
            </li>
            <li>供應商簡稱：
                <asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" ToolTip="名稱關鍵字"></asp:TextBox>
            </li>
            <li>採購人員：
                <asp:DropDownList ID="ddl_Employee" runat="server">
                </asp:DropDownList>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" />
            </li>
            <li>
                <asp:LinkButton ID="lbtn_Export" runat="server" OnClick="lbtn_Export_Click"><img src="../images/System/ico_excel2.png" width="20" border="0" />匯出</asp:LinkButton></li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td width="250px">
                        供應商簡稱
                    </td>
                    <td width="150px">
                        供應商代號
                    </td>
                    <td>
                        一般標準交期
                    </td>
                    <td width="120px">
                        採購人員
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td align="center">
                    <%#Eval("MA002")%>
                </td>
                <td align="center">
                    <%#Eval("MA001")%>
                </td>
                <td align="left">
                    <%#Eval("SupDelDay")%>
                </td>
                <td align="center">
                    <%#Eval("MV002")%>
                </td>
            </tr>
        </ItemTemplate>
        <EmptyDataTemplate>
            <div style="padding: 120px 0px 120px 0px; text-align: center">
                <span style="color: #FD590B;">未新增或無任何符合資料！</span>
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
                <asp:Literal ID="lt_Page_DataCntInfo" runat="server" EnableViewState="False"></asp:Literal></div>
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
