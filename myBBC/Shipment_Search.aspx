<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Shipment_Search.aspx.cs" Inherits="Shipment_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="../css/font-awesome.min.css" rel="stylesheet" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_StartDate").val("");
                $("#tb_EndDate").val("");
                $("#tb_Keyword").val("");
                $("select#ddl_ShipClass")[0].selectedIndex = 0;
            });

            /* 日期選擇器 */
            $("#tb_StartDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $("#tb_EndDate").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#tb_EndDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $("#tb_StartDate").datepicker("option", "maxDate", selectedDate);
                }
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
                <%=Application["WebName"]%></a>&gt;<a>BBC平台</a>&gt;<span>出貨單-內銷</span>
        </div>
        <div class="h2Head">
            <h2>出貨單-內銷</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>下單日期：
                    <asp:TextBox ID="tb_StartDate" runat="server" Style="text-align: center" Width="90px" CssClass="styleBlack"></asp:TextBox>&nbsp;
                    ~
                    <asp:TextBox ID="tb_EndDate" runat="server" Style="text-align: center" Width="90px" CssClass="styleBlack"></asp:TextBox>
                </li>
            </ul>
            <ul>
                <li>狀態：
                    <asp:DropDownList ID="ddl_ShipClass" runat="server" CssClass="styleBlack">
                        <asp:ListItem Value="">-- 所有資料 --</asp:ListItem>
                        <asp:ListItem Value="0">待處理</asp:ListItem>
                        <asp:ListItem Value="1">出貨中</asp:ListItem>
                        <asp:ListItem Value="2">已出貨</asp:ListItem>
                    </asp:DropDownList>
                </li>
                <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="180px" CssClass="styleBlack" ToolTip="自訂單號、網路單號、客戶編號、客戶名稱"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                </li>
            </ul>
        </div>
        <div class="table-responsive">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <thead>
                            <tr class="tdHead">
                                <td width="120px">狀態</td>
                                <td width="150px">網路/自訂單號
                                </td>
                                <td width="240px">客戶資料
                                </td>
                                <td>收貨資料
                                </td>
                                <td width="120px">下單日
                                </td>
                                <td width="180px">功能選項
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
                        <td align="center">
                            <div style="padding-top: 5px">
                                <asp:Label ID="lb_Status" runat="server"></asp:Label>
                            </div>
                            <div style="padding-top: 10px">
                                <asp:Label ID="lb_ShipType" runat="server" CssClass="label label-danger"></asp:Label>
                            </div>
                        </td>
                        <td align="center">
                            <%#Eval("OrderID") %>
                            <div style="padding-top: 8px; font-style: italic;">
                                <%# Eval("MyOrderID").ToString()%>
                            </div>
                        </td>
                        <td valign="top">
                            <div>
                                <span class="L2MainHead">
                                    <%#Eval("CustID").ToString() %>
                                </span>
                            </div>
                            <div style="padding-top: 8px;">
                                <%# Eval("CustName").ToString()%>
                            </div>
                        </td>
                        <td>
                            <label class="label label-default"><%#Eval("ShipWho").ToString() %></label>
                            <label class="label label-success"><%#Eval("ShipNo").ToString() %></label>
                            <div style="padding-top: 8px;">
                                <%# Eval("ShippingAddr").ToString()%>
                            </div>
                        </td>
                        <td align="center">
                            <%#Eval("Create_Time").ToString().ToDateString("yyyy/MM/dd<br>HH:mm") %>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <a class="btn btn-primary btn-sm" href="Shipment_Edit.aspx?DataID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("OrderID").ToString(), DesKey))%>">
                                    <i class="fa fa-pencil"></i>&nbsp;出貨
                                </a>
                                <asp:LinkButton ID="lbtn_Cancel" runat="server" CssClass="btn btn-warning btn-sm" CommandName="Del" OnClientClick="return confirm('是否確定取消訂單?')"><i class="fa fa-ban" aria-hidden="true"></i>&nbsp;取消訂單</asp:LinkButton>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_View" runat="server">
                                <a class="btn btn-default btn-sm" href="Shipment_View.aspx?DataID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("OrderID").ToString(), DesKey))%>">
                                    <i class="fa fa-info-circle"></i>&nbsp;查看
                                </a>
                            </asp:PlaceHolder>
                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("OrderID") %>' />
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
