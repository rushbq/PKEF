<%@ Page Title="報價單匯入 | Step2" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep2.aspx.cs" Inherits="myErpPriceData_ImportStep2" %>

<%@ Register Src="Ascx_StepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
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
                    <div class="section">報價單匯入</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section">Step2.匯入Excel資料&nbsp;&nbsp;(<span class="red-text text-darken-2"><%=Req_DBS %></span>)
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
                    <a href="<%:FuncPath() %>/View.aspx?id=<%=Req_DataID %>">點我看詳細</a>
                </div>
            </asp:PlaceHolder>
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="2" />
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
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                    <div class="field">
                        <label>ERP單別</label>
                        <asp:Label ID="lb_OrderType" runat="server" CssClass="ui basic large label"></asp:Label>
                    </div>
                </div>
                <div class="two fields">
                    <div class="field">
                        <label>生效日</label>
                        <asp:Label ID="lb_validDate" runat="server" CssClass="ui basic large label"></asp:Label>
                    </div>
                    <div class="field">
                        <label>單號 <small>(依單別決定是否填寫)</small></label>
                        <asp:Label ID="lb_OrderNo" runat="server" CssClass="ui basic large label"></asp:Label>
                    </div>
                </div>

                <div class="fields">
                    <div class="sixteen wide required field">
                        <label>選擇要匯入的工作表</label>
                        <asp:DropDownList ID="ddl_Sheet" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Sheet_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="fields">
                    <div class="sixteen wide field">
                        <label>Excel資料預覽</label>
                        <asp:ListView ID="lvViewList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="listTable" class="ui celled compact table" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th>客戶品號/寶工品號</th>
                                            <th>金額</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <b class="blue-text text-darkent-2"><%#Eval("ProdID") %></b>
                                    </td>
                                    <td>
                                        <%#Eval("InputPrice") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="ui placeholder segment">
                                    <div class="ui icon header">
                                        <i class="wheelchair icon"></i>
                                        查無資料
                                    </div>
                                </div>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
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
                        <asp:HiddenField ID="hf_TraceID" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_ErrorBtns" runat="server" Visible="false">
                    <div>
                        <asp:LinkButton ID="lbtn_doReNew" runat="server" CssClass="ui orange button" OnClick="lbtn_ReNew_Click" OnClientClick="return confirm('是否確定?\n')"><i class="recycle icon"></i>重新上傳</asp:LinkButton>
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

            //Save Click
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
                "scrollY": '50vh',
                "scrollCollapse": true,
                "scrollX": true
            });


        });
    </script>

    <style>
        #listTable td {
            word-break: keep-all;
            word-wrap: break-word;
        }
    </style>
    <%-- DataTable End --%>
</asp:Content>

