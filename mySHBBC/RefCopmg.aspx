<%@ Page Title="客戶商品對應" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="RefCopmg.aspx.cs" Inherits="mySHBBC_RefCopmg" %>

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
                    <div class="section">上海BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        客戶商品對應 (京東POP、天貓)
                    </div>
                </div>
            </div>
            <div class="right menu">
                <a href="<%=fn_Params.WebUrl%>mySZBBC_Extend/RefCopmg_Import.aspx" class="item" target="_blank"><i class="copy icon"></i><span class="mobile hidden">匯入對應表</span></a>
                <a href="<%=FuncPath() %>ImportList.aspx" class="item"><i class="undo icon"></i><span class="mobile hidden">返回BBC</span></a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <!-- Advance Search Start -->
        <div class="ui orange attached segment">
            <div class="ui small form">
                <div class="fields">
                    <div class="three wide required field">
                        <label>商城</label>
                        <asp:DropDownList ID="filter_Mall" runat="server"></asp:DropDownList>
                    </div>
                    <div class="seven wide required field">
                        <label>客戶</label>
                        <div class="ui fluid search ac-Cust">
                            <div class="ui right labeled input">
                                <asp:TextBox ID="filter_Cust" runat="server" CssClass="prompt" placeholder="輸入客戶代號或名稱關鍵字"></asp:TextBox>
                                <asp:Label ID="lb_Cust" runat="server" CssClass="ui label" Text="輸入關鍵字,選擇項目"></asp:Label>
                                <asp:TextBox ID="filter_CustName" runat="server" Style="display: none;"></asp:TextBox>
                            </div>
                        </div>

                    </div>
                    <div class="six wide field">
                        <label>品號關鍵字</label>
                        <div class="two fields">
                            <div class="field">
                                <asp:TextBox ID="filter_Keyword" runat="server" autocomplete="off" placeholder="查詢品號或客戶品號" MaxLength="20"></asp:TextBox>
                            </div>
                            <div class="field">
                                <button type="button" id="doSearch" class="ui blue small button"><i class="search icon"></i>帶出清單</button>
                                <asp:Button ID="btn_Search" runat="server" Text="Button" OnClick="btn_Search_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Advance Search End -->

        <!-- List Content Start -->
        <div class="ui green attached segment">
            <asp:PlaceHolder ID="ph_Add" runat="server" Visible="false">
                <div class="ui small form">
                    <div class="eight wide column">
                        <div class="fields">
                            <div class="six wide required field">
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
                            <div class="three wide required field">
                                <label>客戶品號</label>
                                <asp:TextBox ID="tb_CustModelNo" runat="server" autocomplete="off" MaxLength="40"></asp:TextBox>
                            </div>
                            <div class="seven wide field">
                                <label>客戶規格</label>
                                <div class="fields">
                                    <div class="eleven wide field">
                                        <asp:TextBox ID="tb_CustSpec" runat="server" autocomplete="off" MaxLength="90"></asp:TextBox>
                                    </div>
                                    <div class="five wide field">
                                        <asp:Button ID="btn_SaveDetail" runat="server" Text="新增" CssClass="ui teal tiny button" OnClick="btn_Save_Click" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="ui divider"></div>
            </asp:PlaceHolder>

            <!-- Empty Content Start -->
            <asp:PlaceHolder ID="ph_EmptyData" runat="server">
                <div class="ui placeholder segment">
                    <div class="ui icon header">
                        <i class="search icon"></i>
                        <p>請先填寫篩選條件後執行「帶出清單」</p>
                        <p>若仍查無資料,請新增品項.</p>
                    </div>
                </div>
            </asp:PlaceHolder>
            <!-- Empty Content End -->

            <asp:PlaceHolder ID="ph_Data" runat="server">
                <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                    <LayoutTemplate>
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3">寶工品號</th>
                                    <th class="grey-bg lighten-3">客戶品號</th>
                                    <th class="grey-bg lighten-3">客戶規格</th>
                                    <th class="grey-bg lighten-3"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="trItem" runat="server">
                            <td>
                                <strong class="blue-text text-darken-3"><%#Eval("ModelNo") %></strong>
                            </td>
                            <td><%#Eval("CustModelNo") %></td>
                            <td><%#Eval("CustSpec") %></td>
                            <td class="center aligned collapsing">
                                <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui small orange basic icon button" ValidationGroup="List" CommandName="doClose" OnClientClick="return confirm('確定刪除?')"><i class="trash alternate icon"></i></asp:LinkButton>

                                <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
            </asp:PlaceHolder>
        </div>
        <asp:PlaceHolder ID="ph_Pager" runat="server" Visible="false">
            <!-- List Pagination Start -->
            <div class="ui mini bottom attached segment grey-bg lighten-4">
                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
            </div>
            <!-- List Pagination End -->
        </asp:PlaceHolder>
        <!-- List Content End -->

    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
    <div class="ui info message">
        <div class="header">
            功能說明
        </div>
        <ul class="list">
            <li class="red-text text-darken-2"><b>目前僅適用「天貓」、「京東POP」，若有其他商城要使用，請聯絡資訊部。</b></li>
            <li>當客戶品號為一對多, 可在此處維護對應資料.</li>
            <li class="red-text text-darken-2">一對多的商品，客戶規格不可為空值</li>
            <li>選擇客戶後, 按下帶出清單, 即可取得資料</li>
            <li>新增資料的方式與ERP雷同.</li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            $(".myContentBody").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#doSearch").trigger("click");
                    //避免觸發submit
                    e.preventDefault();
                }
            });

            //[搜尋][查詢鈕] - 觸發查詢
            $("#doSearch").click(function () {
                $("#MainContent_btn_Search").trigger("click");
            });

            //init dropdown list
            $('select').dropdown();


        });
    </script>

    <%-- Search UI Start --%>
    <script>
        /* 客戶 (一般查詢) */
        $('.ac-Cust').search({
            minCharacters: 1,
            fields: {
                results: 'results',
                title: 'ID',
                description: 'Label'
            },
            searchFields: [
                'title',
                'description'
            ]
            , onSelect: function (result, response) {
                $("#MainContent_lb_Cust").text(result.Label);
                $("#MainContent_filter_CustName").val(result.Label);
            }
            , apiSettings: {
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?corp=3&q={query}'
            }

        });
    </script>
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
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Prod_v1.ashx?q={query}&V=1',
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
                          maxResults = 30
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

