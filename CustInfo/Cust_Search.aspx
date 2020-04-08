<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cust_Search.aspx.cs" Inherits="Cust_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_Keyword").val("");
                $("select#ddl_Area")[0].selectedIndex = 0;
                $("select#ddl_Country")[0].selectedIndex = 0;
            });

        });

    </script>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a>
                <%=Application["WebName"]%></a>&gt;<a>基本資料維護</a>&gt;<span>客戶基本資料</span>
        </div>
        <div class="h2Head">
            <h2>客戶基本資料</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>地區別：
                    <asp:DropDownList ID="ddl_Area" runat="server" CssClass="styleBlack"></asp:DropDownList>
                </li>
                <li>國家別：
                    <asp:DropDownList ID="ddl_Country" runat="server" CssClass="styleBlack"></asp:DropDownList>
                </li>
                <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="180px" CssClass="styleBlack"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                </li>
            </ul>
        </div>
        <div class="table-responsive">
            <div class="SysTab3">
                <%=Html_Tabs() %>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <thead>
                            <tr class="tdHead">
                                <td>客戶資料</td>
                                <td width="180px">負責業務</td>
                                <td width="100px">主要資料庫</td>
                                <td width="200px">功能選項
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="trItem" runat="server">
                        <td valign="top">
                            <div>
                                <label class="label label-danger"><%#Eval("CustID") %></label>
                                <label class="label label-default"><%#Eval("AreaName").ToString() %></label>
                                <label class="label label-default"><%#Eval("CountryName").ToString() %></label>
                                <label class="label label-warning"><%#string.IsNullOrEmpty(Eval("mySWID").ToString())?"未設定出貨庫別":"" %></label>
                            </div>
                            <div style="padding-top: 10px;">
                                <%# Eval("CustName").ToString()%>
                            </div>
                        </td>
                        <td align="center">
                            <%#Eval("RepSalesID") %><br />
                            <%#Eval("RepSales") %>
                        </td>
                        <td align="center">
                            <%#Eval("Corp_Name") %>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <a class="btn btn-primary btn-sm" href="Cust_Edit.aspx?DataID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("CustID").ToString(), DesKey))%>&SID=<%#mySID %>">
                                    <span class="glyphicon glyphicon-pencil"></span>&nbsp;編輯
                                </a>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_View" runat="server">
                                <a class="btn btn-default btn-sm" href="Cust_View.aspx?DataID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("CustID").ToString(), DesKey))%>&SID=<%#mySID %>">
                                    <span class="glyphicon glyphicon-info-sign"></span>&nbsp;查看明細
                                </a>
                            </asp:PlaceHolder>
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
