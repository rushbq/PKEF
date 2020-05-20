<%@ Page Title="上海-開票平台(內銷)" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="mySHInvoice_List" %>

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
                        <li><a>上海-開票平台(內銷)</a></li>
                        <li class="active">紙本發票</li>
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
                                    <asp:ListItem Value="">-- 所有狀態 --</asp:ListItem>
                                    <asp:ListItem Value="A">未完成轉入</asp:ListItem>
                                    <asp:ListItem Value="B">待產生發票</asp:ListItem>
                                    <asp:ListItem Value="C">發票已產生,待更新</asp:ListItem>
                                    <asp:ListItem Value="D">已完成</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="input-field col s4">
                                <asp:TextBox ID="filter_Keyword" runat="server" placeholder="系統編號, 主旨, 發票號碼" autocomplete="off"></asp:TextBox>
                                <label for="MainContent_filter_Keyword">關鍵字查詢</label>
                            </div>
                            <div class="input-field col s4 right-align">
                                <a id="trigger-keySearch" class="btn waves-effect waves-light blue" title="查詢"><i class="material-icons">search</i></a>
                                <a href="<%=fn_Params.WebUrl %>mySHInvoice/Step1.aspx" class="btn waves-effect waves-light red" title="單筆新增"><i class="material-icons">add</i></a>
                                <a href="<%=fn_Params.WebUrl %>mySHInvoice/BatchStep1.aspx" class="btn waves-effect waves-light purple" title="多筆新增"><i class="material-icons">playlist_add</i></a>
                                <a href="<%=fn_Params.WebUrl %>mySHInvoice/ErpInvStep1.aspx" class="btn waves-effect waves-light teal" title="手動填寫發票號"><i class="material-icons">update</i></a>

                                <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                            </div>
                        </div>
                    </div>

                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound" OnItemCommand="lvDataList_ItemCommand">
                        <LayoutTemplate>
                            <div class="card-content">
                                <table id="myListTable" class="bordered highlight">
                                    <thead class="grey white-text">
                                        <tr>
                                            <th>系統編號</th>
                                            <th>主旨</th>
                                            <th>發票號碼</th>
                                            <th>發票日</th>
                                            <th>狀態</th>
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
                                <td>
                                    <strong class="red-text darken-2"><%#Eval("Inv_UID") %></strong>
                                </td>
                                <td class="grey-text text-darken-3">
                                    <a href="<%=fn_Params.WebUrl %>mySHInvoice/View.aspx?dataID=<%#Eval("Data_ID") %>&type=1" class="green-text text-darken-2" title="詳細"><%#Eval("Inv_Subject") %></a>
                                </td>
                                <td>
                                    <%#Eval("Inv_NO") %>
                                </td>
                                <td>
                                    <%#Eval("Inv_Date").ToString().ToDateString("yyyy/MM/dd") %>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_InvStatus" runat="server"></asp:Literal>
                                </td>
                                <td class="center-align">
                                    <div class="procBtns">
                                        <asp:PlaceHolder ID="ph_Edit" runat="server">
                                            <a href="<%=fn_Params.WebUrl %>mySHInvoice/Step2.aspx?dataID=<%#Eval("Data_ID") %>" class="btn waves-effect waves-light green doProcess" title="繼續轉入,進入預覽頁"><i class="material-icons">edit</i></a>
                                            <button type="button" class="btn waves-effect waves-light blue darken-3 doInsert" data-id="<%#Eval("Data_ID") %>" title="直接轉入不預覽"><i class="material-icons">publish</i></button>
                                            <asp:LinkButton ID="lbtn_Del" runat="server" CommandName="Del" CssClass="btn waves-effect waves-light red lighten-1 btn-small" OnClientClick="return confirm('確定刪除此筆?')"><i class="material-icons">delete_forever</i></asp:LinkButton>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="ph_Update" runat="server">
                                            <button type="button" class="btn waves-effect waves-light orange darken-1 doUpdate" data-id="<%#Eval("Data_ID") %>" title="更新ERP發票號碼">更新ERP</button>
                                        </asp:PlaceHolder>
                                    </div>

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

            //按鈕-Edit
            $(".doProcess").on("click", function () {
                //按下後至上層填入HTML
                $(this).parent().html('<div class="progress"><div class="indeterminate"></div></div>');

            });


            //按鈕-執行發票更新(to ERP)
            $(".doUpdate").on("click", function () {
                //Get ID
                var _dataID = $(this).attr("data-id");
                var _btnUpdate = $(this);

                //Lock
                _btnUpdate.prop("disabled", true);

                var request = $.ajax({
                    url: '<%=fn_Params.WebUrl%>' + "mySHInvoice/Ashx_UpdateData.ashx",
                    method: "POST",
                    data: {
                        DataID: _dataID,
                        Who: '<%=fn_Params.UserGuid%>',
                        type: 'U',
                        invtype: '1' //1:一般紙本發票 2:電商紙本發票 ***注意,不可寫錯***
                    },
                    dataType: "html"
                });

                request.done(function (msg) {
                    if (msg == "success") {
                        //顯示成功訊息
                        _btnUpdate.text('更新成功!!');

                    } else {
                        alert('資料存檔失敗!');
                        console.log(msg);
                        _btnUpdate.prop("disabled", false);
                        event.preventDefault();
                    }
                });

                request.fail(function (jqXHR, textStatus) {
                    alert('存檔時發生錯誤，請連絡系統人員');
                    _btnUpdate.prop("disabled", false);
                    event.preventDefault();
                });

                request.always(function () {
                    //do nothing
                });


            });


            //按鈕-直接轉入
            $(".doInsert").on("click", function () {
                //Get ID
                var _dataID = $(this).attr("data-id");
                var _btn = $(this);
                var _section = $(this).parent(".procBtns");

                //Lock
                _btn.prop("disabled", true);
                _btn.text('轉入中..');

                var request = $.ajax({
                    url: '<%=fn_Params.WebUrl%>' + "mySHInvoice/Ashx_UpdateData.ashx",
                    method: "POST",
                    data: {
                        DataID: _dataID,
                        Who: '<%=fn_Params.UserGuid%>',
                        type: 'C',
                        invtype: '1' //1:一般紙本發票 2:電商紙本發票
                    },
                    dataType: "html"
                });

                request.done(function (msg) {
                    if (msg == "success") {
                        //顯示成功訊息
                        _section.html('<span class=\"blue-text\">轉入成功</span>');

                    } else {
                        alert('資料存檔失敗!');
                        _btn.prop("disabled", false);
                        event.preventDefault();

                    }
                });

                request.fail(function (jqXHR, textStatus) {
                    alert('存檔時發生錯誤，請連絡系統人員');
                    _btn.prop("disabled", false);
                    event.preventDefault();
                });

                request.always(function () {
                    //do something
                    _btn.text('<i class="material-icons">publish</i>');
                });


            });
        });
    </script>
</asp:Content>

