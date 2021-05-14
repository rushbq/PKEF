<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IT_HelpSummary.aspx.cs" Inherits="IT_HelpSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Application["WebName"]%>
    </title>
    <link href="../css/System.css" rel="stylesheet" type="text/css" />
    <link href="../css/layout.css?v=20160622" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <%-- Scroll Top 按扭 Start --%>
    <link href="<%=Application["WebUrl"] %>js/scrollTop/scrollTop.css" rel="stylesheet"
        type="text/css" />
    <script src="<%=Application["WebUrl"] %>js/scrollTop/scrollTop.js" type="text/javascript"></script>
    <%-- Scroll Top 按扭 End --%>

    <%-- bootstrap Start --%>
    <script src="../js/bootstrap/js/bootstrap.min.js"></script>
    <link href="../js/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <%-- bootstrap End --%>

    <%-- jQueryUI Start --%>
    <link href="../js/smoothness/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <%-- jQueryUI End --%>
    <script type="text/javascript">
        $(function () {
            /* 日期選擇器 */
            $(".startDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $(".endDate").datepicker("option", "minDate", selectedDate);
                }
            });
            $(".endDate").datepicker({
                showOn: "button",
                buttonImage: "../images/System/IconCalendary6.png",
                buttonImageOnly: true,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $(".startDate").datepicker("option", "maxDate", selectedDate);
                }
            });

        });
    </script>

    <!-- Google Chart Start -->
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        // Load the Visualization API and the piechart package.
        google.load('visualization', '1', { packages: ['corechart', 'table', 'bar'] });

    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            setDate('1', 'CountByClass');

            //填入預設日期(當年開始~今日)
            var thisDate = new Date();
            var defStartDate = thisDate.getFullYear() + '/01/01';
            var defEndDate = thisDate.getFullYear() + '/' + ('0' + (thisDate.getMonth() + 1)).slice(-2) + '/' + ('0' + thisDate.getDate()).slice(-2);
            $(".startDate").val(defStartDate);
            $(".endDate").val(defEndDate);

            /*
              [進入頁面後,載入預設資料] Start
            */

            //載入圖表 - 案件數(類別)
            SearchbyClass(defStartDate, defEndDate);
            /*
              [進入頁面後,載入預設資料] End
            */

            //Click事件 - 查詢
            $("#doSearch").on("click", function () {
                //取得日期參數值
                var startDate = $(".startDate").val();
                var endDate = $(".endDate").val();

                //案件數(類別)
                SearchbyClass(startDate, endDate);

                //案件數(部門)
                SearchbyDept(startDate, endDate);

                //時數(類別)
                SearchHourByClass(startDate, endDate);

                //時數(結案人)
                SearchHourByWho(startDate, endDate);

                //滿意度
                SearchStars(startDate, endDate);
            });


            //Tabs切換偵測, 並載入圖表
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                //取得判別編號
                var dataID = $(e.target).attr('data-id');
                //取得日期參數值
                var startDate = $(".startDate").val();
                var endDate = $(".endDate").val();

                //依編號載入圖表
                switch (dataID) {
                    case "1":
                        //案件數(類別)
                        SearchbyClass(startDate, endDate);
                        break;

                    case "2":
                        //案件數(部門)
                        SearchbyDept(startDate, endDate);
                        break;

                    case "3":
                        //時數(類別)
                        SearchHourByClass(startDate, endDate);
                        break;

                    case "4":
                        //時數(結案人)
                        SearchHourByWho(startDate, endDate);
                        break;

                    case "5":
                        //滿意度
                        SearchStars(startDate, endDate);
                        break;
                }
            })

        })


        //-- 快查按鈕 --
        function setDate(dateType, searchType) {
            var thisDate = new Date();
            var sDate;
            var eDate = thisDate.getFullYear() + '/' + ('0' + (thisDate.getMonth() + 1)).slice(-2) + '/' + ('0' + thisDate.getDate()).slice(-2);

            //判斷是哪個快查鈕
            switch (dateType) {
                case "1":
                    //3個月, 以今日起算往前推
                    thisDate.setMonth(thisDate.getMonth() - 3);

                    break;

                case "2":
                    //半年, 以今日起算往前推
                    thisDate.setMonth(thisDate.getMonth() - 6);

                    break;

                case "3":
                    //一年, 以今日起算往前推
                    thisDate.setMonth(thisDate.getMonth() - 12);

                    break;
            }
            //設定啟始日
            sDate = thisDate.getFullYear() + '/' + ('0' + (thisDate.getMonth() + 1)).slice(-2) + '/' + ('0' + thisDate.getDate()).slice(-2);

            //填入日期
            $(".startDate").val(sDate);
            $(".endDate").val(eDate);

            //判斷是哪個查詢鈕
            switch (searchType) {
                case "CountByClass":
                    SearchbyClass(sDate, eDate);
                    break;

                case "CountByDept":
                    SearchbyDept(sDate, eDate);
                    break;

                case "HourByClass":   //時數(類別)
                    SearchHourByClass(sDate, eDate);
                    break;


                case "HourByWho":   //時數(結案人)
                    SearchHourByWho(sDate, eDate);
                    break;


                case "Stars":   //滿意度
                    SearchStars(sDate, eDate);
                    break;

                default:
                    alert('You get the wrong way.');
                    break;
            }
        }


        //-- 載入圖表 - 案件數(類別) --
        function SearchbyClass(StartDate, EndDate) {
            /* Pie圖 */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetCountbyClass',
                data: '{StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~' + EndDate;
                        //繪製圖表
                        drawChart_Pie(response.d, myTitle, 800, 500, 'Chart_Pie_byClass', '類別', '案件數');
                    }
            });
        }

        //-- 載入圖表 - 案件數(部門) --
        function SearchbyDept(StartDate, EndDate) {
            /* Pie圖 - by部門(TW) */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetCountbyDept',
                data: '{AreaCode:"TW", StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~ ' + EndDate;
                        //繪製圖表
                        drawChart_Pie(response.d, myTitle, 800, 500, 'Chart_Pie_byDept_TW', '部門', '案件數');
                    }
            });
            /* Pie圖 - by部門(SH) */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetCountbyDept',
                data: '{AreaCode:"SH", StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~ ' + EndDate;
                        //繪製圖表
                        drawChart_Pie(response.d, myTitle, 800, 500, 'Chart_Pie_byDept_SH', '部門', '案件數');
                    }
            });
            /* Pie圖 - by部門(SZ) */
            //$.ajax({
            //    type: 'POST',
            //    dataType: 'json',
            //    contentType: 'application/json',
            //    url: 'GetData.aspx/GetCountbyDept',
            //    data: '{AreaCode:"SZ", StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
            //    success:
            //        function (response) {
            //            var myTitle = StartDate + ' ~ ' + EndDate;
            //            //繪製圖表
            //            drawChart_Pie(response.d, myTitle, 800, 500, 'Chart_Pie_byDept_SZ', '部門', '案件數');
            //        }
            //});
        }

        //-- 載入圖表 - 時數(類別) --
        function SearchHourByClass(StartDate, EndDate) {
            /* Bar Chart */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetHourByClass',
                data: '{StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~' + EndDate;
                        //繪製圖表
                        drawChart_Bar(response.d, myTitle, 'BarChart_byHourClass', '類別', '時數(h)', '案件數', '平均(h)');
                    }
            });
        }

        //-- 載入圖表 - 時數(人員) --
        function SearchHourByWho(StartDate, EndDate) {
            /* Bar Chart (TW) */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetHourByWho',
                data: '{AreaCode:"TW", StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~' + EndDate;
                        //繪製圖表
                        drawChart_Bar(response.d, myTitle, 'BarChart_byHourWho_TW', '人員', '時數(h)', '案件數', '平均(h)');
                    }
            });

            /* Bar Chart (SH) */
            //$.ajax({
            //    type: 'POST',
            //    dataType: 'json',
            //    contentType: 'application/json',
            //    url: 'GetData.aspx/GetHourByWho',
            //    data: '{AreaCode:"SH", StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
            //    success:
            //        function (response) {
            //            var myTitle = StartDate + ' ~' + EndDate;
            //            //繪製圖表
            //            drawChart_Bar(response.d, myTitle, 'BarChart_byHourWho_SH', '人員', '時數(h)', '案件數', '平均(h)');
            //        }
            //});
        }

        //-- 載入圖表 - 滿意度 --
        function SearchStars(StartDate, EndDate) {
            /* Pie圖 */
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: 'GetData.aspx/GetRateStar',
                data: '{StartDate:"' + StartDate + '", EndDate:"' + EndDate + '"}',
                success:
                    function (response) {
                        var myTitle = StartDate + ' ~' + EndDate;
                        //繪製圖表
                        drawChart_Star_Bar(response.d, myTitle, 'PieChart_RateStar', '人員', '一星', '二星', '三星', '四星', '五星');
                        //drawChart_Pie(response.d, myTitle, 800, 500, 'PieChart_RateStar', '星級', '數量');
                    }
            });
        }


        //-- 繪製圖表 Start --
        /*
          [Pie Chart]
          dataValues : 資料值
          titleName : 圖表title
          width : 圖表寬度
          height : 圖表高度
          chartID : 圖表ID
          colName : 欄位名 - 類別
          valName : 欄位名 - 值

          ref:https://developers.google.com/chart/interactive/docs/gallery/piechart
        */
        function drawChart_Pie(dataValues, titleName, width, height, chartID, colName, valName) {
            var data = new google.visualization.DataTable();
            //新增欄
            data.addColumn('string', colName);
            data.addColumn('number', valName);

            //新增列值
            var total = 0;
            for (var i = 0; i < dataValues.length; i++) {
                data.addRow([dataValues[i].ColumnName, dataValues[i].Value]);

                //加總
                total = total + dataValues[i].Value;
            }

            //設定圖表選項
            var options = {
                'title': titleName,
                'width': width,
                'height': height,
                is3D: true
            };

            //載入圖表
            new google.visualization.PieChart(document.getElementById(chartID)).
                draw(data, options);

            //載入表格
            if (total > 0) {
                //新增合計列
                data.addRow(['Total', total]);
            }
            var table = new google.visualization.Table(document.getElementById('table_' + chartID));
            table.draw(data, { 'width': '400px' });

        }

        /*
          [Bar Chart]
          dataValues : 資料值
          titleName : 圖表title
          width : 圖表寬度
          height : 圖表高度
          chartID : 圖表ID
          colName : 欄位名 - 類別
          valName : 欄位名 - 值

          ref:https://developers.google.com/chart/interactive/docs/gallery/barchart
        */
        function drawChart_Bar(dataValues, titleName, chartID, colName, valName, valName1, valName2) {
            var data = new google.visualization.DataTable();
            //新增欄
            data.addColumn('string', colName);
            data.addColumn('number', valName);  //時數
            data.addColumn('number', valName1); //案件數
            data.addColumn('number', valName2); //平均

            //新增列值
            var total1 = 0;
            var total2 = 0;
            var total3 = 0;
            for (var i = 0; i < dataValues.length; i++) {
                data.addRow([dataValues[i].ColumnName, dataValues[i].Value, dataValues[i].Value1, dataValues[i].Value2]);

                //加總
                total1 = total1 + dataValues[i].Value;
                total2 = total2 + dataValues[i].Value1;
                total3 = total3 + dataValues[i].Value2;
            }

            //計算高度
            // set inner height to 30 pixels per row
            var chartAreaHeight = data.getNumberOfRows() * 40;
            // add padding to outer height to accomodate title, axis labels, etc
            var chartHeight = chartAreaHeight + 300;

            //設定圖表選項(Material Bar Charts)
            var options = {
                height: chartHeight,
                legend: { position: 'in' },
                chart: {
                    title: titleName
                    //,subtitle: 'popularity by percentage'
                },
                chartArea: { width: '100%', height: chartAreaHeight },
                bars: 'horizontal' // Required for Material Bar Charts.
            };

            //設定圖表選項(傳統)
            //var options = {
            //    title: titleName,
            //    height: chartHeight,
            //    bar: { groupWidth: "95%" }
            //};


            //載入圖表
            //ref:https://developers.google.com/chart/interactive/docs/gallery/barchart#loading
            //For Material Bar Charts, the visualization's class name is google.charts.Bar.
            //var chart = new google.charts.Bar(document.getElementById(chartID));
            //chart.draw(data, options);
       
            //一般barchart
            //new google.visualization.BarChart(document.getElementById(chartID)).
            //   draw(data, options);
            new google.charts.Bar(document.getElementById(chartID)).
               draw(data, options);
           
            if (total1 > 0) {
                var avg = Math.round((total1 / total2) * 100) / 100;
                //新增合計列
                data.addRow(['Total', total1, total2, avg]);
            }
            var table = new google.visualization.Table(document.getElementById('table_' + chartID));
            table.draw(data, { 'width': '400px' });
         
        }


        /*
          [Bar Chart] 滿意度 by人
        */
        function drawChart_Star_Bar(dataValues, titleName, chartID, colName, valName, valName1, valName2, valName3, valName4) {
            var data = new google.visualization.DataTable();
            //新增欄
            data.addColumn('string', colName);
            data.addColumn('number', valName);  //1
            data.addColumn('number', valName1); //2
            data.addColumn('number', valName2); //3
            data.addColumn('number', valName3); //4
            data.addColumn('number', valName4); //5

            //新增列值
            //var total1 = 0;
            //var total2 = 0;
            //var total3 = 0;
            //var total4 = 0;
            //var total5 = 0;
            for (var i = 0; i < dataValues.length; i++) {
                data.addRow([dataValues[i].ColumnName, dataValues[i].Value
                    , dataValues[i].Value1
                    , dataValues[i].Value2
                    , dataValues[i].Value3
                    , dataValues[i].Value4
                    ]);

                //加總
                //total1 = total1 + dataValues[i].Value;
                //total2 = total2 + dataValues[i].Value1;
                //total3 = total3 + dataValues[i].Value2;
                //total4 = total4 + dataValues[i].Value3;
                //total5 = total5 + dataValues[i].Value4;
            }

            //計算高度
            // set inner height to 30 pixels per row
            var chartAreaHeight = data.getNumberOfRows() * 40;
            // add padding to outer height to accomodate title, axis labels, etc
            var chartHeight = chartAreaHeight + 300;

            //設定圖表選項(Material Bar Charts)
            var options = {
                height: chartHeight,
                legend: { position: 'in' },
                chart: {
                    title: titleName
                    //,subtitle: 'popularity by percentage'
                },
                chartArea: { width: '100%', height: chartAreaHeight },
                bars: 'horizontal' // Required for Material Bar Charts.
            };

            //設定圖表選項(傳統)
            //var options = {
            //    title: titleName,
            //    height: chartHeight,
            //    bar: { groupWidth: "95%" }
            //};


            //載入圖表
            //ref:https://developers.google.com/chart/interactive/docs/gallery/barchart#loading
            //For Material Bar Charts, the visualization's class name is google.charts.Bar.
            //var chart = new google.charts.Bar(document.getElementById(chartID));
            //chart.draw(data, options);

            //一般barchart
            //new google.visualization.BarChart(document.getElementById(chartID)).
            //   draw(data, options);
            new google.charts.Bar(document.getElementById(chartID)).
               draw(data, options);

            //if (total1 > 0) {
            //    //新增合計列
            //    data.addRow(['Total', total1, total2, total3, total4, total5]);
            //}
            var table = new google.visualization.Table(document.getElementById('table_' + chartID));
            table.draw(data, { 'width': '400px' });

        }

        //-- 繪製圖表 End --
    </script>
    <!-- Google Chart End -->
