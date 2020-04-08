<%@ Page Title="台灣電商BBC | Step1" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep1.aspx.cs" Inherits="myTWBBC_Mall_ImportStep1" %>

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
                    <div class="section">台灣電商BBC</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step1.上傳Excel
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
            <div id="formData" class="ui small form attached green segment">
                <div class="fields">
                    <div class="sixteen wide field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                </div>
                <div class="two fields">
                    <div class="required field">
                        <label>商城</label>
                        <asp:DropDownList ID="ddl_Mall" runat="server"></asp:DropDownList>
                    </div>
                    <div class="required field">
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
                <div class="two fields">
                    <div class="required field">
                        <label>
                            訂單明細檔</label>
                        <asp:FileUpload ID="fu_File" runat="server" AllowMultiple="false" accept=".xlsx" />
                    </div>
                    <div class="field">
                        <label>
                            出貨資料檔 (<span class="grey-text text-darken-2">好市多</span>)</label>
                        <asp:FileUpload ID="fu_ShipFile" runat="server" AllowMultiple="false" accept=".xlsx" />
                    </div>
                </div>
                <div class="ui message">
                    <div class="header">注意事項</div>
                    <ul class="list">
                        <li class="red-text text-darken-1"><strong>副檔名限制：.xlsx</strong></li>
                        <li>請注意Excel格式及欄位順序，若任意更改欄位位置，系統會抓不到正確的資料。</li>
                        <li>Excel不要留空白列。</li>
                        <li>若要變更格式，請先與資訊部討論。</li>
                        <li>範本下載：
                            <a href="<%=fn_Params.RefUrl %>PKEF/TWBBC_Mall_Sample/Costco_Order.xlsx">好市多訂單明細</a>、
                            <a href="<%=fn_Params.RefUrl %>PKEF/TWBBC_Mall_Sample/Costco_Ship.xlsx">好市多出貨明細</a>、
                            <%--<a href="<%=fn_Params.RefUrl %>PKEF/TWBBC_Mall_Sample/PChome.xlsx">PChome</a>--%>
                        </li>
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
            //Save Click
            $("#doNext").click(function () {
                $("#formData").addClass("loading");
                $("#MainContent_btn_Next").trigger("click");
            });


            //init dropdown list
            $('select').dropdown();

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
</asp:Content>

