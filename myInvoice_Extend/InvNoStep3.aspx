<%@ Page Title="開票平台 | 發票回填ERP" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="InvNoStep3.aspx.cs" Inherits="myInvoice_Extend_InvNoStep3" %>

<%@ Register Src="Ascx_InvNoStepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">開票平台</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">發票回填ERP</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step3.完成
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
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="3" />
            <!-- 資料 Start -->
            <div id="formData" class="ui small form attached green segment">
                <!-- 基本資料 S -->
                <div class="fields">
                    <div class="two wide field">
                        <label>公司別</label>
                        <asp:Label ID="lb_CompID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="four wide field">
                        <label>客戶</label>
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                    <div class="four wide field">
                        <label>結帳日起訖</label>
                        <asp:Label ID="lb_sDate" runat="server" CssClass="ui basic large label"></asp:Label>
                        ~<asp:Label ID="lb_eDate" runat="server" CssClass="ui basic large label"></asp:Label>
                    </div>
                    <div class="three wide field">
                        <label>發票日期</label>
                        <asp:Label ID="lb_InvDate" runat="server" CssClass="ui teal basic large label"></asp:Label>
                    </div>
                    <div class="three wide field">
                        <label>發票號碼</label>
                        <asp:Label ID="lb_InvNo" runat="server" CssClass="ui teal basic large label"></asp:Label>
                    </div>
                </div>
                <!-- 基本資料 E -->
                <!-- 列表 S -->
                <div class="fields">
                    <div class="sixteen wide field">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="listTable" class="ui celled striped table" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th class="center aligned">已回寫的結帳單號</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="center aligned">
                                        <%#Eval("Erp_AR_ID") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="ui placeholder segment">
                                    <div class="ui icon header">
                                        <i class="wheelchair icon"></i>
                                        查無資料,請重新確認.
                                    </div>
                                </div>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
                <!-- 列表 E -->

                <div class="ui grid">
                    <div class="six wide column">
                        
                    </div>
                    <div class="ten wide column right aligned">
                        <a href="<%=FuncPath() %>/InvNoStep1.aspx?dbs=<%=Req_DBS %>" class="ui blue button">開始新的轉入</a>
                    </div>
                </div>
            </div>
            <!-- 資料 End -->
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">  
    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script>
        $(function () {
            /*
             [使用DataTable]
             注意:標題欄須與內容欄數量相等
           */
            var table = $('#listTable').DataTable({
                "searching": false,  //搜尋
                "ordering": true,   //排序
                "paging": false,     //分頁
                "info": true,      //筆數資訊
                "language": {
                    //自訂筆數顯示選單
                    "lengthMenu": ''
                },
                //讓不排序的欄位在初始化時不出現排序圖
                "order": [],
                //自訂欄位
                "columnDefs": [{
                    "targets": 'no-sort',
                    "orderable": false,
                }],
                //捲軸設定
                "scrollY": '50vh',
                "scrollCollapse": true,
                "scrollX": false

            });

        });

    </script>
    <%-- DataTable End --%>
</asp:Content>

