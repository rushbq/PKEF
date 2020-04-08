<%@ Page Title="深圳-電子發票 | 匯入Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="ImportStep4.aspx.cs" Inherits="mySZInvoiceE_ImportStep4" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">完成匯入</h5>
                    <ol class="breadcrumb">
                        <li><a>深圳-電子發票</a></li>
                        <li><a>匯入Excel</a></li>
                        <li class="active">完成匯入</li>
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
                <p>匯入已完成, 但狀態更新失敗, 請回報錯誤發生的 <strong class="flow-text">詳細狀況</strong>。</p>
                <p><a class="btn waves-effect waves-light grey darken-1" href="<%=Application["WebUrl"] %>mySZInvoice_E/ImportLog.aspx?dataID=<%=Req_DataID %>#log">或點此查看記錄</a></p>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Content" runat="server">
            <div class="card-panel green darken-1 white-text">
                <h4><i class="material-icons right">error_outline</i>匯入成功，資料已更新至ERP。</h4>
            </div>
            <div class="card-panel">
                <div class="row">
                    <div class="col s12">
                        <h4>恭喜你已完成匯入，接下來你可以...</h4>
                    </div>
                </div>
                <div class="section row">
                    <div class="col s12 right-align">
                        <a class="btn-large waves-effect waves-light green" href="<%=Application["WebUrl"] %>mySZInvoice_E/ImportStep1.aspx">開始新的匯入<i class="material-icons left">autorenew</i></a>
                        <a class="btn-large waves-effect waves-light blue" href="<%=Application["WebUrl"] %>mySZInvoice_E/ImportLog.aspx?dataID=<%=Req_DataID %>">查看此次記錄<i class="material-icons left">access_time</i></a>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myBottom" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

