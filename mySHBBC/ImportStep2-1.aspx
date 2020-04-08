<%@ Page Title="上海BBC | 匯入Excel" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep2-1.aspx.cs" Inherits="mySHBBC_PromoConfig_Edit" %>

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
                    <div class="section">上海BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">匯入Excel</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step2.1:Excel資料整理
                    </div>
                </div>
            </div>
            <div class="right menu">
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <!-- 基本資料 Start -->
            <div class="ui segments">
                <div class="ui form green segment">
                    <div class="fields">
                        <div class="eight wide field">
                            <label>匯入模式</label>
                            <div class="ui orange basic label">
                                <asp:Literal ID="lt_TypeName" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="eight wide field">
                            <label>商城</label>
                            <div class="ui orange large basic label">
                                <asp:Literal ID="lt_MallName" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div class="fields">
                        <div class="eight wide field">
                            <label>追蹤編號</label>
                            <div class="ui red large basic label">
                                <asp:Literal ID="lt_TraceID" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="eight wide field">
                            <label>客戶</label>
                            <div class="ui green large basic label">
                                <asp:Literal ID="lt_CustName" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="ui segment grey-bg lighten-5">
                    <div class="two ui large buttons">
                        <a class="ui grey button" href="<%=FuncPath() %>ImportStep2.aspx?dataID=<%=Req_DataID %>"><i class="undo icon"></i>回上一步</a>
                        <asp:LinkButton ID="lbtn_Next" runat="server" CssClass="ui blue button" OnClick="lbtn_Next_Click" ValidationGroup="Next">下一步，確認資料<i class="right arrow icon"></i></asp:LinkButton>
                        <asp:HiddenField ID="hf_MallID" runat="server" />
                        <asp:HiddenField ID="hf_CustID" runat="server" />
                        <asp:HiddenField ID="hf_Type" runat="server" />
                        <asp:HiddenField ID="hf_TraceID" runat="server" />
                    </div>
                </div>
            </div>
            <!-- 基本資料 End -->

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
                            <table class="ui celled selectable compact small table">
                                <thead>
                                    <tr>
                                        <th class="grey-bg lighten-3 center aligned">平台單號</th>
                                        <th class="grey-bg lighten-3 right aligned">金額</th>
                                        <th class="grey-bg lighten-3">物流單號</th>
                                        <th class="grey-bg lighten-3">收貨人</th>
                                        <th class="grey-bg lighten-3">買家備註</th>
                                        <th class="grey-bg lighten-3">賣家備註</th>
                                        <th class="grey-bg lighten-3">發票類型</th>
                                        <th class="grey-bg lighten-3">發票抬頭</th>
                                        <th class="grey-bg lighten-3">納稅人識別號</th>
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
                                        <h4 class="green-text text-darken-3"><%#Eval("OrderID") %></h4>
                                    </td>
                                    <td class="right aligned red-text text-darken-1">
                                        <strong><%#Eval("TotalPrice") %></strong>
                                    </td>
                                    <td>
                                        <strong><%#Eval("ShipmentNo") %></strong>
                                    </td>
                                    <td>
                                        <strong><%#Eval("ShipWho") %></strong>
                                    </td>
                                    <td><small>
                                        <%#Eval("BuyRemark") %>
                                    </small>
                                    </td>
                                    <td><small>
                                        <%#Eval("SellRemark") %></small>
                                    </td>
                                    <td><small>
                                        <%#Eval("InvMessage") %></small>
                                    </td>
                                    <td><small>
                                        <%#Eval("InvTitle") %></small>
                                    </td>
                                    <td><small>
                                        <%#Eval("InvNumber") %></small>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="ph_DataItems" runat="server">
                                <tr id="row<%#Eval("OrderID") %>">
                                    <td colspan="9">
                                        <div class="ui raised tiny segments">
                                            <div class="ui segment">
                                                <div class="fields">
                                                    <div class="eleven wide field">
                                                        <table class="ui celled compact small table">
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
                                                    <div class="five wide field">
                                                        <!-- 發票類型選單 -->
                                                        <select id="invType-<%#Eval("OrderID") %>" class="red-text text-darken-2">
                                                            <option value="0">不開票</option>
                                                            <option value="1" <%#Eval("InvType").ToString().Equals("1")?"selected=\"selected\"":"" %>>普票</option>
                                                            <option value="2" <%#Eval("InvType").ToString().Equals("2")?"selected=\"selected\"":"" %>>專票</option>
                                                        </select>
                                                        <!-- 發票資料填寫欄(順序不可變) -->
                                                        <div>
                                                            <textarea id="invData-<%#Eval("OrderID") %>" class="showTip" data-id="<%#Eval("OrderID") %>"><%#string.IsNullOrEmpty(Eval("InvRemark").ToString())
                                                                ? defRemarkValue(Eval("InvTitle").ToString(), Eval("InvNumber").ToString(), Eval("ShipWho").ToString())
                                                                : Eval("InvRemark").ToString()%></textarea>
                                                        </div>
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
                                                                <button type="button" class="ui small green button doCreate" data-id="<%#Eval("OrderID") %>"><i class="plus icon"></i>New</button>
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
                <li>於左下的下拉選單，<span class="red-text">輸入品號關鍵字搜尋</span>，選擇要新增的項目.</li>
                <li>確認選擇的項目後，按下「New」的按鈕，即可新增，可新增多筆.</li>
                <li>新增後數量預設為 1, 請於清單中修改數量, 並按下Save.</li>
                <li>若下拉選單查無資料，請至<a href="<%=fn_Params.WebUrl%>mySHBBC/RefCopmg.aspx?Mall=<%=hf_MallID.Value %>" target="_blank">商品對應</a>設定.</li>
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
    <div class="ui attached segment grey-bg lighten-5">
        <div class="two ui large buttons">
            <a class="ui grey button" href="<%=FuncPath() %>ImportStep2.aspx?dataID=<%=Req_DataID %>"><i class="undo icon"></i>回上一步</a>
            <asp:LinkButton ID="lbtn_Next_Bottom" runat="server" CssClass="ui blue button" OnClick="lbtn_Next_Click" ValidationGroup="Next">下一步，確認資料<i class="right arrow icon"></i></asp:LinkButton>
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
                    url: '<%=fn_Params.WebUrl%>' + "mySHBBC/Ashx_BBCTempNewItem.ashx",
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
                    url: '<%=fn_Params.WebUrl%>' + "mySHBBC/Ashx_BBCTempData.ashx",
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