</head>
<body class="MainArea">
    <form id="form1" runat="server">
        <div class="Navi">
            <a>
                <%=Application["WebName"]%></a>&gt;<a>資訊服務</a>&gt;<span>資訊需求統計</span>
        </div>
        <div class="h2Head">
            <h2>資訊需求統計</h2>
        </div>
        <div class="Sift">
            <ul>
                <li>登記日期區間：
                    <input type="text" class="startDate styleBlack" style="text-align: center; width: 90px;" />
                    ~
                    <input type="text" class="endDate styleBlack" style="text-align: center; width: 90px;" />
                    &nbsp;&nbsp;&nbsp;
                    <input type="button" id="doSearch" class="btnBlock colorGray" value="查詢" />
                    &nbsp;&nbsp;&nbsp;<a href="../AirMIS/ITHelp_Search.aspx">返回資訊需求</a>
                </li>
            </ul>
        </div>

        <!-- Tabs Start -->
        <div style="margin-top: 10px;">
            <ul id="myTabs" class="nav nav-tabs" role="tablist">
                <li class="active"><a href="#CountByClass" role="tab" data-toggle="tab" data-id="1">案件數(類別)</a></li>
                <li><a href="#CountByDept" role="tab" data-toggle="tab" data-id="2">案件數(部門)</a></li>
                <li><a href="#HourByClass" role="tab" data-toggle="tab" data-id="3">時數(類別)</a></li>
                <li><a href="#HourByWho" role="tab" data-toggle="tab" data-id="4">時數(結案人)</a></li>
                <li><a href="#Stars" role="tab" data-toggle="tab" data-id="5">滿意度</a></li>
            </ul>
            <div class="tab-content">
                <!-- 案件數(類別) -->
                <div class="tab-pane fade in active" id="CountByClass">
                    <div style="margin-top: 10px;">
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-primary" disabled="disabled">快速查詢</button>
                            <button type="button" class="btn btn-default" onclick="setDate('1', 'CountByClass');">3個月</button>
                            <button type="button" class="btn btn-default" onclick="setDate('2', 'CountByClass');">半 年</button>
                            <button type="button" class="btn btn-default" onclick="setDate('3', 'CountByClass');">一 年</button>
                        </div>
                    </div>
                    <div class="bq-callout blue">
                        <h4>案件數統計(依類別)</h4>
                        <div class="form-group">
                            <div id="Chart_Pie_byClass"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_Chart_Pie_byClass"></div>
                        </div>
                    </div>
                </div>

                <!-- 案件數(部門) -->
                <div class="tab-pane fade" id="CountByDept">
                    <div style="margin-top: 10px;">
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-primary" disabled="disabled">快速查詢</button>
                            <button type="button" class="btn btn-default" onclick="setDate('1', 'CountByDept');">3個月</button>
                            <button type="button" class="btn btn-default" onclick="setDate('2', 'CountByDept');">半 年</button>
                            <button type="button" class="btn btn-default" onclick="setDate('3', 'CountByDept');">一 年</button>
                        </div>
                    </div>
                    <div class="bq-callout blue">
                        <h4>案件數統計(依部門) - 台灣</h4>
                        <div class="form-group">
                            <div id="Chart_Pie_byDept_TW"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_Chart_Pie_byDept_TW"></div>
                        </div>
                    </div>
                    <%--<div class="bq-callout orange">
                        <h4>案件數統計(依部門) - 深圳</h4>
                        <div class="form-group">
                            <div id="Chart_Pie_byDept_SZ"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_Chart_Pie_byDept_SZ"></div>
                        </div>
                    </div>--%>
                    <div class="bq-callout green">
                        <h4>案件數統計(依部門) - 上海</h4>
                        <div class="form-group">
                            <div id="Chart_Pie_byDept_SH"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_Chart_Pie_byDept_SH"></div>
                        </div>
                    </div>
                </div>

                <!-- 時數(類別) -->
                <div class="tab-pane fade" id="HourByClass">
                    <div style="margin-top: 10px;">
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-primary" disabled="disabled">快速查詢</button>
                            <button type="button" class="btn btn-default" onclick="setDate('1', 'HourByClass');">3個月</button>
                            <button type="button" class="btn btn-default" onclick="setDate('2', 'HourByClass');">半 年</button>
                            <button type="button" class="btn btn-default" onclick="setDate('3', 'HourByClass');">一 年</button>
                        </div>
                    </div>
                    <div class="bq-callout red">
                        <h4>時數統計(依類別)</h4>
                        <div class="form-group">
                            <div id="BarChart_byHourClass" style="margin: 20px 20px;"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_BarChart_byHourClass"></div>
                        </div>
                    </div>
                </div>

                <!-- 時數(結案人) -->
                <div class="tab-pane fade" id="HourByWho">
                    <div style="margin-top: 10px;">
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-primary" disabled="disabled">快速查詢</button>
                            <button type="button" class="btn btn-default" onclick="setDate('1', 'HourByWho');">3個月</button>
                            <button type="button" class="btn btn-default" onclick="setDate('2', 'HourByWho');">半 年</button>
                            <button type="button" class="btn btn-default" onclick="setDate('3', 'HourByWho');">一 年</button>
                        </div>
                    </div>
                    <div class="bq-callout red">
                        <h4>時數統計(結案人)</h4>
                        <div class="form-group">
                            <div id="BarChart_byHourWho_TW" style="margin: 20px 20px;"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_BarChart_byHourWho_TW"></div>
                        </div>
                    </div>
                    <%--<div class="bq-callout orange">
                        <h4>時數統計(結案人) - 深圳</h4>
                        <div class="form-group">
                            <div id="BarChart_byHourWho_SZ" style="margin: 20px 20px;"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_BarChart_byHourWho_SZ"></div>
                        </div>
                    </div>--%>
                    <%--<div class="bq-callout green">
                        <h4>時數統計(結案人) - 上海</h4>
                        <div class="form-group">
                            <div id="BarChart_byHourWho_SH" style="margin: 20px 20px;"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_BarChart_byHourWho_SH"></div>
                        </div>
                    </div>--%>
                </div>

                <!-- 滿意度 -->
                <div class="tab-pane fade" id="Stars">
                    <div style="margin-top: 10px;">
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-primary" disabled="disabled">快速查詢</button>
                            <button type="button" class="btn btn-default" onclick="setDate('1', 'Stars');">3個月</button>
                            <button type="button" class="btn btn-default" onclick="setDate('2', 'Stars');">半 年</button>
                            <button type="button" class="btn btn-default" onclick="setDate('3', 'Stars');">一 年</button>
                        </div>
                    </div>
                    <div class="bq-callout purple">
                        <h4>滿意度分析</h4>
                        <div class="form-group">
                            <div id="PieChart_RateStar"></div>
                        </div>
                        <div class="form-group">
                            <div id="table_PieChart_RateStar"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Tabs End -->

        <!-- Scroll Top 按扭 Start -->
        <a href="#" class="scrollup">Scroll</a>
        <!-- Scroll Top 按扭 End -->
    </form>
</body>
</html>
