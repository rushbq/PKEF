<%@ Page Title="上海會計 | 百旺開票-紙本發票" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="BatchStep1.aspx.cs" Inherits="mySHInvoice_BatchStep1" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">多筆新增 - 條件篩選</h5>
                    <ol class="breadcrumb">
                        <li><a>上海會計</a></li>
                        <li><a href="<%=fn_Params.WebUrl %>mySHInvoice/List.aspx">百旺開票-紙本發票</a></li>
                        <li class="active">多筆新增</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <asp:PlaceHolder ID="ph_Message" runat="server" Visible="false">
            <div class="card-panel red darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>Oops...發生了一點小問題</h4>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
                <div class="section row">
                    <div class="col s12">
                        <div class="input-field inline">
                            <i class="material-icons prefix">today</i>
                            <asp:TextBox ID="filter_sDate" runat="server" CssClass="datepicker"></asp:TextBox>
                            <label for="MainContent_filter_sDate">結帳日-起</label>
                        </div>
                        <div class="input-field inline">
                            <i class="material-icons prefix">today</i>
                            <asp:TextBox ID="filter_eDate" runat="server" CssClass="datepicker"></asp:TextBox>
                            <label for="MainContent_filter_eDate">結帳日-訖</label>
                        </div>
                    </div>
                    <div class="row col s12 right-align">
                        <asp:LinkButton ID="btn_GetData" runat="server" CssClass="btn waves-effect waves-light green" OnClick="btn_GetData_Click">帶出要匯入客戶</asp:LinkButton>
                    </div>
                </div>
                <div class="row">
                    <div class="col s12">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="listTable" class="stripe">
                                    <thead>
                                        <tr>
                                            <th class="no-sort center-align">
                                                <input type="checkbox" id="cbx_All" />
                                                <label for="cbx_All"></label>
                                            </th>
                                            <th>客戶代號</th>
                                            <th>客戶名稱</th>
                                            <th>金額</th>
                                        </tr>
                                        <tbody>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tbody>
                                    </thead>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="center-align">
                                        <input type="checkbox" id="cbx_<%#Eval("CustID") %>" class="myCbx" value="<%#Eval("CustID") %>" />
                                        <label for="cbx_<%#Eval("CustID") %>"></label>
                                    </td>
                                    <td>
                                        <%#Eval("CustID") %>
                                    </td>
                                    <td>
                                        <%#Eval("CustName") %>
                                    </td>
                                    <td>
                                        <%# Math.Round(Convert.ToDouble(Eval("TotalPrice")), 2) %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>注意事項</label>
                        <div>
                            <ul class="collection">
                                <li class="collection-item"><i class="material-icons left">info</i>客戶的「發票類型」請先至基本資料維護 -&gt; 客戶基本資料, 進行設定</li>
                                <li class="collection-item"><i class="material-icons left">info</i>按下「綠色鈕」，會帶出區間內的內客，此時請勾選要匯入的客戶。</li>
                                <li class="collection-item"><i class="material-icons left">info</i>「下一步」將會依勾選的客戶批次新增，此時狀態為「未完成轉入」並返回列表頁。</li>
                                <li class="collection-item"><i class="material-icons left">info</i>列表頁會出現主旨「批次-」開頭的待轉入資料，請選擇要轉入的資料。</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
    <asp:PlaceHolder ID="ph_Buttons" runat="server">
        <div class="container">
            <div class="section row">
                <div class="col s6">
                    <a href="<%=fn_Params.WebUrl %>mySHInvoice/List.aspx" class="btn-large waves-effect waves-light grey">取消，返回列表<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                    <div id="showProcess" class="progress" style="display: none;">
                        <div class="indeterminate"></div>
                    </div>
                    <a href="#!" id="trigger-Next" class="btn-large waves-effect waves-light blue">下一步，批次產生資料<i class="material-icons right">chevron_right</i></a>
                    <asp:Button ID="btn_Next" runat="server" Text="Next" OnClick="btn_Next_Click" Style="display: none;" />
                    <asp:TextBox ID="tb_CbxValues" runat="server" Style="display: none;"></asp:TextBox>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script src="<%=fn_Params.CDNUrl %>plugin/Materialize/v0.97.8/lib/pickadate/translation/zh_TW.js"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 2, // Creates a dropdown of 15 years to control year
                format: 'yyyy/mm/dd',

                closeOnSelect: false // Close upon selecting a date(此版本無作用)
            }).on('change', function () {
                $(this).next().find('.picker__close').click();
            });


            //trigger Next button
            $("#trigger-Next").click(function () {
                //取得已勾選
                var s = $('input:checkbox:checked.myCbx').map(function () { return $(this).val(); }).get();

                //Check
                if (s.length == 0) {
                    alert('未勾選客戶');
                    return false;
                }

                //填入勾選值
                $("#BottomContent_tb_CbxValues").val(s.join(','));

                //hide button
                $(this).hide();
                //show loading
                $("#showProcess").show();
                //trigger server side button
                $("#BottomContent_btn_Next").trigger("click");
            });

            //Checkbox checked/unchecked all
            $('#cbx_All').click(function () {
                $('input:checkbox').prop('checked', this.checked);
            });

        });
    </script>
    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script>
        $(function () {
            /*
             [使用DataTable]
             注意:標題欄須與內容欄數量相等
           */
            var table = $('#listTable').DataTable({
                "searching": false,  //搜尋
                "ordering": true,   //排序
                "paging": false,     //分頁
                "info": false,      //筆數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                //讓不排序的欄位在初始化時不出現排序圖
                "order": [],
                //自訂欄位
                "columnDefs": [{
                    "targets": 'no-sort',
                    "orderable": false,
                }],
                //捲軸設定
                "scrollY": '50vh',
                "scrollCollapse": true,
                "scrollX": false

            });

        });

    </script>
    <%-- DataTable End --%>
</asp:Content>
