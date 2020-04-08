<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Auth_Sync.aspx.cs" Inherits="Auth_Sync" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            /* DatePicker */
            $("#tb_BgDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd'
            });
            $("#tb_EdDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd'
            });

            //Click事件 - 清除搜尋條件
            $("input#clear_form").click(function () {
                $("#tb_BgDate").val("");
                $("#tb_EdDate").val("");
                $("#tb_Keyword").val("");
            });

            //Click事件 - 同步按鈕
            $("#btn_Sync").click(function () {
                if (confirm('是否確定開始同步!?') == false) {
                    return false;
                }
                else {
                    //Disabled 按鈕
                    $(this).attr("disabled", "disabled");
                    //執行同步
                    ADSync();
                }
            });
        });

        //Ajax - 同步帳號
        function ADSync() {
            var showResult = $("span#SyncResult");   //顯示執行結果
            var btn_Sync = $("#btn_Sync");

            var request = $.ajax({
                url: 'Auth_Sync_Ajax.aspx?' + new Date().getTime(),
                data: { ValidCode: '<%=ValidCode %>' },
                type: "POST",
                dataType: "html"
            });

            request.done(function (response) {
                //載入回傳的Html
                showResult.html(response);
                //將同步按鈕解除鎖定
                btn_Sync.removeAttr("disabled");
            });

            request.fail(function (jqXHR, textStatus) {
                //alert("Request failed: " + textStatus);
                showResult.html("發生錯誤，請聯絡系統人員。");
                //將同步按鈕解除鎖定
                btn_Sync.removeAttr("disabled");
            });
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>權限管理</a>&gt;<span>帳號同步</span>
    </div>
    <div class="h2Head">
        <h2>
            同步記錄</h2>
    </div>
    <!-- Basic Sift -->
    <div class="Sift">
        <asp:PlaceHolder ID="ph_Sync" runat="server">
            <ul>
                <%--<li>設定自動同步：從
                    <asp:DropDownList ID="ddl_Hour" runat="server">
                    </asp:DropDownList>
                    :
                    <asp:DropDownList ID="ddl_Min" runat="server">
                    </asp:DropDownList>
                    起，每隔
                    <asp:TextBox ID="tb_TimePeriod" runat="server" Width="40px" Style="text-align: center;"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="rev_tb_TimePeriod" runat="server" ErrorMessage="-&gt; 請輸入數字"
                        ControlToValidate="tb_TimePeriod" Display="Dynamic" ValidationExpression="^[0-9]+\.{0,1}[0-9]{0,2}$"
                        ForeColor="Red"></asp:RegularExpressionValidator>
                    分設定同步 &raquo;
                    <asp:Button ID="btn_SaveSync" runat="server" Text="儲存設定" CausesValidation="false"
                        OnClick="btn_SaveSync_Click" CssClass="btnBlock colorRed" />
                </li>--%>
                <li>
                    <input id="btn_Sync" type="button" value="立即同步AD帳戶" title="立即同步所有群組&使用者" class="btnBlock colorBlue" />
                    <span id="SyncResult" class="styleGraylight"></span></li>
            </ul>
        </asp:PlaceHolder>
        <!--條件篩選區-->
        <ul>
            <li>起訖日期：
                <asp:TextBox ID="tb_BgDate" runat="server" Style="text-align: center" Width="80px"></asp:TextBox>
                <asp:RegularExpressionValidator ID="rev_tb_BgDate" runat="server" ErrorMessage="-&gt; 「開始日期」格式錯誤"
                    ControlToValidate="tb_BgDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                    Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                ~
                <asp:TextBox ID="tb_EdDate" runat="server" Style="text-align: center" Width="80px"></asp:TextBox>
                <asp:RegularExpressionValidator ID="rev_tb_EdDate" runat="server" ErrorMessage="-&gt; 「結束日期」格式錯誤"
                    ControlToValidate="tb_EdDate" ValidationExpression="(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])"
                    Display="Dynamic" ForeColor="Red"></asp:RegularExpressionValidator>
                <asp:CompareValidator ID="cv_Date" runat="server" ErrorMessage="-&gt; 「結束日期」必須大於「開始日期」"
                    Type="Date" Operator="LessThanEqual" ControlToValidate="tb_BgDate" ControlToCompare="tb_EdDate"
                    Display="Dynamic" ForeColor="Red"></asp:CompareValidator>
            </li>
            <li>關鍵字：
                <asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="180px" ToolTip="AD帳戶/處理描述"></asp:TextBox>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead" style="white-space: nowrap;">
                    <td width="140px">
                        處理時間
                    </td>
                    <td width="80px">
                        類別
                    </td>
                    <td width="80px">
                        處理動作
                    </td>
                    <td width="140px">
                        AD帳戶
                    </td>
                    <td>
                        處理描述
                    </td>
                    <td width="120px">
                        處理者
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td align="center">
                    <%# String.Format("{0:yyyy-MM-dd HH:mm}", Eval("Proc_Time"))%>
                </td>
                <td align="center" class="styleBlue">
                    <asp:Literal ID="lt_Proc_Type" runat="server"></asp:Literal>
                </td>
                <td align="center" class="styleGreen">
                    <%#Eval("Proc_Action")%>
                </td>
                <td align="center">
                    <%#Eval("Proc_Account")%>
                </td>
                <td align="left" class="styleGraylight">
                    <%#Eval("Proc_Desc")%>
                </td>
                <td align="center">
                    <%#Eval("Create_Name")%>
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
