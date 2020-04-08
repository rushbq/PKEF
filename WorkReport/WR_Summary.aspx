<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WR_Summary.aspx.cs" Inherits="WR_Summary" %>

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
            /* 日期選擇器 */
            $("#myCalandar").datepicker({
                dateFormat: 'yy/mm/dd',
                defaultDate: '<%=this.tb_ShowDT.Text %>',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $("#tb_ShowDT").val(selectedDate);
                    $("#btn_Search").trigger("click");
                }
            });

        });

    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
    <div class="Navi">
        <a>
            <%=Application["WebName"]%></a>&gt;<a>工作日誌</a>&gt;<span>日誌總表</span>
    </div>
    <div class="h2Head">
        <h2>
            日誌總表 (預設前一日)</h2>
    </div>
    <div>
        <table width="100%" border="0">
            <tr>
                <td style="width: 250px" valign="top">
                    <div id="myCalandar">
                    </div>
                    <div class="SiftLight" style="padding-top:5px; text-align:center;">(點選日期可查詢當日彙整)</div>
                    <div style="display: none">
                        <asp:TextBox ID="tb_ShowDT" runat="server"></asp:TextBox>
                        <asp:Button ID="btn_Search" runat="server" Text="查詢" OnClick="btn_Search_Click" /></div>
                </td>
                <td valign="top">
                    <div class="Sift">
                        <div style="float: left">
                            查詢日期:<asp:Label ID="lb_SearchDate" runat="server" CssClass="B styleRed"></asp:Label>
                        </div>
                        <div style="float: right">
                            <asp:Button ID="btn_Sh1" runat="server" Text="7日內明細" CssClass="btnBlock colorCoffee"
                                OnClick="btn_Sh1_Click" />
                            <asp:Button ID="btn_Sh2" runat="server" Text="30日內明細" CssClass="btnBlock colorCoffee"
                                OnClick="btn_Sh2_Click" />
                        </div>
                    </div>
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table class="List1" width="100%">
                                <tr class="tdHead">
                                    <td width="25%">
                                        部門
                                    </td>
                                    <td width="25%">
                                        未完成
                                    </td>
                                    <td width="25%">
                                        已完成
                                    </td>
                                    <td width="25%">
                                        總件數
                                    </td>
                                </tr>
                                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr id="trItem" runat="server">
                                <td align="center" class="L2MainHead">
                                    <%# fn_Desc.PubAll.AreaDesc(Eval("Area").ToString())%>
                                    -
                                    <%# Eval("DeptName")%>
                                </td>
                                <td align="center" class="JQ-ui-state-default">
                                    <a href="WR_Search.aspx?DeptID=<%# Server.UrlEncode(Eval("DeptID").ToString())%>&StartDate=<%# Server.UrlEncode(this.tb_ShowDT.Text)%>&EndDate=<%# Server.UrlEncode(this.tb_ShowDT.Text)%>&IsDone=N"
                                        class="styleBlue B" title="查看明細">
                                        <%# Eval("unCompleted")%><span class="JQ-ui-icon ui-icon-extlink"></span></a>
                                </td>
                                <td align="center" class="JQ-ui-state-default">
                                    <a href="WR_Search.aspx?DeptID=<%# Server.UrlEncode(Eval("DeptID").ToString())%>&StartDate=<%# Server.UrlEncode(this.tb_ShowDT.Text)%>&EndDate=<%# Server.UrlEncode(this.tb_ShowDT.Text)%>&IsDone=Y"
                                        class="styleGreen B" title="查看明細">
                                        <%# Eval("Completed")%><span class="JQ-ui-icon ui-icon-extlink"></span></a>
                                </td>
                                <td align="center">
                                    <%# Eval("Total")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div style="padding: 100px 0px 100px 0px; text-align: center">
                                <span style="color: #FD590B; font-size: 12px">
                                    <%=this.tb_ShowDT.Text %>
                                    查無資料！</span>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
