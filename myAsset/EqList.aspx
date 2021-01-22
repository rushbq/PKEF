<%@ Page Title="機房資訊資產管理" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="EqList.aspx.cs" Inherits="myAsset_EqList" %>

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
                <a href="EqEdit.aspx?dbs=<%:Req_DBS %>" class="item"><i class="plus icon"></i><span class="mobile hidden">新增</span></a>
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
                        <label>維護日期(起)</label>
                        <div class="two fields">
                            <div class="field">
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_sDateA" runat="server" placeholder="開始日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate outline icon"></i>
                                </div>
                            </div>
                            <div class="field">
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_eDateA" runat="server" placeholder="結束日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate icon"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="six wide field">
                        <label>維護日期(訖)</label>
                        <div class="two fields">
                            <div class="field">
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_sDateB" runat="server" placeholder="開始日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate outline icon"></i>
                                </div>
                            </div>
                            <div class="field">
                                <div class="ui left icon input datepicker">
                                    <asp:TextBox ID="filter_eDateB" runat="server" placeholder="結束日" autocomplete="off"></asp:TextBox>
                                    <i class="calendar alternate icon"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="four wide field">
                    </div>
                </div>
                <div class="fields">
                    <div class="three wide field">
                        <label>類別</label>
                        <asp:DropDownList ID="filter_ClsLv1" runat="server" CssClass="fluid"></asp:DropDownList>
                    </div>
                    <div class="three wide field">
                        <label>用途</label>
                        <asp:DropDownList ID="filter_ClsLv2" runat="server" CssClass="fluid"></asp:DropDownList>
                    </div>
                    <div class="three wide field">
                        <label>資產編號</label>
                        <asp:TextBox ID="filter_ErpID" runat="server" autocomplete="off" placeholder="查詢:資產編號關鍵字" MaxLength="20"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="ui two column grid">
                <div class="column">
                    <a href="<%=thisPage %>" class="ui small button"><i class="refresh icon"></i>重置條件</a>
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
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
                <LayoutTemplate>
                    <div class="ui green attached segment">
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3 collapsing">系統序號</th>
                                    <th class="grey-bg lighten-3 center aligned">類別</th>
                                    <th class="grey-bg lighten-3 center aligned">用途</th>
                                    <th class="grey-bg lighten-3">名稱</th>
                                    <th class="grey-bg lighten-3 center aligned">上線日</th>
                                    <th class="grey-bg lighten-3 center aligned">維護日</th>
                                    <th class="grey-bg lighten-3 center aligned">IP/網址</th>
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
                        <td class="center aligned">
                            <%#Eval("SeqNo") %>
                        </td>
                        <td class="center aligned collapsing">
                            <div class="ui brown basic fluid label"><%#Eval("ClsLv1Name") %></div>
                        </td>
                        <td class="center aligned collapsing">
                            <div class="ui blue basic fluid label"><%#Eval("ClsLv2Name") %></div>
                        </td>
                        <td>
                            <h5>
                                <%#Eval("AName") %>
                            </h5>
                        </td>
                        <td class="center aligned collapsing">
                            <div>
                                <div class="ui basic fluid label"><%#Eval("OnlineDate").ToString().ToDateString("yyyy/MM/dd") %></div>
                            </div>
                        </td>

                        <td class="left aligned collapsing">
                            <div>
                                <div class="ui basic fluid label">
                                    開始<div class="detail"><%#Eval("StartDate").ToString().ToDateString("yyyy/MM/dd") %></div>
                                </div>
                            </div>
                            <div>
                                <div class="ui basic fluid label">
                                    結束<div class="detail"><%#Eval("EndDate").ToString().ToDateString("yyyy/MM/dd") %></div>
                                </div>
                            </div>
                        </td>
                        <td class="left aligned collapsing">
                            <div>
                                <div class="ui basic fluid label">
                                    IP<div class="detail"><%#Eval("IPAddr") %></div>
                                </div>
                            </div>
                            <div>
                                <div class="ui basic fluid label">
                                    網址<div class="detail"><%#Eval("WebUrl") %></div>
                                </div>
                            </div>
                        </td>
                        <td class="center aligned collapsing">
                            <a class="ui small teal basic icon button" href="EqEdit.aspx?dbs=<%:Req_DBS %>&id=<%#Eval("Data_ID") %>" title="編輯">
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

