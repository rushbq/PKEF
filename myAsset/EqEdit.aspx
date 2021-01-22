<%@ Page Title="機房資訊資產管理" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="EqEdit.aspx.cs" Inherits="myAsset_EqEdit" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">資產管理</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section red-text text-darken-2">
                        機房資訊資產管理 - 
                        <asp:Literal ID="lt_CorpName" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
            <div class="right menu">
                <a class="anchor" id="top" title="置頂用錨點"></a>
                <a href="<%:Page_SearchUrl %>" class="item"><i class="undo icon"></i><span class="mobile hidden">返回列表</span></a>
                <a href="EqEdit.aspx?dbs=<%:Req_DBS %>" class="item"><i class="plus icon"></i><span class="mobile hidden">新增下一筆</span></a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui grid">
            <div class="row">
                <!-- Left Body Content Start -->
                <div id="myStickyBody" class="thirteen wide column">
                    <div class="ui attached segment grey-bg lighten-5">
                        <!-- Section-基本資料 Start -->
                        <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                            <div class="ui negative message">
                                <div class="header">
                                    Oops..執行時發生了小狀況~
                                </div>
                                <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                            </div>
                        </asp:PlaceHolder>
                        <div class="ui segments">
                            <div class="ui green segment">
                                <h5 class="ui header"><a class="anchor" id="baseData"></a>基本資料</h5>
                            </div>
                            <div id="formBase" class="ui small form segment">
                                <div class="fields">
                                    <div class="three wide field">
                                        <label>所在位置</label>
                                        <div class="ui green basic large label"><%:Req_DBS %></div>
                                    </div>
                                    <div class="three wide field">
                                        <label>系統編號</label>
                                        <div class="ui red basic large label">
                                            <asp:Literal ID="lt_SeqNo" runat="server">資料建立中</asp:Literal>
                                        </div>
                                    </div>
                                    <div class="five wide required field">
                                        <label>類別</label>
                                        <asp:DropDownList ID="ddl_ClsLv1" runat="server" CssClass="fluid"></asp:DropDownList>
                                    </div>
                                    <div class="five wide required field">
                                        <label>用途</label>
                                        <asp:DropDownList ID="ddl_ClsLv2" runat="server" CssClass="fluid"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="fields">
                                    <div class="sixteen wide required field">
                                        <label>名稱</label>
                                        <asp:TextBox ID="tb_AName" runat="server" MaxLength="100" placeholder="100字" autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="fields">
                                    <div class="six wide field">
                                        <label>上線日</label>
                                        <div class="ui left icon input datepicker">
                                            <asp:TextBox ID="tb_OnlineDate" runat="server" MaxLength="20" autocomplete="off" placeholder="格式:西元年/月/日"></asp:TextBox>
                                            <i class="calendar alternate outline icon"></i>
                                        </div>
                                    </div>
                                    <div class="ten wide field">
                                        <label>維護日期</label>
                                        <div class="two fields">
                                            <div class="field">
                                                <div class="ui left icon input datepicker">
                                                    <asp:TextBox ID="tb_StartDate" runat="server" placeholder="開始日" autocomplete="off"></asp:TextBox>
                                                    <i class="calendar alternate outline icon"></i>
                                                </div>
                                            </div>
                                            <div class="field">
                                                <div class="ui left icon input datepicker">
                                                    <asp:TextBox ID="tb_EndDate" runat="server" placeholder="結束日" autocomplete="off"></asp:TextBox>
                                                    <i class="calendar alternate icon"></i>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="fields">
                                    <div class="six wide field">
                                        <label>IP</label>
                                        <asp:TextBox ID="tb_IPAddr" runat="server" MaxLength="100" placeholder="50字" autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="ten wide field">
                                        <label>網址 (http,https)</label>
                                        <asp:TextBox ID="tb_WebUrl" runat="server" MaxLength="100" placeholder="200字" autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="fields">
                                    <div class="sixteen wide field">
                                        <label>說明</label>
                                        <asp:TextBox ID="tb_Remark" runat="server" MaxLength="100" placeholder="500字" autocomplete="off" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="ui right aligned segment">
                                <button id="doSaveBase" type="button" class="ui green small button">
                                    <i class="save icon"></i>
                                    <asp:Literal ID="lt_SaveBase" runat="server">完成基本資料，繼續設定資產清單</asp:Literal></button>
                                <asp:Button ID="btn_doSaveBase" runat="server" Text="Save" OnClick="btn_doSaveBase_Click" Style="display: none;" />
                                <asp:HiddenField ID="hf_DataID" runat="server" />
                            </div>
                            <%--<div class="ui bottom attached info small message">
                                <ul>
                                    <li>沒什麼好說的。</li>
                                </ul>
                            </div>--%>
                        </div>
                        <!-- Section-基本資料 End -->

                        <asp:PlaceHolder ID="ph_Details" runat="server" Visible="false">
                            <!-- Section-選擇資產 Start -->
                            <div class="ui segments">
                                <div class="ui brown segment">
                                    <h5 class="ui header"><a class="anchor" id="section1"></a>選擇資產 (來源:ERP)</h5>
                                </div>
                                <div class="ui segment form">
                                    <div class="fields">
                                        <div class="twelve wide required field">
                                            <div class="ui fluid search ac-Asset">
                                                <div class="ui left icon right labeled input">
                                                    <asp:TextBox ID="filter_AssetVal" runat="server" CssClass="prompt" placeholder="輸入關鍵字"></asp:TextBox>
                                                    <i class="search icon"></i>
                                                    <asp:Panel ID="lb_AssetVal" runat="server" CssClass="ui label">輸入關鍵字,選擇項目</asp:Panel>
                                                </div>
                                                <asp:TextBox ID="val_AssetVal" runat="server" Style="display: none"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="four wide field">
                                            <asp:Button ID="btn_SaveDetail" runat="server" Text="新增" CssClass="ui teal small button" OnClick="btn_SaveDetail_Click" />

                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- Section-選擇資產 End -->

                            <!-- Section-資產清單 Start -->
                            <div class="ui segments">
                                <div class="ui blue segment">
                                    <h5 class="ui header"><a class="anchor" id="section2"></a>資產清單</h5>
                                </div>
                                <div class="ui segment">
                                    <asp:ListView ID="lv_Detail" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_Detail_ItemCommand">
                                        <LayoutTemplate>
                                            <table id="table1" class="ui celled selectable compact small table" style="width: 100%;">
                                                <thead>
                                                    <tr>
                                                        <th class="grey-bg lighten-3 center aligned">資產編號</th>
                                                        <th class="grey-bg lighten-3">名稱</th>
                                                        <th class="grey-bg lighten-3 center aligned">取得日期</th>
                                                        <th class="grey-bg lighten-3 right aligned">取得金額</th>
                                                        <th class="grey-bg lighten-3 center aligned">供應商</th>
                                                        <th class="grey-bg lighten-3 center aligned">保管人</th>
                                                        <th class="grey-bg lighten-3"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                </tbody>
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="center aligned green-text text-darken-2">
                                                    <h4><%#Eval("ID") %></h4>
                                                </td>
                                                <td class="center aligned">
                                                    <%#Eval("Label") %>
                                                </td>
                                                <td class="center aligned"><%#Eval("GetItemDate") %></td>
                                                <td class="right aligned">
                                                    <%# String.Format("{0:#,0}",Eval("GetItemMoney"))%>
                                                </td>
                                                <td class="center aligned"><%#Eval("SupName") %></td>
                                                <td class="center aligned"><%#Eval("Who") %></td>
                                                <td class="center aligned collapsing">
                                                    <asp:LinkButton ID="lbtn_Del" runat="server" CssClass="ui small orange basic icon button" ValidationGroup="List" CommandName="doDel" OnClientClick="return confirm('確定刪除?')"><i class="trash alternate icon"></i></asp:LinkButton>

                                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="ui placeholder segment">
                                                <div class="ui icon header">
                                                    <i class="coffee icon"></i>
                                                    尚未加入
                                                </div>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <!-- Section-資產清單 End -->

                            <!-- Section-維護資訊 Start -->
                            <div class="ui segments">
                                <div class="ui grey segment">
                                    <h5 class="ui header"><a class="anchor" id="infoData"></a>維護資訊</h5>
                                </div>
                                <div class="ui segment">
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
                                                    <asp:Literal ID="info_Creater" runat="server">資料建立中...</asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="info_CreateTime" runat="server">資料建立中...</asp:Literal>
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
                            <!-- Section-維護資訊 End -->
                        </asp:PlaceHolder>
                    </div>

                </div>
                <!-- Left Body Content End -->

                <!-- Right Navi Menu Start -->
                <div class="three wide column">
                    <div class="ui sticky">
                        <div id="fastjump" class="ui secondary vertical pointing fluid text menu">
                            <div class="header item">快速跳轉<i class="dropdown icon"></i></div>
                            <a href="#baseData" class="item">基本設定</a>
                            <a href="#section1" class="item">選擇資產</a>
                            <a href="#section2" class="item">資產清單</a>
                            <a href="#top" class="item"><i class="angle double up icon"></i>到頂端</a>
                        </div>

                        <div class="ui vertical text menu">
                            <div class="header item">功能按鈕</div>
                            <div class="item">
                                <a href="<%:Page_SearchUrl %>" class="ui small button"><i class="undo icon"></i>返回列表</a>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Right Navi Menu End -->
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
            //Save Click
            $("#doSaveBase").click(function () {
                $("#formBase").addClass("loading");
                $("#MainContent_btn_doSaveBase").trigger("click");
            });

            //init dropdown list
            $('select').dropdown();

        });
    </script>

    <%-- 快速選單 --%>
    <script src="<%=fn_Params.WebUrl %>js/sticky.js"></script>
    <%-- 日期選擇器 Start --%>
    <link href="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.css" rel="stylesheet" />
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.js"></script>
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').calendar(calendarOpts_Range_unLimit);
        });
    </script>
    <%-- 日期選擇器 End --%>

    <%-- Search UI Start --%>
    <script>
        /* MIS資產 (使用category) */
        $('.ac-Asset').search({
            type: 'category',
            minCharacters: 2,
            searchFields: [
                'title',
                'description'
            ]
            , onSelect: function (result, response) {
                //console.log(result.title);
                $("#MainContent_val_AssetVal").val(result.title);
                $("#MainContent_lb_AssetVal").text(result.description);

            }
            , apiSettings: {
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Asset.ashx?dbs=<%=Req_DBS%>&q={query}&v=1.0',
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

