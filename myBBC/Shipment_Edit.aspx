<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Shipment_Edit.aspx.cs" Inherits="Shipment_Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="../css/font-awesome.min.css" rel="stylesheet" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/public.js"></script>
    <script src="../js/dynamic-ListItem.js"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <script src="../js/blockUI/customFunc.js"></script>
    <%-- blockUI End --%>
    <script type="text/javascript">
        $(document).ready(function () {
            /* 日期選擇器 */
            $("#tb_ShipDate").datepicker({
                onSelect: function () { },
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1
            });

            /* 出貨方式選擇 */
            $("#ddl_ShipType").change(function () {
                //取得出貨方式
                var GetVal = $(this).find(':selected').val();

                Check_ShipType(GetVal);
            });

            $('#ddl_ShipType').trigger('change');


            /* 箱數動態新增 */
            $("#ddl_Box").change(function () {
                //取得值
                var GetValID = $(this).find(':selected').val();

                //取得文字
                var GetValName = $(this).find('option:selected').text();

                if (GetValID != '') {
                    //呼叫動態欄位, 新增項目
                    Add_Item_ShipBox("myItemList", GetValID, GetValName);
                }

            });

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
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
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
                        <td class="TableModifyTd" colspan="3">
                            <asp:DropDownList ID="ddl_ShipType" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfv_ddl_ShipType" runat="server" Display="Dynamic" ErrorMessage="請選擇出貨方式" ControlToValidate="ddl_ShipType" ForeColor="Red" CssClass="help-block" ValidationGroup="Add"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="express" style="display: none;">
                        <td class="TableModifyTdHead" style="width: 150px">物流商</td>
                        <td class="TableModifyTd" style="width: 350px">
                            <asp:DropDownList ID="ddl_ShipVendor" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                        <td class="TableModifyTdHead" style="width: 150px">物流單號</td>
                        <td class="TableModifyTd">
                            <asp:TextBox ID="tb_ShipNo" runat="server" CssClass="form-control"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="express" style="display: none;">
                        <td class="TableModifyTdHead">運費支付方式</td>
                        <td class="TableModifyTd">
                            <asp:DropDownList ID="ddl_ShipFareType" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                        <td class="TableModifyTdHead">運費</td>
                        <td class="TableModifyTd">
                            <div class="form-inline">
                                <div class="input-group">
                                    <span class="input-group-addon"><i class="fa fa-truck"></i></span>
                                    <asp:TextBox ID="tb_ShipFareMoney" runat="server" Width="100px" MaxLength="10" CssClass="form-control text-center"></asp:TextBox>
                                </div>
                            </div>
                            <asp:CompareValidator ID="cv_tb_ShipFareMoney" runat="server" ControlToValidate="tb_ShipFareMoney"
                                Display="Dynamic" ErrorMessage="請輸入數字" Operator="DataTypeCheck" Type="Double"
                                ForeColor="Red" CssClass="help-block" ValidationGroup="Add"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr class="express" style="display: none;">
                        <td class="TableModifyTdHead">出貨日</td>
                        <td class="TableModifyTd">
                            <div class="form-inline">
                                <div class="input-group">
                                    <span class="input-group-addon"><i class="fa fa-calendar-check-o"></i></span>
                                    <asp:TextBox ID="tb_ShipDate" runat="server" Width="150px" CssClass="form-control text-center"></asp:TextBox>
                                </div>
                            </div>
                            <asp:CompareValidator ID="cv_tb_ShipDate" runat="server" ControlToValidate="tb_ShipDate"
                                CultureInvariantValues="True" Display="Dynamic" ErrorMessage="請輸入正確的日期" Operator="DataTypeCheck"
                                Type="Date" ForeColor="Red" CssClass="help-block" ValidationGroup="Add"></asp:CompareValidator>
                        </td>
                        <td class="TableModifyTdHead">設定箱子</td>
                        <td class="TableModifyTd">
                            <asp:DropDownList ID="ddl_Box" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="express" style="display: none;">
                        <td class="TableModifyTd" colspan="4">
                            <div class="form-group">
                                <div data-rel="data-list">
                                    <ul class="list-group" id="myItemList">
                                        <asp:Literal ID="lt_ViewList" runat="server"></asp:Literal>
                                    </ul>

                                    <asp:TextBox ID="myValues" runat="server" ToolTip="ID欄位值集合" Style="display: none;">
                                    </asp:TextBox>
                                    <asp:TextBox ID="myValues_Qty" runat="server" ToolTip="數量欄位值集合" Style="display: none;">
                                    </asp:TextBox>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr class="remark" style="display: none;">
                        <td class="TableModifyTdHead" style="width: 150px">備註</td>
                        <td class="TableModifyTd" colspan="3">
                            <asp:TextBox ID="tb_Remark" runat="server" CssClass="form-control" TextMode="MultiLine" Width="90%" Height="150"></asp:TextBox>
                        </td>
                    </tr>
                </tbody>
                <tr>
                    <td colspan="4" class="TableModifyTd text-right bg-warning">
                        <input onclick="goToList();" type="button" value="返回列表" class="btn btn-default btn-sm" />

                        <input type="button" id="triggerSave" class="btn btn-primary btn-sm" value="存檔，待出貨" />
                        <input type="button" id="triggerGo" class="btn btn-danger btn-sm" value="確認出貨" />
                        <asp:Button ID="btn_Save" runat="server" Text="存檔" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
                        <asp:Button ID="btn_Go" runat="server" Text="出貨" OnClick="btn_Go_Click" ValidationGroup="Add" Style="display: none;" />

                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"
                            ShowMessageBox="true" ValidationGroup="Add" />
                    </td>
                </tr>
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
                                            <h3>ERP尚未有銷貨資料！請確認銷貨單為「網路訂單」、「客戶單號不為空白」</h3>
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

        <script>
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                blockBox1("Add", "資料處理中，請稍候...");

                //取得動態資料欄位
                Get_Item('myItemList', 'myValues', 'Item_ID');
                Get_Item('myItemList', 'myValues_Qty', 'Item_Qty');

                $('#btn_Save').trigger('click');
            });


            $("#triggerGo").click(function () {
                if (confirm("是否確認出貨?\n設為出貨之後，資料欄位將會鎖定。")) {

                    blockBox1("Add", "資料處理中，請稍候...");

                    //取得動態資料欄位
                    Get_Item('myItemList', 'myValues', 'Item_ID');
                    Get_Item('myItemList', 'myValues_Qty', 'Item_Qty');

                    $('#btn_Go').trigger('click');
                }
            });

            //返回列表
            function goToList() {
                location.href = '<%=Session["BackListUrl"] %>';
            }

            /*
               判斷輸入值是否為數字
               因欄位是動態欄位，所以放在頁面最下方
           */
            function checkNum(myObj) {
                myObj.value = myObj.value.replace(/\D/g, '')
            }
        </script>

    </form>
</body>
</html>
