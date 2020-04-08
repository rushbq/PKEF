<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OP_HelpSearch.aspx.cs" Inherits="OP_HelpSearch" %>

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
            //$("input#clear_form").click(function () {
            //    $("#tb_StartDate").val("");
            //    $("#tb_EndDate").val("");
            //    $("#tb_Keyword").val("");
            //    $("select#ddl_Dept")[0].selectedIndex = 0;
            //    $("select#ddl_Req_Class")[0].selectedIndex = 0;
            //    $("select#ddl_Help_Status")[0].selectedIndex = 0;
            //});

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
                <%=Application["WebName"]%></a>&gt;<a>需求記錄表</a>&gt;<span>品號修改需求登記</span>
        </div>
        <div class="h2Head">
            <h2><span class="styleRed">品號修改</span>需求登記</h2>
        </div>

        <div class="panel panel-success">
            <div class="panel-body form-horizontal">
                <div class="row">
                    <div class="col-xs-12 col-sm-10 col-md-10 col-lg-10">
                        <div class="form-group form-inline">
                            <div class="col-sm-5 col-md-5 col-lg-5">
                                <label>登記日</label>
                                <asp:TextBox ID="tb_StartDate" runat="server" Style="text-align: center" Width="80px" CssClass="styleBlack"></asp:TextBox>&nbsp;
                                ~
                                <asp:TextBox ID="tb_EndDate" runat="server" Style="text-align: center" Width="80px" CssClass="styleBlack"></asp:TextBox>
                            </div>
                            <div class="col-sm-3 col-md-3 col-lg-3">
                                <label>狀態</label>
                                <asp:DropDownList ID="ddl_Help_Status" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-sm-4 col-md-4 col-lg-4 text-right">
                                <label>關鍵字</label>
                                <asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" placeholder="追蹤編號, 主旨, 需求者工號" CssClass="form-control" autocomplete="off"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group form-inline">
                            <div class="col-sm-5 col-md-5 col-lg-5">
                                <label>需求部門</label>
                                <asp:DropDownListGP ID="ddl_Dept" runat="server" CssClass="form-control">
                                </asp:DropDownListGP>
                            </div>
                            <div class="col-sm-5 col-md-5 col-lg-5">
                                <label>問題類別</label>
                                <asp:DropDownList ID="ddl_Req_Class" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-md-2 col-lg-2 text-right">
                                <button type="button" class="btn btn-default trigger-search"><span class="glyphicon glyphicon-search"></span>&nbsp;開始查詢</button>
                                <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />
                            </div>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-2 col-md-2 col-lg-2 text-center">
                        <p><a href="OP_HelpEdit.aspx" class="btn btn-warning">新增需求</a></p>
                        <p><asp:Button ID="btn_Export" runat="server" Text="匯出Excel" OnClick="btn_Export_Click" CssClass="btn btn-success" /></p>                      
                    </div>
                </div>
            </div>
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
                        <td valign="top">
                            <div>
                                <asp:Literal ID="lt_onTop" runat="server"></asp:Literal>
                                <a class="L2MainHead" href="OP_HelpView.aspx?TraceID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("TraceID").ToString(), DesKey))%>" title="追蹤編號">
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
                                    <a class="btn btn-primary btn-sm" href="OP_HelpEdit.aspx?TraceID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("TraceID").ToString(), DesKey))%>">
                                        <span class="glyphicon glyphicon-pencil"></span>&nbsp;<asp:Literal ID="lt_Edit" runat="server"></asp:Literal>
                                    </a>
                                    <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="btn btn-danger btn-sm"
                                        OnClientClick="return confirm('是否確定刪除!?')" Visible="false"><span class="glyphicon glyphicon-trash"></span>&nbsp;刪除</asp:LinkButton>

                                    <asp:HiddenField ID="hf_TraceID" runat="server" Value='<%# Eval("TraceID")%>' />
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_None" runat="server">
                                <div style="padding-top: 5px;">
                                    <a class="btn btn-default btn-sm" href="OP_HelpView.aspx?TraceID=<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("TraceID").ToString(), DesKey))%>">
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

    $(function () {
        $(".trigger-search").click(function () {
            //trigger click
            $("#btn_Search").trigger("click");
        });
    });

</script>
