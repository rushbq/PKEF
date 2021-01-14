<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cust_Search.aspx.cs" Inherits="Cust_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<%@ Import Namespace="ExtensionUI" %>
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
                $("select#ddl_ShipFrom")[0].selectedIndex = 0;
                $("select#ddl_Year")[0].selectedIndex = 0;
                $("#tb_Keyword").val("");
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
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a>
                <%=Application["WebName"]%></a>&gt;<a>目標設定</a>&gt;<span>客戶目標設定</span>
        </div>
        <div class="h2Head">
            <h2>客戶目標設定</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>年份：
                <asp:DropDownList ID="ddl_Year" runat="server" CssClass="styleBlack">
                </asp:DropDownList>
                </li>
                <li>出貨地：<asp:DropDownList ID="ddl_ShipFrom" runat="server" CssClass="styleBlack">
                </asp:DropDownList>
                </li>
                <li>客戶關鍵字：
                <asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="180px" ToolTip="客戶代號/簡稱/全稱" CssClass="styleBlack"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                    <asp:PlaceHolder ID="ph_Edit" runat="server">|
                    <asp:Button ID="btn_New" runat="server" Text="新增資料" CssClass="btnBlock colorBlue" OnClick="btn_New_Click" />
                    </asp:PlaceHolder>
                </li>
            </ul>
        </div>
        <div class="table-responsive">
            <div class="SysTab3">
                <%=fn_CustomUI.GetTabHtml(Param_Type,"Cust_Search.aspx") %>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                <LayoutTemplate>
                    <table class="List1" width="100%">
                        <tr class="tdHead">
                            <td width="180px" rowspan="3">客戶
                            </td>
                            <td width="80px" rowspan="3">年份
                            </td>
                            <td colspan="6">金額
                            </td>
                            <td width="80px" rowspan="3">功能選項
                            </td>
                        </tr>
                        <tr class="tdHead">
                            <td colspan="3">銷售
                            </td>
                            <td colspan="3">接單
                            </td>
                        </tr>
                        <tr class="tdHead">
                            <td>USD
                            </td>
                            <td>NTD
                            </td>
                            <td>RMB
                            </td>
                            <td>USD
                            </td>
                            <td>NTD
                            </td>
                            <td>RMB
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server" align="center">
                        <td align="left">
                            <div class="L2MainHead">
                                <%# Eval("CustName")%>
                            </div>
                            <div class="styleBluelight">
                                <%# Eval("CustID")%>
                            </div>
                            <div style="padding-top: 3px;">
                                <span class="styleGraylight">出貨地:</span><span class="styleCafe B">
                                    <%#Eval("SName")%></span>
                            </div>
                        </td>
                        <td class="JQ-ui-state-default">
                            <a href="Cust_ViewAll.aspx?t=<%#Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&TitleName=<%# Server.UrlEncode(Eval("CustName").ToString())%>"
                                class="styleGreen B infoBox" title="展開 - 年度目標">
                                <%#Eval("SetYear")%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <%-- 銷售 --%>
                        <td class="JQ-ui-state-default">
                            <a href="Cust_ViewItem.aspx?t=<%#Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&Column=Amount_USD&SetTitle=<%# Server.UrlEncode("銷售金額(USD)") %>&TitleName=<%# Server.UrlEncode(Eval("CustName").ToString())%>"
                                class="styleBlue infoBox" title="展開 - 銷售金額(USD)">
                                <%#fn_stringFormat.C_format(Eval("Amount_USD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td class="JQ-ui-state-default">
                            <a href="Cust_ViewItem.aspx?t=<%#Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&Column=Amount_NTD&SetTitle=<%# Server.UrlEncode("銷售金額(NTD)") %>&TitleName=<%# Server.UrlEncode(Eval("CustName").ToString())%>"
                                class="styleBlue infoBox" title="展開 - 銷售金額(NTD)">
                                <%#fn_stringFormat.C_format(Eval("Amount_NTD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td class="JQ-ui-state-default">
                            <a href="Cust_ViewItem.aspx?t=<%#Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&Column=Amount_RMB&SetTitle=<%# Server.UrlEncode("銷售金額(RMB)") %>&TitleName=<%# Server.UrlEncode(Eval("CustName").ToString())%>"
                                class="styleBlue infoBox" title="展開 - 銷售金額(RMB)">
                                <%#fn_stringFormat.C_format(Eval("Amount_RMB").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <%-- 接單 --%>
                        <td class="JQ-ui-state-default">
                            <a href="Cust_ViewItem.aspx?t=<%#Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&Column=OrdAmount_USD&SetTitle=<%# Server.UrlEncode("接單金額(USD)") %>&TitleName=<%# Server.UrlEncode(Eval("CustName").ToString())%>"
                                class="infoBox styleGreen" title="展開 - 接單金額(USD)">
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_USD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td class="JQ-ui-state-default">
                            <a href="Cust_ViewItem.aspx?t=<%#Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&Column=OrdAmount_NTD&SetTitle=<%# Server.UrlEncode("接單金額(NTD)") %>&TitleName=<%# Server.UrlEncode(Eval("CustName").ToString())%>"
                                class="infoBox styleGreen" title="展開 - 接單金額(NTD)">
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_NTD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td class="JQ-ui-state-default">
                            <a href="Cust_ViewItem.aspx?t=<%#Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&Column=OrdAmount_RMB&SetTitle=<%# Server.UrlEncode("接單金額(RMB)") %>&TitleName=<%# Server.UrlEncode(Eval("CustName").ToString())%>"
                                class="infoBox styleGreen" title="展開 - 接單金額(RMB)">
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_RMB").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <p>
                                    <a class="btn btn-primary btn-sm" href="Cust_Edit.aspx?t=<%=Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&CustID=<%# Server.UrlEncode(Eval("CustID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&ShowDT=Y">
                                        <span class="glyphicon glyphicon-pencil"></span>&nbsp;設定
                                    </a>
                                </p>
                                <p>
                                    <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Del" CssClass="btn btn-warning btn-sm"
                                        OnClientClick="return confirm('是否確定刪除!?')"><span class="glyphicon glyphicon-trash"></span>&nbsp;刪除</asp:LinkButton>
                                </p>
                                <asp:HiddenField ID="hf_SetYear" runat="server" Value='<%# Eval("SetYear")%>' />
                                <asp:HiddenField ID="hf_ShipFrom" runat="server" Value='<%# Eval("ShipFrom")%>' />
                                <asp:HiddenField ID="hf_CustID" runat="server" Value='<%# Eval("CustID")%>' />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_block" runat="server">- </asp:PlaceHolder>
                            <asp:HiddenField ID="hf_IsRel" runat="server" Value='<%# Eval("IsRel")%>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div style="padding: 120px 0px 120px 0px; text-align: center">
                        <span style="color: #FD590B;">未新增或無任何符合資料！</span>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
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
