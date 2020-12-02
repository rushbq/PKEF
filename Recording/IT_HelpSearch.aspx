<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IT_HelpSearch.aspx.cs" Inherits="IT_HelpSearch" %>

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
                $("select#ddl_Dept")[0].selectedIndex = 0;
                $("select#ddl_Req_Class")[0].selectedIndex = 0;
                $("select#ddl_Help_Status")[0].selectedIndex = 0;
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
                <%=Application["WebName"]%></a>&gt;<a>需求記錄表</a>&gt;<span>資訊需求登記</span>
        </div>
        <div class="h2Head">
            <h2>資訊需求登記</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>查詢登記日：
                    <asp:TextBox ID="tb_StartDate" runat="server" Style="text-align: center" Width="80px" CssClass="styleBlack"></asp:TextBox>&nbsp;
                    ~
                    <asp:TextBox ID="tb_EndDate" runat="server" Style="text-align: center" Width="80px" CssClass="styleBlack"></asp:TextBox>
                </li>
                <li>需求部門：
                    <asp:DropDownListGP ID="ddl_Dept" runat="server" CssClass="styleBlack">
                    </asp:DropDownListGP>
                </li>
                <li>問題類別：
                    <asp:DropDownList ID="ddl_Req_Class" runat="server" CssClass="styleBlack"></asp:DropDownList>
                </li>
            </ul>
            <ul>
                <li>處理狀態：
                    <asp:DropDownList ID="ddl_Help_Status" runat="server" CssClass="styleBlack"></asp:DropDownList>
                </li>
                <li>關鍵字：<asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="180px" placeholder="追蹤編號, 主旨, 需求者工號" CssClass="styleBlack"></asp:TextBox>
                </li>
                <li>
                    <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                    <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                    &nbsp;|&nbsp;
                    <input type="button" value="新增需求登記" onclick="location.href = 'IT_HelpEdit.aspx';" class="btnBlock colorBlue" />
                    &nbsp;<asp:Button ID="btn_Export" runat="server" Text="匯出Excel" OnClick="btn_Export_Click" CssClass="btnBlock colorGreen" />
                </li>
            </ul>
        </div>
        <div class="table-responsive">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand"
                OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <table class="List1 table" width="100%">
                        <thead>
                            <tr class="tdHead">
                                <td width="120px">類別
                                </td>
                                <td>內容
                                </td>
                                <td width="80px">狀態
                                </td>
                                <td width="120px">需求者
                                </td>
                                <td width="200px">時間
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
                        <td align="center" class="styleGreen">
                            <%#Eval("HClass") %>
                        </td>
                        <td valign="top" style="word-break:keep-all">
                            <div>
                                <asp:Literal ID="lt_onTop" runat="server"></asp:Literal>
                                <a class="L2MainHead" href="IT_HelpView.aspx?TraceID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("TraceID").ToString(), DesKey))%>" title="追蹤編號">
                                    <%#Eval("TraceID").ToString().Insert(2,"-").Insert(11,"-") %>
                                </a>
                            </div>
                            <div class="L2Info styleGraylight" style="padding-top: 5px;">
                                <%# fn_stringFormat.StringLimitOutput(Eval("Help_Subject").ToString(), 30, fn_stringFormat.WordType.Bytes, true)%>
                            </div>
                        </td>
                        <td align="center">
                            <div style="margin: 20px;">
                                <asp:Label ID="lb_Status" runat="server"></asp:Label>
                            </div>
                        </td>
                        <td align="center">
                            <%#Eval("Account_Name") %><br />
                            <%#Eval("Display_Name") %>
                        </td>
                        <td align="center">
                            <table class="TableS1">
                                <tr>
                                    <td class="TableS1TdHead" style="width: 50px;">登記日
                                    </td>
                                    <td style="width: 130px;">
                                        <%# Eval("Create_Time").ToString().ToDateString("yyyy-MM-dd")%>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TableS1TdHead">處理日
                                    </td>
                                    <td>
                                        <%# Eval("Reply_Date").ToString().ToDateString("yyyy-MM-dd")%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder ID="ph_Edit" runat="server">
                                <div>
                                    <a class="btn btn-primary btn-sm" href="IT_HelpEdit.aspx?TraceID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("TraceID").ToString(), DesKey))%>">
                                        <span class="glyphicon glyphicon-pencil"></span>&nbsp;<asp:Literal ID="lt_Edit" runat="server"></asp:Literal>
                                    </a>
                                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="btn btn-danger btn-sm"
                                        OnClientClick="return confirm('是否確定刪除!?')" Visible="false"><span class="glyphicon glyphicon-trash"></span>&nbsp;刪除</asp:LinkButton>

                                    <asp:HiddenField ID="hf_TraceID" runat="server" Value='<%# Eval("TraceID")%>' />
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_None" runat="server">
                                <div style="padding-top: 5px;">
                                    <a class="btn btn-default btn-sm" href="IT_HelpView.aspx?TraceID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("TraceID").ToString(), DesKey))%>">
                                        <span class="glyphicon glyphicon-info-sign"></span>&nbsp;查看明細
                                    </a>
                                </div>
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
