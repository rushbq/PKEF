<%@ Page Title="深圳-電子發票 | 記錄查詢" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportLog.aspx.cs" Inherits="mySZInvoiceE_ImportLog" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">記錄查詢</h5>
                    <ol class="breadcrumb">
                        <li><a href="#!">深圳-電子發票</a></li>
                        <li class="active">記錄查詢</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->

    <!-- Body Content Start -->
    <div class="container">
        <div class="section">
            <asp:PlaceHolder ID="ph_Message" runat="server">
                <div class="card-panel red darken-3">
                    <i class="material-icons flow-text white-text">error_outline</i>
                    <span class="flow-text white-text">找不到相關資料</span>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="ph_Data" runat="server">

                <div class="row">
                    <div class="col s12 m9 l10">
                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                            <LayoutTemplate>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </LayoutTemplate>
                            <ItemTemplate>
                                <!-- // 基本資料 // -->
                                <div id="base" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_SearchUrl %>" class="white-text">
                                                <h5><i class="material-icons left">arrow_back</i>基本資料</h5>
                                            </a>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">追蹤編號</span>
                                                <span class="secondary-content red-text text-darken-2 flow-text"><b><%#Eval("TraceID") %></b></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">上傳時間</span>
                                                <span class="secondary-content grey-text text-darken-3"><%#Eval("Create_Time") %></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">上傳者</span>
                                                <span class="secondary-content grey-text text-darken-1"><%#Eval("Create_Name") %></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">匯入完成時間</span>
                                                <span class="secondary-content grey-text text-darken-3"><%#Eval("Import_Time") %></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">匯入者</span>
                                                <span class="secondary-content grey-text text-darken-1"><%#Eval("Import_Name") %></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">Excel檔案</span>
                                                <span class="secondary-content"><a href="<%#UploadFolder %><%#Eval("TraceID") %>/<%#Eval("Upload_File") %>" target="_blank">查看原始檔案</a></span>
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                            </ItemTemplate>
                        </asp:ListView>


                        <!-- // 錯誤記錄 // -->
                        <div id="log" class="scrollspy">
                            <ul class="collection with-header">
                                <li class="collection-header grey">
                                    <a href="<%=Page_SearchUrl %>" class="white-text">
                                        <h5><i class="material-icons left">arrow_back</i>匯入失敗記錄 <small>(此記錄會保留)</small></h5>
                                    </a>
                                </li>
                                <li class="collection-item">
                                    <asp:ListView ID="lv_LogList" runat="server" ItemPlaceholderID="ph_Items">
                                        <LayoutTemplate>
                                            <table class="bordered striped">
                                                <tbody>
                                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                </tbody>
                                            </table>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <%#Eval("Log_Desc") %>
                                                </td>
                                                <td>
                                                    <%#Eval("Create_Time") %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="center-align grey-text text-lighten-1">
                                                <i class="material-icons flow-text">info_outline</i>
                                                <span class="flow-text">目前無記錄</span>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </li>
                            </ul>
                        </div>

                        <!-- // ERP 結帳單 // -->
                        <div id="erpData" class="card grey scrollspy">
                            <div class="card-content white-text">
                                <div class="left">
                                    <a href="<%=Page_SearchUrl %>" class="white-text">
                                        <h5><i class="material-icons left">arrow_back</i>ERP 結帳單</h5>
                                    </a>
                                </div>
                                <div class="right">
                                    <asp:LinkButton ID="btn_Export" runat="server" CssClass="btn waves-effect waves-light orange" OnClick="btn_Export_Click">匯出</asp:LinkButton>
                                </div>
                                <div class="clearfix"></div>
                            </div>
                            <div class="card-content grey lighten-5">
                                <asp:ListView ID="lv_ErpData" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <table class="bordered striped responsive-table">
                                            <thead>
                                                <tr>
                                                    <th class="center-align">平台單號</th>
                                                    <th class="center-align">發票號</th>
                                                    <th class="center-align">發票日</th>
                                                    <th class="right-align">發票金額</th>
                                                    <th class="center-align">結帳單號</th>
                                                    <th class="center-align">銷貨單號</th>
                                                    <th class="center-align">結帳單金額</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="center-align">
                                                <%#Eval("OrderID") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("InvoiceNo") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("InvoiceDate") %>
                                            </td>
                                            <td class="right-align">
                                                <%#Eval("InvPrice") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("ErpID") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("ErpSOID") %>
                                            </td>
                                            <td class="right-align">
                                                <%#Eval("ErpPrice") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="center-align grey-text text-lighten-1">
                                            <i class="material-icons flow-text">info_outline</i>
                                            <span class="flow-text">目前查無資料</span>
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>

                    </div>
                    <div class="col hide-on-small-only m3 l2">
                        <!-- // 快速導覽按鈕 // -->
                        <div class="table-Nav">
                            <ul class="table-of-contents">
                                <li><a href="#base">基本資料</a></li>
                                <li><a href="#log">匯入失敗記錄</a></li>
                                <li><a href="#erpData">ERP 結帳單</a></li>
                                <li><a href="<%=Page_SearchUrl %>">返回列表頁</a></li>
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

