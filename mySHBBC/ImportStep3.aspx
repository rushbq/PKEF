<%@ Page Title="上海BBC | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportStep3.aspx.cs" Inherits="mySHBBC_ImportStep3" %>

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
                        <li><a>上海BBC</a></li>
                        <li><a>匯入Excel</a></li>
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
                <p><a class="btn waves-effect waves-light grey darken-1" href="<%=fn_Params.WebUrl %>mySHBBC/ImportLog.aspx?dataID=<%=Req_DataID %>#log">或點此查看記錄</a></p>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
                <div class="row">
                    <div class="col s12 grey lighten-5">
                        <i class="material-icons">flag</i>&nbsp;
                        目前匯入模式為<span class="orange-text text-darken-2 flow-text"><b><asp:Literal ID="lt_TypeName" runat="server"></asp:Literal></b></span>
                        , 商城為<span class="orange-text text-darken-2 flow-text"><b><asp:Literal ID="lt_MallName" runat="server"></asp:Literal></b></span>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s6">
                        <label>追蹤編號</label>
                        <div class="red-text text-darken-2 flow-text">
                            <strong>
                                <asp:Literal ID="lt_TraceID" runat="server"></asp:Literal></strong>
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
                <div class="section">
                    <div class="divider"></div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label class="green-text text-darken-1 flow-text"><i class="material-icons">playlist_add_check</i> 即將匯入</label>
                        <div>
                            <asp:ListView ID="lvDataList_Y" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_Y_ItemDataBound">
                                <LayoutTemplate>
                                    <table id="listTable" class="myTable stripe" cellspacing="0" width="100%" style="width: 100%;">
                                        <thead>
                                            <tr>
                                                <th>單號</th>
                                                <th>商品編號</th>
                                                <th>購買量</th>
                                                <th>內銷MOQ</th>
                                                <th></th>
                                                <th>購買金額</th>
                                                <th>總金額</th>
                                                <th>運費</th>
                                                <th>ERP品號</th>
                                                <th>ERP價格</th>
                                                <th>收貨人</th>
                                                <th>收貨地址</th>
                                                <th>收貨電話</th>
                                                <th>贈品</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr class="<%#Eval("IsGift").Equals("Y")?"orange-text text-darken-2":"" %>">
                                        <td><%#Eval("OrderID") %></td>
                                        <td><%#Eval("ProdID") %></td>
                                        <td style="text-align: center;"><%#Eval("BuyCnt") %></td>
                                        <td style="text-align: center;"><%#Eval("inMOQ") %></td>
                                        <td>
                                            <asp:PlaceHolder ID="ph_check" runat="server" Visible="false">
                                                <label class="red-text text-darken-2 flow-text" title="購買量不足"><i class="material-icons">error</i></label>
                                            </asp:PlaceHolder>
                                        </td>
                                        <td style="text-align: right;"><%#Eval("BuyPrice") %></td>
                                        <td style="text-align: right;"><%#Eval("TotalPrice") %></td>
                                        <td style="text-align: right;"><%#Eval("Freight") %></td>
                                        <td><b><%#Eval("ERP_ModelNo") %></b></td>
                                        <td style="text-align: right;"><b><%#Eval("ERP_Price") %></b></td>
                                        <td><%#Eval("ShipWho") %></td>
                                        <td><%#Eval("ShipAddr") %></td>
                                        <td><%#Eval("ShipTel") %></td>
                                        <td style="text-align: center;"><%#Eval("IsGift") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>

                <div class="section row">
                    <div class="col s12">
                        <label class="red-text text-darken-1 flow-text"><i class="material-icons">clear</i> 不會匯入</label>
                        <div>
                            <asp:ListView ID="lvDataList_N" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <table id="denyTable" class="myTable stripe" cellspacing="0" width="100%" style="width: 100%;">
                                        <thead>
                                            <tr>
                                                <th>單號</th>
                                                <th>商品編號</th>
                                                <th>購買量</th>
                                                <th>購買金額</th>
                                                <th>總金額</th>
                                                <th>運費</th>
                                                <th>ERP品號</th>
                                                <th>ERP價格</th>
                                                <th>收貨人</th>
                                                <th>收貨地址</th>
                                                <th>收貨電話</th>
                                                <th>贈品</th>
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
                                        <td><%#Eval("OrderID") %></td>
                                        <td><%#Eval("ProdID") %></td>
                                        <td style="text-align: center;"><%#Eval("BuyCnt") %></td>
                                        <td style="text-align: right;"><%#Eval("BuyPrice") %></td>
                                        <td style="text-align: right;"><%#Eval("TotalPrice") %></td>
                                        <td style="text-align: right;"><%#Eval("Freight") %></td>
                                        <td><b><%#Eval("ERP_ModelNo") %></b></td>
                                        <td style="text-align: right;"><%#Eval("ERP_Price") %></td>
                                        <td><%#Eval("ShipWho") %></td>
                                        <td><%#Eval("ShipAddr") %></td>
                                        <td><%#Eval("ShipTel") %></td>
                                        <td style="text-align: center;"><%#Eval("IsGift") %></td>
                                        <td class="orange-text text-darken-4"><%#Eval("doWhat") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Hidden Field -->
            <asp:HiddenField ID="hf_TraceID" runat="server" />
            <asp:HiddenField ID="hf_Type" runat="server" />
            <asp:HiddenField ID="hf_MallID" runat="server" />
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
    <asp:PlaceHolder ID="ph_Buttons" runat="server">
        <div class="container">
            <div class="section row">
                <div class="col s7">
                    <a class="btn-large waves-effect waves-light grey" href="<%=fn_Params.WebUrl %>mySHBBC/ImportStep2.aspx?dataID=<%=Req_DataID %>">上一步，選擇工作表<i class="material-icons left">arrow_back</i></a>
                    <asp:PlaceHolder ID="ph_step" runat="server" Visible="false">
                        <a class="btn-large waves-effect waves-light grey" href="<%=fn_Params.WebUrl %>mySHBBC/ImportStep2-1.aspx?dataID=<%=Req_DataID %>">上一步，Excel資料整理<i class="material-icons left">arrow_back</i></a>
                    </asp:PlaceHolder>
                </div>
                <div class="col s5 right-align">
                    <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Next_Click" ValidationGroup="Next">下一步，EDI匯入<i class="material-icons right">chevron_right</i></asp:LinkButton>
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
                "info": true,      //頁數資訊
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

