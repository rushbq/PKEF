<%@ Page Title="Chart DEMO" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="_TestChart.aspx.cs" Inherits="Recording_TestChart" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">PK</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Charts
                    </div>
                </div>
            </div>
            <div class="right menu">
            </div>
        </div>
    </div>
    <!-- 工具列 End -->
    <div class="myContentBody">
        <div class="ui segment grey-bg lighten-5">
            <div class="ui segments">
                <div class="ui green segment">
                    <h5 class="ui header">案件數統計(需求資源)</h5>
                </div>
                <div class="ui attached segment">
                    <div class="ui grid">
                        <div class="row">
                            <div class="ten wide column">
                                <div id="Chart_Pie_byClass"></div>
                            </div>
                            <div class="six wide column">
                                <div id="table_Chart_Pie_byClass" class="ui celled striped small table"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="ui segments">
                <div class="ui green segment">
                    <h5 class="ui header">時數統計(結案人)</h5>
                </div>
                <div class="ui attached segment">
                    <div class="ui grid">
                        <div class="row">
                            <div class="ten wide column">
                                <div id="BarChart_byHourWho"></div>
                            </div>
                            <div class="six wide column">
                                <div id="table_BarChart_byHourWho" class="ui celled striped small table"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- Google Chart Start -->
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">
        //load google charts
        google.charts.load("current", { packages: ['corechart', 'table', 'bar'], 'callback': drawCharts });

        //預設載入的圖表, 此function必須與callback宣告放在同一個script
        function drawCharts() {
            //填入預設日期(當年開始~今日)
            var thisDate = new Date();
            var defStartDate = thisDate.getFullYear() + '/01/01';
            var defEndDate = thisDate.getFullYear() + '/' + ('0' + (thisDate.getMonth() + 1)).slice(-2) + '/' + ('0' + thisDate.getDate()).slice(-2);

            //載入圖表 - 案件數(類別)
            SearchbyClass(defStartDate, defEndDate);
            //載入圖表 - 時數(人員)
            SearchHourByWho(defStartDate, defEndDate);
        }
    </script>

    <script>
        //-- 載入圖表 - 案件數(類別) --
        function SearchbyClass(StartDate, EndDate) {
            /* Pie圖 */
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
                        drawChart_Pie(response.d, myTitle, 900, 500, 'Chart_Pie_byClass', '類別', '案件數', '時數');
                    }
            });
        }

        //-- 載入圖表 - 時數(人員) --
        function SearchHourByWho(StartDate, EndDate) {
            /* Bar Chart */
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
                        drawChart_Bar(response.d, myTitle, 'BarChart_byHourWho', '人員', '時數(h)', '案件數');
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
        function drawChart_Pie(_dataValues, _titleName, _width, _height, _chartID, _colName, _valName, _valName1) {
            var data = new google.visualization.DataTable();
            var data_table = new google.visualization.DataTable();

            //新增欄-Chart
            data.addColumn('string', _colName);
            data.addColumn('number', _valName);

            //新增欄-Table
            data_table.addColumn('string', _colName);
            data_table.addColumn('number', _valName);
            data_table.addColumn('number', _valName1);

            //新增列值
            //var total = 0;
            for (var i = 0; i < _dataValues.length; i++) {
                var dtCol = _dataValues[i].ColumnName;
                var dtVal_cnt = _dataValues[i].Value1;
                var dtVal_hour = _dataValues[i].Value;

                //AddRow-Chart
                data.addRow([dtCol, dtVal_cnt]);

                //AddRow-Table
                data_table.addRow([dtCol, dtVal_cnt, dtVal_hour]);

                //加總
                //total = total + dtCol;
            }

            //設定圖表選項
            var options = {
                title: _titleName,
                width: _width,
                height: _height,
                is3D: true,
            };

            //載入圖表
            new google.visualization.PieChart(document.getElementById(_chartID)).
                draw(data, options);

            //載入表格
            //if (total > 0) {
            //    //新增合計列
            //    data.addRow(['Total', total]);
            //}
            var table = new google.visualization.Table(document.getElementById('table_' + _chartID));
            table.draw(data_table, { showRowNumber: false, width: '100%', height: '100%' });

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
        function drawChart_Bar(dataValues, titleName, chartID, colName, valName, valName1) {
            var data = new google.visualization.DataTable();
            //新增欄
            data.addColumn('string', colName);
            data.addColumn('number', valName);  //時數
            data.addColumn('number', valName1); //案件數

            //新增列值
            //var total1 = 0;
            //var total2 = 0;
            for (var i = 0; i < dataValues.length; i++) {
                data.addRow([dataValues[i].ColumnName, dataValues[i].Value, dataValues[i].Value1]);

                ////加總
                //total1 = total1 + dataValues[i].Value;
                //total2 = total2 + dataValues[i].Value1;
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
                bars: 'vertical' //(vertical/horizontal) Required for Material Bar Charts.
            };

            //載入圖表
            //ref:https://developers.google.com/chart/interactive/docs/gallery/barchart#loading
            new google.charts.Bar(document.getElementById(chartID)).
               draw(data, options);

            //if (total1 > 0) {
            //    //新增合計列
            //    tableData.addRow(['Total', total1, total2]);
            //}
            var table = new google.visualization.Table(document.getElementById('table_' + chartID));
            table.draw(data, { showRowNumber: false, width: '100%', height: '100%' });

        }

        //-- 繪製圖表 End --
    </script>
</asp:Content>

