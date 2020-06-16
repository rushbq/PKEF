<%@ Page Title="玩具BBC | 記錄查詢" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportLog.aspx.cs" Inherits="mySZBBC_ImportLog" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="blue lighten-5">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">記錄查詢</h5>
                    <ol class="breadcrumb">
                        <li><a href="#!">玩具BBC</a></li>
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
                                                <span class="title grey-text text-darken-1">客戶</span>
                                                <span class="secondary-content green-text text-darken-2 flow-text"><b><%#Eval("CustName") %></b>&nbsp;(<%#Eval("CustID") %>)</span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">目前狀態</span>
                                                <span class="secondary-content"><span class="label label-danger"><%#Eval("StatusName") %></span></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">匯入模式</span>
                                                <span class="secondary-content"><span class="label label-info"><%#Eval("Data_TypeName") %></span></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">商城</span>
                                                <span class="secondary-content"><span class="label label-default"><%#Eval("MallName") %></span></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">匯入完成時間</span>
                                                <span class="secondary-content grey-text text-darken-3"><%#Eval("Import_Time") %></span>
                                            </div>
                                        </li>
                                    </ul>
                                </div>


                                <!-- // 檔案資訊 // -->
                                <div id="fileinfo" class="scrollspy">
                                    <ul class="collection with-header">
                                        <li class="collection-header grey">
                                            <a href="<%=Page_SearchUrl %>" class="white-text">
                                                <h5><i class="material-icons left">arrow_back</i>檔案資訊</h5>
                                            </a>
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
                                                <span class="secondary-content grey-text text-darken-3"><%#Eval("Create_Name") %></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">最後更新時間</span>
                                                <span class="secondary-content grey-text text-darken-3"><%#Eval("Update_Time") %></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">Excel檔案</span>
                                                <span class="secondary-content"><a href="<%#UploadFolder %><%#Eval("TraceID") %>/<%#Eval("Upload_File") %>" target="_blank">查看原始檔案</a></span>
                                            </div>
                                        </li>
                                        <li class="collection-item">
                                            <div>
                                                <span class="title grey-text text-darken-1">Excel工作表</span>
                                                <span class="secondary-content grey-text text-darken-3"><%#Eval("Sheet_Name") %></span>
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


                        <!-- // EDI轉入失敗記錄 // -->
                        <div id="edilog" class="card grey scrollspy">
                            <div class="card-content white-text">
                                <a href="<%=Page_SearchUrl %>" class="white-text">
                                    <h5><i class="material-icons left">arrow_back</i>EDI轉入記錄 <small>(停留在轉入排程中的品項)</small></h5>
                                </a>
                            </div>
                            <div class="card-content grey lighten-5">
                                <asp:ListView ID="lv_EdiLog" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <table class="bordered striped responsive-table">
                                            <thead>
                                                <tr>
                                                    <th class="center-align">原始單號</th>
                                                    <th>品號</th>
                                                    <th>原因</th>
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
                                            <td>
                                                <%#Eval("ModelNo") %>
                                            </td>
                                            <td>
                                                <%#Eval("Why") %>
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
                            </div>
                        </div>

                        <!-- // ERP 訂單/銷貨單 // -->
                        <div id="erpData" class="card grey scrollspy">
                            <div class="card-content white-text">
                                <a href="<%=Page_SearchUrl %>" class="white-text">
                                    <h5><i class="material-icons left">arrow_back</i>ERP 訂單/銷貨單</h5>
                                </a>
                            </div>
                            <div class="card-content grey lighten-5">
                                <asp:ListView ID="lv_ErpData" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <table class="bordered striped responsive-table">
                                            <thead>
                                                <tr>
                                                    <th class="white-text grey center-align">原始單號</th>
                                                    <th class="white-text blue lighten-1 center-align">訂單資訊</th>
                                                    <th class="white-text blue lighten-1">品號/品名</th>
                                                    <th class="white-text blue lighten-1">庫別</th>
                                                    <th class="white-text blue lighten-1">數量</th>
                                                    <th class="white-text red lighten-1 center-align">銷貨單資訊</th>
                                                    <th class="white-text red lighten-1">品號/品名</th>
                                                    <th class="white-text red lighten-1">庫別</th>
                                                    <th class="white-text red lighten-1">數量</th>
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
                                                <%#Eval("OrderID") %>
                                            </td>
                                            <td class="blue-text text-darken-1">
                                                <%#Eval("TC001") %>-<%#Eval("TC002") %>
                                            </td>
                                            <td>
                                                <%#Eval("TD004") %><br />
                                                <%#Eval("TD005") %>
                                            </td>
                                            <td>
                                                <%#Eval("TD007") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("TD008") %>
                                            </td>
                                            <td class="red-text text-darken-1">
                                                <%#Eval("TH001") %>-<%#Eval("TH002") %>
                                            </td>
                                            <td>
                                                <%#Eval("TH004") %><br />
                                                <%#Eval("TH005") %>
                                            </td>
                                            <td>
                                                <%#Eval("TH007") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("TH008") %>
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

                        <!-- // ERP 借出單 // -->
                        <div id="erpInvData" class="card grey scrollspy">
                            <div class="card-content white-text">
                                <a href="<%=Page_SearchUrl %>" class="white-text">
                                    <h5><i class="material-icons left">arrow_back</i>ERP 借出單</h5>
                                </a>
                            </div>
                            <div class="card-content grey lighten-5">
                                <asp:ListView ID="lv_ErpInvData" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <table class="bordered striped responsive-table">
                                            <thead>
                                                <tr>
                                                    <th class="center-align">原始單號</th>
                                                    <th class="center-align">訂單號</th>
                                                    <th>品號/品名</th>
                                                    <th class="center-align">數量</th>
                                                    <th>轉出庫別</th>
                                                    <th>轉入庫別</th>
                                                    <th class="center-align">暫出入單號</th>
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
                                                <%#Eval("TC012") %>
                                            </td>
                                            <td class="blue-text text-darken-1 center-align">
                                                <%#Eval("TG014") %>-<%#Eval("TG015") %>
                                            </td>
                                            <td>
                                                <%#Eval("TG004") %><br />
                                                <%#Eval("TG005") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("TG009") %>
                                            </td>
                                            <td>
                                                <%#Eval("TG007") %>
                                            </td>
                                            <td>
                                                <%#Eval("TG008") %>
                                            </td>
                                            <td class="green-text text-darken-1 center-align">
                                                <%#Eval("TG001") %>-<%#Eval("TG002") %>
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

                        <!-- // ERP 銷退單 // -->
                        <div id="erpRbData" class="card grey scrollspy">
                            <div class="card-content white-text">
                                <a href="<%=Page_SearchUrl %>" class="white-text">
                                    <h5><i class="material-icons left">arrow_back</i>ERP 銷退單</h5>
                                </a>
                            </div>
                            <div class="card-content grey lighten-5">
                                <asp:ListView ID="lv_ErpRbData" runat="server" ItemPlaceholderID="ph_Items">
                                    <LayoutTemplate>
                                        <table class="bordered striped responsive-table">
                                            <thead>
                                                <tr>
                                                    <th class="center-align">單別/單號</th>
                                                    <th>品號/品名</th>
                                                    <th class="center-align">數量</th>
                                                    <th>單頭備註</th>
                                                    <th>單身備註</th>
                                                    <th>銷退原因代號</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="green-text text-darken-1 center-align">
                                                <%#Eval("TI001") %>-<%#Eval("TI002") %>
                                            </td>
                                            <td>
                                                <%#Eval("TJ004") %><br />
                                                <%#Eval("TJ005") %>
                                            </td>
                                            <td class="center-align">
                                                <%#Eval("TJ007") %>
                                            </td>
                                            <td>
                                                <%#Eval("TI020") %>
                                            </td>
                                            <td>
                                                <%#Eval("TJ023") %>
                                            </td>
                                            <td>
                                                <%#Eval("TJ052") %>
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
                                <li><a href="#fileinfo">檔案資訊</a></li>
                                <li><a href="#log">匯入失敗記錄</a></li>
                                <li><a href="#edilog">EDI轉入記錄</a></li>
                                <li><a href="#erpData">ERP 訂單/銷貨單</a></li>
                                <li><a href="#erpInvData">ERP 借出單</a></li>
                                <li><a href="#erpRbData">ERP 銷退單</a></li>
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

