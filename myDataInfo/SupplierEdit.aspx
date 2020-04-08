<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="SupplierEdit.aspx.cs" Inherits="myDataInfo_SupplierEdit" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">供應商關聯設定</h5>
                    <ol class="breadcrumb">
                        <li><a href="#!">基本資料維護</a></li>
                        <li><a href="<%=Page_SearchUrl %>">資料列表</a></li>
                        <li class="active">供應商關聯設定</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <div class="section">
            <asp:PlaceHolder ID="ph_Message" runat="server" Visible="false">
                <div class="card-panel red darken-3">
                    <i class="material-icons flow-text white-text">error_outline</i>
                    <span class="flow-text white-text">資料處理失敗</span>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_Data" runat="server">
                <div class="row">
                    <div class="col s12 m9 l10">
                        <!-- // 基本資料 // -->
                        <div id="base" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">基本資料</h5>
                                </li>
                                <li class="collection-item">
                                    <div>
                                        <span class="title grey-text text-darken-1">系統編號</span>
                                        <span class="secondary-content flow-text"><b>
                                            <asp:Literal ID="lt_Sup_UID" runat="server"></asp:Literal></b></span>
                                    </div>
                                </li>
                                <li class="collection-item">
                                    <div class="row">
                                        <div class="input-field col s12">
                                            <asp:TextBox ID="tb_Sup_Name" runat="server" MaxLength="50" length="50"></asp:TextBox>
                                            <label for="MainContent_tb_Sup_Name">
                                                自訂顯示名稱 *&nbsp;<asp:RequiredFieldValidator ID="rfv_tb_Sup_Name" runat="server" ErrorMessage="此為必填欄位" ControlToValidate="tb_Sup_Name" CssClass="red-text" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator></label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="input-field col s12 right-align">
                                            <a class="btn waves-effect waves-light blue trigger-Save">存檔</a>
                                        </div>
                                    </div>
                                </li>
                            </ul>
                        </div>

                        <!-- // 關聯設定 // -->
                        <div id="dataRel" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">關聯設定</h5>
                                </li>
                                <li class="collection-item">
                                    <div class="row">
                                        <div class="col s4">
                                            <label>公司別</label>
                                            <asp:DropDownList ID="filter_Corp" runat="server" CssClass="browser-default">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col s5">
                                            <div class="input-field">
                                                <label>關鍵字</label>
                                                <asp:TextBox ID="filter_Keyword" runat="server" placeholder="關鍵字查詢：廠商代號, 廠商簡稱" autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col s3 input-field">
                                            <a class="btn waves-effect waves-light green trigger-Search"><i class="material-icons">search</i></a>
                                        </div>
                                    </div>
                                    <div>
                                        <asp:ListView ID="lv_Supplier" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_Supplier_ItemCommand">
                                            <LayoutTemplate>
                                                <table class="bordered striped">
                                                    <thead>
                                                        <tr>
                                                            <th>公司別</th>
                                                            <th>代號</th>
                                                            <th>名稱</th>
                                                            <th></th>
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
                                                        <%#Eval("Corp_Name") %>
                                                    </td>
                                                    <td>
                                                        <%#Eval("ERP_SupID") %>
                                                    </td>
                                                    <td>
                                                        <%#Eval("ERP_SupName") %>
                                                    </td>
                                                    <td class="center-align">
                                                        <asp:LinkButton ID="lbtn_Add" runat="server" CssClass="btn-flat waves-effect waves-blue" ValidationGroup="ListRel"><i class="material-icons">add</i></asp:LinkButton>

                                                        <asp:HiddenField ID="hf_Corp_UID" runat="server" Value='<%#Eval("Corp_UID") %>' />
                                                        <asp:HiddenField ID="hf_ERP_SupID" runat="server" Value='<%#Eval("ERP_SupID") %>' />
                                                        <asp:HiddenField ID="hf_ERP_SupName" runat="server" Value='<%#Eval("ERP_SupName") %>' />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyDataTemplate>
                                                <div class="center-align grey-text text-lighten-1">
                                                    <i class="material-icons flow-text">info_outline</i>
                                                    <span class="flow-text">查無資料</span>
                                                </div>
                                            </EmptyDataTemplate>
                                        </asp:ListView>
                                    </div>
                                </li>
                            </ul>
                        </div>


                        <!-- // 已關聯資料 // -->
                        <div id="supplierRel" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">已關聯資料</h5>
                                </li>
                                <li class="collection-item">
                                    <asp:ListView ID="lv_RelData" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_RelData_ItemCommand">
                                        <LayoutTemplate>
                                            <table class="bordered striped">
                                                <thead>
                                                    <tr>
                                                        <th>公司別</th>
                                                        <th>代號</th>
                                                        <th>名稱</th>
                                                        <th></th>
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
                                                    <%#Eval("Corp_Name") %>
                                                </td>
                                                <td>
                                                    <%#Eval("ERP_SupID") %>
                                                </td>
                                                <td>
                                                    <%#Eval("ERP_SupName") %>
                                                </td>
                                                <td class="center-align">
                                                    <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" ValidationGroup="DataRel"><i class="material-icons">clear</i></asp:LinkButton>

                                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_UID") %>' />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="center-align grey-text text-lighten-1">
                                                <i class="material-icons flow-text">info_outline</i>
                                                <span class="flow-text">尚未設定關聯</span>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </li>
                            </ul>
                        </div>


                        <!-- // 維護資訊 // -->
                        <div>
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">維護資訊</h5>
                                </li>
                                <li class="collection-item">
                                    <table class="bordered striped responsive-table">
                                        <tbody>
                                            <tr>
                                                <th>建立資訊
                                                </th>
                                                <td>
                                                    <asp:Literal ID="lt_Creater" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="lt_CreateTime" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>最後更新
                                                </th>
                                                <td>
                                                    <asp:Literal ID="lt_Updater" runat="server"></asp:Literal>
                                                </td>
                                                <td>
                                                    <asp:Literal ID="lt_UpdateTime" runat="server"></asp:Literal>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="col hide-on-small-only m3 l2">
                        <!-- // 快速導覽按鈕 // -->
                        <div class="table-Nav">
                            <ul class="table-of-contents">
                                <li><a href="#base">基本資料</a></li>
                                <li><a href="#dataRel">關聯設定</a></li>
                                <li><a href="#supplierRel">已關聯資料</a></li>
                            </ul>
                        </div>
                    </div>

                    <!-- // Hidden buttons // -->
                    <div class="SrvSide-Buttons" style="display: none;">
                        <asp:HiddenField ID="hf_DataID" runat="server" />
                        <asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" />
                        <asp:Button ID="btn_Search" runat="server" Text="Save" OnClick="btn_Search_Click" ValidationGroup="Search" />
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" HeaderText="尚有資料未填寫" ValidationGroup="Add" />
                    </div>
                </div>

            </asp:PlaceHolder>
        </div>

        <!-- // Bottom buttons // -->
        <div class="fixed-action-btn toolbar">
            <a class="btn-floating btn-large red">
                <i class="large material-icons">menu</i>
            </a>
            <ul>
                <li class="waves-effect waves-light">
                    <asp:LinkButton ID="lbtn_DelData" runat="server" CssClass="btn-flat waves-effect waves-red" OnClientClick="return confirm('是否確定刪除資料?\n注意:刪除後無法復原!')" OnClick="lbtn_DelData_Click" ValidationGroup="DelData"><i class="material-icons">delete_forever</i>刪除資料</asp:LinkButton>
                </li>
                <li class="waves-effect waves-light grey"><a href="<%=Page_SearchUrl %>"><i class="material-icons">list</i>回列表</a></li>
            </ul>
        </div>
    </div>
    <!-- Body Content End -->

</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //scrollSpy
            $('.scrollspy').scrollSpy();

            //pushpin
            $('.table-Nav').pushpin({
                top: 97
            });

            //[基本資料][按鈕] - 觸發儲存
            $(".trigger-Save").click(function () {
                $("#MainContent_btn_Save").trigger("click");
            });

            //[基本資料][Enter鍵] - 觸發儲存
            $("#MainContent_tb_Sup_Name").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Save").trigger("click");

                    e.preventDefault();
                }
            });

            //[關聯設定][按鈕] - 觸發Search
            $(".trigger-Search").click(function () {
                $("#MainContent_btn_Search").trigger("click");
            });
            //[關聯設定][Enter鍵] - 觸發Search
            $("#MainContent_filter_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Search").trigger("click");

                    e.preventDefault();
                }
            });

        });
    </script>
</asp:Content>

