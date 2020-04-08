<%@ Page Title="出貨明細表" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ShipmentList.aspx.cs" Inherits="myTWBBC_Mall_ShipmentList" %>

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
                        出貨明細表
                    </div>
                </div>
            </div>
            <div class="right menu">
                <a href="<%=fn_Params.WebUrl %>myTWBBC_Mall/ShipmentHistory.aspx" class="item"><i class="history icon"></i><span class="mobile hidden">匯出記錄</span></a>
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
            <div class="ui placeholder red attached segment">
                <div class="ui two column stackable center aligned grid">
                    <div class="ui vertical divider">Or</div>
                    <div class="middle aligned row">
                        <div class="column">
                            <div class="ui icon header">
                                <i class="attention icon"></i>
                                本次查詢的資料中，有<asp:Label ID="lb_chkCnt" runat="server" CssClass="red-text"></asp:Label>
                                筆物流單號為空白。<br />
                                空白的物流單號在匯出時會被排除。
                            </div>
                            <div class="red-text">
                                <asp:Literal ID="lt_errMsg" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="left aligned column">
                            <div style="padding-left: 15px;">
                                <div class="ui header">
                                    請執行下列步驟, 進行物流單號回寫
                                </div>
                                <div class="ui segments">
                                    <div class="ui segment">
                                        <div class="ui grid">
                                            <div class="twelve wide column">
                                                1. 下載物流單號為空白的銷貨單
                                            </div>
                                            <div class="four wide column">
                                                <asp:LinkButton ID="lbtn_doDownload" runat="server" CssClass="ui tiny orange basic button" OnClick="lbtn_doDownload_Click"><i class="file excel icon"></i>下載</asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="ui segment">
                                        2. 將下載後的Excel上傳至&nbsp;<i class="truck icon"></i><strong>貨運公司</strong>
                                        , 完成後下載貨運公司的&nbsp;<i class="clipboard list icon"></i><strong>物流單</strong>
                                    </div>
                                    <div class="ui segment">
                                        3. 上傳<strong>物流單</strong>, 完成銷貨單與物流單的對應 (若要併單,請填寫相同的物流單號)
                                    </div>
                                    <div class="ui segment">
                                        <div class="ui grid">
                                            <div class="twelve wide column">
                                                <asp:FileUpload ID="fu_ShipFile" runat="server" />
                                                <small>(預設讀取第一個工作表)</small>
                                            </div>
                                            <div class="four wide column">
                                                <asp:LinkButton ID="lbtn_JobUpload" runat="server" CssClass="ui small red basic button" OnClick="lbtn_JobUpload_Click"><i class="coffee icon"></i>匯入</asp:LinkButton>
                                            </div>
                                            <div class="sixteen wide column red-text">
                                                <b>** 請注意Excel欄位：訂單號碼(C)、十碼貨號(O)</b>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="ui segment">
                                        4. 重新按下「查詢」
                                    </div>
                                </div>
                                <!-- Steps 示意 -->
                                <div class="ui mini steps">
                                    <div class="step">
                                        <i class="file excel icon"></i>
                                        <div class="content">
                                            <div class="description">下載銷貨單</div>
                                        </div>
                                    </div>
                                    <div class="step">
                                        <i class="truck icon"></i>
                                        <div class="content">
                                            <div class="description">上傳至貨運公司</div>
                                        </div>
                                    </div>
                                    <div class="step">
                                        <i class="clipboard list icon"></i>
                                        <div class="content">
                                            <div class="description">下載物流單</div>
                                        </div>
                                    </div>
                                    <div class="step">
                                        <i class="coffee icon"></i>
                                        <div class="content">
                                            <div class="description">匯入物流單</div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
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
                                    <th class="grey-bg lighten-3">商城單號</th>
                                    <th class="grey-bg lighten-3 center aligned">商城</th>
                                    <th class="grey-bg lighten-3 center aligned">客戶</th>
                                    <th class="grey-bg lighten-3">品號</th>
                                    <th class="grey-bg lighten-3 center aligned">數量</th>
                                    <th class="grey-bg lighten-3">銷貨單號</th>
                                    <th class="grey-bg lighten-3">物流單號</th>
                                    <th class="grey-bg lighten-3">客戶名 / 收貨人地址</th>
                                    <th class="grey-bg lighten-3">手機號碼</th>
                                    <th class="grey-bg lighten-3">總金額</th>
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
                        <td class="center aligned"><%#Eval("CustName") %><br />
                            (<%#Eval("CustID") %>)</td>
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
                        <td class="right aligned red-text text-darken-1">
                            <%#Eval("TotalPrice").ToString().ToMoneyString() %>
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
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?corp=1&q={query}'
            }

        });
    </script>
    <%-- Search UI End --%>
</asp:Content>

