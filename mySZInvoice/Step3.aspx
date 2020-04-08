<%@ Page Title="深圳-開票平台" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="Step3.aspx.cs" Inherits="mySZInvoice_Step3" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">Step3 - 轉入完成</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳-開票平台</a></li>
                        <li><a href="<%=Application["WebUrl"] %>mySZInvoice/List.aspx">紙本發票</a></li>
                        <li class="active">轉入完成</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <div class="card-panel green darken-1 white-text">
            <h4><i class="material-icons right">error_outline</i>轉入完成，資料已進入開票系統。</h4>
        </div>
        <div class="card-panel">
            <div class="row">
                <div class="col s12">
                    <h5>恭喜您已完成轉入，接下來你可以...</h5>
                </div>
            </div>
            <div class="section row">
                <div class="col s12 right-align">
                    <a class="btn-large waves-effect waves-light grey" href="<%=Application["WebUrl"] %>mySZInvoice/<%=Req_Type.Equals("1")?"List.aspx":"BBCList.aspx" %>">返回列表<i class="material-icons left">arrow_back</i></a>
                    <%if (Req_Type.Equals("1"))
                      { %>
                    <a class="btn-large waves-effect waves-light red" href="<%=Application["WebUrl"] %>mySZInvoice/Step1.aspx">開始新的發票轉入<i class="material-icons left">autorenew</i></a>
                    <%} %>
                    <a class="btn-large waves-effect waves-light blue" href="<%=Application["WebUrl"] %>mySZInvoice/View.aspx?dataID=<%=Req_DataID %>&type=<%=Req_Type %>">查看匯入明細<i class="material-icons left">history</i></a>
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
                        keyword: request.term
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    id: item.ID,
                                    label: item.Label,
                                    sales: item.SalesID
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
