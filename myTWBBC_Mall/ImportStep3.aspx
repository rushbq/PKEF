<%@ Page Title="台灣電商BBC | Step3" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep3.aspx.cs" Inherits="myTWBBC_Mall_ImportStep3" %>

<%@ Register Src="Ascx_StepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">業務行銷</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">台灣電商BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step3.整理Excel資料
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="3" />
            <!-- 基本資料 Start -->
            <div class="ui small form attached green segment">
                <div class="fields">
                    <div class="five wide field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="five wide field">
                        <label>商城</label>
                        <asp:Label ID="lb_Mall" runat="server" CssClass="ui brown basic large label"></asp:Label>
                    </div>
                    <div class="six wide field">
                        <label>客戶</label>
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                </div>
            </div>
            <!-- 基本資料 End -->
            <div class="ui hidden divider"></div>
            <div class="ui two column grid">
                <div class="column">
                    <a href="<%=Page_SearchUrl %>" class="ui button"><i class="undo icon"></i>返回列表</a>
                    <a class="ui grey button" href="<%=FuncPath() %>ImportStep2.aspx?id=<%=Req_DataID %>"><i class="chevron left icon"></i>回上一步</a>
                </div>
                <div class="column right aligned">
                    <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="ui green button" OnClick="lbtn_Next_Click" ValidationGroup="Next">下一步，確認資料<i class="chevron right icon"></i></asp:LinkButton>
                </div>
                <asp:HiddenField ID="hf_MallID" runat="server" />
                <asp:HiddenField ID="hf_CustID" runat="server" />
                <asp:HiddenField ID="hf_Type" runat="server" />
                <asp:HiddenField ID="hf_TraceID" runat="server" />
            </div>
            <!-- 資料清單 Start -->
            <div class="ui segments">
                <div class="ui teal segment">
                    <div class="ui grid">
                        <div class="ten wide column">
                            <h5 class="ui header">Excel資料</h5>
                        </div>
                        <div class="six wide column right aligned">
                            <a id="tips" href="javascript:void(0)" class="ui mini orange basic button">
                                <i class="question circle icon"></i>功能說明
                            </a>
                        </div>
                    </div>
                </div>
                <div id="DTList" class="ui small form segment">
                    <asp:ListView ID="lvDetailList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <table class="ui celled selectable compact table">
                                <thead>
                                    <tr>
                                        <th class="grey-bg lighten-3 center aligned">商城訂單號</th>
                                        <th class="grey-bg lighten-3">收貨人</th>
                                        <th class="grey-bg lighten-3">電話</th>
                                        <th class="grey-bg lighten-3">收貨地址</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <asp:PlaceHolder ID="ph_DataGroup" runat="server">
                                <tr>
                                    <td class="collapsing center aligned">
                                        <h4 class="green-text text-darken-2"><%#Eval("OrderID") %></h4>
                                    </td>
                                    <td>
                                        <b><%#Eval("ShipWho") %></b>
                                    </td>
                                    <td>
                                        <%#Eval("ShipTel") %>
                                    </td>
                                    <td>
                                        <%#Eval("ShipAddr") %>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_DataItems" runat="server">
                                <tr id="row<%#Eval("OrderID") %>">
                                    <td colspan="4">
                                        <div class="ui tiny segments">
                                            <div class="ui grey-bg lighten-4 segment">
                                                <div class="fields">
                                                    <div class="sixteen wide field">
                                                        <table class="ui celled compact table">
                                                            <thead>
                                                                <tr>
                                                                    <th style="width: 25%">商品</th>
                                                                    <th style="width: 15%">規格</th>
                                                                    <th style="width: 50%">品名</th>
                                                                    <th style="width: 10%">數量</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Literal ID="lt_Items" runat="server"></asp:Literal>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                                <div class="fields">
                                                    <!-- New Item Section Start -->
                                                    <div class="ten wide field">
                                                        <div class="two fields">
                                                            <div class="field">
                                                                <select id="drp<%#Eval("OrderID") %>" class="ac-drpProd ui fluid search selection dropdown">
                                                                    <option value="">新增商品:填入關鍵字,選擇項目</option>
                                                                </select>
                                                            </div>
                                                            <div class="field">
                                                                <button type="button" class="ui small olive button doCreate" data-id="<%#Eval("OrderID") %>"><i class="plus icon"></i>New</button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <!-- New Item Section End -->
                                                    <div class="six wide field right aligned">
                                                        <strong class="red-text tip-<%#Eval("OrderID") %>" style="display: none;">(記得要按下「Save」,修改過的資料才會儲存)</strong>
                                                        <button type="button" class="ui small teal button doSave" data-id="<%#Eval("OrderID") %>"><i id="icon<%#Eval("OrderID") %>" class="save icon"></i>Save</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="ui placeholder segment">
                                <div class="ui icon header">
                                    <i class="gift icon"></i>
                                    查無資料，請重新確認。
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </div>
            <!-- 資料清單 End -->

        </div>
    </div>
    <!-- Tips Start -->
    <div id="tipPage" class="ui modal">
        <div class="content">
            <div class="ui header">
                如何新增自訂品項
            </div>
            <ul class="ui list">
                <li>於左下的下拉選單，輸入品號關鍵字搜尋，選擇要新增的項目.</li>
                <li>確認選擇的項目後，按下「New」的按鈕，即可新增，可新增多筆.</li>
                <li>新增後數量預設為 1, 請於清單中修改數量, 並按下Save.</li>
                <li>若下拉選單查無資料，請至<a href="<%=fn_Params.WebUrl%>myTWBBC_Mall/RefCopmg.aspx?Mall=<%=hf_MallID.Value %>" target="_blank">商品對應</a>設定.</li>
                <li class="red-text text-darken-2">新增前請注意，若其他欄位有變動，請先按下Save</li>
            </ul>
            <div class="ui header">
                如何修改清單上的資料
            </div>
            <ul class="ui list">
                <li>於各欄位填寫完畢後，按下「Save」的按鈕，即可完成修改.</li>
            </ul>
        </div>
        <div class="actions">
            <div class="ui cancel button">
                關閉視窗
            </div>
        </div>
    </div>
    <!-- Tips End -->
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
    <div class="ui two column padded grid">
        <div class="column">
            <a href="<%=Page_SearchUrl %>" class="ui button"><i class="undo icon"></i>返回列表</a>
            <a class="ui grey button" href="<%=FuncPath() %>ImportStep2.aspx?id=<%=Req_DataID %>"><i class="chevron left icon"></i>回上一步</a>
        </div>
        <div class="column right aligned">
            <asp:LinkButton ID="lbtn_Next_Bottom" runat="server" CssClass="ui green button" OnClick="lbtn_Next_Click" ValidationGroup="Next">下一步，確認資料<i class="chevron right icon"></i></asp:LinkButton>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //init
            $('select').dropdown();

            //指定欄位動作後,顯示提示.
            $(".showTip").change(function () {
                //取得編號
                var id = $(this).attr("data-id");

                $(".tip-" + id).show();
            });

            //說明視窗(Modal)
            $("#tips").click(function () {
                $('#tipPage').modal('show');
            });

        });
    </script>
    <script>
        $(function () {
            /*
              功能按鈕:新增品項 / 儲存by單號
            */
            //New Prod Click
            $(".doCreate").click(function () {
                //取得編號
                var id = $(this).attr("data-id");
                var ts = $.now();

                //單頭參數
                var _parentID = '<%=Req_DataID%>';
                var _orderID = id;

                //取得下拉選單選擇值(refCOPMG.Data_ID)
                var _value = $("#drp" + id).dropdown("get value");
                if (_value == '') {
                    alert('未正確選擇商品');
                    return false;
                }

                /* Ajax Start */
                var saveBtn = $(this);
                //button 加入loading
                saveBtn.addClass("loading");

                var request = $.ajax({
                    url: '<%=fn_Params.WebUrl%>' + "myTWBBC_Mall/Ashx_BBCTempNewItem.ashx",
                    method: "POST",
                    //contentType: 'application/json',    //傳送格式
                    dataType: "html",   //遠端回傳格式
                    data: {
                        parentID: _parentID,
                        orderID: _orderID,
                        itemID: _value
                    },
                });

                request.done(function (msg) {
                    //button 移除loading
                    saveBtn.removeClass("loading");

                    if (msg == "success") {
                        //顯示成功訊息
                        location.replace('<%=thisPage%>&v=' + ts + '#row' + _orderID);

                } else {
                    event.preventDefault();
                    alert(_orderID + ':資料建立失敗!');
                    console.log(msg);
                }
                });

                request.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert(_orderID + ':存檔時發生錯誤,請連絡MIS.');
                    saveBtn.removeClass("loading");
                });

                request.always(function () {
                    //do something
                });
                /* Ajax End */

            });


            //Save click
            $(".doSave").click(function () {
                //取得編號
                var id = $(this).attr("data-id");

                //單頭參數
                var _ParentID = '<%=Req_DataID%>';
                var _OrderID = id;
                var _InvData = $("#invData-" + id).val();
                var _InvType = $("#invType-" + id).val();

                //單身參數陣列
                var _aryProd = [];
                //取得品項值
                $(".prodID-" + id).each(function () {
                    var _DataID = $(this).attr("data-id");
                    var _Val = $("#prodID-" + _DataID).val();
                    var _Cnt = $("#buyCnt-" + _DataID).val();

                    //Check null
                    if (_Val != "") {
                        //填入陣列
                        _aryProd.push({
                            ParentID: _ParentID,
                            OrderID: _OrderID,
                            InvData: _InvData,
                            InvType: _InvType,
                            DataID: _DataID,
                            ProdID: _Val,
                            BuyCnt: _Cnt
                        });
                    }
                });


                /* Ajax Start */
                var saveBtn = $(this);
                var saveIcon = $("#icon" + id);
                //button 加入loading
                saveBtn.addClass("loading");

                var request = $.ajax({
                    url: '<%=fn_Params.WebUrl%>' + "myTWBBC_Mall/Ashx_BBCTempData.ashx",
                    method: "POST",
                    contentType: 'application/json',    //傳送格式
                    dataType: "html",   //遠端回傳格式
                    data: JSON.stringify(_aryProd),
                });

                request.done(function (msg) {
                    //button 移除loading
                    saveBtn.removeClass("loading");

                    if (msg == "success") {
                        //顯示成功訊息
                        saveBtn.removeClass("blue").addClass("green");
                        saveIcon.removeClass("save").removeClass("exclamation").addClass("check");

                    } else {
                        event.preventDefault();
                        alert(_OrderID + ':資料存檔失敗!');
                        saveBtn.removeClass("blue").addClass("red");
                        saveIcon.removeClass("save").removeClass("check").addClass("exclamation");
                    }
                });

                request.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert(_OrderID + ':存檔時發生錯誤,請連絡MIS.');
                    saveBtn.removeClass("loading");
                });

                request.always(function () {
                    //do something
                });
                /* Ajax End */

            });

        });
    </script>
    <%-- Search UI Start --%>
    <script>
        /* RefCOPMG (一般查詢) */
        var _cust = $("#MainContent_hf_CustID").val();
        var _mall = $("#MainContent_hf_MallID").val();

        $('.ac-drpProd').dropdown({
            fields: {
                remoteValues: 'results',
                name: 'Label',
                value: 'ID'
            },
            apiSettings: {
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_RefCopmg.ashx?cust=' + _cust + '&mall=' + _mall + '&q={query}&v=1.2'
            }

        });
    </script>
    <%-- Search UI End --%>
    <script>
        /* 當網址有#錨點時,觸發以下動作 */
        if (window.location.hash) {
            var
              $element = $(window.location.hash),
              position = $element.offset().top - 60
            ;
            //$element.addClass('active');
            $('html, body')
              .stop()
              .animate({
                  scrollTop: position
              }, 400)
            ;
        }
    </script>
</asp:Content>

