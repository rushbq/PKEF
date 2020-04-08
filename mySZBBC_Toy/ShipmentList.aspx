<%@ Page Title="出貨明細表" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ShipmentList.aspx.cs" Inherits="mySZBBC_ShipmentList" %>

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
                    <div class="section">玩具BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        出貨明細表
                    </div>
                </div>
            </div>
            <div class="right menu">
                <a href="<%=fn_Params.WebUrl %>mySZBBC_Toy/ShipmentHistory.aspx" class="item"><i class="history icon"></i><span class="mobile hidden">匯出記錄</span></a>
                <asp:LinkButton ID="lbtn_Excel" runat="server" OnClick="lbtn_Excel_Click" CssClass="item"><i class="file excel icon"></i><span class="mobile hidden">匯出</span></asp:LinkButton>

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
                    <div class="five wide field">
                        <label>匯入日期</label>
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

                    <div class="five wide field">
                        <label>客戶</label>
                        <div class="ui fluid search ac-Cust">
                            <div class="ui right labeled input">
                                <asp:TextBox ID="filter_Cust" runat="server" CssClass="prompt" placeholder="輸入客戶代號或名稱關鍵字"></asp:TextBox>
                                <asp:Panel ID="lb_Cust" runat="server" CssClass="ui label">輸入關鍵字,選擇項目</asp:Panel>
                            </div>
                        </div>
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

        <!-- Check Content Start -->
        <asp:PlaceHolder ID="ph_CheckData" runat="server" Visible="false">
            <div class="ui placeholder red segment">
                <div class="ui icon header">
                    <i class="attention icon"></i>
                    本次查詢的資料中，有 <asp:Label ID="lb_chkCnt" runat="server" CssClass="red-text"></asp:Label> 筆物流單號為空白。<br />
                    空白的物流單號在匯出時會被排除。
                </div>
            </div>
        </asp:PlaceHolder>
        <!-- Check Content End -->

        <!-- List Content Start -->
        <asp:PlaceHolder ID="ph_Data" runat="server">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <div class="ui green attached segment">
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3">訂單號</th>
                                    <th class="grey-bg lighten-3 center aligned">商城</th>
                                    <th class="grey-bg lighten-3 center aligned">客戶</th>
                                    <th class="grey-bg lighten-3">品號</th>
                                    <th class="grey-bg lighten-3 center aligned">訂購數</th>
                                    <th class="grey-bg lighten-3">銷貨單號</th>
                                    <th class="grey-bg lighten-3">物流單號</th>
                                    <th class="grey-bg lighten-3">客戶名 / 收貨人地址</th>
                                    <th class="grey-bg lighten-3">手機號碼</th>
                                    <th class="grey-bg lighten-3">發票類型</th>
                                    <th class="grey-bg lighten-3">開票金額</th>
                                    <th class="grey-bg lighten-3">發票抬頭</th>
                                    <th class="grey-bg lighten-3">納稅人識別號</th>
                                    <th class="grey-bg lighten-3">地址電話</th>
                                    <th class="grey-bg lighten-3">開戶行帳號</th>
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
                        <td><%#Eval("OrderID") %></td>
                        <td class="center aligned"><%#Eval("MallName") %></td>
                        <td class="center aligned"><%#Eval("CustName") %><br />(<%#Eval("CustID") %>)</td>
                        <td>
                            <strong class="blue-text text-darken-3"><%#Eval("ModelNo") %></strong>
                        </td>
                        <td class="center aligned"><%#Eval("BuyCnt") %></td>
                        <td>
                            <strong class="green-text text-darken-3"><%#Eval("Erp_SO_ID") %></strong>
                        </td>
                        <td class="<%#setCss(Eval("ShipNo")) %>">
                            <%#Eval("ShipNo") %>
                        </td>
                        <td>
                            <%#Eval("ShipWho") %><br />
                            <small><%#Eval("ShipAddr") %></small>
                        </td>
                        <td><%#Eval("ShipTel") %></td>
                        <td><%#Eval("InvType") %></td>
                        <td class="right aligned red-text text-darken-1">
                            <%#Eval("InvPrice").ToString().ToMoneyString() %>
                        </td>
                        <td><%#Eval("InvTitle") %></td>
                        <td><%#Eval("InvNumber") %></td>
                        <td><%#Eval("InvAddrInfo") %></td>
                        <td><%#Eval("InvBankInfo") %></td>
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
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js?v=1"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').calendar(calendarOptsByTime_Range);
        });
    </script>
    <%-- 日期選擇器 End --%>

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
            }
            , apiSettings: {
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?corp=3&q={query}'
            }

        });
    </script>
    <%-- Search UI End --%>
</asp:Content>

