<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Order_Edit.aspx.cs" Inherits="Order_Edit" %>

<%@ Register Src="../Ascx_ScrollIcon.ascx" TagName="Ascx_ScrollIcon" TagPrefix="ucIcon" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="../css/layout.css?v=20160622" rel="stylesheet" type="text/css" />
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.1/css/font-awesome.min.css" rel="stylesheet" />
    <script src="../js/jquery.js"></script>
    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <%-- blockUI Start --%>
    <script src="../js/blockUI/jquery.blockUI.js" type="text/javascript"></script>
    <script src="../js/ValidCheckPass.js" type="text/javascript"></script>
    <script src="../js/blockUI/customFunc.js"></script>
    <%-- blockUI End --%>
    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>
    <script src="../js/public.js"></script>
    <script src="../js/dynamic-ListItem.js"></script>
    <%-- steps Start --%>
    <link href="../js/steps/jquery-steps.css" rel="stylesheet" />
    <script src="../js/steps/jquery-steps.js"></script>
    <%-- steps End --%>
    <%-- Autocompelete Start --%>
    <link href="../js/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="../js/catcomplete/catcomplete.js"></script>
    <script>
        $(function () {
            /* Autocomplete - 品號 */
            $("#tb_myFilterItem").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ModelNo.aspx",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: "(" + item.id + ") " + item.label,
                                        category: item.category,
                                        value: item.label,
                                        id: item.id
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    $("#tb_getItemID").val(ui.item.id);
                    $("#tb_getItemName").val(ui.item.value);

                    //呼叫動態欄位, 新增項目
                    Add_Item("myItemList", "tb_getItemID", "tb_getItemName", true, false, '-', '下一步取得');

                    //清除輸入欄
                    $(this).val("");
                    event.preventDefault();
                }
            });

            /* Autocomplete - 客戶 */
            $("#tb_CustName").autocomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "../AC_Customer.aspx?f=myCust",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: "(" + item.custId + ") " + item.shortName,
                                        value: item.shortName,
                                        myDB: item.myDB,
                                        custId: item.custId,
                                        shortName: item.shortName,
                                        currency: item.currency,
                                        shipWho: item.shipWho,
                                        tel: item.tel,
                                        shipAddr: item.shipAddr,
                                        SWID: item.SWID,
                                        salesID: item.salesID

                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    $("#lb_custID").text(ui.item.custId);
                    $("#lb_custName").text(ui.item.shortName);
                    $("#lb_currency").text(ui.item.currency);

                    $("#tb_CustID").val(ui.item.custId);
                    $("#tb_Currency").val(ui.item.currency);
                    $("#tb_ShipWho").val(ui.item.shipWho);
                    $("#tb_ShipAddr").val(ui.item.shipAddr);
                    $("#tb_ContactTel").val(ui.item.tel);
                    $("#tb_DBS").val(ui.item.myDB);
                    $("#tb_SWID").val(ui.item.SWID);
                    $("#tb_SalesID").val(ui.item.salesID);
                }
            });

            /* Autocomplete - 贈品 */
            $("#tb_myFilterItem_Gift").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ModelNo.aspx",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: "(" + item.id + ") " + item.label,
                                        category: item.category,
                                        value: item.label,
                                        id: item.id
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    $("#tb_getItemID_Gift").val(ui.item.id);
                    $("#tb_getItemName_Gift").val(ui.item.value);


                    //取得資料庫別
                    var DBS = $("#tb_DBS").val();
                    //取得庫別
                    var SWID = $("#tb_SWID").val();

                    //WebService 取得庫存
                    getStock(DBS, SWID, ui.item.id, function (stockNum) {
                        //呼叫動態欄位, 新增項目
                        Add_Item("myItemList_Gift", "tb_getItemID_Gift", "tb_getItemName_Gift", true, false, '-', stockNum);
                    });


                    //清除輸入欄
                    $(this).val("");
                    event.preventDefault();
                }
            });

        });


        //取得庫存量 - webservice
        var getStock = function (DBS, SWID, modelNo, callback) {
            var myAjaxReq;
            var dataReturn;

            //-- Ajax處理 Start --
            //Call API_ErpData
            myAjaxReq = $.ajax({
                url: '<%=Application["WebUrl"]%>Ajax_Data/API_ERPData.asmx/GetStockNum',
                method: "POST",
                data: {
                    DBS: DBS,
                    SWID: SWID,
                    ModelNo: modelNo
                }
            });

            myAjaxReq.done(function (data) {
                //判斷是否為fail
                if (data.indexOf("fail:") != -1) {
                    //警示訊息
                    //alert(data.replace(/fail:/i, ""));
                    dataReturn = "-1";


                } else {
                    //輸出結果
                    dataReturn = data.replace("-", "0");
                }

                callback(dataReturn);
            });

            myAjaxReq.fail(function (jqXHR, textStatus) {
                alert('無法取得庫存資料' + textStatus);

            });

            //-- Ajax處理 End --

        }
    </script>
    <%-- Autocompelete End --%>
    <script>
        $(function () {
            //Affix
            $('.myAffix').affix({
                offset: {
                    top: function () {
                        return (this.top = $('.progressbar').offset())
                    }
                }
            });

            //Submit - Step4
            $("#triggerSave").click(function () {
                //贈品資料
                Get_Item('myItemList_Gift', 'myValues_Gift', 'Item_ID');
                Get_Item('myItemList_Gift', 'myValues_Gift_Qty', 'Item_Qty');
                Get_Item('myItemList_Gift', 'myValues_Gift_StockNum', 'Item_Stock');
                
                //BlockUI
                blockBox1("Add", "資料處理中，請稍候...");

                //Save
                $("#btn_Save").trigger("click");
            });


            /* Step2 to Step3 - Click事件, 取得價格 Start */
            $("#GetItemList").click(function () {
                var myAjaxReq;

                //鎖定畫面
                blockBox2("資料擷取中，請稍候...");

                //取得Step2產品清單
                Get_Item('myItemList', 'myValues', 'Item_ID');
                Get_Item('myItemList', 'myValues_Qty', 'Item_Qty');

                /* 取得價格 & 庫存 */
                //取得客戶編號
                var custID = $("#tb_CustID").val();
                //取得資料庫別
                var DBS = $("#tb_DBS").val();
                //取得庫別
                var SWID = $("#tb_SWID").val();
                //取得品號
                var modelNo = $("#myValues").val();
                //取得輸入數量
                var qty = $("#myValues_Qty").val();

                //判斷空值
                if (custID == '' || modelNo == '' || qty == '') {
                    //關閉鎖定
                    $.unblockUI()

                    alert('請將資料填寫齊全，即將跳轉回第一步');
                    location.href = '<%=PageUrl%>';

                    myAjaxReq.abort();

                }

                //-- Ajax處理 Start --
                //Call API_ErpData
                myAjaxReq = $.ajax({
                    url: '<%=Application["WebUrl"]%>Ajax_Data/API_ERPData.asmx/GetData',
                    method: "POST",
                    data: {
                        custID: custID,
                        DBS: DBS,
                        valModelNo: modelNo,
                        valQty: qty,
                        SWID: SWID
                    },
                    dataType: "html"
                });

                myAjaxReq.done(function (data) {
                    //關閉鎖定
                    $.unblockUI()

                    //判斷是否為fail
                    if (data.indexOf("fail:") != -1) {
                        //警示訊息
                        alert(data.replace(/fail:/i, ""));


                    } else {
                        //輸出結果html
                        $("#CheckItemList").html(data);
                    }
                });

                myAjaxReq.fail(function (jqXHR, textStatus) {
                    //關閉鎖定
                    $.unblockUI()

                    alert('無法取得資料');

                });

                //-- Ajax處理 End --
            });
            /* Step2 to Step3 - Click事件, 取得價格 End */

        });
    </script>
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="page-header">
            <h3>新增訂單</h3>
        </div>
        <div class="row">
            <div class="col-xs-12">
                <div class="form-horizontal">
                    <div class="mySteps">
                        <!-- progressbar -->
                        <ul class="progressbar">
                            <li class="active">填寫基本資料</li>
                            <li>填寫購買產品及數量</li>
                            <li>確認購買清單</li>
                            <li>即將完成訂單</li>
                        </ul>
                        <div class="stepContainer">
                            <!-- Step 1 -->
                            <fieldset data-rel="step1">
                                <div class="vertical-container">
                                    <div class="row">
                                        <div class="col-xs-10 col-sm-10 vcenter">
                                            <div id="s1_alert" class="alert alert-danger" role="alert">
                                                <span>請先確認以下事項是否已設定完成:</span>
                                                <ul>
                                                    <li>個人所屬公司別</li>
                                                    <li>客戶出貨庫別</li>
                                                </ul>
                                            </div>
                                            <div class="bq-callout orange">
                                                <h4>客戶資料</h4>
                                                <div class="form-group has-error">
                                                    <label class="col-sm-2 text-right">選擇客戶</label>
                                                    <div class="col-sm-10">
                                                        <asp:TextBox ID="tb_CustName" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:Label ID="lb_CustID" runat="server" CssClass="styleEarth"></asp:Label>
                                                        <asp:RequiredFieldValidator ID="rfv_tb_CustID" runat="server"
                                                            ControlToValidate="tb_CustID" Display="Dynamic" ValidationGroup="Add"><div class="alert alert-danger">此欄位必填</div></asp:RequiredFieldValidator>
                                                        <div class="help-block">(請輸入關鍵字：客戶編號或名稱)</div>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">客戶代號</label>
                                                    <div class="col-sm-10">
                                                        <span id="lb_custID"></span>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">客戶名稱</label>
                                                    <div class="col-sm-10">
                                                        <span id="lb_custName"></span>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">幣別</label>
                                                    <div class="col-sm-10">
                                                        <span id="lb_currency"></span>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">自訂單號</label>
                                                    <div class="col-sm-10">
                                                        <asp:TextBox ID="tb_myOrderID" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="bq-callout red">
                                                <h4>收貨資料</h4>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">收貨人</label>
                                                    <div class="col-sm-10">
                                                        <asp:TextBox ID="tb_ShipWho" runat="server" CssClass="form-control" MaxLength="25"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfv_tb_ShipWho" runat="server"
                                                            ControlToValidate="tb_ShipWho" Display="Dynamic" ValidationGroup="Add" ErrorMessage="收貨人"><div class="alert alert-danger">此欄位必填</div></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">郵遞區號</label>
                                                    <div class="col-sm-10 form-inline">
                                                        <asp:TextBox ID="tb_ZipCode" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                        <%--<asp:RequiredFieldValidator ID="rfv_tb_ZipCode" runat="server"
                                                            ControlToValidate="tb_ZipCode" Display="Dynamic" ValidationGroup="Add" ErrorMessage="郵遞區號"><div class="alert alert-danger">此欄位必填</div></asp:RequiredFieldValidator>--%>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">收貨地址</label>
                                                    <div class="col-sm-10">
                                                        <asp:TextBox ID="tb_ShipAddr" runat="server" CssClass="form-control" MaxLength="200"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfv_tb_ShipAddr" runat="server"
                                                            ControlToValidate="tb_ShipAddr" Display="Dynamic" ValidationGroup="Add" ErrorMessage="收貨地址"><div class="alert alert-danger">此欄位必填</div></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-2 text-right">聯絡電話</label>
                                                    <div class="col-sm-10">
                                                        <asp:TextBox ID="tb_ContactTel" runat="server" CssClass="form-control" MaxLength="40"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfv_tb_ContactTel" runat="server"
                                                            ControlToValidate="tb_ContactTel" Display="Dynamic" ValidationGroup="Add" ErrorMessage="聯絡電話"><div class="alert alert-danger">此欄位必填</div></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col-xs-2 col-sm-1 vcenter myAffix">
                                            <button type="button" class="next btn btn-primary"><i class="fa fa-arrow-right fa-3x"></i></button>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                            <!-- Step 2 -->
                            <fieldset data-rel="step2">
                                <div class="vertical-container">
                                    <div class="row">
                                        <div class="col-xs-2 col-sm-2 vcenter myAffix">
                                            <button type="button" class="previous btn btn-default"><i class="fa fa-arrow-left fa-3x"></i></button>
                                        </div>
                                        <div class="col-xs-8 col-sm-9 vcenter">
                                            <div class="form-group has-error">
                                                <label class="col-sm-2 text-right">選擇產品</label>
                                                <div class="col-sm-10">
                                                    <asp:TextBox ID="tb_myFilterItem" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <input type="hidden" id="tb_getItemID" />
                                                    <input type="hidden" id="tb_getItemName" />
                                                    <div class="help-block">(請輸入關鍵字：品號或品名)</div>
                                                </div>
                                            </div>

                                            <div class="form-group">
                                                <div data-rel="data-list">
                                                    <ul class="list-group myStriped" id="myItemList">
                                                        <asp:Literal ID="lt_ViewList" runat="server"></asp:Literal>
                                                    </ul>

                                                    <asp:TextBox ID="myValues" runat="server" ToolTip="品號欄位值集合" Style="display: none;">
                                                    </asp:TextBox>
                                                    <asp:TextBox ID="myValues_Qty" runat="server" ToolTip="數量欄位值集合" Style="display: none;">
                                                    </asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfv_myValues" runat="server" Display="None"
                                                        ControlToValidate="myValues" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-xs-2 col-sm-1 vcenter myAffix">
                                            <button type="button" class="next btn btn-primary" id="GetItemList"><i class="fa fa-arrow-right fa-3x"></i></button>

                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                            <!-- Step 3 -->
                            <fieldset data-rel="step3">
                                <div class="vertical-container">
                                    <div class="row">
                                        <div class="col-xs-2 col-sm-2 vcenter myAffix">
                                            <button type="button" class="previous btn btn-default"><i class="fa fa-arrow-left fa-3x"></i></button>
                                        </div>
                                        <div class="col-xs-8 col-sm-9 vcenter">
                                            <div class="alert alert-danger" role="alert">
                                                <ul>
                                                    <li>此頁面的價格，有可能會因ERP資料變更而有所變動。</li>
                                                    <li>在完成訂單前，ERP價格有變動，系統將會在產生訂單時，自動取得最新價格。</li>
                                                    <li>庫存數量=目前庫存 -預計銷 - (安全存量*20%)</li>
                                                </ul>
                                            </div>
                                            <!-- 產品List -->
                                            <ul class="list-group myStriped" id="CheckItemList"></ul>

                                            <!-- 贈品 -->
                                            <ul class="list-group myStriped" id="myItemList_Gift">
                                                <li class="list-group-item list-group-item-default">
                                                    <h4><i class="fa fa-gift"></i>&nbsp;贈品清單</h4>
                                                    <asp:TextBox ID="tb_myFilterItem_Gift" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <input type="hidden" id="tb_getItemID_Gift" />
                                                    <input type="hidden" id="tb_getItemName_Gift" />
                                                    <div class="help-block">(請輸入關鍵字：品號或品名)</div>
                                                    <asp:TextBox ID="myValues_Gift" runat="server" ToolTip="品號欄位值集合" Style="display: none;">
                                                    </asp:TextBox>
                                                    <asp:TextBox ID="myValues_Gift_Qty" runat="server" ToolTip="數量欄位值集合" Style="display: none;">
                                                    </asp:TextBox>
                                                    <asp:TextBox ID="myValues_Gift_StockNum" runat="server" ToolTip="庫存欄位值集合" Style="display: none;">
                                                    </asp:TextBox>
                                                </li>
                                                <asp:Literal ID="lt_ViewList_Gift" runat="server"></asp:Literal>
                                            </ul>

                                            <ul class="list-group">
                                                <!-- 運費 -->
                                                <li class="list-group-item list-group-item-success">
                                                    <div class="row">
                                                        <div class="col-xs-5">
                                                            <h4><i class="fa fa-truck"></i>&nbsp;運費(B001)</h4>
                                                        </div>
                                                        <div class="col-xs-7 text-right">
                                                            $&nbsp;<asp:TextBox ID="tb_Freight" runat="server" CssClass="text-center" onkeyup="checkNum(this)" Width="100px">0</asp:TextBox>
                                                        </div>
                                                    </div>
                                                </li>
                                                <!-- 自訂折扣 -->
                                                <li class="list-group-item list-group-item-info">
                                                    <div class="row">
                                                        <div class="col-xs-5">
                                                            <h4><i class="fa fa-strikethrough"></i>&nbsp;自訂折扣(W001)</h4>
                                                        </div>
                                                        <div class="col-xs-7 text-right">
                                                            <i class="fa fa-minus text-danger"></i>&nbsp;$&nbsp;
                                                                   <asp:TextBox ID="tb_DisPrice" runat="server" CssClass="text-center" onkeyup="checkNum(this)" Width="100px">0</asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-xs-12">
                                                            <asp:TextBox ID="tb_DisRemark" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="備註說明(最多200字)" MaxLength="200"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </li>
                                            </ul>
                                        </div>
                                        <div class="col-xs-2 col-sm-1 vcenter myAffix">
                                            <button type="button" class="next btn btn-primary"><i class="fa fa-arrow-right fa-3x"></i></button>
                                        </div>
                                    </div>
                                </div>

                            </fieldset>
                            <!-- Step 4 -->
                            <fieldset data-rel="step4">
                                <div class="vertical-container">
                                    <div class="row">
                                        <div class="col-xs-2 col-sm-2 vcenter myAffix">
                                            <button type="button" class="previous btn btn-default"><i class="fa fa-arrow-left fa-3x"></i></button>
                                        </div>
                                        <div class="col-xs-10 col-sm-10 vcenter">
                                            <div class="form-group">
                                                <h4>即將完成</h4>
                                                <p>資料確認無誤後，請按下「送出訂單」，資料將會開始轉入ERP。</p>
                                                <p>轉入過程請勿關閉視窗，或點按其他連結，以免發生不可預期的錯誤。</p>
                                                <div class=" col-xs-offset-9">
                                                    <input type="button" id="triggerSave" class="finish btn btn-success" value="送出訂單" />
                                                    <asp:Button ID="btn_Save" runat="server" ValidationGroup="Add" Style="display: none;" OnClick="btn_Save_Click" />
                                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" HeaderText="資料填寫不完全，請返回檢查!" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </fieldset>


                            <%-- 隱藏欄位值 --%>
                            <div class="hidden">
                                <asp:TextBox ID="tb_CustID" runat="server" Style="display: none" ToolTip="客戶ERP代號"></asp:TextBox>
                                <asp:TextBox ID="tb_Currency" runat="server" Style="display: none" ToolTip="幣別"></asp:TextBox>
                                <asp:TextBox ID="tb_DBS" runat="server" Style="display: none" ToolTip="DBS"></asp:TextBox>
                                <asp:TextBox ID="tb_SWID" runat="server" Style="display: none" ToolTip="庫別代號ShippingWarehouse"></asp:TextBox>
                                <asp:TextBox ID="tb_SalesID" runat="server" Style="display: none" ToolTip="業務人員代號"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

    </form>
    <script>
        /*
            判斷輸入值是否為數字
            因欄位是動態欄位，所以放在頁面最下方
        */
        function checkNum(myObj) {
            myObj.value = myObj.value.replace(/\D/g, '')
        }

    </script>
</body>
</html>
