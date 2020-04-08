<%@ Page Title="深圳-開票平台" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ErpInvStep2.aspx.cs" Inherits="mySZInvoice_ErpInvStep2" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">ERP未開票資料 - Step2</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳-開票平台</a></li>
                        <li><a href="<%=Application["WebUrl"] %>mySZInvoice/List.aspx">紙本發票</a></li>
                        <li class="active">手動填發票-Step2</li>
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
                    <div class="col s12">
                        <label>客戶</label>
                        <div class="green-text text-darken-2 flow-text">
                            <strong>
                                <asp:Literal ID="lt_CustName" runat="server"></asp:Literal></strong>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col s12">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="listTable" class="highlight">
                                    <thead>
                                        <tr>
                                            <th>銷貨單號</th>
                                            <th>結帳日期</th>
                                            <th>結帳單號</th>
                                            <th>發票號碼</th>
                                            <th>發票日期</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%#Eval("Erp_SO_ID") %>
                                    </td>
                                    <td>
                                        <%#Eval("ArDate") %>
                                    </td>
                                    <td>
                                        <%#Eval("Erp_AR_ID") %>
                                    </td>
                                    <td>
                                        <input id="InvNo_<%#Eval("SerialNo") %>" type="text" maxlength="20" placeholder="填入發票號碼" />
                                    </td>
                                    <td>
                                        <input type="date" id="InvDate_<%#Eval("SerialNo") %>" value="<%#DateTime.Now.ToShortDateString().ToDateString("yyyy-MM-dd") %>" />
                                    </td>
                                    <td class="center-align">
                                        <button type="button" class="btn waves-effect waves-light grey doCancel" title="復原" data-id="<%#Eval("Erp_AR_ID") %>" style="display:none ;"><i class="material-icons">restore</i></button>
                                        <button type="button" class="btn waves-effect waves-light green lighten-1 doUpdate" data-id="<%#Eval("Erp_AR_ID") %>" data-target="<%#Eval("SerialNo") %>" title="更新ERP發票號碼"><i class="material-icons">save</i></button>                                        
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
                    <a href="<%=Application["WebUrl"] %>mySZInvoice/ErpInvStep1.aspx" class="btn-large waves-effect waves-light grey">回上一步，重新選擇<i class="material-icons left">autorenew</i></a>
                </div>
                <div class="col s6 right-align">
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //按鈕-發票更新至ERP
            $(".doUpdate").on("click", function () {

                //宣告/取得必要參數
                var _dataID = $(this).attr("data-id");  //Data ID
                var _target = $(this).attr("data-target"); //發票欄編號
                var _invNo = $("#InvNo_" + _target).val(); //發票號碼欄位值
                var _invDate = $("#InvDate_" + _target).val(); //發票日期欄位值
                var _btn = $(this);
                var _btnCancel = $(this).parent().find(".doCancel"); //復原鈕

                //檢查發票欄是否空白
                if (_invNo === "") {
                    alert('請填入發票號碼.')
                    return false;
                }

                //Lock
                _btn.prop("disabled", true);
                _btn.text('轉入中..');

                var request = $.ajax({
                    url: '<%=Application["WebUrl"]%>' + "mySZInvoice/Ashx_UpdateErpInvoice.ashx",
                    method: "POST",
                    data: {
                        DataID: _dataID,
                        Who: '<%=fn_Params.UserGuid%>',
                        DataVal: _invNo,
                        InvDate: _invDate,
                        type: 'U'
                    },
                    dataType: "html"
                });

                request.done(function (msg) {
                    if (msg == "success") {
                        //顯示成功訊息
                        _btnCancel.css("display", "inline-block");
                        _btn.prop("disabled", false);
                        _btn.css("display", "none");

                    } else {
                        event.preventDefault();
                        alert('資料存檔失敗!'+ msg);
                        _btn.prop("disabled", false);

                    }
                });

                request.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    console.log(textStatus);
                    alert('存檔時發生錯誤，請連絡系統人員');
                    _btn.prop("disabled", false);
                });

                request.always(function () {
                    //do nothing
                    _btn.text('Save');
                });


            });


        });
    </script>
    <script>
        $(function () {
            //按鈕-取消更新至ERP
            $(".doCancel").on("click", function () {

                //宣告/取得必要參數
                var _dataID = $(this).attr("data-id");  //Data ID
                var _btn = $(this);
                var _btnSave = $(this).parent().find(".doUpdate"); //Save鈕

                if(confirm('確認:是否要將結帳單-發票號碼清空?'))

                //Lock
                _btn.prop("disabled", true);
                _btn.text('轉入中..');

                var request = $.ajax({
                    url: '<%=Application["WebUrl"]%>' + "mySZInvoice/Ashx_UpdateErpInvoice.ashx",
                    method: "POST",
                    data: {
                        DataID: _dataID,
                        Who: '<%=fn_Params.UserGuid%>',
                        type: 'D'
                    },
                    dataType: "html"
                });

                request.done(function (msg) {
                    if (msg == "success") {
                        //顯示成功訊息
                        _btnSave.css("display", "inline-block");
                        _btn.prop("disabled", false);
                        _btn.css("display", "none");

                    } else {
                        event.preventDefault();
                        alert('資料存檔失敗!');
                        _btn.prop("disabled", false);

                    }
                });

                request.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert('存檔時發生錯誤，請連絡系統人員');
                    _btn.prop("disabled", false);
                });

                request.always(function () {
                    //do nothing
                    _btn.text('Restore');
                });


            });


        });
    </script>
</asp:Content>
