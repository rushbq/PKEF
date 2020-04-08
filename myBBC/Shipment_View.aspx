<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Shipment_View.aspx.cs" Inherits="Shipment_View" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="../css/font-awesome.min.css" rel="stylesheet" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            /* 出貨方式選擇 */
            $("#ddl_ShipType").change(function () {
                //取得出貨方式
                var GetVal = $(this).find(':selected').val();

                Check_ShipType(GetVal);
            });

            $('#ddl_ShipType').trigger('change');

        });

        //判斷出貨方式
        function Check_ShipType(inputVal) {
            var myExpress = $(".express");  //物流配送
            var myRemark = $(".remark");    //其他

            switch (inputVal) {
                case "1":
                    myExpress.hide('fast');
                    myRemark.hide('fast');
                    break;

                case "2":
                    myExpress.show('slow');
                    myRemark.show('slow');
                    break;

                case "3":
                    myExpress.hide('fast');
                    myRemark.show('slow');
                    break;

                default:
                    myExpress.hide('fast');
                    myRemark.hide('fast');
                    break;
            }
        }
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a><%=Application["WebName"]%></a>&gt;<a>BBC平台</a>&gt;<span>出貨單-內銷</span>
        </div>
        <div class="h2Head">
            <h2>
                <asp:Literal ID="lt_Top_CompanyName" runat="server"></asp:Literal>
                -
                <asp:Literal ID="lt_Top_OrderID" runat="server"></asp:Literal></h2>
        </div>
        <!-- 基本資料 Start -->
        <div class="table-responsive">
            <table class="TableModify table table-bordered">
                <tr class="ModifyHead">
                    <td colspan="4">基本資料<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody>
                    <tr>
                        <td class="TableModifyTdHead" style="width: 150px">網路單號</td>
                        <td class="TableModifyTd styleBlue B" style="width: 350px">
                            <asp:Literal ID="lt_OrderID" runat="server"></asp:Literal>
                            <asp:Literal ID="lt_MyOrderID" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead" style="width: 150px">狀態</td>
                        <td class="TableModifyTd">
                            <asp:Label ID="lb_Status" runat="server"></asp:Label>
                            |
                            <a href="javascript:void(0)" data-toggle="modal" data-target="#myModal">查看下單品項&nbsp;<i class="fa fa-external-link"></i></a>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">客戶代號</td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_CustID" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">客戶簡稱</td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_CustName" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">收貨人</td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_ShipWho" runat="server"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">收貨地址</td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_ShipAddr" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTdHead">建立者
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_Create_Who" runat="server" Text="資料維護中"></asp:Literal>
                        </td>
                        <td class="TableModifyTdHead">建立時間
                        </td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_Create_Time" runat="server" Text="資料維護中"></asp:Literal>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- 基本資料 End -->
        <!-- 物流資料 Start -->
        <div class="table-responsive">
            <table class="TableModify table table-bordered">
                <tr class="ModifyHead">
                    <td colspan="4">物流資料<em class="TableModifyTitleIcon"></em>
                    </td>
                </tr>
                <tbody>
                    <tr class="Must">
                        <td class="TableModifyTdHead" style="width: 150px"><em>(*)</em> 選擇出貨方式</td>
                        <td class="TableModifyTd" style="width: 350px">
                            <asp:DropDownList ID="ddl_ShipType" runat="server" CssClass="form-control" Enabled="false"></asp:DropDownList>
                        </td>

                        <td class="TableModifyTdHead" style="width: 150px">出貨日</td>
                        <td class="TableModifyTd">
                            <i class="fa fa-calendar-check-o"></i>&nbsp;
                            <asp:Literal ID="lt_ShipDate" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr class="express" style="display: none;">
                        <td class="TableModifyTdHead" style="width: 150px">物流商</td>
                        <td class="TableModifyTd" style="width: 350px">
                            <asp:DropDownList ID="ddl_ShipVendor" runat="server" CssClass="form-control" Enabled="false"></asp:DropDownList>
                        </td>
                        <td class="TableModifyTdHead" style="width: 150px">物流單號</td>
                        <td class="TableModifyTd">
                            <asp:Literal ID="lt_ShipNo" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr class="express" style="display: none;">
                        <td class="TableModifyTdHead">運費支付方式</td>
                        <td class="TableModifyTd">
                            <asp:DropDownList ID="ddl_ShipFareType" runat="server" CssClass="form-control" Enabled="false"></asp:DropDownList>
                        </td>
                        <td class="TableModifyTdHead">運費</td>
                        <td class="TableModifyTd">
                            <i class="fa fa-truck"></i>&nbsp;
                            <asp:Literal ID="lt_ShipFareMoney" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr class="express" style="display: none;">
                        <td class="TableModifyTdHead">箱子</td>
                        <td class="TableModifyTd" colspan="3">
                            <div>
                                <ul class="list-group" id="myItemList">
                                    <asp:Literal ID="lt_ViewList" runat="server"></asp:Literal>
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr class="remark" style="display: none;">
                        <td class="TableModifyTdHead" style="width: 150px">備註</td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:Literal ID="lt_Remark" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- 物流資料 End -->
        <!-- 下單品項 Start -->
        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">下單品項資料</h4>
                    </div>
                    <div class="modal-body">
                        <div>
                            <asp:ListView ID="lvDataList_OrderProd" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <table class="table table-striped">
                                        <thead>
                                            <tr>
                                                <th width="35%">品號</th>
                                                <th width="25%">賣價</th>
                                                <th width="10%">數量</th>
                                                <th width="30%">備註</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr id="trItem" runat="server">
                                        <td>
                                            <p><strong><%#Eval("Model_No") %></strong></p>
                                        </td>
                                        <td>
                                            <em><%#Eval("Currency") %></em>$&nbsp;<%#Eval("SellPrice") %>
                                        </td>
                                        <td>
                                            <%#Eval("BuyCnt") %>
                                        </td>
                                        <td>
                                            <%#Eval("Remark") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <div style="padding: 30px 0;" class="text-center text-danger">
                                        <h3>查無符合資料！</h3>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
        <!-- 下單品項 End -->
        <!-- 銷貨資料 Start -->
        <div>
            <table class="TableModify table table-bordered">
                <tbody>
                    <tr class="ModifyHead">
                        <td colspan="4">銷貨明細<em class="TableModifyTitleIcon"></em>
                        </td>
                    </tr>
                    <tr>
                        <td class="TableModifyTd">
                            <div class="table-responsive">
                                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <table class="List1 table" width="100%">
                                            <thead>
                                                <tr class="tdHead">
                                                    <td width="200px">銷貨單號</td>
                                                    <td>品號</td>
                                                    <td>品名</td>
                                                    <td width="100px">數量</td>
                                                    <td width="100px">贈/備品量</td>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:PlaceHolder ID="ph_Items" runat="server"></asp:PlaceHolder>
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr id="trItem" runat="server">
                                            <td align="center" class="text-danger">
                                                <strong><%#Eval("SaleNoType") %> - <%#Eval("SaleNo") %></strong>
                                            </td>
                                            <td><strong><%#Eval("ModelNo") %></strong></td>
                                            <td>
                                                <%#Eval("ModelName") %>
                                            </td>
                                            <td align="center">
                                                <%# Convert.ToInt16(Eval("BuyCnt")) %>
                                            </td>
                                            <td align="center">
                                                <%# Convert.ToInt16(Eval("GiftCnt")) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <div style="padding: 30px 0;" class="text-center text-danger">
                                            <h3>ERP尚未有銷貨資料！</h3>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <!-- 銷貨資料 End -->

        <ucIcon:Ascx_ScrollIcon ID="Ascx_ScrollIcon1" runat="server" ShowSave="N" ShowList="Y"
            ShowTop="Y" ShowBottom="Y" />
    </form>
</body>
</html>
