<%@ Page Title="基本資料維護|品號延伸欄位" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ProdList.aspx.cs" Inherits="myProdExtend_ProdList" %>

<%@ Import Namespace="PKLib_Method.Methods" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <ol class="breadcrumb">
                        <li><a>基本資料維護</a></li>
                        <li class="active">品號延伸欄位</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <div class="row">
            <div class="col s12">
                <div class="card">
                    <div class="card-content">
                        <div class="row">
                            <div class="input-field col s6">
                                <asp:TextBox ID="filter_Keyword" runat="server" placeholder="品號/品名" autocomplete="off"></asp:TextBox>
                                <label for="MainContent_filter_Keyword">關鍵字查詢</label>
                            </div>
                            <div class="input-field col s6 right-align">
                                <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
                                <asp:LinkButton ID="lbtn_Export" runat="server" CssClass="btn waves-effect waves-light orange" OnClick="lbtn_Export_Click" ToolTip="匯至Excel"><i class="material-icons">archive</i></asp:LinkButton>
                                <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                            </div>
                        </div>

                    </div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <div class="card-content">
                                <table id="myListTable" class="bordered highlight">
                                    <thead>
                                        <tr>
                                            <th>品號</th>
                                            <th>品名</th>
                                            <th>&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </div>
                            <div class="section">
                                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <span class="left blue-text text-darken-2 invOK-<%#Eval("SerialNo") %>" style="display: none"><i class="material-icons">check</i></span>
                                    <span class="left red-text text-darken-2 invFail-<%#Eval("SerialNo") %>" style="display: none"><i class="material-icons">priority_high</i></span>
                                    <b class="green-text darken-4"><%#Eval("ModelNo") %></b>
                                </td>
                                <td>
                                    <%#Eval("Name_TW") %>
                                </td>
                                <td class="center-align">
                                    <a href="#!" class="btn waves-effect waves-light grey btn-OpenInvoice" data-id="<%#Eval("SerialNo") %>"><i class="material-icons">edit</i></a>
                                </td>
                            </tr>
                            <!-- 明細資料 Start -->
                            <tr id="tar-Invoice-<%#Eval("SerialNo") %>" style="display: none;">
                                <td colspan="3" style="padding: 0px 0px 20px 20px;">
                                    <blockquote>
                                        <table>
                                            <tr>
                                                <td class="grey lighten-5">電商安全庫存</td>
                                                <td class="grey lighten-5">A01安全庫存</td>
                                                <td class="grey lighten-5">B01安全庫存</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <input id="SafeQty_SZEC<%#Eval("SerialNo") %>" value="<%#Eval("SafeQty_SZEC") %>" type="number" min="0" max="99999" />
                                                </td>
                                                <td>
                                                    <input id="SafeQty_A01<%#Eval("SerialNo") %>" value="<%#Eval("SafeQty_A01") %>" type="number" min="0" max="99999" />
                                                </td>
                                                <td>
                                                    <input id="SafeQty_B01<%#Eval("SerialNo") %>" value="<%#Eval("SafeQty_B01") %>" type="number" min="0" max="99999" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div class="row">
                                            <div class="col s12 right-align">
                                                <!-- Button Save -->
                                                <a class="btn-CloseInvoice waves-effect waves-teal btn-flat" title="Close" data-id="<%#Eval("SerialNo") %>">CLOSE</a>
                                                <input type="hidden" id="dataID-<%#Eval("SerialNo") %>" value="<%#Eval("ModelNo") %>" />
                                                <a class="btn waves-effect waves-light green lighten-1 btn-InvoiceSave" data-id="<%#Eval("SerialNo") %>" title="Save"><i class="material-icons">save</i></a>
                                            </div>
                                        </div>
                                        <div class="row msg-Invoice-ok-<%#Eval("SerialNo") %>" style="display: none;">
                                            <div class="col s12">
                                                <div class="card-panel grey darken-1">
                                                    <i class="material-icons white-text left">error_outline</i>
                                                    <span class="white-text">資料已儲存</span>
                                                </div>
                                            </div>
                                        </div>
                                    </blockquote>
                                </td>
                            </tr>
                            <!-- 明細資料 End -->

                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="section">
                                <div class="card-panel grey darken-1">
                                    <i class="material-icons flow-text white-text">error_outline</i>
                                    <span class="flow-text white-text">目前的篩選條件找不到資料，請重新篩選。</span>
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>

                </div>

            </div>
        </div>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //[搜尋][查詢鈕] - 觸發關鍵字快查
            $("#trigger-keySearch").click(function () {
                $("#MainContent_btn_KeySearch").trigger("click");
            });


            //[搜尋][Enter鍵] - 觸發關鍵字快查
            $("#MainContent_filter_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_KeySearch").trigger("click");

                    e.preventDefault();
                }
            });

        });
    </script>

    <%-- 發票維護 --%>
    <script>
        $(function () {
            //按鈕 - 編輯
            $(".btn-OpenInvoice").click(function () {
                var id = $(this).attr("data-id");

                boxInvoice(id, true);
            });

            //按鈕 - 關閉
            $(".btn-CloseInvoice").click(function () {
                var id = $(this).attr("data-id");

                boxInvoice(id, false);
            });

            //FUNCTION - 發票開關
            function boxInvoice(id, isOpen) {
                var myBox = $("#tar-Invoice-" + id);

                if (isOpen) {
                    myBox.show();
                } else {
                    myBox.hide();
                }
            }


            //按鈕 - 儲存
            $(".btn-InvoiceSave").click(function () {
                //取得編號
                var id = $(this).attr("data-id");

                //取得輸入值
                var _dataID = $("#dataID-" + id).val();
                var _SafeQty_SZEC = $("#SafeQty_SZEC" + id).val();
                var _SafeQty_A01 = $("#SafeQty_A01" + id).val();
                var _SafeQty_B01 = $("#SafeQty_B01" + id).val();


                //其他欄位
                var msgOK = $(".msg-Invoice-ok-" + id); //成功訊息
                var iconOK = $(".invOK-" + id); //成功圖示
                var iconFail = $(".invFail-" + id); //失敗圖示

                var request = $.ajax({
                    url: '<%=Application["WebUrl"]%>' + "myProdExtend/Ashx_ProdData.ashx",
                    method: "POST",
                    data: {
                        DataID: _dataID,
                        SafeQty_SZEC: _SafeQty_SZEC,
                        SafeQty_A01: _SafeQty_A01,
                        SafeQty_B01: _SafeQty_B01,
                        Who: '<%=fn_Params.UserGuid%>'
                    },
                    dataType: "html"
                });

                request.done(function (msg) {
                    if (msg == "success") {
                        //顯示成功訊息
                        msgOK.slideDown("slow");
                        iconOK.slideDown("slow");

                    } else {
                        event.preventDefault();
                        alert('資料存檔失敗!');
                        iconFail.slideDown("slow");
                    }
                });

                request.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert('存檔時發生錯誤，請連絡系統人員');
                    iconFail.slideUp("slow");
                });

                request.always(function () {
                    //關閉成功訊息
                    msgOK.delay(2500).slideUp(800);
                });

            });
        });
    </script>
</asp:Content>

