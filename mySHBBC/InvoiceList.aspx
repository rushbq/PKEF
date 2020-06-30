<%@ Page Title="上海寶工BBC | 發票資料" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="InvoiceList.aspx.cs" Inherits="InvoiceList" %>

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
                        <li><a>上海BBC</a></li>
                        <li class="active">發票資料</li>
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
                            <div class="col s4">
                                <label>開票狀態</label>
                                <asp:DropDownListGP ID="filter_Status" runat="server" CssClass="select-control">
                                    <asp:ListItem Value="0">全部資料</asp:ListItem>
                                    <asp:ListItem Value="1" Selected="True">未開票</asp:ListItem>
                                    <asp:ListItem Value="2">開票中</asp:ListItem>
                                    <asp:ListItem Value="3">已開票</asp:ListItem>
                                </asp:DropDownListGP>
                            </div>
                            <div class="col s8">
                                <div class="input-field inline">
                                    <i class="material-icons prefix">today</i>
                                    <asp:TextBox ID="filter_sDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                    <label for="MainContent_filter_sDate">銷貨日-起</label>
                                </div>
                                <div class="input-field inline">
                                    <i class="material-icons prefix">today</i>
                                    <asp:TextBox ID="filter_eDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                    <label for="MainContent_filter_eDate">銷貨日-訖</label>
                                </div>
                            </div>
                        </div>


                        <div class="row">
                            <div class="input-field col s6">
                                <asp:TextBox ID="filter_Keyword" runat="server" placeholder="客戶單號/銷貨單號/訂單單號/客戶暱稱/發票抬頭/客戶代號/客戶簡稱" autocomplete="off"></asp:TextBox>
                                <label for="MainContent_filter_Keyword">關鍵字查詢</label>
                            </div>
                            <div class="input-field col s6 right-align">
                                <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
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
                                            <th class="red lighten-4 center-align">狀態</th>
                                            <th class="red lighten-4">客戶單號</th>
                                            <th class="red lighten-4">客戶暱稱</th>
                                            <th class="green lighten-4 center-align">銷貨單號</th>
                                            <th class="green lighten-4">客戶代號/客戶名稱</th>
                                            <th class="green lighten-4 center-align">銷貨日期</th>
                                            <th class="green lighten-4 right-align">開票金額</th>
                                            <th class="green lighten-4 center-align">發票抬頭</th>
                                            <th class="lime lighten-3 center-align">結帳單號</th>
                                            <th class="lime lighten-3">發票號碼</th>
                                            <th class="lime lighten-3">發票日期</th>
                                            <th class="lime lighten-3">&nbsp;</th>
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
                                <td class="center-align">
                                    <div>
                                        <span class="left green-text text-darken-2 invOK-<%#Eval("SerialNo") %>" style="display: none"><i class="material-icons">check</i></span>
                                        <span class="left red-text text-darken-2 invFail-<%#Eval("SerialNo") %>" style="display: none"><i class="material-icons">priority_high</i></span>
                                        <%#fn_Desc.BBC.Get_StatusName(Eval("InvStatus").ToString()) %>
                                    </div>
                                </td>
                                <td>
                                    <%#Eval("OrderID") %>
                                </td>
                                <td>
                                    <%#Eval("NickName") %>
                                </td>
                                <td class="center-align">
                                    <%#Eval("SO_FID") %>-<%#Eval("SO_SID") %>
                                </td>
                                <td>
                                    <p><%#Eval("CustID") %></p>
                                    <p><%#Eval("CustName") %></p>
                                </td>
                                <td class="center-align">
                                    <%#Eval("SO_Date") %>
                                </td>
                                <td class="right-align">
                                    <%# String.Format("{0:#,0.00}",Eval("TotalPrice"))%>
                                </td>
                                <td class="center-align">
                                    <asp:Literal ID="lt_Edit" runat="server"></asp:Literal>
                                </td>
                                <td class="center-align">
                                    <%#Eval("BI_FID") %>-<%#Eval("BI_SID") %>
                                </td>
                                <td>
                                    <%#Eval("InvoiceNo") %>
                                </td>
                                <td>
                                    <%#Eval("InvoiceDate") %>
                                </td>
                                <td>
                                    <a href="#!" class="btn-OpenDetail" data-id="<%#Eval("SerialNo") %>">明細</a>

                                    <input type="hidden" id="FID<%#Eval("SerialNo") %>" value="<%#Eval("SO_FID") %>" />
                                    <input type="hidden" id="SID<%#Eval("SerialNo") %>" value="<%#Eval("SO_SID") %>" />
                                </td>
                            </tr>
                            <!-- 發票資料 Start -->
                            <tr id="tar-Invoice-<%#Eval("SerialNo") %>" style="display: none;">
                                <td colspan="12" style="padding: 0px 0px 20px 20px;">
                                    <blockquote>
                                        <table>
                                            <tr>
                                                <td class="grey lighten-5">發票類型</td>
                                                <td class="grey lighten-5">發票抬頭</td>
                                                <td class="grey lighten-5">稅號</td>
                                                <td class="grey lighten-5">地址/電話</td>
                                                <td class="grey lighten-5">開戶行/帳號</td>
                                                <td class="grey lighten-5">寄票信息</td>
                                                <td class="grey lighten-5">備註</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <select id="InvType<%#Eval("SerialNo") %>" class="browser-default" style="width: 70px">
                                                        <option value="<%#Eval("InvType") %>"><%#string.IsNullOrEmpty(Eval("InvType").ToString())?"------": fn_Desc.BBC.GetInvTypeName(Eval("InvType").ToString()) %></option>
                                                        <option value="1">普票</option>
                                                        <option value="2">专票</option>
                                                    </select>
                                                </td>
                                                <td>
                                                    <input id="InvTitle<%#Eval("SerialNo") %>" type="text" maxlength="50" value="<%#Eval("InvTitle") %>" />
                                                </td>
                                                <td>
                                                    <input id="InvNumber<%#Eval("SerialNo") %>" type="text" maxlength="100" value="<%#Eval("InvNumber") %>" />
                                                </td>
                                                <td>
                                                    <input id="InvAddrInfo<%#Eval("SerialNo") %>" type="text" maxlength="100" value="<%#Eval("InvAddrInfo") %>" />
                                                </td>
                                                <td>
                                                    <input id="InvBankInfo<%#Eval("SerialNo") %>" type="text" maxlength="100" value="<%#Eval("InvBankInfo") %>" />
                                                </td>
                                                <td>
                                                    <input id="InvMessage<%#Eval("SerialNo") %>" type="text" maxlength="50" value="<%#Eval("InvMessage") %>" />
                                                </td>
                                                <td>
                                                    <input id="InvRemark<%#Eval("SerialNo") %>" type="text" maxlength="50" value="<%#Eval("InvRemark") %>" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div class="row">
                                            <div class="col s6">
                                                <!-- Button 選擇歷史資料 -->
                                                <%--<a class="btn waves-effect waves-light waves-green white black-text modal-trigger" data-source="<%=Application["WebUrl"] %>mySHBBC/GetHtml_InvSelector.ashx?id=<%#Eval("SerialNo") %>&keyword=<%#Server.UrlEncode(Eval("NickName").ToString()) %>" href="#remoteModal"><i class="material-icons left">search</i>選擇歷史資料</a>--%>
                                            </div>
                                            <div class="col s6 right-align">
                                                <!-- Button Save -->
                                                <a class="btn-CloseInvoice waves-effect waves-teal btn-flat" title="Close" data-id="<%#Eval("SerialNo") %>">CLOSE</a>
                                                <asp:PlaceHolder ID="ph_InvSave" runat="server">
                                                    <input type="hidden" id="OrderID<%#Eval("SerialNo") %>" value="<%#Eval("OrderID") %>" />
                                                    <input type="hidden" id="TraceID<%#Eval("SerialNo") %>" value="<%#Eval("TraceID") %>" />
                                                    <a class="btn waves-effect waves-light green lighten-1 btn-InvoiceSave" data-id="<%#Eval("SerialNo") %>" title="Save"><i class="material-icons">save</i></a>
                                                </asp:PlaceHolder>
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
                            <!-- 發票資料 End -->

                            <!-- 銷貨明細 Start -->
                            <tr id="tar-Detail-<%#Eval("SerialNo") %>" style="display: none;">
                                <td colspan="12" style="padding: 0px 0px 20px 20px;">
                                    <blockquote>
                                        <div class="Detail-<%#Eval("SerialNo") %>">
                                            <div class="row">
                                                <div class="col s8 offset-s2">
                                                    <div class="progress">
                                                        <div class="indeterminate"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- close -->
                                        <div class="section right-align">
                                            <a class="btn-CloseDetail waves-effect waves-teal btn-flat" title="Close" data-id="<%#Eval("SerialNo") %>">CLOSE</a>
                                        </div>
                                    </blockquote>
                                </td>
                            </tr>
                            <!-- 銷貨明細 End -->
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
    <!-- Modal Structure -->
    <div id="remoteModal" class="showModal modal modal-fixed-footer">
        <div class="modal-content"></div>
        <div class="modal-footer">
            <a href="#!" class="modal-action modal-close waves-effect waves-red btn-flat">Close</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script src="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/lib/pickadate/translation/zh_TW.js"></script>
    <script>
        $(function () {
            //載入選單
            $('.select-control').material_select();


            //載入datepicker
            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 5, // Creates a dropdown of 15 years to control year
                format: 'yyyymmdd',

                closeOnSelect: false // Close upon selecting a date(此版本無作用)
            }).on('change', function () {
                $(this).next().find('.picker__close').click();
            });


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

            ////[Modal搜尋][Enter鍵] - 觸發關鍵字快查
            //$("#remoteModal").on("keypress", '.subSearch_Keyword', function (e) {
            //    code = (e.keyCode ? e.keyCode : e.which);
            //    if (code == 13) {
            //        $(".trigger-subSearch").trigger("click");

            //        e.preventDefault();
            //    }
            //});

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
                var _OrderID = $("#OrderID" + id).val();
                var _TraceID = $("#TraceID" + id).val();
                var _InvTitle = $("#InvTitle" + id).val();
                var _InvType = $("#InvType" + id).val();
                var _InvNumber = $("#InvNumber" + id).val();
                var _InvAddrInfo = $("#InvAddrInfo" + id).val();
                var _InvBankInfo = $("#InvBankInfo" + id).val();
                var _InvMessage = $("#InvMessage" + id).val();
                var _InvRemark = $("#InvRemark" + id).val();

                //Check null
                if (_InvType == "" || _InvTitle == "") {
                    alert('資料未填寫');
                    return false;
                }

                //其他欄位
                var msgOK = $(".msg-Invoice-ok-" + id); //成功訊息
                var iconOK = $(".invOK-" + id); //成功圖示
                var iconFail = $(".invFail-" + id); //失敗圖示

                var request = $.ajax({
                    url: '<%=Application["WebUrl"]%>' + "mySHBBC/Ashx_InvoiceData.ashx",
                    method: "POST",
                    data: {
                        OrderID: _OrderID,
                        TraceID: _TraceID,
                        InvTitle: _InvTitle,
                        InvType: _InvType,
                        InvNumber: _InvNumber,
                        InvAddrInfo: _InvAddrInfo,
                        InvBankInfo: _InvBankInfo,
                        InvMessage: _InvMessage,
                        InvRemark: _InvRemark,
                        Who: '<%=fn_Params.UserGuid%>'
                    },
                    dataType: "html"
                });

                request.done(function (msg) {
                    if (msg == "success") {
                        $("#btn-OpenInvoice-" + id).text(_InvTitle);
                        //顯示成功訊息
                        msgOK.slideDown("slow");
                        iconOK.slideDown("slow");

                    } else {
                        event.preventDefault();
                        console.log(msg);
                        alert('資料存檔失敗! (發票)');
                        iconFail.slideDown("slow");
                    }
                });

                request.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert('存檔時發生錯誤，請連絡系統人員 (發票)');
                    iconFail.slideUp("slow");
                });

                request.always(function () {
                    //關閉成功訊息
                    msgOK.delay(2500).slideUp(800);
                });

            });
        });
    </script>
    <%-- 查詢發票歷史資料 --%>
    <script>
        //$(function () {
        //    //Modal - Ajax載入資料
        //    $(".modal-trigger").on("click", function () {
        //        //Get Url
        //        var url = $(this).attr("data-source");
        //        var $modalEle = $("#remoteModal .modal-content");

        //        //loading
        //        $modalEle.html('<div class="progress"><div class="indeterminate"></div></div>');

        //        $.get(url, function (result) {
        //            //Get Result
        //            $modalEle.html(result);
        //        });
        //    });

        //    //觸發Modal 模組
        //    $('.showModal').modal();


        //    //觸發子查詢 - 重新載入Modal
        //    $("#remoteModal").on("click", '.trigger-subSearch', function () {
        //        var data = $(".subSearch_Keyword").val();
        //        var id = $(".subSearch_id").val();
        //        if (data == "") {
        //            alert('請填入關鍵字');
        //            return false;
        //        }

        //        //Get Url
        //        var url = "GetHtml_InvSelector.ashx?id=" + id + "&keyword=" + encodeURI(data);
        //        var $modalEle = $("#remoteModal .modal-content");

        //        //loading
        //        $modalEle.html('<div class="progress"><div class="indeterminate"></div></div>');

        //        $.get(url, function (result) {
        //            //Get Result
        //            $modalEle.html(result);
        //        });
        //    });


        //    //子查詢 - 歷史資料傳入按鈕
        //    $("#remoteModal").on("click", '.subSearch_submit', function () {
        //        //取得ROW ID
        //        var id = $(".subSearch_id").val();
        //        //取得資料ID
        //        var dataId = $(this).attr("data-id");;

        //        //取得資料
        //        var InvType = $(".InvType" + dataId).val();
        //        var InvTitle = $(".InvTitle" + dataId).val();
        //        var InvNumber = $(".InvNumber" + dataId).val();
        //        var InvAddrInfo = $(".InvAddrInfo" + dataId).val();
        //        var InvBankInfo = $(".InvBankInfo" + dataId).val();
        //        var InvMessage = $(".InvMessage" + dataId).val();
        //        var invType = $(".InvType" + dataId).val();

        //        //回填資料
        //        $("#InvType" + id).val(InvType);
        //        $("#InvTitle" + id).val(InvTitle);
        //        $("#InvNumber" + id).val(InvNumber);
        //        $("#InvAddrInfo" + id).val(InvAddrInfo);
        //        $("#InvBankInfo" + id).val(InvBankInfo);
        //        $("#InvMessage" + id).val(InvMessage);
        //        $("#invType" + id).val(invType);

        //        //Close Modal
        //        $('#remoteModal').modal('close');
        //    });
        //});
    </script>

    <%-- 銷貨明細 --%>
    <script>
        $(function () {
            //按鈕 - 開明細
            $(".btn-OpenDetail").click(function () {
                var id = $(this).attr("data-id");

                boxDetail(id, true);
            });

            //按鈕 - 關明細
            $('.btn-CloseDetail').click(function () {
                var id = $(this).attr("data-id");

                boxDetail(id, false);
            });

            //FUNCTION - 明細開關
            function boxDetail(id, isOpen) {
                var myBox = $("#tar-Detail-" + id);

                if (isOpen) {
                    myBox.show();

                    loadDetail(id);

                } else {
                    myBox.hide();
                }
            }

            //Ajax - 讀取明細
            function loadDetail(id) {
                //取得目標容器
                var container = $(".Detail-" + id);

                //取得輸入值
                var _SO_FID = $("#FID" + id).val();
                var _SO_SID = $("#SID" + id).val();

                //填入Ajax Html
                var url = "<%=Application["WebUrl"]%>mySHBBC/GetHtml_SOdetail.ashx?fid=" + _SO_FID + "&sid=" + _SO_SID;
                container.load(url);
            }


<%--            //明細表的按鈕事件 (因應AJAX使用.on)
            $('#myListTable').on('click', '.btn-UpdateDetail', function (event) {
                //取值
                var _FID = $(this).attr("data-fid"); //單別
                var _SID = $(this).attr("data-sid"); //單號
                var _SNO = $(this).attr("data-no"); //序號
                var _SetInvoice = $(this).attr("data-value");   //要變更成 XXX 的值
                var _relID = $(this).attr("data-rel"); //切換用ID = 單別 + 單號 + 序號
                ;
                //判斷空值
                if (_FID == "" || _SID == "" || _SetInvoice == "") {
                    event.preventDefault();
                    alert('無法執行更新, 請聯絡系統管理員 (明細)');
                    return false;

                } else {
                    var cfmWord;
                    if (_SetInvoice == "Y") {
                        cfmWord = "是否要將此品項還原為「要開發票」";
                    } else {
                        cfmWord = "是否要將此品項設定為「不開發票」";
                    }

                    //comfirm
                    if (confirm(cfmWord)) {
                        var request = $.ajax({
                            url: '<%=Application["WebUrl"]%>' + "mySHBBC/Ashx_InvoiceUpdate.ashx",
                            method: "POST",
                            data: {
                                FID: _FID,
                                SID: _SID,
                                SNO: _SNO,
                                Val: _SetInvoice
                            },
                            dataType: "html"
                        });

                        request.done(function (msg) {
                            if (msg == "success") {
                                //切換按鈕
                                $(".btn-" + _relID).toggle();

                            } else {
                                event.preventDefault();
                                alert('資料存檔失敗! (明細)');
                            }
                        });

                        request.fail(function (jqXHR, textStatus) {
                            //console.log("failed," + textStatus);
                            event.preventDefault();
                            alert('存檔時發生錯誤，請連絡系統人員 (明細)');
                        });
                    }
                }
            });--%>

        });
    </script>
</asp:Content>

