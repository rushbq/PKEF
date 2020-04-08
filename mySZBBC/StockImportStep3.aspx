<%@ Page Title="電商庫存 | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="StockImportStep3.aspx.cs" Inherits="mySZBBC_StockImportStep3" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">Step3 - 確認匯入資料</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳BBC</a></li>
                        <li><a>電商庫存-匯入Excel</a></li>
                        <li class="active">Step3</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <asp:PlaceHolder ID="ph_Message" runat="server">
            <div class="card-panel red darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>Oops...發生了一點小問題</h4>
                <p>若持續看到此訊息, 請回報錯誤發生的 <strong class="flow-text">詳細狀況</strong> 及 <strong class="flow-text">追蹤編號</strong>。</p>
                <p>
                    <asp:Literal ID="lt_Msg" runat="server"></asp:Literal>
                </p>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
                <div class="row">
                    <div class="col s12 grey lighten-5">
                        <i class="material-icons">flag</i>&nbsp;
                        目前商城為<span class="orange-text text-darken-2 flow-text"><b><asp:Literal ID="lt_MallName" runat="server"></asp:Literal></b></span>
                    </div>
                </div>
                <div class="section">
                    <div class="divider"></div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label class="green-text text-darken-1 flow-text"><i class="material-icons">playlist_add_check</i> 已匯入</label>
                        <div>
                            <asp:ListView ID="lvDataList_Y" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <table id="listTable" class="myTable stripe" cellspacing="0" width="100%" style="width: 100%;">
                                        <thead>
                                            <tr>
                                                <th>SKU</th>
                                                <th>品號</th>
                                                <th style="text-align: center;">庫存</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("ProdID") %></td>
                                        <td><%#Eval("ERP_ModelNo") %></td>
                                        <td style="text-align: center;"><%#Eval("StockNum") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>

                <div class="section row">
                    <div class="col s12">
                        <label class="red-text text-darken-1 flow-text"><i class="material-icons">clear</i> 有問題未匯入</label>
                        <div>
                            <asp:ListView ID="lvDataList_N" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <table id="denyTable" class="myTable stripe" cellspacing="0" width="100%" style="width: 100%;">
                                        <thead>
                                            <tr>
                                                <th>SKU</th>
                                                <th>品號</th>
                                                <th style="text-align: center;">庫存</th>
                                                <th>原因</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("ProdID") %></td>
                                        <td><%#Eval("ERP_ModelNo") %></td>
                                        <td style="text-align: center;"><%#Eval("StockNum") %></td>
                                        <td class="orange-text text-darken-4"><%#Eval("doWhat") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Hidden Field -->
            <asp:HiddenField ID="hf_MallID" runat="server" />
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
    <asp:PlaceHolder ID="ph_Buttons" runat="server">
        <div class="container">
            <div class="section row">
                <div class="col s6">
                    <a class="btn-large waves-effect waves-light grey" href="<%=fn_Params.WebUrl %>mySZBBC/StockImportStep2.aspx?dataID=<%=Req_DataID %>">上一步，選擇工作表<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                    <a href="<%=fn_Params.WebUrl %>mySZBBC/StockDataList.aspx" class="btn-large waves-effect waves-light blue">離開此頁，前往列表</a>
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
            //使用DataTable
            $('#listTable').DataTable({
                "searching": false,  //搜尋
                "ordering": false,   //排序
                "paging": true,     //分頁
                "info": false,      //頁數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                "pageLength": 20,   //顯示筆數預設值     
                //捲軸設定
                "scrollY": '50vh',
                "scrollCollapse": true,
                "scrollX": true
            });

            $('#denyTable').DataTable({
                "searching": false,  //搜尋
                "ordering": false,   //排序
                "paging": true,     //分頁
                "info": false,      //頁數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                "pageLength": 20,   //顯示筆數預設值     
                //捲軸設定
                "scrollY": '50vh',
                "scrollCollapse": true,
                "scrollX": true
            });
        });
    </script>
    <%-- DataTable End --%>
    <style>
        .myTable td {
            word-break: keep-all;
            word-wrap: break-word;
        }
    </style>
</asp:Content>

