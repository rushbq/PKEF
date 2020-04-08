<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WR_Class_Search.aspx.cs"
    Inherits="WR_Class_Search" %>

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
            <%=Application["WebName"]%></a>&gt;<a>工作日誌</a>&gt;<span>類別維護</span>
    </div>
    <div class="h2Head">
        <h2>
            類別維護</h2>
    </div>
    <div class="Sift">
        <ul>
            <li>部門：
                <asp:DropDownListGP ID="ddl_Dept" runat="server">
                </asp:DropDownListGP>
            </li>
            <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="120px"></asp:TextBox>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                |
                <input type="button" value="新增類別" href="WR_Class_Edit.aspx" class="btnBlock colorBlue infoBox" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td width="180px">
                        部門
                    </td>
                    <td>
                        類別名稱
                    </td>
                    <td width="80px">
                        是否顯示
                    </td>
                    <td width="80px">
                        排序
                    </td>
                    <td width="140px">
                        功能選項
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td align="center" class="styleBlue">
                    <%#Eval("DeptName")%>
                </td>
                <td align="center" class="L2MainHead">
                    <%#Eval("Class_Name")%>
                </td>
                <td align="center">
                    <%#Eval("Display")%>
                </td>
                <td align="center">
                    <%#Eval("Sort")%>
                </td>
                <td align="center">
                    <asp:PlaceHolder ID="ph_Edit" runat="server"><a class="Edit infoBox" href="WR_Class_Edit.aspx?Class_ID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("Class_ID").ToString()))%>">
                        修改</a>
                        <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                            OnClientClick="return confirm('是否確定刪除!?')">刪除</asp:LinkButton>
                        <asp:HiddenField ID="hf_Class_ID" runat="server" Value='<%# Eval("Class_ID")%>' />
                    </asp:PlaceHolder>
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
