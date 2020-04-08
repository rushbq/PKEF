<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cust_View.aspx.cs" Inherits="Cust_View" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>基本資料維護</a>&gt;<span>客戶基本資料</span>
        </div>
        <div class="h2Head">
            <h2>客戶基本資料</h2>
        </div>
        <div class="table-responsive">
            <table class="TableModify table table-bordered">
                <!-- 主檔 Start -->
                <tr class="ModifyHead">
                    <td colspan="4">基本資料<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody>
                    <tr>
                        <td class="TableModifyTdHead" style="width: 150px">客戶代號
                        </td>
                        <td class="TableModifyTd styleBlue B" style="width: 400px">
                            <asp:Literal ID="lt_CustID" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead" style="width: 150px">主要資料庫
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_DBName" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr class="Must">
                        <td class="TableModifyTdHead">客戶簡稱
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_CustSortName" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead"><em>(*)</em> 出貨庫別
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_SW" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">客戶全稱
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_CustFullName" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">地區/國家
                        </td>
                        <td class="TableModifyTd">
                            <asp:Label ID="lb_AreaName" runat="server" CssClass="label label-default"></asp:Label>
                            <asp:Label ID="lb_CountryName" runat="server" CssClass="label label-default"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">客戶Email
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_Email" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">交易幣別
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_Currency" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">出貨地址
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_ShipAddr" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">負責業務
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_RepSales" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">維護資訊
                        </td>
                        <td class="TableModifyTd" colspan="3">
                            <table cellpadding="3" border="0">
                                <tr>
                                    <td align="right" width="100px">最後更新者：
                                    </td>
                                    <td class="styleGreen" width="200px">
                                        <asp:Literal ID="lt_Update_Who" runat="server" Text="資料維護中"></asp:Literal>
                                    </td>
                                    <td align="right" width="100px">最後更新時間：
                                    </td>
                                    <td class="styleGreen" width="250px">
                                        <asp:Literal ID="lt_Update_Time" runat="server" Text="資料維護中"></asp:Literal>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                </tbody>
                <!-- 主檔 End -->
            </table>
        </div>

        <div class="table-responsive">
            <table class="TableModify table table-bordered">
                <tbody>
                    <tr class="ModifyHead">
                        <td colspan="4">其他資料<em class="TableModifyTitleIcon"></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead" style="width: 150px">負責人
                        </td>
                        <td class="TableModifyTd" style="width: 400px">
                            <asp:Literal ID="lt_MA004" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead" style="width: 150px">連絡人
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA005" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead styleRed">戶名(開票用)<br />(勿超過100字,含空白)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA110" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead styleRed">稅號(開票用)<br />(勿超過20字,含空白)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA071" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">TEL_NO(一)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA006" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">TEL_NO(二)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA007" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">FAX_NO
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA008" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead"></td>
                        <td class="TableModifyTd"></td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">通路別
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA017" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">最近交易日
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA022" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">登記地址(一)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA023" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">登記地址(二)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA024" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">發票地址(一)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA025" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">發票地址(二)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA026" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">送貨地址(一)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA027" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead"></td>
                        <td class="TableModifyTd"></td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">價格條件
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA030" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">付款條件
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA031" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">發票聯數
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA037" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">課稅別
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA038" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">郵遞區號
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA040" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">收款方式
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA041" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">運輸方式
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA048" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">目的地
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA051" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">總店號
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA065" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">總公司請款
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA066" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">分店數
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA067" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">型態別
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA076" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">路線別
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA077" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">其他別
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA078" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">發票地址-郵遞區號
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA079" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">送貨地址-郵遞區號
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA080" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">文件地址-郵遞區號
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA081" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">帳單郵遞區號
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA098" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">帳單地址(一)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA099" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">帳單地址(二)
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA100" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">帳單收件人
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA101" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">稅別碼
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_MA118" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <script>
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                $('#btn_Save').trigger('click');
            });

            //返回列表
            function goToList() {
                location.href = '<%=Session["BackListUrl"] %>';
            }
        </script>

        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
