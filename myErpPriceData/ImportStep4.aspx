<%@ Page Title="報價單匯入 | 完成轉入" Language="C#" MasterPageFile="~/Site_S_UI.master" AutoEventWireup="true" CodeFile="ImportStep4.aspx.cs" Inherits="myErpPriceData_ImportStep4" %>

<%@ Register Src="Ascx_StepMenu.ascx" TagName="Ascx_Menu" TagPrefix="ucMenu" %>
<%@ Import Namespace="PKLib_Method.Methods" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- 工具列 Start -->
    <div class="myContentHeader">
        <div class="ui small menu toolbar">
            <div class="item">
                <div class="ui small breadcrumb">
                    <div class="section">業務行銷</div>
                    <i class="right angle icon divider"></i>
                    <div class="section">報價單匯入</div>
                    <i class="right angle icon divider"></i>
                    <h5 class="active section">Step4.完成&nbsp;&nbsp;(<span class="red-text text-darken-2"><%=Req_DBS %></span>)
                    </h5>
                </div>
            </div>
        </div>
    </div>
    <!-- 工具列 End -->

    <!-- 內容 Start -->
    <div class="myContentBody">
        <div class="ui attached segment grey-bg lighten-5">
            <asp:PlaceHolder ID="ph_ErrMessage" runat="server" Visible="false">
                <div class="ui negative message">
                    <div class="header">
                        Oops...發生了一點小問題
                    </div>
                    <asp:Literal ID="lt_ShowMsg" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
            <!-- Steps menu -->
            <ucMenu:Ascx_Menu ID="Ascx_Menu1" runat="server" nowIndex="4" />
            <!-- 資料 Start -->
            <div class="ui small form attached green segment">
                <div class="two fields">
                    <div class="field">
                        <label>追蹤碼</label>
                        <asp:Label ID="lb_TraceID" runat="server" CssClass="ui red basic large label"></asp:Label>
                    </div>
                    <div class="field">
                        <label>資料庫</label>
                        <asp:Label ID="lb_DBS" runat="server" CssClass="ui blue basic large label"></asp:Label>
                    </div>
                </div>
                <div class="two fields">
                    <div class="field">
                        <label>客戶</label>
                        <asp:Label ID="lb_Cust" runat="server" CssClass="ui green basic large label"></asp:Label>
                    </div>
                    <div class="field">
                        <label>ERP單別</label>
                        <asp:Label ID="lb_OrderType" runat="server" CssClass="ui basic large label"></asp:Label>
                    </div>
                </div>
                <div class="two fields">
                    <div class="field">
                        <label>生效日</label>
                        <asp:Label ID="lb_validDate" runat="server" CssClass="ui basic large label"></asp:Label>
                    </div>
                    <div class="field">
                        <label>單號</label>
                        <asp:Label ID="lb_OrderNo" runat="server" CssClass="ui basic large label"></asp:Label>
                    </div>
                </div>
                <div class="field">
                    <div class="ui placeholder segment">
                        <div class="ui icon header">
                            <i class="thumbs up icon"></i>
                            資料已轉入EDI排程，接下來你可以...
                        </div>
                        <div class="inline">
                            <a class="ui button" href="<%=Page_SearchUrl %>">回列表頁</a>
                            <a class="ui blue button" href="<%=FuncPath() %>/ImportStep1.aspx?dbs=<%=Req_DBS %>">新增匯入</a>
                        </div>
                    </div>
                </div>
            </div>
            <!-- 資料 End -->
        </div>
    </div>
    <!-- 內容 End -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomContent" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

