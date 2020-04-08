<%@ Page Title="促銷活動設定" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="PromoConfig_Edit.aspx.cs" Inherits="mySZBBC_PromoConfig_Edit" %>

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
                    <div class="section">深圳BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        促銷活動設定
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
                <div class="ui green segment">
                    <h5 class="ui header">活動基本資料
                    </h5>
                </div>
                <div class="ui small form attached segment">
                    <div class="fields">
                        <div class="eight wide required field">
                            <label>活動名稱</label>
                            <asp:TextBox ID="tb_PromoName" runat="server" MaxLength="20" placeholder="填寫活動名稱" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="eight wide field">
                            <label>系統編號</label>
                            <div class="ui green basic small label">
                                <asp:Literal ID="lt_DataID" runat="server">資料新增中</asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div class="fields">
                        <div class="eight wide field">
                            <label>活動時間</label>
                            <div class="two fields">
                                <div class="field">
                                    <div class="ui left icon input datepicker">
                                        <asp:TextBox ID="tb_sDate" runat="server" placeholder="開始時間" autocomplete="off"></asp:TextBox>
                                        <i class="calendar alternate outline icon"></i>
                                    </div>
                                </div>
                                <div class="field">
                                    <div class="ui left icon input datepicker">
                                        <asp:TextBox ID="tb_eDate" runat="server" placeholder="結束時間" autocomplete="off"></asp:TextBox>
                                        <i class="calendar alternate outline icon"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="eight wide field">
                            <label>對應商城</label>
                            <asp:DropDownList ID="ddl_Mall" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="ui horizontal divider">活動類型</div>
                    <div class="fields">
                        <div class="sixteen wide field">
                            <table class="ui very basic celled table">
                                <tr>
                                    <td class="collapsing">
                                        <div class="ui radio checkbox">
                                            <asp:RadioButton ID="rb_Type1" runat="server" GroupName="PromoType" />
                                            <label>滿額送贈品</label>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="ui labeled input">
                                            <div class="ui label">
                                                $
                                            </div>
                                            <asp:TextBox ID="tb_TargetMoney" runat="server" MaxLength="10" placeholder="填寫金額" autocomplete="off" type="number"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td class="collapsing grey-text">
                                        (購買金額達成設定金額時,送指定贈品)
                                    </td>
                                </tr>
                                <tr>
                                    <td class="collapsing">
                                        <div class="ui radio checkbox">
                                            <asp:RadioButton ID="rb_Type2" runat="server" GroupName="PromoType" />
                                            <label>買指定商品送贈品</label>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="ui search ac-ModelNo-src">
                                            <div class="ui left icon right labeled input">
                                                <asp:TextBox ID="tb_TargetItem" runat="server" CssClass="prompt" placeholder="輸入指定商品"></asp:TextBox>
                                                <i class="search icon"></i>
                                                <asp:Panel ID="lb_TargetItem" runat="server" CssClass="ui label">輸入關鍵字,選擇項目</asp:Panel>
                                            </div>
                                            <asp:TextBox ID="val_TargetItem" runat="server" Style="display: none"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td class="collapsing grey-text">
                                        (購買的商品與此設定相同時,送指定贈品)
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="ui grid">
                        <div class="four wide column">
                            <a href="<%=Page_SearchUrl %>" class="ui small button"><i class="undo icon"></i>返回列表</a>
                        </div>
                        <div class="twelve wide column right aligned">
                            <button id="doSaveThenStay" type="button" class="ui green small button"><i class="save icon"></i>存檔後,留在本頁</button>
                            <asp:PlaceHolder ID="ph_Btn" runat="server">
                                <button id="doSave" type="button" class="ui green small button"><i class="save icon"></i>存檔後,返回列表</button>
                            </asp:PlaceHolder>
                            <asp:Button ID="btn_Save" runat="server" Text="Button" OnClick="btn_Save_Click" Style="display: none;" />
                            <asp:Button ID="btn_SaveStay" runat="server" Text="Button" OnClick="btn_SaveStay_Click" Style="display: none;" />
                            <asp:HiddenField ID="hf_DataID" runat="server" />
                        </div>
                    </div>
                </div>
                <%--<div class="ui bottom attached red small message">
                    <ul class="list">
                        <li>以上資料若有變更，記得按下「存檔」，避免資料遺失。</li>
                    </ul>
                </div>--%>
            </div>
            <!-- 基本資料 End -->

            <!-- 贈品清單 Start -->
            <div class="ui segments">
                <div class="ui teal segment">
                    <h5 class="ui header">贈品清單</h5>
                </div>
                <div id="DTList" class="ui small form segment">
                    <div class="ui internally celled grid">
                        <div class="row">
                            <div class="eight wide column">
                                <asp:ListView ID="lvDetailList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDetailList_ItemCommand">
                                    <LayoutTemplate>
                                        <table class="ui celled compact small table">
                                            <thead>
                                                <tr>
                                                    <th>品號</th>
                                                    <th class="center aligned">數量</th>
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
                                                <%#Eval("ModelNo") %>
                                            </td>
                                            <td class="center aligned">
                                                <%#Eval("Qty") %>
                                            </td>
                                            <td class="center aligned collapsing">
                                                <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui mini orange basic icon button" ValidationGroup="List" CommandName="doClose" OnClientClick="return confirm('確定刪除?')"><i class="trash alternate icon"></i></asp:LinkButton>
                                                <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="ui placeholder segment">
                                            <div class="ui icon header">
                                                <i class="gift icon"></i>
                                                尚未加入資料，請於「右方欄位」填寫
                                            </div>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                            <div class="eight wide column">
                                <div class="fields">
                                    <div class="four wide field">
                                        <label>數量</label>
                                        <asp:TextBox ID="tb_Qty" runat="server" MaxLength="3" placeholder="數量" autocomplete="off" type="number" min="1">1</asp:TextBox>
                                    </div>
                                    <div class="twelve wide required field">
                                        <label>品號</label>
                                        <div class="ui fluid search ac-ModelNo">
                                            <div class="ui left icon right labeled input">
                                                <asp:TextBox ID="filter_ModelNo" runat="server" CssClass="prompt" placeholder="輸入品號或品名關鍵字"></asp:TextBox>
                                                <i class="search icon"></i>
                                                <asp:Panel ID="lb_ModelNo" runat="server" CssClass="ui label">輸入關鍵字,選擇項目</asp:Panel>
                                            </div>
                                            <asp:TextBox ID="val_ModelNo" runat="server" Style="display: none"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="ui grid">
                                    <div class="column right aligned">
                                        <asp:Label ID="lb_Msg" runat="server" Text="請先設定基本資料" CssClass="ui red basic large label"></asp:Label>
                                        <asp:Button ID="btn_SaveDetail" runat="server" Text="新增" CssClass="ui teal tiny button" OnClick="btn_SaveDetail_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- 贈品清單 End -->

            <div class="ui grey padded segment">
                <table class="ui celled small four column table">
                    <thead>
                        <tr>
                            <th colspan="2" class="center aligned">建立</th>
                            <th colspan="2" class="center aligned">最後更新</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr class="center aligned">
                            <td>
                                <asp:Literal ID="info_Creater" runat="server">資料建立中..</asp:Literal>
                            </td>
                            <td>
                                <asp:Literal ID="info_CreateTime" runat="server">資料建立中..</asp:Literal>
                            </td>
                            <td>
                                <asp:Literal ID="info_Updater" runat="server"></asp:Literal>
                            </td>
                            <td>
                                <asp:Literal ID="info_UpdateTime" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
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

            $('.ui.radio.checkbox').checkbox();

            //[觸發][SAVE鈕]
            $("#doSave").click(function () {
                $("#MainContent_btn_Save").trigger("click");
            });
            $("#doSaveThenStay").click(function () {
                $("#MainContent_btn_SaveStay").trigger("click");
            });

        });
    </script>


    <%-- 日期選擇器 Start --%>
    <link href="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.css" rel="stylesheet" />
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.js"></script>
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').calendar(calendarOptsByTime_Range_unLimit);
        });
    </script>
    <%-- 日期選擇器 End --%>

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
    <script>
        /* 品號 (使用category) */
        $('.ac-ModelNo-src').search({
            type: 'category',
            minCharacters: 1,
            searchFields: [
                'title',
                'description'
            ]
            , onSelect: function (result, response) {
                //console.log(result.title);
                $("#MainContent_val_TargetItem").val(result.title);
                $("#MainContent_lb_TargetItem").text(result.description);

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

