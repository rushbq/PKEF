<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Sales_RelCust_Search.aspx.cs"
    Inherits="Sales_RelCust_Search" %>

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
                $("#tb_SalesWord").val('');
                $("#tb_CustWord").val('');
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
            <%=Application["WebName"]%></a>&gt;<a>目標設定</a>&gt;<span>客戶關聯設定</span>
    </div>
    <div class="h2Head">
        <h2>
            客戶關聯設定 <small>(只會顯示業務單位)</small></h2>
    </div>
    <div class="Sift">
        <ul>
            <li>部門：
                <asp:DropDownListGP ID="ddl_Dept" runat="server">
                </asp:DropDownListGP>
            </li>
            <li>人員關鍵字：<asp:TextBox ID="tb_SalesWord" runat="server" MaxLength="50" Width="120px"
                ToolTip="ERP員工代號/名稱"></asp:TextBox>
            </li>
            <li>客戶關鍵字：
                <asp:TextBox ID="tb_CustWord" runat="server" MaxLength="50" Width="120px" ToolTip="客戶代號/簡稱/全稱"></asp:TextBox></li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td width="100px">
                        地區
                    </td>
                    <td>
                        部門
                    </td>
                    <td width="120px">
                        ERP員工代號
                    </td>
                    <td>
                        人員名稱
                    </td>
                    <td>
                        已關聯客戶
                    </td>
                    <td width="140px" style="display:none;">
                        功能選項
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td align="center">
                    <%#Eval("Area")%>
                </td>
                <td align="center">
                    <%#Eval("DeptName")%>
                </td>
                <td align="center" class="styleBlue">
                    <%#Eval("ERP_UserID")%>
                </td>
                <td align="center" class="L2MainHead">
                    <%#Eval("Display_Name")%>
                </td>
                <td align="center" class="JQ-ui-state-default">
                    <a href="Sales_RelCust_View.aspx?StaffID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("StaffID").ToString()))%>&Display_Name=<%# Server.UrlEncode(Eval("Display_Name").ToString())%>"
                        class="styleGreen infoBox" title="展開關聯客戶">共 <span class="styleBlue" id="Cnt_<%# Eval("StaffID").ToString()%>">
                            <%#Eval("DataCnt")%></span> 家 <span class="JQ-ui-icon ui-icon-newwin"></span>
                    </a>
                </td>
                <td align="center" style="display:none;">
                    <asp:PlaceHolder ID="ph_Edit" runat="server" Visible="false">
                        <a class="Edit infoBox" href="Sales_RelCust_Edit.aspx?StaffID=<%# Server.UrlEncode(Cryptograph.Encrypt(Eval("StaffID").ToString()))%>">
                        設定</a>
                        <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="Delete"
                            OnClientClick="return confirm('是否確定移除所有關聯客戶!?')">移除</asp:LinkButton>
                        <asp:HiddenField ID="hf_StaffID" runat="server" Value='<%# Eval("StaffID")%>' />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_block" runat="server">-</asp:PlaceHolder>
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
