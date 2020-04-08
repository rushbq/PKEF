<%@ Page Title="供應商資料編輯" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="mySupInfo_Edit" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">供應商資料編輯<asp:Literal ID="lt_CorpName" runat="server"></asp:Literal></h5>
                    <ol class="breadcrumb">
                        <li><a>供應商基本資料</a></li>
                        <li><a href="<%=Page_SearchUrl %>">資料列表</a></li>
                        <li class="active">供應商資料編輯</li>
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
                        <div id="base" class="card grey scrollspy">
                            <div class="card-content white-text">
                                <h5>基本資料</h5>
                            </div>
                            <div class="card-content grey lighten-5">
                                <table class="bordered">
                                    <tr>
                                        <th style="width: 15%">代號</th>
                                        <td class="flow-text red-text text-darken-2" style="width: 35%"><b>
                                            <asp:Literal ID="lt_SupID" runat="server"></asp:Literal>
                                        </b></td>
                                        <th style="width: 15%">簡稱</th>
                                        <td class="flow-text green-text text-darken-2" style="width: 35%"><b>
                                            <asp:Literal ID="lt_SupName" runat="server"></asp:Literal>
                                        </b></td>
                                    </tr>
                                    <tr>
                                        <th>採購人員</th>
                                        <td class="input-field col s12">
                                            <asp:TextBox ID="AC_UserID" runat="server" CssClass="AC-Users" data-target="MainContent_Rel_UserID_Val" data-label="MainContent_lb_UserName" ValidationGroup="AddBase"></asp:TextBox>
                                            <asp:TextBox ID="Rel_UserID_Val" runat="server" ValidationGroup="AddBase" Style="display: none"></asp:TextBox>
                                            <label for="MainContent_AC_UserID">
                                                輸入關鍵字(工號或人名),選擇人員
                                            </label>
                                        </td>
                                        <td colspan="2">
                                            <asp:Label ID="lb_UserName" runat="server" CssClass="orange-text text-darken-2">未設定</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <div class="row">
                                    <div class="input-field col s12 right-align">
                                        <a class="btn waves-effect waves-light blue trigger-BaseSave">存檔</a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- // 資料設定 // -->
                        <div id="dataSet" class="card grey scrollspy">
                            <div class="card-content white-text">
                                <h5>新增通訊人</h5>
                            </div>
                            <div class="card-content grey lighten-5">
                                <div class="row">
                                    <div class="input-field col s7">
                                        <asp:TextBox ID="tb_FullName" runat="server" MaxLength="50" length="50"></asp:TextBox>
                                        <label for="MainContent_tb_FullName">
                                            姓名 *&nbsp;
                                            <asp:RequiredFieldValidator ID="rfv_tb_FullName" runat="server" ErrorMessage="必填欄位請填寫" ControlToValidate="tb_FullName" CssClass="red-text" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                        </label>
                                    </div>
                                    <div class="input-field col s5">
                                        <asp:RadioButtonList ID="rbl_IsSend" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Value="Y">發送&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                            <asp:ListItem Value="N" Selected="True">不發,&nbsp;&nbsp;&nbsp;&nbsp;採購單PDF</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s7">
                                        <asp:TextBox ID="tb_NickName" runat="server" MaxLength="50" length="50"></asp:TextBox>
                                        <label for="MainContent_NickName">
                                            䁥稱</label>
                                    </div>
                                    <div class="input-field col s5">
                                        <asp:RadioButtonList ID="rbl_Gender" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Value="M" Selected="True">先生&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                            <asp:ListItem Value="F">小姐</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s7">
                                        <asp:TextBox ID="tb_Phone" runat="server" MaxLength="20" length="20" type="tel"></asp:TextBox>
                                        <label for="MainContent_Phone">聯絡電話</label>
                                    </div>
                                    <div class="input-field col s5 inline">
                                        <i class="material-icons prefix">today</i>
                                        <asp:TextBox ID="tb_Birthday" runat="server" CssClass="datepicker center-align"></asp:TextBox>
                                        <label for="MainContent_tb_Birthday">生日</label>

                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12">
                                        <asp:TextBox ID="tb_Email" runat="server" MaxLength="200" length="200" type="email"></asp:TextBox>
                                        <label for="MainContent_tb_Email">
                                            EMail *&nbsp;
                                            <asp:RequiredFieldValidator ID="rfv_tb_Email" runat="server" ErrorMessage="必填欄位請填寫" ControlToValidate="tb_Email" CssClass="red-text" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="rev_tb_Email" runat="server" ControlToValidate="tb_Email" ErrorMessage="輸入格式錯誤" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" CssClass="red-text" Display="Dynamic" ValidationGroup="Add"></asp:RegularExpressionValidator>
                                        </label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="input-field col s12 right-align">
                                        <a class="btn waves-effect waves-light blue trigger-Save">存檔</a>
                                    </div>
                                </div>
                            </div>
                        </div>


                        <!-- // 通訊錄 // -->
                        <div id="dataList" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">通訊錄</h5>
                                </li>
                                <li class="collection-item">
                                    <asp:ListView ID="lv_Members" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lv_Members_ItemCommand">
                                        <LayoutTemplate>
                                            <table class="bordered striped">
                                                <thead>
                                                    <tr>
                                                        <th>姓名</th>
                                                        <th>Email</th>
                                                        <th class="center-align">PDF發送</th>
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
                                                    <%#Eval("FullName") %>&nbsp;&nbsp;<span class="grey-text text-darken-2"><%#fn_Desc.PubAll.Gender(Eval("Gender").ToString()) %></span>
                                                </td>
                                                <td>
                                                    <%#Eval("Email") %>
                                                </td>
                                                <td class="center-align">
                                                    <%#fn_Desc.PubAll.YesNo(Eval("IsSendOrder").ToString()) %>
                                                </td>
                                                <td class="center-align">
                                                    <a href="<%#PageUrl+"&dtid="+Eval("Data_ID") %>" class="btn-flat waves-effect waves-teal"><i class="material-icons">mode_edit</i></a>
                                                    <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="btn-flat waves-effect waves-red" ValidationGroup="DataRel" OnClientClick="return confirm('是否確定刪除!?')"><i class="material-icons">clear</i></asp:LinkButton>

                                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Data_ID") %>' />
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="center-align grey-text text-lighten-1">
                                                <i class="material-icons flow-text">info_outline</i>
                                                <span class="flow-text">尚未新增資料</span>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </li>
                            </ul>
                        </div>

                    </div>
                    <div class="col hide-on-small-only m3 l2">
                        <!-- // 快速導覽按鈕 // -->
                        <div class="table-Nav">
                            <ul class="table-of-contents">
                                <li><a href="#base">基本資料</a></li>
                                <li><a href="#dataSet">新增通訊人</a></li>
                                <li><a href="#dataList">通訊錄</a></li>
                                <li></li>
                                <li><a href="<%=Page_SearchUrl %>"><i class="material-icons left">list</i>回列表</a></li>
                            </ul>
                        </div>
                    </div>

                    <!-- // Hidden buttons // -->
                    <div class="SrvSide-Buttons" style="display: none;">
                        <asp:HiddenField ID="hf_CorpUID" runat="server" />
                        <asp:HiddenField ID="hf_SupID" runat="server" />
                        <asp:HiddenField ID="hf_DtID" runat="server" />
                        <asp:Button ID="btn_Save" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" />
                        <asp:HiddenField ID="hf_InfoID" runat="server" />
                        <asp:Button ID="btn_BaseSave" runat="server" Text="Save" OnClick="btn_BaseSave_Click" ValidationGroup="AddBase" />
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="false" HeaderText="資料填寫不完整，請重新確認!" ValidationGroup="Add" />
                    </div>
                </div>

            </asp:PlaceHolder>
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
            //scrollSpy
            $('.scrollspy').scrollSpy();

            //pushpin
            $('.table-Nav').pushpin({
                top: 97
            });


            //載入datepicker (生日用)
            var currDate = new Date();
            var restDate = new Date(currDate);
            restDate.setFullYear(currDate.getFullYear() - 10);
            var minDate = new Date(currDate);
            minDate.setFullYear(currDate.getFullYear() - 100);

            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 99, // Creates a dropdown of 15 years to control year
                format: 'yyyy-mm-dd',
                today: '',
                max: restDate,
                min: minDate,

                closeOnSelect: false // Close upon selecting a date(此版本無作用)
            }).on('change', function () {
                $(this).next().find('.picker__close').click();
            });


            //[通訊人][按鈕] - 觸發儲存
            $(".trigger-Save").click(function () {
                $("#MainContent_btn_Save").trigger("click");
            });


            //[基本資料][按鈕] - 觸發儲存
            $(".trigger-BaseSave").click(function () {
                $("#MainContent_btn_BaseSave").trigger("click");
            });
        });
    </script>
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/jquery-ui.min.css" rel="stylesheet" />
    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/catcomplete/catcomplete.css" rel="stylesheet" />
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/jquery-ui.min.js"></script>
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI-1.12.1/catcomplete/catcomplete.js"></script>
    <%-- Catcomplete Start --%>
    <script>
        /* Autocomplete  */
        $(".AC-Users").catcomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"] %>Ajax_Data/GetData_Users.ashx",
                    data: {
                        keyword: request.term
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    id: item.ID,
                                    label: '(' + item.ID + ') ' + item.Label,
                                    name: item.Label,
                                    category: item.Category
                                }
                            }));
                        }
                    }
                });
            },
            select: function (event, ui) {
                //目前欄位
                $(this).val(ui.item.value);

                //實際欄位-儲存值
                var targetID = $(this).attr("data-target");
                $("#" + targetID).val(ui.item.id);
                var targetName = $(this).attr("data-label");
                $("#" + targetName).text(ui.item.name);

                event.preventDefault();
            }
        });
    </script>
</asp:Content>

