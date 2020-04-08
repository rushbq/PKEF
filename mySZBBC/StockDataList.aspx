<%@ Page Title="電商庫存表" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="StockDataList.aspx.cs" Inherits="mySZBBC_StockDataList" %>

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
                    <div class="section">深圳BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        電商庫存表
                    </div>
                </div>
            </div>
            <div class="right menu">
                <asp:LinkButton ID="lbtn_Excel" runat="server" OnClick="lbtn_Excel_Click" CssClass="item"><i class="file excel icon"></i><span class="mobile hidden">匯出資料</span></asp:LinkButton>

                <a class="item" href="<%=fn_Params.WebUrl %>mySZBBC/StockImportStep1.aspx">
                    <i class="plus icon"></i>
                    <span class="mobile hidden">匯入庫存</span>
                </a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <!-- Search Start -->
        <div class="ui orange attached segment">
            <div class="ui small form">
                <div class="fields">
                    <div class="four wide field">
                        <label>日期</label>
                        <div class="ui left icon input datepicker">
                            <asp:TextBox ID="filter_setDate" runat="server" autocomplete="off"></asp:TextBox>
                            <i class="calendar alternate outline icon"></i>
                        </div>
                    </div>
                    <div class="eight wide field">
                        <label>品號</label>
                        <div class="two fields">
                            <div class="field">
                                <asp:TextBox ID="filter_Keyword" runat="server" autocomplete="off" placeholder="輸入關鍵字"></asp:TextBox>
                            </div>
                            <div class="field">
                                <button type="button" id="doSearch" class="ui blue small button"><i class="search icon"></i>查詢</button>
                                <asp:Button ID="btn_Search" runat="server" Text="Button" OnClick="btn_Search_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Search End -->

        <!-- Empty Content Start -->
        <asp:PlaceHolder ID="ph_EmptyData" runat="server">
            <div class="ui placeholder segment">
                <div class="ui icon header">
                    <i class="search icon"></i>
                    目前條件查無資料，請重新查詢或匯入新資料。
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- Empty Content End -->

        <!-- List Content Start -->
        <asp:PlaceHolder ID="ph_Data" runat="server">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <div class="ui green attached segment">
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3" rowspan="2">品號</th>
                                    <th class="grey-bg lighten-3 center aligned" colspan="2">京東POP</th>
                                    <th class="grey-bg lighten-3 center aligned" colspan="2">天貓</th>
                                    <th class="grey-bg lighten-3 center aligned" colspan="2">唯品會</th>
                                    <th class="grey-bg lighten-3 center aligned" colspan="2">京東廠送</th>
                                    <th class="grey-bg lighten-3 center aligned" colspan="3">上海倉(B01)</th>
                                </tr>
                                <tr>
                                    <!-- 京東POP -->
                                    <th class="grey-bg lighten-3 center aligned">SKU</th>
                                    <th class="grey-bg lighten-3 right aligned">庫存</th>
                                    <!-- 天貓 -->
                                    <th class="grey-bg lighten-3 center aligned">SKU</th>
                                    <th class="grey-bg lighten-3 right aligned">庫存</th>
                                    <!-- 唯品會 -->
                                    <th class="grey-bg lighten-3 center aligned">SKU</th>
                                    <th class="grey-bg lighten-3 right aligned">庫存</th>
                                    <!-- 京東廠送 -->
                                    <th class="grey-bg lighten-3 center aligned">SKU</th>
                                    <th class="grey-bg lighten-3 right aligned">庫存</th>
                                    <!-- 上海倉 -->
                                    <th class="grey-bg lighten-3 right aligned">庫存</th>
                                    <th class="grey-bg lighten-3 right aligned">預計銷</th>
                                    <th class="grey-bg lighten-3 right aligned">預計進</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tbody>
                        </table>
                    </div>

                    <!-- List Pagination Start -->
                    <div class="ui mini bottom attached segment grey-bg lighten-4">
                        <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                    </div>
                    <!-- List Pagination End -->
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <strong class="blue-text text-darken-3"><%#Eval("ModelNo") %></strong>
                        </td>
                        <td class="grey-text text-darken-2 center aligned"><%#Eval("EC1_SKU") %></td>
                        <td class="right aligned"><%#Eval("EC1_StockNum") %></td>
                        <td class="grey-text text-darken-2 center aligned"><%#Eval("EC2_SKU") %></td>
                        <td class="right aligned"><%#Eval("EC2_StockNum") %></td>
                        <td class="grey-text text-darken-2 center aligned"><%#Eval("EC3_SKU") %></td>
                        <td class="right aligned"><%#Eval("EC3_StockNum") %></td>
                        <td class="grey-text text-darken-2 center aligned"><%#Eval("EC4_SKU") %></td>
                        <td class="right aligned"><%#Eval("EC4_StockNum") %></td>
                        <td class="right aligned positive"><strong><%#Eval("StockQty_B01") %></strong></td>
                        <td class="right aligned positive"><strong><%#Eval("PreSell_B01") %></strong></td>
                        <td class="right aligned positive"><strong><%#Eval("PreIN_B01") %></strong></td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </asp:PlaceHolder>
        <!-- List Content End -->

    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
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

        });
    </script>
    <%-- 日期選擇器 Start --%>
    <link href="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.css" rel="stylesheet" />
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.js"></script>
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').calendar(calendarOpts_Range);
        });
    </script>
    <%-- 日期選擇器 End --%>
</asp:Content>

