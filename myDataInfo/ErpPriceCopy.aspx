<%@ Page Title="ERP資料複製" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ErpPriceCopy.aspx.cs" Inherits="myDataInfo_ErpPriceCopy" %>

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
                    <div class="active section">
                        ERP資料複製 (核價/報價)
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                <div class="ui negative message">
                    <div class="header">
                        Oops...發生了一點小問題
                    </div>
                    <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>

            <!-- 基本資料 Start -->
            <div id="formData" class="ui form attached green segment">
                <div class="fields">
                    <div class="three wide field">
                        <label>來源資料庫&nbsp;<a href="#!" id="openTip1" class="black-text"><i class="question circle icon"></i></a></label>
                        <asp:DropDownList ID="ddl_SrcDB" runat="server" CssClass="fluid">
                            <asp:ListItem Value="">請選擇</asp:ListItem>
                            <asp:ListItem Value="TW">台灣</asp:ListItem>
                            <asp:ListItem Value="SH">上海</asp:ListItem>
                            <asp:ListItem Value="SZ">深圳</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="seven wide field">
                        <label>來源單別/單號</label>
                        <div class="fields">
                            <div class="six wide field">
                                <asp:TextBox ID="tb_PrimaryID" runat="server" CssClass="fluid" placeholder="單別" MaxLength="4" autocomplete="off"></asp:TextBox>
                            </div>
                            <div class="ten wide field">
                                <asp:TextBox ID="tb_SubID" runat="server" CssClass="fluid" placeholder="單號" MaxLength="11" autocomplete="off"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="three wide field">
                        <label>設定生效日</label>
                        <div class="ui left icon input datepicker">
                            <asp:TextBox ID="tb_validDate" runat="server" placeholder="生效日" autocomplete="off"></asp:TextBox>
                            <i class="calendar alternate outline icon"></i>
                        </div>
                    </div>
                    <div class="three wide field">
                        <label>設定失效日</label>
                        <div class="ui left icon input datepicker">
                            <asp:TextBox ID="tb_invalidDate" runat="server" placeholder="失效日" autocomplete="off"></asp:TextBox>
                            <i class="calendar alternate outline icon"></i>
                        </div>
                    </div>
                </div>
                <div class="fields">
                    <div class="three wide required field">
                        <label>目標資料庫</label>
                        <asp:DropDownList ID="ddl_TarDB" runat="server" CssClass="fluid" AutoPostBack="true" OnSelectedIndexChanged="ddl_TarDB_SelectedIndexChanged">
                            <asp:ListItem Value="">請選擇</asp:ListItem>
                            <asp:ListItem Value="TW">台灣</asp:ListItem>
                            <asp:ListItem Value="SH">上海</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="five wide required field">
                        <label>採購核價單 / 業務報價單</label>
                        <asp:DropDownList ID="ddl_flowType" runat="server" CssClass="fluid" AutoPostBack="true" OnSelectedIndexChanged="ddl_flowType_SelectedIndexChanged">
                            <asp:ListItem Value="0">請選擇</asp:ListItem>
                            <asp:ListItem Value="A">採購核價單</asp:ListItem>
                            <asp:ListItem Value="B">業務報價單</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="five wide field">
                        <label>目標單別&nbsp;<span class="blue-text">(請先選擇核價單或報價單)</span></label>
                        <asp:DropDownList ID="ddl_TarTypeID" runat="server" CssClass="fluid">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="ui message">
                    <div class="header">注意事項</div>
                    <ul class="list">
                        <li>複製完成後，單號將由系統取號，單據的狀態為「<span class="red-text text-darken-1">未確認</span>」。</li>
                        <li>單號的編碼組合為 <b class="red-text">EF</b> + 年月日後6碼 + 流水號3碼。</li>
                        <li class="blue-text text-darken-4">廠商核價單：
                            <ul>
                                <li>1. 複製後不能修改供應商代號，如果跨公司別複製，要確認二邊的供應商代號相同</li>
                                <li>2. 單身的「最低補量」，會自動帶出品號基本資料的「最低補量」，如果跨公司別複製，要特別注意這點</li>
                            </ul>
                        </li>
                        <li class="green-text text-darken-4">客戶報價單：
                            <ul>
                                <li>1. 單身的內盒數量，會帶出品號基本資料的「內盒數量」，如果跨公司別複製，要特別注意這點</li>
                            </ul>
                        </li>
                    </ul>
                </div>

                <div class="ui grid">
                    <div class="six wide column">
                        <a href="<%=thisPage %>" class="ui button"><i class="undo icon"></i>重新選擇</a>
                    </div>
                    <div class="ten wide column right aligned">
                        <button id="doNext" type="button" class="ui green button">開始複製<i class="chevron right icon"></i></button>
                        <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                    </div>
                </div>
            </div>
            <!-- 基本資料 End -->
        </div>
    </div>
    <!-- 內容 End -->
    <!-- Modal Start -->
    <div id="showTip1" class="ui modal">
        <div class="header">來源資料庫</div>
        <div class="scrolling content">
            <p>
                要從哪個資料庫取得資料,複製內容將以此資料庫為主。
            </p>
        </div>
        <div class="actions">
            <div class="ui black deny button">
                Close
            </div>
        </div>
    </div>
    <!--  Modal End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //init dropdown list
            $('select').dropdown();

            //Save Click
            $("#doNext").click(function () {
                if (confirm("確認開始複製?")) {
                    $("#formData").addClass("loading");
                    $("#MainContent_btn_Next").trigger("click");
                }
            });

            //說明視窗(Modal)
            $("#openTip1").click(function () {
                $('#showTip1').modal('show');
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
            $('.datepicker').calendar(calendarOpts_Range_unLimit);
        });
    </script>
    <%-- 日期選擇器 End --%>
</asp:Content>

