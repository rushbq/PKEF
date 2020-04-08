<%@ Page Language="C#" AutoEventWireup="true" CodeFile="fullPrice.aspx.cs" Inherits="myPrice_fullPrice" %>

<%@ Import Namespace="ExtensionMethods" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.1/css/font-awesome.min.css" rel="stylesheet" />
    <link href="../css/layout.css?v=20160622" rel="stylesheet" type="text/css" />
    <script src="https://code.jquery.com/jquery-2.2.3.min.js" integrity="sha256-a23g1Nt4dtEYOj7bR+vTu7+T8VP13humZFBJNIYoEJo=" crossorigin="Proskit"></script>
    <%-- bootstrap Start --%>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="Proskit" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js" integrity="sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS" crossorigin="Proskit"></script>
    <%-- bootstrap End --%>
    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.11/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/dataTables.bootstrap.min.js"></script>
    <script>
        $(function () {
            /*
             [使用DataTable]
             注意:標題欄須與內容欄數量相等
             ref:https://datatables.net/reference/option/columns.render
           */
            $('#listTable').DataTable({
                "processing": true,
                "ajax": "../Ajax_Data/GetFullPriceData.ashx?DataID=<%=Req_DataID%>",
                "searching": true,  //搜尋
                "ordering": true,   //排序
                "paging": true,     //分頁
                "info": false,      //筆數資訊
                //指定欄位值
                "columns": [
                    {
                        "orderable": false,
                        "data": null,
                        "defaultContent": ''
                    },
                    { "data": "Stop_Offer", "orderable": false },
                    { "data": "Model_No" },
                    { "data": "Currency", "orderable": false },
                    { "data": "myPrice" },
                    { "data": "Unit", "orderable": false },
                    { "data": "QuoteDate" },
                    { "data": "MOQ" },
                    { "data": "Vol" },
                    { "data": "Page" },
                    { "data": "InnerBox_Qty", "orderable": false },
                    { "data": "InnerBox_NW", "orderable": false },
                    { "data": "InnerBox_GW", "orderable": false },
                    { "data": "InnerBox_Cuft", "orderable": false },
                    { "data": "BarCode" },
                    { "data": "Packing_zh_TW", "orderable": false },
                    { "data": "Ship_From" },
                    { "data": "TransTermValue", "orderable": false }
                ],

                //讓不排序的欄位在初始化時不出現排序圖
                "order": [],
                //自訂欄位
                "columnDefs": [{
                    "targets": 0,   //第 n 欄
                    "data": "Model_No", //欄位資料
                    "render": function (data, type, full, meta) {
                        return urlFormat(data);
                    }
                },
                {
                    "targets": 2,   //第 n 欄
                    "data": "Model_Name_zh_TW", //欄位資料
                    "render": function (data, type, full, meta) {
                        return nameFormat(full);
                    }
                }],
                "pageLength": 25,   //顯示筆數預設值  
                //捲軸設定
                "scrollY": '500px',
                "scrollCollapse": true,
                "scrollX": true
            });
        });

        //回傳圖片路徑Html
        function urlFormat(d) {
            var data = encodeURIComponent(d.Model_No);
            return '<a href="https://api.prokits.com.tw/myProd/' + data + '/" data-fancybox-type="iframe" class="myPhoto btn btn-default" title="' + data + '">' +
              '<i class="fa fa-file-image-o"></i></a>';
        }

        //回傳名稱Html
        function nameFormat(d) {
            return '<div class="text-danger"><strong>' + d.Model_No + '</strong></div><div>' + d.Model_Name_zh_TW + '</div>';
        }
    </script>
    <%-- DataTable End --%>
    <%-- fancybox Start --%>
    <script src="../js/fancybox/jquery.fancybox.pack.js" type="text/javascript"></script>
    <link href="../js/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <script>
        $(function () {
            //fancybox - 圖片顯示
            $(".myPhoto").fancybox({
                fitToView: true,
                autoSize: true,
                closeClick: true,
                openEffect: 'elastic',
                closeEffect: 'elastic',
                helpers: {
                    title: {
                        type: 'over'
                    }
                }
            });

        });
    </script>
    <%-- fancybox End --%>
</head>
<body class="myBody">
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="page-header">
                        <h2>客戶報價
                    <small>
                        <a>業務行銷</a>&nbsp;
                        <i class="fa fa-chevron-right"></i>&nbsp;<span>客戶報價</span>
                    </small>
                        </h2>
                    </div>
                </div>
            </div>
            <!-- Filter -->
            <div class="panel panel-warning">
                <div class="panel-heading">客戶名稱</div>
                <div class="panel-body">
                    <div class="pull-left">
                        <h4 class="text-danger">
                            <asp:Literal ID="lt_CustName" runat="server"></asp:Literal></h4>
                    </div>
                    <div class="pull-right">
                        <asp:Button ID="btn_PriceList" runat="server" Text="匯出Excel" OnClick="btn_PriceList_Click" CssClass="btn btn-primary" />
                        <a href="index.aspx" class="btn btn-default">返回查詢</a>
                    </div>
                    <div class="clearfix"></div>
                </div>
            </div>
            <!-- Result -->
            <div class="panel panel-info">
                <div class="panel-heading">查詢結果 (執行時間若超過<strong class="text-danger">5分鐘</strong>無資料，請重試。若仍無資料請<a href="http://pkef.prokits.com.tw/?t=ithelp" class="text-success" target="_blank"><strong>回報</strong></a>)</div>
                <div style="padding-top: 10px;">
                    <table id="listTable" class="table table-bordered table-striped">
                        <thead>
                            <tr>
                                <th>&nbsp;</th>
                                <th>Stop Offer</th>
                                <th>Item NO / Desc.</th>
                                <th>Currency</th>
                                <th>Unit Price</th>
                                <th>Unit</th>
                                <th>Quote Date</th>
                                <th>MOQ</th>
                                <th>VOL</th>
                                <th>Page</th>
                                <th>Qty Inner</th>
                                <th>NW</th>
                                <th>GW</th>
                                <th>CUFT</th>
                                <th>BarCode</th>
                                <th>Packing</th>
                                <th>Ship From</th>
                                <th>Term</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
