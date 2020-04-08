<%@ Page Title="台灣BBC | Step3" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep3.aspx.cs" Inherits="myTWBBC_ImportStep3" %>

<%@ Register Src="Ascx_StepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
<%@ Import Namespace="PKLib_Method.Methods" %>
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
                    <div class="section">台灣BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step3.確認轉入資料
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                <div class="ui negative message">
                    <div class="header">
                        Oops...發生了一點小問題
                    </div>
                    <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="3" />
            <!-- 資料 Start -->
            <div id="formData" class="ui small form attached green segment">
                <!-- 基本資料 S -->
                <div class="fields">
                    <div class="six wide field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="six wide field">
                        <label>客戶</label>
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                    <div class="four wide field">
                        <div class="ui middle aligned divided list">
                            <div class="item">
                                <div class="right floated content">
                                    <strong>
                                        <asp:Label ID="lb_DataType" runat="server"></asp:Label>
                                    </strong>
                                </div>
                                <div class="content">
                                    匯入類型
                                </div>
                            </div>
                            <div class="item">
                                <div class="right floated content">
                                    <strong>
                                        <asp:Label ID="lb_OrderType" runat="server"></asp:Label>
                                    </strong>
                                </div>
                                <div class="content">
                                    ERP單別
                                </div>
                            </div>
                            <div class="item">
                                <div class="right floated content">
                                    <asp:Label ID="lb_Currency" runat="server"></asp:Label>
                                </div>
                                <div class="content">
                                    幣別
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <!-- 基本資料 E -->
                <!-- 新增項目 S -->
                <div class="fields">
                    <div class="sixteen wide field">
                        <label>新增項目</label>
                        <div class="fields">
                            <div class="seven wide field">
                                <div class="ui fluid search ac-ModelNo">
                                    <div class="ui left icon right labeled input">
                                        <asp:TextBox ID="filter_ModelNo" runat="server" CssClass="prompt" placeholder="輸入品號或品名關鍵字"></asp:TextBox>
                                        <i class="search icon"></i>
                                        <asp:Panel ID="lb_ModelNo" runat="server" CssClass="ui label">輸入關鍵字,選擇項目</asp:Panel>
                                    </div>
                                    <asp:TextBox ID="val_ModelNo" runat="server" Style="display: none"></asp:TextBox>
                                </div>
                            </div>
                            <div class="three wide field">
                                <div class="ui labeled input">
                                    <div class="ui label">
                                        數量
                                    </div>
                                    <asp:TextBox ID="tb_InputCnt" runat="server" CssClass="fluid" type="number" min="1" MaxLength="6" autocomplete="off" placeholder="填入數量">1</asp:TextBox>
                                </div>
                            </div>
                            <div class="three wide field">
                                <asp:DropDownList ID="ddl_IsGift" runat="server" CssClass="fluid">
                                    <asp:ListItem Value="N" Selected="True">非贈品</asp:ListItem>
                                    <asp:ListItem Value="Y">設為贈品</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="three wide field">
                                <asp:LinkButton ID="lbtn_NewItem" runat="server" CssClass="ui teal tiny icon button" OnClick="lbtn_NewItem_Click" OnClientClick="return confirm('確定新增?\n若下方列表資料有變動,記得按下「存檔」')" ToolTip="新增品項"><i class="plus icon"></i></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 新增項目 E -->
                <!-- 單身列表 S -->
                <div class="fields">
                    <div class="sixteen wide field">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
                            <LayoutTemplate>
                                <table class="ui celled compact small table" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th class="grey-bg lighten-3 center aligned collapsing">編號</th>
                                            <th class="grey-bg lighten-3 center aligned collapsing">出貨地</th>
                                            <th class="grey-bg lighten-3">
                                                <span class="green-text text-darken-2">寶工品號</span><br />
                                                <span class="blue-text text-darken-2">客戶品號</span>
                                            </th>
                                            <th class="grey-bg lighten-3 center aligned collapsing">訂單數量</th>
                                            <th class="grey-bg lighten-3 center aligned collapsing" title="訂單數量/內盒數">比對數量</th>
                                            <th class="grey-bg lighten-3 center aligned collapsing">修改數量<br />
                                                <small>(ERP訂單數量)</small>
                                            </th>
                                            <th class="grey-bg lighten-3 center aligned">是否為<br />
                                                贈品</th>
                                            <th class="grey-bg lighten-3 right aligned collapsing">訂單價格</th>
                                            <th class="grey-bg lighten-3 right aligned collapsing">ERP價格</th>
                                            <th class="grey-bg lighten-3 center aligned collapsing">核價日</th>
                                            <th class="grey-bg lighten-3 center aligned collapsing">上次銷貨日</th>
                                            <th class="grey-bg lighten-3 center aligned collapsing">內盒數</th>
                                            <th class="grey-bg lighten-3 center aligned collapsing">外箱整箱數</th>
                                            <th class="grey-bg lighten-3">產銷訊息<br />
                                                <small>(點文字可放大)</small></th>
                                            <th class="grey-bg lighten-3 collapsing">檢查</th>
                                            <th class="grey-bg lighten-3">
                                                <button type="button" id="doExport" class="ui icon green small basic button" title="匯出Excel"><i class="file excel icon"></i></button>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="center aligned">
                                        <%#Eval("Data_ID") %>
                                    </td>
                                    <td class="center aligned">
                                        <strong><%#Eval("ShipFrom") %></strong>
                                    </td>
                                    <td class="collapsing">
                                        <p class="green-text text-darken-2">
                                            <b><%#Eval("ERP_ModelNo") %></b>
                                        </p>
                                        <p class="blue-text text-darken-2">
                                            <b><%#Eval("Cust_ModelNo") %></b>
                                        </p>
                                    </td>
                                    <td class="center aligned positive">
                                        <strong><%#Eval("BuyCnt") %></strong>
                                    </td>
                                    <td class="center aligned">
                                        <%--(訂單數量/內盒數=小數第一位)--%>
                                        <strong>
                                            <%#
                                        Convert.ToInt32(Eval("InnerBox")) > 0
                                        ? Math.Round(Convert.ToDouble(Eval("BuyCnt"))/Convert.ToDouble(Eval("InnerBox")), 1)
                                        : 0 
                                            %>
                                        </strong>
                                    </td>
                                    <td class="center aligned">
                                        <asp:TextBox ID="tb_InputCnt" runat="server" CssClass="fluid" Text='<%#Eval("InputCnt") %>' type="number" min="1" MaxLength="6"></asp:TextBox>
                                    </td>
                                    <td class="center aligned">
                                        <!--是否為贈品-->
                                        <asp:DropDownList ID="ddl_IsGift" runat="server" CssClass="fluid">
                                            <asp:ListItem Value="N">否</asp:ListItem>
                                            <asp:ListItem Value="Y">是</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td class="right aligned">
                                        <!--訂單價格-->
                                        <%#Eval("BuyPrice") %>
                                    </td>
                                    <td class="right aligned warning">
                                        <!--ERP價格-->
                                        <strong><%#Eval("ERP_Price") %></strong>
                                    </td>
                                    <td class="center aligned">
                                        <!--核價日-->
                                        <%#Eval("QuoteDate") %>
                                    </td>
                                    <td class="center aligned">
                                        <!--上次銷貨日-->
                                        <%#Eval("LastSalesDay") %>
                                    </td>
                                    <td class="center aligned">
                                        <!--內盒數-->
                                        <%#Eval("InnerBox") %>
                                    </td>
                                    <td class="center aligned">
                                        <!--外箱整箱數-->
                                        <%#Eval("OuterBox") %>
                                    </td>
                                    <td class="showPopup" data-id="remark<%#Eval("Data_ID") %>" title="點一下可放大" style="cursor: pointer;">
                                        <!--產銷訊息-->
                                        <%#Eval("ProdMsg") %>

                                        <!-- Modal Start -->
                                        <div id="remark<%#Eval("Data_ID") %>" class="ui modal">
                                            <div class="header"><%#Eval("ERP_ModelNo") %>&nbsp;產銷訊息</div>
                                            <div class="scrolling content"><%#Eval("ProdMsg").ToString().Replace("\n", "<br />") %></div>
                                            <div class="actions">
                                                <div class="ui black deny button">
                                                    Close
                                                </div>
                                            </div>
                                        </div>
                                        <!--  Modal End -->
                                    </td>
                                    <td class="center aligned">
                                        <asp:PlaceHolder ID="ph_Gift" runat="server">
                                            <i class="gift icon grey-text text-darken-3" title="這是贈品"></i>
                                        </asp:PlaceHolder>

                                        <asp:PlaceHolder ID="ph_attention" runat="server">
                                            <asp:Label ID="lb_attaSign" runat="server" CssClass="showExcept orange-text text-darken-3" ToolTip="點我查看" Style="cursor: pointer;"><i class="attention icon"></i></asp:Label>
                                            <!-- Modal Start -->
                                            <div id="atta<%#Eval("Data_ID") %>" class="ui modal">
                                                <div class="header"><%#Eval("ERP_ModelNo") %>&nbsp;異常原因</div>
                                                <div class="scrolling content"><%#Eval("doWhat").ToString().Replace("\n", "<br />") %></div>
                                                <div class="actions">
                                                    <div class="ui black deny button">
                                                        Close
                                                    </div>
                                                </div>
                                            </div>
                                            <!--  Modal End -->
                                        </asp:PlaceHolder>
                                    </td>
                                    <td class="left aligned collapsing">
                                        <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui small red basic icon button" ValidationGroup="List" CommandName="doDel" OnClientClick="return confirm('確定移除?\n執行後無法復原.')" ToolTip="移除"><i class="trash alternate icon"></i></asp:LinkButton>
                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="ui placeholder segment">
                                    <div class="ui icon header">
                                        <i class="wheelchair icon"></i>
                                        查無單身資料,請重新確認.
                                    </div>
                                </div>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
                <!-- 單身列表 E -->

                <div class="ui grid">
                    <div class="six wide column">
                        <a href="<%=Page_SearchUrl %>" class="ui button"><i class="undo icon"></i>返回列表</a>
                        <a href="<%=prevPage %>" class="ui grey button"><i class="chevron left icon"></i>回上一步</a>
                        <a href="#top" class="ui grey button"><i class="chevron up icon"></i>回頁首</a>
                    </div>
                    <div class="ten wide column right aligned">
                        <button id="doSave" type="button" class="ui blue button"><i class="save icon"></i>存檔不轉入</button>
                        <asp:Button ID="btn_Save" runat="server" Text="next" OnClick="btn_Save_Click" Style="display: none;" />

                        <asp:PlaceHolder ID="ph_WorkBtns" runat="server">
                            <button id="doNext" type="button" class="ui green button">轉入排程<i class="chevron right icon"></i></button>
                            <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_ErrTips" runat="server">
                            <asp:Label ID="lb_showTip" runat="server" CssClass="ui red large label"></asp:Label>
                        </asp:PlaceHolder>
                    </div>

                    <asp:HiddenField ID="hf_DataType" runat="server" />
                    <asp:HiddenField ID="hf_CustID" runat="server" />
                    <asp:Button ID="btn_Export" runat="server" Text="Excel" OnClick="btn_Export_Click" Style="display: none;" />
                </div>

            </div>
            <!-- 資料 End -->
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //init dropdown list
            $('select').dropdown();

            //focus then select text
            $("input").focus(function () {
                $(this).select();
            });

            //Click:下一步
            $("#doNext").click(function () {
                //confirm
                var r = confirm("資料是否已確認完畢?\n「確定」:開始轉入排程\n「取消」:停留本頁繼續編輯");
                if (r == true) {

                } else {
                    return false;
                }

                //loading
                $("#formData").addClass("loading");
                $("#MainContent_btn_Next").trigger("click");
            });

            //Click:存檔
            $("#doSave").click(function () {
                $("#formData").addClass("loading");
                $("#MainContent_btn_Save").trigger("click");
            });

            //Click:Excel
            $("#doExport").click(function () {
                $("#MainContent_btn_Export").trigger("click");
            });

            //Modal-產銷訊息
            $(".showPopup").on("click", function () {
                var id = $(this).attr("data-id");
                //show modal
                $('#' + id)
                    .modal({
                        selector: {
                            close: '.close, .actions .button'
                        }
                    })
                    .modal('show');
            });

            //Modal-異常訊息
            $(".showExcept").on("click", function () {
                var id = $(this).attr("data-id");
                //show modal
                $('#' + id)
                    .modal({
                        selector: {
                            close: '.close, .actions .button'
                        }
                    })
                    .modal('show');
            });

        });
    </script>

    <%-- Search UI Start --%>
    <script>
        /* 品號 (使用category) */
        $('.ac-ModelNo').search({
            type: 'category',
            minCharacters: 1,
            searchFields: [
                'title',
                'description'
            ]
            , onSelect: function (result, response) {
                //console.log(result.title);
                $("#MainContent_val_ModelNo").val(result.title);
                $("#MainContent_lb_ModelNo").text(result.description);

            }
            , apiSettings: {
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Prod_v1.ashx?q={query}',
                onResponse: function (ajaxResp) {
                    //宣告空陣列
                    var response = {
                        results: {}
                    }
                    ;
                    // translate API response to work with search
                    /*
                      取得遠端資料後處理
                      .results = 物件名稱
                      item.Category = 要群組化的欄位
                      maxResults = 查詢回傳筆數

                    */
                    $.each(ajaxResp.results, function (index, item) {
                        var
                          categoryContent = item.Category || 'Unknown',
                          maxResults = 20
                        ;
                        if (index >= maxResults) {
                            return false;
                        }
                        // create new categoryContent category
                        if (response.results[categoryContent] === undefined) {
                            response.results[categoryContent] = {
                                name: categoryContent,
                                results: []
                            };
                        }

                        //重組回傳結果(指定顯示欄位)
                        response.results[categoryContent].results.push({
                            title: item.ID,
                            description: item.Label
                        });
                    });
                    return response;
                }
            }

        });
    </script>
    <%-- Search UI End --%>
</asp:Content>

