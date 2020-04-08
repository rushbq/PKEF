<%@ Page Title="台灣電商BBC | Step2" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep2.aspx.cs" Inherits="myTWBBC_Mall_ImportStep2" %>

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
                        Step2.匯入Excel資料
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
                    <%--<a href="<%:FuncPath() %>/View.aspx?id=<%=Req_DataID %>">點我看詳細</a>--%>
                </div>
            </asp:PlaceHolder>
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="2" />
            <!-- 基本資料 Start -->
            <div id="formData" class="ui small form attached green segment">
                <div class="fields">
                    <div class="five wide field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="five wide field">
                        <label>商城</label>
                        <asp:Label ID="lb_Mall" runat="server" CssClass="ui brown basic large label"></asp:Label>
                    </div>
                    <div class="six wide field">
                        <label>客戶</label>
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                </div>
                <div class="ui hidden divider"></div>
                <!-- 訂單資料 Excel -->
                <div class="ui segments">
                    <div class="ui blue segment">
                        <h5 class="ui header">訂單明細</h5>
                    </div>
                    <div class="ui grey-bg lighten-5 segment">
                        <div class="ui small form">
                            <div class="fields">
                                <div class="sixteen wide required field">
                                    <label>選擇工作表-<span class="blue-text text-darken-2">訂單明細</span></label>
                                    <asp:DropDownList ID="ddl_Sheet" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Sheet_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="fields">
                                <div class="sixteen wide field">
                                    <label>Excel資料預覽</label>
                                    <table id="listTable" class="ui celled compact table nowrap" style="width: 100%">
                                        <asp:Literal ID="lt_DataHtml" runat="server"><thead><tr><th class="grey-text text-darken-1">工作表未選擇</th></tr></thead></asp:Literal>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 出貨資料 Excel -->
                <asp:PlaceHolder ID="ph_ShipDetail" runat="server">
                    <div class="ui segments">
                        <div class="ui brown segment">
                            <h5 class="ui header">出貨資料</h5>
                        </div>
                        <div class="ui grey-bg lighten-5 segment">
                            <div class="ui small form">
                                <div class="fields">
                                    <div class="sixteen wide field">
                                        <label>選擇工作表-<span class="brown-text text-darken-2">出貨資料</span></label>
                                        <asp:DropDownList ID="ddl_ShipSheet" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_ShipSheet_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="fields">
                                    <div class="sixteen wide field">
                                        <label>Excel資料預覽</label>
                                        <table id="shipTable" class="ui celled compact table nowrap" style="width: 100%">
                                            <asp:Literal ID="lt_ShipHtml" runat="server"><thead><tr><th class="grey-text text-darken-1">工作表未選擇</th></tr></thead></asp:Literal>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="ph_WorkBtns" runat="server">
                    <div class="ui grid">
                        <div class="six wide column">
                            <a href="<%=Page_SearchUrl %>" class="ui button"><i class="undo icon"></i>返回列表</a>
                            <asp:LinkButton ID="lbtn_ReNew" runat="server" CssClass="ui orange button" OnClick="lbtn_ReNew_Click" OnClientClick="return confirm('是否確定?\n1.資料將會重新設定\n2.舊的匯入檔案會刪除')"><i class="recycle icon"></i>重新上傳</asp:LinkButton>
                        </div>
                        <div class="ten wide column right aligned">
                            <button id="doNext" type="button" class="ui green button">下一步<i class="chevron right icon"></i></button>
                            <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                        </div>

                        <asp:HiddenField ID="hf_FullFileName" runat="server" />
                        <asp:HiddenField ID="hf_ShipFullFileName" runat="server" />
                        <asp:HiddenField ID="hf_TraceID" runat="server" />
                        <asp:HiddenField ID="hf_DataID" runat="server" />
                    </div>
                </asp:PlaceHolder>
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

            //Click:下一步
            $("#doNext").click(function () {
                $("#formData").addClass("loading");
                $("#MainContent_btn_Next").trigger("click");
            });
        });
    </script>

    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script>
        $(function () {
            //使用DataTable
            $('#listTable').DataTable({
                "searching": false,  //搜尋
                "ordering": false,   //排序
                "paging": true,     //分頁
                "info": true,      //頁數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                "pageLength": 20,   //顯示筆數預設值     
                //捲軸設定
                "scrollY": '40vh',
                "scrollCollapse": true,
                "scrollX": true
            });
            $('#shipTable').DataTable({
                "searching": false,  //搜尋
                "ordering": false,   //排序
                "paging": false,     //分頁
                "info": false,      //頁數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                "pageLength": 20,   //顯示筆數預設值     
                //捲軸設定
                "scrollY": '40vh',
                "scrollCollapse": true,
                "scrollX": true
            });

        });
    </script>

    <%-- DataTable End --%>
</asp:Content>

