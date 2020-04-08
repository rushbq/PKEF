<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ToyMsg_Search.aspx.cs" Inherits="ToyMsg_Search" %>

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
                $("#tb_StartDate").val("");
                $("#tb_EndDate").val("");
                $("#tb_Keyword").val("");
                $("select#ddl_Req_Class")[0].selectedIndex = 0;
                $("select#ddl_Status")[0].selectedIndex = 0;
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
                <%=Application["WebName"]%></a>&gt;<a>業務行銷</a>&gt;<span>科學玩具網站訊息</span>
        </div>
        <div class="h2Head">
            <h2>科學玩具網站訊息</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>查詢建立日：
                    <asp:TextBox ID="tb_StartDate" runat="server" Style="text-align: center" Width="90px" CssClass="styleBlack"></asp:TextBox>&nbsp;
                    ~
                    <asp:TextBox ID="tb_EndDate" runat="server" Style="text-align: center" Width="90px" CssClass="styleBlack"></asp:TextBox>
                </li>
                <li>訊息類別：
                    <asp:DropDownList ID="ddl_Req_Class" runat="server" CssClass="styleBlack"></asp:DropDownList>
                </li>
            </ul>
            <ul>
                <li>處理狀態：
                    <asp:DropDownList ID="ddl_Status" runat="server" CssClass="styleBlack"></asp:DropDownList>
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
                <ul>
                    <li <%if (Param_Type.Equals("1"))
                          { %>class="TabAc"
                        <%} %>><a href="ToyMsg_Search.aspx?t=1">一般訊息</a></li>
                    <li <%if (Param_Type.Equals("2"))
                          { %>class="TabAc"
                        <%} %>><a href="ToyMsg_Search.aspx?t=2">作廢信件</a></li>
                </ul>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand"
                OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <thead>
                            <tr class="tdHead">
                                <td width="200px">類別/狀態</td>
                                <td>內容
                                </td>
                                <td width="120px">回覆人
                                </td>
                                <td width="220px">時間
                                </td>
                                <td width="120px">功能選項
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
                            <div style="padding: 10px 0px 10px 0px">
                                <label class="label label-danger"><%#Eval("Class_Name") %></label>
                                <asp:Label ID="lb_Status" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td valign="top">
                            <div>
                                <span class="L2MainHead" title="追蹤編號">
                                    <%#Eval("TraceID").ToString() %>
                                </span>
                            </div>
                            <div style="padding-top: 10px;">
                                <%# Eval("ClientMsg").ToString()%> ...
                            </div>
                        </td>
                        <td align="center">
                            <%#Eval("Reply_Name") %>
                        </td>
                        <td align="center">
                            <table class="TableS1">
                                <tr>
                                    <td class="TableS1TdHead" style="width: 70px;">建立日
                                    </td>
                                    <td style="width: 140px;">
                                        <%# Eval("Create_Time").ToString().ToDateString("yyyy-MM-dd HH:mm")%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TableS1TdHead">回覆日
                                    </td>
                                    <td>
                                        <%# Eval("Reply_Time").ToString().ToDateString("yyyy-MM-dd HH:mm")%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <a class="btn btn-primary btn-sm" href="ToyMsg_Edit.aspx?DataID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("InquiryID").ToString(), DesKey))%>">
                                    <span class="glyphicon glyphicon-pencil"></span>&nbsp;訊息回覆
                                </a>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_View" runat="server">
                                <a class="btn btn-default btn-sm" href="ToyMsg_View.aspx?DataID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("InquiryID").ToString(), DesKey))%>" style="margin-top:3px;">
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
