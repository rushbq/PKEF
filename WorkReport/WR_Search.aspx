<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WR_Search.aspx.cs" Inherits="WR_Search" %>

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
                //$("#tb_StartDate").val("");
                //$("#tb_EndDate").val("");
                $("select#ddl_IsDone")[0].selectedIndex = 0;
                $("#tb_Keyword").val("");
                if ($("select#ddl_Class").length) $("select#ddl_Class")[0].selectedIndex = 0;
                if ($("select#ddl_Dept").length) $("select#ddl_Dept")[0].selectedIndex = 0;
                if ($("select#ddl_Employee").length) $("select#ddl_Employee")[0].selectedIndex = 0;
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
    <%-- highslide Start --%>
    <script type="text/javascript" src="../js/highslide/highslide-with-html.packed.js"></script>
    <link rel="stylesheet" type="text/css" href="../js/highslide/highslide.css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $(".showMore").click(function () {
                var thisHead = $(this).attr("head");
                hs.graphicsDir = '../js/highslide/graphics/';
                hs.outlineType = 'rounded-white';
                hs.showCredits = false;
                hs.wrapperClassName = 'draggable-header';
                hs.htmlExpand(this, {
                    headingText: thisHead
                });
            });

        });     
    </script>
    <%-- highslide End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>工作日誌</a>&gt;<span>日誌查詢</span>
    </div>
    <div class="h2Head">
        <h2>
            日誌查詢</h2>
    </div>
    <div class="Sift">
        <asp:PlaceHolder ID="ph_Menu" runat="server">
            <ul>
                <li>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            部門：
                            <asp:DropDownListGP ID="ddl_Dept" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Dept_SelectedIndexChanged">
                            </asp:DropDownListGP>
                            人員：
                            <asp:DropDownListGP ID="ddl_Employee" runat="server">
                            </asp:DropDownListGP>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </li>
            </ul>
        </asp:PlaceHolder>
        <ul>
            <li>日期區間：
                <asp:TextBox ID="tb_StartDate" runat="server" Style="text-align: center" Width="80px"></asp:TextBox>&nbsp;
                ~
                <asp:TextBox ID="tb_EndDate" runat="server" Style="text-align: center" Width="80px"></asp:TextBox>
            </li>
            <li>狀態：
                <asp:DropDownListGP ID="ddl_IsDone" runat="server">
                    <asp:ListItem Value="">-- 所有資料 --</asp:ListItem>
                    <asp:ListItem Value="N">未完成</asp:ListItem>
                    <asp:ListItem Value="Y">已完成</asp:ListItem>
                </asp:DropDownListGP>
            </li>
        </ul>
        <ul>
            <asp:PlaceHolder ID="ph_ClassMenu" runat="server" Visible="false">
                <li>類別：<asp:DropDownList ID="ddl_Class" runat="server">
                </asp:DropDownList>
                </li>
            </asp:PlaceHolder>
            <li>項目關鍵字：
                <asp:TextBox ID="tb_Keyword" runat="server" MaxLength="50" Width="150px"></asp:TextBox>
            </li>
            <li class="styleBlack">依
                <asp:Label ID="lb_SortName" runat="server" CssClass="styleEarth B"></asp:Label>
                排序,
                <asp:DropDownListGP ID="ddl_Sortby" runat="server">
                    <asp:ListItem Value="ASC" Selected="True">由小至大</asp:ListItem>
                    <asp:ListItem Value="DESC">由大至小</asp:ListItem>
                </asp:DropDownListGP>
            </li>
            <li>
                <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" CssClass="btnBlock colorGray" />
                <input type="button" id="clear_form" value="清除" title="清除目前搜尋條件" class="btnBlock colorGray" />
                |<input type="button" class="btnBlock colorGreen" onclick="location.href='WR_Summary.aspx';"
                    value="返回總表" />
            </li>
        </ul>
    </div>
    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
        <LayoutTemplate>
            <table class="List1" width="100%">
                <tr class="tdHead">
                    <td width="120px" class="JQ-ui-state-error">
                        類別<asp:LinkButton ID="lbtn_sClass" runat="server" OnClick="btn_sClass"><span class="JQ-ui-icon ui-icon-circle-triangle-s"></span></asp:LinkButton>
                    </td>
                    <td>
                        工作項目
                    </td>
                    <td width="140px" class="JQ-ui-state-error">
                        建立時間<asp:LinkButton ID="lbtn_sCreateTime" runat="server" OnClick="btn_sCreateTime"><span class="JQ-ui-icon ui-icon-circle-triangle-s"></span></asp:LinkButton>
                    </td>
                    <td width="140px" class="JQ-ui-state-error">
                        完成時間<asp:LinkButton ID="lbtn_sCompTime" runat="server" OnClick="btn_sCompTime"><span class="JQ-ui-icon ui-icon-circle-triangle-s"></span></asp:LinkButton>
                    </td>
                    <td width="120px" class="JQ-ui-state-error">
                        建立者<asp:LinkButton ID="lbtn_sCreateWho" runat="server" OnClick="btn_sCreateWho"><span class="JQ-ui-icon ui-icon-circle-triangle-s"></span></asp:LinkButton>
                    </td>
                </tr>
                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="trItem" runat="server">
                <td align="center" class="styleBlue">
                    <%#Eval("Class_Name")%>
                </td>
                <td align="left" class="JQ-ui-state-default">
                    <a class="showMore L2MainHead" style="cursor: pointer" title="展開" head="<%#Eval("Task_Name")%>">
                        <%#Eval("Task_Name")%>
                        <span class="JQ-ui-icon ui-icon-newwin"></span></a>
                    <div class="highslide-maincontent">
                        <%# Eval("Remark").ToString().Replace("\r","<br/>") %>
                        <div style="text-align: right; padding-top: 5px;">
                            <input type="button" value="關閉" onclick="return hs.close(this);" class="btnBlock colorGray" /></div>
                    </div>
                </td>
                <td align="center">
                    <%#Eval("Create_Time").ToString().ToDateString("yyyy-MM-dd HH:mm")%>
                </td>
                <td align="center">
                    <%#Eval("Complete_Time").ToString().ToDateString("yyyy-MM-dd HH:mm")%>
                </td>
                <td align="center">
                    <%#Eval("Display_Name")%>
                </td>
            </tr>
        </ItemTemplate>
        <EmptyDataTemplate>
            <div style="padding: 120px 0px 120px 0px; text-align: center">
                <span style="color: #FD590B; font-size: 12px">查無任何符合資料！</span>
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
