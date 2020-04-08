<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustDB_Edit.aspx.cs" Inherits="CustDB_Edit" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
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
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <%-- blockUI End --%>
    <script type="text/javascript" language="javascript">
        $(function () {
            //使用jQueryUI 將radio Group
            //$(".showRadioGrp").buttonset();

            /* Autocomplete - 客戶 */
            $("#tb_CustName").autocomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_Customer.aspx?f=custDB",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: "(" + item.custid + ") " + item.shortName,
                                        value: item.shortName,
                                        custid: item.custid,
                                        dbc: item.myDB,
                                        dbcName: item.myDBName
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    $("#tb_CustID").val(ui.item.custid);
                    $("#lb_CustID").text("(" + ui.item.custid + ")");

                    if (ui.item.dbc == "InProcess" || ui.item.dbc == "") {
                        $("#lb_CurrDB").text("-- 主要資料庫空白, 請儘快完成設定 --");
                    }
                    else {
                        $("#lb_CurrDB").text(ui.item.dbcName);
                    }
                }
            });
        });

        //返回列表
        function goToList() {
            location.href = '<%=Session["BackListUrl"] %>';
        }

    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>基本資料維護</a>&gt;<span>客戶主要資料庫</span>
        </div>
        <div class="h2Head">
            <h2>尚未設定主要資料庫的客戶</h2>
        </div>
        <table class="TableModify">
            <!-- 主檔 Start -->
            <tr class="ModifyHead">
                <td colspan="4">基本資料<em class="TableModifyTitleIcon"></em>
                </td>
            </tr>
            <tbody>
                <tr>
                    <td class="TableModifyTdHead" style="width: 150px">客戶
                    </td>
                    <td class="TableModifyTd">
                        <asp:TextBox ID="tb_CustName" runat="server" MaxLength="50" ToolTip="輸入客戶關鍵字" Width="300px"></asp:TextBox>
                        <asp:TextBox ID="tb_CustID" runat="server" Style="display: none"></asp:TextBox>
                        <asp:Label ID="lb_CustID" runat="server" CssClass="styleEarth"></asp:Label>
                        <asp:RequiredFieldValidator ID="rfv_tb_CustID" runat="server" ErrorMessage="-&gt; 請輸入正確的「客戶」"
                            ControlToValidate="tb_CustID" Display="Dynamic" ForeColor="Red" ValidationGroup="Add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead">目前主要資料庫
                    </td>
                    <td class="TableModifyTd">
                        <asp:Label ID="lb_CurrDB" runat="server" CssClass="styleBlue" Text="-- 請先選擇客戶 --"></asp:Label>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 變更主要資料庫
                    </td>
                    <td class="TableModifyTd">
                        <asp:RadioButtonList ID="rbl_NewDB" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" CellPadding="5"></asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="rfv_rbl_NewDB" runat="server" ErrorMessage="請選擇「主要資料庫」" ControlToValidate="rbl_NewDB" Display="Dynamic" ValidationGroup="Add" ForeColor="Red"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr class="Must">
                    <td class="TableModifyTdHead">
                        <em>(*)</em> 報價資料庫
                    </td>
                    <td class="TableModifyTd">
                        <asp:CheckBoxList ID="cbl_PriceDB" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" class="SubmitAreaS">
                        <asp:Button ID="btn_Save" runat="server" Text="儲存設定" OnClick="btn_Save_Click"
                            CssClass="btnBlock colorBlue" ValidationGroup="Add" />

                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                            ShowMessageBox="true" ValidationGroup="Add" />
                    </td>
                </tr>
                <tr>
                    <td class="TableModifyTdHead styleRed" valign="top">未設定的客戶清單
                    </td>
                    <td class="TableModifyTd">
                        <div>
                            <asp:ListView ID="lvDataList" runat="server" GroupPlaceholderID="ph_Group" ItemPlaceholderID="ph_Items"
                                GroupItemCount="4">
                                <LayoutTemplate>
                                    <table class="List1" width="100%">
                                        <asp:PlaceHolder ID="ph_Group" runat="server" />
                                    </table>
                                </LayoutTemplate>
                                <GroupTemplate>
                                    <tr>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tr>
                                </GroupTemplate>
                                <ItemTemplate>
                                    <td align="left" width="25%">
                                        <a href="<%=PageUrl %>?CustID=<%#Eval("custid") %>" title="前往設定">(<%#Eval("custid") %>) <%#Eval("shortName") %></a>
                                    </td>
                                </ItemTemplate>
                                <EmptyItemTemplate>
                                    <td></td>
                                </EmptyItemTemplate>
                            </asp:ListView>
                        </div>
                    </td>
                </tr>
            </tbody>
            <!-- 主檔 End -->
        </table>
    </form>
</body>
</html>
