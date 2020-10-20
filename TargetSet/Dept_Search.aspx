<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dept_Search.aspx.cs" Inherits="Dept_Search" %>

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
                $("select#ddl_Dept")[0].selectedIndex = 0;
                $("select#ddl_Year")[0].selectedIndex = 0;
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
                <%=Application["WebName"]%></a>&gt;<a>目標設定</a>&gt;<span>部門目標設定</span>
        </div>
        <div class="h2Head">
            <h2>部門目標設定</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>年份：
                    <asp:DropDownList ID="ddl_Year" runat="server" CssClass="styleBlack">
                    </asp:DropDownList>
                </li>
                <li>區域：
                    <asp:DropDownList ID="ddl_ShipFrom" runat="server" CssClass="styleBlack">
                    </asp:DropDownList>
                </li>
                <li>部門：
                    <asp:DropDownListGP ID="ddl_Dept" runat="server" CssClass="styleBlack">
                    </asp:DropDownListGP>
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
                <%=fn_CustomUI.GetTabHtml(Param_Type,"Dept_Search.aspx") %>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand">
                <LayoutTemplate>
                    <table class="List1" width="100%">
                        <tr class="tdHead">
                            <td width="100px" rowspan="3">部門</td>
                            <td width="80px" rowspan="3">年份</td>
                            <td colspan="9">金額</td>
                            <td width="80px" rowspan="3">功能選項</td>
                        </tr>
                        <tr class="tdHead">
                            <td colspan="3">銷售</td>
                            <td colspan="3">接單</td>
                            <td colspan="3">挑戰</td>
                        </tr>
                        <tr class="tdHead">
                            <td>USD</td>
                            <td>NTD</td>
                            <td>RMB</td>
                            <td>USD</td>
                            <td>NTD</td>
                            <td>RMB</td>
                            <td>USD</td>
                            <td>NTD</td>
                            <td>RMB</td>
                        </tr>
                        <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server">
                        <td align="center">
                            <div class="L2MainHead">
                                <%#Eval("DeptName")%>
                            </div>
                            <div>
                                <span class="styleGraylight">區域:</span><span class="styleCafe B">
                                    <%#Eval("SName")%></span>
                            </div>
                        </td>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="Dept_ViewAll.aspx?ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&DeptID=<%# Server.UrlEncode(Eval("DeptID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>&DeptName=<%# Server.UrlEncode(Eval("DeptName").ToString())%>"
                                class="styleGreen B infoBox" title="展開 - 年度目標">
                                <%#Eval("SetYear")%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <%-- 銷售 --%>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"Amount_USD", "銷售金額(USD)"
                                    , Eval("DeptName").ToString()})%>"
                                class="styleBlue infoBox" title="展開 - 銷售金額(USD)">
                                <%#fn_stringFormat.C_format(Eval("Amount_USD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"Amount_NTD", "銷售金額(NTD)"
                                    , Eval("DeptName").ToString()})%>"
                                class="styleBlue infoBox" title="展開 - 銷售金額(NTD)">
                                <%#fn_stringFormat.C_format(Eval("Amount_NTD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"Amount_RMB", "銷售金額(RMB)"
                                    , Eval("DeptName").ToString()})%>"
                                class="styleBlue infoBox" title="展開 - 銷售金額(RMB)">
                                <%#fn_stringFormat.C_format(Eval("Amount_RMB").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <%-- 接單 --%>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"OrdAmount_USD", "接單金額(USD)"
                                    , Eval("DeptName").ToString()})%>"
                                class="infoBox styleGreen" title="展開 - 接單金額(USD)">
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_USD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"OrdAmount_NTD", "接單金額(NTD)"
                                    , Eval("DeptName").ToString()})%>"
                                class="infoBox styleGreen" title="展開 - 接單金額(NTD)">
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_NTD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"OrdAmount_RMB", "接單金額(RMB)"
                                    , Eval("DeptName").ToString()})%>"
                                class="infoBox styleGreen" title="展開 - 接單金額(RMB)">
                                <%#fn_stringFormat.C_format(Eval("OrdAmount_RMB").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <%-- 挑戰 --%>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"ChAmount_USD", "挑戰金額(USD)"
                                    , Eval("DeptName").ToString()})%>"
                                class="styleReddark infoBox" title="展開 - 挑戰金額(USD)">
                                <%#fn_stringFormat.C_format(Eval("ChAmount_USD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"ChAmount_NTD", "挑戰金額(NTD)"
                                    , Eval("DeptName").ToString()})%>"
                                class="styleReddark infoBox" title="展開 - 挑戰金額(NTD)">
                                <%#fn_stringFormat.C_format(Eval("ChAmount_NTD").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center" class="JQ-ui-state-default">
                            <a href="<%# myUrl_ViewItem(new string[]{
                                    Eval("ShipFrom").ToString()
                                    , Eval("DeptID").ToString()
                                    , Eval("SetYear").ToString()
                                    ,"ChAmount_RMB", "挑戰金額(RMB)"
                                    , Eval("DeptName").ToString()})%>"
                                class="styleReddark infoBox" title="展開 - 挑戰金額(RMB)">
                                <%#fn_stringFormat.C_format(Eval("ChAmount_RMB").ToString())%>
                                <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <p>
                                    <a class="btn btn-primary btn-sm" href="Dept_Edit.aspx?t=<%=Param_Type %>&ShipFrom=<%# Server.UrlEncode(Eval("ShipFrom").ToString())%>&DeptID=<%# Server.UrlEncode(Eval("DeptID").ToString())%>&SetYear=<%# Server.UrlEncode(Eval("SetYear").ToString())%>">
                                        <span class="glyphicon glyphicon-pencil"></span>&nbsp;設定
                                    </a>
                                </p>
                                <p>
                                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="btn btn-warning btn-sm"
                                        OnClientClick="return confirm('是否確定刪除!?')"><span class="glyphicon glyphicon-trash"></span>&nbsp;刪除</asp:LinkButton>
                                </p>
                                <asp:HiddenField ID="hf_SetYear" runat="server" Value='<%# Eval("SetYear")%>' />
                                <asp:HiddenField ID="hf_ShipFrom" runat="server" Value='<%# Eval("ShipFrom")%>' />
                                <asp:HiddenField ID="hf_DeptID" runat="server" Value='<%# Eval("DeptID")%>' />
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_block" runat="server">- </asp:PlaceHolder>
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
