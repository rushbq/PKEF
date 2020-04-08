<%@ Page Title="基本資料維護 | 供應商關聯設定" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="SupplierList.aspx.cs" Inherits="myDataInfo_SupplierList" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">基本資料維護</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        供應商關聯設定
                    </div>
                </div>
            </div>
            <div class="right menu">
                <a href="<%=fn_Params.WebUrl %>myDataInfo/SupplierEdit.aspx" class="item"><i class="plus icon"></i><span class="mobile hidden">新增關聯</span></a>
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
                        <label>關鍵字查詢</label>
                        <asp:TextBox ID="filter_Keyword" runat="server" autocomplete="off" placeholder="查詢:主要供應商名稱" MaxLength="20"></asp:TextBox>
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
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <div class="ui green attached segment">
                        <table class="ui celled selectable compact small table">
                            <thead>
                                <tr>
                                    <th class="grey-bg lighten-3 center aligned collapsing">系統編號</th>
                                    <th class="grey-bg lighten-3">主要供應商名稱</th>
                                    <th class="grey-bg lighten-3 center aligned">台灣</th>
                                    <th class="grey-bg lighten-3 center aligned">上海</th>
                                    <th class="grey-bg lighten-3 center aligned">深圳</th>
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
                            <%#Eval("Sup_UID") %>
                        </td>
                        <td class="green-text text-darken-2">
                            <h4><%#Eval("Sup_Name") %></h4>
                        </td>
                        <td class="center aligned">
                            <%#Eval("twCnt") %>
                        </td>
                        <td class="center aligned">
                            <%#Eval("shCnt") %>
                        </td>
                        <td class="center aligned">
                            <%#Eval("szCnt") %>
                        </td>
                        <td class="left aligned collapsing">
                            <a class="ui small teal basic icon button" href="<%#fn_Params.WebUrl %>myDataInfo/SupplierEdit.aspx?DataID=<%#Cryptograph.MD5Encrypt(Eval("Sup_UID").ToString(),DesKey) %>" title="編輯">
                                <i class="pencil icon"></i>
                            </a>
                            <a class="ui small grey basic icon button btn-OpenDetail" data-id="<%#Eval("Sup_UID") %>" title="展開明細">
                                <i class="folder open icon"></i>
                            </a>
                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Sup_UID") %>' />
                        </td>
                    </tr>
                    <%-- 帶出名單 --%>
                    <tr id="tar-Detail-<%#Eval("Sup_UID") %>" class="grey-bg lighten-5" style="display: none;">
                        <td class="right aligned">
                            <i class="horizontally flipped level up alternate icon"></i>
                        </td>
                        <td colspan="4">
                            <div class="Detail-<%#Eval("Sup_UID") %>">
                                <div class="ui icon message">
                                    <i class="notched circle loading icon"></i>
                                    <div class="content">
                                        <div class="header">
                                            資料擷取,請稍候....
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td class="center aligned">
                            <a class="ui small grey button btn-CloseDetail" data-id="<%#Eval("Sup_UID") %>">CLOSE</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </asp:PlaceHolder>
        <!-- List Content End -->

    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //[搜尋][查詢鈕] - 觸發關鍵字快查
            $("#doSearch").click(function () {
                $("#MainContent_btn_Search").trigger("click");
            });

            //[搜尋][Enter鍵] - 觸發關鍵字快查
            $("#filter_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Search").trigger("click");

                    e.preventDefault();
                }
            });

            <%--            //[列表] - collapsible Click 觸發
            $('.collapsible .collapsible-header').on('click', function (event) {
                //取得目標物件
                var target = $(this);

                //取得資料編號
                var dataID = target.attr("data-id");
                //取得目標容器
                var container = $("#" + dataID);

                //填入Ajax Html
                container.load("<%=Application["WebUrl"]%>Ajax_Data/GetHtml_Supplier.ashx?DataID=" + dataID);

            });--%>

        });
    </script>
    <script>
        $(function () {
            //按鈕 - 開明細
            $(".btn-OpenDetail").click(function () {
                var id = $(this).attr("data-id");

                boxDetail(id, true);
            });

            //按鈕 - 關明細
            $('.btn-CloseDetail').click(function () {
                var id = $(this).attr("data-id");

                boxDetail(id, false);
            });

            //FUNCTION - 明細開關
            function boxDetail(id, isOpen) {
                var myBox = $("#tar-Detail-" + id);

                if (isOpen) {
                    myBox.show();

                    loadDetail(id);

                } else {
                    myBox.hide();
                }
            }

            //Ajax - 讀取明細
            function loadDetail(id) {
                //取得目標容器
                var container = $(".Detail-" + id);

                //填入Ajax Html
                var url = "<%=fn_Params.WebUrl%>Ajax_Data/GetHtml_Supplier.ashx?DataID=" + id;
                container.load(url);
            }

        });
    </script>
</asp:Content>

