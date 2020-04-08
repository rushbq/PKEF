<%@ Page Title="供應商基本資料" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="mySupInfo_Search" %>

<%@ Import Namespace="PKLib_Method.Methods" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <ol class="breadcrumb">
                        <li><a>供應商基本資料</a></li>
                        <li class="active">
                            <asp:Literal ID="lt_CorpName" runat="server" Text="公司別名稱"></asp:Literal></li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <div class="row">
            <div class="col s12">
                <div class="card">
                    <div class="card-content">
                        <div class="row">
                            <div class="input-field col s6">
                                <asp:TextBox ID="filter_Keyword" runat="server" placeholder="供應商代號 / 供應商簡稱" autocomplete="off"></asp:TextBox>
                                <label for="MainContent_filter_Keyword">關鍵字查詢</label>
                            </div>
                            <div class="input-field col s6 right-align">
                                <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
                                <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                            </div>
                        </div>

                    </div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <div class="right-align">
                                <asp:Literal ID="lt_TopPager" runat="server"></asp:Literal>
                            </div>
                            <div class="card-content">
                                <table id="myListTable" class="bordered highlight">
                                    <thead>
                                        <tr>
                                            <th class="center-align">供應商代號</th>
                                            <th>供應商簡稱</th>
                                            <th>採購人員</th>
                                            <th class="center-align" style="width: 100px">&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </div>
                            <div class="right-align">
                                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="center-align">
                                    <%#Eval("ERP_SupID") %>
                                </td>
                                <td>
                                    <%#Eval("ERP_SupName") %>
                                </td>
                                <td>
                                    <%#Eval("User_Name") %>
                                </td>
                                <td>
                                    <a class="waves-effect waves-light btn green" href="<%#Application["WebUrl"] %>mySupInfo/Edit.aspx?ds=<%#Req_ds %>&id=<%#Eval("ERP_SupID") %>" title="編輯"><i class="material-icons">edit</i></a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="section">
                                <div class="card-panel grey darken-1">
                                    <i class="material-icons flow-text white-text">error_outline</i>
                                    <span class="flow-text white-text">目前的篩選條件找不到資料，請重新篩選。</span>
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>

                </div>

            </div>
        </div>
    </div>
    <!-- Body Content End -->

</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //[搜尋][查詢鈕] - 觸發查詢
            $("#trigger-keySearch").click(function () {
                $("#MainContent_btn_KeySearch").trigger("click");
            });


            //[搜尋][Enter鍵] - 觸發查詢
            $("#MainContent_filter_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_KeySearch").trigger("click");

                    e.preventDefault();
                }
            });

        });
    </script>
</asp:Content>

