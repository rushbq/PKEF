<%@ Page Title="深圳-開票平台" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Step1.aspx.cs" Inherits="mySZInvoice_Step1" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">Step1 - 條件篩選</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳-開票平台</a></li>
                        <li><a href="<%=Application["WebUrl"] %>mySZInvoice/List.aspx">紙本發票</a></li>
                        <li class="active">Step1</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <asp:PlaceHolder ID="ph_Message" runat="server" Visible="false">
            <div class="card-panel red darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>Oops...發生了一點小問題</h4>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel">
                <div class="section row">
                    <div class="col s12">
                        <label>選擇客戶</label>
                        <asp:TextBox ID="Cust_Name" runat="server" CssClass="AC-Customer" data-target="MainContent_Cust_ID_Val" placeholder="輸入客戶關鍵字"></asp:TextBox>
                        <asp:TextBox ID="Cust_ID_Val" runat="server" Style="display: none"></asp:TextBox>
                        <div class="grey-text text-darken-1">(輸入客戶代號或名稱關鍵字，再點按選單上的選項)</div>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <div class="input-field inline">
                            <i class="material-icons prefix">today</i>
                            <asp:TextBox ID="filter_sDate" runat="server" CssClass="datepicker"></asp:TextBox>
                            <label for="MainContent_filter_sDate">結帳日-起</label>
                        </div>
                        <div class="input-field inline">
                            <i class="material-icons prefix">today</i>
                            <asp:TextBox ID="filter_eDate" runat="server" CssClass="datepicker"></asp:TextBox>
                            <label for="MainContent_filter_eDate">結帳日-訖</label>
                        </div>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12">
                        <label>注意事項</label>
                        <div>
                            <ul class="collection">
                                <li class="collection-item red-text"><i class="material-icons left">info</i>若查找不到客戶，請確認客戶資料已建立</li>
                                <li class="collection-item red-text"><i class="material-icons left">info</i>若查找不到客戶，請確認客戶的戶名、稅號已填寫</li>
                                <li class="collection-item"><i class="material-icons left">info</i>客戶的「發票類型」請先至基本資料維護 -&gt; 客戶基本資料, 進行設定</li>
                                <li class="collection-item"><i class="material-icons left">info</i>下一步將會產生「系統編號」,此編號即為開票系統的編號</li>                                
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
    <asp:PlaceHolder ID="ph_Buttons" runat="server">
        <div class="container">
            <div class="section row">
                <div class="col s6">
                    <a href="<%=Application["WebUrl"] %>mySZInvoice/List.aspx" class="btn-large waves-effect waves-light grey">取消，返回列表<i class="material-icons left">arrow_back</i></a>
                </div>
                <div class="col s6 right-align">
                    <div id="showProcess" class="progress" style="display: none;">
                        <div class="indeterminate"></div>
                    </div>
                    <a href="#!" id="trigger-Next" class="btn-large waves-effect waves-light blue">下一步，資料預覽<i class="material-icons right">chevron_right</i></a>
                    <asp:Button ID="btn_Next" runat="server" Text="Next" OnClick="btn_Next_Click" Style="display: none;" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script src="<%=Application["CDN_Url"] %>plugin/Materialize/v0.97.8/lib/pickadate/translation/zh_TW.js"></script>
    <script>
        $(function () {
            //載入datepicker
            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 2, // Creates a dropdown of 15 years to control year
                format: 'yyyy/mm/dd',

                closeOnSelect: false // Close upon selecting a date(此版本無作用)
            }).on('change', function () {
                $(this).next().find('.picker__close').click();
            });


            //trigger button
            $("#trigger-Next").click(function () {
                $(this).hide();
                $("#showProcess").show();

                $("#BottomContent_btn_Next").trigger("click");
            });
        });
    </script>

    <link href="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.css" rel="stylesheet" />
    <script src="<%=Application["CDN_Url"] %>plugin/jqueryUI/jquery-ui.min.js"></script>
    <%-- Catcomplete Start --%>
    <script>
        /* Autocomplete 客戶 */
        $(".AC-Customer").autocomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"]%>Ajax_Data/GetData_Customer.ashx",
                    data: {
                        keyword: request.term,
                        doBlock: 'Y'
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    id: item.ID,
                                    label: item.Label + ' (' + item.ID + ')'
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


                event.preventDefault();
            }
        });
    </script>
    <%-- Catcomplete End --%>
</asp:Content>
