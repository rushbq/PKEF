<%@ Page Title="促銷活動設定" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="PromoConfig.aspx.cs" Inherits="myTWBBC_Mall_PromoConfig" %>

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
                    <div class="section">台灣電商BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        促銷活動設定
                    </div>
                </div>
            </div>
            <div class="right menu">
                <a href="<%=FuncPath() %>ImportList.aspx" class="item"><i class="undo icon"></i><span class="mobile hidden">返回BBC</span></a>
                <a href="<%=FuncPath() %>PromoConfig_Edit.aspx" class="item"><i class="plus icon"></i><span class="mobile hidden">新增活動</span></a>
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
                    <div class="four wide field">
                        <label>活動日期</label>
                        <div class="field">
                            <div class="ui left icon input datepicker">
                                <asp:TextBox ID="filter_sDate" runat="server" placeholder="活動日期" autocomplete="off"></asp:TextBox>
                                <i class="calendar alternate outline icon"></i>
                            </div>
                        </div>
                        <%--<div class="two fields">
                            
                            <div class="field">
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_eDate" runat="server" placeholder="結束日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate outline icon"></i>
                                </div>
                            </div>
                        </div>--%>
                    </div>
                    <div class="four wide field">
                        <label>商城</label>
                        <asp:DropDownList ID="filter_Mall" runat="server"></asp:DropDownList>
                    </div>
                    <div class="four wide field">
                        <label>關鍵字</label>
                        <asp:TextBox ID="filter_Keyword" runat="server" autocomplete="off" placeholder="活動名稱" MaxLength="20"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="ui two column grid">
                <div class="column">
                    <a href="<%=FuncPath() %>PromoConfig.aspx" class="ui small button"><i class="refresh icon"></i>重置條件</a>
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
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <div class="ui green attached segment">
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3">活動名稱</th>
                                    <th class="grey-bg lighten-3 center aligned">活動類型</th>
                                    <th class="grey-bg lighten-3 center aligned">對應商城</th>
                                    <th class="grey-bg lighten-3 center aligned">活動時間</th>
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
                    <tr id="trItem" runat="server">
                        <td>
                            <asp:Literal ID="lt_Warning" runat="server"></asp:Literal>
                            <strong class="blue-text"><%#Eval("PromoName") %></strong>
                        </td>
                        <td class="center aligned"><%#Eval("TypeName") %></td>
                        <td class="center aligned"><%#Eval("MallName") %></td>
                        <td class="center aligned collapsing">
                            <div>
                                <div class="ui basic fluid label green-text text-darken-3">
                                    開始
                                    <div class="detail"><%#Eval("StartTime") %></div>
                                </div>
                            </div>
                            <div>
                                <div class="ui basic fluid label red-text text-darken-2">
                                    結束
                                    <div class="detail"><%#Eval("EndTime") %></div>
                                </div>
                            </div>
                        </td>
                        <td class="center aligned collapsing">
                            <a class="ui small teal basic icon button" href="<%=FuncPath() %>PromoConfig_Edit.aspx?id=<%#Eval("Data_ID") %>" title="編輯">
                                <i class="pencil icon"></i>
                            </a>
                            <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui small orange basic icon button" ValidationGroup="List" CommandName="doClose" OnClientClick="return confirm('確定刪除?')"><i class="trash alternate icon"></i></asp:LinkButton>

                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
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
            $('.datepicker').calendar(calendarOpts_Range_unLimit);
        });
    </script>
    <%-- 日期選擇器 End --%>
</asp:Content>

