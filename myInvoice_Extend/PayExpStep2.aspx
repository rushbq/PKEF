<%@ Page Title="上海會計 | 轉出匯款資料" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="PayExpStep2.aspx.cs" Inherits="myInvoice_Extend_PayExpStep2" %>

<%@ Import Namespace="PKLib_Method.Methods" %>
<%@ Register Src="Ascx_PayExpStepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
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
                    <div class="section">轉出匯款資料</div>
                    <i class="right angle icon divider"></i>
                    <div class="active section">
                        Step2.檢查匯款資料
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
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="2" />
            <!-- 資料 Start -->
            <div id="formData" class="ui small form attached green segment">
                <!-- 列表 S -->
                <div class="fields">
                    <div class="sixteen wide field">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="listTable" class="ui celled striped small table" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th>付款人名称</th>
                                            <th>付款人帐号</th>
                                            <th>付费账户</th>
                                            <th>收款人名称</th>
                                            <th>收款人开户行</th>
                                            <th>开户行名称</th>
                                            <th>接收行</th>
                                            <th>收款人账号</th>
                                            <th>付款金额</th>
                                            <th>ERP付款單號</th>
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
                                        <%#Eval("PayWho") %>
                                    </td>
                                    <td>
                                        <%#Eval("PayAcc1") %>
                                    </td>
                                    <td>
                                        <%#Eval("PayAcc2") %>
                                    </td>
                                    <td>
                                        <%#Eval("cn_AccName") %>
                                    </td>
                                    <td>
                                        <%#Eval("cn_BankType") %>
                                    </td>
                                    <td>
                                        <%#Eval("cn_BankName") %>
                                    </td>
                                    <td>
                                        <%#Eval("cn_BankID") %>
                                    </td>
                                    <td>
                                        <%#Eval("cn_Account") %>
                                    </td>
                                    <td class="right aligned red-text text-darken-2">
                                        <%#Eval("PayPrice").ToString().ToMoneyString() %>
                                    </td>
                                    <td>
                                        <%#Eval("PayERPID") %>
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
                        <asp:Button ID="btn_Back" runat="server" CssClass="ui grey button" Text="重來，回上一步" OnClick="btn_Back_Click" />
                    </div>
                    <div class="ten wide column right aligned">
                        <button id="doNext" type="button" class="ui green button">下載匯款資料<i class="cloud download right icon"></i></button>
                        <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                        <asp:Button ID="btn_Done" runat="server" CssClass="ui blue button" Text="完成" OnClick="btn_Back_Click" />
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
    <script>
        $(function () {
            
            //Save Click
            $("#doNext").click(function () {
               
                //trigger
                $("#formData").addClass("loading");
                $("#MainContent_btn_Next").trigger("click");

                //wait 1 sec then removeclass
                window.setTimeout(function () {
                    $("#formData").removeClass("loading");
                }, 1000);
               
            }); 

        });
    </script>

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

