<%@ Page Title="上海會計 | 轉出匯款資料" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="PayExpStep1.aspx.cs" Inherits="myInvoice_Extend_PayExpStep1" %>

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
            <div id="formData" class="ui small form attached green segment">
                <div class="fields">
                    <div class="sixteen wide field">
                        <label>付款日起訖</label>
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
                <div class="three fields">
                    <div class="required field">
                        <label>付款人名称</label>
                        <asp:TextBox ID="tb_PayWho" runat="server" MaxLength="50" autocomplete="off">上海宝工工具有限公司</asp:TextBox>
                    </div>
                    <div class="required field">
                        <label>付款人帐号</label>
                        <asp:TextBox ID="tb_PayAcc1" runat="server" MaxLength="32" autocomplete="off">452059231369</asp:TextBox>
                    </div>
                    <div class="required field">
                        <label>付费账户</label>
                        <asp:TextBox ID="tb_PayAcc2" runat="server" MaxLength="32" autocomplete="off">452059231369</asp:TextBox>
                    </div>
                </div>

                <%--<div class="ui message">
                    <div class="header">注意事項</div>
                    <ul class="list">
                        <li>請慎選<strong class="red-text text-darken-1">公司別</strong></li>
                        <li>已在開票平台上的單號不會顯示。</li>
                        <li>已有發票號碼的結帳單不會顯示。</li>
                    </ul>
                </div>--%>

                <div class="ui grid">
                    <div class="six wide column">
                        <a href="<%=thisPage %>" class="ui button"><i class="undo icon"></i>重置</a>
                    </div>
                    <div class="ten wide column right aligned">
                        <button id="doGetData" type="button" class="ui blue button">帶出資料<i class="tasks right icon"></i></button>
                        <asp:Button ID="btn_GetData" runat="server" Text="get" OnClick="btn_GetData_Click" Style="display: none;" />

                        <asp:PlaceHolder ID="ph_Next" runat="server" Visible="false">
                            <button id="doNext" type="button" class="ui green button">下一步<i class="chevron right icon"></i></button>
                            <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                            <asp:TextBox ID="tb_CbxValues" runat="server" Style="display: none;"></asp:TextBox>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
            <!-- 基本資料 End -->
            <!-- 列表 S -->
            <div class="fields">
                <div class="sixteen wide field">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table id="listTable" class="ui celled striped small table" style="width: 100%">
                                <thead>
                                    <tr>
                                        <th class="no-sort center aligned collapsing" style="width: 80px;">
                                            <div class="ui checkbox">
                                                <input type="checkbox" id="cbx_All" />
                                                <label for="cbx_All"></label>
                                            </div>
                                            <div>
                                                (已選: <strong class="orange-text text-darken-4" id="countCbx">0</strong> )
                                            </div>
                                        </th>
                                        <th>供應商</th>
                                        <th>付款單號</th>
                                        <th>付款日期</th>
                                        <th>本幣<br />
                                            貸方金額</th>
                                        <th>表頭備註</th>
                                        <th>收款帳號</th>
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
                                        <input type="checkbox" id="cbx_<%#Eval("SerialNo") %>" class="myCbx" value="<%#Eval("PayFid").ToString() + Eval("PaySid").ToString() %>" />
                                        <label for="cbx_<%#Eval("SerialNo") %>"></label>
                                    </div>
                                </td>
                                <td>(<%#Eval("SupID") %>)
                                    <b class="green-text text-darken-2"><%#Eval("SupName") %></b>
                                </td>
                                <td>
                                    <%#Eval("PayFid") %>-<%#Eval("PaySid") %>
                                </td>
                                <td>
                                    <%#Eval("PayDate") %>
                                </td>
                                <td class="right aligned red-text text-darken-2">
                                    <%#Eval("PayPrice").ToString().ToMoneyString() %>
                                </td>
                                <td>
                                    <%#Eval("Remark") %>
                                </td>
                                <td>
                                    <%#Eval("cn_AccName") %><br />
                                    <%#Eval("cn_Account") %>
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

            //btn Click
            $("#doGetData").click(function () {
                $("#formData").addClass("loading");
                $("#MainContent_btn_GetData").trigger("click");
            });

            //Save Click
            $("#doNext").click(function () {
                //取得已勾選
                var s = $('input:checkbox:checked.myCbx').map(function () { return $(this).val(); }).get();

                //Check
                if (s.length == 0) {
                    alert('至少要勾選一個項目');
                    return false;
                }

                //填入勾選值
                $("#MainContent_tb_CbxValues").val(s.join(','));

                $("#formData").addClass("loading");
                $("#MainContent_btn_Next").trigger("click");
            });


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

    <%-- 日期選擇器 Start --%>
    <link href="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.css" rel="stylesheet" />
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/calendar.min.js"></script>
    <script src="<%=fn_Params.CDNUrl %>plugin/Semantic-UI-Calendar0.0.8/options.js?v=1"></script>
    <script>
        $(function () {
            //取得設定值(往前天數, 往後天數)
            var calOpt = getCalOptByDate(720, 30);
            //載入datepicker
            $('.datepicker').calendar(calOpt);
        });
    </script>
    <%-- 日期選擇器 End --%>
</asp:Content>

