<%@ Page Title="上海BBC | 匯入記錄" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportList.aspx.cs" Inherits="mySHBBC_ImportList" %>

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
                        匯入記錄
                    </div>
                </div>
            </div>
            <div class="right menu">
                <a href="RefCopmg.aspx?Mall=1" class="item"><i class="cogs icon"></i><span class="mobile hidden">客戶商品對應</span></a>
                <a href="PromoConfig.aspx" class="item"><i class="cogs icon"></i><span class="mobile hidden">促銷活動設定</span></a>
                <a href="ImportIndex.aspx" class="item"><i class="plus icon"></i><span class="mobile hidden">新增匯入</span></a>
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
                    <div class="six wide field">
                        <label>建立日期</label>
                        <div class="two fields">
                            <div class="field">
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_sDate" runat="server" placeholder="開始日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate outline icon"></i>
                                </div>
                            </div>
                            <div class="field">
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_eDate" runat="server" placeholder="結束日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate outline icon"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="four wide field">
                        <label>商城</label>
                        <asp:DropDownList ID="filter_Mall" runat="server"></asp:DropDownList>
                    </div>
                    <div class="six wide field">
                        <label>關鍵字查詢</label>
                        <asp:TextBox ID="filter_Keyword" runat="server" autocomplete="off" placeholder="查詢:平台單號, 客戶, 追蹤編號, 物流單號, 電話" MaxLength="20"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="ui two column grid">
                <div class="column">
                    <a href="<%=FuncPath() %>" class="ui small button"><i class="refresh icon"></i>重置條件</a>
                </div>
                <div class="column right aligned">
                    <button type="button" id="doSearch" class="ui blue small button"><i class="search icon"></i>查詢</button>
                    <asp:Button ID="btn_Search" runat="server" Text="Button" OnClick="btn_Search_Click" Style="display: none" />
                </div>
            </div>

        </div>
        <!-- Advance Search End -->

        <!-- Empty Content Start -->
        <asp:PlaceHolder ID="ph_EmptyData" runat="server">
            <div class="ui placeholder segment">
                <div class="ui icon header">
                    <i class="search icon"></i>
                    目前條件查無資料，請重新查詢。
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- Empty Content End -->

        <!-- List Content Start -->
        <asp:PlaceHolder ID="ph_Data" runat="server">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <div class="ui green attached segment">
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3">追蹤編號</th>
                                    <th class="grey-bg lighten-3">客戶</th>
                                    <th class="grey-bg lighten-3 center aligned">商城</th>
                                    <th class="grey-bg lighten-3 center aligned">狀態/類型</th>
                                    <th class="grey-bg lighten-3 center aligned">時間</th>
                                    <th class="grey-bg lighten-3 center aligned">人員</th>
                                    <th class="grey-bg lighten-3"></th>
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
                        <td class="left aligned collapsing">
                            <b class="red-text text-darken-2">
                                <asp:Literal ID="lt_LogCnt" runat="server"><i class="red-text text-darken-3 exclamation triangle icon" title="此筆有錯誤記錄"></i></asp:Literal><%#Eval("TraceID") %></b>
                        </td>
                        <td class="green-text text-darken-2">
                            <h4><%#Eval("CustName") %> (<%#Eval("CustID") %>)</h4>
                        </td>
                        <td class="center aligned collapsing">
                            <div class="ui brown fluid label"><%#Eval("MallName") %></div>
                        </td>
                        <td class="center aligned collapsing">
                            <div>
                                <div class="ui <%#GetStatusColor(Eval("Status").ToString()) %> basic fluid label">
                                    <%#Eval("StatusName") %>
                                </div>
                            </div>
                            <div>
                                <div class="ui grey basic fluid label">
                                    <%#Eval("Data_TypeName") %>
                                </div>
                            </div>
                        </td>
                        <td class="left aligned collapsing">
                            <div>
                                <div class="ui basic fluid label">
                                    建立<div class="detail"><%#Eval("Create_Time") %></div>
                                </div>
                            </div>
                            <div>
                                <div class="ui basic fluid label">
                                    匯入<div class="detail"><%#Eval("Import_Time") %></div>
                                </div>
                            </div>
                        </td>
                        <td class="left aligned collapsing">
                            <div>
                                <div class="ui basic fluid label">
                                    建立<div class="detail"><%#Eval("Create_Name") %></div>
                                </div>
                            </div>
                            <div>
                                <div class="ui basic fluid label">
                                    更新<div class="detail"><%#Eval("Update_Name") %></div>
                                </div>
                            </div>
                        </td>
                        <td class="left aligned collapsing">
                            <asp:PlaceHolder ID="ph_KeepGo" runat="server">
                                <a class="ui small teal basic icon button" href="<%#keepGoUrl(Convert.ToInt16(Eval("Status")),Eval("Data_ID").ToString()) %>" title="繼續匯入">
                                    <i class="pencil icon"></i>
                                </a>
                            </asp:PlaceHolder>
                            <a class="ui small grey basic icon button" href="<%#fn_Params.WebUrl %>mySHBBC/ImportLog.aspx?dataID=<%#Eval("Data_ID") %>" title="查看記錄">
                                <i class="file alternate icon"></i>
                            </a>
                        </td>
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

            //init dropdown list
            $('select').dropdown();


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

