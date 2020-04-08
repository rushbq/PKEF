<%@ Page Title="深圳-電子發票 | 匯入記錄" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportList.aspx.cs" Inherits="mySZInvoiceE_List" %>

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
                        <li><a>深圳-電子發票</a></li>
                        <li class="active">匯入記錄</li>
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
                                <label>狀態</label>
                                <asp:DropDownList ID="filter_Status" runat="server" CssClass="select-control">
                                </asp:DropDownList>
                            </div>
                            <div class="input-field col s4">
                                <asp:TextBox ID="filter_Keyword" runat="server" placeholder="追蹤編號" autocomplete="off"></asp:TextBox>
                                <label for="MainContent_filter_Keyword">關鍵字查詢</label>
                            </div>
                            <div class="input-field col s4 right-align">
                                <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
                                <a href="<%=Application["WebUrl"] %>mySZInvoice_E/ImportStep1.aspx" class="btn waves-effect waves-light red" title="開始匯入"><i class="material-icons">add</i></a>

                                <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                            </div>
                        </div>
                    </div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <div class="card-content">
                                <table id="myListTable" class="bordered highlight">
                                    <thead class="grey white-text">
                                        <tr>
                                            <th class="center-align">追蹤編號</th>
                                            <th class="center-align">狀態</th>
                                            <th>上傳時間</th>
                                            <th>匯入時間</th>
                                            <th>&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                    </tbody>
                                </table>
                            </div>
                            <div class="section">
                                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="center-align flow-text">
                                    <asp:Literal ID="lt_LogCnt" runat="server"><i class="material-icons light-blue-text text-darken-3" title="此筆有錯誤記錄">priority_high</i></asp:Literal>
                                    <strong class="red-text text-darken-2"><%#Eval("TraceID") %></strong>
                                </td>
                                <td class="center-align">
                                    <strong class="green-text text-darken-2"><%#Eval("StatusName") %> </strong>

                                </td>
                                <td>
                                    <p><%#Eval("Create_Time").ToString().ToDateString("yyyy/MM/dd HH:mm") %></p>
                                    <p class="grey-text text-darken-2"><%#Eval("Create_Name") %></p>
                                </td>
                                <td>
                                    <p><%#Eval("Import_Time").ToString().ToDateString("yyyy/MM/dd HH:mm") %></p>
                                    <p class="grey-text text-darken-2"><%#Eval("Import_Name") %></p>
                                </td>
                                <td class="center-align">
                                    <asp:PlaceHolder ID="ph_Edit" runat="server">
                                        <a href="<%=Application["WebUrl"] %>mySZInvoice_E/ImportStep2.aspx?dataID=<%#Eval("Data_ID") %>" class="btn waves-effect waves-light green" title="繼續匯入"><i class="material-icons">edit</i></a>
                                    </asp:PlaceHolder>
                                    <a href="<%=Application["WebUrl"] %>mySZInvoice_E/ImportLog.aspx?dataID=<%#Eval("Data_ID") %>" class="btn waves-effect waves-light blue" title="查看LOG"><i class="material-icons">access_time</i></a>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
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

            //載入選單
            $('.select-control').material_select();


        });
    </script>
</asp:Content>

