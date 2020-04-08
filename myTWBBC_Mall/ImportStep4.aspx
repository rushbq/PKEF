<%@ Page Title="台灣電商BBC | Step4" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep4.aspx.cs" Inherits="myTWBBC_Mall_ImportStep4" %>

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
                        Step4.資料確認
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
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="4" />
            <!-- 資料 Start -->
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

                <!-- 即將匯入的資料 -->
                <div class="ui segments">
                    <div class="ui blue segment">
                        <h5 class="ui header"><i class="tasks icon"></i>即將匯入的資料</h5>
                    </div>
                    <div class="ui grey-bg lighten-5 segment">
                        <asp:ListView ID="lvDataList_Y" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="tableYes" class="ui celled compact small table nowrap" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th>商城訂單號</th>
                                            <th>商品編號</th>
                                            <th>購買量</th>
                                            <th>購買金額</th>
                                            <th>購買總金額</th>
                                            <th>ERP品號</th>
                                            <th>ERP價格</th>
                                            <th>收貨人</th>
                                            <th>收貨地址</th>
                                            <th>收貨電話</th>
                                            <th>贈品</th>
                                            <th>活動</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr class="<%#Eval("IsGift").Equals("Y")?"orange-text text-darken-3":"" %>">
                                    <td><%#Eval("OrderID") %></td>
                                    <td><%#Eval("ProdID") %></td>
                                    <td class="center aligned"><%#Eval("BuyCnt") %></td>
                                    <td class="right aligned"><%#Eval("BuyPrice") %></td>
                                    <td class="right aligned"><%#Eval("TotalPrice") %></td>
                                    <td><b><%#Eval("ERP_ModelNo") %></b></td>
                                    <td class="right aligned"><b><%#Eval("ERP_Price") %></b></td>
                                    <td><%#Eval("ShipWho") %></td>
                                    <td><%#Eval("ShipAddr") %></td>
                                    <td><%#Eval("ShipTel") %></td>
                                    <td class="center aligned"><%#Eval("IsGift") %></td>
                                    <td><%#Eval("PromoName") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </div>

                <!-- 不會匯入的資料 -->
                <div class="ui segments">
                    <div class="ui pink segment">
                        <h5 class="ui header"><i class="ban icon"></i>不會匯入的資料</h5>
                    </div>
                    <div class="ui grey-bg lighten-5 segment">
                        <asp:ListView ID="lvDataList_N" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <table id="tableNo" class="ui celled compact small table nowrap" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th>商城訂單號</th>
                                            <th>商品編號</th>
                                            <th>購買量</th>
                                            <th>購買金額</th>
                                            <th>購買總金額</th>
                                            <th>ERP品號</th>
                                            <th>ERP價格</th>
                                            <th>收貨人</th>
                                            <th>收貨地址</th>
                                            <th>收貨電話</th>
                                            <th>贈品</th>
                                            <th>原因</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%#Eval("OrderID") %></td>
                                    <td><%#Eval("ProdID") %></td>
                                    <td class="center aligned"><%#Eval("BuyCnt") %></td>
                                    <td class="right aligned"><%#Eval("BuyPrice") %></td>
                                    <td class="right aligned"><%#Eval("TotalPrice") %></td>
                                    <td><b><%#Eval("ERP_ModelNo") %></b></td>
                                    <td class="right aligned"><b><%#Eval("ERP_Price") %></b></td>
                                    <td><%#Eval("ShipWho") %></td>
                                    <td><%#Eval("ShipAddr") %></td>
                                    <td><%#Eval("ShipTel") %></td>
                                    <td class="center aligned"><%#Eval("IsGift") %></td>
                                    <td class="orange-text text-darken-4"><%#Eval("doWhat") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </div>

                <asp:PlaceHolder ID="ph_WorkBtns" runat="server">
                    <div class="ui grid">
                        <div class="six wide column">
                            <a href="<%=Page_SearchUrl %>" class="ui button"><i class="undo icon"></i>返回列表</a>
                            <a class="ui grey button" href="<%=FuncPath() %>/ImportStep2.aspx?id=<%=Req_DataID %>"><i class="chevron left icon"></i>回上一步</a>
                        </div>
                        <div class="ten wide column right aligned">
                            <button id="doNext" type="button" class="ui green button">下一步，轉入排程<i class="chevron right icon"></i></button>
                            <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                        </div>

                    </div>
                    <asp:HiddenField ID="hf_DataID" runat="server" />
                    <asp:HiddenField ID="hf_TraceID" runat="server" />
                    <asp:HiddenField ID="hf_MallID" runat="server" />
                </asp:PlaceHolder>
            </div>
            <!-- 資料 End -->
        </div>
    </div>
    <!-- 內容 End -->
    <!-- Lock Modal Start -->
    <asp:PlaceHolder ID="ph_LockModal" runat="server">
        <div id="lockPage" class="ui small basic modal">
            <div class="ui icon header">
                <i class="tasks icon"></i>
                最後確認
            </div>
            <div class="content">
                <p>請檢查資料是否無誤，確認完成請按「綠色按鈕」。</p>
                <p>若要繼續檢查或取消動作，請按「紅色按鈕」</p>
            </div>
            <div class="actions">
                <div class="ui red cancel inverted button">
                    <i class="remove icon"></i>
                    再檢查一下好了...
                </div>
                <div class="ui green ok inverted button">
                    <i class="checkmark icon"></i>
                    我發誓檢查沒問題了!
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    <!-- Lock Modal End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //Click:下一步
            $("#doNext").click(function () {
                //Lock顯示(Modal)
                $('#lockPage').modal({
                    closable: false,
                    onDeny: function () {
                        //window.location.href = '<%:Page_SearchUrl %>';
                    },
                    onApprove: function () {
                        $("#formData").addClass("loading");
                        $("#MainContent_btn_Next").trigger("click");
                    }
                }).modal('show');

            });
        });
    </script>

    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.13/css/jquery.dataTables.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script>
        $(function () {
            //使用DataTable
            $('#tableYes').DataTable({
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
            $('#tableNo').DataTable({
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

