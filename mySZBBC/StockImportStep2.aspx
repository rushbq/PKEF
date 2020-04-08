<%@ Page Title="電商庫存 | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="StockImportStep2.aspx.cs" Inherits="mySZBBC_StockImportStep2" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">Step2 - 選擇工作表</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳BBC</a></li>
                        <li><a>電商庫存-匯入Excel</a></li>
                        <li class="active">Step2</li>
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
                <p>若持續看到此訊息, 請回報錯誤發生的 <strong class="flow-text">詳細狀況</strong></p>
                <p>
                    <asp:Literal ID="lt_Msg" runat="server"></asp:Literal></p>
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
                <div class="section row">
                    <div class="col s12">
                        <label>選擇工作表&nbsp;<asp:RequiredFieldValidator ID="rfv_ddl_Sheet" runat="server" ErrorMessage="此為必填欄位「選擇工作表」" ControlToValidate="ddl_Sheet" CssClass="red-text" Display="Dynamic" ValidationGroup="Next"></asp:RequiredFieldValidator></label>
                        <asp:DropDownList ID="ddl_Sheet" runat="server" CssClass="browser-default" AutoPostBack="true" OnSelectedIndexChanged="ddl_Sheet_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="section row">
                    <div class="col s12">
                        <label>資料預覽</label>
                        <div>
                            <table id="listTable" class="stripe" cellspacing="0" width="100%" style="width: 100%;">
                                <asp:Literal ID="lt_tbBody" runat="server"></asp:Literal>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>注意事項</label>
                        <div>
                            <ul class="collection">
                                <li class="collection-item"><i class="material-icons left">info</i>空白的資料列會自動排除</li>
                                <li class="collection-item"><i class="material-icons left">info</i>重複的資料將不會匯入</li>
                                <li class="collection-item"><i class="material-icons left">info</i>不存在於ERP客戶品號資料檔的商品將不會匯入</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Hidden Field -->
            <asp:HiddenField ID="hf_FullFileName" runat="server" />
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
                    <a href="<%=fn_Params.WebUrl %>mySZBBC/StockImportStep1.aspx" class="btn-large waves-effect waves-light grey">
                        回上一步<i class="material-icons left">arrow_back</i>
                    </a>
                </div>
                <div class="col s6 right-align">
                    <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="btn-large waves-effect waves-light blue" OnClick="lbtn_Next_Click" ValidationGroup="Next">下一步，匯入資料<i class="material-icons right">chevron_right</i></asp:LinkButton>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" ValidationGroup="Next" />
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


        });
    </script>
    <%-- DataTable End --%>
    <style>
        #listTable td {
            word-break: keep-all;
            word-wrap: break-word;
        }
    </style>
</asp:Content>

