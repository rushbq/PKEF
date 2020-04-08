<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="DataExport.aspx.cs" Inherits="mySZBBC_DataExport" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">ERP單據匯出</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳-工具BBC</a></li>
                        <li class="active">單據匯出</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <div class="section">
            <asp:PlaceHolder ID="ph_Data" runat="server">
                <div class="row">
                    <div class="col s12 m9 l10">
                        <!-- // Data1 // -->
                        <div id="data1" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">VC退貨單                                        
                                            <small>(僅可選擇一年內的時間區間)</small>
                                    </h5>
                                </li>
                                <li class="collection-item">
                                    <div class="input-field inline">
                                        <i class="material-icons prefix">today</i>
                                        <asp:TextBox ID="filter_sDate" runat="server" CssClass="datepicker" placeholder="選擇銷退日區間"></asp:TextBox>
                                        <label for="MainContent_filter_sDate">銷退日-起</label>
                                    </div>
                                    <div class="input-field inline">
                                        <i class="material-icons prefix">today</i>
                                        <asp:TextBox ID="filter_eDate" runat="server" CssClass="datepicker" placeholder="選擇銷退日區間"></asp:TextBox>
                                        <label for="MainContent_filter_eDate">銷退日-訖</label>
                                    </div>
                                    <div class="right-align">
                                        <asp:LinkButton ID="lbtn_Export1" runat="server" CssClass="btn waves-effect waves-light orange" OnClick="lbtn_Export1_Click"><i class="material-icons">archive</i></asp:LinkButton>
                                    </div>
                                </li>
                            </ul>
                        </div>

                        <!-- // Data2 // -->
                        <div id="data2" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <h5 class="white-text">經銷商專區                                        
                                            <small>(僅可選擇一年內的時間區間)</small>
                                    </h5>
                                </li>
                                <li class="collection-item">
                                    <div class="input-field inline">
                                        <i class="material-icons prefix">today</i>
                                        <asp:TextBox ID="so_sDate" runat="server" CssClass="datepicker" placeholder="選擇訂單日區間"></asp:TextBox>
                                        <label for="MainContent_so_sDate">訂單日-起</label>
                                    </div>
                                    <div class="input-field inline">
                                        <i class="material-icons prefix">today</i>
                                        <asp:TextBox ID="so_eDate" runat="server" CssClass="datepicker" placeholder="選擇訂單日區間"></asp:TextBox>
                                        <label for="MainContent_so_eDate">訂單日-訖</label>
                                    </div>
                                    <div class="right-align">
                                        <asp:LinkButton ID="lbtn_Export2" runat="server" CssClass="btn waves-effect waves-light orange" OnClick="lbtn_Export2_Click"><i class="material-icons">archive</i></asp:LinkButton>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="col hide-on-small-only m3 l2">
                        <!-- // 快速導覽按鈕 // -->
                        <div class="table-Nav">
                            <ul class="table-of-contents">
                                <li><a href="#data1">VC退貨單</a></li>
                                <li><a href="#data2">經銷商專區</a></li>
                            </ul>
                        </div>
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
            //載入datepicker
            $('.datepicker').pickadate({
                selectMonths: true, // Creates a dropdown to control month
                selectYears: 5, // Creates a dropdown of 15 years to control year
                format: 'yyyy/mm/dd',

                closeOnSelect: false // Close upon selecting a date(此版本無作用)
            }).on('change', function () {
                $(this).next().find('.picker__close').click();
            });


        });
    </script>
    <script>
        (function ($) {
            $(function () {
                //scrollSpy
                $('.scrollspy').scrollSpy();

                //pushpin
                $('.table-Nav').pushpin({
                    top: 97
                });


            }); // end of document ready
        })(jQuery); // end of jQuery name space
    </script>
</asp:Content>

