<%@ Page Title="報價單匯入 | 查看記錄" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="View.aspx.cs" Inherits="myErpPriceData_View" %>

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
                    <h5 class="active section">查看記錄
                    </h5>
                </div>
                <a class="anchor" id="top"></a>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui grid">
            <div class="row">
                <!-- Left Body Content Start -->
                <div id="myStickyBody" class="thirteen wide column">
                    <div class="ui attached segment grey-bg lighten-5">
                        <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                            <div class="ui negative message">
                                <div class="header">
                                    Oops...發生了一點小問題
                                </div>
                                <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                            </div>
                        </asp:PlaceHolder>
                        <!-- Section-基本資料 Start -->
                        <div class="ui segments">
                            <div class="ui green segment">
                                <h5 class="ui header"><a class="anchor" id="baseData"></a>基本資料</h5>
                            </div>
                            <asp:ListView ID="lv_BaseData" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lv_BaseData_ItemDataBound">
                                <LayoutTemplate>
                                    <div class="ui small form segment">
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </div>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <div class="fields">
                                        <div class="five wide field">
                                            <label>追蹤編號</label>
                                            <div class="ui blue basic large label">
                                                <%#Eval("TraceID") %>
                                            </div>
                                        </div>
                                        <div class="six wide field">
                                            <label>客戶</label>
                                            <div class="ui green basic large label">
                                                <%#Eval("CustName") %>&nbsp;(<%#Eval("CustID") %>)
                                            </div>
                                        </div>
                                        <div class="five wide field">
                                            <label>狀態</label>
                                            <div class="ui orange basic large label">
                                                <%#Eval("StatusName") %>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="fields">
                                        <div class="five wide field">
                                            <label>公司別</label>
                                            <div class="ui basic large fluid label">
                                                <%#Eval("DBS") %>
                                            </div>
                                        </div>
                                        <div class="six wide field">
                                            <label>單別</label>
                                            <div class="ui basic large fluid label">
                                                <%#Eval("OrderType") %>
                                            </div>
                                        </div>
                                        <div class="five wide field">
                                            <label>單號</label>
                                            <div class="ui basic large fluid label">
                                                <%#Eval("OrderNo") %>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="fields">
                                        <div class="five wide field">
                                            <label>原始上傳檔案</label>
                                            <div class="ui basic large fluid label">
                                                <i class="cloud download icon"></i>
                                                <a href="<%#UploadFolder %><%#Eval("TraceID") %>/<%#Eval("Upload_File") %>" target="_blank">點我下載</a>
                                            </div>
                                        </div>
                                        <div class="six wide field">
                                            <label>選擇的工作表名稱</label>
                                            <div class="ui basic large fluid label">
                                                <%#Eval("Sheet_Name") %>&nbsp;
                                            </div>
                                        </div>
                                        <div class="five wide field">
                                            <label>匯入完成時間</label>
                                            <div class="ui basic large fluid label">
                                                <%#Eval("Import_Time") %>&nbsp;
                                            </div>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="ph_ErrLog" runat="server">
                                        <div class="fields">
                                            <div class="sixteen wide field">
                                                <label>錯誤訊息</label>
                                                <div class="ui basic red large fluid label">
                                                    <%#Eval("ErrMessage") %>
                                                    <div class="detail"><%#Eval("ErrTime") %></div>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                    <table class="ui celled small four column table">
                                        <thead>
                                            <tr>
                                                <th colspan="2" class="center aligned">建立</th>
                                                <th colspan="2" class="center aligned">最後更新</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr class="center aligned">
                                                <td>
                                                    <%#Eval("Create_Name") %>
                                                </td>
                                                <td>
                                                    <%#Eval("Create_Time") %>
                                                </td>
                                                <td>
                                                    <%#Eval("Update_Name") %>
                                                </td>
                                                <td>
                                                    <%#Eval("Update_Time") %>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                        <!-- Section-基本資料 End -->

                        <!-- Section-匯入清單 Start -->
                        <div class="ui segments">
                            <div class="ui grey segment">
                                <div class="ui accordion">
                                    <div class="title active">
                                        <i class="icon dropdown"></i>
                                        匯入清單
                                        <a class="anchor" id="detailData"></a>
                                    </div>
                                    <div class="content active">
                                        <asp:ListView ID="lv_DetailList" runat="server" ItemPlaceholderID="ph_Items">
                                            <LayoutTemplate>
                                                <table class="ui celled compact small table" style="width: 100%">
                                                    <thead>
                                                        <tr>
                                                            <th class="grey-bg lighten-3">Excel品號</th>
                                                            <th class="grey-bg lighten-3 green-text text-darken-2">寶工品號</th>
                                                            <th class="grey-bg lighten-3 blue-text text-darken-2">客戶品號</th>
                                                            <th class="grey-bg lighten-3 center aligned collapsing">金額</th>
                                                            <th class="grey-bg lighten-3">錯誤訊息</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                    </tbody>
                                                </table>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="collapsing">
                                                        <%#Eval("ProdID") %>
                                                    </td>
                                                    <td class="green-text text-darken-2">
                                                        <b><%#Eval("ERP_ModelNo") %></b>
                                                    </td>
                                                    <td class="blue-text text-darken-2">
                                                        <b><%#Eval("Cust_ModelNo") %></b>
                                                    </td>
                                                    <td class="center aligned positive">
                                                        <%#Eval("InputPrice") %>
                                                    </td>
                                                    <td>
                                                        <%#Eval("doWhat").ToString().Replace("\n", "<br />") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- Section-匯入清單 End -->


                        <!-- Section-EDIXA Start -->
                        <div class="ui segments">
                            <div class="ui grey segment">
                                <h5 class="ui header"><a class="anchor" id="infoData"></a>EDI排程中的品項</h5>
                            </div>
                            <div class="ui segment">
                                <asp:ListView ID="lv_EdiLog" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <table class="ui celled small table">
                                            <thead>
                                                <tr>
                                                    <th>品號</th>
                                                    <th>執行結果</th>
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
                                                <%#Eval("ModelNo") %>
                                            </td>
                                            <td>
                                                <%#Eval("Why") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        已轉入ERP或尚未匯入排程
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                        <!-- Section-EDIXA End -->

                    </div>

                </div>
                <!-- Left Body Content End -->

                <!-- Right Navi Menu Start -->
                <div class="three wide column">
                    <div class="ui sticky">
                        <div id="fastjump" class="ui secondary vertical pointing fluid text menu">
                            <div class="header item">快速跳轉<i class="dropdown icon"></i></div>
                            <a href="#baseData" class="item">基本資料</a>
                            <a href="#detailData" class="item">匯入清單</a>
                            <a href="#infoData" class="item">EDI排程</a>
                            <a href="#top" class="item"><i class="angle double up icon"></i>到頂端</a>
                        </div>

                        <div class="ui vertical text menu">
                            <div class="header item">功能按鈕</div>
                            <div class="item">
                                <a href="<%:Page_SearchUrl %>" class="ui small button"><i class="undo icon"></i>返回列表</a>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Right Navi Menu End -->
            </div>
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%-- 快速選單 --%>
    <script src="<%=fn_Params.WebUrl %>js/sticky.js"></script>
    <script>
        //tab
        $('.menu .item').tab();

        //匯入清單收合(讀取完畢後關閉, 第0個accordion)
        $('.ui.accordion').accordion('close', 0);
    </script>
</asp:Content>

