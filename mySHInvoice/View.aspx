<%@ Page Title="上海會計 | 百旺開票-紙本發票" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="View.aspx.cs" Inherits="mySHInvoice_View" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">檢視明細</h5>
                    <ol class="breadcrumb">
                        <li><a>上海會計</a></li>
                        <li><a href="<%=ListUrl %>">百旺開票-紙本發票</a></li>
                        <li class="active">檢視明細</li>
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
                <p>
                    <asp:Literal ID="lt_ErrMsg" runat="server"></asp:Literal>
                </p>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
                <div class="section row">
                    <div class="col s6">
                        <label>系統編號</label>
                        <div class="red-text text-darken-2 flow-text">
                            <strong>
                                <asp:Literal ID="lt_Inv_UID" runat="server"></asp:Literal></strong>
                        </div>
                    </div>
                    <div class="col s6">
                        <label>客戶</label>
                        <div class="green-text text-darken-2 flow-text">
                            <strong>
                                <asp:Literal ID="lt_CustName" runat="server"></asp:Literal></strong>
                        </div>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s6">
                        <label>發票類型</label>
                        <div>
                            <asp:Literal ID="lt_InvType" runat="server"></asp:Literal>
                        </div>
                    </div>
                    <div class="col s6">
                        <label>购方名称</label>
                        <div>
                            <asp:Literal ID="lt_vendeename" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col s12">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="listTable" class="stripe">
                                    <thead>
                                        <tr>
                                            <th>品號</th>
                                            <th>品名</th>
                                            <th>數量</th>
                                            <th>單位</th>
                                            <th>單價</th>
                                            <th style="width: 100px;">金額</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <th colspan="5" style="text-align: right">本頁小計:<br />
                                                <span class="blue-text text-darken-2">總計:</span></th>
                                            <th style="text-align: right"></th>
                                        </tr>
                                    </tfoot>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <strong><%#Eval("model") %></strong>
                                    </td>
                                    <td>
                                        <%#Eval("tradename") %>
                                    </td>
                                    <td>
                                        <%#Eval("qty") %>
                                    </td>
                                    <td>
                                        <%#Eval("unit") %>
                                    </td>
                                    <td>
                                        <%# Math.Round(Convert.ToDouble(Eval("price")), 2) %>
                                    </td>
                                    <td style="text-align: right">
                                        <%#Eval("shpamt") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
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
                    <a href="<%=ListUrl %>" class="btn-large waves-effect waves-light grey">返回列表<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
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
                "ordering": false,   //排序
                "paging": true,     //分頁
                "pageLength": 10,
                "info": true,      //筆數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                "footerCallback": function (row, data, start, end, display) {
                    var api = this.api(), data;

                    // Remove the formatting to get integer data for summation
                    var intVal = function (i) {
                        return typeof i === 'string' ?
                            i.replace(/[\$,]/g, '') * 1 :
                            typeof i === 'number' ?
                            i : 0;
                    };

                    // 計算總金額
                    total = api
                        .column(5)
                        .data()
                        .reduce(function (a, b) {
                            return Math.round((intVal(a) + intVal(b)) * 100) / 100;
                        }, 0);
                  
                    // 計算每頁小計
                    pageTotal = api
                        .column(5, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return Math.round((intVal(a) + intVal(b)) * 100) / 100;
                        }, 0);
                  
                    // Update footer
                    $(api.column(5).footer()).html(
                        pageTotal + '<br><span class="blue-text text-darken-2">' + total + '</span>'
                    );
                   
                }

            });

        });

    </script>
    <%-- DataTable End --%>
</asp:Content>
