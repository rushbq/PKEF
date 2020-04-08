<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="AssetsList.aspx.cs" Inherits="myDataInfo_AssetsList" %>

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
                        <li><a>MIS資產設備</a></li>
                        <li class="active">資料查詢</li>
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
                            <div class="col s4">
                                <label>保管部門</label>
                                <asp:DropDownListGP ID="filter_Dept" runat="server" CssClass="select-control">
                                </asp:DropDownListGP>
                            </div>
                            <div class="col s8">
                                <div class="input-field inline">
                                    <i class="material-icons prefix">today</i>
                                    <asp:TextBox ID="filter_sDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                    <label for="MainContent_filter_sDate">開始日</label>
                                </div>
                                <div class="input-field inline">
                                    <i class="material-icons prefix">today</i>
                                    <asp:TextBox ID="filter_eDate" runat="server" CssClass="datepicker"></asp:TextBox>
                                    <label for="MainContent_filter_eDate">結束日</label>
                                </div>
                            </div>
                        </div>


                        <div class="row">
                            <div class="input-field col s6">
                                <asp:TextBox ID="filter_Keyword" runat="server" placeholder="編號, 名稱, 規格, 人名, 工號" autocomplete="off"></asp:TextBox>
                                <label for="MainContent_filter_Keyword">關鍵字查詢</label>
                            </div>
                            <div class="col s6 right-align">
                                <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
                                <asp:LinkButton ID="lbtn_Reset" runat="server" class="btn waves-effect waves-light grey" ToolTip="重置條件" OnClick="lbtn_Reset_Click"><i class="material-icons">refresh</i></asp:LinkButton>
                                <asp:LinkButton ID="lbtn_Export" runat="server" CssClass="btn waves-effect waves-light green" OnClick="lbtn_Export_Click">Excel</asp:LinkButton>

                                <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                            </div>
                        </div>

                    </div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <div class="card-content">
                                <table class="bordered striped">
                                    <thead>
                                        <tr>
                                            <th>保管人</th>
                                            <th class="center-align">資產編號</th>
                                            <th>名稱</th>
                                            <th>規格</th>
                                            <th class="center-align">取得日期</th>
                                            <th class="right-align">取得金額</th>
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
                                <td>
                                    <%#Eval("Who") %>
                                </td>
                                <td class="center-align">
                                    <%#Eval("ID") %>
                                </td>
                                <td>
                                    <%#Eval("Name") %>
                                </td>
                                <td>
                                    <%#Eval("Spec") %>
                                </td>
                                <td class="center-align">
                                    <%#Eval("GetItemDate") %>
                                </td>
                                <td class="right-align">
                                    <%# String.Format("{0:#,0}",Eval("GetItemMoney"))%>
                                </td>

                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="section">
                                <div class="card-panel grey darken-1">
                                    <i class="material-icons flow-text white-text">error_outline</i>
                                    <span class="flow-text white-text">找不到資料!<i class="material-icons right">arrow_upward</i></span>
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
    <script src="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/lib/pickadate/translation/zh_TW.js"></script>
    <script>
        $(function () {
            //載入選單
            $('.select-control').material_select();


            //載入datepicker
            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 5, // Creates a dropdown of 15 years to control year
                format: 'yyyymmdd',

                closeOnSelect: false // Close upon selecting a date(此版本無作用)
            }).on('change', function () {
                $(this).next().find('.picker__close').click();
            });


            //[搜尋][查詢鈕] - 觸發關鍵字快查
            $("#trigger-keySearch").click(function () {
                $("#MainContent_btn_KeySearch").trigger("click");
            });

            //[搜尋][Enter鍵] - 觸發關鍵字快查
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

