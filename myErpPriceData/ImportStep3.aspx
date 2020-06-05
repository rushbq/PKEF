<%@ Page Title="報價單匯入 | Step3" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep3.aspx.cs" Inherits="myErpPriceData_ImportStep3" %>

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
                    <h5 class="active section">Step3.確認轉入資料&nbsp;&nbsp;(<span class="red-text text-darken-2"><%=Req_DBS %></span>)
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
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="3" />
            <!-- 資料 Start -->
            <div id="formData" class="ui small form attached green segment">
                <!-- 基本資料 S -->
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
                <!-- 基本資料 E -->

                <!-- 單身列表 S -->
                <div class="fields">
                    <div class="sixteen wide field">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
                            <LayoutTemplate>
                                <table class="ui celled compact small table" style="width: 100%">
                                    <thead>
                                        <tr>
                                            <th class="grey-bg lighten-3 center aligned collapsing">編號</th>
                                            <th class="grey-bg lighten-3">
                                                <span class="green-text text-darken-2">寶工品號</span>
                                            </th>
                                            <th class="grey-bg lighten-3">
                                                <span class="blue-text text-darken-2">客戶品號</span>
                                            </th>
                                            <th class="grey-bg lighten-3 right aligned">金額</th>
                                            <th class="grey-bg lighten-3">檢查</th>
                                            <th class="grey-bg lighten-3">
                                                <%--<button type="button" id="doExport" class="ui icon green small basic button" title="匯出Excel"><i class="file excel icon"></i></button>--%>
                                            </th>
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
                                        <%#Eval("Data_ID") %>
                                    </td>
                                    <td>
                                        <p class="green-text text-darken-2">
                                            <b><%#Eval("ERP_ModelNo") %></b>
                                        </p>

                                    </td>
                                    <td>
                                        <p class="blue-text text-darken-2">
                                            <b><%#Eval("Cust_ModelNo") %></b>
                                        </p>
                                    </td>
                                    <td class="right aligned">
                                        <!--價格-->
                                        <%#Eval("InputPrice") %>
                                    </td>
                                    <td class="center aligned">
                                        <asp:PlaceHolder ID="ph_attention" runat="server">
                                            <asp:Label ID="lb_attaSign" runat="server" CssClass="showExcept orange-text text-darken-3" ToolTip="點我查看" Style="cursor: pointer;"><i class="attention icon"></i></asp:Label>
                                            <!-- Modal Start -->
                                            <div id="atta<%#Eval("Data_ID") %>" class="ui modal">
                                                <div class="header"><%#Eval("ERP_ModelNo") %>&nbsp;異常原因</div>
                                                <div class="scrolling content"><%#Eval("doWhat").ToString().Replace("\n", "<br />") %></div>
                                                <div class="actions">
                                                    <div class="ui black deny button">
                                                        Close
                                                    </div>
                                                </div>
                                            </div>
                                            <!--  Modal End -->
                                        </asp:PlaceHolder>
                                    </td>
                                    <td class="left aligned collapsing">
                                        <asp:LinkButton ID="lbtn_Close" runat="server" CssClass="ui small red basic icon button" ValidationGroup="List" CommandName="doDel" OnClientClick="return confirm('確定移除?\n執行後無法復原.')" ToolTip="移除"><i class="trash alternate icon"></i></asp:LinkButton>
                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="ui placeholder segment">
                                    <div class="ui icon header">
                                        <i class="wheelchair icon"></i>
                                        查無單身資料,請重新確認.
                                    </div>
                                </div>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
                <!-- 單身列表 E -->

                <div class="ui grid">
                    <div class="six wide column">
                        <a href="<%=Page_SearchUrl %>" class="ui button"><i class="undo icon"></i>返回列表</a>
                        <a href="<%=prevPage %>" class="ui grey button"><i class="chevron left icon"></i>回上一步</a>
                        <a href="#top" class="ui grey button"><i class="chevron up icon"></i>回頁首</a>
                    </div>
                    <div class="ten wide column right aligned">
                        <asp:PlaceHolder ID="ph_WorkBtns" runat="server">
                            <button id="doNext" type="button" class="ui green button">轉入排程<i class="chevron right icon"></i></button>
                            <asp:Button ID="btn_Next" runat="server" Text="next" OnClick="btn_Next_Click" Style="display: none;" />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_ErrTips" runat="server">
                            <asp:Label ID="lb_showTip" runat="server" CssClass="ui red large label"></asp:Label>
                        </asp:PlaceHolder>
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
            //init dropdown list
            $('select').dropdown();

            //focus then select text
            $("input").focus(function () {
                $(this).select();
            });

            //Click:下一步
            $("#doNext").click(function () {
                //confirm
                var r = confirm("資料是否已確認完畢?\n「確定」:開始轉入排程\n「取消」:停留本頁繼續編輯");
                if (r == true) {

                } else {
                    return false;
                }

                //loading
                $("#formData").addClass("loading");
                $("#MainContent_btn_Next").trigger("click");
            });


            //Click:Excel
            $("#doExport").click(function () {
                $("#MainContent_btn_Export").trigger("click");
            });


            //Modal-異常訊息
            $(".showExcept").on("click", function () {
                var id = $(this).attr("data-id");
                //show modal
                $('#' + id)
                    .modal({
                        selector: {
                            close: '.close, .actions .button'
                        }
                    })
                    .modal('show');
            });

        });
    </script>

</asp:Content>

