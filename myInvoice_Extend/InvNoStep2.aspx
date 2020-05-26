<%@ Page Title="開票平台 | 發票回填ERP" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="InvNoStep2.aspx.cs" Inherits="myInvoice_Extend_InvNoStep2" %>

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
                        Step2.勾選結帳單
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
                                <table id="listTable" class="ui celled striped large table" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th class="no-sort center aligned collapsing">
                                                <div class="ui checkbox">
                                                    <input type="checkbox" id="cbx_All" />
                                                    <label for="cbx_All"></label>
                                                </div>
                                                <div>
                                                    (已勾選: <strong class="orange-text text-darken-4" id="countCbx">0</strong> )
                                                </div>
                                            </th>
                                            <th>銷貨單號</th>
                                            <th>結帳日期</th>
                                            <th>結帳單號</th>
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
                                        <div class="ui checkbox">
                                            <input type="checkbox" id="cbx_<%#Eval("SerialNo") %>" class="myCbx" value="<%#Eval("Erp_AR_ID") %>" />
                                            <label for="cbx_<%#Eval("SerialNo") %>"></label>
                                        </div>
                                    </td>
                                    <td>
                                        <%#Eval("Erp_SO_ID") %>
                                    </td>
                                    <td>
                                        <%#Eval("ArDate") %>
                                    </td>
                                    <td>
                                        <%#Eval("Erp_AR_ID") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="ui placeholder segment">
                                    <div class="ui icon header">
                                        <i class="wheelchair icon"></i>
                                        ERP查無資料,請重新確認.
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
                        <button id="doNext" type="button" class="ui green button">下一步<i class="chevron right icon"></i></button>
                        <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                    </div>
                    <asp:HiddenField ID="hf_DataID" runat="server" />
                    <asp:HiddenField ID="hf_CompID" runat="server" />
                    <asp:TextBox ID="tb_CbxValues" runat="server" Style="display: none;"></asp:TextBox>
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
            //全選方塊
            $('#cbx_All').click(function () {
                $('input:checkbox').prop('checked', this.checked);

                //計數
                countBox();
            });

            //偵測單一checkbox
            $(".myCbx").click(function () {
                //計數
                countBox();
            });

            //計算勾選數
            function countBox() {
                var numberOfChecked = $('input:checkbox.myCbx:checked').length;

                $("#countCbx").text(numberOfChecked);
            }

            //Save Click
            $("#doNext").click(function () {
                if (!confirm("確認要回寫已勾選的單據?")) {
                    return false;
                }

                //取得已勾選
                var s = $('input:checkbox:checked.myCbx').map(function () { return $(this).val(); }).get();

                //Check
                if (s.length == 0) {
                    alert('至少要勾選一個項目');
                    return false;
                }

                //填入勾選值
                $("#MainContent_tb_CbxValues").val(s.join(','));

                //trigger
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

