<%@ Page Title="報價單匯入 | Step1" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep1.aspx.cs" Inherits="myErpPriceData_ImportStep1" %>

<%@ Register Src="Ascx_StepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
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
                    <div class="section">報價單匯入</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section">
                        Step1.上傳Excel&nbsp;&nbsp;(<span class="red-text text-darken-2"><%=Req_DBS %></span>)
                    </h5>
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
            <div id="formData" class="ui small form attached green segment">
                <div class="two fields">
                    <div class="field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="field">
                        <label>資料庫</label>
                        <asp:Label ID="lb_DBS" runat="server" CssClass="ui blue basic large label"></asp:Label>
                    </div>
                </div>
                <div class="two fields">
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
                    <div class="field">
                        <label>ERP單別</label>
                        <asp:DropDownList ID="ddl_OrderType" runat="server">
                            <asp:ListItem Value="">請選擇</asp:ListItem>
                        </asp:DropDownList>
                        <asp:TextBox ID="val_OrderType" runat="server" Style="display: none;"></asp:TextBox>
                    </div>
                </div>
                <div class="two fields">
                    <div class="field">
                        <label>生效日</label>
                        <div class="ui left icon input datepicker">
                            <asp:TextBox ID="tb_validDate" runat="server" placeholder="生效日" autocomplete="off"></asp:TextBox>
                            <i class="calendar alternate outline icon"></i>
                        </div>
                    </div>
                    <div class="disabled field">
                        <label>單號 <small>(依單別決定是否填寫)</small></label>
                        <asp:TextBox ID="tb_OrderNo" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="fields">
                    <div class="sixteen wide required field">
                        <label>
                            上傳檔案</label>
                        <asp:FileUpload ID="fu_File" runat="server" AllowMultiple="false" accept=".xlsx" />
                    </div>
                </div>
                <div class="ui message">
                    <div class="header">注意事項</div>
                    <ul class="list">
                        <li class="red-text text-darken-2"><h2>無分量計價導入功能</h2></li>
                        <li class="red-text text-darken-1"><strong>副檔名限制：.xlsx</strong></li>
                        <li>請注意Excel格式及欄位順序，若任意更改欄位位置，系統會抓不到正確的資料。</li>
                        <li>Excel不要留空白列。</li>
                        <li>若要變更格式，請先與資訊部討論。</li>
                        <li><a href="<%=fn_Params.RefUrl %>PKEF/Samples/ErpPriceData.xlsx">範本下載</a></li>
                    </ul>
                </div>

                <div class="ui grid">
                    <div class="six wide column">
                        <a href="<%=Page_SearchUrl %>" class="ui button"><i class="undo icon"></i>返回列表</a>
                    </div>
                    <div class="ten wide column right aligned">
                        <button id="doNext" type="button" class="ui green button">下一步<i class="chevron right icon"></i></button>
                        <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                    </div>

                    <asp:HiddenField ID="hf_TraceID" runat="server" />
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


            //單別onchange
            $("#MainContent_ddl_OrderType").change(function () {
                var _getVal = $(this).val();
                var _aryVal = _getVal.split("#");

                //單別
                var _typeID = _aryVal[0];
                //編碼方式
                var _way = _aryVal[1];

                //Check
                if (_typeID == "" || _way == "") {
                    alert('請選擇正確的單別');
                    return false;
                }

                //填入單別至隱藏欄位(後端取用及檢查)
                $("#MainContent_val_OrderType").val(_typeID);

                //單號欄位
                var fld_OrderNo = $("#MainContent_tb_OrderNo");
                if (_way == "4") {
                    //開放手填單號
                    fld_OrderNo.prop("disabled", false).prop("readonly", false).prop("placeholder", "請填寫單號");
                    fld_OrderNo.parent().removeClass("disabled field").addClass("required error field");
                } else {
                    fld_OrderNo.prop("disabled", true).prop("readonly", true).prop("placeholder", "").val('');
                    fld_OrderNo.parent().removeClass("required error field").addClass("disabled field");
                }

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
                url: '<%=fn_Params.WebUrl%>Ajax_Data/GetData_Customer_v1.ashx?dbs=<%=Req_DBS%>&q={query}'
            }

        });
    </script>
    <%-- Search UI End --%>
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

