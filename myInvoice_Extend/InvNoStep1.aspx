<%@ Page Title="上海會計 | 發票回填-批次多筆" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="InvNoStep1.aspx.cs" Inherits="myInvoice_Extend_InvNoStep1" %>

<%@ Register Src="Ascx_InvNoStepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">上海會計</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">發票回填-批次多筆</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step1.查詢 & 設定基本參數
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
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="1" />
            <!-- 基本資料 Start -->
            <div id="formData" class="ui form attached green segment">
                <div class="two fields">
                    <div class="field">
                        <label>公司別</label>
                        <asp:DropDownList ID="ddl_DBS" runat="server" CssClass="fluid">
                            <asp:ListItem Value="">請選擇</asp:ListItem>
                            <%--<asp:ListItem Value="TW">台灣</asp:ListItem>--%>
                            <asp:ListItem Value="SH">上海</asp:ListItem>
                            <asp:ListItem Value="SZ">深圳</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="field">
                        <label>客戶</label>
                        <div class="ui fluid search ac-Cust">
                            <div class="ui right labeled input">
                                <asp:TextBox ID="filter_Cust" runat="server" CssClass="prompt" placeholder="輸入客戶代號或名稱關鍵字"></asp:TextBox>
                                <asp:Panel ID="lb_Cust" runat="server" CssClass="ui label">輸入關鍵字,選擇項目</asp:Panel>
                                <asp:TextBox ID="val_Cust" runat="server" Style="display: none;"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="fields">
                    <div class="sixteen wide field">
                        <label>結帳日起訖</label>
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
                                    <i class="calendar alternate icon"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="two fields">
                    <div class="required field">
                        <label>填寫發票日期</label>
                        <div class="datepicker">
                            <asp:TextBox ID="tb_InvDate" runat="server" MaxLength="10" placeholder="選擇發票日期" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="required field">
                        <label>填寫發票號碼</label>
                        <asp:TextBox ID="tb_InvNo" runat="server" MaxLength="30" placeholder="輸入發票號碼" autocomplete="off"></asp:TextBox>
                    </div>
                </div>

                <div class="ui message">
                    <div class="header">功能說明</div>
                    <ul class="list">
                        <li>請慎選<strong class="red-text text-darken-1">公司別</strong></li>
                        <li>已在開票平台上的單號不會顯示。</li>
                        <li>已有發票號碼的結帳單不會顯示。</li>
                        <li class="orange-text text-darken-4">填入發票號碼/日期，回寫至勾選的結帳單</li>
                    </ul>
                </div>

                <div class="ui grid">
                    <div class="six wide column">
                        <a href="<%=thisPage %>" class="ui button"><i class="undo icon"></i>重置</a>
                    </div>
                    <div class="ten wide column right aligned">
                        <button id="doNext" type="button" class="ui green button">下一步<i class="chevron right icon"></i></button>
                        <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                    </div>

                </div>
            </div>
            <!-- 基本資料 End -->
        </div>
    </div>
    <!-- 內容 End -->
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
                $("#formData").addClass("loading");
                $("#MainContent_btn_Next").trigger("click");
            });

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
                $("#MainContent_val_Cust").val(result.ID);
            }
            , apiSettings: {
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?q={query}'
            }

        });
    </script>
    <%-- Search UI End --%>
    <%-- 日期選擇器 Start --%>
    <link href="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.css" rel="stylesheet" />
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.js"></script>
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js?v=1"></script>
    <script>
        $(function () {
            //取得設定值(往前天數, 往後天數)
            var calOpt = getCalOptByDate(720, 7);
            //載入datepicker
            $('.datepicker').calendar(calOpt);
        });
    </script>
    <%-- 日期選擇器 End --%>
</asp:Content>

